using System.Collections.Generic;
using RoR2;
using UnityEngine;
using UnityEngine.Networking;
using R2API.Networking;
using R2API.Networking.Interfaces;

#nullable enable

namespace RORPlus
{
    internal class ReviveManager
    {

        static Dictionary<NetworkInstanceId, ReviveData> _reviveDataLookup = new();

        public static void RegisterHooks()
        {
            On.RoR2.GlobalEventManager.OnPlayerCharacterDeath += OnPlayerCharacterDeath;
            On.RoR2.Run.AdvanceStage += OnAdvanceStage;
            On.RoR2.Run.BeginGameOver += OnBeginGameOver;
            On.RoR2.Run.OnUserRemoved += OnUserRemoved; 
        }

        private static void OnBeginGameOver(On.RoR2.Run.orig_BeginGameOver orig, Run self, GameEndingDef gameEndingDef)
        {
            Reset();
            orig(self, gameEndingDef);
        }

        private static void OnAdvanceStage(On.RoR2.Run.orig_AdvanceStage orig, Run self, SceneDef nextScene)
        {
            Reset();
            orig(self, nextScene);
        }

        private static void OnPlayerCharacterDeath(On.RoR2.GlobalEventManager.orig_OnPlayerCharacterDeath orig, GlobalEventManager self, DamageReport damageReport, NetworkUser victimNetworkUser)
        {
            _reviveDataLookup.Add(victimNetworkUser.netId, new ReviveData(victimNetworkUser.netId, victimNetworkUser.GetCurrentBody().corePosition));

            RLogger.LogInfo($"{N(victimNetworkUser)} died to {damageReport.attacker} at {victimNetworkUser.GetCurrentBody().corePosition}");
            RLogger.LogInfo($"DeathDataLookup: {string.Join("; ", _reviveDataLookup)}");

            orig(self, damageReport, victimNetworkUser);
        }

        private static void OnUserRemoved(On.RoR2.Run.orig_OnUserRemoved orig, Run self, NetworkUser user)
        {
            _reviveDataLookup.Remove(user.netId);
            orig(self, user);
        }

        private static void Reset()
        {
            _reviveDataLookup.Clear();
        }

        public static void SendPerfomReviveMessage(NetworkUser reviver)
        {
            RLogger.LogInfo($"Sending PerformReviveMessage from {N(reviver)}");
            PerformReviveMessage message = new(reviver);
            message.Send(NetworkDestination.Server);
        }
        
        public static void AttemptRevive(NetworkInstanceId reviverNetId, Vector3 reviverPosition)
        {
            if (!NetworkServer.active)
            {
                RLogger.LogError("Non-host machine cannot perform revive. Aborting");
                return;
            }

            NetworkUser? reviver = ResolveNetworkUserFromNetworkId(reviverNetId);
            
            if (reviver == null)
            {
                RLogger.LogError($"Could not resolve reviver from NetId={reviverNetId}");
                return;
            }

            CharacterMaster reviverMaster = reviver.master;

            if (reviverMaster.IsDeadAndOutOfLivesServer())
            {
                RLogger.LogInfo($"{N(reviver)} cannot perform revive as player is dead");
                return;
            }

            RLogger.LogInfo($"{N(reviver)} attempting to initiate revive at {reviverPosition}");

            if (_reviveDataLookup.Count == 0)
            {
                RLogger.LogInfo("No players to revive");
                return;
            } else
            {
                RLogger.LogInfo($"DeathDataLookup: {string.Join("; ", _reviveDataLookup)}");
            }

            List<ReviveData> revivalCandidates = GetRevivalCandidates(reviver, reviverPosition);
            if (revivalCandidates.Count == 0)
            {
                RLogger.LogInfo($"No players are eligible for revival by {N(reviver)}");
                return;
            }
            ReviveData target = SelectTargetFromRevivalCandidates(revivalCandidates);
            bool success = PerformRevive(target);

            if (success)
            {
                _reviveDataLookup.Remove(target.NetId);
            }
        }
        
        // Returns a list of ReviveData instances corresponding to the players which are eligibile to be revived by the given player.
        private static List<ReviveData> GetRevivalCandidates(NetworkUser reviverNetUser, Vector3 reviverPosition)
        {
            List<ReviveData> revivalCandidates = new();
            foreach (KeyValuePair<NetworkInstanceId, ReviveData> entry in _reviveDataLookup)
            {
                RLogger.LogInfo($"Checking revive eligibility of {entry.Key}");
                if (!entry.Value.CanRevive(reviverPosition))
                {
                    RLogger.LogInfo($"{entry.Key} cannot be revived by {N(reviverNetUser)}");
                    continue;
                }

                RLogger.LogInfo($"{entry.Key} can be revived by {N(reviverNetUser)}");
                revivalCandidates.Add(entry.Value);
            }
            return revivalCandidates;
        }

        private static ReviveData SelectTargetFromRevivalCandidates(List<ReviveData> revivalCandidates)
        {
            // TODO Select target based on proximity to reviver and/or other criteria (e.g. first downed)
            return revivalCandidates[0];
        }

        // Revives player corresponding to given ReviveData instance, and returns true if revive is successful. Otherwise, returns false.
        private static bool PerformRevive(ReviveData reviveData)
        {
            NetworkUser? networkUser = ResolveNetworkUserFromNetworkId(reviveData.NetId);
            if (networkUser == null)
            {
                RLogger.LogInfo($"NetworkUser with NetId={reviveData.NetId} could not be found");
                return false;
            }

            RLogger.LogInfo($"Revived {N(networkUser)}");
            networkUser.master.RespawnExtraLife();
            
            return true; 
        }

        private static NetworkUser? ResolveNetworkUserFromNetworkId(NetworkInstanceId netId)
        {
            GameObject gameObject = Util.FindNetworkObject(netId);
            if (!gameObject)
            {
                return null;
            }
            
            return gameObject.GetComponent<NetworkUser>();
        }

        private static string N(NetworkUser user)
        {
            return $"{user.userName} (NetId={user.netId})";
        }
    }
}
