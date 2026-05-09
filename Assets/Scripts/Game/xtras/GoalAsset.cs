using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Fulbo.Game.Xtras
{
    public class GoalAsset : MonoBehaviour
    {
        public GameObject goalAsset;
        public GameObject notGoalAsset;
        public GameObject[] teams;
        bool isOn;

        void Awake()
        {
            switch(UnityEngine.SceneManagement.SceneManager.GetActiveScene().name)
            {
                case "Game":
                case "Tutorial":
                    isOn = true;
                    break;
            }
        }
        void Start()
        {
            if (notGoalAsset == null || goalAsset == null) return;
            if (isOn)
            {
                Reset();
                Events.OnGoal += OnGoal;
            }
        }
        void Reset()
        {
            notGoalAsset.SetActive(true);
            goalAsset.SetActive(false);
        }
        void OnDestroy()
        {
            Events.OnGoal -= OnGoal;
        }
        void OnGoal(int teamID, Character character)
        {
            if (teams == null || teams.Length == 0 || teams[character.teamID - 1] == null)
                return;

            notGoalAsset.SetActive(false);
            goalAsset.SetActive(true);

            foreach (GameObject go in teams)
                go.SetActive(false);

            teams[character.teamID - 1].SetActive(true);

            Invoke("Reset", 5);
        }
    }
}
