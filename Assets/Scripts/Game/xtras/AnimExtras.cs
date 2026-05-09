using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimExtras : MonoBehaviour
{
    public AudioClip[] clipsID;

    public void PlayAudioClip(int id)
    {
          if(clipsID.Length>=id)
            Events.PlayCharacterSFX(clipsID[id], transform.position);
    }
}