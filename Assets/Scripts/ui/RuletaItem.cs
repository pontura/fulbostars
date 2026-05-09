using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using static Fulbo.CharactersData;

public class RuletaItem : MonoBehaviour {

    public Image image;
    public Color color;
    public int id;
    public CharacterData characterData;

    public void Init(Ruleta.ItemData item,  int height)
    {
        this.id = item.id;
        image.sprite = item.sprite;
        image.transform.localScale = new Vector2(0.8f, 1);
      //  GetComponent<Image>().color = Color.white;  //item.color;
        GetComponent<LayoutElement>().minHeight = height;
    }
}
