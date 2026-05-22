using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Fulbo.Game
{

    public class Intro : MonoBehaviour
    {
        [SerializeField] Image progress;
        [SerializeField] Text field;
        float progressValue = 0;
        float totalFilesToLoad = 12; // Hardcoded oh yeah! (no stadiums ni pinballs)
        float fileID;
        float fakeAdvance;
      //  string[] texts;

        void Awake()
        {
            Events.OnLoading += OnLoading;
            Events.OnLoadingProgress += OnLoadingProgress;
            progress.fillAmount = 0;
        }
        void Start()
        {
            OnLoading("");
            FakeLoading();
          //  texts = DB.DBManager.Instance.dBServerConfig.configData.loading;
            //print("texts " + texts.Length);
            //Utils.Shuffle(texts);
          //  LoopTexts();
        }
        void OnDestroy()
        {
            Events.OnLoading -= OnLoading;
            Events.OnLoadingProgress -= OnLoadingProgress;
        }
        int id;
        void LoopTexts()
        {
            //if (id > texts.Length - 1)
            //    id = 0;
            //field.text = texts[id];
            //id++;
            Invoke("LoopTexts", Random.Range(2, 4));
        }
        void OnLoading(string text)
        {
            //if (text == "Bundles")
            //    field.text = "Upgrading";

            //else if (text == "Positions") // assets bundle done!
            //    field.text = "Entering";

            //else if (text == "LevelBonus") // assets bundle done!
            //    field.text = "";

            fileID++;
            print("_______" + fileID + " text: " + text);
            OnLoadingProgress(0);
            if (text == "AllDone")
            {
                //Data.Instance.LoadLevel("Splash");
                Data.Instance.LoadLevel("TournamentSelector");
            }
        }
        void OnLoadingProgress(float value)
        {
          //  print("fileID: " + fileID + " value: " + value + " _______totalFilesToLoad: " + totalFilesToLoad);
            if (fileID == 0) return;
            float init;
            if (fileID == 1) init = 0;
            else init = ((fileID-1) / totalFilesToLoad) + fakeAdvance;
            float end = fileID / totalFilesToLoad + fakeAdvance;
            if (end > 1) end = 1;
            progressValue = Mathf.Lerp(init, end, value);
        }
        void FakeLoading()
        {
            Invoke("FakeLoading", Random.Range(0.5f, 1f));
            if (Random.Range(0, 10) < 6)
            {
                float value = 0.0015f;
                fakeAdvance += value;
                progressValue += value;
            }
        }
        void LateUpdate()
        {
            progress.fillAmount = Mathf.Lerp(progress.fillAmount, progressValue, 30 * Time.deltaTime);
        }
    }

}