using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace Fulbo.FX
{
    public class Shadow : FollowTarget
    {
        private void OnEnable()
        {
            target = transform.parent;
            transform.SetParent(Fulbo.Game.GameManager.Instance.shadows);
            transform.localPosition = Vector3.zero;
            transform.localScale = Vector3.one;
        }
    }
}