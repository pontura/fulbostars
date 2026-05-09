using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

namespace Fulbo
{
    [Serializable]
    public class ClubData
    {
        public int designID = 1; // diseño de la remera
        public int shieldDesignID = 1; // diseño del escudo
        public int logo = 0; // logo del escudo

        public string name_abr;

        public int clubColor1;
        public int clubColor2;
        public int clubColor3;
        public int clubColor4;

        public Color GetColor(int id)
        {
            switch (id)
            {
                case 1: return Data.Instance.settings.GetColorByIndex(clubColor1);
                case 2: return Data.Instance.settings.GetColorByIndex(clubColor2);
                case 3: return Data.Instance.settings.GetColorByIndex(clubColor3);
                default: return Data.Instance.settings.GetColorByIndex(clubColor4);
            }
        }

        public Color GetAnotherColor(int otherColorIndex) {
            List<int> colorIndexes = Data.Instance.settings.GetOppositeColorIndexesByIndex(otherColorIndex);
            foreach(int index in colorIndexes) {
                if(index == clubColor1)
                    return Data.Instance.settings.GetColorByIndex(clubColor1);
                if (index == clubColor2)
                    return Data.Instance.settings.GetColorByIndex(clubColor2);
                if (index == clubColor3)
                    return Data.Instance.settings.GetColorByIndex(clubColor3);
                if (index == clubColor4)
                    return Data.Instance.settings.GetColorByIndex(clubColor4);
            }
            return Data.Instance.settings.GetColorByIndex(colorIndexes[0]);
        }

        public void SetDataFromString(string str)
        {
            if (str == null || str == "" || !str.Contains("."))
            {
                //default colors
                clubColor1 = 6;//azul
                clubColor2 = 7;//celeste
                return;
            }
            if (str != null && str.Length > 1)
            {
                string[] arr = str.Split("."[0]);
                if (arr.Length < 2) return;
                shieldDesignID = int.Parse(arr[0]);
                clubColor1 = int.Parse(arr[1]);
                clubColor2 = int.Parse(arr[2]);
                clubColor3 = int.Parse(arr[3]);
                clubColor4 = int.Parse(arr[4]);
                designID = int.Parse(arr[5]);
                logo = int.Parse(arr[6]);
            }
        }
    }
}