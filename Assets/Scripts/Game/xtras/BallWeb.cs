using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;

namespace Fulbo.Game.Xtras
{
    public class BallWeb : MonoBehaviour
    {
        Cloth cloth;
        string sceneName;
        bool isOn;

        void Awake()
        {
            sceneName = UnityEngine.SceneManagement.SceneManager.GetActiveScene().name;

            switch (sceneName)
            {
                case "Game":
                case "Tutorial":
                case "Arcos":
                case "Penalty":
                    isOn = true;
                    break;
            }
        }

        void Start()
        {
            if (isOn)
            {
                cloth = GetComponent<Cloth>();
                Invoke("Delayed", 0.5f);
            }
        }
        void Delayed()
        {
            GameObject ball;
            if (sceneName == "Arcos" || sceneName == "Penalty")
                ball = GameObject.Find("Ball");
            else if(isOn)
                ball = GameManager.Instance.ball.gameObject;             
            else
                return;

            var colliders = new ClothSphereColliderPair[1];
            colliders[0] = new ClothSphereColliderPair(ball.GetComponent<SphereCollider>());
            cloth.sphereColliders = colliders;
        }
        private void OnTriggerEnter(Collider other)
        {
            if (other.tag == "Ball")
            {
                other.gameObject.GetComponent<Rigidbody>().velocity /= 1.25f;
                AudioManager.Instance.PlayBallSound(Stadiums.StadiumsData.Instance.active.net, 1);
            }
        }
    }
}
