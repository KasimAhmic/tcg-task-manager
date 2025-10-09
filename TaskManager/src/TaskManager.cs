using System.Collections.Generic;
using System.Linq;
using BepInEx;
using HarmonyLib;
using JetBrains.Annotations;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace TaskManager
{
    [HarmonyPatch(typeof(Input), nameof(Input.GetAxis))]
    internal class BlockMouseAxisGetAxis
    {
        [UsedImplicitly]
        private static bool Prefix(string axisName, ref float __result)
        {
            if (!TaskManager.IsMenuOpen || (axisName != "Mouse X" && axisName != "Mouse Y")) return true;
            __result = 0f;
            return false; // skip original
        }
    }

    [HarmonyPatch(typeof(Input), nameof(Input.GetAxisRaw))]
    internal class BlockMouseAxisGetAxisRaw
    {
        [UsedImplicitly]
        private static bool Prefix(string axisName, ref float __result)
        {
            if (!TaskManager.IsMenuOpen || (axisName != "Mouse X" && axisName != "Mouse Y")) return true;
            __result = 0f;
            return false; // skip original
        }
    }

    [BepInPlugin("org.ahmic.taskmanager", "TaskManager", "1.0.0")]
    [BepInProcess("Card Shop Simulator.exe")]
    public class TaskManager : BaseUnityPlugin
    {
        private const string StockShelves = "Stock Shelves";
        private const string ManCounter = "Man Counter";
        private const string SetPrices = "Set Prices";

        private static Harmony _harmony;

        private bool _cursorOverridden;
        private bool _open;
        private bool _prevCursorVisible;
        private CursorLockMode _prevLockMode;
        private Rect _window = new Rect(5, 70, 300, 300);

        public static bool IsMenuOpen { get; private set; }

        private void Awake()
        {
            _harmony = new Harmony("org.ahmic.taskmanager");
            _harmony.PatchAll();
        }

        public void Update()
        {
            if (Input.GetKeyDown(KeyCode.T))
            {
                if (SceneManager.GetActiveScene().name == "Title")
                    // Opening the menu on the title screen causes a side effect of starting the WorkerManager with null
                    // references, leading to Workers not loading when a game is started.
                    return;
                
                Toggle();
            }

            // Some games re-lock the cursor each frame; enforce while open
            if (_open && !_cursorOverridden) SetCursorMode(true);
            if (!_open && _cursorOverridden) SetCursorMode(false);
        }

        public void OnGUI()
        {
            if (!_open) return;
            UI.Init();

            _window = GUILayout.Window(1337, _window, Draw, "Task Manager", UI.GetStyle(Style.Window));
        }

        [UsedImplicitly]
        public static void OnHotUnload()
        {
            try
            {
                _harmony?.UnpatchSelf();
            }
            catch
            {
                // ignored
            }
        }

        private void Toggle()
        {
            _open = !_open;
            IsMenuOpen = _open;
            SetCursorMode(_open);
        }

        private void Draw(int id)
        {
            GUILayout.Label("Assign Task to Team", UI.GetStyle(Style.Title));

            GUILayout.BeginHorizontal();
            HandleBulkAssignTaskButton(StockShelves, EWorkerTask.RestockShelf);
            HandleBulkAssignTaskButton(ManCounter, EWorkerTask.ManCounter);
            HandleBulkAssignTaskButton(SetPrices, EWorkerTask.SetPrice);
            GUILayout.EndHorizontal();

            GUILayout.Space(20);
            GUILayout.Label("Assign Task to Individual", UI.GetStyle(Style.Title));

            GUILayout.BeginHorizontal();
            GUILayout.Label("Name", UI.GetStyle(Style.Header), UI.GetLayout(Layout.WidthMedium));
            GUILayout.Label("Task", UI.GetStyle(Style.Header), UI.GetLayout(Layout.WidthLarge));
            GUILayout.Label("Stock Speed", UI.GetStyle(Style.Header), UI.GetLayout(Layout.WidthMedium));
            GUILayout.Label("Checkout Speed", UI.GetStyle(Style.Header), UI.GetLayout(Layout.WidthMedium));
            GUILayout.EndHorizontal();

            foreach (var worker in GetActiveWorkers())
            {
                GUILayout.BeginHorizontal();
                GUILayout.Label(worker.GetWorkerData().GetName(), UI.GetLayout(Layout.WidthMedium));
                HandleTaskButton(worker, EWorkerTask.RestockShelf, Style.StockShelvesButton, Layout.WidthSmall);
                HandleTaskButton(worker, EWorkerTask.ManCounter, Style.ManCountersButton, Layout.WidthSmall);
                HandleTaskButton(worker, EWorkerTask.SetPrice, Style.SetPricesButton, Layout.WidthSmall);
                GUILayout.Label(worker.GetWorkerData().GetRestockSpeedText(), UI.GetLayout(Layout.WidthMedium));
                GUILayout.Label(worker.GetWorkerData().GetCheckoutSpeedText(), UI.GetLayout(Layout.WidthMedium));
                GUILayout.EndHorizontal();
            }

            GUILayout.Space(20);
            if (GUILayout.Button("Close [T]", UI.GetStyle(Style.Button))) Toggle();
        }

        private static void HandleTaskButton(Worker worker, EWorkerTask task, Style style, Layout layout)
        {
            if (GUILayout.Button("", UI.GetStyle(style), UI.GetLayout(layout)))
                SetTask(worker, task);
        }

        private void HandleBulkAssignTaskButton(string label, EWorkerTask task)
        {
            if (GUILayout.Button(label, UI.GetStyle(Style.Button))) GetActiveWorkers().ForEach(worker => SetTask(worker, task));
        }

        private static void SetTask(Worker worker, EWorkerTask task)
        {
            worker.SetTask(task);
            worker.SetLastTask(task);
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

        private static List<Worker> GetActiveWorkers()
        {
            return WorkerManager.GetWorkerList().Where(worker => worker.IsActive()).ToList();
        }
    }
}