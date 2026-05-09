using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Fulbo.Game
{
    public class LineSignal : MonoBehaviour
    {
        [SerializeField] GameObject asset;
        Character character;
        Character other;
        public float offset = 1;

        Character Character()
        {
            if (character == null)
                character = GetComponentInParent<Character>();
            return character;
        }
        public void SetOn(bool isOn)
        {
            CancelInvoke();
            this.gameObject.SetActive(isOn);
            if (isOn)
                CheckIfPasePossible();
            else
                OnDisable();
        }
        private void OnDisable()
        {
            asset.SetActive(false);
            CancelInvoke();
            other = null;
        }
        void CheckIfPasePossible()
        {
            other = Fulbo.Game.GameManager.Instance.charactersManager.charactersStrategy.GetCharacterEnPase(Character());
            if (other == null)
                asset.SetActive(false);
            else
                asset.SetActive(true);
            Invoke("CheckIfPasePossible", 0.25f);
        }
        void Update()
        {
            if (other == null)
                return;

            Vector3 lookTo = other.transform.position;
            float dist = Vector3.Distance(transform.position, other.transform.position);
            asset.transform.LookAt(lookTo);
            asset.transform.localScale = new Vector3(1, 1, dist * offset);
        }
        public bool HasCharacterInLine()
        {
            return other;
        }
    }

}