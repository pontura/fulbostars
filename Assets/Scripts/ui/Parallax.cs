using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Parallax : MonoBehaviour
{
    public float speed = 10;
    public float _width = 6.2f;

    void Update()
    {
        Vector3 pos = transform.localPosition;
        pos.x -= speed * Time.deltaTime;
        if (pos.x < -_width)
            pos.x = 0;
        transform.localPosition = pos;
    }
}
