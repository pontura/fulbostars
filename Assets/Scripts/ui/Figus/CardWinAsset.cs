using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Fulbo.UI
{
    public class CardWinAsset : MonoBehaviour
    {
        public Image thumb;
        public Animation anim;
        public int characterID;
        public bool isGoalKeeper;
        CardsWinScreen manager;
        public bool isDone;
        public Text field;
        [SerializeField] CardAsset cardAsset;
        float speed = 10;
        public void InitPlayer(CardsWinScreen manager, int characterID, bool isGoalKeeper, int totalStats, int uniqueID)
        {
            this.manager = manager;
            this.characterID = characterID;
            this.isGoalKeeper = isGoalKeeper;
            CharactersData.CharacterData cData = CharactersData.Instance.GetCharacterData(characterID, isGoalKeeper);
            cData.uniqueID = uniqueID;
            cardAsset.Init(cData, totalStats);
            cardAsset.gameObject.SetActive(false);
        }
        private void Update()
        {
            transform.localPosition = Vector2.Lerp(transform.localPosition, Vector2.zero, Time.deltaTime*speed);
        }
        public void TurnAutomatically()
        {
            TurnReal();
        }
        public void Turn() // by click
        {
            //TurnReal();
        }
        public void TurnReal()
        {
            if (isDone) return;
            isDone = true;
            anim.Play("turn");
            StartCoroutine(Turn(this));
        }
        public IEnumerator Turn(CardWinAsset c)
        {
            AudioManager.Instance.PlaySoundOneShot("ui", "/ui/cards/ui_turn_card");
            yield return new WaitForSeconds(0.2f);
            Events.SayCharacterName(characterID, isGoalKeeper);
            yield return new WaitForSeconds(1f);
            anim.Play("idle");
            manager.CheckReady();
            yield return null;
        }

        public void CardEnter() {
            anim.Play("enter");
        }
    }
}