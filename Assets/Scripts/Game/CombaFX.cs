using UnityEngine;

namespace Fulbo.Game
{
    public class CombaFX : MonoBehaviour
    {
        int control_id;
        Rigidbody rb;
        float force = 1500;
        [SerializeField] GameObject combaSignal;
        [SerializeField] GameObject up;
        [SerializeField] GameObject down;
        bool isOn;

        void Start()
        {
            Events.OnBallKicked += OnBallKicked;
            rb = GetComponent<Rigidbody>();
        }
        void OnDestroy()
        {
            Events.OnBallKicked -= OnBallKicked;
        }
        void OnBallKicked(CharacterStates.kickTypes type, float force, Character character)
        {
            Reset();
            if (character == null) return;
            if (character.isBeingControlled && (type == CharacterStates.kickTypes.HARD || type == CharacterStates.kickTypes.KICK_TO_GOAL)  && character.type != Character.types.GOALKEEPER)
                Init(character.control_id);
        }
        private void Init(int control_id)
        {
            isOn = true;
            combaSignal.SetActive(true);
            this.control_id = control_id;
        }
        public void Reset()
        {
            isOn = false;
            combaSignal.SetActive(false);
            control_id = 0;
        }
        void Update()
        {
            if (control_id == 0) return;
            if (Fulbo.Game.GameManager.Instance != null)
            {
                float _y = GameManager.Instance.inputManagerGame._y;
                float forceReal = force * _y * (Time.deltaTime);
                rb.AddForce(Vector3.forward * forceReal);
                if(isOn)
                    SetComba(_y);
            }
        }
        void SetComba(float _y)
        {
            up.SetActive    (_y > 0);
            down.SetActive  (_y < 0);
            combaSignal.transform.eulerAngles = Vector3.zero;
        }
    }

}