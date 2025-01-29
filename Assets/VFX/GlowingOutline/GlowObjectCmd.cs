using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

public class GlowObjectCmd : MonoBehaviour
{
	[ColorUsage(true, true)] public Color GlowColor = new Color(1,1,1,0);

	public Renderer m_renderer{get;private set;}

	void OnEnable()
	{
		m_renderer = GetComponent<Renderer>();
		// GlowController.RegisterGlowObject(this);
		Glow_URP.RegisterGlowObject(this);
	}
	void OnDisable(){
		// GlowController.UnregisterGlowObject(this);
		Glow_URP.UnregisterGlowObject(this);
	}
}
