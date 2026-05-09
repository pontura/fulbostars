using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using Fulbo.Mundial;

namespace Fulbo.UI
{
    public class Texts : DataLoader
    {
        //public string url_en = "https://docs.google.com/spreadsheets/d/e/2PACX-1vR_IX7b2VUn8q2_yvC3lD2JpNxZZiObI31lFeyX2OV-eqc3OYlrQ9U_ffo1kiKeHgvtpVi0v5hvQ54s/pub?gid=1545330465&single=true&output=tsv";

        public List<TextData> content;
        public bool loaded;

        char splitChar = '|';
        char newLineChar = '&';

        [Serializable]
        public class TextData
        {
            public string id;
            public string text_en;
            public string text_es;
        }
        Action OnDone;
        public void Load(Action OnDone)
        {
            this.OnDone = OnDone;
            if (loaded) return;
            LoadData(null);
        }
        public override void OnLoaded(List<List<string>> d)
        {
            OnDataLoaded(content, d);
            OnDone();
        }
        public string Get(string key)
        {
            string lang = Data.Instance.langsManager.GetLang();
            foreach (TextData t in content)
                if (t.id == key)
                {
                    switch (lang)
                    {
                        case "es": return CheckAndReplaceVarsIn(t.text_es);
                        default: return CheckAndReplaceVarsIn(t.text_en);
                    }
                }

            return "";
        }
        public string CheckAndReplaceVarsIn(string text)
        {
            
             if (text.Contains("[name cup]"))
            {
                string cupName = "";
                int cup = Data.Instance.matchData.levelData.cupID;
                int tier = Data.Instance.matchData.levelData.tier;

                if (cup > 0 && tier > 0)
                    cupName = CupsData.Instance.GetCupData(cup, tier).cup_name;
                
                return text.Replace("[name cup]", cupName.ToUpper());
            }
            //if (text.Contains("{country}"))
            //{
            //    MundialData.LevelData l = MundialData.Instance.GetCountryData(DB.DBManager.Instance.DbUserData.data.country);
            //    if (l != null)
            //        return text.Replace("{country}", l.name.ToUpper());
            //}
            if (text.Contains(splitChar+""))
            {
                string[] arr = text.Split(splitChar);
                return arr[UnityEngine.Random.Range(0, arr.Length)];
            }

            text = text.Replace(newLineChar, '\n');

            return text;
        }
        public string ReplaceTextsByCharacter(string text, int avatarID, string avatarName)
        {
            return text.Replace("{avatar" + avatarID + "}", avatarName);
        }
        void OnDataLoaded(List<TextData> content, List<List<string>> d)
        {
            content.Clear();
            int colID = 0;
            int rowID = 0;
            TextData contentLine = null;
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
                                contentLine = new TextData();
                                contentLine.id = value;
                                content.Add(contentLine);
                            }
                        }
                        else
                        {
                            if (colID == 1 && value != "")
                            {
                                contentLine.text_en = value;
                            }
                            if (colID == 2 && value != "")
                            {
                                contentLine.text_es = value;
                            }
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