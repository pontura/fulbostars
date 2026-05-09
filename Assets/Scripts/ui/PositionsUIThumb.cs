using Coffee.UIEffects;
using Fulbo.Game;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Fulbo.UI
{
    public class PositionsUIThumb : MonoBehaviour, IPointerDownHandler, IPointerUpHandler
    {
        [SerializeField] GameObject[] info;

        public Image image;
        PositionsUIManager manager;
        public GameObject selected;
        public int characterID;
        public bool isGoalKeeper;
        public Text levelField;
        CharacterStats stats;
        // CharactersData.CharacterData cData;
        public int uniqueID;
        public CharactersPositions.CharacterPositionData characterPositionData;
        int teamID;
        Vector3 pos;
        [SerializeField] GameObject[] positionIcons;
        [SerializeField] GameObject[] positionStatIcons;
        [SerializeField] UIShadow uiShadow;
        int totalStats = 0;

        private void OnDestroy()
        {
            Events.CharacterUpdatedData -= CharacterUpdatedData;
        }
        void CharacterUpdatedData (DB.DBUserData.DBCharacterData u)     
        {
            SetHappinessByPosition(teamID);
        }

        public void Init(PositionsUIManager manager, CharactersPositions.CharacterPositionData characterPositionData, int characterID, int teamID, DB.DBUserData.DBCharacterData dbData = null)
        {
            this.characterPositionData = characterPositionData;

            this.characterID = characterID;
            this.manager = manager;
            this.teamID = teamID;

            ResetAllPositionStats();

            if (characterPositionData.type == Character.types.GOALKEEPER)
                isGoalKeeper = true;
            else
                isGoalKeeper = false;

            foreach (GameObject go in positionIcons)
                go.SetActive(false);
            Character.types type = characterPositionData.type;
            if (dbData != null)
            {
                if(dbData.IsGoalkeeper())
                    type = Character.types.GOALKEEPER;
                else if(dbData.position == 0)
                    type = Character.types.DEF;
                else if (dbData.position == 1)
                    type = Character.types.MID;
                else if (dbData.position == 2)
                    type = Character.types.FOR;
            }
            switch (type)
            {
                case Character.types.DEF:
                    positionIcons[0].SetActive(true); break;
                case Character.types.MID:
                    positionIcons[1].SetActive(true); break;
                case Character.types.FOR:
                    positionIcons[2].SetActive(true); break;
                case Character.types.GOALKEEPER:
                    positionIcons[3].SetActive(true); break;
            }

            stats = new CharacterStats();

            image.sprite = CharactersData.Instance.GetCharacterData(characterID, isGoalKeeper).thumb;
            uiShadow = image.GetComponent<UIShadow>();
            uiShadow.effectColor = new Color(0, 0, 0, 0);
            if (dbData != null)
                uniqueID = dbData.id;

            float _x = characterPositionData.pos[1] * manager.scaleFactor.x;
            float _y = (2 * (characterPositionData.pos[0] * manager.scaleFactor.y)) - manager.scaleFactor.y;

            transform.localPosition = new Vector2(_x, _y);

           if (dbData != null)
            {
                if (dbData.IsGoalkeeper()) // offset para uqe no haya otro atras
                    transform.localPosition = new Vector2(_x, _y+6);

                if (teamID == 1)
                {
                    manager.SetFinalPosition(this);
                    Events.CharacterUpdatedData += CharacterUpdatedData;
                }
                SetHappinessByPosition(teamID);
                stats.ForceStats(dbData);

                totalStats = stats.GetTotal(true);
            }
            else if (teamID == 2) // muestra los stats por default del level:
            {
                stats = CupsData.Instance.GetActualLevel().GetTotalStats(characterPositionData.type);
                totalStats = stats.GetTotal(false);
            }
            else
            {
                stats.ForceStats(50, 50, 50, 50, 50);
                totalStats = stats.GetTotal(false);
            }
           if(Data.Instance.mode == Data.modes.PARTYMODE)
            { 
                foreach(GameObject g in info)
                {
                    g.SetActive(false);
                }                    
            }else
                 SetTotalStatsField();
        }
        void SetTotalStatsField()
        {
            levelField.text = totalStats.ToString();
        }
        bool IsInteractive()
        {
            if (teamID != 1) return false;
            return true;
        }
        public bool CanDragged()
        {
            if (!IsInteractive()) return false;
            if (characterPositionData.type == Character.types.GOALKEEPER) return false;
            return true;
        }
        public void SetSelected(bool isOn)
        {
            if (!IsInteractive() && isOn) return;
            if (isOn)
                image.color = Color.black;
            else
                image.color = Color.white;
        }
        public void OnMouseDown()
        {
            if (!IsInteractive()) return;
           // if (!CanDragged()) return;
            pos = transform.position;
            manager.Drag(this, true);
        }
        public void OnMouseUp()
        {
            if (!IsInteractive()) return;

            if (Vector3.Distance(transform.position, pos) < 3)
                manager.OnClick(uniqueID);
            else
                SetRealPos();

            // if (!CanDragged()) return;
            manager.Drag(this, false);

            SetHappinessByPosition(teamID);
            Events.OnTeamPowerRefresh();
        }
        void SetRealPos()
        {
            characterPositionData.pos[1] = transform.localPosition.x / manager.scaleFactor.x;
            characterPositionData.pos[0] = (transform.localPosition.y + manager.scaleFactor.y) / manager.scaleFactor.y / 2;

            Data.Instance.charactersPositions.SavePlayerPositionData(manager);

           // AudioManager.Instance.PlaySoundOneShot("ui", "ui/ui_click2", false);
        }

        public void OnPointerDown(PointerEventData eventData)
        {
            print("d");
            OnMouseDown();
        }

        public void OnPointerUp(PointerEventData eventData)
        {
            print("u");
            OnMouseUp();
        }
        void ResetAllPositionStats()
        {
            foreach (GameObject go in positionStatIcons)
                go.SetActive(false);
        }
        public void SetHappinessByPosition(int teamID)
        {
           // print(teamID + " uniqueID: " + uniqueID);
            ResetAllPositionStats();

            if (!isGoalKeeper)
            {
                //  int originalTypeIDByPosition = Data.Instance.myTeam.GetCharacterType(characterID);
                int originalTypeIDByPosition;
                if (teamID == 1)
                    originalTypeIDByPosition = DB.DBManager.Instance.DbUserData.data.GetPlayerByID(uniqueID).position;
                else
                    originalTypeIDByPosition = Data.Instance.pvpData.GetPlayer(uniqueID, isGoalKeeper).position;

                int happinesID = characterPositionData.GetHappiness(originalTypeIDByPosition, characterPositionData.type);

                positionStatIcons[happinesID].SetActive(true);
                stats.SetHappiness(happinesID);

                if (happinesID == 1)
                    uiShadow.effectColor = Color.yellow;
                else if (happinesID == 2)
                    uiShadow.effectColor = Color.red;
                else
                    uiShadow.effectColor = new Color(0, 0, 0, 0);
                totalStats = stats.GetTotal(true);
            }
            SetTotalStatsField();
        }
        public int GetStats()
        {
            return totalStats;
        }
    }

}