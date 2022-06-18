using System.Collections.Generic;
using RoR2;
using RoR2.ContentManagement;
using UnityEngine;

namespace Aerolt.Buttons
{
    public class EditPlayerItemButton : MonoBehaviour
    {
        public GameObject buttonPrefab;
        public GameObject buttonParent;

        public GameObject itemListParent;
        
        private Dictionary<ItemDef, int> itemDef = new();
        private Dictionary<ItemDef, InventoryItemAddRemoveButtonGen> itemDefRef = new();
        private NetworkUser user;

        public void Awake()
        {
            foreach (var def in ContentManager._itemDefs)
                itemDefRef[def] = new InventoryItemAddRemoveButtonGen(def, buttonPrefab, itemDef, buttonParent, itemListParent, false);
        }

        public void Initialize(NetworkUser userIn)
        {
            user = userIn;
            foreach (var def in ContentManager._itemDefs)
            {
                itemDefRef[def].SetAmount(user.master.inventory.GetItemCount(def));
            }
        }

        public void GiveItems()
        {
            var inv = user.master.inventory;
            foreach (var (key, value) in itemDef)
            {
                inv.GiveItem(key, value - inv.GetItemCount(key));
            }
        }
    }
}