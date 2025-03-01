﻿using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace Planning
{
    [AddComponentMenu("Planning/Planner")]
    public class Planner : MonoBehaviour
    {
        // Should world/goal be a component that we point to?
        public StateList world;
        public StateList goal;

        [Tooltip("How many options do we want to consider per frame. Set to 0 or negative to do all planning in one frame.")]
        public int ticksPerFrame = 0;
        [Tooltip("Turning this off may give performance boosts when there are less possible paths")]
        public bool ignoreRedundantPaths = true;

        [Tooltip("Do we want to use actions in the scene marked as broadcast?")]
        public bool useBroadcastActions = true;

        private Action[] possibleActions;

        private bool currentlyPlanning;

        private PlanTree tree;
        private List<PlanTree.Node> visited; // NOTE: this might not be needed

        public Queue<Action> plan;

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
            tree = new PlanTree(world);
            // Store visited nodes in an unsorted list
            visited = new List<PlanTree.Node>();
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
            while (nodesLookedAt < MAX_NODES && !tree.IsEmpty())
            {
                // Get the next unexplored node
                PlanTree.Node leaf = tree.PopCheapestLeaf();
                nodesLookedAt++;
                // Give the action a chance to update anything it needs
                // Did we reach our goal?
                if (leaf.state.Matches(goal))
                {
                    plan = tree.GetPlan(leaf);
                    Debug.Log("Found plan of " + plan.Count +
                        " actions after looking at " + nodesLookedAt + " nodes!");
                    currentlyPlanning = false;
                    yield break;
                }
                // See if we can do any possible actions on this tree
                foreach (Action act in possibleActions)
                {
                    bool validAction = act.CheckPreconditions(leaf.state, goal);
                    if (validAction)
                    {
                        tree.AddAction(leaf, act);
                    }
                }
                // Wait for end of frame if that's what you're into
                if (ticksPerFrame > 0 && (nodesLookedAt % ticksPerFrame == 0))
                {
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

        // Checks the crrent planning tree to see if this is a world we've already considered
        private bool IsUniqueOutcome(StateList world)
        {
            // Check each visited node's world state
            foreach (PlanTree.Node n in visited)
            {
                // Matches is asymetric, so we check twice for real equivalency
                if (world.Matches(n.state) && n.state.Matches(world))
                    return false;
            }
            return true;
        }
    };
}
