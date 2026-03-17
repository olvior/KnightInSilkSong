using KIS.Utils;
using DebugMod;

namespace KIS;

public class DebugModIntegration
{
    internal static bool debugModFound;
    internal static DebugMod.DebugMod debugMod;

    public static bool IsActive()
    {
        return (debugModFound && KnightInSilksong.IsKnight);
    }

    public static void Init()
    {
        "trying to found debugmod".LogInfo();
        if ((bool)DebugMod.DebugMod.instance)
        {
            "Yes found debugmod".LogInfo();
            debugModFound = true;

            debugMod = DebugMod.DebugMod.instance;
        }
    }

    public static void Update()
    {
        if (!DebugModIntegration.IsActive())
            return;

        Knight.PlayerData pd = Knight.PlayerData.instance;
        Knight.HeroController hc = Knight.HeroController.instance;

        if (DebugMod.DebugMod.infiniteSilk)
        {
            if (pd.MPCharge < pd.maxMP)
            {
                pd.MPCharge = pd.maxMP;
                hc.SoulGain();
            }
        }

        if (DebugMod.DebugMod.noclip)
        {
            Knight.HeroController.instance.gameObject.transform.position = DebugMod.DebugMod.noclipPos;
        }
    }
}

// Patches
[HarmonyPatch(typeof(Knight.PlayerData), nameof(Knight.PlayerData.TakeHealth))]
public class DebugMod_Patch_Knight_PlayerData_TakeHealth : GeneralPatch
{
    private static bool Prefix(ref int amount)
    {
        if (!DebugModIntegration.IsActive())
            return true;

        PlayerData.instance.health = Knight.PlayerData.instance.health;
        DebugMod.ModHooks.PlayerData_TakeHealth(ref amount);

        return true;
    }
}

// Still not working
[HarmonyPatch(typeof(DebugMod.SaveStates.SaveState), nameof(DebugMod.SaveStates.SaveState.Load))]
public class DebugMod_Patch_Knight_SaveState_Load : GeneralPatch
{
    private static void Postfix()
    {
        if (!DebugModIntegration.IsActive())
            return;

        Knight.PlayerData.instance.health = PlayerData.instance.health;
        Knight.PlayerData.instance.MPCharge = PlayerData.instance.silk * 9;
    }
}

