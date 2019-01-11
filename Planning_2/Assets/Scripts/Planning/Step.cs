/* Luc Kadletz - 12/12/2018 */

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Planning
{
	public abstract class Step
	{
		public float Cost { get; protected set; }

		public World Expected { get; protected set; }

		// Returns true when finished, false to run again next frame
		public abstract bool Action();
	}

	public class MessageStep : Step
	{
		protected GameObject Target;
		protected string Message;

		// The bool returned after the message is sent
		public bool CompletedAfterCall = true;

		public MessageStep(GameObject target, 
			string message, 
			float cost, 
			World expected)
		{
			Target = target;
			Expected = expected;
			Message = message;
			Cost = cost;
		}

		public override bool Action()
		{
			Target.SendMessage(Message);
			return CompletedAfterCall;
		}
	}

}
