using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Fulbo.UI.Paginator
{
    public class PaginatorManager : MonoBehaviour
    {
        [SerializeField] int paginatorID;
        [SerializeField] int total;
        int totalItemsInPage;

        [SerializeField] ButtonCustom nextButton;
        [SerializeField] ButtonCustom prevButton;
        [SerializeField] ButtonCustom button;
        [SerializeField] List<ButtonCustom> buttons;
        [SerializeField] Transform container;

        System.Action<int, int> OnChange;
        bool started;

        public void Init(int totalItems, int totalItemsInPage, System.Action<int, int> OnChange)
        {
            buttons.Clear();
            this.totalItemsInPage = totalItemsInPage;
            this.OnChange = OnChange;
            SetPaginatorID(0); 
            this.total = (int)( Mathf.Ceil( (float)totalItems / (float)totalItemsInPage ) );
            if (total <= 1)
            {
                RefreshAction();
                gameObject.SetActive(false); return;
            }
            else
            {
                gameObject.SetActive(true);
                if (!started)
                {
                    prevButton.Init(-2, Goto, "<");
                    nextButton.Init(-1, Goto, ">");
                    started = true;
                }
                InitPaginator();
                Goto(0);
            }

        }
        void Goto(int buttonID)
        {
            if (buttonID == -1)             paginatorID++;
            else if(buttonID == -2)         paginatorID--;

            if (paginatorID < 0)                { paginatorID = 0; return; }
            else if (paginatorID > total - 1)   { paginatorID = total - 1; return; }

            Refresh();
        }
        private void InitPaginator()
        {
            nextButton.gameObject.transform.SetParent(transform.parent.transform);
            prevButton.gameObject.transform.SetParent(transform.parent.transform);
            Utils.RemoveAllChildsIn(container);
            prevButton.gameObject.transform.SetParent(container);
            AddItems();
            nextButton.gameObject.transform.SetParent(container);
        }
        void AddItems()
        {
            for (int a = 0; a < total; a++)
            {
                ButtonCustom buttonNew = Instantiate(button, container);
                buttonNew.Init(a, OnPaginatorButtonClicked, (a + 1).ToString() );
                buttons.Add(buttonNew);
            }
        }
        public void SetPaginatorID(int paginatorID)
        {
            this.paginatorID = paginatorID;
        }
        void OnPaginatorButtonClicked(int paginatorID)
        {
            this.paginatorID = paginatorID;
            Refresh();
        }
        void Refresh()
        {
            foreach (ButtonCustom button in buttons)
            {
                if (button.buttonID == paginatorID)
                    button.OnSelected(true);
                else
                    button.OnSelected(false);
            }
            RefreshAction();
        }
        void RefreshAction()
        {
            int from = paginatorID * totalItemsInPage;
            int to = from + totalItemsInPage;
            OnChange(from, to);
        }
    }
}
