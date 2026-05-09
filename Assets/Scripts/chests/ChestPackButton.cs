using UnityEngine;
using System.Collections;
using UnityEngine.UI;
namespace Fulbo.UI.Shop
{
    public class ChestPackButton : ButtonCustom
    {
        [SerializeField] Transform container;

        public void InitPack(GameObject pack)
        {
            Utils.RemoveAllChildsIn(container);
            GameObject go = Instantiate(pack, container);
            go.transform.localScale = Vector2.one;
        }
    }
}
