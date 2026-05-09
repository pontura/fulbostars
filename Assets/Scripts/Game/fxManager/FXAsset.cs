using UnityEngine;
using System;

namespace Fulbo.FX
{
    public class FXAsset : MonoBehaviour
    {
        public float timer = 0.5f;

        public void Init(Vector3 pos)
        {
            this.gameObject.SetActive(true);
            transform.position = pos;
            Invoke("Reset", timer);
        }
        private void Reset()
        {
            this.gameObject.SetActive(false);
        }
    }
}