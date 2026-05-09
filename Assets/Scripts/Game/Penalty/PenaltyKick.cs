using Fulbo.Input;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Fulbo.Game.Penalty
{
    public class PenaltyKick : MonoBehaviour
    {
        public states state;
        public enum states
        {
            IDLE,
            CHARACTER_RUNNING,
            KICK,
            END
        }
        [SerializeField] GameObject panel;
        [SerializeField] GameObject arrow;
        [SerializeField] GameObject shadow;
        [SerializeField] Transform target;

        float kikedTimeoutForComba;
        float penalty_sensibilitty = 5;

        int limits_x = 7;
        int limits_y = 5;
        float speed = 0.5f;

        public PenaltyInputManager inputManager;
        float _x, _y;
        float _penalty_sensibilitty;

        private void Start()
        {
            penalty_sensibilitty = Data.Instance.settings.GetSetting("penalty_sensibilitty");
            _penalty_sensibilitty = penalty_sensibilitty;
            Events.OnButtonClick += OnButtonClick;
        }
        private void OnDestroy()
        {
            Events.OnButtonClick -= OnButtonClick;
        }
        void OnButtonClick(int a, int s)
        {
            OnButtonUp();
        }
        void Update()
        {
            UpdateAim();
        }
        void UpdateAim()
        {
            if (state == states.CHARACTER_RUNNING || state == states.END) return;

            if (state == states.KICK)
            {
                kikedTimeoutForComba += Time.deltaTime;
                if (kikedTimeoutForComba > 0.3f)
                {
                    state = states.END;
                }
            }
            Vector3 pos = target.transform.position;
            pos.x += inputManager._x/ _penalty_sensibilitty;
            pos.y += inputManager._y / _penalty_sensibilitty;
            if (pos.x < -limits_x) pos.x = -limits_x;
            else if (pos.x > limits_x) pos.x = limits_x;

            if (pos.y < 1.5f) pos.y = 1.5f;
            else if (pos.y > limits_y) pos.y = limits_y;
           
            target.transform.position = pos;

            if (state != states.IDLE) return;
            arrow.transform.LookAt(pos);
            pos.y = 1;
            shadow.transform.LookAt(pos);
        }
        public void OnButtonUp()
        {
            if (state != states.IDLE) return;
            shadow.SetActive(false);
            arrow.SetActive(false);
            state = states.CHARACTER_RUNNING;
            AudioManager.Instance.PlaySoundOneShot("ingame", "_new/ingame/kick_ball");
            GameManager.Instance.GetComponent<PenaltyCharactersManager>().OnKick(target.transform.position, Kicked);
        }
        void Kicked()
        {
            _penalty_sensibilitty = penalty_sensibilitty * 10;
            state = states.KICK;
        }
        public void Catched()
        {
            state = states.END;
        }
    }
}