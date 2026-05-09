using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Fulbo.DB;

public class CharacterValue : MonoBehaviour
{
    [SerializeField] Text text;
    [SerializeField] Animator anim;
    // Start is called before the first frame update
    void Start()
    {
        
    }    
    
    public void SetPrice(DBUserData.DBCharacterData uData) {
        text.text = ""+uData.sell_price;
        anim.Play("ValueUp", -1, 0f); ;
    }
}
