using R2API.Networking.Interfaces;
using UnityEngine.Networking;
using UnityEngine;
using RoR2;

namespace RORPlus
{
    internal class PerformReviveMessage : INetMessage
    {
        NetworkInstanceId reviverNetId;
        Vector3 reviverPosition;

        public PerformReviveMessage()
        {

        }
        
        public PerformReviveMessage(NetworkUser reviver)
        {
            reviverNetId = reviver.netId;

            CharacterBody reviverBody = reviver.master.GetBody();
            reviverPosition = reviverBody != null ? reviverBody.corePosition : Vector3.negativeInfinity;
        }

        public void OnReceived()
        {
            ReviveManager.AttemptRevive(reviverNetId, reviverPosition);
        }

        public void Serialize(NetworkWriter writer)
        {
            writer.Write(reviverNetId);
            writer.Write(reviverPosition);
        }
        
        public void Deserialize(NetworkReader reader)
        {
            reviverNetId = reader.ReadNetworkId();
            reviverPosition = reader.ReadVector3();
        }
    }
}
