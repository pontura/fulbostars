using UnityEngine;
namespace Fulbo.Tournamets
{
    public class TournamentButton : MonoBehaviour
    {
        Animator anim;
        private void Start()
        {
            anim = GetComponent<Animator>();
        }
        public void SetOn(bool isOn)
        {
            anim.SetBool("isOn", isOn);
        }
    }
}
