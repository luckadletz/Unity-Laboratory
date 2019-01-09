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
    
            /*  TODO Proper GUI
                IStateSource target for selection
                Reflect to get all planning properties as a dropdown
                Show property drawer

                How to do functions like MinimizeCost, MaximizeScore, etc?                
            */ 
            

            Rect nameRect = new Rect(
                position.x, position.y, 
                position.width * 0.5f, position.height);

            Rect valRect = new Rect(
                position.x + position.width * 0.5f, position.y, 
                position.width * 0.5f, position.height);

            string stateName = "asdfasdf";
            stateName = EditorGUI.DelayedTextField(nameRect, stateName);

            string valName = "FGH";
            valName = EditorGUI.DelayedTextField(valRect, valName);

            EditorGUI.EndProperty();
        }
    }
}