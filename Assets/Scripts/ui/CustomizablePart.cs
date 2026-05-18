using Fulbo.Game;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Fulbo.UI
{
    public class CustomizablePart : MonoBehaviour
    {
        bool setted;
        [SerializeField] int teamID;

        void Start()
        {
            if (setted)
                return;
            SetDesign();
        }
        void SetDesign()
        {
            Character character = GetComponentInParent<Character>();

            if (character)
                teamID = character.teamID;

            string[] arr = gameObject.name.Split("_"[0]);
            string colorName = arr[arr.Length - 1];
            ClubData clubData = Data.Instance.clubsData.GetData(teamID);
            print("____________colorName " + colorName + " clubData:" + clubData.name_abr);
            switch (colorName)
            {
                case "A": GetComponent<SpriteRenderer>().color = clubData.GetColor(1); break;
                case "B": GetComponent<SpriteRenderer>().color = clubData.GetColor(2); break;
                case "C": GetComponent<SpriteRenderer>().color = clubData.GetColor(3); break;
                case "D": GetComponent<SpriteRenderer>().color = clubData.GetColor(4); break;
            }

            setted = true;
        }
        public void Refresh()
        {
            SetDesign();
        }

    }
}
