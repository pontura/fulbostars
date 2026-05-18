using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using Fulbo.UI;

namespace Fulbo
{
    public class CharactersDefaultData : DataLoader
    {
        public bool reload = true;

        System.Action OnDone;
        public void Init(System.Action OnDone)
        {
            print("LoadCharactersDefaultData ");
            this.OnDone = OnDone;
            if (reload)
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
            CharactersData.CharacterData data = null;
            CharactersData cData = CharactersData.Instance;
            string type = "";
            foreach (List<string> line in d)
            {
                colID = 0;
                data = null;
                type = "";
                string avatarName = "";
                foreach (string value in line)
                {
                    if (rowID >= 1)
                    {
                        switch (colID)
                        {
                            case 0:
                                avatarName = value; break;
                            case 1:
                                type = value;
                                break;
                            case 2:
                                if (value != "")
                                {
                                    int id = int.Parse(value);
                                    if (type == "CH")
                                        data = cData.GetCharacterData(id, false, true);
                                    else if (type == "GK")
                                        data = cData.GetCharacterData(id, true, true);
                                    if (data != null)
                                    {
                                        data.stats = new CharacterStats();
                                        data.avatarName = avatarName;
                                        print("avatarName: " + data.avatarName);
                                    }
                                }
                                else
                                    data = null;

                                break;
                            case 3:
                                if (data != null && value != "")
                                {
                                    FigusData.rarities id = (FigusData.rarities)(int.Parse(value));
                                    data.rarity = id;
                                }
                                break;
                            case 4:
                                if (data != null)
                                    data.status = value;
                                break;
                            case 5:
                                if (data != null)
                                    data.text = value;
                                break;
                        }
                    }
                    colID++;
                }
                rowID++;
            }
        }
    }
}