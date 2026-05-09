using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Fulbo.Game
{
    public class Referi : Character
    {
        Vector3 initialPos;
        Ball ball;
        public bool isInsideGame = true;
        GameManager gameManager;

        private void Awake()
        {
            initialPos = transform.position;
        }
        public override void OnStart()
        {
            initialPos = transform.position;
            Events.OnGameStatusChanged += OnGameStatusChanged;
            Events.OnRestartGame += OnRestartGame;
        }
        public void OnDestroy()
        {
            Events.OnRestartGame -= OnRestartGame;
            Events.OnGameStatusChanged -= OnGameStatusChanged;
        }
        public void OnRestartGame()
        {
            transform.position = initialPos;
            states.Stopped();
        }
        void OnGameStatusChanged(Fulbo.Game.GameManager.states state)
        {
            switch (state)
            {
                case Fulbo.Game.GameManager.states.GOAL:
                    Invoke("Pita", 0.5f); break;
                case Fulbo.Game.GameManager.states.PLAYING:
                    states.Pita(0); break;
            }
        }
        void Pita()
        {
            states.Pita(0);
        }
        public void InitReferi(CharactersManager charactersManager, GameObject asset_to_instantiate)
        {
            gameManager = Fulbo.Game.GameManager.Instance;
            ball = gameManager.ball;
            scaleFactor = Data.Instance.settings.GetSetting("scaleFactor");
            this.charactersManager = charactersManager;
            speed = Data.Instance.settings.GetSetting("referiSpeed");
            stats.speed = speed;
            GameObject asset = Instantiate(asset_to_instantiate);
            asset.transform.SetParent(characterContainer);
            asset.transform.localEulerAngles = asset.transform.localPosition = Vector3.zero;
            asset.transform.localScale = new Vector3(scaleFactor, scaleFactor, scaleFactor);
            //  actions.Init(asset, 0);
          //  Invoke("ChangeZ", Random.Range(12, 14));
            int id = CharactersData.Instance.GetReferi().id;
            data = Data.Instance.textsData.GetReferisData(id);
            dataSources = CharactersData.Instance.GetReferi(id);
            states = GetComponent<CharacterStates>();
            states.Init(asset);
            SetLimits();
            type = types.REFERI;
        }
        public override void SetPosition(float _x, float _y)
        {
            //if (!isInsideGame) return;
            //MoveTo(_x, _y);
        }
        float timer;
        void Update()
        {
            if (gameManager == null) return;
            if (gameManager.state != Fulbo.Game.GameManager.states.PLAYING) return;
            states.UpdatedByCharacter();
            if (!isInsideGame) return;
            if (ball == null)   return;
            if (states.CanMove() && 
                gameManager.state != Fulbo.Game.GameManager.states.GOAL && states.currentState.type != CharacterStates.types.SPECIAL_ACTION)
            {
                timer += Time.deltaTime;
                int _z = 0;
                int _x = 0;
                float dest_x = ball.transform.position.x / 1.5f;
                float dest_z = ball.transform.position.z / 1.5f;
                float dist_x = Mathf.Abs(transform.position.x - dest_x);
                float dist_z = Mathf.Abs(transform.position.z - dest_z);

                if (dist_z > 1)
                {
                    if (transform.position.z < dest_z)
                        _z = 1;
                    else
                        _z = -1;
                }
                if (dist_x > 1)
                {
                    if (transform.position.x < dest_x)
                        _x = 1;
                    else
                        _x = -1;
                }

                if (timer < 3) _x = 0;
                if (timer < 5) _z = 0;

                MoveTo(_x, _z);
            }
        }
        public override Vector3 MoveTo(float _x, float _y)
        {
            if (_x == 0 && _y == 0)
            {
                if (states.currentState.type != CharacterStates.types.IDLE)
                    states.currentState.Stopped();
                return Vector2.zero;
            }
            else
            {
                states.LookTo((int)Mathf.Ceil(_x));
                if (states.currentState.type != CharacterStates.types.RUN)
                {
                    states.currentState.Move(1);
                }
            }

            Vector3 pos = transform.position;

            if (pos.z > limits_y.x) _y = -1;
            else if (pos.z < limits_y.y) _y = 1;

            Vector3 speedX = Vector3.right * _x * speed;
            Vector3 speedZ = Vector3.forward * _y * speed;

            Vector3 forwardVector = (speedX + speedZ) * Time.deltaTime;

            transform.Translate(forwardVector);
            return forwardVector;
        }
    }
}