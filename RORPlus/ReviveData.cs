using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using UnityEngine.Networking;
using RoR2;
using R2API.Networking;

namespace RORPlus
{
    internal class ReviveData
    {
        private NetworkInstanceId _netId;
        private Vector3 _revivePosition;


        public ReviveData(NetworkInstanceId netId, Vector3 position)
        {
            _netId = netId;
            _revivePosition = GetGroundPositionFromPosition(position);
        }

        public Vector3 RevivePosition 
        {
            get => _revivePosition;
        }

        public NetworkInstanceId NetId
        {
            get => _netId;
        }

        public bool CanRevive(Vector3 reviverPosition)
        {
            float distanceToTarget = Vector3.Distance(reviverPosition, _revivePosition);
            RLogger.LogInfo($"Distance to revive target: {distanceToTarget}");
            
            return distanceToTarget <= ConfigManager.ReviveRange.Value;
        }

        private static Vector3 GetGroundPositionFromPosition(Vector3 position)
        {
            return position;
        }

        public override string ToString()
        {
            return $"ReviveData(netId={_netId}, revivePosition={_revivePosition})";
        }

    }
}
