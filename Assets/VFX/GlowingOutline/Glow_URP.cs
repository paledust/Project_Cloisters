using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

[System.Serializable, VolumeComponentMenuForRenderPipeline("Custom/Outline", typeof(UniversalRenderPipeline))]
public class Glow_URP : VolumeComponent, IPostProcessComponent
{
    [ColorUsage(true, true)] public ColorParameter outlineColor = new ColorParameter(Color.white, true, true, false);
    [Range(0, 0.1f)] public FloatParameter Intensity = new FloatParameter(0f);
    
	public FloatParameter blurRadius = new FloatParameter(0.7f);
    public FloatParameter Thickness = new FloatParameter(0f);
    public FloatParameter Smoothness = new FloatParameter(1f);

    public bool IsActive()=>Intensity.value > 0;
    public bool IsTileCompatible() => true;

#region Glow Control
	public static HashSet<GlowObjectCmd> glowableObjects{get; private set;} = new HashSet<GlowObjectCmd>();
	public static HashSet<GlowMaskObjectCmd> glowmaskObjects{get; private set;} = new HashSet<GlowMaskObjectCmd>();

	public static void RegisterGlowObject(GlowObjectCmd glowObj)=>glowableObjects.Add(glowObj);
	public static void UnregisterGlowObject(GlowObjectCmd glowObj)=>glowableObjects.Remove(glowObj);
	public static void RegisterMask(GlowMaskObjectCmd glowMask)=>glowmaskObjects.Add(glowMask);
	public static void UnregisterMask(GlowMaskObjectCmd glowMask)=>glowmaskObjects.Remove(glowMask);
#endregion
}
[System.Serializable]
public class Glow_URP_Pass : ScriptableRenderPass
{
#region GlowCompose
    private Material _composeMaterial;
    private int screenTexID = Shader.PropertyToID("ScreenTexID");
    private int outlineColID = Shader.PropertyToID("_OutlineColor");
    private int intensityID = Shader.PropertyToID("_Intensity");
    private int thicknessID = Shader.PropertyToID("_Thickness");
    private int smoothnessID = Shader.PropertyToID("_Smoothness");
    private RenderTargetIdentifier src;
    private RenderTargetIdentifier tempID;
#endregion

#region GlowControl
	public HashSet<GlowObjectCmd> _glowableObjects;
	public HashSet<GlowMaskObjectCmd> _glowmaskObjects;

	private Material _glowMat;
	private Material _blurMaterial;

	private Vector2 _blurTexelSize;

	private int _prePassRenderTexID;
	private int _blurPassRenderTexID;
	private int _temp1RenderTexID;
	private int _maskPassRenderTexID;
	
	private int _blurSizeID;
	private int _blurRadiusID;
	private int _glowColorID;

	private float blurRadius;

	private RenderTargetIdentifier prepassID;
	private RenderTargetIdentifier blurID;
	private RenderTargetIdentifier temp1ID;
	private RenderTargetIdentifier maskID;
#endregion

    public Glow_URP_Pass(){
        renderPassEvent = RenderPassEvent.BeforeRenderingPostProcessing;
    }

    public override void OnCameraSetup(CommandBuffer cmd, ref RenderingData renderingData){
    //Glow Control
		_glowMat = new Material(Shader.Find("Hidden/AmplifyShaders/GlowReplace"));
		_blurMaterial = new Material(Shader.Find("Hidden/GausseBlur"));

		_blurSizeID = Shader.PropertyToID("_BlurSize");
		_blurRadiusID = Shader.PropertyToID("_BlurRadius");
		_glowColorID = Shader.PropertyToID("GlowColor");

		_prePassRenderTexID = Shader.PropertyToID("_GlowPrePassTex");
		_blurPassRenderTexID = Shader.PropertyToID("_GlowBlurredTex");
		_temp1RenderTexID = Shader.PropertyToID("_TempTex0");
		_maskPassRenderTexID = Shader.PropertyToID("_GlowMaskTex");

		temp1ID = new RenderTargetIdentifier(_temp1RenderTexID);
		prepassID = new RenderTargetIdentifier(_prePassRenderTexID);
		blurID = new RenderTargetIdentifier(_blurPassRenderTexID);
		maskID = new RenderTargetIdentifier(_maskPassRenderTexID);
        
    //Glow Compose
        _composeMaterial = CoreUtils.CreateEngineMaterial(Shader.Find("Hidden/GlowComposite"));

        src = renderingData.cameraData.renderer.cameraColorTargetHandle;
        
        RenderTextureDescriptor descriptor = renderingData.cameraData.cameraTargetDescriptor;
        cmd.GetTemporaryRT(screenTexID, descriptor, FilterMode.Bilinear);
        tempID = new RenderTargetIdentifier(screenTexID);
    }
    public override void Execute(ScriptableRenderContext context, ref RenderingData renderingData){
        if(_composeMaterial == null){
            Debug.LogError("Outline effect Materials instance is null");
            return;
        }
    //Get Glow Effect Stack
        var glowEffect = VolumeManager.instance.stack.GetComponent<Glow_URP>();
        if(glowEffect.IsActive()){
            _glowableObjects = Glow_URP.glowableObjects;
		    _glowmaskObjects = Glow_URP.glowmaskObjects;

            if(_glowableObjects.Count == 0) 
                return;

            _composeMaterial.SetFloat(intensityID, glowEffect.Intensity.value);
            _composeMaterial.SetFloat(thicknessID, glowEffect.Thickness.value);
            _composeMaterial.SetFloat(smoothnessID, glowEffect.Smoothness.value);
            _composeMaterial.SetColor(outlineColID, glowEffect.outlineColor.value);
            blurRadius = glowEffect.blurRadius.value;
        }
        else
            return;
        
    //Render Glow Object
        CommandBuffer cmd = CommandBufferPool.Get("Outline");
        cmd.Clear();

		int width = Screen.width;
		int height = Screen.height;

		cmd.GetTemporaryRT(_prePassRenderTexID, width, height, 0, FilterMode.Bilinear, RenderTextureFormat.ARGB32, RenderTextureReadWrite.Default);
		cmd.SetRenderTarget(prepassID);
		cmd.ClearRenderTarget(true, true, Color.clear);

		foreach(var glowObj in _glowableObjects){
			cmd.SetGlobalColor(_glowColorID, glowObj.GlowColor);
			var renderer = glowObj.m_renderer;
			for(int subIndex = 0; subIndex<renderer.sharedMaterials.Length; subIndex++){
				cmd.DrawRenderer(glowObj.m_renderer, _glowMat, subIndex);
			}
		}

		cmd.GetTemporaryRT(_blurPassRenderTexID, width >> 1, height >> 1, 0, FilterMode.Bilinear);
		cmd.GetTemporaryRT(_temp1RenderTexID, width >> 1, height >> 1, 0, FilterMode.Bilinear);

		cmd.Blit(prepassID, blurID);

		_blurTexelSize = new Vector2(1.5f / (width >> 1), 1.5f / (height >> 1));
		cmd.SetGlobalVector(_blurSizeID, _blurTexelSize);
		cmd.SetGlobalFloat(_blurRadiusID, blurRadius);

		cmd.Blit(blurID, temp1ID, _blurMaterial, 0);
		for (int i = 0; i < 8; i++)
		{
			cmd.Blit(temp1ID, blurID, _blurMaterial, 1);
			cmd.Blit(blurID, temp1ID, _blurMaterial, 2);
		}
		cmd.Blit(temp1ID, blurID);

    //Render Glow Mask
		cmd.GetTemporaryRT(_maskPassRenderTexID, width, height, 0, FilterMode.Bilinear, RenderTextureFormat.ARGB32, RenderTextureReadWrite.Default);
		cmd.SetRenderTarget(maskID);
		cmd.ClearRenderTarget(true, true, Color.clear);

		foreach(var maskObj in _glowmaskObjects){
			cmd.SetGlobalColor(_glowColorID, Color.white);

			if(!maskObj.m_Renderer.isVisible) continue; //Skip unseen object

			for(int subIndex = 0; subIndex<maskObj.m_Renderer.sharedMaterials.Length; subIndex++){
				maskObj.OnDraw(cmd, _glowMat, subIndex);
				cmd.DrawRenderer(maskObj.m_Renderer, _glowMat, subIndex);
			}
		}

    //Compose Command
        Blit(cmd, src, tempID, _composeMaterial, 0);
        Blit(cmd, tempID, src);

        context.ExecuteCommandBuffer(cmd);
        CommandBufferPool.Release(cmd);
    }
    public override void OnCameraCleanup(CommandBuffer cmd){
        cmd.ReleaseTemporaryRT(screenTexID);
        cmd.ReleaseTemporaryRT(_prePassRenderTexID);
		cmd.ReleaseTemporaryRT(_blurPassRenderTexID);
		cmd.ReleaseTemporaryRT(_temp1RenderTexID);
		cmd.ReleaseTemporaryRT(_maskPassRenderTexID);
    }
}