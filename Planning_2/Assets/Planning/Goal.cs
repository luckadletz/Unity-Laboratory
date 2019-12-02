/* Luc Kadletz - 1/1/2019 */

using System;

namespace Planning
{
    public interface IGoal
    {
        bool MeetsGoal(World possible);

        // TODO add a float for minimization style goals?
    }

    [Serializable]
    public class ExpectedWorldGoal : IGoal
    {
        public World Expected;

        public ExpectedWorldGoal()
        {
            Expected = new World();
        }

        public ExpectedWorldGoal(World expected)
        {
            if(expected == null) throw new ArgumentNullException();
        }

        public bool MeetsGoal(World possible)
        {
            return possible.Matches(Expected);
        }
    }
}