using BepInEx;
using R2API;
using RoR2;

namespace RORPlus
{
    [BepInPlugin(PluginInfo.PLUGIN_GUID, PluginInfo.PLUGIN_NAME, PluginInfo.PLUGIN_VERSION)]
    public class RORPlus : BaseUnityPlugin
    {
        void Awake()
        {
            Logger.LogInfo($"Plugin {PluginInfo.PLUGIN_GUID} is loaded!");
        }

        void Start() {
            RoR2.Run.onRunStartGlobal += (run) =>
            {
                SetMinimumBaseJumpCount(2);
            };
        }

        private void SetMinimumBaseJumpCount(int count) {
            foreach (SurvivorDef survivor in RoR2.SurvivorCatalog.allSurvivorDefs)
            {
                CharacterBody body = survivor.bodyPrefab.GetComponent<CharacterBody>();
                var actualCount = count;
                if (survivor.cachedName.Equals("Merc"))
                {
                    // Ensure that Merc has one more jump than all other survivors
                    actualCount += 1;
                }
                if (body.baseJumpCount <= actualCount)
                {
                    // Prevent new base jump count being smaller than existing base for survivor
                    body.baseJumpCount = actualCount;

                }
                Logger.LogInfo($"baseJumpCount of {survivor.cachedName} is {body.baseJumpCount}");
            }
        }
    }
}