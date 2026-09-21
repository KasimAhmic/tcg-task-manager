using BepInEx;
using BepInEx.Logging;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace TaskManager;

[BepInPlugin(MyPluginInfo.PLUGIN_GUID, MyPluginInfo.PLUGIN_NAME, MyPluginInfo.PLUGIN_VERSION)]
public class Plugin : BaseUnityPlugin
{
    private static TaskManager _taskManager;

    internal new static ManualLogSource Logger;

    private void Awake()
    {
        Logger = base.Logger;
        Patches.Apply();
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    private static void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        if (!ReferenceEquals(_taskManager, null) && _taskManager)
        {
            Logger.LogInfo("TaskManager already exists, skipping creation");
            return;
        }

        Logger.LogInfo($"{MyPluginInfo.PLUGIN_NAME} is loading...");

        var gameObject = new GameObject("TCGTaskManager");
        DontDestroyOnLoad(gameObject);
        _taskManager = gameObject.AddComponent<TaskManager>();

        Logger.LogInfo($"{MyPluginInfo.PLUGIN_NAME} is loaded!");
    }
}