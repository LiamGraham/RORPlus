using BepInEx;
using R2API;
using RoR2;
using UnityEngine;
using UnityEngine.Networking;

namespace RORPlus
{
    [BepInPlugin(PluginInfo.PLUGIN_GUID, PluginInfo.PLUGIN_NAME, PluginInfo.PLUGIN_VERSION)]
    public class RORPlus : BaseUnityPlugin
    {
        ReviveManager reviveManager;

        void Awake()
        {
            Logger.LogInfo($"Plugin {PluginInfo.PLUGIN_GUID} is loaded!");
            reviveManager = new ReviveManager();
            
            ConfigManager.InitialiseConfig(Config);
            RegisterAllHooks();
        }

        void Update()
        {
            if (!ConfigManager.ModIsEnabled.Value)
            {
                return;
            }

            if (Input.GetKeyDown(ConfigManager.PerformReviveKey.Value))
            {
                reviveManager.AttemptRevive(PlayerCharacterMasterController.instances[0]);
            }
        }
        public void RegisterAllHooks()
        {
            RegisterHooks();
            SurvivorModifier.RegisterHooks();
            reviveManager.RegisterHooks();
        }

        private void RegisterHooks()
        {
            On.RoR2.Run.OnUserAdded += OnUserAdded;
            On.RoR2.Run.OnServerCharacterBodySpawned += OnServerCharacterBodySpawned;
        }

        private void OnServerCharacterBodySpawned(On.RoR2.Run.orig_OnServerCharacterBodySpawned orig, Run self, CharacterBody characterBody)
        {
            if (characterBody.master.playerCharacterMasterController != null)
            {
                PlayerCharacterMasterController player = characterBody.master.playerCharacterMasterController;

                Debug.Log($"[RORPlus] CharacterBody spawned with PlayerCharacterMasterController: NetworkId={player.networkUser.netId}");
            }
            orig(self, characterBody);
        }

        private void OnUserAdded(On.RoR2.Run.orig_OnUserAdded orig, Run self, NetworkUser user)
        {
            Debug.Log($"[RORPlus] NetworkUser added: NetworkID={user.netId}, UserName={user.userName}");
            orig(self, user); 
        }
    }
}