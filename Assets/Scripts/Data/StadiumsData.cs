using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using Fulbo.AssetsBundle;

namespace Fulbo.Stadiums
{
    public class StadiumsData : MonoBehaviour
    {
        static StadiumsData mInstance = null;

        public StadiumData active;
        public StadiumData[] all;

        public List<StadiumData> GetAll(bool isCup)
        {
            List<StadiumData> arr = new List<StadiumData>();
            foreach (StadiumData s in all)
            {
                if (s.IsCup == isCup) arr.Add(s);
            }
            return arr;
        }
        [Serializable]
        public class StadiumAsset
        {
            public string size;
            public GameObject asset;
            public float size_x;
            public float size_y;
            public GameObject powerupsSpawners;
            public CamereSettings cameraSettings;

            [Serializable]
            public class CamereSettings
            {
                public float originalZoomSize = 5;
                public float offsetZ = 9;
            }
        }

        [Serializable]
        public class StadiumData
        {
            public bool IsCup;
            public string name;
            public int difficulty_level = 1; //1: easy -> to hard
            public string selectedSize = "large";
            public int id;
            public Color color;
            public bool unavailable; // cant be played:
            public Sprite thumbBG;

            public StadiumAsset[] assets;
            public GameObject ball;

            public AudioClip ambience_end_loop;
            public AudioClip ambience_loop;
            public AudioClip opening;
            public AudioClip[] crowd_good;
            public AudioClip[] crowd_chance;
            public AudioClip[] crowd_bad;
            public AudioClip[] crowd_foul;
            public AudioClip[] crowd_gol;

            public float crowd_expr_vol;

            public AudioClip[] kick_hard;
            public AudioClip[] kick_pass;
            public AudioClip[] kick_balloon;

            public AudioClip kick_soft;

            public AudioClip[] ball_hit_character_soft;
            public AudioClip[] ball_hit_character_hard;

            public AudioClip[] kick_head; 
            public AudioClip[] kick_chilena;

            public AudioClip ball_catch;
            public AudioClip ball_gk_catch;
            public AudioClip ball_gk_saca;

            public AudioClip[] ball_carry;

            public AudioClip[] pica;
            public AudioClip picaSoft;
            public AudioClip[] palo;
            public AudioClip net;

            public AudioClip[] wallSoft;
            public AudioClip[] wall;

            public GameObject penaltyGO;
            

            public StadiumAsset GetAsset(string size)
            {
                if (size == "") size = "small";
                foreach (StadiumAsset sData in assets)
                    if (sData.size == size && sData.asset != null)
                        return sData;
                Debug.LogError("ERROR: No hay stadium para " + name + " con size: " + size);
                return GetAvailableAsset();
            }
            StadiumAsset GetAvailableAsset()
            {
                foreach (StadiumAsset sData in assets)
                    if (sData.asset != null)
                        return sData;
                return null;
            }
            public StadiumAsset GetAssetBySelectedSize()
            {
                return GetAsset(selectedSize);
            }
            public StadiumAsset GetPenalty()
            {
                return GetAsset(selectedSize);
            }
            public string GetDifficultyString()
            {
                return Data.Instance.texts.Get("level_difficulty_" + difficulty_level);
            }
        }
        void Awake()
        {
            if (!mInstance)
                mInstance = this;
            else
            {
                Destroy(this.gameObject);
                return;
            }
            DontDestroyOnLoad(this.gameObject);

            foreach(StadiumData stadData in all)
            {
                if (!stadData.unavailable)
                {
                    availableStadiums.Add(stadData.id);
                }
            }
            id = UnityEngine.Random.Range(0, availableStadiums.Count);
        }
        public static StadiumsData Instance { get { return mInstance; } }

        public void SetActiveStadium(int id, string size)
        {
            active = GetStadium(id);
            active.selectedSize = size;
        }
        public StadiumData GetStadium(int id)
        {
            foreach (StadiumData sData in all)
            {
                if (sData.id == id)
                    return sData;
            }
            Debug.LogError("No stadium for " + id);
            return null;
        }

        List<int> availableStadiums = new List<int>();
        int id;
        public void SetRandomStadium()
        {
            print("SetRandomStadium");
            SetActiveStadium(availableStadiums[id], "medium");
            id++;
            if (id >= availableStadiums.Count)
                id = 0;
        }

        public AudioClip GetOpeningAudioClip()
        {
            return active.opening;
        }
    }
}