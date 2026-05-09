using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Fulbo.Dashoard
{
    public class DashboardData : DataLoader
    {

        static DashboardData mInstance = null;

        public List<DashboardContentData> content;
       // [SerializeField] DashboardUI ui;
        [SerializeField] bool loaded;

        System.Action OnDone;

        public static DashboardData Instance  {  get   {  return mInstance;  }   }

        void Awake()
        {
            if (!mInstance)
                mInstance = this;
            ShowDashboard(false);
        }

        public void Init(System.Action OnDone)
        {
            this.OnDone = OnDone;
            if (!loaded)
            {
                content.Clear();
                LoadData(null);
               // ui.LoadOnInit();
            }
            else
                OnDone();
        }
        public void ShowDashboard(bool show)
        {
           // ui.SetActive(show);
        }
        public override void OnLoaded(List<List<string>> d)
        {
            OnDataLoaded(content, d);
            OnDone();
            // base.OnLoaded(d);
        }
        public DashboardContentData GetByType(DashboardContentData.types type)
        {
            print("_______________ADD: " + type);
            Utils.Shuffle(content);
            foreach (DashboardContentData d in content)
            {
                if (d.type == type)
                {
                    print("type: " + type);
                    print("d: " + d);
                    print("d.param: " + d.param);

                    if (d.param != null && d.param.Contains("difGoles="))
                    {
                        print("param: " + d.param);

                        string[] arr = d.param.Split("difGoles="[8]);

                        print("arr: " + arr);
                        if (arr != null && arr.Length > 1)
                        {
                            int myDiffGoles = int.Parse(arr[1]);
                            int diffGoles = Data.Instance.matchData.GetDiffGoles();

                            if (myDiffGoles == diffGoles)
                                return d;
                        }
                    }
                    else
                        return d;
                }
            }
            return null;
        }
        void OnDataLoaded(List<DashboardContentData> content, List<List<string>> d)
        {
            int colID = 0;
            int rowID = 0;
            DashboardContentData contentLine = null;
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
                                contentLine = new DashboardContentData();
                                switch (value)
                                {
                                    case "header": contentLine.type = DashboardContentData.types.header; break;
                                    case "hero": contentLine.type = DashboardContentData.types.hero; break;
                                    case "caja_1_medio": contentLine.type = DashboardContentData.types.caja_1_medio; break;
                                    case "caja_1_tercio": contentLine.type = DashboardContentData.types.caja_1_tercio; break;
                                    case "caja_2_tercios": contentLine.type = DashboardContentData.types.caja_2_tercios; break;
                                    case "stats": contentLine.type = DashboardContentData.types.stats; break;
                                    case "two_characters": contentLine.type = DashboardContentData.types.two_characters; break;
                                    case "win": contentLine.type = DashboardContentData.types.win; break;
                                }
                                content.Add(contentLine);
                            }
                        }
                        else
                        {
                            if (colID == 1 && value != "")
                            {
                                contentLine.color = Data.Instance.settings.GetColorFor(value);
                                //switch (value)
                                //{
                                //    case "rojo": contentLine.color = Color.red; break;
                                //    case "amarillo": contentLine.color = Color.yellow; break;
                                //    case "verde": contentLine.color = Color.green; break;
                                //    case "rosa": contentLine.color = Color.magenta; break;
                                //    case "naranja": contentLine.color = Color.green; break;
                                //    case "violeta": contentLine.color = Color.grey; break;
                                //    case "azul": contentLine.color = Color.blue; break;
                                //}
                            }
                            if (colID == 2 && value != "")
                            {
                                contentLine.title = value;
                            }
                            if (colID == 3 && value != "")
                            {
                                contentLine.copete = value;
                            }
                            if (colID == 4 && value != "")
                            {
                                contentLine.text = value;
                            }
                            if (colID == 5 && value != "")
                            {
                                contentLine.param = value;
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