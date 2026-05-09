using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using Fulbo.Stadiums;
using Fulbo.Game;
using Fulbo.DB;
using Random = UnityEngine.Random;

namespace Fulbo
{
    public class ChestsData : DataLoader
    {
        public int cupID = 1;

        public static ChestsData mInstance;
        public static ChestsData Instance { get { return mInstance; } }

        public List<ChestData> all;

        public List<AssetsData> assets;
        [Serializable]
        public class AssetsData
        {
            public GameObject asset;
            public int id; // needs to be equal to the one in the database
        }
        [Serializable]
        public class ChestData
        {           
            public int id;
            //public string name;

            //public int soft_min;
            //public int soft_max;

            //public int energy_min;
            //public int energy_max;

            //public int softValue;
            //public int energyValue;

            //public int SetSoftValue()
            //{
            //    softValue = Random.Range(soft_min, soft_max);
            //    return softValue;
            //}
            //public int SetEnergyValue()
            //{
            //    energyValue = Random.Range(energy_min, energy_max);
            //    return energyValue;
            //}

            public GameObject GetAsset()
            {
                AssetsData assetData = GetAsset(id);
                if (assetData != null)
                    return assetData.asset;
                else
                    Debug.LogError("No asset for: " + id);
                return GetAsset(ChestsData.Instance.all[0].id).asset;
            }
            AssetsData GetAsset(int id)
            {
                foreach (AssetsData d in ChestsData.Instance.assets)
                    if (d.id == id) return d;
                return null;
            }
        }

        void Awake()
        {
            if (mInstance != null)
                Destroy(gameObject);
            else
            {
                mInstance = this;
                DontDestroyOnLoad(gameObject);
            }
        }
        public ChestData GetChest(int id)
        {
            foreach( ChestData c in all)
            {
                if (c.id == id)
                    return c;
            }
            return all[0];
        }

        Action OnDone;
        public void OnLoad(Action OnDone)
        {
            this.OnDone = OnDone;
            LoadData(null);
        }
        public override void OnLoaded(List<List<string>> d)
        {
            OnDataLoaded(d);
            if (OnDone != null)
            {
                OnDone();
                OnDone = null;
            }
        }        
        void OnDataLoaded(List<List<string>> d)
        {
            int colID = 0;
            int rowID = 0;
            ChestData contentLine = null;
            foreach (List<string> line in d)
            {
                foreach (string value in line)
                {
                    //print("row: " + rowID + "  colID: " + colID + "  value: " + value);
                    if (rowID >= 1)
                    {
                        if (colID == 0)
                        {
                            if (value != "")
                            {
                                contentLine = new ChestData();
                                contentLine.id = int.Parse(value);
                                all.Add(contentLine);
                            }
                            else
                                return;
                        }
                        else
                        {
                            //if (colID == 1 && value != "")
                            //{
                            //    contentLine.name = value;
                            //}
                            //if (colID == 2 && value != "")
                            //{
                            //    contentLine.soft_min = int.Parse(value);
                            //}
                            //if (colID == 3 && value != "")
                            //{
                            //    contentLine.soft_max = int.Parse(value);
                            //}
                            //if (colID == 4 && value != "")
                            //{
                            //    contentLine.energy_min = int.Parse(value);
                            //}
                            //if (colID == 5 && value != "")
                            //{
                            //    contentLine.energy_max = int.Parse(value);
                            //}
                        }
                    }
                    colID++;
                }
                colID = 0;
                rowID++;
            }
        }
    }
}