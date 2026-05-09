using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Fulbo.Game
{
    public class BallAI : MonoBehaviour
    {
        states state;
        enum states
        {
            IDLE,
            PASE
        }
        Ball ball;
        Character target;
        float _y = 0;
        float speed;
        float mid_distance;
        bool isGoingUp;

        [SerializeField] float _speed_y = 60;
        [SerializeField] float _baloon_speed = 70;
        [SerializeField] float _common_speed = 1500;
        [SerializeField] float snap_duration_baloon = 0.9f;
        [SerializeField] float snap_duration_common = 0.6f;
        [SerializeField] float max_height_baloon = 11;

        private void Start()
        {
            ball = GetComponent<Ball>();
        }
        public void Pase(Character characterReceiving)
        {
            CancelInvoke();
            GameManager.Instance.charactersManager.charactersStrategy.SetCharacterReceiver(characterReceiving);
            state = states.PASE;
            target = characterReceiving;
            _y = 0;
            float delayToReset;
            float distance = Vector3.Distance(ball.transform.position, target.transform.position);
            mid_distance = distance / 2;

            if (ball.kickType == CharacterStates.kickTypes.BALOON || ball.kickType == CharacterStates.kickTypes.CENTRO)
            {
                isGoingUp = true;
                if (distance < 6) distance = 6;
                if (distance > 11) distance = 11;
                _y = distance * 1.9f;
                if (_y > max_height_baloon) _y = max_height_baloon;
                delayToReset = snap_duration_baloon;
                speed = _baloon_speed * distance;
            }
            else
            {
                speed = _common_speed;
                delayToReset = snap_duration_common;
            }
            if (target.type == Character.types.GOALKEEPER) delayToReset /= 2f;
            Invoke("Reset", delayToReset);
            Loop();
        }
        public void Reset()
        {
            CancelInvoke();
            state = states.IDLE;
            target = null;
        }
        void Loop()
        {
            if (target == null) return;
            float speed_y = _speed_y;

            if (ball.kickType == CharacterStates.kickTypes.CENTRO) speed_y /= 2;

            Vector3 targetPos = target.transform.position;
            float target_y = target.transform.position.y;
            if (ball.kickType == CharacterStates.kickTypes.CENTRO)
            {
                target_y = 2f;
            }
            if (target != null && state == states.PASE)
            {
                if (ball.kickType == CharacterStates.kickTypes.BALOON || 
                    ball.kickType == CharacterStates.kickTypes.CENTRO) // Curva 
                {
                    Vector3 d1 = targetPos;
                    Vector3 d2 = ball.transform.position;
                    d1.y = target_y; d2.y = target_y;

                    if (Vector3.Distance(d1, d2) < mid_distance && isGoingUp)
                    {
                        isGoingUp = false;
                        _y = ball.transform.position.y;
                    }
                    if (Vector3.Distance(d1, d2) < 0.5f)
                    {
                        if (ball.transform.position.y < target_y)
                        {
                            targetPos.y = target_y;
                            ball.transform.position = targetPos;
                            ball.rb.velocity = Vector3.zero;
                            return;
                        }
                        _y -= Time.deltaTime * speed_y;
                    }
                    else if (_y <= target_y)
                        _y = target_y;
                    else
                        _y -= Time.deltaTime * speed_y;

                    targetPos.y = _y;
                }
                transform.LookAt(targetPos);
                ball.rb.velocity = Vector3.zero;
                ball.rb.AddForce(transform.forward * speed);
            }
            Invoke("Loop", 0.05f);
        }
    }
}








//using System.Collections;
//using System.Collections.Generic;
//using UnityEngine;

//namespace Fulbo.Game
//{
//    public class BallAI : MonoBehaviour
//    {
//        [SerializeField] AnimationCurve curve;
//        [SerializeField] AnimationCurve curve2;

//        states state;
//        enum states
//        {
//            IDLE,
//            PASE
//        }
//        Ball ball;
//        Character target;
//        float max_y;
//        float _y;

//        [SerializeField] float speed = 18;
//        [SerializeField] float speed_globo = 12;
//        [SerializeField] float max_y_centro = 6;
//        [SerializeField] float max_y_common = 2;
//        [SerializeField] float head_position_center = 3;

//        bool isCentro;
//        Vector3 pos_init;
//        Vector3 pos_dest;
//        float distance;
//        float distanceDone;
//        bool isGoingUp;

//        private void Start()
//        {
//            ball = GetComponent<Ball>();
//        }
//        public void Pase(Character characterReceiving)
//        {
//            GameManager.Instance.charactersManager.charactersStrategy.SetCharacterReceiver(characterReceiving);
//            state = states.PASE;
//            target = GameManager.Instance.charactersManager.charactersStrategy.GetReceiver();

//            if ((ball.characterThatKicked != null && ball.characterThatKicked.GetPosition() == Character.PositionsInGame.CENTRO) ||
//                   characterReceiving.GetPosition() == Character.PositionsInGame.IN_AREA_ATTACKING)
//                isCentro = true;
//            else
//                isCentro = false;

//            pos_init = ball.transform.position;
//            pos_dest = characterReceiving.transform.position;
//            distanceDone = 0;
//            isGoingUp = true;
//            _y = pos_init.y;
//            distance = Vector3.Distance(pos_init, pos_dest);

//            if (ball.kickType == CharacterStates.kickTypes.BALOON || isCentro)
//                max_y = max_y_centro;
//            else
//                max_y = max_y_common;

//            ball.rb.velocity = Vector3.zero;
//            ball.rb.isKinematic = true;
//        }
//        public void Reset()
//        {
//            state = states.IDLE;
//            target = null;
//            ball.rb.isKinematic = false;
//            ball.rb.velocity = Vector3.zero;
//        }
//        void Update()
//        {
//            if (target == null || state != states.PASE)
//                return;
//            if (ball.kickType == CharacterStates.kickTypes.BALOON || isCentro)
//                distanceDone += Time.deltaTime * speed_globo; 
//            else
//                distanceDone += Time.deltaTime * speed;

//            float d = distanceDone / distance;

//            if (isCentro)
//                pos_dest.y = head_position_center;

//            Vector3 newPos = Vector3.Lerp(pos_init, pos_dest, d);


//            if (distanceDone < distance / 2)
//            {
//                d = (distanceDone) / (distance / 2);
//               // newPos.y = Mathf.Lerp(pos_init.y, max_y, Mathf.SmoothStep(0.0f, 1.0f, d));
//                var easing = curve.Evaluate(d);
//                newPos.y = Mathf.Lerp(newPos.y, max_y, easing);
//            }
//            else
//            {
//                d = (distanceDone) / (distance / 2)-1;
//               // newPos.y = Mathf.Lerp(max_y, pos_init.y, Mathf.SmoothStep(0.0f, 1.0f, d));
//                var easing = curve2.Evaluate(d);
//                newPos.y = Mathf.Lerp(max_y, newPos.y, easing);
//            }


//            //print((distanceDone < distance / 2) + " " +  pos_y +  "   d: " + d);

//            //newPos.y = pos_y;

//            //


//            ball.transform.position = newPos;

//            if (distanceDone >= distance)
//                Reset();
//        }
//    }
//}