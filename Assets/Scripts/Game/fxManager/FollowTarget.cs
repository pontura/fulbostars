using UnityEngine;

namespace Fulbo.FX
{
    public class FollowTarget : MonoBehaviour
    {
        public Transform target;

        void Update()
        {
            if (target == null)
                Destroy(this.gameObject);
            else
            {
                Vector3 pos = target.localPosition;
                pos.y = 0;
                transform.localPosition = pos;
            }
        }
    }

}