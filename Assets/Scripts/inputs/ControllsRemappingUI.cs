using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

namespace Fulbo.Input
{
    public class ControllsRemappingUI : MonoBehaviour
    {
        [SerializeField] private InputActionAsset asset;
        [SerializeField] private TextAsset default_1;
        [SerializeField] private TextAsset default_2;

        void OnEnable()
        {
            asset.Disable();
            var rebinds = PlayerPrefs.GetString("rebinds");
            if (!string.IsNullOrEmpty(rebinds))
                asset.LoadBindingOverridesFromJson(rebinds);
        }
        public void OnDisable()
        {
            var rebinds = asset.SaveBindingOverridesAsJson();
            //print(" rebinds: " + rebinds);
            PlayerPrefs.SetString("rebinds", rebinds);
            Save();
           // Data.Instance.GetComponent<InputManager>().Init();
        }
        public void Back()
        {
            gameObject.SetActive(false);
        }
        public void Save()
        {
            foreach (var actionMap in asset.actionMaps)
            {
                foreach (var binding in actionMap.bindings)
                {
                    if (!string.IsNullOrEmpty(binding.overridePath))
                    {
                        string key = binding.id.ToString();
                        string val = binding.overridePath;
                        PlayerPrefs.SetString(key, val);
                    }
                }
            }
        }
        public void SetDefault(int id)
        {

            string rebinds;
            if (id == 1)
                rebinds = default_1.text;
            else
                rebinds = default_2.text;
            PlayerPrefs.SetString("rebinds", rebinds);
            asset.Disable();
            asset.LoadBindingOverridesFromJson(rebinds);
         //   Data.Instance.GetComponent<InputManager>().Init();
        }

       
    }
}
