using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Text.RegularExpressions;

namespace Fulbo
{
    public class SpreadsheetLoader : MonoBehaviour
    {
        [Serializable]
        public class Line
        {
            public string[] data;
        }
        public void LoadFromTo(string googleURL, System.Action<List<Line>> onDone)
        {
            Debug.Log("[URL]: " + googleURL);
            StartCoroutine(GetData(googleURL, onDone));
        }
        IEnumerator GetData(string url, System.Action<List<Line>> onDone)
        {
            using (WWW www = new WWW(url))
            {
                yield return www;
                if(www.error != null)
                {
                    StartCoroutine(GetData(url, onDone));
                    Events.OnPopup("Connection error. Lets try again...", null);
                } else
                    CreateListFromFile(www.text, onDone);
            }
        }
        public void CreateListFromFile(string text, System.Action<List<Line>> onDone)
        {
            string[] lines = text.Split("\n"[0]);
            List<Line> arr = new List<Line>();
            foreach (string line in lines)
                arr.Add(ParseLine(line));

            onDone(arr);
        }
        public Line ParseLine(string lineData)
        {
            Line line = new Line();
            line.data = lineData.Split("\t"[0]);
            int id = 0;
            foreach (string s in line.data)
            {
                line.data[id] = ParseString(s);
                id++;
            }
            return line;
        }
        string ParseString(string text)
        {
            return text.Replace("\r", "").Replace("\n", "");
        }
    }
}
