using PowerupAPI.Powerups;
using PowerupAPI.Base;
using HarmonyLib;
using BepInEx;
using Panik;
using UnityEngine;


[BepInPlugin("com.unconscious.cirnocharm", "Cirno Fumo Charm", "1.0.0")]
[BepInDependency("com.unconscious.powerupapi", BepInDependency.DependencyFlags.HardDependency)]
public class CirnoFumoCharm : BaseUnityPlugin
{
    public static PowerupScript.Identifier cirnoCharmID;
    void Awake()
    {
        string dllPath = System.IO.Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location);
        string bundlePath = System.IO.Path.Combine(dllPath, "cirno");

        AssetBundle bundle = AssetBundle.LoadFromFile(bundlePath);

        GameObject cirnoPrefab = bundle.LoadAsset<GameObject>("cirno");
        AudioClip cirnoSound = bundle.LoadAsset<AudioClip>("cirnosound");
        cirnoPrefab.name = "Powerup Cirno Fumo";
        cirnoSound.name = "Cirno Sound";

        cirnoPrefab.transform.localScale = new UnityEngine.Vector3(1,1,1);
        cirnoPrefab.transform.localRotation = UnityEngine.Quaternion.Euler(-90f, 0f, 0f);

        CustomPowerup cirno = new CustomPowerup(
            startingPrice: 3,
            prefab: cirnoPrefab,
            sound: cirnoSound,
            displayName: "Cirno Fumo",
            description: "Increases 7s to 9s",
            onEquip: PFunc_OnEquip_Cirno
        );

        cirnoCharmID = CustomPowerupAPI.AddCustomPowerup(cirno);

        var harmony = new Harmony("com.unconscious.cirnocharm");
        harmony.PatchAll();
        Logger.LogInfo("CirnoFumoCharm patches applied!");
    }


    public static void PFunc_OnEquip_Cirno(PowerupScript powerup)
    {
        PowerupScript.PlayTriggeredAnimation(powerup.identifier);

    }


    [HarmonyPatch(typeof(GameplayData), nameof(GameplayData.Symbol_CoinsValue_GetBasic))]
    public class Patch_Symbol_CoinsValue_GetBasic
    {
        static void Postfix(SymbolScript.Kind kind, ref int __result)
        {
            if (kind == SymbolScript.Kind.seven && PowerupScript.IsEquipped(cirnoCharmID))
            {
                __result += 2;
            }
        }
    }
}