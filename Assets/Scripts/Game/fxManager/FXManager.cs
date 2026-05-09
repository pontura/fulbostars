using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Linq;

namespace Fulbo.FX
{
    public class FXManager : MonoBehaviour
    {
        [SerializeField] FXData[] all;
        [SerializeField] Transform container;
        Dictionary<string, List<FXAsset>> pool;

        [Serializable]
        public class FXData
        {
            public types type;
            public FXAsset asset;
        }
        public enum types
        {
            EXPLOTION,
            SAND,
            REFEREE_HIT,
            KICK,
            POWERUP_SUPERKICK,
            POWERUP_BOMB,
            POWERUP_SPEED,
            BAD_KICK,
            FUZZY
        }
        void Start()
        {
            pool = new Dictionary<string, List<FXAsset>>(); 
            Events.OnFX += OnFX;
        }
        void OnDestroy()
        {
            Events.OnFX -= OnFX;
        }
        void OnFX(types type, Vector3 pos)
        {
           // print("OnFX " + type + " pos: " + pos);
            FXAsset fxAsset = GetFromPool(type);
            fxAsset.Init(pos);
        }
        FXAsset GetFromPool(types type)
        {
            List<FXAsset> arr;
            if (pool.ContainsKey(type.ToString()))
            {
                arr = pool[type.ToString()];
                foreach (FXAsset asset in arr)
                    if (!asset.isActiveAndEnabled)
                        return asset;

                return SetNewPool(type, arr);
            }
            else
            {
                arr = new List<FXAsset>();
                FXAsset fxAsset = SetNewPool(type, arr);
                pool.Add(type.ToString(), arr);
                return fxAsset;
            }
        }
        FXAsset SetNewPool(types type, List<FXAsset> arr)   
        {
            FXAsset fxAsset = Instantiate(GetFX(type).asset, container);            
            arr.Add(fxAsset);
            return fxAsset;
        }
        FXData GetFX(types type)
        {
            foreach (FXData f in all)
                if (f.type == type)
                    return f;
            return null;
        }
    }
}