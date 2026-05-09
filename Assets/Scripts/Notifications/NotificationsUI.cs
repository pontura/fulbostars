using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using System.Collections.Generic;

namespace Fulbo.UI
{
    public class NotificationsUI : CascadeList
    {
        [SerializeField] GameObject panel;
        [SerializeField] NotificationButton line;
        [SerializeField] Transform container;
        List<NotificationButton> lines;
        int id;

        private void Start()
        {
            Close();
            Events.OpenNotifications += OpenNotifications;
            Events.CheckForImportantNotifications += CheckForImportantNotifications;
            
        }
        public void CheckForImportantNotifications(string sceneName)
        {
            string not = DB.DBManager.Instance.DbUserData.data.gameData.notifications;
            string[] readNotifs;
            if (not == null || not.Length < 1)
                readNotifs = new string[]{""};
            else
                readNotifs = not.Split(","[0]);
            int id = 0;
            foreach (NotificationsData.NotifData data in NotificationsData.Instance.all)
            {
                bool read = data.read;
                if (data.important == sceneName)
                {
                    foreach (string s in readNotifs)
                    {
                        int num;
                        if (int.TryParse(s, out num))
                            if (num == data.id)
                                read = true;
                    }
                    if (!read)
                    {
                        OpenPopup(id, data);
                    }
                }
                id++;
            }
        }

        private void OnDestroy()
        {
            Events.OpenNotifications -= OpenNotifications;
            Events.CheckForImportantNotifications -= CheckForImportantNotifications;
        }
        public void OpenNotifications()
        {
            string[] readNotifs = DB.DBManager.Instance.DbUserData.data.gameData.notifications.Split(","[0]);
            lines = new List<NotificationButton>();
            panel.SetActive(true);
            string title = Data.Instance.texts.Get("notifications");
            Data.Instance.ui.SetBackButton(true, Back, title);
            InitCascade();
            Utils.RemoveAllChildsIn(container);
            int id = 0;
            foreach (NotificationsData.NotifData data in NotificationsData.Instance.all)
            {
                NotificationButton newLine = Instantiate(line, container);
                newLine.Init(id, OnClicked);
                id++;
                bool read = data.read;
                if (!data.read)
                {
                    foreach (string s in readNotifs)
                    {
                        int num;
                        if (int.TryParse(s, out num))
                            if (num == data.id)
                                read = true;
                    }
                }
                newLine.SetContent(data.title, data.date, read);
                AddToCascade(newLine);
                lines.Add(newLine);
            }
            StartCascade();
        }
        void OnClicked(int _id)
        {
            NotificationsData.NotifData data = NotificationsData.Instance.all[_id];
            lines[_id].SetContent(data.title, data.date, true);
            OpenPopup(_id, data);
        }
        void OpenPopup(int id, NotificationsData.NotifData data)
        {
            string title = data.title;
            string text = data.text;
            id = data.id;
            data.read = true;
            Events.PopupText(title, text, OnRead);
            DB.DBManager.Instance.DbUserData.data.gameData.AddNotificationRead(id, null);
        }
        void OnRead(bool isDone)
        {
        }
        public void Back()
        {
            Data.Instance.ui.SetBackButton(false);
            Close();
        }
        public void Close()
        {
            panel.SetActive(false);
        }
    }
}
