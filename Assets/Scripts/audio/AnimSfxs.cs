using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimSfxs : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    public void PlayUiSfx(string filename) {
        AudioManager.Instance.PlaySoundOneShot("ui", filename);
    }

    public void PlaySfx(string filename) {
        AudioManager.Instance.PlaySoundOneShot("fx", filename);
    }

    public void PlayMusic(string filename) {
        AudioManager.Instance.PlaySound("music", filename, false);
    }
   
    public void PlayMusicLoop(string filename) {
        AudioManager.Instance.PlaySound("music", filename, true);
    }

}
