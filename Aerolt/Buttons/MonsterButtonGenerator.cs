﻿using System;
using System.Collections.Generic;
using System.Linq;
using Aerolt.Helpers;
using Aerolt.Managers;
using RoR2;
using RoR2.ContentManagement;
using RoR2.UI;
using TMPro;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.Serialization;
using UnityEngine.UI;

namespace Aerolt.Buttons
{
    public class MonsterButtonGenerator : MonoBehaviour
    {
        public GameObject buttonPrefab;
        public GameObject buttonParent;

        public TMP_Dropdown teamIndexDropDown;
        
        public TMP_Text monsterText;
        private CharacterBody _body;
        private CharacterMaster _master;
        
        public static Dictionary<ItemDef, int> ItemDef = new Dictionary<ItemDef, int>();
        private List<string> options = new List<string>();
        private void Awake()
        {
            foreach (var master in MasterCatalog.allAiMasters)
            {
                var body = master.bodyPrefab.GetComponent<CharacterBody>();
                
                GameObject newButton = Instantiate(buttonPrefab, buttonParent.transform);
                newButton.GetComponent<CustomButton>().ButtonText.text = Language.GetString(master.name);
                newButton.GetComponent<Image>().sprite = Sprite.Create((Texture2D)body.portraitIcon, new Rect(0, 0, body.portraitIcon.width, body.portraitIcon.height), new Vector2(0.5f, 0.5f));
                newButton.GetComponent<Button>().onClick.AddListener(() => SetMonsterName(body,master));
            }
            foreach (string team in Enum.GetNames(typeof(TeamIndex))) options.Add(team);
            teamIndexDropDown.AddOptions(options);

        }

        public void SetMonsterName(CharacterBody body, CharacterMaster master)
        {
            monsterText.text = Language.GetString(body.baseNameToken);
            _master = master;
            _body = body;
            
        }
        public void SpawnMonster(int teamIndex)
        {
            var localUser = GetUser.FetchUser(GetComponentInParent<PanelManager>().hud);
            if(localUser == null || !localUser.cachedBody)
                return;
            
            var location = localUser.cachedBody.transform.position;
            var body = _master.GetComponent<CharacterMaster>().bodyPrefab;

            var bodyGameObject = UnityEngine.Object.Instantiate<GameObject>(_master.gameObject, location, Quaternion.identity);
            var master = bodyGameObject.GetComponent<CharacterMaster>();

            master.teamIndex = (TeamIndex) teamIndexDropDown.value;
            
            if (ItemDef.Any())
                foreach (var item in ItemDef)
                    master.inventory.GiveItem(item.Key, item.Value);
            

            NetworkServer.Spawn(bodyGameObject);
            master.bodyPrefab = body;
            master.SpawnBody(localUser.cachedBody.transform.position, Quaternion.identity);
            
        }
        
    }
}