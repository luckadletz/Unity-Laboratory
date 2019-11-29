using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace Planning
{
    [AddComponentMenu("Planning/Planner")]
    public class Planner : MonoBehaviour
    {
        public bool useCoroutines = false;
        public bool useDebugDraw;
        [Tooltip("Turning this off may give performance boosts when there are less possible paths")]
        public bool ignoreRedundantPaths = true;
        public int coroutineNodesPerFrame = 8;

        public StateList world;
        public StateList goal;
        public Action[] possibleActions;

        public bool useBroadcastActions = true;
        private Node planTree;
        private List<Node> visited;
        private List<Node> leaves;
        public bool currentlyPlanning { get; private set; }

        public Queue<Action> plan;

        private void RefreshPossibleActions()
        {
            List<Action> actions = new List<Action>();
            // Get All broadcast actions
            if (useBroadcastActions)
            {
                actions.AddRange(FindObjectsOfType<Action>());
                actions.RemoveAll(x => x.broadcast == false);
            }

            // Get all components on us and our children
            actions.AddRange(GetComponentsInChildren<Action>());

            possibleActions = actions.ToArray();
        }

        private Node PopCheapestLeaf()
        {
            // Loop through and find the cheapest
            Node cheapest = leaves[0];
            foreach (Node n in leaves)
            {
                if (n.cost < cheapest.cost)
                    cheapest = n;
            }
            // Remove it from the leaves list
            leaves.Remove(cheapest);
            // And then add it to visited
            visited.Add(cheapest);
            return cheapest;
        }

        private void AddLeaf(Node leaf)
        {
            // See if the leaf has a unique outcome
            // We don't want to consider actions that get us a world state we've seen already
            if (!ignoreRedundantPaths || IsUniqueOutcome(leaf.state))
            {
                // Note - we might want to see if this leaf is cheaper than the other route
                leaves.Add(leaf);
            }
        }

        // Checks the crrent planning tree to see if this is a world we've already considered
        private bool IsUniqueOutcome(StateList world)
        {
            // Check each visited node's world state
            foreach (Node n in visited)
            {
                // Matches is asymetric, so we check twice for real equivalency
                if (world.Matches(n.state) && n.state.Matches(world))
                    return false;
            }
            return true;
        }

        public void DoPlanning()
        {
            // If we're currently planning, stop it.
            if(currentlyPlanning)
            {
                Debug.Log("Canceling current plan...");
                StopCoroutine("DoPlanningHelper");
                currentlyPlanning = false;
            }
            Debug.Log("Starting Planning . . .");
            // Update the things we can do
            RefreshPossibleActions();
            // Reset our planning tree
            planTree = new Node(null, world, null);
            // Store all unexplored nodes in an unsorted list
            leaves = new List<Node>();
            // Store visited nodes in an unsorted list
            visited = new List<Node>();
            // Start with the current world as our starting node
            leaves.Add(planTree);
            // Start a coroutine that looks at nodes every frame
            StartCoroutine(DoPlanningHelper());
        }

        private IEnumerator DoPlanningHelper()
        {
            currentlyPlanning = true;
            // The amount of nodes we'll look at before we give up
            const int MAX_NODES = 2048;
            // The ammount of nodes we look at each frame
            int nodesLookedAt = 0;
            // Look at nodes until we find a path or give up
            while (nodesLookedAt < MAX_NODES && leaves.Count != 0)
            {
                // Get the next unexplored node
                Node leaf = PopCheapestLeaf();
                nodesLookedAt++;
                // Give the action a chance to update anything it needs
                // Did we reach our goal?
                if (leaf.state.Matches(goal))
                {
                    plan = leaf.GetHistory();
                    Debug.Log("Found plan of " + plan.Count +
                        " actions after looking at " + nodesLookedAt + " nodes!");
                    // Do the cool line thing
                    var p = GetComponentInChildren<debugDrawPlanning>();
                    if (p) p.DrawPlan(plan);

                    currentlyPlanning = false;
                    yield break;
                }
                // See if we can do any possible actions on this tree
                foreach (Action act in possibleActions)
                {
                    bool validAction = act.CheckPreconditions(leaf.state, goal);
                    if (validAction)
                    {
                        Debug.Log("discarding action " + act.gameObject.name);
                        Node result = leaf.AddChild(act, goal);
                        AddLeaf(result);
                    }
                    if (useDebugDraw)
                    {


                        Vector3 startPos = leaf.action ? leaf.action.transform.position : transform.position;
                        Debug.DrawLine(startPos, act.transform.position,
                            validAction ? Color.blue : (Color.red * 0.5f), validAction? 5.0f : 0.1f, false);
                    }
                }
                // Wait for end of frame if that's what you're into
                if (useCoroutines &&
                    (nodesLookedAt % coroutineNodesPerFrame == 0))
                {
                    // Do the cool line thing
                    var p = GetComponentInChildren<debugDrawPlanning>();
                    if (p) p.DrawTree(leaf);

                    yield return null;
                }
            }
            plan = new Queue<Action>();
            Debug.Log("Couldn't find a plan after looking at " + nodesLookedAt + " nodes.");
            currentlyPlanning = false;
            yield break;
        }

        public void ExecutePlan()
        {
            if(currentlyPlanning )
            {
                Debug.Log("Plan isn't finished yet!");
            }
            else
            {
                // Start a coroutine that will execute every frame until done
                StartCoroutine(ExecutePlanHelper());
            }
        }

        private IEnumerator ExecutePlanHelper()
        {
            while (plan.Count > 0)
            {
                bool finished = plan.Peek().DoAction(world, goal, gameObject);
                // If the action is done, take it off the queue
                if (finished)
                    plan.Dequeue();
                // Wait until next frame to procede
                yield return null;
            }
            yield break;
        }

        public class Node
        {
            // The most recent action that got us to this state
            public Action action;
            // The state of the world at this point
            public StateList state;
            // Other actions we're considering after this one
            public List<Node> children;
            // The parent of this node
            public Node parent = null;

            public float cost;

            public Node(Action act, StateList s, Node par, float c = 0.0f)
            {
                // Start with no children
                children = new List<Node>();
                // Remember what action created this node
                action = act;
                state = s;
                // Who's your daddy?
                parent = par;
                cost = c;
            }

            public Node AddChild(Action action, StateList goal)
            {
                // The state is our nodes current state with the action applied
                // Cost goes up by one
                Node child = new Node(action, action.Simulate(state, goal), this, cost + 1.0f);
                children.Add(this);
                return child;
            }

            // This gives you a list of all actions that got you to this node
            public Queue<Action> GetHistory()
            {
                Stack<Action> history = new Stack<Action>();
                Node n = this;
                while (n.action != null)
                {
                    history.Push(n.action);
                    n = n.parent;
                }
                // Reverse our stack and return
                Queue<Action> q = new Queue<Action>();
                while (history.Count > 0)
                    q.Enqueue(history.Pop());
                return q;
            }
        }

    };
}
