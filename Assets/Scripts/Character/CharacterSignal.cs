using Fulbo.UI;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace Fulbo.Game
{
    public class CharacterSignal : MonoBehaviour
    {

        public Sprite[] sprites;
        public SpriteRenderer sr;

        public SpriteRenderer[] all;
        [SerializeField] Transform container;
        Transform target;
        public ProgressBar powerupsProgressBar; // para el click and release INGAME
        Color lastColor;
        states state;
        enum states
        {
            IDLE,
            SNAPPING
        }
        private void Start()
        {
            Events.OnPowerupActive += OnPowerupActive;
            OnPowerupActive(false, null);
        }
        private void OnDestroy()
        {
            Events.OnPowerupActive -= OnPowerupActive;
        }
        void OnPowerupActive(bool isOn, Character character)
        {
            if (character == null) return;
        }
        public void Init(Color color, int playerID)
        {
            if (lastColor != color)
            {
                lastColor = color;
                foreach (SpriteRenderer sr in all)
                    sr.color = color;
                powerupsProgressBar.image.color = color;
            }
            if (powerupsProgressBar != null)
            {
                powerupsProgressBar.SetOff();
            }

            if (Data.Instance.mode == Data.modes.PARTYMODE)
            {
                sr.sprite = sprites[playerID - 1];             
            }
            else
            {
                sprites = null;
                sr.enabled = false;
            }
        }
        public void MoveTo(Transform target)
        {
            this.target = target;
            state = states.SNAPPING;
        }
        void Update()
        {
            if (state == states.SNAPPING)
            {
                float dist = Vector3.Distance(transform.position, target.transform.position);              
                transform.position = Vector3.Lerp(transform.position, target.transform.position, Time.deltaTime*15);
                if (dist < 0.75f)
                {
                    state = states.IDLE;
                    transform.position = target.transform.position;
                }
            }
            else if(target != null)
                transform.position = target.transform.position;
            //  if (parentT != null)
            //     container.transform.localScale = new Vector3(transform.parent.transform.localScale.x, 1, 1);
        }
    }
}
