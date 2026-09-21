using HarmonyLib;
using JetBrains.Annotations;
using UnityEngine;

namespace TaskManager;

public static class Patches
{
    public static void Apply()
    {
        var harmony = new Harmony(MyPluginInfo.PLUGIN_GUID);
        harmony.PatchAll();
    }

    [HarmonyPatch(typeof(Input), nameof(Input.GetAxis))]
    internal static class BlockMouseAxisGetAxis
    {
        [HarmonyPrefix, UsedImplicitly]
        private static bool Prefix(string axisName, ref float __result)
        {
            if (!TaskManager.IsMenuOpen || (axisName != "Mouse X" && axisName != "Mouse Y")) return true;
            __result = 0f;
            return false;
        }
    }

    [HarmonyPatch(typeof(Input), nameof(Input.GetAxisRaw))]
    internal static class BlockMouseAxisGetAxisRaw
    {
        [HarmonyPrefix, UsedImplicitly]
        private static bool Prefix(string axisName, ref float __result)
        {
            if (!TaskManager.IsMenuOpen || (axisName != "Mouse X" && axisName != "Mouse Y")) return true;
            __result = 0f;
            return false;
        }
    }

    [HarmonyPatch(typeof(InputManager), nameof(InputManager.GetKeyDownAction))]
    internal static class BlockInteractKeyDown
    {
        [HarmonyPrefix, UsedImplicitly]
        private static bool Prefix(EGameAction action, ref bool __result)
        {
            if (!TaskManager.IsMenuOpen ||
                (action != EGameAction.InteractLeft && action != EGameAction.InteractRight)) return true;

            __result = false;
            return false;
        }
    }

    [HarmonyPatch(typeof(InputManager), nameof(InputManager.GetKeyUpAction))]
    internal static class BlockInteractKeyUp
    {
        [HarmonyPrefix, UsedImplicitly]
        private static bool Prefix(EGameAction action, ref bool __result)
        {
            if (!TaskManager.IsMenuOpen ||
                (action != EGameAction.InteractLeft && action != EGameAction.InteractRight)) return true;

            __result = false;
            return false;
        }
    }

    [HarmonyPatch(typeof(InputManager), nameof(InputManager.GetKeyHoldAction))]
    internal static class BlockInteractKeyHold
    {
        [HarmonyPrefix, UsedImplicitly]
        private static bool Prefix(EGameAction action, ref bool __result)
        {
            if (!TaskManager.IsMenuOpen ||
                (action != EGameAction.InteractLeft && action != EGameAction.InteractRight)) return true;

            __result = false;
            return false;
        }
    }

#if PATCH_GAME_LOG_SPAM
    /*
     * The game has a "bug" where it will spam the log with null reference exceptions if the PlayCardSet is null. This
     * patch prevents that from happening.
     */

    [HarmonyPatch(typeof(PlayCardSetUI), "Update")]
    private static class PlayCardSetUIUpdatePatch
    {
        [HarmonyPrefix, UsedImplicitly]
        private static bool Prefix(PlayCardSet ___m_PlayCardSet)
        {
            return ___m_PlayCardSet != null;
        }
    }

    [HarmonyPatch(typeof(PlayCardSetUI), "LateUpdate")]
    private static class PlayCardSetUILateUpdatePatch
    {
        [HarmonyPrefix, UsedImplicitly]
        private static bool Prefix(PlayCardSet ___m_PlayCardSet)
        {
            return ___m_PlayCardSet != null;
        }
    }
#endif
}