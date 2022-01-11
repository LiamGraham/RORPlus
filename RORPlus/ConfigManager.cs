using BepInEx;
using BepInEx.Configuration;
using UnityEngine;

namespace RORPlus
{
    internal class ConfigManager
    {
        public static ConfigEntry<bool>
            ModIsEnabled,
            RevivesEnabled;

        public static ConfigEntry<float>
            ReviveTime,
            ReviveRange;

        public static ConfigEntry<int>
            BaseJumpCountModifier;

        public static ConfigEntry<KeyCode>
            PerformReviveKey;

        public static void InitialiseConfig(ConfigFile config)
        {
            ModIsEnabled = config.Bind(
                "General",
                "ModIsEnabled",
                true,
                "Whether or not the mod is enabled."
            );

            RevivesEnabled = config.Bind(
                "Reviving",
                "RevivesEnabled",
                true,
                "Whether or not dead players can be revived."
            );

            ReviveTime = config.Bind(
                "Reviving",
                "ReviveTime",
                5.0f,
                "Time in seconds required to revive a dead player"
            );

            ReviveRange = config.Bind(
                "Reviving",
                "ReviveRange",
                5.0f,
                "Maximum distance to dead player at which another player cannot initiate a revive."
            );
            
            PerformReviveKey = config.Bind(
                "Key Mappings",
                "PerformReviveKey",
                KeyCode.E,
                "Key to perform revive when within revive range"
            );

            BaseJumpCountModifier = config.Bind(
                "Survivors",
                "BaseJumpCountModifier",
                1,
                "Amount by which the base jump counts for all survivors is increased."
            );            
        }
    }
}
