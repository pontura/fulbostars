using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Fulbo.Game.Xtras
{
    public class TextFromDB : MonoBehaviour
    {
        [SerializeField] string text_in_db;
        void Start()
        {
            string realTet = Data.Instance.texts.Get(text_in_db);
            UnityEngine.UI.Text field = GetComponent<UnityEngine.UI.Text>();

            if (field == null)
                Debug.Log("No text-field for database text: " + text_in_db);
            else if (realTet != "")
                field.text = realTet;
            else
                Debug.Log("No text in database for: " + text_in_db);
        }
    }
}
