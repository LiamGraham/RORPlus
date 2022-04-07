using BepInEx;
using RoR2;
using UnityEngine;
using R2API.Networking;
using R2API.Utils;
using System.Collections.Generic;

namespace RORPlus
{
    [R2APISubmoduleDependency(nameof(NetworkingAPI))]
    [BepInPlugin(PluginInfo.PLUGIN_GUID, PluginInfo.PLUGIN_NAME, PluginInfo.PLUGIN_VERSION)]
    public class RORPlus : BaseUnityPlugin
    {
        private static bool godModeEnabled = false;

        void Awake()
        {
            CommandHelper.AddToConsoleWhenReady();

            RLogger.logger = Logger;
            RLogger.LogInfo($"Plugin {PluginInfo.PLUGIN_GUID} is loaded!");
            
            ConfigManager.InitialiseConfig(Config);
            RegisterAllHooks();
            RegisterMessageTypes();
        }

        void Update()
        {
            if (!ConfigManager.ModIsEnabled.Value && RoR2.Run.instance == null)
            {
                return;
            }

            if (Input.GetKeyDown(ConfigManager.PerformReviveKey.Value))
            {
                ReviveManager.SendPerfomReviveMessage(LocalUserManager.GetFirstLocalUser().currentNetworkUser);
            }
        }
        public void RegisterAllHooks()
        {
            RegisterHooks();
            SurvivorModifier.RegisterHooks();
            ReviveManager.RegisterHooks();
        }

        private void RegisterHooks()
        {
            RLogger.LogWarning("Self-connection enabled. Remove for non-local testing");
            On.RoR2.Networking.GameNetworkManager.OnClientConnect += (s, u, t) => { };

            On.RoR2.Run.OnUserAdded += OnUserAdded;
            On.RoR2.Run.OnServerCharacterBodySpawned += OnServerCharacterBodySpawned;
            On.RoR2.HealthComponent.TakeDamage += OnTakeDamage;
        }

        private void RegisterMessageTypes()
        {
            NetworkingAPI.RegisterMessageType<PerformReviveMessage>();
        }

        private void OnServerCharacterBodySpawned(On.RoR2.Run.orig_OnServerCharacterBodySpawned orig, Run self, CharacterBody characterBody)
        {
            if (characterBody.master.playerCharacterMasterController != null)
            {
                PlayerCharacterMasterController player = characterBody.master.playerCharacterMasterController;

                RLogger.LogInfo($"CharacterBody spawned with PlayerCharacterMasterController: NetworkId={player.networkUser.netId}");
            }
            orig(self, characterBody);
        }

        private void OnUserAdded(On.RoR2.Run.orig_OnUserAdded orig, Run self, NetworkUser user)
        {
            RLogger.LogInfo($"NetworkUser added: NetworkID={user.netId}, UserName={user.userName}");
            orig(self, user); 
        }
        
        private void OnTakeDamage(On.RoR2.HealthComponent.orig_TakeDamage orig, HealthComponent self, DamageInfo damageInfo)
        {
            CharacterBody damagedBody = self.GetComponent<CharacterBody>();
            CharacterBody localUserBody = LocalUserManager.GetFirstLocalUser().cachedBody;

            if (godModeEnabled && damagedBody != null && damagedBody.Equals(localUserBody))
            {
                return;
            }
            orig(self, damageInfo);
        }

        [ConCommand(commandName = "god_mode", flags = ConVarFlags.None, helpText = "Sets if god mode is enabled")]
        private static void CcSetGodMode(ConCommandArgs args)
        {
            if (args.Count != 1)
            {
                Debug.Log("god_mode [enabled]");
                return;
            }
            bool? enabled = Utils.TryGetBool(args[0]);
            if (!enabled.HasValue)
            {
                Debug.Log($"{args[0]} cannot be parsed as boolean");
            }

            godModeEnabled = enabled.Value;
            Debug.Log($"God mode {(enabled.Value ? "enabled" : "disbled")}");
        }
    }
}