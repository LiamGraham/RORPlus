using System;
using System.Collections.Generic;
using System.Text;
using RoR2;

namespace RORPlus
{
    internal class SurvivorModifier
    {
        public static void RegisterHooks()
        {
            RoR2Application.onLoad += AdjustBaseJumpCount;
        }

        public static void AdjustBaseJumpCount()
        {
            foreach (SurvivorDef survivor in SurvivorCatalog.allSurvivorDefs)
            {
                survivor.bodyPrefab.GetComponent<CharacterBody>().baseJumpCount += ConfigManager.BaseJumpCountModifier.Value;
            }
        }
    }
}
