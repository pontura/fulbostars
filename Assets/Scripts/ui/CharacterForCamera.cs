using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace Fulbo.Game
{
    public class CharacterForCamera : MonoBehaviour
    {
        public Transform characterContainer;
        public CharacterStates states;
        int lastActionID = 0;
        [SerializeField] Camera cam;

        public void Init(CharactersData.CharacterData data, string anim = "run")
        {
            Init(data.asset, anim);
        }
        GameObject asset;
        string animName;
        Animator anim;
        public void Init(GameObject asset_to_instantiate, string animName)
        {
            gameObject.SetActive(true);
            Utils.RemoveAllChildsIn(characterContainer);

            int characterID = int.Parse(asset_to_instantiate.name); //con el nombre sacamos el id:

            asset = Instantiate(asset_to_instantiate, characterContainer);
            asset.transform.localEulerAngles = asset.transform.localPosition = Vector3.zero;
            asset.transform.localScale = Vector3.one;
            anim = GetComponent<Animator>();
            SetAnim(animName);
        }        

        public void DestroyCharacter() {
            Destroy(asset);
        }

        public void SetAnim(string animName)
        {
            this.animName = animName;
            if (anim == null && asset != null)
            {
                anim = asset.GetComponent<Animator>();
            }
            if(anim != null)
            {
                anim.StopPlayback();
                Invoke("AnimDelayed", 0.1f);
            }
        }
        public void AnimDelayed()
        {
            if (asset == null) return;
            if (anim == null)
                anim = asset.GetComponent<Animator>();
            anim.Play(animName);
        }
        public void SetScaleX(int scale_x = 1)
        {
            Vector3 sc = asset.transform.localScale;
            sc.x *= scale_x;
            asset.transform.localScale = sc;
        }
        public RenderTexture SetCamera(bool isOn, RenderTexture _rt = null)
        {
            cam.gameObject.SetActive(isOn);
            if (_rt != null)
            {
                RenderTexture rt = new RenderTexture(_rt);
                cam.targetTexture = rt;
            }
            return cam.targetTexture;
        }
    }
}