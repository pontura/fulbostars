using Fulbo.Stadiums;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Fulbo.Game
{
    public class StadiumsManager : MonoBehaviour
    {
        public GameObject powerupsDefaultSpawners;
        ///
        public bool DEBUG_FORCE_STADIUM;
        public string force_size = "large";
        public int force_stadium_id;
        ///

        StadiumsData.StadiumAsset stadiumAsset;
        [SerializeField] GameObject lateral_top;
        [SerializeField] GameObject lateral_bottom;
        [SerializeField] GameObject corner_right;
        [SerializeField] GameObject corner_left;
        [SerializeField] GameObject raycastForArco;

        public void Init()
        {
            stadiumAsset = StadiumsData.Instance.active.GetAssetBySelectedSize();
#if UNITY_EDITOR || UNITY_STANDALONE 
            if (DEBUG_FORCE_STADIUM) //fuerza el default
            {
                StadiumsData.Instance.SetActiveStadium(force_stadium_id, force_size);
                stadiumAsset = StadiumsData.Instance.active.GetAssetBySelectedSize();
            }
#endif
            GameObject stadiumGO;
            if (Data.Instance.newScene == "Penalty")
                stadiumGO = StadiumsData.Instance.active.penaltyGO;
            else
                stadiumGO = stadiumAsset.asset;

            if (stadiumAsset.powerupsSpawners == null)
                stadiumAsset.powerupsSpawners = powerupsDefaultSpawners;

            if (Fulbo.Game.GameManager.Instance.powerupsManager != null)
            {
                GameObject powerupsSpawners = Instantiate(stadiumAsset.powerupsSpawners, transform);
                powerupsSpawners.transform.localPosition = Vector3.zero;
                Fulbo.Game.GameManager.Instance.InitPowerups(powerupsSpawners);
            }

            GameObject go = Instantiate(stadiumGO);
            go.transform.SetParent(transform);
            go.transform.localPosition = Vector3.zero;
            SetLimits();

            AddRaycastForArco(stadiumAsset.size_x / 2);
            AddRaycastForArco(-stadiumAsset.size_x / 2);

#if UNITY_EDITOR || UNITY_STANDALONE 
            SetGoalOff(go);
#endif
        }
        void SetGoalOff(GameObject go)
        {
            if (Data.Instance.settings.mainSettings.goals_blocked)
            {
                foreach (Collider c in go.GetComponentsInChildren<Collider>())
                    if (c.name == "GOAL")
                        c.isTrigger = false;
            }

        }
        void SetLimits()
        {
            if (lateral_top == null) return;

            float _z = stadiumAsset.size_y / 2 + (lateral_top.transform.localScale.z / 2);
            float _x = stadiumAsset.size_x / 2 + (corner_left.transform.localScale.x / 2);

            lateral_top.transform.localPosition = new Vector3(0, 0, _z);
            lateral_bottom.transform.localPosition = new Vector3(0, 0, -_z);

            corner_left.transform.localPosition = new Vector3(-_x, 0, 0);
            corner_right.transform.localPosition = new Vector3(_x, 0, 0);
        }
        void AddRaycastForArco(float _x)
        {
            if (raycastForArco == null) return;
            GameObject go = Instantiate(raycastForArco);
            go.transform.localPosition = new Vector2(_x,0);
        }
    }

}