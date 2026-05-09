using Fulbo.DB;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Fulbo.UI
{
    public class MyTeamTabs : MonoBehaviour
    {
        [SerializeField] MyTeamSelector myTeamSelector;
        [SerializeField] Button opponents;
        [SerializeField] ReplaceSignal replaceSignal;

        private void Start()
        {
            Events.CharacterUpdatedData += CharacterUpdatedData;
        }
        private void OnDestroy()
        {
            Events.CharacterUpdatedData -= CharacterUpdatedData;
        }
        public void Init()
        {
            replaceSignal.SetState(false);
        }
        public void OnReplaceMoment(DBUserData.DBCharacterData dbUserData)
        {
           // myTeamSelector.Filter(dbUserData.IsGoalkeeper());
            replaceSignal.Init(dbUserData);
        }
        void CharacterUpdatedData(DB.DBUserData.DBCharacterData u)
        {
           // myTeamSelector.ShowAll();
        }
    }

}