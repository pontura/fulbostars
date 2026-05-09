using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Fulbo.UI
{
    public class ShareAppUI : MonoBehaviour
    {
        [SerializeField] GameObject panel;
        [SerializeField] ButtonCustom closeBtn;
        [SerializeField] ButtonCustom shareBtn;
        [SerializeField] ButtonCustom copyBtn;
        [SerializeField] GameObject mobileUI;
        [SerializeField] GameObject webGLUI;
        [SerializeField] InputField inputField;

        void Start()
        {
            Close();
            copyBtn.Init(1, Copy);
            closeBtn.Init(0, ButtonClicked);
            Events.OpenShareApp += OpenShareApp;
        }
        private void OnDestroy()
        {
            Events.OpenShareApp -= OpenShareApp;
        }
        public void OpenShareApp()
        {
            if (Application.platform == RuntimePlatform.Android || Application.platform == RuntimePlatform.IPhonePlayer)
            {
                shareBtn.Init(1, ButtonClicked, Data.Instance.texts.Get("share"));
                mobileUI.SetActive(true);
                webGLUI.SetActive(false);
            }
            else
            {
                copyBtn.SetText(Data.Instance.texts.Get("copy"));
                mobileUI.SetActive(false);
                webGLUI.SetActive(true);
                string link = Data.Instance.GetURLForReferrerLink() + "?referrer=" + DB.DBManager.Instance.DbUserData.data.id;
                inputField.text = link;
            }
            panel.SetActive(true);
        }
        void ButtonClicked(int id)
        {
            switch(id)
            {
                case 0:
                    Close(); break;
                case 1:
                    Share(); break;
            }
        }
        void Share()
        {
            string shareText = Data.Instance.texts.Get("share_text");
            string link = Data.Instance.GetURLForReferrerLink() + "?referrer=" + DB.DBManager.Instance.DbUserData.data.id;
            new NativeShare().SetSubject(shareText).SetText(link).Share();
            Close();
        }
        void Close()
        {
            panel.SetActive(false);
        }
        public void Copy(int id)
        {
            string link = Data.Instance.GetURLForReferrerLink() + "?referrer=" + DB.DBManager.Instance.DbUserData.data.id;
            GUIUtility.systemCopyBuffer = link;
            Close();
        }
    }
}
