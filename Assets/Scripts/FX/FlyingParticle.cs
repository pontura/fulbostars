using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Fulbo.UI
{
    public class FlyingParticle : MonoBehaviour
    {
        public FlyingParticlesUI.types type;
        public int totalParticles;
        public float from;
        public float to;
        public int id;

        public IEnumerator Fly(float duration1, float duration2, float smoothIn, float smoothOut, Vector2 dest1, Vector2 destTotal, Transform container,  System.Action<Transform, FlyingParticle> OnReady)
        {
            GameObject asset = this.gameObject;
            float timer;
            timer = 0;
            while (timer < duration1 && asset!=null)
            {
                timer += Time.deltaTime;
                asset.transform.position = Vector2.Lerp(asset.transform.position, dest1, (timer / duration1) / smoothIn);
                yield return new WaitForEndOfFrame();
            }
            timer = 0;
            while (timer < duration2/1.2f && asset != null)
            {
                timer += Time.deltaTime;
                asset.transform.position = Vector2.Lerp(asset.transform.position, destTotal, Mathf.SmoothStep(0f, 1f, (timer / duration2) / smoothOut));
                yield return new WaitForEndOfFrame();
            }
            OnReady(container, this);
            yield return null;
        }
    }
}
