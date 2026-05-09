using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Fulbo.Game
{
    public class BallRaycast : MonoBehaviour
    {
        enum states
        {
            ON,
            OFF
        }
        states state;
        Vector3 pos = new Vector3();
        bool raycastActive;
       // [SerializeField] GameObject go;

        private void Start()
        {
            SetOff();
          //  go.transform.SetParent(GameManager.Instance.transform);
        }
        public void SetOff()
        {
            state = states.OFF;
        }
        public void SetOn()
        {
            state = states.ON;
        }
        void FixedUpdate()
        {
            if (state == states.OFF) return;
            GetRayCast();
        }
        public Vector3 GetPosition()
        {
            if (state == states.OFF || !raycastActive)
                return transform.position;
            else
                return pos;
        }
        public Vector3 GetRayCast()
        {
            int layerMask = 1 << 14; // the layer Raycast:
            layerMask = ~layerMask;
            RaycastHit hit;
            if (Physics.Raycast(transform.position, transform.TransformDirection(Vector3.forward), out hit, Mathf.Infinity, layerMask))
            {
                Debug.DrawRay(transform.position, transform.TransformDirection(Vector3.forward) * hit.distance, Color.yellow);
                raycastActive = true;
                pos = hit.point;
                pos.y = 1;
            }
            else
            {
                pos = transform.position;
                raycastActive = false;
            }
            return pos;
        }
    }
}