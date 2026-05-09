using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Linq;
using Fulbo.Game;
using UnityEngine.InputSystem;

namespace Fulbo.UI
{
    public class PositionsUIManager : MonoBehaviour
    {
        [SerializeField] PositionsUIThumb dragItem;
        public Transform container;
        public PositionsUIThumb thumb;
        public Vector2 scaleFactor;
        public List<PositionsUIThumb> all;
        public MyTeamSelector myTeamSelector;

        //opponents
        public void Init(CharactersPositions.PositionsData data, List<int> characters, int positionTeamID)
        {
            all.Clear();
            Utils.RemoveAllChildsIn(container);
            int id = 0;
            foreach (int characterID in characters)
            {
                if (id >= data.posData.Length) return; // para que no se pase.

                PositionsUIThumb newThumb = Instantiate(thumb);
                newThumb.transform.SetParent(container);
                newThumb.transform.localScale = Vector2.one;
                newThumb.Init(this, data.posData[id], characterID, positionTeamID);
                all.Add(newThumb);
                id++;
            }
        }
        //my team
        public void Init(CharactersPositions.PositionsData data, List<DB.DBUserData.DBCharacterData> characterData, int positionTeamID)
        {
            all.Clear();
            Utils.RemoveAllChildsIn(container);
            int id = 0;
            foreach (DB.DBUserData.DBCharacterData dbData in characterData)
            {
                if (id >= data.posData.Length) { Debug.Log("no hay tantas posiciones!");  return; }// para que no se pase.
                if (dbData == null) { Debug.Log("no hay tantas dbData!"); return; }// para que no se pase.
                if (id == 0)
                    data.posData[id].type = Character.types.GOALKEEPER;

                PositionsUIThumb newThumb = Instantiate(thumb);
                newThumb.transform.SetParent(container);
                newThumb.transform.localScale = Vector2.one;
                newThumb.Init(this, data.posData[id], dbData.player_id, positionTeamID, dbData);
                all.Add(newThumb);
                id++;
            }
            Events.OnTeamPowerRefresh();
        }


        public void OnClick(int uniqueID)
        {
            //myTeamSelector.OnSelectCharacterForReplace(uniqueID);
            //print("SELECT PositionsUIThumb uniqueID + " + uniqueID);
            //foreach (PositionsUIThumb positionsUIThumb in all)
            //{
            //    if (myTeamSelector.state == MyTeamSelector.states.REPLACING && positionsUIThumb.uniqueID == uniqueID)// && positionsUIThumb.isGoalKeeper == cData.isGoalkeeper)
            //    {
            //        print("SELECT PositionsUIThumb uniqueID + " + positionsUIThumb.uniqueID + "cData.uniqueID" + uniqueID);
            //        positionsUIThumb.SetSelected(true);
            //    }
            //    else
            //        positionsUIThumb.SetSelected(false);
            //}
        }
        public void Reset()
        {
            foreach (PositionsUIThumb positionsUIThumb in all)
                positionsUIThumb.SetSelected(false);
        }
        Vector3 initialDragPosition;
        public void Drag(PositionsUIThumb item, bool initDrag)
        {
            if (initDrag)
                InitDrag(item);
            else
                StopDrag(item);
        }
        void InitDrag(PositionsUIThumb item)
        {
            Vector3 mousePos = GetOriginalPos();
            initialDragPosition = item.transform.localPosition - mousePos;
            dragItem = item;
            item.transform.SetParent(transform);
            item.transform.SetParent(container);
        }
        void StopDrag(PositionsUIThumb item)
        {
            RefreshPositions();
            dragItem = null;
        }
        Vector3 GetOriginalPos()
        {
#if UNITY_EDITOR
            return Mouse.current.position.ReadValue();
#elif UNITY_ANDROID  || UNITY_IOS 
            return Touchscreen.current.position.ReadValue();
#else
            return Mouse.current.position.ReadValue();
#endif
        }
        private void Update()
        {
            if (dragItem == null) return;
            if (!dragItem.CanDragged()) return;

            Vector3 mousePos = GetOriginalPos();
            Vector3 pos = initialDragPosition + mousePos;

            if (pos.x < -scaleFactor.x)
                pos.x = -scaleFactor.x;
            else if (pos.x > scaleFactor.x)
                pos.x = scaleFactor.x;

            if (pos.y < -scaleFactor.y)
                pos.y = -scaleFactor.y;
            else if (pos.y > scaleFactor.y)
                pos.y = scaleFactor.y;

            //Si estoy arrastrando un chabóncito sobre el arquero, bajarlo un toque
            if (Mathf.Abs(pos.x) < scaleFactor.x / 4 && pos.y > (scaleFactor.y / 4) * 3)
            {
                pos.y = (scaleFactor.y / 4) * 3;
            }

            dragItem.transform.localPosition = pos;
        }
        void RefreshPositions()
        {
            foreach (PositionsUIThumb p in all)
                p.transform.SetParent(transform);

            all.Sort(CompareY);

            foreach (PositionsUIThumb p in all)
                SetFinalPosition(p);
        }
        public void SetFinalPosition(PositionsUIThumb p)
        {
            if (p.characterPositionData.type != Character.types.GOALKEEPER)
            {
                if (p.transform.localPosition.y > 65)
                    p.characterPositionData.type = Character.types.DEF;
                else if (p.transform.localPosition.y > -65)
                    p.characterPositionData.type = Character.types.MID;
                else
                    p.characterPositionData.type = Character.types.FOR;
            }
            p.transform.SetParent(container);
        }
        public int CompareY(PositionsUIThumb p1, PositionsUIThumb p2)
        {
            float v1 = p1.transform.position.y;
            float v2 = p2.transform.position.y;

            if (v1 > v2) return -1;
            else return 1;
        }
        public int GetTotalStatsForCurrentPlayers()
        {
            int stats = 0;
            foreach (PositionsUIThumb positionsUIThumb in all)
                stats += positionsUIThumb.GetStats();
            return stats;
        }
    }

}