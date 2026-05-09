using UnityEngine;
using System.Collections;
using System.Collections.Generic;
#if UNITY_ANDROID
using Google.Play.Review;

namespace Fulbo
{
    public class ReviewRequest : MonoBehaviour
    {
        ReviewManager _reviewManager;
        PlayReviewInfo _playReviewInfo;

        int reviewRequestState;
        public void Check()
        {
            reviewRequestState = DB.DBManager.Instance.DbUserData.data.gameData.reviewRequestState;
            print("Check review reviewRequestState:" + reviewRequestState);
            // 1 sended
            // 2 dont
            // 3 later
            if (reviewRequestState != 1 && reviewRequestState != 2 && IsANewDay() && DB.DBManager.Instance.DbUserData.data.gameData.cups.cupsPlayed.Count>=2)
            {
                string title = Data.Instance.texts.Get("review_title");
                string text = Data.Instance.texts.Get("review_text");

                string btn1 = Data.Instance.texts.Get("review_ok");
                string btn2 = Data.Instance.texts.Get("review_no");
                string btn3 = Data.Instance.texts.Get("review_later");

                Events.OnConfirmPanel3Buttons(title, text, OnClicked, btn1, btn2, btn3);
            }
        }
        void OnClicked(int id)
        {
            if (id == 1)
            {
                OpenPopupReview();
                //  Application.OpenURL(Data.Instance.GetStoreURL());
            }
            // 1 send
            // 2 dont send
            // 3 later
            DB.DBManager.Instance.DbUserData.data.gameData.SaveReview(id, Saved);

            Dictionary<string, object> param = new Dictionary<string, object>();
            string result = "";
            switch (id)
            {
                case 1: result = "send"; break;
                case 2:  result = "dont"; break;
                case 3:  result = "later"; break;
            }
            param["action"] = result;
            Events.OnTrack("ReviewRequest", param);
        }
        void Saved(bool isOk, string text)
        {
           
        }
        public bool IsANewDay()
        {
            return Utils.Today(DB.DBManager.Instance.versionMode == DB.DBManager.versionModes.PROD) != DB.DBManager.Instance.DbUserData.data.gameData.review_day;
        }


        public void OpenPopupReview()
        {
            _reviewManager = new ReviewManager();
            StartCoroutine(review());
        }

        IEnumerator review()
        {
            yield return new WaitForSeconds(1f);

            var requestFlowOperation = _reviewManager.RequestReviewFlow();
            yield return requestFlowOperation;
            if (requestFlowOperation.Error != ReviewErrorCode.NoError)
            {
                Events.OnPopup(requestFlowOperation.Error.ToString(), null);
                yield break;
            }
            _playReviewInfo = requestFlowOperation.GetResult();
            var launchFlowOperation = _reviewManager.LaunchReviewFlow(_playReviewInfo);
            yield return launchFlowOperation;
            _playReviewInfo = null; // Reset the object
            if (launchFlowOperation.Error != ReviewErrorCode.NoError)
            {
                Events.OnPopup(requestFlowOperation.Error.ToString(), null);
                // Log error. For example, using requestFlowOperation.Error.ToString().
                yield break;
            }
            // The flow has finished. The API does not indicate whether the user
            // reviewed or not, or even whether the review dialog was shown. Thus, no
            // matter the result, we continue our app flow.

        }
    }
}

#endif              