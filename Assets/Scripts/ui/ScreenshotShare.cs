using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using System.IO;

namespace Fulbo.UI
{
    public class ScreenshotShare : MonoBehaviour
    {
        string destination;
        private bool isProcessing = false;
        public float startX;
        public float startY;
        public float valueX;
        public float valueY;
        Texture2D screenTexture;
        public Image image;
        public GameObject panel;

        public GameObject[] allToHide;
        public GameObject show;
        bool isOn;


        private void Start()
        {
            panel.SetActive(false);
            show.SetActive(false);
            Events.GameOver += GameOver;
        }
        private void OnDestroy()
        {
            Events.GameOver -= GameOver;
        }
        void GameOver()
        {
            if (!isOn) return;
            print("GameOver");
            Close();
        }
        public void CreateScreenshot()
        {
            isOn = true;
            if (Fulbo.Game.GameManager.Instance.state == Fulbo.Game.GameManager.states.GAMEOVER) return;
            Events.OnSkipOff();
            AudioManager.Instance.PlaySound("common", "sfx_photo", false);
            show.SetActive(true);
            SetOn(false);
            if (!isProcessing)
                StartCoroutine(captureScreenshot());
        }

        public IEnumerator captureScreenshot()
        {
            print("captureScreenshot");
            isProcessing = true;
            yield return new WaitForEndOfFrame();

            int _size = (int)(Screen.height * valueX);
            float init_x = (Screen.width / 2) - (_size / 2);
            screenTexture = new Texture2D(_size, _size, TextureFormat.RGB24, true);

            screenTexture.ReadPixels(new Rect
                                     (init_x, (Screen.height * startY), _size, _size)
                                     , 0, 0);
            screenTexture.Apply();
            SetImage();
            yield return new WaitForSeconds(0.1f);
            Time.timeScale = 0;
            panel.SetActive(true);
        }
        void SetImage()
        {
            Rect rec = new Rect(0, 0, screenTexture.width, screenTexture.height);
            Sprite.Create(screenTexture, rec, new Vector2(0, 0), 1);
            image.sprite = Sprite.Create(screenTexture, rec, new Vector2(0, 0), .01f);
        }


        public void Share()
        {
            print("share");
            byte[] dataToSave = screenTexture.EncodeToPNG();
            destination = Path.Combine(Application.persistentDataPath, System.DateTime.Now.ToString("yyyy-MM-dd-HHmmss") + ".png");
            File.WriteAllBytes(destination, dataToSave);
            Invoke("Delayed", 0.25f);

            Destroy(screenTexture);

            new NativeShare().AddFile(destination)
                .SetSubject("Fulbo Stars")
                .SetText("TumbaGames")
                .Share();

            Close();
        }
        public void Close()
        {
            isOn = false;
            Time.timeScale = 1;
            isProcessing = false;
            screenTexture = null;
            panel.SetActive(false);
            SetOn(true);
            show.SetActive(false);
        }
        void SetOn(bool isOn)
        {
            foreach (GameObject go in allToHide)
                go.SetActive(isOn);
//#if UNITY_ANDROID
//            foreach (GameObject go in GetComponent<IngameMenu>().mobilePanels)
//                go.SetActive(isOn);
//#endif
        }
    }
}