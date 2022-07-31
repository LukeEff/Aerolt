using System.Collections.Generic;
using System.Linq;
using Aerolt.Buttons;
using Aerolt.Helpers;
using Aerolt.Messages;
using BepInEx;
using RoR2;
using TMPro;
using UnityEngine;

namespace Aerolt.Managers
{
    public class BodyManager : MonoBehaviour
    {

        public GameObject buttonPrefab;
        public GameObject buttonParent;
        
        private GameObject _newBody;
        private NetworkUser target;
        public TMP_InputField searchFilter;
        private Dictionary<CharacterBody, CustomButton> bodyDefRef = new();

        private void Awake()
        {
            foreach(var body in BodyCatalog.allBodyPrefabBodyBodyComponents)
            {
                GameObject newButton = Instantiate(buttonPrefab, buttonParent.transform);
                var customButton = newButton.GetComponent<CustomButton>(); 
                customButton.buttonText.text = Language.GetString(body.baseNameToken);
                customButton.image.sprite = Sprite.Create((Texture2D)body.portraitIcon, new Rect(0, 0, body.portraitIcon.width, body.portraitIcon.height), new Vector2(0.5f, 0.5f));
                customButton.button.onClick.AddListener(() => SetBodyDef(body));
                bodyDefRef[body] = customButton;
            }
            if(searchFilter)
                searchFilter.onValueChanged.AddListener(FilterUpdated);
        }

        public void SpawnAsBody()
        {
            new SetBodyMessage(target, _newBody.GetComponent<CharacterBody>()).SendToServer();
        }

        public void SetBodyDef(CharacterBody body)
        {
            _newBody = BodyCatalog.FindBodyPrefab(body);
            SpawnAsBody();
            GetComponentInParent<LobbyPlayerPageManager>().SwapViewState();
        }

        public void Initialize(NetworkUser currentUser)
        {
            target = currentUser;
        }
        private void FilterUpdated(string text)
        {
            if (text.IsNullOrWhiteSpace())
            {
                foreach (var buttonGen in bodyDefRef)
                {
                    buttonGen.Value.gameObject.SetActive(true);
                }
                return;
            }
            
            var arr = bodyDefRef.Values.ToArray();
            var matches = Tools.FindMatches(arr, x => x.buttonText.text, text);
            foreach (var buttonGen in arr)
            {
                buttonGen.gameObject.SetActive(matches.Contains(buttonGen));
            }
        }
    }
    
}