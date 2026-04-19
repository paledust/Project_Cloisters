Shader "Water_FX/WaterSimulation"
{
	Properties
	{
    	_S2("PhaseVelocity^2", Range(0.0, .5)) = 0.2
    	[PowerSlider(0.01)]
    	_Atten("Attenuation", Range(0.0, 1.0)) = 0.999
    	_DeltaUV("Delta UV", Float) = 3
		_Force("Force", Float) = 0
		_FlowAmount("Flow Amount", Float) = 50
		_FlowSpeed("Flow Speed", Float) = 0.1
		_FlowNoise("Flow Noise", Range(0,1)) = 0
		_NoiseStrength("Noise Strength", Float) = 1
		_NoiseScale("Noise Scale", Float) = 100
		_DynamicTex("DynamicTexture", 2D) = "Black"
	}
	//CGINCLUDE------------------------
		CGINCLUDE

		#include "UnityCustomRenderTexture.cginc"
		#pragma shader_feature USE_TEXTURE
		
		uniform half _S2;
		uniform half _NoiseScale;
		uniform half _NoiseStrength;
		uniform half _FlowAmount;
		uniform float _Atten;
		uniform float _DeltaUV;
		uniform float _Force;
		uniform float _FlowSpeed;
		uniform float _FlowNoise;

		uniform sampler2D _DynamicTex;

		float random (float2 st) {
			return frac(sin(dot(st.xy,
								float2(12.9898,78.233)))*
				43758.5453123);
		}
		float randomTime(float2 st) {
			return frac(sin(dot(st.xy, float2(12.9898,78.233)) + random(float2(_Time.xx)))*43758.5453123);
		}
		float2 GradientNoiseDir( float2 x )
		{
			const float2 k = float2( 0.3183099, 0.3678794 );
			x = x * k + k.yx;
			return -1.0 + 2.0 * frac( 16.0 * k * frac( x.x * x.y * ( x.x + x.y ) ) );
		}
		
		float GradientNoise( float2 UV, float Scale )
		{
			float2 p = UV * Scale;
			float2 i = floor( p );
			float2 f = frac( p );
			float2 u = f * f * ( 3.0 - 2.0 * f );
			return lerp( lerp( dot( GradientNoiseDir( i + float2( 0.0, 0.0 ) ), f - float2( 0.0, 0.0 ) ),
					dot( GradientNoiseDir( i + float2( 1.0, 0.0 ) ), f - float2( 1.0, 0.0 ) ), u.x ),
					lerp( dot( GradientNoiseDir( i + float2( 0.0, 1.0 ) ), f - float2( 0.0, 1.0 ) ),
					dot( GradientNoiseDir( i + float2( 1.0, 1.0 ) ), f - float2( 1.0, 1.0 ) ), u.x ), u.y );
		}
		fixed4 frag (v2f_customrendertexture i) : SV_Target
		{
			float2 uv = i.globalTexcoord;


    		float du = 1.0 / _CustomRenderTextureWidth;
    		float dv = 1.0 / _CustomRenderTextureHeight;
    		float3 duv = float3(du, dv, 0) * _DeltaUV;

    		float4 c = tex2D(_SelfTexture2D, uv);

			float height = (2 * c.r - c.g + _S2 * (
        					tex2D(_SelfTexture2D, uv - duv.zy).r +
        					tex2D(_SelfTexture2D, uv + duv.zy).r +
        					tex2D(_SelfTexture2D, uv - duv.xz).r +
        					tex2D(_SelfTexture2D, uv + duv.xz).r - 4 * c.r)) * _Atten;

			float _dh = tex2D(_DynamicTex, uv).r*_Force;
			height += _dh;

			// float noise = saturate(GradientNoise(uv+float2(0.255123,_Time.x*_DeltaUV),100));
			// float noise = saturate(GradientNoise(uv+float2(0.255123,0.61345),20));

			float noisePan = float2(0.255123,_Time.x*_DeltaUV);
			float flow = c.a;
			flow += _dh*_FlowAmount;
			float change = _FlowSpeed*(
				(tex2D(_SelfTexture2D, uv - duv.zy).a-c.a) * lerp(1, 1-_NoiseStrength, _FlowNoise*(GradientNoise(uv - duv.zy + noisePan,_NoiseScale))) +
        		(tex2D(_SelfTexture2D, uv + duv.zy).a-c.a) * lerp(1, 1-_NoiseStrength, _FlowNoise*(GradientNoise(uv + duv.zy + noisePan,_NoiseScale))) +
        		(tex2D(_SelfTexture2D, uv - duv.xz).a-c.a) * lerp(1, 1-_NoiseStrength, _FlowNoise*(GradientNoise(uv - duv.xz + noisePan,_NoiseScale))) +
        		(tex2D(_SelfTexture2D, uv + duv.xz).a-c.a) * lerp(1, 1-_NoiseStrength, _FlowNoise*(GradientNoise(uv + duv.xz + noisePan,_NoiseScale)))

				// tex2D(_SelfTexture2D, uv - duv.zy).a * lerp(1, 1-_NoiseStrength, _FlowNoise*(GradientNoise(uv - duv.zy + noisePan,_NoiseScale))) +
        		// tex2D(_SelfTexture2D, uv + duv.zy).a * lerp(1, 1-_NoiseStrength, _FlowNoise*(GradientNoise(uv + duv.zy + noisePan,_NoiseScale))) +
        		// tex2D(_SelfTexture2D, uv - duv.xz).a * lerp(1, 1-_NoiseStrength, _FlowNoise*(GradientNoise(uv - duv.xz + noisePan,_NoiseScale))) +
        		// tex2D(_SelfTexture2D, uv + duv.xz).a * lerp(1, 1-_NoiseStrength, _FlowNoise*(GradientNoise(uv + duv.xz + noisePan,_NoiseScale))) - c.a*4
			);
			
			flow += change;
			flow = max(0,flow);
			flow = max(c.a, flow);

			return float4(height, c.r, c.a, flow);
		}

		float4 frag_left_click(v2f_customrendertexture i) : SV_Target
		{
			return float4(-_Force, 0, 0, 0);
		}

		float4 frag_right_click(v2f_customrendertexture i) : SV_Target
		{
			return float4(_Force, 0, 0, 0);
		}

		ENDCG
		
	SubShader
	{
		Tags { "RenderType"="Opaque" }

		Cull Off ZWrite Off ZTest Always

		Pass
		{
			Name "Update"
			CGPROGRAM
			#pragma vertex CustomRenderTextureVertexShader
			#pragma fragment frag

			ENDCG
		}

		Pass
		{
			Name "LeftClick"
			CGPROGRAM
			#pragma vertex CustomRenderTextureVertexShader
			#pragma fragment frag_left_click

			ENDCG
		}

		Pass
		{
			Name "RightClick"
			CGPROGRAM
			#pragma vertex CustomRenderTextureVertexShader
			#pragma fragment frag_right_click

			ENDCG
		}
	}
}
