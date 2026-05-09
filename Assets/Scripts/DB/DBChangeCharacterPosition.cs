using UnityEngine;
using System.Collections;
using System;
using UnityEngine.Networking;
using System.Collections.Generic;
using Fulbo.Game;

namespace Fulbo.DB
{
    public class DBChangeCharacterPosition
    {
        [Serializable]
        class RequestData
        {
            public int position;
            public string device;
            public string version;
            public string hash;
        }

        public void Request(int characterId, int position, string url, System.Action<bool, string> OnSuccess)
        {
            Debug.Log("[REQUEST] ChangeCharacterPosition  url: " + url);
            WWWForm form = new WWWForm();

            RequestData tData = new RequestData();
            tData.position = position;
            tData.version = Application.version;
            tData.device = Application.platform.ToString();
            tData.version = Application.version;

            string hashText =
                DBManager.Instance.Email +
                DBManager.HASH_SALT1 +
                characterId +
                position;

            tData.hash = Utils.SHA(hashText);

            string json = JsonUtility.ToJson(tData, true);
            DBManager.Instance.Request(url, json, OnSuccess, "PUT");//, Data.Instance.texts.Get("http_updating_user"));

            //Analytics
            Dictionary<string, object> param = new Dictionary<string, object>();
            DB.DBUserData.DBCharacterData cData = DB.DBManager.Instance.DbUserData.data.GetPlayerByID(characterId);
            param["characterName"] = cData.AvatarName();
            param["prevRole"] = Data.Instance.myTeam.GetCharacterType(cData.player_id);
            param["role"] = position;
            param["rarity"] = cData.rarity;

            Events.OnTrack("CharacterChangedPosition", param);
        }
    }
}
