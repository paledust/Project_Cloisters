// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

Shader "Hidden/GlowComposite"
{
	Properties
	{
		_MainTex ("Texture", 2D) = "white" {}
	}
    HLSLINCLUDE
// StdLib.hlsl holds pre-configured vertex shaders (VertDefault), varying structs (VaryingsDefault), and most of the data you need to write common effects.
    #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/SurfaceInput.hlsl"

    TEXTURE2D(_MainTex);
    TEXTURE2D(_GlowMaskTex);
	TEXTURE2D(_GlowBlurredTex);
	TEXTURE2D(_GlowPrePassTex);

    SAMPLER(sampler_MainTex);
    SAMPLER(sampler_GlowMaskTex);
    SAMPLER(sampler_GlowBlurredTex);
    SAMPLER(sampler_GlowPrePassTex);

	float4 _OutlineColor;
    float _Intensity;
	float _Thickness;
	float _Smoothness;
    

    struct Attributes
    {
        float4 positionOS : POSITION;
        float2 uv         : TEXCOORD0;
    };

    struct Varyings
    {
        float2 uv        : TEXCOORD0;
        float4 vertex : SV_POSITION;
        UNITY_VERTEX_OUTPUT_STEREO
    };

    Varyings vert(Attributes input)
    {
        Varyings o = (Varyings)0;
        UNITY_INITIALIZE_VERTEX_OUTPUT_STEREO(o)
        VertexPositionInputs vertexInput = GetVertexPositionInputs(input.positionOS.xyz);
        o.vertex = vertexInput.positionCS;
        o.uv = input.uv;
        
        return o;
    }
    float4 Frag (Varyings input) : SV_Target 
    {
        // UNITY_SETUP_STEREO_EYE_INDEX_POST_VERTEX(input);
    	float4 color = SAMPLE_TEXTURE2D(_MainTex, sampler_MainTex, input.uv);
        float4 mask = SAMPLE_TEXTURE2D(_GlowMaskTex, sampler_GlowMaskTex, input.uv);
		float4 glow = max(0, SAMPLE_TEXTURE2D(_GlowBlurredTex, sampler_GlowBlurredTex, input.uv) - 4*SAMPLE_TEXTURE2D(_GlowPrePassTex, sampler_GlowPrePassTex, input.uv));
		glow.rgb = smoothstep(_Thickness, _Thickness+_Smoothness, glow.rgb);
        glow.rgb = max(0, glow.rgb - mask.rgb);
		return color + glow*_Intensity*_OutlineColor;
    }

    ENDHLSL
    SubShader
    {
        Cull Off ZWrite Off ZTest Always
        Pass
        {
            HLSLPROGRAM
                #pragma vertex vert
                #pragma fragment Frag
            ENDHLSL
        }
    }
}
