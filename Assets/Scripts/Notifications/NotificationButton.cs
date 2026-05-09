using UnityEngine;
using UnityEngine.UI;

using System.Collections;

namespace Fulbo.UI
{
    public class NotificationButton : ButtonCascade
    {
        [SerializeField] Text descField;
        [SerializeField] GameObject readGO;

        public void SetContent(string title, string date, bool read)
        {
            field.text = title;
            descField.text = date;
            readGO.SetActive(!read);
        }
    }
}
