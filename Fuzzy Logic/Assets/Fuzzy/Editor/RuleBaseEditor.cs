/*
 * Author: Luc Kadletz
 * 3/11/2016
 * 
 */


using UnityEngine;
using System.Collections.Generic;
using UnityEditorInternal;
using UnityEditor;

[CustomPropertyDrawer(typeof(RuleBase.Rule))]
public class RuleDrawer : PropertyDrawer
{

    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        // Grab our properties
        SerializedProperty precedent = property.FindPropertyRelative("input");
        SerializedProperty input = property.FindPropertyRelative("inputProperty");
        SerializedProperty antecedent = property.FindPropertyRelative("output");
        SerializedProperty output = property.FindPropertyRelative("outputProperty");

        GUIStyle style = new GUIStyle();
        
        position.height = 3 * EditorGUIUtility.singleLineHeight;
        // Divide into sixths
        float eighth = position.width / 8.0f;
        float quarterLineHeight = EditorGUIUtility.singleLineHeight * 0.25f;

        position.y += quarterLineHeight; // Padding
        // Draw "when"
        style.alignment = TextAnchor.MiddleRight;
        EditorGUI.LabelField(
            new Rect(position.x, position.y, eighth, EditorGUIUtility.singleLineHeight),
            "When ", style);

        // Draw first object reference
        precedent.objectReferenceValue = EditorGUI.ObjectField(
            new Rect(position.x + eighth, position.y, 3 * eighth, EditorGUIUtility.singleLineHeight),
            precedent.objectReferenceValue, typeof(LinguisticVariable),true);

        // Draw "is"
        style.alignment = TextAnchor.MiddleCenter;
        EditorGUI.LabelField(
            new Rect(position.x + 4 * eighth, position.y, eighth, EditorGUIUtility.singleLineHeight),
            "is", style);

        // Draw the first string dropdown
        // Get a list of strings based of the selected object
        List<string> strings = new List<string>();
        if(precedent.objectReferenceValue != null)
            strings = (precedent.objectReferenceValue as LinguisticVariable).GetTermStrings();
        // Find the index of our selected string so we can use it for the popup
        int input_string_index = 0;
        if (strings.Contains(input.stringValue))
        {
            input_string_index = strings.IndexOf(input.stringValue);
        }

        // Draw the popup
        if(strings.Count != 0)
        {
            input_string_index = EditorGUI.Popup(
                new Rect(position.x + 5 * eighth, position.y, 2 * eighth, EditorGUIUtility.singleLineHeight),
                input_string_index, strings.ToArray());
            // Record the string value
            input.stringValue = strings[input_string_index];
        }

        // Move to second line and pad
        position.y += 5 * quarterLineHeight;

        // Draw "then"
        style.alignment = TextAnchor.MiddleRight;
        EditorGUI.LabelField(
            new Rect(position.x + eighth, position.y, eighth, EditorGUIUtility.singleLineHeight),
            "Then ", style);

        // Draw second object reference
        antecedent.objectReferenceValue = EditorGUI.ObjectField(
            new Rect(position.x + 2 *eighth, position.y, 3 * eighth, EditorGUIUtility.singleLineHeight),
            antecedent.objectReferenceValue, typeof(LinguisticVariable), true);

        // Draw "is"
        style.alignment = TextAnchor.MiddleCenter;
        EditorGUI.LabelField(
            new Rect(position.x + 5 * eighth, position.y, eighth, EditorGUIUtility.singleLineHeight),
            "is ", style);

        // Draw the second string dropdown
        // See above
        strings.Clear();
        if (antecedent.objectReferenceValue != null)
            strings = (antecedent.objectReferenceValue as LinguisticVariable).GetTermStrings();
        int output_string_index = 0;
        if (strings.Contains(output.stringValue))
        {
            output_string_index = strings.IndexOf(output.stringValue);
        }
        if(strings.Count != 0)
        {
            output_string_index = EditorGUI.Popup(
                new Rect(position.x + 6 * eighth, position.y, 2 * eighth, EditorGUIUtility.singleLineHeight),
                output_string_index, strings.ToArray());
            // Record the string value
            output.stringValue = strings[output_string_index];
        }
    }
}


[CustomEditor(typeof(RuleBase))]
public class RuleBaseEditor : Editor
{

    private ReorderableList list;


    void OnEnable()
    {
        list = new ReorderableList(serializedObject,
            serializedObject.FindProperty("Rules"),
            true, true, true, true);

        // Set the header callback
        list.drawHeaderCallback = (Rect rect) =>
        {
            EditorGUI.LabelField(rect, "Linguistic Terms");
        };

        // set the Term callback
        list.drawElementCallback = (Rect rect, int index, bool isActive, bool isFocused) =>
        {
            // Make sure our index is still valid
            if (index > list.serializedProperty.arraySize)
                index = 0;
            if (list.serializedProperty.arraySize == 0)
                return;

            SerializedProperty rule = list.serializedProperty.GetArrayElementAtIndex(index);

            EditorGUI.PropertyField(
                rect, rule);
        };

        list.elementHeight = 3 * EditorGUIUtility.singleLineHeight;
    }

    public override void OnInspectorGUI()
    {
        serializedObject.Update();
        list.DoLayoutList();
        serializedObject.ApplyModifiedProperties();
    }
}

