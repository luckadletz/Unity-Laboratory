using UnityEngine;
using NUnit.Framework;
using Planning;
using System.Collections.Generic; // List<>

[TestFixture]
public class StateListTests
{
    [Test]
    public void StateMatchingPerfect()
    {
        // Tests a perfect match
        // Populate our world
        StateList world = new StateList();
        world.SetState("health", 10.0f);
        world.SetState("ammo", 40.0f);
        world.SetState("locationX", 15.0f);
        world.SetState("locationY", -35.0f);
        // Set our goal
        StateList goal = new StateList();
        goal.SetState("health", 10.0f);
        goal.SetState("ammo", 40.0f);
        goal.SetState("locationX", 15.0f);
        goal.SetState("locationY", -35.0f);
        // They should be perfectly equal
        Assert.True(world.Matches(goal));
        Assert.True(goal.Matches(world));
    }

    [Test]
    public void StateMatchingOutOfOrder()
    {
        // Test a match with elements added out of order

        // Populate our world
        StateList world = new StateList();
        world.SetState("health", 10.0f);
        world.SetState("ammo", 40.0f);
        world.SetState("locationX", 15.0f);
        world.SetState("locationY", -35.0f);
        // Set our goal
        StateList goal = new StateList();
        goal.SetState("locationY", -35.0f);
        goal.SetState("ammo", 40.0f);
        goal.SetState("locationX", 15.0f);
        goal.SetState("health", 10.0f);
        // They should still match
        Assert.True(world.Matches(goal));
        Assert.True(goal.Matches(world));
    }

    [Test]
    public void StateMatchingPartial()
    {
        // Test a match of a goal that doesn't care about some parts of our world
        // Populate our world
        StateList world = new StateList();
        world.SetState("health", 10.0f);
        world.SetState("ammo", 40.0f);
        world.SetState("locationX", 15.0f);
        world.SetState("locationY", -35.0f);
        // our goal only cares about location
        StateList goal = new StateList();
        goal.SetState("locationX", 15.0f);
        goal.SetState("locationY", -35.0f);
        // This is no longer bidirectional
        Assert.True(world.Matches(goal));
    }

    [Test]
    public void StateMatchingMissingInfo()
    {
        // Test a match failure of a world that doesn't have info our goal needs
        // Populate our world - no ammo
        StateList world = new StateList();
        world.SetState("health", 10.0f);
        world.SetState("locationX", 15.0f);
        world.SetState("locationY", -35.0f);
        // our goal only cares about location and ammo
        StateList goal = new StateList();
        goal.SetState("ammo", 40.0f);
        goal.SetState("locationX", 15.0f);
        goal.SetState("locationY", -35.0f);
        goal.SaveCache();
        // We can't match info we don't have
        Assert.False(world.Matches(goal));
    }


}

[TestFixture]
public class PlanningTests
{

    [Test]
    public void TestPlannerNoActions()
    {
        // Test if we can plan successfuly when our start state and our world state are the same
        Planner p = new GameObject().AddComponent<Planner>();
        p.useBroadcastActions = false; // We don't want to pick up any actions from the scene

        p.world = new StateList();
        p.world.SetState("Alpha", 2.0f);
        p.world.SetState("Beta", 3.0f);
        p.world.SetState("Gamma", 4.0f);

        p.goal = new StateList();
        p.goal.SetState("Alpha", 2.0f);
        p.goal.SetState("Beta", 3.0f);
        p.goal.SetState("Gamma", 4.0f);

        p.DoPlanning();
        Queue<Action> plan = p.plan;

        Assert.True(plan.Count == 0.0f);
    }

    [Test]
    public void TestPlanner1Action1Path()
    {
        // Test if we can plan successfuly when our start state and our world state are one step apart
        // And we have exactly one action that is the one we need
        GameObject g = new GameObject();
        Planner p = g.AddComponent<Planner>();
        p.useBroadcastActions = false; // We don't want to pick up any actions from the scene

        p.world = new StateList();
        p.world.SetState("Gamma", 4.0f);

        p.goal = new StateList();
        p.goal.SetState("Gamma", 6.0f);

        SimpleAction[] temp = new SimpleAction[1];
        temp[0] = g.AddComponent<SimpleAction>();
        temp[0].preconditions.SetState("Gamma", 4.0f);
        temp[0].postconditions.SetState("Gamma", 6.0f);

        // p.possibleActions = temp;
        p.DoPlanning();
        Queue<Action> plan = p.plan;
        Assert.AreEqual(1, plan.Count);
    }

    [Test]
    public void TestPlanner3Actions2Paths()
    {
        // Test if we can plan successfuly when our start state and our world state are one step apart
        // And we have three actions and two possible ways of getting to our goal
        GameObject g = new GameObject();
        Planner p = g.AddComponent<Planner>();
        p.useBroadcastActions = false; // We don't want to pick up any actions from the scene

        p.world = new StateList();
        p.world.SetState("Alarm", 4.0f);

        p.goal = new StateList();
        p.goal.SetState("Alarm", 6.0f);

        SimpleAction[] temp = new SimpleAction[3];
        temp[0] = g.AddComponent< SimpleAction>();
        temp[0].preconditions.SetState("Alarm", 4.0f);
        temp[0].postconditions.SetState("Alarm", 1.0f);

        temp[1] = g.AddComponent<SimpleAction>();
        temp[1].preconditions.SetState("Alarm", 1.0f);
        temp[1].postconditions.SetState("Alarm", 6.0f);

        temp[2] = g.AddComponent<SimpleAction>();
        temp[2].preconditions.SetState("Alarm", 4.0f);
        temp[2].postconditions.SetState("Alarm", 6.0f);

        // p.possibleActions = temp;

        p.DoPlanning();
        Queue<Action> plan = p.plan;
        Assert.AreEqual(1, plan.Count);
    }

    [Test]
    public void TestPlannerEndlessLoop()
    {
        // Test if we can plan successfuly when we can't actually form a path
        // But each step looks like it could get us a bit closer
        GameObject g = new GameObject();
        Planner p = g.AddComponent<Planner>();
        p.useBroadcastActions = false; // We don't want to pick up any actions from the scene

        p.world = new StateList();
        p.world.SetState("Fire", 0.0f);
        p.world.SetState("Blood", 5.0f);

        p.goal = new StateList();
        p.goal.SetState("Fire", 5.0f);
        p.goal.SetState("Blood", 5.0f);

        // Fire -> Blood
        SimpleAction[] temp = new SimpleAction[2];
        temp[0] = g.AddComponent<SimpleAction>();
        temp[0].preconditions.SetState("Fire", 5.0f);
        temp[0].postconditions.SetState("Fire", 0.0f);
        temp[0].postconditions.SetState("Blood", 5.0f);

        // Blood -> Fire
        temp[1] = g.AddComponent<SimpleAction>();
        temp[1].preconditions.SetState("Blood", 5.0f);
        temp[1].postconditions.SetState("Fire", 5.0f);
        temp[1].postconditions.SetState("Blood", 0.0f);

        // We should give up and return no path
        p.DoPlanning();
        Queue<Action> plan = p.plan;
        Assert.AreEqual(0, plan.Count);
    }

    [Test]
    public void TestPlanner2Actions1Path()
    {
        // Test if we can plan successfuly when our start state and our world state are two steps apart
        // And we have two actions and one possible way of getting to our goal
        GameObject g = new GameObject();
        Planner p = g.AddComponent<Planner>();
        p.useBroadcastActions = false; // We don't want to pick up any actions from the scene

        p.world = new StateList();
        p.world.SetState("Score", 0.0f);

        p.goal = new StateList();
        p.goal.SetState("Score", 2.0f);

        // Each action gets us a little bit closer to the goal score
        SimpleAction[] temp = new SimpleAction[2];
        for (int i = 0; i < 2; ++i)
        {
            SimpleAction s = g.AddComponent<SimpleAction>();
            s.preconditions.SetState("Score", i);
            s.postconditions.SetState("Score", i + 1);
            temp[i] = s;
        }

        // We should give up and return no path
        p.DoPlanning();
        Queue<Action> plan = p.plan;
        Assert.AreEqual(2 , plan.Count);
    }

    [Test]
    public void TestPlanner10Actions1Path()
    {
        // Test if we can plan successfuly when our start state and our world state are two steps apart
        // And we have two actions and one possible way of getting to our goal
        GameObject g = new GameObject();
        Planner p = g.AddComponent<Planner>();
        p.useBroadcastActions = false;

        p.world = new StateList();
        p.world.SetState("Score", 1.0f);

        p.goal = new StateList();
        p.goal.SetState("Score", 11.0f);

        // Each action gets us a little bit closer to the goal score
        //p.possibleActions = new SimpleAction[10];
        for (int i = 0; i < 10; ++i)
        {
            SimpleAction act =  g.AddComponent<SimpleAction>();
            act.preconditions.SetState("Score", i + 1);
            act.postconditions.SetState("Score", i + 2);
        }

        // We should give up and return no path
        p.DoPlanning();
        Queue<Action> plan = p.plan;
        Assert.AreEqual(10, plan.Count);
    }

    [Test]
    public void TestPlannerRGB()
    {
        // Flip three levers to the on position when you can only some ways
        GameObject g = new GameObject();
        Planner p = g.AddComponent<Planner>();
        p.useBroadcastActions = false;

        p.world = new StateList();
        p.world.SetState("Red", 0.0f);
        p.world.SetState("Green", 0.0f);
        p.world.SetState("Blue", 0.0f);

        p.goal = new StateList();
        p.goal.SetState("Red", 1.0f);
        p.goal.SetState("Green", 1.0f);
        p.goal.SetState("Blue", 1.0f);

        SimpleAction[] temp = new SimpleAction[12];
        for (int i = 0; i < 12; ++i)
            temp[i] = g.AddComponent<SimpleAction>();
        // Each action can flip two at a time
        #region actions
        // Turn on red and green
        temp[0].preconditions.SetState("Red", 0.0f);
        temp[0].preconditions.SetState("Green", 0.0f);
        temp[0].postconditions.SetState("Red", 1.0f);
        temp[0].postconditions.SetState("Green", 1.0f);
        // Turn off red and green
        temp[1].preconditions.SetState("Red", 1.0f);
        temp[1].preconditions.SetState("Green", 1.0f);
        temp[1].postconditions.SetState("Red", 0.0f);
        temp[1].postconditions.SetState("Green", 0.0f);
        // Turn on red and off green
        temp[2].preconditions.SetState("Red", 0.0f);
        temp[2].preconditions.SetState("Green", 1.0f);
        temp[2].postconditions.SetState("Red", 1.0f);
        temp[2].postconditions.SetState("Green", 0.0f);
        // Turn off red and on green
        temp[3].preconditions.SetState("Red", 1.0f);
        temp[3].preconditions.SetState("Green", 0.0f);
        temp[3].postconditions.SetState("Red", 0.0f);
        temp[3].postconditions.SetState("Green", 1.0f);
        // Turn on red and blue
        temp[4].preconditions.SetState("Red", 0.0f);
        temp[4].preconditions.SetState("Blue", 0.0f);
        temp[4].postconditions.SetState("Red", 1.0f);
        temp[4].postconditions.SetState("Blue", 1.0f);
        // Turn off red and Blue
        temp[5].preconditions.SetState("Red", 1.0f);
        temp[5].preconditions.SetState("Blue", 1.0f);
        temp[5].postconditions.SetState("Red", 0.0f);
        temp[5].postconditions.SetState("Blue", 0.0f);
        // Turn on red and off Blue
        temp[6].preconditions.SetState("Red", 0.0f);
        temp[6].preconditions.SetState("Blue", 1.0f);
        temp[6].postconditions.SetState("Red", 1.0f);
        temp[6].postconditions.SetState("Blue", 0.0f);
        // Turn off red and on Blue
        temp[7].preconditions.SetState("Red", 1.0f);
        temp[7].preconditions.SetState("Blue", 0.0f);
        temp[7].postconditions.SetState("Red", 0.0f);
        temp[7].postconditions.SetState("Blue", 1.0f);
        // Turn on Green and blue
        temp[8].preconditions.SetState("Green", 0.0f);
        temp[8].preconditions.SetState("Blue", 0.0f);
        temp[8].postconditions.SetState("Green", 1.0f);
        temp[8].postconditions.SetState("Blue", 1.0f);
        // Turn off Green and Blue
        temp[9].preconditions.SetState("Green", 1.0f);
        temp[9].preconditions.SetState("Blue", 1.0f);
        temp[9].postconditions.SetState("Green", 0.0f);
        temp[9].postconditions.SetState("Blue", 0.0f);
        // Turn on only Blue
        temp[10].preconditions.SetState("Blue", 1.0f);
        temp[10].postconditions.SetState("Blue", 0.0f);
        // Turn off only Green
        temp[11].preconditions.SetState("Green", 1.0f);
        temp[11].postconditions.SetState("Green", 0.0f);
        #endregion

        p.DoPlanning();
        Queue<Action> plan = p.plan;
        Assert.AreEqual(3, plan.Count);
    }
}