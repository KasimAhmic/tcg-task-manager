using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

namespace TaskManager;

public class TaskManager : MonoBehaviour
{
    private bool _cursorOverridden;
    private bool _isPrimarySelected = true;
    private bool _prevCursorVisible;
    private CursorLockMode _prevLockMode;
    private Rect _window = new(5, 70, 300, 300);
    public static bool IsMenuOpen { get; private set; }

    public void Update()
    {
        if (Keyboard.current != null && Keyboard.current.tKey.wasPressedThisFrame)
        {
            if (SceneManager.GetActiveScene().name == "Title") return;

            SetUI(!IsMenuOpen);

            if (IsMenuOpen && !_cursorOverridden) SetCursorMode(true);
            if (!IsMenuOpen && _cursorOverridden) SetCursorMode(false);
        }
    }

    public void OnDestroy()
    {
        UI.Unload();
    }

    public void OnGUI()
    {
        if (!IsMenuOpen) return;
        UI.Init();

        _window = GUILayout.Window(1337, _window, Draw, "Task Manager", UI.GetStyle(Style.Window));
    }

    private void Draw(int id)
    {
        GUILayout.Label("Assign Task to Team", UI.GetStyle(Style.Title));
        UI.HorizontalSeparator(Color.white);

        GUILayout.BeginHorizontal();
        HandleBulkAssignTaskButton("Stock Shelves", EWorkerTask.RestockShelf);
        HandleBulkAssignTaskButton("Man Counter", EWorkerTask.ManCounter);
        HandleBulkAssignTaskButton("Set Prices", EWorkerTask.SetPrice);
        GUILayout.EndHorizontal();
        GUILayout.BeginHorizontal();
        HandleBulkAssignTaskButton("Refill Cleanser", EWorkerTask.RefillCleanser);
        HandleBulkAssignTaskButton("Fill Pack Machine", EWorkerTask.RefillCardOpener);
        HandleBulkAssignTaskButton("Restock Cards", EWorkerTask.RestockCardDisplay);
        HandleBulkAssignTaskButton("Rest", EWorkerTask.Rest);
        GUILayout.EndHorizontal();

        GUILayout.Space(20);
        GUILayout.Label("Assign Task to Individual", UI.GetStyle(Style.Title));
        UI.HorizontalSeparator(Color.white);

        GUILayout.BeginHorizontal();
        GUILayout.Label("Employee", UI.GetStyle(Style.Header), UI.GetLayout(Layout.WidthSemiLarge));
        GUILayout.Label("Stocking", UI.GetStyle(Style.Header), UI.GetLayout(Layout.WidthSmall));
        GUILayout.Label("Checkout", UI.GetStyle(Style.Header), UI.GetLayout(Layout.WidthSmall));

        GUILayout.BeginVertical();
        GUILayout.BeginHorizontal();

        if (UI.TextShadowButton("Primary Task",
                _isPrimarySelected ? UI.GetStyle(Style.ButtonToggled) : UI.GetStyle(Style.Button),
                UI.GetLayout(Layout.WidthSemiLarge)))
        {
            _isPrimarySelected = true;
            SoundManager.PlayAudio("SFX_ButtonLightTap");
        }

        if (UI.TextShadowButton("Secondary Task",
                !_isPrimarySelected ? UI.GetStyle(Style.ButtonToggled) : UI.GetStyle(Style.Button),
                UI.GetLayout(Layout.WidthSemiLarge)))
        {
            _isPrimarySelected = false;
            SoundManager.PlayAudio("SFX_ButtonLightTap");
        }

        GUILayout.EndHorizontal();
        GUILayout.EndVertical();

        GUILayout.EndHorizontal();

        UI.HorizontalSeparator(Color.white);

        foreach (var worker in GetActiveWorkers())
        {
            GUILayout.BeginHorizontal();
            GUILayout.Box(worker.GetWorkerData().icon.texture, UI.GetLayout(Layout.TinyBox));
            GUILayout.Label(worker.GetWorkerData().GetName(), UI.GetStyle(Style.Label), UI.GetLayout(Layout.WidthMedium));
            GUILayout.Label(worker.GetWorkerData().GetRestockSpeedText(), UI.GetStyle(Style.Label), UI.GetLayout(Layout.WidthSmall));
            GUILayout.Label(worker.GetWorkerData().GetCheckoutSpeedText(), UI.GetStyle(Style.Label), UI.GetLayout(Layout.WidthSmall));

            HandleTaskButton(worker, EWorkerTask.RestockShelf, GetButtonStyle(worker, EWorkerTask.RestockShelf), Layout.WidthTiny);
            HandleTaskButton(worker, EWorkerTask.ManCounter, GetButtonStyle(worker, EWorkerTask.ManCounter), Layout.WidthTiny);
            HandleTaskButton(worker, EWorkerTask.SetPrice, GetButtonStyle(worker, EWorkerTask.SetPrice), Layout.WidthTiny);
            HandleTaskButton(worker, EWorkerTask.RefillCleanser, GetButtonStyle(worker, EWorkerTask.RefillCleanser), Layout.WidthTiny);
            HandleTaskButton(worker, EWorkerTask.RefillCardOpener, GetButtonStyle(worker, EWorkerTask.RefillCardOpener), Layout.WidthTiny);
            HandleTaskButton(worker, EWorkerTask.RestockCardDisplay, GetButtonStyle(worker, EWorkerTask.RestockCardDisplay), Layout.WidthTiny);
            HandleTaskButton(worker, EWorkerTask.Rest, GetButtonStyle(worker, EWorkerTask.Rest), Layout.WidthTiny);

            GUILayout.EndHorizontal();
        }

        GUILayout.Space(20);
        if (UI.TextShadowButton("Close [ T ]", UI.GetStyle(Style.Button))) SetUI(false);
    }

    private Style GetButtonStyle(Worker worker, EWorkerTask task)
    {
        var currentTask = _isPrimarySelected ? worker.m_PrimaryTask : worker.m_SecondaryTask;

        return task switch
        {
            EWorkerTask.RestockShelf => currentTask == EWorkerTask.RestockShelf
                ? Style.StockShelvesButtonToggled
                : Style.StockShelvesButton,
            EWorkerTask.ManCounter => currentTask == EWorkerTask.ManCounter
                ? Style.ManCountersButtonToggled
                : Style.ManCountersButton,
            EWorkerTask.SetPrice => currentTask == EWorkerTask.SetPrice
                ? Style.SetPricesButtonToggled
                : Style.SetPricesButton,
            EWorkerTask.RefillCleanser => currentTask == EWorkerTask.RefillCleanser
                ? Style.RefillScentButtonToggled
                : Style.RefillScentButton,
            EWorkerTask.RefillCardOpener => currentTask == EWorkerTask.RefillCardOpener
                ? Style.PackMachineButtonToggled
                : Style.PackMachineButton,
            EWorkerTask.RestockCardDisplay => currentTask == EWorkerTask.RestockCardDisplay
                ? Style.RestockCardsButtonToggled
                : Style.RestockCardsButton,
            EWorkerTask.Rest => currentTask == EWorkerTask.Rest
                ? Style.RestButtonToggled
                : Style.RestButton,
            EWorkerTask.Fired => throw new ArgumentException("Cannot assign a fired task to a worker.", nameof(task)),
            _ => throw new ArgumentOutOfRangeException(nameof(task), task, null)
        };
    }

    private void SetUI(bool open, bool runSideEffects = true)
    {
        SetCursorMode(open);
        IsMenuOpen = open;

        switch (open)
        {
            case true when runSideEffects:
                SoundManager.GenericMenuOpen();
                InteractionPlayerController.Instance.StopCameraLerp();
                InteractionPlayerController.Instance.ShowCursor();
                CSingleton<InteractionPlayerController>.Instance.m_WalkerCtrl.SetStopMovement(true);
                GameUIScreen.HideEnterGoNextDayIndicatorVisible();
                break;
            case false when runSideEffects:
                SoundManager.GenericMenuClose();
                InteractionPlayerController.Instance.HideCursor();
                CSingleton<InteractionPlayerController>.Instance.m_WalkerCtrl.SetStopMovement(false);
                GameUIScreen.ResetEnterGoNextDayIndicatorVisible();
                break;
        }
    }

    private void HandleTaskButton(Worker worker, EWorkerTask task, Style style, Layout layout)
    {
        if (GUILayout.Button("", UI.GetStyle(style), UI.GetLayout(layout)))
        {
            SetTask(worker, task, _isPrimarySelected);
            SoundManager.PlayAudio("SFX_ButtonLightTap");
        }
    }

    private void HandleBulkAssignTaskButton(string label, EWorkerTask task)
    {
        if (UI.TextShadowButton(label, UI.GetStyle(Style.Button)))
        {
            GetActiveWorkers().ForEach(worker => SetTask(worker, task, true));
            GetActiveWorkers().ForEach(worker => SetTask(worker, task, false));
            SoundManager.PlayAudio("SFX_ButtonLightTap");
        }
    }

    private static void SetTask(Worker worker, EWorkerTask task, bool isPrimary)
    {
        if (isPrimary)
        {
            worker.SetTask(task);
            worker.SetLastTask(task);
        }
        else
        {
            worker.SetSecondaryTask(task);
        }
    }

    private static List<Worker> GetActiveWorkers()
    {
        return WorkerManager.GetWorkerList().Where(worker => worker.IsActive()).ToList();
    }

    private void SetCursorMode(bool uiOpen)
    {
        if (uiOpen)
        {
            // Save current state once
            if (!_cursorOverridden)
            {
                _prevCursorVisible = Cursor.visible;
                _prevLockMode = Cursor.lockState;
            }

            Cursor.visible = true;
            Cursor.lockState = CursorLockMode.None; // release capture
            _cursorOverridden = true;
        }
        else
        {
            // Restore previous state
            if (!_cursorOverridden) return;

            Cursor.visible = _prevCursorVisible;
            Cursor.lockState = _prevLockMode;
            _cursorOverridden = false;
        }
    }
}