using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Fulbo.UI.Carrousel
{
    public class CarrouselManager : MonoBehaviour
    {
        [SerializeField] CarrouselItem item_to_instantiate;
        [SerializeField] GameObject ui;
        [SerializeField] ButtonCustom nextButton;
        [SerializeField] ButtonCustom prevButton;
        [SerializeField] Transform container;
        [SerializeField] int total = 7;
        [SerializeField] float distanceToCenter = 400;
        [SerializeField] Camera camera;
        float _last_x;
        float speed = 10000;
        float automaticSpeed = 8;
        Vector3 repositionateTo;
        public int id = 1;

        [SerializeField] states state;
        enum states
        {
            IDLE,
            DRAGGING,
            REPOSITIONATE
        }
        private void Start()
        {
            nextButton.Init(1, MoveCarrousel, Data.Instance.texts.Get("next"));
            prevButton.Init(2, MoveCarrousel, Data.Instance.texts.Get("prev"));
        }
        public void AddItem(int id, GameObject go, int total)
        {
            this.id = id;
            this.total = total;
            CarrouselItem item = Instantiate(item_to_instantiate,container);
            item.InitCarrousel(id*-360/total, distanceToCenter, camera.transform);
            item.AddAsset(go);
        }
        private void OnEnable()
        {
            OnInit();
        }
        public void OnInit()
        {
            CancelInvoke();
            Invoke("Delayed", 0.1f);
        }
        void Delayed()
        {
            id = 0;
            MoveCarrousel(1);
        }
        float last_x_pos = 0;
        void PressInit()
        {
            _last_x = Mouse.current.position.ReadValue().x / Screen.width;
            last_x_pos = _last_x;
        }
        void OnPressed()
        {
            Vector2 pos = Mouse.current.position.ReadValue();

            float new_x = pos.x / Screen.width;
            if (_last_x != new_x)
            {
                float value = (_last_x - new_x) * speed * Time.deltaTime;
                container.localEulerAngles += new Vector3(0, value, 0);
                _last_x = new_x;
            }
            id = (int)Mathf.Round((container.transform.eulerAngles.y * total / 360));
            if (id == 7) id = 0;
            if (last_x_pos != new_x)
            {
                state = states.DRAGGING;
                SetOffUI();
            }
        }
        private void Update()
        {
#if UNITY_EDITOR || UNITY_STANDALONE
            if (Mouse.current.leftButton.wasPressedThisFrame) PressInit();
            if (Mouse.current.leftButton.isPressed) OnPressed();
            else if (state == states.DRAGGING) Repositionate();
#elif UNITY_ANDROID || UNITY_IOS

#endif

            if (state == states.REPOSITIONATE)
            {
                if (repositionateTo.y < 0.1f && container.transform.localEulerAngles.y > 300) repositionateTo = new Vector3(0, 360, 0);//HACK para el 360 = 0 angles:

                Vector3 rot = container.transform.localEulerAngles;
                container.transform.localEulerAngles = Vector3.Lerp(rot, repositionateTo, automaticSpeed*Time.deltaTime);
                if (Mathf.Abs(container.transform.localEulerAngles.y - repositionateTo.y) < 0.5f)
                {
                    Idle();
                }
            }
        }
        void SetOffUI()
        {
            ui.SetActive(false);
        }
        void OnRound(bool up)
        {
            if(up) container.transform.localEulerAngles = new Vector3(0, 359.9f, 0);
        }
        void Idle()
        {
            if (repositionateTo.y >= 359) repositionateTo.y = 0;
            container.transform.localEulerAngles = repositionateTo;
            state = states.IDLE;
            ui.SetActive(true);
        }
        void Repositionate()
        {
            state = states.REPOSITIONATE;
            float to = id * 360 / total;
            if (container.transform.localEulerAngles.y >180 && container.transform.localEulerAngles.y < 359 && id == 0)
            {
                to = 359.9f;
            }
            repositionateTo = new Vector3(0, to, 0);
        }
        public void MoveCarrousel(int next)
        {
            SetOffUI();
            AudioManager.Instance.PlaySound("common", "ui/ui_transicion", false);
            
            if (next == 1) id++;
            else id--;
            if (id < 0)
            {
                OnRound(true);
                id = total - 1;
            }
            else if (id > total - 1)
            {
                id = 0;
            }            
            Repositionate();
        }
    }
}
