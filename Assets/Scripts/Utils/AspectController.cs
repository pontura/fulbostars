using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AspectController : MonoBehaviour
{
    public float new_scale = 1;
    public Vector3 add_position;

    public enum aspects
    {
        IPAD,
        OTHER
    }
    public aspects aspect;

    private void Start()
    {
        float aspectValue = (float)Screen.width / (float)Screen.height;
        if (aspectValue <= 1.6f && aspect == aspects.IPAD)
            DoIt();
    }
    void DoIt()
    {
        if(new_scale != 1)
            transform.localScale = new Vector2(new_scale, new_scale);
        if (add_position != Vector3.zero)
            transform.localPosition += add_position;
    }
}
