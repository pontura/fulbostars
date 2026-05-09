using Fulbo.Stadiums;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Fulbo.Game
{
    public class CameraInGame : MonoBehaviour
    {
        Animation anim;
        public Camera cam;
        float initial_y_position;
        [SerializeField] Transform target;
        Ball ball;
        float speed = 2.5f;
        public bool filmingPlayer;
        public float originalSize = 5f; // se pisa por : stadiumAsset.cameraSettings.originalZoomSize
        float filming_y;
        float offsetZ = 9;// se pisa por : stadiumAsset.cameraSettings.offsetZ
        float offset_x = 8f;
        int pos_z;
        int pos_z_max;
        float offsetX = 2;
        float max_pos_x;

        states state;
        enum states
        {
            PLAYING,
            GOAL,
            FREEZED
        }

        private void Awake()
        {
            anim = GetComponent<Animation>();
            cam = GetComponent<Camera>();
            Events.OnBallKicked += OnBallKicked;
            Events.CatchBall += CatchBall;
            Events.OnPowerupCharging += OnPowerupCharging;
        }
        private void OnDestroy()
        {
            Events.OnBallKicked -= OnBallKicked;
            Events.CatchBall -= CatchBall;
            Events.OnPowerupCharging -= OnPowerupCharging;
        }
        public void Init()
        {
            initial_y_position = transform.position.y;
            StadiumsData.StadiumAsset stadiumAsset = Fulbo.Game.GameManager.Instance.stadiumData.active.GetAssetBySelectedSize();
            originalSize = stadiumAsset.cameraSettings.originalZoomSize + 0.5f; // 5 era antes:
            offsetZ = stadiumAsset.cameraSettings.offsetZ; // 5 era antes:
            max_pos_x = (stadiumAsset.size_x / 2) - offset_x;
            pos_z = (int)(stadiumAsset.size_y / 2) + 1;
            pos_z_max = (int)(stadiumAsset.size_y / 5) - (((int)originalSize-5)*4);

        }
        public void Restart()
        {
            if (anim != null)
            {
                anim.enabled = true;
                anim.Stop();
                anim.Play();
            }
        }
        public void Reset()
        {
            if (anim != null)
                anim.enabled = false;
            filmingPlayer = false;

            StartCoroutine(Zoom(originalSize, 1.5f));
        }
        public float GetOriginalSize() { return originalSize; }
        public void SetTargetTo(Transform t)
        {
            target = t;
        }
        public void LookAtBall()
        {
            state = states.PLAYING;
            ball = Fulbo.Game.GameManager.Instance.ball;
            target = ball.transform;
        }
        void Update()
        {

            if (target == null)
                return;
            float _offsetX = 0;
            if (ball != null)
            {
                Character character = ball.character;
                if (character != null)
                {
                    if (character.transform.localScale.x > 0) _offsetX = offsetX; else _offsetX = -offsetX;
                }
            }
            Positionate(_offsetX);
        }
        public void Positionate(float _offsetX = 0)
        {
            Vector3 pos = transform.position;
            pos.x = target.position.x + _offsetX;
            pos.z = target.position.z - offsetZ;


            if (!filmingPlayer)
                pos.x *= 0.97f;
            else
            {
                pos.x *= 0.93f;
                pos.z -= 2;
            }

            if (state == states.PLAYING && !zoomIn)
            {
                if (pos.x < -max_pos_x) pos.x = -max_pos_x;
                else if (pos.x > max_pos_x) pos.x = max_pos_x;

                if (pos.z < -pos_z) pos.z = -pos_z;
                else if (pos.z > pos_z_max) pos.z = pos_z_max;
            }
            if (state == states.FREEZED)
                UpdateFreezed();
            else
                transform.position = Vector3.Lerp(transform.position, pos, speed * Time.deltaTime);
            transform.localEulerAngles = new Vector3(20, target.position.x * 15 / 20, target.position.x * 5 / 20);
        }
        public void OnSetTarget(Transform target)
        {
            this.target = target;
            // cam.orthographicSize -= 0.1f;
        }
        public void OnGoal(Character character)
        {
            StopAllCoroutines();
            state = states.GOAL;
            StartCoroutine(GoalCoroutine(character));
        }
        IEnumerator GoalCoroutine(Character character)
        {
            yield return new WaitForSeconds(1.2f);
            filmingPlayer = true;
            SetTargetTo(character.transform);
            StartCoroutine(Zoom(1.85f, 0.75f));
        }
        public void ForcePositionTo(Transform target)
        {
            GetComponent<Animation>().enabled = false;
            Vector3 pos = transform.position;
            pos.x = target.position.x;
            pos.z = target.position.z - offsetZ;
            transform.position = pos;
        }
        bool zoomIn = false;
        IEnumerator Zoom(float ToOrthographicSize, float speed, bool bakToOriginalSize = false, float delayToZoomOut = 0)
        {
            if (ToOrthographicSize < cam.orthographicSize) zoomIn = true; else zoomIn = false;
            if (zoomIn)
            {
                while (cam.orthographicSize > ToOrthographicSize)
                {
                    cam.orthographicSize -= Time.deltaTime * speed; yield return new WaitForEndOfFrame();
                }
            }
            else
            {
                while (cam.orthographicSize < ToOrthographicSize)
                {
                    cam.orthographicSize += Time.deltaTime * speed; yield return new WaitForEndOfFrame();
                }
            }
            cam.orthographicSize = ToOrthographicSize;
            if (bakToOriginalSize)
            {
                if (delayToZoomOut > 0)
                    yield return new WaitForSeconds(delayToZoomOut);
                StartCoroutine(Zoom(GetOriginalSize(), speed, false));
            }
        }
        void OnBallKicked(CharacterStates.kickTypes type, float speed, Character ch)
        {
            if (ch != null && ch.type == Character.types.GOALKEEPER)
            {
                GoalKeeperKick();
                return;
            }
            if (Fulbo.Game.GameManager.Instance.isTutorial) return;
            if (type == CharacterStates.kickTypes.CENTRO && !zoomIn)
                StartCoroutine(Zoom(GetOriginalSize() - 0.5f, 1.5f, true));
        }
        void GoalKeeperKick()
        {
            StopAllCoroutines();
            StartCoroutine(Zoom(GetOriginalSize(), 2.5f, false));
            goalkeeperHasBall = false;
        }
        bool goalkeeperHasBall;
        void CatchBall(Character character)
        {
            if (Fulbo.Game.GameManager.Instance.isTutorial) return;
            if (character.type == Character.types.GOALKEEPER)
            {
                goalkeeperHasBall = true;
                StopAllCoroutines();
                StartCoroutine(Zoom(GetOriginalSize() - 2, 2.5f, false));
            } else if(goalkeeperHasBall)
            {
                GoalKeeperKick();
            }
        }
        Vector3 freezedPosition;
        public void Freeze(Vector3 freezedPosition)
        {
            state = states.FREEZED;
            this.freezedPosition = freezedPosition;
        }
        void UpdateFreezed()
        {
            transform.localPosition = Vector3.Lerp(transform.localPosition, freezedPosition, 0.05f);
        }
        public void Unfreeze()
        {
            state = states.PLAYING;
        }
        void OnPowerupCharging(bool isOn, Character character)
        {
            if(isOn)
            {
                filmingPlayer = true;
                SetTargetTo(character.transform);
                StartCoroutine(Zoom(GetOriginalSize() - 2.5f, 12, false));
            }
            else
            {
                SetTargetTo(ball.transform);
                filmingPlayer = false;
                StartCoroutine(Zoom(GetOriginalSize(), 5f, false));
            }
        }
    }

}