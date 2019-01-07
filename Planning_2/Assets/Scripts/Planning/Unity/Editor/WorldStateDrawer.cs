/* Luc Kadletz - 1/1/2019 */

using UnityEditor;
using UnityEngine;

namespace Planning
{
    [CustomPropertyDrawer(typeof(World))]
    public class WorldDrawer : PropertyDrawer
    {
        bool isFolded = true;

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            EditorGUI.BeginProperty(position, label, property);
            
            // TODO

            EditorGUI.EndProperty();
        }
    }
}