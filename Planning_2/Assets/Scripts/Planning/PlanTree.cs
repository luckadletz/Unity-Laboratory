/* Luc Kadletz - 12/23/2018 */

using System;
using System.Collections.Generic;

namespace Planning
{
	public class PlanTree
	{

		private Node Root;
		private List<Node> OpenLeaves;

		public PlanTree(World start)
		{
			OpenLeaves = new List<Node>();
			Root = new Node(start);
			OpenLeaves.Add(Root);
		}

		public bool IsEmpty()
		{
			return OpenLeaves.Count == 0;
		}

		public Node PopCheapestLeaf()
		{
			Node cheapest = OpenLeaves[0];
			foreach(Node n in OpenLeaves)
			{
				if(n.TotalCost < cheapest.TotalCost)
				{
					cheapest = n;
				}
			}
			OpenLeaves.Remove(cheapest);
			return cheapest;
		}

		public Plan GetPlan(Node end)
		{
			Stack<Step> history = new Stack<Step>();
			while(end.Step != null)
			{
				history.Push(end.Step);
				end = end.Parent;
			}
			return new Plan(history.ToArray());
		}

		public void AddStep(Node previous, Step step)
		{
			Node leaf = new Node(previous, step);
			OpenLeaves.Add(leaf);
		}

		public class Node
		{
			public readonly Node Parent;
			public IList<Node> Children { get; private set; }
			public readonly Step Step;
			public readonly float TotalCost;

			public Node(Node parent, Step step)
			{
				if (parent == null) throw new ArgumentNullException();
				if (step == null) throw new ArgumentNullException();

				Parent = parent;
				Parent.Children.Add(this);

				Children = new List<Node>();

				Step = step;

				TotalCost = Parent.TotalCost + Step.Cost;
			}

			public Node(World start)
			{
				if (start == null) throw new ArgumentNullException();

				Parent = null;

				Children = new List<Node>();

				Step = null;

				TotalCost = 0.0f;
			}
		}
	}
}