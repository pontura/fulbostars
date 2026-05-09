using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace Fulbo.Game
{
    public class CharacterBillboard : MonoBehaviour
    {
        [SerializeField] CharacterForCamera characterForCamera;
        [SerializeField] MeshRenderer meshRenderer;
        [SerializeField] RenderTexture renderTexture;
        CharacterForCamera character;

        public void Init(CharactersData.CharacterData data, int bilboardID)
        {
            character = Instantiate(characterForCamera, transform);
            character.Init(data, "run");
            meshRenderer.material.mainTexture = character.SetCamera(true, renderTexture);
            character.transform.localPosition = new Vector3(1000 * bilboardID, 0, 0);

        }
        public void SetAnim(string animName)
        {
            character.SetAnim(animName);
        }
        public void SetScaleX(int scale_x = 1)
        {
            character.SetScaleX(scale_x);
        }
        public void Reset()
        {
            if (renderTexture != null)
            {
                meshRenderer = null;
                renderTexture.Release();
                renderTexture.DiscardContents();
                renderTexture = null;
            }
        }
        private void OnDisable()
        {
            if (renderTexture != null)
            {
                renderTexture.Release();
                renderTexture.DiscardContents();
            }
        }
        public void LookTo(bool left)
        {
            Vector3 sc = character.transform.localScale;
            if (!left && sc.x > 0)
                sc.x = Mathf.Abs(sc.x) * -1;
            else if (left && sc.x < 0)
                sc.x = Mathf.Abs(sc.x);
            character.transform.localScale = sc;
        }
    }
}
