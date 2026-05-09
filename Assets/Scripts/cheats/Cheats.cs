using Fulbo.Game;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace Fulbo.UI.Cheats
{
    public class Cheats : MonoBehaviour
    {
        [SerializeField] ButtonCustom buttonToInstantiate;
        [SerializeField] GameObject logToInstantiate;
        [SerializeField] GameObject statLine;

        [SerializeField] GameObject panel;
        [SerializeField] GameObject panelLogs;
        [SerializeField] GameObject panelStats;

        [SerializeField] ButtonCustom buttonOpen;
        [SerializeField] ButtonCustom buttonLogsOpen;
        [SerializeField] ButtonCustom buttonStats;

        [SerializeField] Transform container;
        [SerializeField] Transform containerLogs;

        [SerializeField] Transform stats1Container;
        [SerializeField] Transform stats2Container;

        [SerializeField] Text statsTitle;

        public static bool moneyCheat = false;

        private void Start()
        {
            if (DB.DBManager.Instance.versionMode == DB.DBManager.versionModes.PROD)
            {
                Reset();
                return;
            }
            Events.OnSceneLoaded += OnSceneLoaded;
            Events.Log += Log;
            Utils.RemoveAllChildsIn(container);
            buttonOpen.Init(0, OnClick, "Cheats");
            buttonLogsOpen.Init(0, OnLogClick, "Logs");
            buttonStats.Init(0, OnStats, "Stats");

            //buttons:
            AddButton(1, "Win Match");
            AddButton(6, "Lose Match");
            AddButton(7, "Draw Match");
            AddButton(8, "Unlock all Cups");
            AddButton(2, "Add Coins to Score");
           // AddButton(3, "Unlock stadiums and levels");
            AddButton(4, "Spawn Powerups");
            AddButton(5, "Refill Energy");

            Reset();
        }
        private void OnDestroy()
        {
            Events.OnSceneLoaded -= OnSceneLoaded;
            Events.Log -= Log;
        }
        private void Reset()
        {
            panel.SetActive(false);
            panelLogs.SetActive(false);
            panelStats.SetActive(false);

            buttonOpen.gameObject.SetActive(false);
            buttonLogsOpen.gameObject.SetActive(false);
            buttonStats.gameObject.SetActive(false);
        }
        void OnSceneLoaded(string scene)
        {
            if (scene != "Game") Reset();
            else
            {
                buttonOpen.gameObject.SetActive(true);
                buttonLogsOpen.gameObject.SetActive(true);
                buttonStats.gameObject.SetActive(true);
                Utils.RemoveAllChildsIn(containerLogs);
            }
        }
        void OnStats(int id)
        {
            ToggleStats();
        } 
        void OnLogClick(int id)
        {
            ToggleLogs();
        }
        void OnClick(int id)
        {
            if (Data.Instance.newScene != "Game") return;

            switch (id)
            {
                //case 0: Toggle();  break;
                case 1: WinMatch(); break;
                case 2: UnlockMoney(); break;
                case 3: UnlockLevels(); break;
                case 4: Events.CheatThrowPowerups(); break;
                case 5: GainEnergy(); break;
                case 6: LoseMatch(); break;
                case 7: DrawMatch(); break;
                case 8: WinCups(); break;
            }

            Toggle();
        }
        bool isOn;
        void Toggle()
        {
            isOn = !isOn;
            panel.SetActive(isOn);
        }
        bool logIsOn;
        void ToggleLogs()
        {
            logIsOn = !logIsOn;
            panelLogs.SetActive(logIsOn);
            if (logIsOn)
                Time.timeScale = 0;
            else
                Time.timeScale = 1;
        }
        bool statsIsOn;
        void ToggleStats()
        {
            statsIsOn = !statsIsOn;
            panelStats.SetActive(statsIsOn);
            if (statsIsOn)
            {
                Time.timeScale = 0;
                LoadStats();
            }
            else
                Time.timeScale = 1;
        }
        private void AddButton(int id, string text)
        {
            ButtonCustom button = Instantiate(buttonToInstantiate, container);
            button.Init(id, OnClick, text);
         }
        void Log(string text)
        {
            GameObject g = Instantiate(logToInstantiate, containerLogs);
            g.GetComponentInChildren<Text>().text = text;
        }
        void LoseMatch()
        {
            Data.Instance.matchData.time = 11111;
            Data.Instance.matchData.score = new Vector2(1, 0);
        }
        void DrawMatch()
        {
            Data.Instance.matchData.time = 11111;
            Data.Instance.matchData.score = new Vector2(1, 1);
        }       
        void WinMatch()
        {
            Data.Instance.matchData.time = 11111;
            Data.Instance.matchData.score = new Vector2(0, 5);
        }
        void WinCups()
        {
            DB.DBManager.Instance.DbUserData.data.gameData.cups.UnlockAll();
        }
        void UnlockMoney()
        {
            moneyCheat = true;
        }

        [ContextMenu("Unlock Levels")]
        void UnlockLevels()
        {
            //StoryModeData.Instance.UnlockAll();
        }

        [ContextMenu("Gain Energy")]
        void GainEnergy()
        {
            Data.Instance.energySystem.EnergyCheat();
        }
        void LoadStats()
        {
            print("LoadStats");
            Utils.RemoveAllChildsIn(stats1Container);
            Utils.RemoveAllChildsIn(stats2Container);
            List<Character> all = GameManager.Instance.charactersManager.team1;
            foreach (Character ch in all) AddStat(1, ch, stats1Container);

            all = GameManager.Instance.charactersManager.team2;
            foreach (Character ch in all) AddStat(2, ch, stats2Container);
            int duelGK = CupsData.Instance.GetActualLevel().duelStatsGK;
            int duelPlayers = CupsData.Instance.GetActualLevel().duelStatsPlayer;
            statsTitle.text = "Duel Players =" + duelPlayers + "  Duel GK =" + duelGK;


            print("all " + all.Count);
        }
        void AddStat(int teamID, Character ch, Transform container)
        {
            CharacterStats cs = ch.characterStats;
            GameObject go = Instantiate(statLine, container);
            Text[] arr = go.GetComponentsInChildren<Text>();
            arr[0].text = CharactersData.Instance.GetCharacterData(ch.data.id, ch.type == Character.types.GOALKEEPER).avatarName + "(" + ch.type +")";
            arr[1].text = cs.accuracy.ToString();
            arr[2].text = cs.stamina.ToString();
            arr[3].text = cs.speed.ToString();
            arr[4].text = cs.dexterity.ToString();
            arr[5].text = cs.awareness.ToString();
            arr[6].text = cs.GetTotal(true).ToString();
    }
    }
}