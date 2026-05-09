using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class PlayerTeamSignalUI : MonoBehaviour
{
    public Image bg;
    public Text field;
    public int teamID;
    public int moveX;

    public void Init(int playerID)
    {
        field.text = "P" + playerID;
        SetPos();
    }
    public void MoveRight(bool isRight)
    {
        if (isRight)
        {
            if (teamID == 0)
                teamID = 2;
            else if (teamID == 1)
                teamID = 0;
        }
        else
        {
            if (teamID == 2)
                teamID = 0;
            else if (teamID == 0)
                teamID = 1;
        }
        SetPos();
    }
    void SetPos()
    {
        Vector2 pos = transform.localPosition;
        if (teamID == 1)
        {
            bg.color = Color.green;
            pos.x = -moveX;
        }           
        else if (teamID == 2)
        {
            bg.color = Color.green;
            pos.x = moveX;
        }           
        else
        {
            pos.x = 0;
            bg.color = Color.red;
        }       
        transform.localPosition = pos;
    }
}
