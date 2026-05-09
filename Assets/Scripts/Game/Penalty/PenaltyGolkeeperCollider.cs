using UnityEngine;
using UnityEditor;
namespace Fulbo.Game.Penalty
{
    public class PenaltyGolkeeperCollider : MonoBehaviour
    {
        PenaltyCharactersManager charactersManager;

        public void Start()
        {
            GetComponent<MeshRenderer>().enabled = false;
        }
        public void Init(PenaltyCharactersManager charactersManager, Character goalkeeper)
        {
            transform.SetParent(goalkeeper.transform);
            this.charactersManager = charactersManager;
        }
        private void OnCollisionEnter(Collision collision)
        {
            if(collision.gameObject.tag == "Ball")
            {
                charactersManager.CatchBall();
            }
        }
    }
}