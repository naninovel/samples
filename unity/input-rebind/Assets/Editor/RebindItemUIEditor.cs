using System;
using System.Linq;
using UnityEditor;
using UnityEngine;
using UnityEngine.InputSystem;

[CustomEditor(typeof(RebindItemUI))]
public class RebindActionUIEditor : Editor
{
    private static class Styles
    {
        public static readonly GUIStyle BoldLabel = new GUIStyle("MiniBoldLabel");
    }

    private SerializedProperty actionProperty;
    private SerializedProperty bindingIdProperty;
    private SerializedProperty actionLabelProperty;
    private SerializedProperty bindingTextProperty;
    private SerializedProperty rebindOverlayProperty;
    private SerializedProperty rebindTextProperty;
    private SerializedProperty displayStringOptionsProperty;

    private GUIContent bindingLabel = new GUIContent("Binding");
    private GUIContent displayOptionsLabel = new GUIContent("Display Options");
    private GUIContent uiLabel = new GUIContent("UI");
    private GUIContent eventsLabel = new GUIContent("Events");
    private GUIContent[] bindingOptions;
    private string[] bindingOptionValues;
    private int selectedBindingOption;

    public override void OnInspectorGUI ()
    {
        EditorGUI.BeginChangeCheck();

        EditorGUILayout.LabelField(bindingLabel, Styles.BoldLabel);
        using (new EditorGUI.IndentLevelScope())
        {
            EditorGUILayout.PropertyField(actionProperty);

            var newSelectedBinding = EditorGUILayout.Popup(bindingLabel, selectedBindingOption, bindingOptions);
            if (newSelectedBinding != selectedBindingOption)
            {
                var bindingId = bindingOptionValues[newSelectedBinding];
                bindingIdProperty.stringValue = bindingId;
                selectedBindingOption = newSelectedBinding;
            }

            var optionsOld = (InputBinding.DisplayStringOptions)displayStringOptionsProperty.intValue;
            var optionsNew = (InputBinding.DisplayStringOptions)EditorGUILayout.EnumFlagsField(displayOptionsLabel, optionsOld);
            if (optionsOld != optionsNew) displayStringOptionsProperty.intValue = (int)optionsNew;
        }

        EditorGUILayout.Space();
        EditorGUILayout.LabelField(uiLabel, Styles.BoldLabel);
        using (new EditorGUI.IndentLevelScope())
        {
            EditorGUILayout.PropertyField(actionLabelProperty);
            EditorGUILayout.PropertyField(bindingTextProperty);
            EditorGUILayout.PropertyField(rebindOverlayProperty);
            EditorGUILayout.PropertyField(rebindTextProperty);
        }

        if (EditorGUI.EndChangeCheck())
        {
            serializedObject.ApplyModifiedProperties();
            RefreshBindingOptions();
        }
    }

    private void OnEnable ()
    {
        actionProperty = serializedObject.FindProperty("action");
        bindingIdProperty = serializedObject.FindProperty("bindingId");
        actionLabelProperty = serializedObject.FindProperty("actionLabel");
        bindingTextProperty = serializedObject.FindProperty("bindingText");
        rebindOverlayProperty = serializedObject.FindProperty("rebindOverlay");
        rebindTextProperty = serializedObject.FindProperty("rebindText");
        displayStringOptionsProperty = serializedObject.FindProperty("displayStringOptions");

        RefreshBindingOptions();
    }

    private void RefreshBindingOptions ()
    {
        var actionReference = (InputActionReference)actionProperty.objectReferenceValue;

        if (!actionReference || actionReference.action is null)
        {
            bindingOptions = Array.Empty<GUIContent>();
            bindingOptionValues = Array.Empty<string>();
            selectedBindingOption = -1;
            return;
        }

        var action = actionReference.action;
        var bindings = action.bindings;
        var bindingCount = bindings.Count;

        bindingOptions = new GUIContent[bindingCount];
        bindingOptionValues = new string[bindingCount];
        selectedBindingOption = -1;

        var currentBindingId = bindingIdProperty.stringValue;
        for (var i = 0; i < bindingCount; ++i)
        {
            var binding = bindings[i];
            var bindingId = binding.id.ToString();
            var haveBindingGroups = !string.IsNullOrEmpty(binding.groups);

            var displayOptions = InputBinding.DisplayStringOptions.DontUseShortDisplayNames | InputBinding.DisplayStringOptions.IgnoreBindingOverrides;
            if (!haveBindingGroups) displayOptions |= InputBinding.DisplayStringOptions.DontOmitDevice;

            var displayString = "";
            try { displayString = action.GetBindingDisplayString(i, displayOptions); }
            catch (NotImplementedException) { }

            if (binding.isPartOfComposite)
                displayString = $"{ObjectNames.NicifyVariableName(binding.name)}: {displayString}";

            displayString = displayString.Replace('/', '\\');

            if (haveBindingGroups)
            {
                var asset = action.actionMap?.asset;
                if (asset != null)
                {
                    var controlSchemes = string.Join(", ",
                        binding.groups.Split(InputBinding.Separator)
                            .Select(x => asset.controlSchemes.FirstOrDefault(c => c.bindingGroup == x).name));

                    displayString = $"{displayString} ({controlSchemes})";
                }
            }

            bindingOptions[i] = new GUIContent(displayString);
            bindingOptionValues[i] = bindingId;

            if (currentBindingId == bindingId)
                selectedBindingOption = i;
        }
    }
}
