using Fulbo.Game;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Fulbo.UI
{
    public class CustomizableDesign : MonoBehaviour
    {
        [SerializeField] private GameObject[] designs;
        bool setted;
        void Start()
        {
            if (setted) return;
            setted = true;
            SetDesign();
        }
        void SetDesign()
        {
            Character character = GetComponentInParent<Character>();
            foreach (GameObject go in designs)
            {
                if(go != null)
                    go.SetActive(false);
            }
            ClubData clubData;

            if (character == null)
                clubData = Data.Instance.clubsData.GetData(0);
            else
                clubData = Data.Instance.clubsData.GetData(character.teamID);
            if (clubData.designID < 1)
                clubData.designID = 1;

            int designID = clubData.designID - 1;
            if(designs.Length > designID && designs[designID] != null)
                designs[designID].SetActive(true);
        }
        public void Refresh()
        {
            SetDesign();
            foreach (CustomizablePart cp in GetComponentsInChildren<CustomizablePart>())
                cp.Refresh();
        }
    }
}
