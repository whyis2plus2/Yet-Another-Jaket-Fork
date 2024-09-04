namespace Jaket.Patches;

using HarmonyLib;
using System;
using UnityEngine;

using Jaket.Assets;
using Jaket.Content;

[HarmonyPatch(typeof(EnemyInfoPage), "Start")]
public class BestiaryPatch
{
    static void Prefix(ref SpawnableObjectsDatabase ___objects)
    {
        // there is no point in adding V3 twice
        if (Array.Exists(___objects.enemies, obj => obj.identifier == "jaket.v3")) return;
        if (Array.Exists(___objects.enemies, obj => obj.identifier == "jaket.v4")) return;

        var v3 = ScriptableObject.CreateInstance<SpawnableObject>();
        var v3Entry = BestiaryEntry.Load(3);

        // set up all sorts of things
        v3.identifier = "jaket.v3";
        v3.enemyType = EnemyType.Filth;

        v3.backgroundColor = ___objects.enemies[11].backgroundColor;
        v3.gridIcon = DollAssets.Icon;

        v3.objectName = v3Entry.name;
        v3.type = v3Entry.type;
        v3.description = v3Entry.description;
        v3.strategy = v3Entry.strategy;
        v3.preview = DollAssets.Preview;

        // insert V3 after the turret in the list
        Array.Resize(ref ___objects.enemies, ___objects.enemies.Length + 1);
        Array.Copy(___objects.enemies, 15, ___objects.enemies, 16, ___objects.enemies.Length - 16);
        ___objects.enemies[15] = v3;

        var v4 = ScriptableObject.CreateInstance<SpawnableObject>();
        var v4Entry = BestiaryEntry.Load(4);

        // set up all sorts of things
        v4.identifier = "jaket.v4";
        v4.enemyType = EnemyType.Filth;

        v4.backgroundColor = ___objects.enemies[11].backgroundColor;
        v4.gridIcon = DollAssets.V4Icon;

        v4.objectName = v4Entry.name;
        v4.type = v4Entry.type;
        v4.description = v4Entry.description;
        v4.strategy = v4Entry.strategy;
        v4.preview = DollAssets.CreatePreviewWithSkin(
            DollAssets.WingTextures[(int)Team.White],
            DollAssets.BodyTextures[(int)Team.White]
        );

        // insert v4 after v3 in the list
        Array.Resize(ref ___objects.enemies, ___objects.enemies.Length + 1);
        Array.Copy(___objects.enemies, 16, ___objects.enemies, 17, ___objects.enemies.Length - 17);
        ___objects.enemies[16] = v4;
    }
}

[Serializable]
public class BestiaryEntry
{
    /// <summary> Bestiary entry fields displayed in terminal. </summary>
    public string name, type, description, strategy;
    /// <summary> Loads the V3 bestiary entry from the bundle. </summary>
    public static BestiaryEntry Load(byte vModel) 
    {
        vModel = Math.Min((byte)4, Math.Max((byte)3, vModel));
        return JsonUtility.FromJson<BestiaryEntry>(DollAssets.Bundle.LoadAsset<TextAsset>($"V{vModel}-bestiary-entry").text);
    }
}
