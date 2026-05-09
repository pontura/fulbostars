using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Rendering;

public class MaskedUI : Image
{
    public override Material materialForRendering {
        get {
            Material material = new Material(base.materialForRendering);
            material.SetColor("_Color", Color.white);
            material.SetFloat("_StencilComp", 3f);
            material.SetFloat("_Stencil", 2);
            material.SetFloat("_StencilOp", 0);
            material.SetFloat("_StencilWriteMask", 0);
            material.SetFloat("_StencilReadMask", 1);
            material.SetFloat("_ColorMask", 15);
            material.SetFloat("_UseUIAlphaClip", 0);
            return material;
        }
    }
}
