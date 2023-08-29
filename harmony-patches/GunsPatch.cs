namespace Jaket.HarmonyPatches;

using HarmonyLib;
using System.Collections.Generic;
using UnityEngine;

using Jaket.Net;
using Jaket.Net.EntityTypes;

[HarmonyPatch(typeof(GunControl), nameof(GunControl.SwitchWeapon), typeof(int), typeof(List<GameObject>), typeof(bool), typeof(bool), typeof(bool))]
public class GunSwitchPatch
{
    // caching different things for optimization
    static void Postfix() => Networking.LocalPlayer.UpdateWeapon();
}

[HarmonyPatch(typeof(GunControl), nameof(GunControl.ForceWeapon))]
public class GunForcePatch
{
    // picked weapons also need to be painted
    static void Postfix() => Networking.LocalPlayer.UpdateWeapon();
}

[HarmonyPatch(typeof(GunColorGetter), nameof(GunColorGetter.UpdateColor))]
public class GunColorPatch
{
    // remote players update the colors of their weapons themselves
    static bool Prefix(GunColorGetter __instance) => __instance.GetComponentInParent<RemotePlayer>() == null;
}

[HarmonyPatch(typeof(WeaponIcon), nameof(WeaponIcon.UpdateIcon))]
public class GunIconPatch
{
    // remote player's weapons shouldn't update an icon in the local player's HUD
    static bool Prefix(WeaponIcon __instance) => __instance.GetComponentInParent<RemotePlayer>() == null;
}

[HarmonyPatch(typeof(HookArm), "Update")]
public class HookPatch
{
    static void Prefix(ref bool ___lightTarget, bool ___forcingFistControl, Vector3 ___hookPoint)
    {
        // nothing to comment here
        if (LobbyController.Lobby == null) return;

        // clients should be pulled to all enemies
        if (!LobbyController.IsOwner) ___lightTarget = false;

        // synchronize hook position
        Networking.LocalPlayer.hook = ___forcingFistControl ? ___hookPoint : Vector3.zero;
    }
}
