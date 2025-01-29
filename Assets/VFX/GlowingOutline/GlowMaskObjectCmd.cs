using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

public class GlowMaskObjectCmd : MonoBehaviour
{
	public Renderer m_Renderer{get; protected set;}
	public virtual void OnDraw(CommandBuffer cmd, Material drawMat, int subIndex=0){}
	void OnEnable()
	{
		m_Renderer = GetComponent<Renderer>();
		// GlowController.RegisterMask(this);
		Glow_URP.RegisterMask(this);
	}
    void OnDisable(){
        // GlowController.UnregisterMask(this);
		Glow_URP.UnregisterMask(this);
    }
}