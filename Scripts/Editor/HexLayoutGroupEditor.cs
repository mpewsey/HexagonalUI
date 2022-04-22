using UnityEditor;
using UnityEngine;

namespace MPewsey.HexagonalUI.Editor
{
    /// <summary>
    /// The HexLayoutGroup custom inspector.
    /// </summary>
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
                    EditorGUILayout.PropertyField(prop, true);

                GUI.enabled = true;
                enterChildren = false;
            }

            serializedObject.ApplyModifiedProperties();
        }
    }
}

