using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Fulbo.UI.EditTeam
{
    public class ClubShield : MonoBehaviour
    {
        //[SerializeField] Image maskShape;
        [SerializeField] Transform container;
        [SerializeField] Transform containerForPattern;
        Image[] colors;
        [SerializeField] GameObject patternGO;
        //[SerializeField] GameObject[] patterns;
        //[SerializeField] GameObject[] shapes;

        [SerializeField]  ClubData data;
        public void Init()
        {
            Init(Data.Instance.clubsData.GetData(0));
        }
        public void Init(ClubData data)
        {
            this.data = data;
            if(data == null)
            {
                Debug.Log("No hay data para el escudo");
                return;
            }
            if (patternGO != null)
                patternGO.transform.SetParent(transform);
            Utils.RemoveAllChildsIn(container);

            if (data.shieldDesignID < 1) data.shieldDesignID = 1; 

            GameObject go = Instantiate(Data.Instance.clubsData.shapes[data.shieldDesignID - 1].asset, container);

            containerForPattern = go.GetComponentInChildren<Image>().transform;

            if (patternGO != null)
                patternGO.transform.SetParent(containerForPattern);

            patternGO = Instantiate(Data.Instance.clubsData.patterns[data.designID-1].asset);
            patternGO.transform.SetParent(containerForPattern);   

            colors = patternGO.GetComponentsInChildren<Image>();
            int id = 0;
            foreach (Image i in colors)
            {
                if (id == 0) i.color = data.GetColor(1);
                else i.color = data.GetColor(2);
                id++;
            }
            patternGO.transform.localPosition = Vector2.zero;
            patternGO.transform.localScale = Vector2.one;

            SetLogo(data.logo);
        }
        void SetLogo(int logoID)
        {
            foreach (Image l in gameObject.GetComponentsInChildren<Image>())
            {
                if (l.name == "logo_container")
                {
                    Image logo = l.GetComponent<Image>();
                    if (logoID == 0)
                        logo.enabled = false;
                    else
                    {
                        logo.enabled = true;
                        logo.sprite = Data.Instance.clubsData.logos[logoID - 1];
                        float scale = 0.5f;
                        logo.transform.localScale = new Vector2(scale, scale);
                    }
                    return;
                }

            }
        }
        public void OnClicked()
        {
           // Events.OpenAccountSettings();
        }
    }
}
