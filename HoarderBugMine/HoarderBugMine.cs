using System.IO;
using System.Reflection;
using BepInEx;
using BepInEx.Logging;
using HarmonyLib;
using UnityEngine;
using File = UnityEngine.IO.File;

namespace HoarderBugMine
{
    [BepInPlugin(PluginInfo.PLUGIN_GUID, PluginInfo.PLUGIN_NAME, PluginInfo.PLUGIN_VERSION)]
    public class MineMineMineBase : BaseUnityPlugin
    {
        private readonly Harmony _harmony = new Harmony(PluginInfo.PLUGIN_GUID);
        private static MineMineMineBase? _instance;
        private static ManualLogSource? mls;
        private void Awake()
        {
            if (_instance == null)
                _instance = this;
            
            mls = BepInEx.Logging.Logger.CreateLogSource("HoarderBugMine mod");
            mls.LogInfo($"Plugin {PluginInfo.PLUGIN_GUID} is loading");

            mls.LogInfo("loading assets");
            Assets.LoadAssets();
            mls.LogInfo("assets loaded");
            
            _harmony.PatchAll(typeof(MineMineMineBase));
            _harmony.PatchAll(typeof(HoarderBugAudioPatch));
            // _harmony.PatchAll(typeof(HoarderBugModelPatch));
            mls.LogInfo($"{PluginInfo.PLUGIN_GUID} is now loaded");
        }
        
        [HarmonyPatch(typeof(HoarderBugAI))]
        internal class HoarderBugAudioPatch
        {
            [HarmonyPatch("Start")]
            [HarmonyPostfix]
            public static void PatchAudio(ref AudioClip[] ___chitterSFX)
            {
                AudioClip[] newAudio = Assets.SeagullSound;
                ___chitterSFX = newAudio;
            }
        }

        private class Assets
        {
            private static AssetBundle _bundle;
            internal static AudioClip[] SeagullSound;
            // internal static GameObject SeagullPrefab;
            // internal static Transform[] SeagullAnimations;
            
            internal static void LoadAssets()
            {
                string bundlePath =
                    MineMineMineBase._instance.Info.Location.TrimEnd(Assembly.GetExecutingAssembly().FullName
                        .ToCharArray()) + "minemine";
                _bundle = AssetBundle.LoadFromFile(bundlePath);
                SeagullSound = _bundle.LoadAssetWithSubAssets<AudioClip>("assets/mineMineMine.mp3");
                // seagullPrefab = assetBundle.LoadAsset<GameObject>("Assets/Seagull.prefab");
            }
        }
    }
}

