using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Fulbo.Game.Powerups
{
    public class Powerup : MonoBehaviour
    {
        public GameObject asset;
        public Character character;
        public Character characterThatCollide;
        public types type;
        public bool isOn;
        public Collider collider;
        public Rigidbody rb;
        public PowerupsManager manager;

        public enum types
        {
            SPEED,
            BOMB,
            SUPERKICK
        }
        public void Init(PowerupsManager manager, Character character)
        {            
            this.manager = manager;
            if(collider != null) collider.enabled = false;
            this.character = character;
            isOn = false;
            OnInstanced();
            Events.OnFX(manager.GetPowerupData(character.powerupsManager.GetPowerupType()).FX_use, transform.position);
        }
        public virtual void OnInstanced() { }
        public void OnShow()
        {
            if (collider != null) collider.enabled = true;
            isOn = true;
        }
        void OnCollisionEnter(Collision other)
        {
            if (!isOn) return;
            else
            {
                Character characterThatCollide = other.gameObject.GetComponent<Character>();
                if (characterThatCollide == null) return;
                CollideWithCharacter(characterThatCollide);
            }
        }
        public virtual void CollideWithCharacter(Character characterThatCollide)
        {
            if (characterThatCollide == character) return;
            this.characterThatCollide = characterThatCollide;
            OnCharacterHitted();            
        }
        public void OnHide()
        {
            if(asset != null)  asset.SetActive(false);
            isOn = false;
            if (rb != null) rb.useGravity = false;
        }
        public virtual void OnCharacterHitted() { }
    }

}