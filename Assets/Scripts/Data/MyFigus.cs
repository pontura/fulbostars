using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MyFigus : MonoBehaviour
{
    public List<int> all;
    public string saved_figus;

    //private void Update()
    //{
    //    if(Input.GetKeyDown(KeyCode.Alpha0))
    //    {
    //        SetNew(1);
    //    }
    //}
    private void Start()
    {
        //LoadSavedData();
    }
    public bool IsEmpty()
    {
        if (PlayerPrefs.GetString("saved_figus", "") == "")
            return true;
        return false;
    }
    void LoadSavedData()
    {
        saved_figus = PlayerPrefs.GetString("saved_figus", "");

        if (saved_figus == "") return;

        string[] all;

        all = saved_figus.Split(","[0]);
        if (all == null || all.Length < 2) return;
    }
    //void Save()
    //{
    //    saved_figus = "";
    //    int a = 0;
    //    foreach (int id in all)
    //    {
    //        a++;
    //        saved_figus += id.ToString();
    //        if (a < all.Count)
    //            saved_figus += ",";
    //    }
    //    PlayerPrefs.SetString("saved_figus", saved_figus);
    //}

    //public void SetNew(int newID)
    //{
    //    all.Add(newID);
    //    Save();
    //}
  
    //public void RemoveFigu(int id)
    //{
    //    all.Remove(id);
    //    Save();
    //}
}
