using UnityEngine;

namespace Fulbo.Stadiums
{
    public class GalaxyMapButtonAsset : MonoBehaviour
    {
        [SerializeField] GameObject lockedGO;
        [SerializeField] GameObject unlockedGO;

        public void SetLocked(bool isLocked)
        {
            lockedGO.SetActive(false);
            unlockedGO.SetActive(false);

            if (isLocked)
                lockedGO.SetActive(true);
            else
                unlockedGO.SetActive(true);

            transform.localPosition = Vector3.zero;
            transform.localEulerAngles = Vector3.zero;
        }
    }
}