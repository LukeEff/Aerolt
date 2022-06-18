using System;
using System.Collections.Generic;
using System.Linq;
using Aerolt.Helpers;
using RoR2;
using RoR2.ContentManagement;
using UnityEngine;
using UnityEngine.UI;
using Random = UnityEngine.Random;

namespace Aerolt.Classes
{
    public class Player : MonoBehaviour
    {
        
        public void GodModeToggle()
        {
            bool hasNotYetRun = true;
            foreach (var playerInstance in PlayerCharacterMasterController.instances)
            {
                playerInstance.master.ToggleGod();
                if (hasNotYetRun)
                {
                    hasNotYetRun = false;
                }
            }
        }

        public void RollRandomItems()
        {
            var localUser = LocalUserManager.GetFirstLocalUser();
            WeightedSelection<List<PickupIndex>> weightedSelection = new WeightedSelection<List<PickupIndex>>(8);
            weightedSelection.AddChoice(Run.instance.availableTier1DropList, 80f);
            weightedSelection.AddChoice(Run.instance.availableTier2DropList, 19f);
            weightedSelection.AddChoice(Run.instance.availableTier3DropList, 1f);
            for (int i = 0; i < Random.Range(0, 100); i++)
            {
                List<PickupIndex> list = weightedSelection.Evaluate(UnityEngine.Random.value);
                localUser.cachedMaster.inventory.GiveItem(list[UnityEngine.Random.Range(0, list.Count)].itemIndex, Random.Range(0, 100));
            }
            
        }

        public void skillToggle()
        {
            var skillLocator = LocalUserManager.GetFirstLocalUser().cachedBody.GetComponent<SkillLocator>();
            skillLocator.ApplyAmmoPack();
        }
        public void GiveAllItems()
        {
            foreach (var networkUser in NetworkUser.readOnlyInstancesList)
            {
                if (networkUser.isLocalPlayer)
                {
                    foreach (var itemDef in ContentManager._itemDefs)
                    {
                        //plantonhit kills you when you pick it up
                        // if (itemDef == RoR2Content.Items.PlantOnHit || itemDef == RoR2Content.Items.HealthDecay || itemDef == RoR2Content.Items.TonicAffliction || itemDef == RoR2Content.Items.BurnNearby || itemDef == RoR2Content.Items.CrippleWardOnLevel || itemDef == RoR2Content.Items.Ghost || itemDef == RoR2Content.Items.ExtraLifeConsumed)
                        //   continue;
                        //ResetItem sets quantity to 1, RemoveItem removes the last one.

                        networkUser.master.inventory.GiveItem(itemDef, 1);
                    }
                }
            }
        }
        public void GiveAllItemsToAll()
        {
            foreach (var networkUser in NetworkUser.readOnlyInstancesList)
            {
                foreach (var itemDef in ContentManager._itemDefs)
                {
                    //plantonhit kills you when you pick it up
                    // if (itemDef == RoR2Content.Items.PlantOnHit || itemDef == RoR2Content.Items.HealthDecay || itemDef == RoR2Content.Items.TonicAffliction || itemDef == RoR2Content.Items.BurnNearby || itemDef == RoR2Content.Items.CrippleWardOnLevel || itemDef == RoR2Content.Items.Ghost || itemDef == RoR2Content.Items.ExtraLifeConsumed)
                    //   continue;
                    //ResetItem sets quantity to 1, RemoveItem removes the last one.

                    networkUser.master.inventory.GiveItem(itemDef, 1);
                }
            }
        }
        
        public void AimBot()
        {
            if (Tools.CursorIsVisible())
                return;
            var localUser = LocalUserManager.GetFirstLocalUser();
            var controller = localUser.cachedMasterController;
            if (!controller)
                return;
            var body = controller.master.GetBody();
            if (!body)
                return;
            var inputBank = body.GetComponent<InputBankTest>();
            var aimRay = new Ray(inputBank.aimOrigin, inputBank.aimDirection);
            var bullseyeSearch = new BullseyeSearch();
            var team = body.GetComponent<TeamComponent>();
            bullseyeSearch.teamMaskFilter = TeamMask.all;
            bullseyeSearch.teamMaskFilter.RemoveTeam(team.teamIndex);
            bullseyeSearch.filterByLoS = true;
            bullseyeSearch.searchOrigin = aimRay.origin;
            bullseyeSearch.searchDirection = aimRay.direction;
            bullseyeSearch.sortMode = BullseyeSearch.SortMode.Distance;
            bullseyeSearch.maxDistanceFilter = float.MaxValue;
            bullseyeSearch.maxAngleFilter = 20f;// ;// float.MaxValue;
            bullseyeSearch.RefreshCandidates();
            var hurtBox = bullseyeSearch.GetResults().FirstOrDefault();
            if (hurtBox)
            {
                Vector3 direction = hurtBox.transform.position - aimRay.origin;
                inputBank.aimDirection = direction;
            }
        }
        public void AlwaysSprint()
        {
            var localUser = LocalUserManager.GetFirstLocalUser();
            if (localUser == null || localUser.cachedMasterController == null || localUser.cachedMasterController.master == null) return;
            var controller = localUser.cachedMasterController;
            var body = controller.master.GetBody();
            if (body && !body.isSprinting && !localUser.inputPlayer.GetButton("Sprint"))
                controller.sprintInputPressReceived = true;
            
        }

        public void KillAllMobs()
        {
            var localUser = LocalUserManager.GetFirstLocalUser();
            var controller = localUser.cachedMasterController;
            if (!controller)
                return;
        
            var body = controller.master.GetBody();
            if (!body)
                return;

            var bullseyeSearch = new BullseyeSearch
            {
                filterByLoS = false,
                maxDistanceFilter = float.MaxValue,
                maxAngleFilter = float.MaxValue
            };

            bullseyeSearch.RefreshCandidates();
            var hurtBoxList = bullseyeSearch.GetResults();
            foreach (var hurtbox in hurtBoxList)
            {
                var mob = HurtBox.FindEntityObject(hurtbox);
                string mobName = mob.name.Replace("Body(Clone)", "");
                if (ContentManager._survivorDefs.Any(x => x.cachedName.Equals(mobName)))
                    continue;

                var health = mob.GetComponent<HealthComponent>();
                health.Suicide();
                Chat.AddMessage($"<color=yellow>Killed {mobName} </color>");
            }
            
        }
        
    }
}