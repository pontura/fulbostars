using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Fulbo.Game.Xtras
{
    public class AnimmationControls : MonoBehaviour
    {
        public float randomDelay;
        Animation anim;

        void Start()
        {
            anim = GetComponent<Animation>();
            if (randomDelay != 0)
                InitDelayed();
        }
        void InitDelayed()
        {
            float desired_play_time = Random.Range(0, randomDelay);
            anim[anim.clip.name].time = desired_play_time;
        }
    }
}
