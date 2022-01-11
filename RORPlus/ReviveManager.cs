using System;
using System.Linq;
using System.Collections.Generic;
using RoR2;
using UnityEngine;
using UnityEngine.Networking;

namespace RORPlus
{
    internal class ReviveManager
    {

        Dictionary<NetworkInstanceId, ReviveData> _reviveDataLookup;

        public ReviveManager()
        {
            _reviveDataLookup = new();
        }

        public void RegisterHooks()
        {
            On.RoR2.GlobalEventManager.OnPlayerCharacterDeath += OnPlayerCharacterDeath;
            On.RoR2.Run.AdvanceStage += OnAdvanceStage;
            On.RoR2.Run.BeginGameOver += OnBeginGameOver;
        }

        private void OnBeginGameOver(On.RoR2.Run.orig_BeginGameOver orig, Run self, GameEndingDef gameEndingDef)
        {
            Reset();
            orig(self, gameEndingDef);
        }

        private void OnAdvanceStage(On.RoR2.Run.orig_AdvanceStage orig, Run self, SceneDef nextScene)
        {
            Reset();
            orig(self, nextScene);
        }

        private void OnPlayerCharacterDeath(On.RoR2.GlobalEventManager.orig_OnPlayerCharacterDeath orig, GlobalEventManager self, DamageReport damageReport, NetworkUser victimNetworkUser)
        {
            Debug.Log($"{victimNetworkUser.netId} died to {damageReport.attacker} at {victimNetworkUser.GetCurrentBody().corePosition}");
            
            _reviveDataLookup.Add(victimNetworkUser.netId, new ReviveData(victimNetworkUser.netId, victimNetworkUser.GetCurrentBody().corePosition));
            orig(self, damageReport, victimNetworkUser);
        }

        private void Reset()
        {
            _reviveDataLookup.Clear();
        }

        public void AttemptRevive(PlayerCharacterMasterController reviver)
        {
            NetworkInstanceId reviverNetId = reviver.networkUser.netId;
            CharacterBody reviverBody = reviver.master.GetBody();
            
            if (IsPlayerDead(reviver))
            {
                Debug.Log($"[ReviveManager] {reviverNetId} cannot perform revive as player is dead");
                return;
            }

            if (reviverBody != null)
            {
                Debug.Log($"[ReviveManager] {reviverNetId} attempting to initiate revive at {reviver.master.GetBody().corePosition}");
            } else
            {
                Debug.LogError($"[ReviveManager] {reviverNetId} does not have body");
                return;
            }

            if (_reviveDataLookup.Count == 0)
            {
                Debug.Log("[ReviveManager] No players to revive");
                return;
            } else
            {
                Debug.Log($"[ReviveManager] DeathDataLookup: {string.Join("; ", _reviveDataLookup)}");
            }

            List<NetworkInstanceId> revivalCandidates = new();
            foreach (KeyValuePair<NetworkInstanceId, ReviveData> entry in _reviveDataLookup)
            {
                Debug.Log($"[ReviveManager] Checking revive eligibility of {entry.Key}");
                if (!entry.Value.CanRevive(reviver))
                {
                    Debug.Log($"[ReviveManager] {entry.Key} cannot be revived by {reviverNetId}");
                    continue;
                }
                
                Debug.Log($"[ReviveManager] {entry.Key} can be revived by {reviverNetId}");
                revivalCandidates.Add(entry.Key);
            }
        }

        private bool IsPlayerDead(PlayerCharacterMasterController player)
        {
            CharacterBody body = player.master.GetBody();

            return (body != null && !body.healthComponent.alive) || player.master.IsDeadAndOutOfLivesServer();
        }
    }
}
