using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace MPewsey.HexagonalUI.Editor
{
    [CustomEditor(typeof(HexLayoutGroup))]
    public class HexLayoutGroupEditor : UnityEditor.Editor
    {
        public override void OnInspectorGUI()
        {
            serializedObject.Update();
            var constraint = serializedObject.FindProperty("_constraint");
            var flexibleConstraint = constraint.enumValueIndex == (int)HexLayoutGroup.ConstraintType.Flexible;
            var prop = serializedObject.GetIterator();
            GUI.enabled = false;
            var enterChildren = true;
            
            while (prop.NextVisible(enterChildren))
            {
                if (prop.name != "_constraintCount" || !flexibleConstraint)
                {
                    EditorGUILayout.PropertyField(prop, true);
                }

                GUI.enabled = true;
                enterChildren = false;
            }

            serializedObject.ApplyModifiedProperties();
        }
    }
}

