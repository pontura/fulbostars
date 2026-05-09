using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class PoolObjects : MonoBehaviour
{
    [SerializeField] Transform container;

    [SerializeField] List<GameObject> objectsToPool;
    Dictionary<string, List<GameObject>> all;

    private void Awake()
    {
        all = new Dictionary<string, List<GameObject>>();
        foreach (GameObject go in objectsToPool)
        {
            all.Add(go.name, new List<GameObject>());
            AddNeObject(go.name);
        }
    }
    public GameObject Get(string key)
    {
        foreach (KeyValuePair<string, List<GameObject>> d in all)
        {
            if (d.Key == key)
            {
                GameObject go = GetObjectInDic(d.Value);
                if (go == null)
                    go = AddNeObject(key);
                go.gameObject.SetActive(true);
                return go;
            }
        }
        Debug.LogError("No obj on pool: " + key);
        return null;
    }
    public GameObject GetObjectInDic(List<GameObject> allInDic)
    {
        foreach (GameObject go in allInDic)
        {
        if (!go.activeSelf)
                return go;
        }
        return null;
    }
    public GameObject AddNeObject(string key)
    {
        foreach (GameObject go in objectsToPool)
        {
            if (key == go.name)
            {
                foreach (KeyValuePair<string, List<GameObject>> d in all)
                {
                    if (d.Key == key)
                    {
                        GameObject newGO = Instantiate(go, container);
                        newGO.name = key;
                        newGO.SetActive(false);
                        d.Value.Add(newGO);
                        return newGO;
                    }
                }
            }
        }
        return null;
    }
    public void Pool(GameObject go)
    {
        go.SetActive(false);
        go.transform.SetParent(container);
    }
}