using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

namespace Fulbo.UI
{
    public class CascadeList : MonoBehaviour
    {
        [SerializeField] float delayToStart;
        [SerializeField] float dalayBetweenChilds;
        List<ButtonCascade> allButtons;
        int buttonID = 0;

        protected void InitCascade()
        {
            //Debug.Log("init cascade");
            buttonID = 0;
            allButtons = new List<ButtonCascade>();
        }
        protected void AddToCascade(ButtonCascade button)
        {
            button.buttonID = buttonID;
            buttonID++;
            button.SetAllActive(false);
            allButtons.Add(button);
        }
        protected void StartCascade()
        {
            StopAllCoroutines();
            StartCoroutine(CascadeC());
        }
        protected void Reset()
        {
            StopAllCoroutines();
            SetAllVisisble(false);
        }
        protected void ForceReady()
        {
            StopAllCoroutines();
            SetAllVisisble(true);
        }
        IEnumerator CascadeC()
        {
            yield return new WaitForSeconds(delayToStart);
            SetAllVisisble(false);
            foreach (ButtonCascade buttonCascade in allButtons)
            {
                SetVisisble(buttonCascade, true);
                AudioManager.Instance.PlaySound("ui", "ui/ui_listItem_on", false);
                buttonCascade.Enter(this); 
                 yield return new WaitForSeconds(dalayBetweenChilds);
            }
            SetAllVisisble(true);
        }
        void SetAllVisisble(bool isOn)
        {
            foreach (ButtonCascade button in allButtons)
                button.SetAllActive(isOn);
        }
        void SetVisisble(ButtonCascade button, bool isOn)
        {
            button.SetAllActive(isOn);
        }
        public virtual void OnButtonCascadePressed(ButtonCascade cascadeButton) {  }
    }
}