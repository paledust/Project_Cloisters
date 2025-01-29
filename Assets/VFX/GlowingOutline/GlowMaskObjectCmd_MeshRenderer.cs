using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

public class GlowMaskObjectCmd_MeshRenderer : GlowMaskObjectCmd
{
    private const string MainTexName = "_MainTex";

    private MaterialPropertyBlock mpb;
    void Start(){
        mpb = new MaterialPropertyBlock();
        m_Renderer.GetPropertyBlock(mpb, 0);
    }
	public override void OnDraw(CommandBuffer cmd, Material drawMat, int subIndex=0){
        m_Renderer.GetPropertyBlock(mpb, subIndex);
        mpb.SetTexture(MainTexName, m_Renderer.sharedMaterials[subIndex].mainTexture);
        m_Renderer.SetPropertyBlock(mpb, subIndex);
	}
}
