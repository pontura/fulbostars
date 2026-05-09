using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Fulbo.UI
{
    public class HideByGameType : MonoBehaviour
    {
        public types hideIfGameIs;
        public enum types
        {
            MOBILE,
            STANDALONE
        }
        void Start()
        {
            if (Data.Instance.isMobile && hideIfGameIs == types.MOBILE || !Data.Instance.isMobile && hideIfGameIs == types.STANDALONE)
                gameObject.SetActive(false);
        }
    }
}


//using System.Collections;
//using System.Collections.Generic;
//using UnityEngine;

//namespace Fulbo.UI
//{
//    public class HideByGameType : MonoBehaviour
//    {
//        public types hideIfGameIs;
//        public enum types
//        {
//            MOBILE,
//            STANDALONE
//        }
//        void Start()
//        {
//            print("HideByGameType" + gameObject.name);
//#if UNITY_ANDROID || UNITY_IOS
//            if (hideIfGameIs == types.MOBILE)
//                gameObject.SetActive(false);
//#else
//            if (hideIfGameIs == types.STANDALONE)
//                gameObject.SetActive(false);
//#endif
//        }
//    }
//}

