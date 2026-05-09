using UnityEngine;
using System.Collections;
using UnityEngine.UI;

namespace Fulbo.UI
{
    public class ButtonCustom : MonoBehaviour
    {
        [SerializeField] protected Text field;
        [SerializeField] float delayToSelectedAnim;
        [SerializeField] AnimationClip selectedAnim;
        [SerializeField] AnimationClip idleAnim;
        [SerializeField] protected types btnType;
        [SerializeField] protected Animator anim;
        public bool isSelected;
        public int buttonID;
        protected System.Action<int> OnClickCallback;
        protected Button button;

        public GameObject[] items;

        public enum types
        {
            UI_GENERIC,
            TAB,
            LIST_ITEM,
            KEY_PRESSED,
            CLOSE
        }
        protected bool disableIfClicked;

        void Awake()
        {
            anim = GetComponent<Animator>();
            button = GetComponent<Button>();
            Navigation customNav = new Navigation();
            customNav.mode = Navigation.Mode.None;
            if (button != null)
            {
                button.navigation = customNav;
                button.onClick.AddListener(OnClick);
            }
        }
        public void Init(int id, System.Action<int> OnClickCallback, string text = "", bool disableIfClicked = true)
        {
            anim = GetComponent<Animator>();
            this.buttonID = id;
            this.OnClickCallback = OnClickCallback;
            if (field != null) field.text = text;
            this.disableIfClicked = disableIfClicked;

            if(text == "items")
            {
                items[0].SetActive(false);
                items[1].SetActive(true);
            }
        }
        public void SetText(string text)
        {
            if (field != null) field.text = text;
        }
        public virtual void OnClick()
        {
            Events.OnButtonPressed(btnType); 
            if (OnClickCallback != null)
                OnClickCallback(buttonID);
        }
        private void OnDisable()
        {
            CancelInvoke();
        }
        public void OnSelected(bool isOn)
        {            
            isSelected = isOn;
            if (anim == null)
            {
                //Debug.Log("No anim in ButtonCustom " + gameObject.name);
                return;
            }
            if (isOn)
                SelectedAnim();
            else
                Idle();
            SetSelected();
        }
        private void OnEnable()
        {
            if (isSelected && delayToSelectedAnim > 0)
                Invoke("SelectedAnim", delayToSelectedAnim);
        }        
        void Idle()
        {
            if (button == null) return;
            if(disableIfClicked) SetInteraction(true);
            if (idleAnim != null) Anim(idleAnim.name);
        }
        void SelectedAnim()
        {
            if (button == null) return;
            if (disableIfClicked) SetInteraction(false);
            if (selectedAnim != null) Anim(selectedAnim.name);
        }
        void SetSelected()
        {
            if (button == null) return;
            if (anim != null)
                anim.SetBool("isOn", isSelected);
        }
        public void Anim(string animName)
        {
            if (anim != null)
                anim.Play(animName);
        }
        public void SetBool(string key, bool value)
        {
            if (anim != null)
                anim.SetBool(key, value);
        }
        private void OnDestroy()
        {
            OnClickCallback = null;
            OnDestroyed();
            if(GetComponent<Button>() != null)
                GetComponent<Button>().onClick.RemoveAllListeners();
        }
        protected virtual void OnDestroyed(){}
        public void SetInteraction(bool isOn)
        {
            if (button == null) return;
            button.interactable = isOn;
        }
        public void SetType(types type)
        {
            this.btnType = type;
        }
    }
}