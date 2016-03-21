/*
 * Author: Luc Kadletz
 * 3/11/2016
 * 
 */

using UnityEngine;
using UnityEditor;
using UnityEditorInternal;
using System.Collections.Generic;
using System.Reflection;

/*
    Special thanks to:
    va.lent.in/unity-make-your-lists-functional-with-reorderablelist/
*/

[CustomEditor(typeof(LinguisticVariable))]
public class LinguisticEditor : Editor
{
    private ReorderableList list;
    private float term_min = 0, term_max = 0;
    private LinguisticVariable L;

    
    private List<FieldInfo> fields = new List<FieldInfo>();
    private List<MonoBehaviour> field_components = new List<MonoBehaviour>();
    private List<string> field_names = new List<string>();
    private int field_selected = 0;

    private void InitializeFieldReflectionLists()
    {
        fields.Clear();
        field_components.Clear();
        field_names.Clear();

        var all_components = L.GetComponents<MonoBehaviour>();

        var flags =
            BindingFlags.Instance |
            BindingFlags.Public |
            BindingFlags.FlattenHierarchy |
            BindingFlags.Default;

        // Loop through each component
        foreach (var mb in all_components)
        {
            // Get all of the fields of this component
            FieldInfo[] component_fields = mb.GetType().GetFields(flags);
            foreach (FieldInfo f in component_fields)
            {
                // If the field is a public float
                if (f.FieldType == typeof(float))
                {
                    // Add it to all three lists
                    fields.Add(f);
                    field_names.Add(f.Name);
                    field_components.Add(mb);
                }
                // else { Debug.Log("Ignored property " + f.Name); }
            }
        }

        // Make sure our lists are all the same length
        Debug.Assert(fields.Count == field_components.Count &&
            fields.Count == field_names.Count);

        // Set our selected field index
        field_selected = field_names.FindIndex((x) => (x == L.target_field_name));
    }

    void OnEnable()
    {
        L = target as LinguisticVariable;

        InitializeFieldReflectionLists();

        list = new ReorderableList(serializedObject,
            serializedObject.FindProperty("terms"),
            true, true, true, true);

        list.elementHeight = 3 * EditorGUIUtility.singleLineHeight;

        // set the Term callback
        list.drawElementCallback = (Rect rect, int index, bool isActive, bool isFocused) => 
        {
            var element = list.serializedProperty.GetArrayElementAtIndex(index);

            // Padding
            rect.y += 2;
            // Draw the name
            rect.height += EditorGUIUtility.singleLineHeight;
            EditorGUI.PropertyField(
                new Rect(rect.x, rect.y, 150, EditorGUIUtility.singleLineHeight),
                element.FindPropertyRelative("name"), GUIContent.none);
            // Draw the triangle fuzzy
            EditorGUI.PropertyField(
                new Rect(rect.x + 160, rect.y, rect.width - 160, EditorGUIUtility.singleLineHeight),
                element.FindPropertyRelative("values"), GUIContent.none);

            // We wrote on the first line, so reduce the size of our list
            rect.yMin += EditorGUIUtility.singleLineHeight + 2;
            rect.height -= EditorGUIUtility.singleLineHeight ;
            rect.height -= 6; // Padding
            rect.x += 160.0f;
            rect.width -= 160.0f;
            // Draw the animation curve
            AnimationCurve curve = new AnimationCurve();
            TriangleFuzzyNumber tri = L.terms[index].values;
            // Compute slopes
            float l = 1.0f / (tri.core - tri.close_left);
            float r = -1.0f / (tri.close_right - tri.core);

            Keyframe left = new Keyframe(tri.close_left, 0.0f);
            left.outTangent = l;
            curve.AddKey(left);

            Keyframe mid = new Keyframe(tri.core, 1.0f);
            mid.inTangent = l;
            mid.outTangent = r;
            curve.AddKey(mid);

            Keyframe right = new Keyframe(tri.close_right, 0.0f);
            right.inTangent = r;
            curve.AddKey(right);

            Keyframe min = new Keyframe(term_min, 0.0f);
            curve.AddKey(min);

            Keyframe max = new Keyframe(term_max, 0.0f);
            curve.AddKey(max);

            EditorGUI.CurveField(
                rect,
                curve, GetBrightColor(index, L.terms.Length)
                , new Rect(0,0,0,0));
        };

        // Set the header callback
        list.drawHeaderCallback = (Rect rect) =>
        {
            EditorGUI.LabelField(rect, "Linguistic Terms");
        };
    }

    public override void OnInspectorGUI()
    {
        serializedObject.Update();
        L = target as LinguisticVariable;

        // If we have no valid fields, tell the user and gtfo
        if (fields.Count == 0)
        {
            EditorGUILayout.HelpBox(
                "You need at least one attached component with a" +
                "public float field to use this component!",
                MessageType.Error,
                true);
            return;
        }

        // Draw a dropdown for input parameter
        field_selected = EditorGUILayout.Popup(
            "Parameter Field",
            field_selected,
            field_names.ToArray()
        );

        // Clamp this input in case our field is no more
        field_selected = Mathf.Clamp(field_selected, 0, fields.Count);

        // Tell our component the new target
        L.target_component_name = field_components[field_selected].GetType().Name;
        L.target_field_name = fields[field_selected].Name;

        // Update animation min and max
        if(L.terms != null)
        {
            term_min = term_max = 0;
            for(int i = 0; i < L.terms.Length; ++i)
            {
                term_min = Mathf.Min(L.terms[i].values.close_left, term_min);
                term_max = Mathf.Max(L.terms[i].values.close_right, term_max);
            }
        }

        // Draw the list
        list.DoLayoutList();

        serializedObject.ApplyModifiedProperties();
    }

    // Get some cool colors
    public Color GetBrightColor(int index, int max)
    {
        float deltaHue = 1.0f / max;

        return EditorGUIUtility.HSVToRGB(index * deltaHue, 1.0f, 1.0f);
    }
}

