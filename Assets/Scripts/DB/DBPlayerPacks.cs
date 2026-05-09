using UnityEngine;
using System.Collections;
using System;
using UnityEngine.Networking;
using System.Collections.Generic;

namespace Fulbo.DB
{
    public class DBPlayerPacks
    {
        [Serializable]
        class SendData
        {
            public string device;
            public string version;
            public string hash;
        }

        //Compra un pack de jugadores dado el id del pack (1 a 4), 
        //la cantidad de jugadores del pack (1, 5, 8 u 11, el pack de id 4 solo permite cantidad 1 u 11) 
        //y la currency a pagar (soft o hard, el pack de id 4 solo permite hard). 
        //En los packs de 11 se garantiza un jugador de la rareza del pack. Devuelve los characters obtenidos.

        public void Buy(int pack, int quantity, string currency, System.Action<bool, string> OnSuccess)
        {
            string url;
            //POST /users/{email}/buyPack/{pack}/{quantity}/{currency}
            url = DBManager.Instance.URL + "/users/" + DBManager.Instance.Email + "/buyPack/" + pack + "/" + quantity + "/" + currency;

            DBUserData uData = DBManager.Instance.DbUserData;
            string hashString =
                DBManager.Instance.Email +
                DBManager.HASH_SALT1 + 
                pack + 
                quantity + 
                currency;

            SendData d = new SendData();
            d.device = Application.platform.ToString();
            d.version = Application.version;
            d.hash = Utils.SHA(hashString);

            string json = JsonUtility.ToJson(d, true);
            DBManager.Instance.Request(url, json, OnSuccess, "POST", "Buy Players");
            
        }
    }
}
