using UnityEngine;

namespace Fulbo.Game.Xtras
{
    public class BallAutomatic : MonoBehaviour
    {
        public float force = 1000;
        Vector3 originalPos;
        Rigidbody rb;

        void Start()
        {
            rb = GetComponent<Rigidbody>();
            originalPos = transform.position;
            Loop();
        }
        void Loop()
        {
            rb.velocity = Vector3.zero;
            transform.position = originalPos;
            transform.localEulerAngles = new Vector3(Random.Range(-5, -12), Random.Range(-100, -80), 0);
            rb.AddForce(transform.forward * force);
            Invoke("Loop", 2);
        }
    }
}
