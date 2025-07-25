﻿using UnityEditor;
using UnityEngine;

namespace CelesteEditor.PropertyDrawers.Parameters
{
    public abstract class ParameterReferencePropertyDrawer : PropertyDrawer
    {
        private const float TOGGLE_VALUE_SPACING = 4;

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            EditorGUI.BeginProperty(position, label, property);

            position = EditorGUI.PrefixLabel(position, GUIUtility.GetControlID(FocusType.Passive), label);

            if (property.objectReferenceValue != null)
            {
                SerializedObject serializedReference = new SerializedObject(property.objectReferenceValue);
                serializedReference.Update();

                SerializedProperty isConstantProperty = serializedReference.FindProperty("isConstant");
                if (isConstantProperty != null)
                {
                    Rect constantToggleRect = new Rect(position.x, position.y, 16, position.height);
                    using (EditorGUI.ChangeCheckScope changeScope = new EditorGUI.ChangeCheckScope())
                    {
                        isConstantProperty.boolValue = EditorGUI.Toggle(constantToggleRect, isConstantProperty.boolValue, EditorStyles.radioButton);

                        if (isConstantProperty.boolValue && changeScope.changed)
                        {
                            serializedReference.FindProperty("referenceValue").objectReferenceValue = null;
                        }
                    }

                    float gap = constantToggleRect.width + TOGGLE_VALUE_SPACING;
                    Rect valueRect = new Rect(constantToggleRect.x + gap, constantToggleRect.y, position.width - gap, position.height);
                    if (isConstantProperty.boolValue)
                    {
                        SerializedProperty constantValueProperty = serializedReference.FindProperty("constantValue");
                        if (constantValueProperty != null)
                        {
                            EditorGUI.PropertyField(valueRect, constantValueProperty, GUIContent.none);
                        }
                        else
                        {
                            EditorGUI.LabelField(valueRect, "ERROR - No Constant Value found on parameter...");
                        }
                    }
                    else
                    {
                        SerializedProperty referenceValueProperty = serializedReference.FindProperty("referenceValue");
                        if (referenceValueProperty != null)
                        {
                            EditorGUI.PropertyField(valueRect, referenceValueProperty, GUIContent.none);
                        }
                        else
                        {
                            EditorGUI.LabelField(valueRect, "ERROR - No Reference Value found on parameter...");
                        }
                    }
                }

                serializedReference.ApplyModifiedProperties();
            }
            else
            {
                EditorGUI.PropertyField(position, property);
            }

            EditorGUI.EndProperty();
        }
    }
}
