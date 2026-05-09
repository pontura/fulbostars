using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Fulbo.AssetsBundle
{
    public class RotationContinue : MonoBehaviour
    {
        [SerializeField] bool _x;
        [SerializeField] bool _y;
        [SerializeField] bool _z;
        [SerializeField] float _speed;

        private void Update()
        {
            Vector3 rot = new Vector3();
            if (_x) rot.x = 1;
            if (_y) rot.y = 1;
            if (_z) rot.z = 1;
            rot *= _speed * Time.deltaTime;
            transform.Rotate(rot, Space.Self);
        }
    }
}
