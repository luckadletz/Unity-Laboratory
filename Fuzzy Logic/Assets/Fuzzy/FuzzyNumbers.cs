/*
 * Author: Luc Kadletz
 * 3/11/2016
 * 
 */

using UnityEngine;
using System.Collections.Generic;
using System;

[System.Serializable]
public struct FuzzyOutput
{
    [Range(0, 1.0f)]
    float value;

    // Constructor
    public FuzzyOutput(float v)
    {
        Debug.Assert(v >= 0.0f, "Fuzzy Output " + v + " below zero!");
        Debug.Assert(v <= 1.0f, "Fuzzy Output " + v + " above one!");
        value = v;
    }

    // Implicit float conversions
    public static implicit operator float(FuzzyOutput f)
    {
        return f.value;
    }

    public static implicit operator FuzzyOutput(float f)
    {
        return new FuzzyOutput(f);
    }
}

public interface IFuzzy : ICloneable
{
    //// Leftmost value for which Membership(x) == 1
    //float core_left { get; }
    //// Rightmost value for which Membership(x) == 1
    //float core_right { get; }
    // Leftmost value for which Membership(x) != 0
    float close_left { get; }
    // Rightmost value for which Membership(x) != 0
    float close_right { get; }
    // Evaluates the membership of a number at the given value
    FuzzyOutput Membership(float input);

    AnimationCurve GetCurve();
    // NOTE I might want to add COG / COA here
    // NOTE Maybe get r+ and r- delegates might also be useful?
}

[System.Serializable]
public class TriangleFuzzyNumber : IFuzzy
{
    public float close_left, core, close_right;

    float IFuzzy.close_left
    { get { return close_left; } }

    float IFuzzy.close_right
    { get { return close_right; } }

    public TriangleFuzzyNumber(float a, float b, float c)
    {
        close_left = a;
        core = b;
        close_right = c;
    }

    public FuzzyOutput Membership(float input)
    {
        // Check outside number
        if (input <= close_left || input >= close_right)
            return 0;
        // Check left slope
        else if (input <= core)
            return (input - close_left) / (core - close_left);
        // Check right slope
        else // if (input > core)
            return (close_right - input) / (close_right - core);
    }

    public object Clone()
    {
        return new TriangleFuzzyNumber(close_left, core, close_right);
    }

    override public string ToString()
    {
        return "(" + close_left + ", "
            + core + ", "
            + close_right + ")";
    }

    public AnimationCurve GetCurve()
    {
        AnimationCurve curve = new AnimationCurve();

        // Compute slopes
        float l = 1.0f / (core - close_left);
        float r = -1.0f / (close_right - core);

        Keyframe left = new Keyframe(close_left, 0.0f);
        left.outTangent = l;
        curve.AddKey(left);

        Keyframe mid = new Keyframe(core, 1.0f);
        mid.inTangent = l;
        mid.outTangent = r;
        curve.AddKey(mid);

        Keyframe right = new Keyframe(close_right, 0.0f);
        right.inTangent = r;
        curve.AddKey(right);

        return curve;
    }
};

[System.Serializable]
public class TrapezoidFuzzyNumber : IFuzzy
{
    public float 
        close_left, 
        core_left, 
        core_right, 
        close_right;

    float IFuzzy.close_left
    { get { return close_left; } }

    float IFuzzy.close_right
    { get { return close_right; } }

    public TrapezoidFuzzyNumber(float a0, float a1, float b1, float b0)
    {
        close_left = a0;
        core_left = a1;
        core_right = b1;
        close_right = b0;
    }

    public object Clone()
    {
        return new TrapezoidFuzzyNumber(close_left, core_left, core_right, close_right);
    }

    override public string ToString()
    {
        return "(" + close_left + ", "
            + core_left + ", "
            + core_right + ", "
            + close_right + ")";
    }

    public FuzzyOutput Membership(float input)
    {
        // Check outside number
        if (input <= close_left || input >= close_right)
            return 0;
        // Check inside core
        else if (input >= core_left && input <= core_right)
            return 1;
        // Check left slope
        else if (input < core_left)
            return (input - close_left) / (core_left-close_left);
        // Check right slope
        else // if (input > core_right)
            return (close_right - input) / (close_right- core_right);
    }

    public AnimationCurve GetCurve()
    {
        AnimationCurve curve = new AnimationCurve();

        // Compute slopes
        float l = 1.0f / (core_left - close_left);
        float r = -1.0f / (close_right - core_right);
        
        Keyframe closeLeftKey = new Keyframe(close_left, 0.0f);
        closeLeftKey.inTangent = 0f;
        closeLeftKey.outTangent = l;
        curve.AddKey(closeLeftKey);

        Keyframe coreLeftKey = new Keyframe(core_left, 1.0f);
        coreLeftKey.inTangent = l;
        //coreLeftKey.outTangent = 0f;
        curve.AddKey(coreLeftKey);

        Keyframe coreRightKey = new Keyframe(core_right, 1.0f);
        //coreRightKey.inTangent = 0f;
        coreRightKey.outTangent = r;
        curve.AddKey(coreRightKey);

        Keyframe closeRightKey = new Keyframe(close_right, 0.0f);
        closeRightKey.inTangent = r;
        closeRightKey.outTangent = 0f;
        curve.AddKey(closeRightKey);

        return curve;
    }
};

// Declare delegate signature
[System.Serializable]
public class DelegateFuzzy : IFuzzy
{
    public float close_left, close_right;

    float IFuzzy.close_left
    { get { return close_left; } }

    float IFuzzy.close_right
    { get { return close_right; } }

    public delegate FuzzyOutput MembershipDelegate(float x);

    MembershipDelegate D;

    public DelegateFuzzy(MembershipDelegate d, float left, float right )
    {
        close_left = left;
        close_right = right;
        D = d;
    }

    public object Clone()
    {
        return new DelegateFuzzy(D, close_left, close_right);
    }

    public FuzzyOutput Membership(float input)
    {
        if (input < close_left || input > close_right)
            return 0.0f;
        return D(input);
    }

    public AnimationCurve GetCurve()
    {
        throw new NotImplementedException();
    }
}


static class Fuzzy
{
    static public IFuzzy Union(IFuzzy A, IFuzzy B)
    {
        // Copy A and B so that later changes don't modify this result
        IFuzzy copy_A = (IFuzzy) A.Clone();
        IFuzzy copy_B = (IFuzzy) B.Clone();

        // Compute the new bounds
        float left = Mathf.Min(A.close_left, B.close_left);
        float right = Mathf.Max(A.close_right, B.close_right);

        return new DelegateFuzzy((x) =>
            Mathf.Max(copy_A.Membership(x), copy_B.Membership(x)),
            left, right
        );
    }

    static public IFuzzy Intersection(IFuzzy A, IFuzzy B)
    {
        // Copy A and B so that later changes don't modify this result
        IFuzzy copy_A = (IFuzzy)A.Clone();
        IFuzzy copy_B = (IFuzzy)B.Clone();

        // Compute the new bounds
        float left = Mathf.Max(A.close_left, B.close_left);
        float right = Mathf.Min(A.close_right, B.close_right);

        return new DelegateFuzzy((x) =>
           Mathf.Min(copy_A.Membership(x), copy_B.Membership(x)),
           left, right
        );
    }

    static public IFuzzy Zero()
    {
        return new DelegateFuzzy((x) => 0.0f, 0.0f, 0.0f);
    }
    
    static public IFuzzy Universe()
    {
        return new DelegateFuzzy((x) => 1.0f, Mathf.NegativeInfinity, Mathf.Infinity);
    }

    static public IFuzzy Complement(IFuzzy A)
    {
        // Copy A so that later changes don't modify
        IFuzzy copy = (IFuzzy) A.Clone();

        // We lose our bounds :(
        return new DelegateFuzzy((x) => 
            1.0f - copy.Membership(x)
        , Mathf.NegativeInfinity, Mathf.Infinity);
    }

    // Lukasiewicz t-norm derived equivalence
    static public IFuzzy Equivalence(IFuzzy A, IFuzzy B)
    {
        // Take snapshots of A and B
        IFuzzy copy_A = (IFuzzy) A.Clone();
        IFuzzy copy_B = (IFuzzy) B.Clone();

        // We lose our bounds :(
        return new DelegateFuzzy((x) =>
            Mathf.Min( 
                1 - copy_A.Membership(x) + copy_B.Membership(x),
                1 - copy_B.Membership(x) + copy_A.Membership(x)),
                Mathf.NegativeInfinity, Mathf.Infinity);
    }

    // Lukasiewicz t-norm derived implication
    static public IFuzzy Implication(IFuzzy A, IFuzzy B)
    {
        // Take snapshots of A and B
        IFuzzy copy_A = (IFuzzy)A.Clone();
        IFuzzy copy_B = (IFuzzy)B.Clone();

        // We lose our bounds :(
        return new DelegateFuzzy((x) =>
            Mathf.Min(1 - copy_A.Membership(x) + copy_B.Membership(x), 1),
            Mathf.NegativeInfinity, Mathf.Infinity
        );
    }

    // COG
    public static float Defuzzify(IFuzzy A)
    {
        if (A.close_left == Mathf.NegativeInfinity ||
           A.close_right == Mathf.Infinity)
            Debug.LogWarning("Tried to defuzzify with infinite bounds!");

        // Defuzzy crisp is just the crisp
        if (A.close_left == A.close_right)
            return A.close_right;

        // The ammount of points sampled for a numerical integral approximation of the value
        const int resolution = 32;

        float interval = (A.close_right - A.close_left) / resolution;

        // Use trapezoidal rule integration approximation
        float moment = 0.0f;
        float sum = 0.0f;
        for (int i = 0; i < resolution; ++i)
        {
            // Left bound of integration
            float left = i * interval + A.close_left;
            // Right bound of integration
            float right = left + interval;

            float integral = interval * (A.Membership(left) + A.Membership(right)) * 0.5f;

            Debug.Assert(integral >= 0.0f);

            moment += (left + interval * 0.5f) * integral;
            sum += integral;
        }

        // If this is a flat integral, then give up and give them the midpoint
        if (sum <= 0.0f)
        {
            // Debug.LogWarning("Tried to defuzzy an empty number:" + sum);
            return (A.close_right + A.close_left) * 0.5f;
        }

        return moment / sum;
    }
}
