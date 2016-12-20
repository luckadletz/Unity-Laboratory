/**
    Author: Luc Kadletz
    Date: 12/20/2016

    This is a tree of "considered" actions.

*/
using System.Collections.Generic;

namespace Planning
{
    public class PlanTree
    {
        // The starting point of the plan, containing no action and the state of the world
        private Node root;
        // A list of all nodes that have not been checked
        private List<Node> openLeaves;
        
        // Constructs an empty PlanTree for a given world state
        public PlanTree(StateList world)
        {
            openLeaves = new List<Node>();
            root = new Node(null, null, world, 0.0f);
            openLeaves.Add(root);
        }

        public bool IsEmpty()
        {
            return openLeaves.Count == 0;
        }

        // Gets an open leaf node with the lowest cost
        public Node PopCheapestLeaf()
        {
            // Loop through and find the cheapest
            Node cheapest = openLeaves[0];
            foreach(Node n in openLeaves)
            {
                if (n.cost < cheapest.cost)
                    cheapest = n;
            }
            // Remove it from the leaves list
            openLeaves.Remove(cheapest);
            return cheapest;
        }

        // Gets the sequence of actions that gets from the root to n
        public Queue<Action> GetPlan(Node n)
        {
            // Walk backwards until we get to the root
            Stack<Action> history = new Stack<Action>();
            while(n.action != null)
            {
                history.Push(n.action);
                n = n.parent;
            }
            // Reverse the stack
            Queue<Action> q = new Queue<Action>();
            while (history.Count > 0)
                q.Enqueue(history.Pop());
            return q;
        }

        public void AddAction(Node previous, Action a)
        {
            // Calculate the world state after this action
            StateList nextState = a.Simulate(previous.state);
            // Calculate the total cost of the plan including this action
            float totalCost = previous.cost + a.cost;
            // Create the leaf node (which hooks up parent/child ref
            Node leaf = new Node(previous, a, nextState, totalCost);
            openLeaves.Add(leaf);
        }
        
        public class Node
        {
            // The action before this one
            public Node parent { get; private set; }
            // Actions being concidered after this one
            public List<Node> children { get; private set; }
            // The action at this step in the plan
            public Action action;
            // The expected state of the world after doing this plan
            public StateList state;
            // How expensive is this branch of the plan relative to others? (cumulative)
            public float cost;

            public Node(Node parent, Action action, StateList state, float cost)
            {
                this.action = action;
                this.state = state;
                this.cost = cost;
                this.children = new List<Node>();
                // Hook up parent/child ref
                if(parent != null)
                {
                    this.parent = parent;
                    parent.children.Add(this);
                }
                
            }
        }
    }
}
