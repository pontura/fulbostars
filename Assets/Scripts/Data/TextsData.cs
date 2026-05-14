using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using Fulbo.AssetsBundle;

namespace Fulbo
{
    public class TextsData : MonoBehaviour
    {
        public TextAsset fileAsset;
        public TextData data;
        public string url;

        [Serializable]
        public class TextData
        {
            public DialoguesData dialogs;
            public CharacterData[] characters;
            public CharacterData[] goalkeepers;
            public CharacterData[] referis;
        }
        [Serializable]
        public class CharacterData
        {
            public int id;
            public DialoguesData dialogs;
        }

        [Serializable]
        public class DialoguesData
        {
            public List<string> random;
            public List<string> goal;
            public List<string> full;
            public List<string> init;
        }
        System.Action OnDone;
        public void Init(System.Action OnDone)
        {
            this.OnDone = OnDone;
            if (Data.Instance.loadType == Data.loadTypes.LOCAL || Data.Instance.loadType == Data.loadTypes.DATABASE)
            {
                AllLoaded(fileAsset.text);
            }
            else
            {
                print("texts_" + Data.Instance.langsManager.GetLang() + ".json");
                AssetsBundleLoader abl = AssetsBundleManager.Instance.assetsBundleLoader;
                AllLoaded(abl.GetJsonText("texts_" + Data.Instance.langsManager.GetLang() + ".json"));
            }
        }
        
        private void AllLoaded(string text)
        {
            data = JsonUtility.FromJson<TextData>(text);
            OnDone();
        }
        public string GetRandomReferiDialogue(string dialogueType)
        {
            int referiID = CharactersData.Instance.GetReferi().id;
            foreach (CharacterData d in data.referis)
            {
                if (d.id == referiID)
                {
                    return GetText(dialogueType, d, false);
                }
            }
            return "";
        }
        public string GetRandomDialogue(string dialogueType, int characterID, bool isGoalKeeper = false)
        {
            CharacterData characterData = GetCharactersData(characterID, isGoalKeeper);
            return GetText(dialogueType, characterData, true);
        }
        string GetText(string dialogueType, CharacterData characterData, bool isPlayer)
        {
            List<string> arr = new List<string>();
            switch (dialogueType)
            {
                case "random":
                    if (characterData.dialogs.random.Count > 0 || !isPlayer)
                        arr = characterData.dialogs.random;
                    break;
                case "goal":
                    if (UnityEngine.Random.Range(0, 10) < 4 && characterData.dialogs.goal.Count > 0 || !isPlayer)
                        arr = characterData.dialogs.goal;
                    else
                        arr = data.dialogs.goal;
                    break;
                case "init":
                    if (UnityEngine.Random.Range(0, 10) < 4 && characterData.dialogs.init.Count > 0 || !isPlayer)
                        arr = characterData.dialogs.init;
                    else
                        arr = data.dialogs.init;
                    break;
                default:
                    if (UnityEngine.Random.Range(0, 10) < 4 && characterData.dialogs.full.Count > 0 || !isPlayer)
                        arr = characterData.dialogs.full;
                    else
                        arr = data.dialogs.full;
                    break;
            }
            if (arr == null || arr.Count == 0)
                return null;
            return arr[UnityEngine.Random.Range(0, arr.Count)];
        }
        public CharacterData GetCharactersData(int characterID, bool isGoalKeeper = false)
        {
            if (isGoalKeeper)
            {
                foreach (CharacterData data in data.goalkeepers)
                {
                    if (data.id == characterID)
                        return data;
                }
            }
            foreach (CharacterData data in data.characters)
            {
                if (data.id == characterID)
                    return data;
            }
            Debug.LogError("No character id: " + characterID + "   isgoalkeeper : " + isGoalKeeper);
            return null;
        }
        public CharacterData GetReferisData(int characterID)
        {
            foreach (CharacterData data in data.referis)
            {
                if (data.id == characterID)
                    return data;
            }
            return null;
        }
        public string GetPositionName(bool isGoalkeeper, int uniqueID, bool fullName = false)
        {
            if (isGoalkeeper)
                if (fullName)
                    return Data.Instance.texts.Get("position_goalkeeper_full");
                else
                    return Data.Instance.texts.Get("position_goalkeeper");
            else
            {
                int originalTypeIDByPosition = Data.Instance.myTeam.GetCharacterType(uniqueID);
                return GetPositionString(originalTypeIDByPosition, fullName);
            }
            return "";
        }
        public string GetPositionName(CharactersData.CharacterData data, bool fullName = false)
        {
            return GetPositionName(data.isGoalkeeper, data.uniqueID, fullName);
        }
        public string GetPositionString(int posID, bool fullName)
        {
            switch (posID)
            {
                case 0:
                    if (fullName)
                        return Data.Instance.texts.Get("position_def_full");
                    else
                        return Data.Instance.texts.Get("position_def");
                case 1:
                    if (fullName)
                        return Data.Instance.texts.Get("position_mid_full");
                    else
                        return Data.Instance.texts.Get("position_mid");
                case 2:
                    if (fullName)
                        return Data.Instance.texts.Get("position_for_full");
                    else
                        return Data.Instance.texts.Get("position_for");
            }
            return "";
        }
    }
}
