using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Fulbo.UI
{
    public class TeamPoster : MonoBehaviour
    {
        [SerializeField] float scale = 0.4f;
        [SerializeField] Image characterImageThumb;
        [SerializeField] Transform container;
        [SerializeField] int _width = 440;
        [SerializeField] int offset = -30;

        public void AddData(LevelData levelData)
        {
            Utils.RemoveAllChildsIn(container);
            int i = 0;
            float separation = 0;
            foreach (int a in levelData.oponents)
            {
                Sprite sprite;
                if (i == 0)
                    sprite = CharactersData.Instance.GetCharacterData(a, true).thumb;
                else
                    sprite = CharactersData.Instance.GetCharacterData(a, false).thumb;
                i++;
                Image imageNew = Instantiate(characterImageThumb, container);
                imageNew.sprite = sprite;
                imageNew.SetNativeSize();
                imageNew.transform.localScale = new Vector2(scale, scale);
                separation = i * (_width / levelData.oponents.Count);
                imageNew.transform.localPosition = new Vector2(separation, 0);
            }
            container.transform.localPosition = new Vector2(offset - (separation / 2), 0);

            //  Para algo que quiero implementar (Luka) en el futuro que me pidió Nacho
            //  Debug.Log("Nuevo equipo tiene " + i + " jugadores", context:this);
        }
    }
}
