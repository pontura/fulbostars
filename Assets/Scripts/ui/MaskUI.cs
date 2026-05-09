using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Rendering;

public class MaskUI : Image
{
    public override Material materialForRendering {
        get {
            Material material = new Material(base.materialForRendering);
            material.SetColor("_Color", new Color(1f, 1f, 1f, 0));
            material.SetFloat("_StencilComp", 8f);
            material.SetFloat("_Stencil", 1);
            material.SetFloat("_StencilOp", 2);
            material.SetFloat("_StencilWriteMask", 255);
            material.SetFloat("_StencilReadMask", 255);
            material.SetFloat("_ColorMask", 0);
            material.SetFloat("_UseUIAlphaClip", 1);
            return material;
        }
    }
}
