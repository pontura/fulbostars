using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Fulbo.UI.Progress
{
    public class ProgressBarPiece : MonoBehaviour
    {
        public enum types
        {
            SELECTED,
            DONE,
            INACTIVE
        }
        Animation anim;

        [SerializeField] AnimationClip selectedClip;
        [SerializeField] AnimationClip doneClip;
        [SerializeField] AnimationClip inactiveClip;

        types TYPE;
        bool initialized;
        public void Init(types TYPE)
        {
            initialized = true;
            this.TYPE = TYPE;
            anim = GetComponent<Animation>();
            switch(TYPE)
            {
                case types.SELECTED: anim.Play(selectedClip.name); break;
                case types.DONE: anim.Play(doneClip.name); break;
                case types.INACTIVE: anim.Play(inactiveClip.name); break;
            }
        }
        private void OnEnable()
        {
            if(initialized)
                Init(TYPE);
        }
    }
}
