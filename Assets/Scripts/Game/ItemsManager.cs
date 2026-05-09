using UnityEngine;
using System;

namespace Fulbo.Game
{
    public class ItemsManager : MonoBehaviour
    {
        [Serializable]
        class Item
        {
            public int total;
            public GameObject asset;
        }
        [SerializeField] Item[] items;
        [SerializeField] Transform container;

        void Start()
        {
            //if (Data.Instance.matchData.IsTutorial())  return; // is Tutorial
            //Invoke("AddItems", 1);
        }
        void AddItems()
        {
            foreach (Item item in items)
            {
                for (int a = 0; a < item.total; a++)
                {
                    GameObject g = Instantiate(item.asset, container);
                    Add(g);
                }
            }
        }
        void Add(GameObject g)
        {
            float _x = Utils.GetRandomFloatBetween(-15, 15);
            float _z = Utils.GetRandomFloatBetween(-10, 10);
            g.transform.position = new Vector3(_x, 4, _z);
        }
    }

}