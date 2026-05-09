using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ScoreIngameUI : MonoBehaviour
{
    public Text field;
    public void Init(int qty)
    {
        field.text = "" + qty;
        Invoke("Reset", 2);
    }
    void Reset()
    {
        Destroy(gameObject);
    }
}
