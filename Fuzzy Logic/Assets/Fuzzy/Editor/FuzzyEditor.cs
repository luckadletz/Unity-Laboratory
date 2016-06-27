/*
 * Author: Luc Kadletz
 * 3/11/2016
 * 
 */

using UnityEngine;
using System.Collections;
using UnityEditor;


[CustomPropertyDrawer(typeof(TriangleFuzzyNumber), true)]
public class TriangleFuzzyNumberPropertyDrawer : PropertyDrawer
{
    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        SerializedProperty left = property.FindPropertyRelative("close_left");
        SerializedProperty mid = property.FindPropertyRelative("core");
        SerializedProperty right = property.FindPropertyRelative("close_right");

        float thirdwidth = position.width / 3.0f;

        left.floatValue =  EditorGUI.FloatField(
            new Rect( position.x, position.y, thirdwidth, position.height),
            left.floatValue);
        mid.floatValue = EditorGUI.FloatField(
            new Rect( position.x + thirdwidth, position.y, thirdwidth, position.height),
            mid.floatValue);
        right.floatValue = EditorGUI.FloatField(
            new Rect( position.x + 2 * thirdwidth, position.y, thirdwidth, position.height
            ), right.floatValue);

        // clamp left and right
        left.floatValue = Mathf.Min(left.floatValue, mid.floatValue);
        right.floatValue = Mathf.Max(right.floatValue, mid.floatValue);
    }
}

[CustomPropertyDrawer(typeof(TrapezoidFuzzyNumber), true)]
public class TrapezoidFuzzyNumberPropertyDrawer : PropertyDrawer
{
    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        SerializedProperty close_left = property.FindPropertyRelative("close_left");
        SerializedProperty core_left = property.FindPropertyRelative("core_left");
        SerializedProperty core_right = property.FindPropertyRelative("core_right");
        SerializedProperty close_right = property.FindPropertyRelative("close_right");

        float quarterwidth = position.width / 4.0f;

        // TODO Clean up this __stanky__ code with a fancy rect that we move instead of new ones

        EditorGUI.BeginChangeCheck();
        close_left.floatValue = EditorGUI.FloatField(
            new Rect(position.x, position.y, quarterwidth, position.height),
            close_left.floatValue);
        if(EditorGUI.EndChangeCheck())
        {
            // Push core_left to be explicitly right of close_left if necessary
            core_left.floatValue = Mathf.Max(core_left.floatValue,
                close_left.floatValue + Mathf.Epsilon);
        }

        EditorGUI.BeginChangeCheck();
        core_left.floatValue = EditorGUI.FloatField(
            new Rect(position.x + quarterwidth, position.y, quarterwidth, position.height),
            core_left.floatValue);
        if (EditorGUI.EndChangeCheck())
        {
            // Push close_left to be explicitly left of core_left if necessary
            close_left.floatValue = Mathf.Min( close_left.floatValue,
                core_left.floatValue - Mathf.Epsilon);
            // Push core_right to be explicitly right of core_left if necessary
            core_right.floatValue = Mathf.Max(core_right.floatValue,
                core_left.floatValue + Mathf.Epsilon);
        }

        EditorGUI.BeginChangeCheck();
        core_right.floatValue = EditorGUI.FloatField(
            new Rect(position.x + 2 *quarterwidth, position.y, quarterwidth, position.height),
            core_right.floatValue);
        if (EditorGUI.EndChangeCheck())
        {
            // Push core_left to be explicitly left of core_right if necessary
            core_left.floatValue = Mathf.Min(core_left.floatValue,
                core_right.floatValue - Mathf.Epsilon);
            // Push close_right to be explicitly right of core_right if necessary
            close_right.floatValue = Mathf.Max(close_right.floatValue,
                core_right.floatValue + Mathf.Epsilon);
        }

        EditorGUI.BeginChangeCheck();
        close_right.floatValue = EditorGUI.FloatField(
            new Rect(position.x + 3 * quarterwidth, position.y, quarterwidth, position.height),
            close_right.floatValue);
        if (EditorGUI.EndChangeCheck())
        {
            // Push core_right to be explicitly left of core_right if necessary
            core_right.floatValue = Mathf.Min(core_right.floatValue,
                close_right.floatValue - Mathf.Epsilon);
        }
        // clamp left


        close_right.floatValue = Mathf.Max(close_right.floatValue, core_right.floatValue + Mathf.Epsilon);
    }
}
