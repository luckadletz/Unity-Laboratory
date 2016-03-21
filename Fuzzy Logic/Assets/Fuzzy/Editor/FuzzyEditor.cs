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

        //base.OnGUI(position, property, label);
    }
}