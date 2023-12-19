using BepInEx;
using BepInEx.Logging;
using HarmonyLib;
using UnityEngine;
using Object = System.Object;

namespace HoarderBugMine
{
    [BepInPlugin(PluginInfo.PLUGIN_GUID, PluginInfo.PLUGIN_NAME, PluginInfo.PLUGIN_VERSION)]
    public class MineMineMineMod : BaseUnityPlugin
    {
        private readonly Harmony _harmony = new Harmony(PluginInfo.PLUGIN_GUID);
        private static MineMineMineMod? _instance;
        internal ManualLogSource? mls;
        internal static AudioClip[] newSFX;
        private void Awake()
        {
            if (_instance == null)
                _instance = this;
            
            mls = BepInEx.Logging.Logger.CreateLogSource("HoarderBugMine mod");
            mls.LogInfo($"Plugin {PluginInfo.PLUGIN_GUID} is loading!");
            
            string location = _instance.Info.Location;
            string folder = location.TrimEnd("HoarderBugMine.dll".ToCharArray());
            string audio = folder + "minemine";
            AssetBundle bundle = AssetBundle.LoadFromFile(audio);
            
            if ( bundle == null)
            {
                mls.LogError("Failed to load asset bundle");
                return;
            }
            
            newSFX = bundle.LoadAssetWithSubAssets<AudioClip>("assets/mineMineMine.mp3");
            
            _harmony.PatchAll(typeof(HoarderBugPatch));
            mls.LogInfo($"{PluginInfo.PLUGIN_GUID} is now loaded");
        }
        
        
        [HarmonyPatch(typeof(HoarderBugAI))]
        internal class HoarderBugPatch
        {
            [HarmonyPatch("Start")]
            [HarmonyPostfix]
            public static void hoarderBugAudioPatch(ref AudioClip[] ___chitterSFX)
            {
                AudioClip[] newSFX = MineMineMineMod.newSFX;
                ___chitterSFX = newSFX;
            }
        }
    }
}

