using UnityEngine;
using UnityEditor;
using UnityEditorInternal;
using System.Collections;
using Planning;
using System.Collections.Generic; // List<>

[CustomPropertyDrawer(typeof(Planning.State))]
public class StateEditor : PropertyDrawer
{

    // Draw the property inside the given rect
    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        // Using BeginProperty / EndProperty on the parent property means that
        // prefab override logic works on the entire property.
        EditorGUI.BeginProperty(position, label, property);

        // calculate some rectangles
        Rect nameRect = new Rect(
            position.x,
            position.y,
            position.width * 0.66f,
            position.height);
        Rect valueRect = new Rect(
            position.x + nameRect.width,
            position.y, 
            position.width - nameRect.width, 
            position.height);

        // Draw fields
        EditorGUI.PropertyField(nameRect, property.FindPropertyRelative("Name"), new GUIContent("State"));
        EditorGUI.PropertyField(valueRect, property.FindPropertyRelative("Value"), GUIContent.none);

        EditorGUI.EndProperty();
    }

}

//[CustomPropertyDrawer(typeof(StateList))]
//public class StateListEditor : PropertyDrawer
//{

//    // Draw the property inside the given rect
//    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
//    {
        


//    }

//}
