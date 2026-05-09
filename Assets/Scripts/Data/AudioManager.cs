using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using Fulbo.AssetsBundle;

public class AudioManager : MonoBehaviour
{
    static AudioManager mInstance = null;

    public AudioSourceManager[] all;
    [Serializable]
    public class AudioSourceManager
    {
        public string sourceName;
        public AudioSource audioSource;
        public float volume = 1;
    }


    public static AudioManager Instance {
        get {
            return mInstance;
        }
    }

    void Awake() {
        if (!mInstance)
            mInstance = this;
        else
        {
            Destroy(this.gameObject);
            return;
        }

        DontDestroyOnLoad(this);
    }

    void Start(){
        foreach (AudioSourceManager m in all)
        {
            if(m.audioSource==null)
                m.audioSource = gameObject.AddComponent<AudioSource>();
            m.audioSource.volume = m.volume;
        }
        Events.PlayCharacterSFX += PlayCharacterSFX;
        Events.OnButtonPressed += OnButtonPressed;
        Events.Back += Back;

        ChangeVolume("music", 0.5f);
    }
    void OnDestroy()
    {
        Events.PlayCharacterSFX -= PlayCharacterSFX;
        Events.OnButtonPressed -= OnButtonPressed;
        Events.Back -= Back;
    }
    void Back()
    {
        OnButtonPressed(Fulbo.UI.ButtonCustom.types.UI_GENERIC);
    }
    void OnButtonPressed(Fulbo.UI.ButtonCustom.types type)
    {
        switch(type)
        {
            case Fulbo.UI.ButtonCustom.types.UI_GENERIC:
                PlaySoundOneShot("ui", "ui/ui_click"); break;
            case Fulbo.UI.ButtonCustom.types.TAB:
                PlaySoundOneShot("ui", "ui/ui_tabs"); break;
            case Fulbo.UI.ButtonCustom.types.LIST_ITEM:
                PlaySoundOneShot("ui", "ui/ui_click2"); break;
            case Fulbo.UI.ButtonCustom.types.KEY_PRESSED:
                PlaySoundOneShot("ui", "ui/ui_click3"); break;
            case Fulbo.UI.ButtonCustom.types.CLOSE:
                PlaySoundOneShot("ui", "ui/ui_close"); break;
        }
    }
    public void ChangePitch(string sourceName, float pitch) {
        foreach (AudioSourceManager m in all) {
            if (m.sourceName == sourceName)
                m.audioSource.pitch = pitch;
        }
    }

    public void ChangeVolume(string sourceName, float volume)
    {
        foreach (AudioSourceManager m in all)
        {
            if (m.sourceName == sourceName)
                m.audioSource.volume = volume;
        }
    }
    public void PlaySpecificSoundInArray(AudioClip[] allClips)
    {
        PlaySpecificSound(allClips[UnityEngine.Random.Range(0, allClips.Length)]);
    }
    public void PlaySpecificSound(AudioClip audioClip, string sourceName = "common", bool loop = false, bool noRepeat = false)
    {
        AudioSource audioSource = GetAudioSource(sourceName); if (audioSource == null) return;
        if (noRepeat) {
            if (audioSource.clip == audioClip && audioSource.isPlaying)
                return;
        }
        audioSource.clip = audioClip;
        audioSource.loop = loop;
        audioSource.Play();
    }
    public void PlayBallSound(AudioClip[] audioClips, float volume = 1, float limit_min_max = 0)
    {
        if (audioClips == null || audioClips.Length == 0) return;
        if(audioClips.Length>1 && limit_min_max > 0 && volume>limit_min_max)
            PlayBallSound(audioClips[1], volume);
        else
            PlayBallSound(audioClips[0], volume);
    }
    public void PlayCrowd(AudioClip[] audioClips)
    {
        AudioSource audioSource = GetAudioSource("crowd");
        if (audioClips.Length > 0)
        {
            audioSource.clip = audioClips[UnityEngine.Random.Range(0, audioClips.Length)];
            audioSource.loop = false;
            audioSource.Play();
        }
    }
    public void PlayBallSound(AudioClip audioClip, float volume = 1)
    {
        if (audioClip == null) return;
        AudioSource audioSource = GetAudioSource("ball");
        audioSource.clip = audioClip;
        audioSource.loop = false;
        audioSource.volume = volume;
        audioSource.Play();
    }
    public void PlaySound(string sourceName, string audioName, bool loop, bool noRepeat = false)
    {
        AudioSource audioSource = GetAudioSource(sourceName);
        if (audioSource == null) return;


        AudioClip clip = Resources.Load<AudioClip>("Audio/" + audioName) as AudioClip;
        //print("soiurce: " + sourceName + " clip: " + clip + "     Play Audio/" + audioName);
        //AudioClip clip = AssetsBundleManager.Instance.assetsBundleLoader.GetAssetAsAudioClip("audio", audioName);
        if (noRepeat) {
            if (audioSource.clip == clip && audioSource.isPlaying)
                return;
        }
        audioSource.clip = clip;
        audioSource.loop = loop;
        audioSource.Play();        
    }
    public void PlaySoundOneShot(string sourceName, string audioName, bool noRepeat = false) {
       
        AudioSource audioSource = GetAudioSource(sourceName);
        if (audioSource == null) return;

        if (audioName == "")  {
            audioSource.Stop(); return;
        }

        AudioClip clip = Resources.Load<AudioClip>("Audio/" + audioName) as AudioClip;
       
        //AudioClip clip = AssetsBundleManager.Instance.assetsBundleLoader.GetAssetAsAudioClip("audio", audioName);
        if (noRepeat) {
            if (audioSource.clip == clip && audioSource.isPlaying)
                return;
        }
        audioSource.PlayOneShot(clip);
    }
    AudioSource GetAudioSource(string sourceName)
    {
        foreach (AudioSourceManager m in all)
        {
            if (m.sourceName == sourceName)
                return m.audioSource;
        }
        return null;
    }

    public void Play2Musics(string audioName1, string audioName2)
    {
        AudioSource audioSourceToListen = GetAudioSource("music2");
        AudioSource audioSource2 = GetAudioSource("music");

        AudioClip clip = Resources.Load<AudioClip>("Audio/" + audioName1) as AudioClip;
        AudioClip clip2 = Resources.Load<AudioClip>("Audio/" + audioName2) as AudioClip;

        audioSourceToListen.clip = clip;
        audioSource2.clip = clip2;

        audioSourceToListen.Play();
        audioSource2.Play();

        audioSourceToListen.volume = 1;
        audioSource2.volume = 0;
        audioSourceToListen.loop = false;
        StartCoroutine(WaitToInitMusic2(audioSourceToListen) );
    }
    IEnumerator WaitToInitMusic2(AudioSource audioSourceToListen)
    {
        while (audioSourceToListen.isPlaying)
            yield return null;
        AudioSource audioSource2 = GetAudioSource("music");
        audioSource2.volume = 1;
        audioSource2.time = 0;
        audioSource2.loop = true;
        audioSourceToListen.Stop();
        audioSourceToListen = null;
    }
    public void FadeVolume(string audioSourceName, float to, float speed = 0.25f)
    {
        StartCoroutine(FadeVolumeC(audioSourceName, to, speed));
    }
    IEnumerator FadeVolumeC(string audioSourceName, float to, float speed)
    {
        AudioSource audioSource = GetAudioSource(audioSourceName);
        if (audioSource != null)
        {
            float vol = audioSource.volume;
            if (vol < to)
                while (vol < to)
                {
                    vol += speed * Time.deltaTime;
                    audioSource.volume = vol;
                    yield return null;
                }
            else if (vol > to)
                while (vol > to)
                {
                    vol -= speed * Time.deltaTime;
                    audioSource.volume = vol;
                    yield return null;
                }
        }
    }
    public void SetActive(string audioSourceName, bool state)
    {
        AudioSource audioSource = GetAudioSource(audioSourceName);
        audioSource.enabled = state;
        if (state)
            audioSource.Play();
        print("audioSourceName" + state);
    }
    public void PlayCharacterSFX(AudioClip audioClip, Vector3 pos)
    {
       // print("_____________ " + audioClip.name);
        AudioSource audioSource = GetFreeCharacterSFXAudioSource();
        if (audioSource == null) return;        

        if (Fulbo.Game.GameManager.Instance != null && Fulbo.Game.GameManager.Instance.cameraInGame != null)
        {
            Fulbo.Game.CameraInGame camera = Fulbo.Game.GameManager.Instance.cameraInGame;
            //print("cam x: " + camera.transform.position.x + "   character in x : " + pos.x);
            float distToCam = Mathf.Abs( camera.transform.position.x - pos.x);
            if (distToCam > 10)
                return;
            audioSource.volume = 0.7f - (distToCam/14);
        }
        audioSource.clip = audioClip;
        audioSource.loop = false;
        audioSource.Play();
    }
    AudioSource GetFreeCharacterSFXAudioSource()
    {
        AudioSource audioSource;
        for (int a = 1; a < 4; a++)
        {
            audioSource = GetAudioSource("characterSFX_" + a);
            if (!audioSource.isPlaying)
                return audioSource;
        }
        return null;
    }
    public float GetVolumeFor(float min, float max, float value)
    {
        if (value > max) value = max;
        else
        if (value < min) value = 0;
        if (value > 0)
            return value / max;
        return 0;
    }
}
