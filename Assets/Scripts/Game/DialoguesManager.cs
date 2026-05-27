using Fulbo.Game;
using Fulbo.UI;
using System.Collections.Generic;
using UnityEngine;
using Fulbo.Onboarding;
using System;

namespace Fulbo
{
    public class DialoguesManager : MonoBehaviour
    {
        public CharacterDialogueUI characterDialogueUI_to_instantiate;
        public List<CharacterDialogueUI> all;
        public Transform container;
        float random_x;
        float random_y;
        public bool randomMessagesOn = true;

        private void Start()
        {
            if (Data.Instance.mode != Data.modes.PARTYMODE && (
                Fulbo.Game.GameManager.Instance == null || Fulbo.Game.GameManager.Instance.isTutorial))
                return;
            Events.SetDialogue += SetDialogue;
            Events.CharacterCatchBall += OnCatch;
        }
        public void Init()
        {
            if (randomMessagesOn && Fulbo.Game.GameManager.Instance != null)
            {
                Invoke(nameof(LoopRandomDialogues), 5);
                Invoke(nameof(InitReferi), 0.1f);
            }
            random_x = Data.Instance.settings.GetSetting("dialoguesTimeToAppear_x");
            random_y = Data.Instance.settings.GetSetting("dialoguesTimeToAppear_y");
        }

        private void OnCatch(Character character)
        {
          //  string text = Data.Instance.textsData.GetRandomDialogue("random", character.data.id, character.type == Character.types.GOALKEEPER);
          //  Events.SetDialogue(character, text);
        }

        private void InitReferi()
        {
            SetReferi("init");
        }
        void OnDestroy()
        {
            Events.SetDialogue -= SetDialogue;
            Events.CharacterCatchBall -= OnCatch;
        }

        void LoopRandomDialogues()
        {
            Invoke(nameof(LoopRandomDialogues), UnityEngine.Random.Range(random_x, random_y));
            Character character = null;

            if (UnityEngine.Random.Range(0, 10) < 5 && Fulbo.Game.GameManager.Instance.charactersManager.team1.Count > 0)
                character = Fulbo.Game.GameManager.Instance.charactersManager.team1[UnityEngine.Random.Range(0, Fulbo.Game.GameManager.Instance.charactersManager.team1.Count)];
            else if (Fulbo.Game.GameManager.Instance.charactersManager.team2.Count > 0)
                character = Fulbo.Game.GameManager.Instance.charactersManager.team2[UnityEngine.Random.Range(0, Fulbo.Game.GameManager.Instance.charactersManager.team2.Count)];

            if (character == null) return;
            if (character.ballCatcher.HasBall()) return;

            string text = Data.Instance.textsData.GetRandomDialogue("random", character.data.id, character.type == Character.types.GOALKEEPER);

            if (text != "")
                Events.SetDialogue(character, text);

            int referiRandom = UnityEngine.Random.Range(0, 10);
            if (referiRandom < 3)
                SetReferi("random");
            else if (referiRandom < 4)
            {
                SetReferi("full");
            }
        }
        void SetReferi(string type)
        {
            string text = Data.Instance.textsData.GetRandomReferiDialogue(type);
            Events.SetDialogue(Fulbo.Game.GameManager.Instance.charactersManager.referi, text);
        }
        void SetDialogue(Character character, string text)
        {
            // skip on tutorial
            if (Data.Instance.mode != Data.modes.PARTYMODE && Data.Instance.newScene == "Game" && !Data.Instance.onBoardingManager.IsBoardingStepDone(OnBoardingManager.BoardingStepStates.FIRST_MATCH_PLAYED))
                return;
            if (text == "" || character == null || !Data.Instance.settings.mainSettings.speech_bubbles_on) return;
            CharacterDialogueUI c = GetDialogue(character);
            if (c == null)
            {
                /*
                if (GameManager.Instance != null && GameManager.Instance.ball != null)
                {
                    Vector3 ballPos = GameManager.Instance.ball.transform.position;
                    ballPos.y = 2;
                    Vector3 charPos = character.transform.position + Vector3.forward;
                    charPos.y = 2;

                    float zDif = charPos.z - ballPos.z;
                    float dif = Vector3.Distance(ballPos, charPos);

                    Color color = Color.green;

                    //Utils.PrintColor("orange", zDif.ToString() + " " + dif.ToString());
                    Debug.DrawLine(charPos, ballPos, color, 5f);

                    if (zDif < -1f && dif < 2)
                    {
                        return;
                    }
                }
                */

                c = Instantiate(characterDialogueUI_to_instantiate);
                c.transform.SetParent(container);
                c.transform.localScale = Vector3.one;

                all.Add(c);
            }
            c.Init(character, text);
        }
        CharacterDialogueUI GetDialogue(Character character)
        {
            foreach (CharacterDialogueUI c in all)
            {
                if (!c.enabled || c.character == character)
                    return c;
            }
            return null;
        }
    }

}