using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Fulbo.UI.Carrousel
{
    public class CarrouselItem : ButtonCustom
    {
        [SerializeField] Vector3 offsetView;
        [SerializeField] Transform container;
      //  Transform camera;

        public void InitCarrousel(float degrees, float distance, Transform camera)
        {
            //this.camera = camera;
            //container.transform.localPosition = new Vector3(0, 0, -distance);
            //transform.localEulerAngles = new Vector3(0, degrees, 0);
            //button.Init(ui);
        }
        public void AddAsset(GameObject go)
        {
            go.transform.SetParent(container);
            go.transform.localPosition = Vector3.zero;
            go.transform.localEulerAngles = Vector3.zero;
        }
        private void Update()
        {
          //  container.transform.LookAt(camera.transform.position  + offsetView);
        }
    }
}