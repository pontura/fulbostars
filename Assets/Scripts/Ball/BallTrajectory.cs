using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Fulbo.Game
{
    public class BallTrajectory : MonoBehaviour
    {
        [SerializeField] GameObject asset;
        Ball ball;
        Rigidbody ballRigidbody;
        public List<Vector3> positionList;
        float ballRadius = 2;
        Vector3 destination;
        public bool DEBUG;

        public void Init()
        {
            ball = GameManager.Instance.ball;
            ballRigidbody = GameManager.Instance.ball.rb;
            Loop();
            if (!DEBUG && asset != null) Destroy(asset.gameObject);
            else if(asset != null)
                asset.SetActive(true);
        }
        void Loop()
        {
            positionList.Clear();
            Invoke("Loop", 0.15f);
            if (ball.character != null) return;
            destination = ball.transform.position;
            int maxIterations = Mathf.RoundToInt(8f / Time.fixedDeltaTime);
            Vector3 pos = ball.transform.position;
            Vector3 vel = ballRigidbody.velocity;
            float drag = Mathf.Clamp01(1.0f - (ballRigidbody.drag * Time.fixedDeltaTime));
            positionList.Add(pos);
            float elapsedTime = 0.0f;
            for (int i = 0; i < maxIterations; i++)
            {
                vel = vel + (Physics.gravity * Time.fixedDeltaTime);
                vel *= drag;
                pos += vel * Time.fixedDeltaTime;
                elapsedTime += Time.fixedDeltaTime;
                positionList.Add(pos);
                if (ballRigidbody.velocity.y < 0 && pos.y <= 2.5f)
                {
                    destination = pos;
                    destination.y = 0;
                    if (DEBUG && asset) asset.transform.position = destination;
                    break;
                }
                // if (pos.y - ballRadius < targetHeight && Vector3.Dot(vel, Vector3.down) > 0) break;
            }
           
        }
        public Vector3 GetPosition()
        {
            if (ball.character != null) return ball.transform.position;
            if(positionList.Count < 1)
            {
               // Debug.Log("___PROJECTION NOT CALCULATED YET: ");
                return ball.transform.position;
            }
           // Debug.Log("PROJECTION:" + destination);
            return destination;
        }
    }
}