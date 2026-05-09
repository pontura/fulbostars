using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Fulbo.Game.Powerups
{
    public class PowerupSpawner : MonoBehaviour
    {
        Grabbable grabbable;
        Vector3 offset = new Vector3(0, 2f, 0);

        private void Start()
        {
            Events.OnGrab += OnGrab;
        }
        private void OnDestroy()
        {
            Events.OnGrab -= OnGrab;
        }
        public void Init()
        {
            MeshRenderer mr = gameObject.GetComponent<MeshRenderer>();
            if(mr != null) mr.enabled = false; // turn off graphic:
            SetState(false);
        }
        public void SetState(bool hasGrabbableItem)
        {
            gameObject.SetActive(hasGrabbableItem);
        }
        public bool IsAvailable()
        {
            return grabbable == null;
        }
        public void AddPowerup(Grabbable grabbablePowerUp)
        {
            grabbable = Instantiate(grabbablePowerUp, transform.parent.transform);
            grabbable.transform.localPosition = transform.position + offset;
            SetState(true);
        }
        void OnGrab(Grabbable _grabbable)
        {
            if (grabbable == _grabbable)
            {
                grabbable = null;
            }
        }
    }
}
