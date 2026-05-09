using System.Collections;
using UnityEngine;
using UnityEngine.UI;

namespace Fulbo.UI
{
    public class ProgressBarCustom : MonoBehaviour
    {
        [SerializeField] Image xpProgressBar;      // the bar
        [SerializeField] Image xpProgressBar_new;  // the new progressBar
        [SerializeField] GameObject uploadingFXAsset;   // asset turned on while uploading

        [SerializeField] string animFillName ="fill";// name for fill anim
        [SerializeField] string animEndName = "end";// name for end anim
        [SerializeField] float offsetScaleDisplacement = 0.7f;// size of bar
        [SerializeField] float offsetDisplacement = -121.66f;// offset for bar
        [SerializeField] float totalWidth = 346;        // pos.x of the filled bar
        Animator uploadingAnim;                         // anim while uploading anim
        [SerializeField] GameObject onReadyAsset;       // asset for completedBar

        System.Action OnReady;

        private void Start()
        {            
            //Init(0.1f);
            //Animate(0.7f, 2, null);
        }
        public void Init(float value) // force initial value
        {
            Debug.Log("PPPPPPPPPROGRESS Init " + value);
            if (uploadingFXAsset != null)
            {
                uploadingFXAsset.GetComponent<Animator>();
                uploadingFXAsset.SetActive(false);
            }
            if (onReadyAsset != null)
                onReadyAsset.SetActive(false);

            xpProgressBar.fillAmount = value;
            xpProgressBar_new.fillAmount = value;
        }
        public void Animate(float value, float duration, System.Action OnReady)
        {
            this.OnReady = OnReady;

            if (uploadingFXAsset != null)
            {
                uploadingFXAsset.SetActive(true);
                AnimFX(animFillName);
            }
            StartCoroutine( AnimateTo(value, value * totalWidth,  duration) );
        }
        void OnAnimDone()  {
            AnimFX(animEndName);
            if (OnReady != null) OnReady();
        }
        IEnumerator AnimateTo(float value, float valuePos, float duration)
        {
            Debug.Log("PPPPPPPPPROGRESS AnimateTo " + value);
            AnimFX("fill");
            float initialPos = xpProgressBar_new.fillAmount * totalWidth;
            float timer = 0;
            float dest = valuePos;
            while (timer < duration)
            {
                timer += Time.deltaTime;
                float new_pos1 = Mathf.SmoothStep(xpProgressBar_new.fillAmount, value, timer / duration);
                float new_pos = Mathf.SmoothStep(initialPos, dest, timer/ duration);
                xpProgressBar_new.fillAmount = new_pos1;
                if (uploadingFXAsset != null)
                    uploadingFXAsset.transform.localPosition = new Vector2((new_pos* offsetScaleDisplacement) + offsetDisplacement, 0);
                if (new_pos >= totalWidth-1)
                    BarCompleted();
                yield return new WaitForEndOfFrame();
            }
            OnAnimDone();
        }
        void BarCompleted()
        {
            if (onReadyAsset != null)
                onReadyAsset.SetActive(true);
        }
        void AnimFX(string name)
        {
            if (uploadingAnim != null)
                uploadingAnim.Play(name);
        }


    }
}
