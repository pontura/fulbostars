using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

namespace Fulbo.Dashoard
{
    public class DashboardUI : MonoBehaviour
    {
        public DashboardSignal header;
        public DashboardSignal two_characters;
        public DashboardSignal Hero;
        public DashboardSignal Double;
        public DashboardSignal ThirdQuarterLeft;
        public DashboardSignal ThirdQuarterRight;
        public DashboardSignal Triple;
       // public DashboardSignal Win;

        public List<DashboardSignal> all;
        public Transform container;
        [SerializeField] Scrollbar scrollBar;
        [SerializeField] float speed;

        private void Start()
        {
            gameObject.SetActive(false);
            Events.OnShowDasboard += OnShowDasboard;
        }
        private void OnDestroy()
        {
            Events.OnShowDasboard -= OnShowDasboard;
        }
        private void Update()
        {
            scrollBar.value -= Time.deltaTime * speed;
            if (scrollBar.value <= 0) scrollBar.value = 1;
        }

        void OnShowDasboard(bool showIt)
        {
            if(showIt)
            {
                gameObject.SetActive(true);
                Utils.RemoveAllChildsIn(container);
                scrollBar.value = 1;
                Add(header);
                Add(Hero);
                Add(Double);
                Add(two_characters);
                Add(ThirdQuarterLeft);
                Add(ThirdQuarterRight);
                Add(Triple);
            }
            else
            {
                gameObject.SetActive(false);
            }
           
        }
        void Add(DashboardSignal a)
        {
            DashboardSignal dSignal = Instantiate(a, container);
            all.Add(dSignal);
            dSignal.Init(this);
        }
        public void DestroySignal(DashboardSignal s)
        {
           // Debug.Log("Destroy Signal " + s);
            all.Remove(s);
            Destroy(s.gameObject);
        }
    }

}