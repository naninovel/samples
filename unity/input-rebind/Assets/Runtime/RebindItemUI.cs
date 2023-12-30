using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class RebindItemUI : MonoBehaviour
{
    public InputActionReference ActionReference
    {
        get => action;
        set
        {
            action = value;
            UpdateActionLabel();
            UpdateBindingDisplay();
        }
    }

    public string BindingId
    {
        get => bindingId;
        set
        {
            bindingId = value;
            UpdateBindingDisplay();
        }
    }

    public InputBinding.DisplayStringOptions DisplayStringOptions
    {
        get => displayStringOptions;
        set
        {
            displayStringOptions = value;
            UpdateBindingDisplay();
        }
    }

    public Text ActionLabel
    {
        get => actionLabel;
        set
        {
            actionLabel = value;
            UpdateActionLabel();
        }
    }

    public Text BindingText
    {
        get => bindingText;
        set
        {
            bindingText = value;
            UpdateBindingDisplay();
        }
    }

    public Text RebindPrompt
    {
        get => rebindText;
        set => rebindText = value;
    }

    public GameObject RebindOverlay
    {
        get => rebindOverlay;
        set => rebindOverlay = value;
    }

    public InputActionRebindingExtensions.RebindingOperation OngoingRebind => rebindOperation;

    [Tooltip("Reference to action that is to be rebound from the UI.")]
    [SerializeField] private InputActionReference action;
    [SerializeField] private string bindingId;
    [SerializeField] private InputBinding.DisplayStringOptions displayStringOptions;
    [Tooltip("Text label that will receive the name of the action. Optional. Set to None to have the rebind UI not show a label for the action.")]
    [SerializeField] private Text actionLabel;
    [Tooltip("Text label that will receive the current, formatted binding string.")]
    [SerializeField] private Text bindingText;
    [Tooltip("Optional UI that will be shown while a rebind is in progress.")]
    [SerializeField] private GameObject rebindOverlay;
    [Tooltip("Optional text label that will be updated with prompt for user input.")]
    [SerializeField] private Text rebindText;

    private static List<RebindItemUI> rebindActionUIs;
    private InputActionRebindingExtensions.RebindingOperation rebindOperation;

    public bool ResolveActionAndBinding (out InputAction action, out int bindingIndex)
    {
        bindingIndex = -1;
        action = null;

        if (!this.action || this.action.action is null) return false;
        action = this.action.action;

        if (string.IsNullOrEmpty(this.bindingId)) return false;

        var bindingId = new Guid(this.bindingId);
        bindingIndex = action.bindings.IndexOf(x => x.id == bindingId);
        if (bindingIndex == -1)
        {
            Debug.LogError($"Cannot find binding with ID '{bindingId}' on '{action}'", this);
            return false;
        }

        return true;
    }

    public void UpdateBindingDisplay ()
    {
        var displayString = string.Empty;

        if (this.action && this.action.action != null)
        {
            var bindingIndex = this.action.action.bindings.IndexOf(x => x.id.ToString() == bindingId);
            if (bindingIndex != -1)
                displayString = this.action.action.GetBindingDisplayString(bindingIndex, out _, out _, DisplayStringOptions);
        }

        if (bindingText != null)
            bindingText.text = displayString;
    }

    public void ResetToDefault ()
    {
        if (!ResolveActionAndBinding(out var action, out var bindingIndex))
            return;

        if (action.bindings[bindingIndex].isComposite)
            for (var i = bindingIndex + 1; i < action.bindings.Count && action.bindings[i].isPartOfComposite; ++i)
                action.RemoveBindingOverride(i);
        else action.RemoveBindingOverride(bindingIndex);

        UpdateBindingDisplay();
    }

    public void StartInteractiveRebind ()
    {
        if (!ResolveActionAndBinding(out var action, out var bindingIndex))
            return;

        if (action.bindings[bindingIndex].isComposite)
        {
            var firstPartIndex = bindingIndex + 1;
            if (firstPartIndex < action.bindings.Count && action.bindings[firstPartIndex].isPartOfComposite)
                PerformInteractiveRebind(action, firstPartIndex, allCompositeParts: true);
        }
        else PerformInteractiveRebind(action, bindingIndex);
    }

    private void PerformInteractiveRebind (InputAction action, int bindingIndex, bool allCompositeParts = false)
    {
        action.Disable();
        
        rebindOperation?.Cancel();

        rebindOperation = action.PerformInteractiveRebinding(bindingIndex)
            .OnCancel(
                operation => {
                    if (rebindOverlay) rebindOverlay.SetActive(false);
                    UpdateBindingDisplay();
                    CleanUp();
                })
            .OnComplete(
                operation => {
                    if (rebindOverlay) rebindOverlay.SetActive(false);
                    UpdateBindingDisplay();
                    CleanUp();

                    if (allCompositeParts)
                    {
                        var nextBindingIndex = bindingIndex + 1;
                        if (nextBindingIndex < action.bindings.Count && action.bindings[nextBindingIndex].isPartOfComposite)
                            PerformInteractiveRebind(action, nextBindingIndex, true);
                    }
                });

        var partName = default(string);

        if (action.bindings[bindingIndex].isPartOfComposite)
            partName = $"Binding '{action.bindings[bindingIndex].name}'. ";

        if (rebindOverlay) rebindOverlay.SetActive(true);

        if (rebindText != null)
        {
            var text = !string.IsNullOrEmpty(rebindOperation.expectedControlType)
                ? $"{partName}Waiting for {rebindOperation.expectedControlType} input..."
                : $"{partName}Waiting for input...";
            rebindText.text = text;
        }

        if (rebindOverlay == null && rebindText == null && bindingText != null)
            bindingText.text = "<Waiting...>";

        rebindOperation.Start();
        
        action.Enable();

        void CleanUp ()
        {
            rebindOperation?.Dispose();
            rebindOperation = null;
        }
    }

    protected void OnEnable ()
    {
        if (rebindActionUIs == null)
            rebindActionUIs = new List<RebindItemUI>();
        rebindActionUIs.Add(this);
        if (rebindActionUIs.Count == 1)
            InputSystem.onActionChange += OnActionChange;
    }

    protected void OnDisable ()
    {
        rebindOperation?.Dispose();
        rebindOperation = null;

        rebindActionUIs.Remove(this);
        if (rebindActionUIs.Count == 0)
        {
            rebindActionUIs = null;
            InputSystem.onActionChange -= OnActionChange;
        }
    }

    private static void OnActionChange (object obj, InputActionChange change)
    {
        if (change != InputActionChange.BoundControlsChanged)
            return;

        var action = obj as InputAction;
        var actionMap = action?.actionMap ?? obj as InputActionMap;
        var actionAsset = actionMap?.asset ? actionMap.asset : obj as InputActionAsset;

        for (var i = 0; i < rebindActionUIs.Count; ++i)
        {
            var component = rebindActionUIs[i];
            var referencedAction = component.ActionReference && component.ActionReference.action != null
                ? component.ActionReference.action
                : null;
            if (referencedAction == null)
                continue;

            if (referencedAction == action ||
                referencedAction.actionMap == actionMap ||
                referencedAction.actionMap?.asset == actionAsset)
                component.UpdateBindingDisplay();
        }
    }

    #if UNITY_EDITOR
    protected void OnValidate ()
    {
        UpdateActionLabel();
        UpdateBindingDisplay();
    }
    #endif

    private void UpdateActionLabel ()
    {
        if (actionLabel != null && action)
        {
            var action = this.action.action;
            actionLabel.text = action != null ? action.name : string.Empty;
        }
    }
}
