using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Fulbo.Game.Tutorial
{ 
    public class TutorialPath : MonoBehaviour
    {
        [SerializeField] GameObject[] paths;

        public List<Vector3> GetPath()
        {
            List<Vector3> arr = new List<Vector3>();
            foreach (GameObject go in paths)
                arr.Add(go.transform.position);
            return arr;
        }   
    }
}
