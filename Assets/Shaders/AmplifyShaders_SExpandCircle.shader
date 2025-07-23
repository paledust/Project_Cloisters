// Made with Amplify Shader Editor v1.9.8.1
// Available at the Unity Asset Store - http://u3d.as/y3X 
Shader "AmplifyShaders/Sprite ExpandCircle"
{
	Properties
	{
		[HideInInspector] _EmissionColor("Emission Color", Color) = (1,1,1,1)
		[HideInInspector] _AlphaCutoff("Alpha Cutoff ", Range(0, 1)) = 0.5
		_MainTex("MainTex", 2D) = "white" {}
		_CircleRadius("CircleRadius", Float) = 0.3
		_CircleSmooth("CircleSmooth", Float) = 0.05
		_Glow("Glow", Float) = 0
		_GlowSmooth("GlowSmooth", Float) = 1
		_GlowStrength("GlowStrength", Range( 0 , 1)) = 1
		_NoiseMap("NoiseMap", 2D) = "white" {}
		_NoiseMin("NoiseMin", Float) = 0
		_NoiseSmooth("NoiseSmooth", Float) = 1
		_NoiseStrength("NoiseStrength", Float) = 0.2
		_NoisePan("NoisePan", Vector) = (0,0,0,0)
		_ExternalNoiseMovement("ExternalNoiseMovement", Float) = 0
		_RadialScale("RadialScale", Float) = 1
		_LengthScale("LengthScale", Float) = 1
		_DissolveDistance("DissolveDistance", Float) = 0
		_DissolveNoise("DissolveNoise", 2D) = "white" {}
		_DissolveNoiseStrength("DissolveNoiseStrength", Float) = 0.1
		_Dissolve("Dissolve", Float) = 0
		_DissolveSmooth("DissolveSmooth", Float) = 0.01
		_HurScale("HurScale", Float) = 1
		_HurlStrength("HurlStrength", Float) = 0.5
		[HideInInspector] _texcoord( "", 2D ) = "white" {}

		[HideInInspector][NoScaleOffset] unity_Lightmaps("unity_Lightmaps", 2DArray) = "" {}
        [HideInInspector][NoScaleOffset] unity_LightmapsInd("unity_LightmapsInd", 2DArray) = "" {}
        [HideInInspector][NoScaleOffset] unity_ShadowMasks("unity_ShadowMasks", 2DArray) = "" {}
	}

	SubShader
	{
		LOD 0

		

		Tags { "RenderPipeline"="UniversalPipeline" "RenderType"="Transparent" "Queue"="Transparent" }

		Cull Off

		HLSLINCLUDE
		#pragma target 2.0
		#pragma prefer_hlslcc gles
		// ensure rendering platforms toggle list is visible

		#include "Packages/com.unity.render-pipelines.core/ShaderLibrary/Common.hlsl"
		#include "Packages/com.unity.render-pipelines.core/ShaderLibrary/Filtering.hlsl"

		ENDHLSL

		
		Pass
		{
			Name "Sprite Unlit"
            Tags { "LightMode"="Universal2D" }

			Blend SrcAlpha OneMinusSrcAlpha, One OneMinusSrcAlpha
			ZTest LEqual
			ZWrite Off
			Offset 0 , 0
			ColorMask RGBA
			

			HLSLPROGRAM

			#define ASE_VERSION 19801
			#define ASE_SRP_VERSION 120112


			#pragma vertex vert
			#pragma fragment frag

            #define _SURFACE_TYPE_TRANSPARENT 1
            #define ATTRIBUTES_NEED_NORMAL
            #define ATTRIBUTES_NEED_TANGENT
            #define ATTRIBUTES_NEED_TEXCOORD0
            #define ATTRIBUTES_NEED_COLOR
            #define VARYINGS_NEED_POSITION_WS
            #define VARYINGS_NEED_TEXCOORD0
            #define VARYINGS_NEED_COLOR
            #define FEATURES_GRAPH_VERTEX

			#define SHADERPASS SHADERPASS_SPRITEUNLIT

			#include "Packages/com.unity.render-pipelines.core/ShaderLibrary/Color.hlsl"
			#include "Packages/com.unity.render-pipelines.core/ShaderLibrary/Texture.hlsl"
			#include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
			#include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Lighting.hlsl"
			#include "Packages/com.unity.render-pipelines.core/ShaderLibrary/TextureStack.hlsl"
			#include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/ShaderGraphFunctions.hlsl"
			#include "Packages/com.unity.render-pipelines.universal/Editor/ShaderGraph/Includes/ShaderPass.hlsl"

			#include "Packages/com.unity.render-pipelines.universal/Shaders/2D/Include/SurfaceData2D.hlsl"
			#include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Debug/Debugging2D.hlsl"

			#define ASE_NEEDS_FRAG_COLOR
			#pragma multi_compile_instancing


			sampler2D _MainTex;
			sampler2D _NoiseMap;
			sampler2D _DissolveNoise;
			UNITY_INSTANCING_BUFFER_START(AmplifyShadersSpriteExpandCircle)
				UNITY_DEFINE_INSTANCED_PROP(float4, _MainTex_ST)
				UNITY_DEFINE_INSTANCED_PROP(float4, _NoiseMap_ST)
				UNITY_DEFINE_INSTANCED_PROP(float4, _DissolveNoise_ST)
				UNITY_DEFINE_INSTANCED_PROP(float, _ExternalNoiseMovement)
			UNITY_INSTANCING_BUFFER_END(AmplifyShadersSpriteExpandCircle)
			CBUFFER_START( UnityPerMaterial )
			float4 _NoisePan;
			float _CircleSmooth;
			float _DissolveNoiseStrength;
			float _DissolveDistance;
			float _DissolveSmooth;
			float _Dissolve;
			float _GlowStrength;
			float _GlowSmooth;
			float _Glow;
			float _NoiseStrength;
			float _LengthScale;
			float _RadialScale;
			float _NoiseSmooth;
			float _NoiseMin;
			float _CircleRadius;
			float _HurScale;
			float _HurlStrength;
			CBUFFER_END


			struct VertexInput
			{
				float4 positionOS : POSITION;
				float3 normal : NORMAL;
				float4 tangent : TANGENT;
				float4 uv0 : TEXCOORD0;
				float4 color : COLOR;
				
				UNITY_VERTEX_INPUT_INSTANCE_ID
			};

			struct VertexOutput
			{
				float4 positionCS : SV_POSITION;
				float4 texCoord0 : TEXCOORD0;
				float4 color : TEXCOORD1;
				float3 positionWS : TEXCOORD2;
				float4 ase_texcoord3 : TEXCOORD3;
				UNITY_VERTEX_INPUT_INSTANCE_ID
				UNITY_VERTEX_OUTPUT_STEREO
			};

			#if ETC1_EXTERNAL_ALPHA
				TEXTURE2D(_AlphaTex); SAMPLER(sampler_AlphaTex);
				float _EnableAlphaTexture;
			#endif

			float4 _RendererColor;

			
			VertexOutput vert( VertexInput v  )
			{
				VertexOutput o = (VertexOutput)0;

				UNITY_SETUP_INSTANCE_ID(v);
				UNITY_TRANSFER_INSTANCE_ID(v, o);
				UNITY_INITIALIZE_VERTEX_OUTPUT_STEREO(o);

				float4 ase_positionCS = TransformObjectToHClip( ( v.positionOS ).xyz );
				float4 screenPos = ComputeScreenPos( ase_positionCS );
				o.ase_texcoord3 = screenPos;
				
				#ifdef ASE_ABSOLUTE_VERTEX_POS
					float3 defaultVertexValue = v.positionOS.xyz;
				#else
					float3 defaultVertexValue = float3(0, 0, 0);
				#endif
				float3 vertexValue = defaultVertexValue;
				#ifdef ASE_ABSOLUTE_VERTEX_POS
					v.positionOS.xyz = vertexValue;
				#else
					v.positionOS.xyz += vertexValue;
				#endif
				v.normal = v.normal;
				v.tangent.xyz = v.tangent.xyz;

				VertexPositionInputs vertexInput = GetVertexPositionInputs(v.positionOS.xyz);

				o.positionCS = vertexInput.positionCS;
				o.positionWS = vertexInput.positionWS;
				o.texCoord0 = v.uv0;
				o.color = v.color;
				return o;
			}

			half4 frag( VertexOutput IN  ) : SV_Target
			{
				UNITY_SETUP_INSTANCE_ID(IN);
				UNITY_SETUP_STEREO_EYE_INDEX_POST_VERTEX(IN);

				float4 _MainTex_ST_Instance = UNITY_ACCESS_INSTANCED_PROP(AmplifyShadersSpriteExpandCircle,_MainTex_ST);
				float2 uv_MainTex = IN.texCoord0.xy * _MainTex_ST_Instance.xy + _MainTex_ST_Instance.zw;
				float4 tex2DNode8 = tex2D( _MainTex, uv_MainTex );
				float2 texCoord4 = IN.texCoord0.xy * float2( 1,1 ) + float2( 0,0 );
				float temp_output_23_0 = length( ( texCoord4 - float2( 0.5,0.5 ) ) );
				float _ExternalNoiseMovement_Instance = UNITY_ACCESS_INSTANCED_PROP(AmplifyShadersSpriteExpandCircle,_ExternalNoiseMovement);
				float2 appendResult100 = (float2(_ExternalNoiseMovement_Instance , 0.0));
				float2 appendResult73 = (float2(_NoisePan.x , _NoisePan.y));
				float4 _NoiseMap_ST_Instance = UNITY_ACCESS_INSTANCED_PROP(AmplifyShadersSpriteExpandCircle,_NoiseMap_ST);
				float2 uv_NoiseMap = IN.texCoord0.xy * _NoiseMap_ST_Instance.xy + _NoiseMap_ST_Instance.zw;
				float2 temp_output_34_0_g3 = ( uv_NoiseMap - float2( 0.5,0.5 ) );
				float2 break39_g3 = temp_output_34_0_g3;
				float2 appendResult50_g3 = (float2(( _RadialScale * ( length( temp_output_34_0_g3 ) * 2.0 ) ) , ( ( atan2( break39_g3.x , break39_g3.y ) * ( 1.0 / TWO_PI ) ) * _LengthScale )));
				float2 panner37 = ( 1.0 * _Time.y * appendResult73 + appendResult50_g3);
				float smoothstepResult94 = smoothstep( _NoiseMin , ( _NoiseMin + _NoiseSmooth ) , tex2D( _NoiseMap, ( appendResult100 + panner37 ) ).r);
				float temp_output_42_0 = ( -( temp_output_23_0 - _CircleRadius ) + ( smoothstepResult94 * _NoiseStrength ) );
				float smoothstepResult12 = smoothstep( 0.0 , ( 0.0 + _CircleSmooth ) , temp_output_42_0);
				float smoothstepResult43 = smoothstep( _Glow , ( _Glow + _GlowSmooth ) , temp_output_42_0);
				float2 appendResult74 = (float2(_NoisePan.z , _NoisePan.w));
				float2 dissolvePan75 = appendResult74;
				float4 _DissolveNoise_ST_Instance = UNITY_ACCESS_INSTANCED_PROP(AmplifyShadersSpriteExpandCircle,_DissolveNoise_ST);
				float2 uv_DissolveNoise = IN.texCoord0.xy * _DissolveNoise_ST_Instance.xy + _DissolveNoise_ST_Instance.zw;
				float2 panner76 = ( 1.0 * _Time.y * dissolvePan75 + uv_DissolveNoise);
				float dissolveGuide53 = ( tex2D( _DissolveNoise, panner76 ).r * _DissolveNoiseStrength );
				float4 screenPos = IN.ase_texcoord3;
				float4 ase_positionSSNorm = screenPos / screenPos.w;
				ase_positionSSNorm.z = ( UNITY_NEAR_CLIP_VALUE >= 0 ) ? ase_positionSSNorm.z : ase_positionSSNorm.z * 0.5 + 0.5;
				float2 appendResult82 = (float2(ase_positionSSNorm.x , ase_positionSSNorm.y));
				float dotResult4_g4 = dot( ( ( appendResult82 / ase_positionSSNorm.w ) * _HurScale ) , float2( 12.9898,78.233 ) );
				float lerpResult10_g4 = lerp( 0.0 , 1.0 , frac( ( sin( dotResult4_g4 ) * 43758.55 ) ));
				float hurNoise88 = ( lerpResult10_g4 * _HurlStrength );
				float smoothstepResult61 = smoothstep( _Dissolve , ( _Dissolve + _DissolveSmooth ) , -( ( temp_output_23_0 - ( _CircleRadius - _DissolveDistance ) ) + dissolveGuide53 + hurNoise88 ));
				float dissolve67 = smoothstepResult61;
				float circle15 = saturate( ( saturate( ( smoothstepResult12 + ( smoothstepResult43 * _GlowStrength ) ) ) - dissolve67 ) );
				float4 appendResult9 = (float4(tex2DNode8.rgb , ( tex2DNode8.a * circle15 )));
				
				float4 Color = ( IN.color * appendResult9 );

				#if ETC1_EXTERNAL_ALPHA
					float4 alpha = SAMPLE_TEXTURE2D(_AlphaTex, sampler_AlphaTex, IN.texCoord0.xy);
					Color.a = lerp( Color.a, alpha.r, _EnableAlphaTexture);
				#endif

				#if defined(DEBUG_DISPLAY)
				SurfaceData2D surfaceData;
				InitializeSurfaceData(Color.rgb, Color.a, surfaceData);
				InputData2D inputData;
				InitializeInputData(IN.positionWS.xy, half2(IN.texCoord0.xy), inputData);
				half4 debugColor = 0;

				SETUP_DEBUG_DATA_2D(inputData, IN.positionWS);

				if (CanDebugOverrideOutputColor(surfaceData, inputData, debugColor))
				{
					return debugColor;
				}
				#endif

				Color *= IN.color * _RendererColor;
				return Color;
			}

			ENDHLSL
		}

		
		Pass
		{
			
            Name "Sprite Unlit Forward"
            Tags { "LightMode"="UniversalForward" }

			Blend SrcAlpha OneMinusSrcAlpha, One OneMinusSrcAlpha
			ZTest LEqual
			ZWrite Off
			Offset 0 , 0
			ColorMask RGBA
			

			HLSLPROGRAM

			#define ASE_VERSION 19801
			#define ASE_SRP_VERSION 120112


			#pragma vertex vert
			#pragma fragment frag

            #define _SURFACE_TYPE_TRANSPARENT 1
            #define ATTRIBUTES_NEED_NORMAL
            #define ATTRIBUTES_NEED_TANGENT
            #define ATTRIBUTES_NEED_TEXCOORD0
            #define ATTRIBUTES_NEED_COLOR
            #define VARYINGS_NEED_POSITION_WS
            #define VARYINGS_NEED_TEXCOORD0
            #define VARYINGS_NEED_COLOR
            #define FEATURES_GRAPH_VERTEX

			#define SHADERPASS SHADERPASS_SPRITEFORWARD

			#include "Packages/com.unity.render-pipelines.core/ShaderLibrary/Color.hlsl"
			#include "Packages/com.unity.render-pipelines.core/ShaderLibrary/Texture.hlsl"
			#include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
			#include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Lighting.hlsl"
			#include "Packages/com.unity.render-pipelines.core/ShaderLibrary/TextureStack.hlsl"
			#include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/ShaderGraphFunctions.hlsl"
			#include "Packages/com.unity.render-pipelines.universal/Editor/ShaderGraph/Includes/ShaderPass.hlsl"

			#include "Packages/com.unity.render-pipelines.universal/Shaders/2D/Include/SurfaceData2D.hlsl"
			#include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Debug/Debugging2D.hlsl"

			#define ASE_NEEDS_FRAG_COLOR
			#pragma multi_compile_instancing


			sampler2D _MainTex;
			sampler2D _NoiseMap;
			sampler2D _DissolveNoise;
			UNITY_INSTANCING_BUFFER_START(AmplifyShadersSpriteExpandCircle)
				UNITY_DEFINE_INSTANCED_PROP(float4, _MainTex_ST)
				UNITY_DEFINE_INSTANCED_PROP(float4, _NoiseMap_ST)
				UNITY_DEFINE_INSTANCED_PROP(float4, _DissolveNoise_ST)
				UNITY_DEFINE_INSTANCED_PROP(float, _ExternalNoiseMovement)
			UNITY_INSTANCING_BUFFER_END(AmplifyShadersSpriteExpandCircle)
			CBUFFER_START( UnityPerMaterial )
			float4 _NoisePan;
			float _CircleSmooth;
			float _DissolveNoiseStrength;
			float _DissolveDistance;
			float _DissolveSmooth;
			float _Dissolve;
			float _GlowStrength;
			float _GlowSmooth;
			float _Glow;
			float _NoiseStrength;
			float _LengthScale;
			float _RadialScale;
			float _NoiseSmooth;
			float _NoiseMin;
			float _CircleRadius;
			float _HurScale;
			float _HurlStrength;
			CBUFFER_END


			struct VertexInput
			{
				float4 positionOS : POSITION;
				float3 normal : NORMAL;
				float4 tangent : TANGENT;
				float4 uv0 : TEXCOORD0;
				float4 color : COLOR;
				
				UNITY_VERTEX_INPUT_INSTANCE_ID
			};

			struct VertexOutput
			{
				float4 positionCS : SV_POSITION;
				float4 texCoord0 : TEXCOORD0;
				float4 color : TEXCOORD1;
				float3 positionWS : TEXCOORD2;
				float4 ase_texcoord3 : TEXCOORD3;
				UNITY_VERTEX_INPUT_INSTANCE_ID
				UNITY_VERTEX_OUTPUT_STEREO
			};

			#if ETC1_EXTERNAL_ALPHA
				TEXTURE2D( _AlphaTex ); SAMPLER( sampler_AlphaTex );
				float _EnableAlphaTexture;
			#endif

			float4 _RendererColor;

			
			VertexOutput vert( VertexInput v  )
			{
				VertexOutput o = (VertexOutput)0;
				UNITY_SETUP_INSTANCE_ID(v);
				UNITY_TRANSFER_INSTANCE_ID(v, o);
				UNITY_INITIALIZE_VERTEX_OUTPUT_STEREO(o);

				float4 ase_positionCS = TransformObjectToHClip( ( v.positionOS ).xyz );
				float4 screenPos = ComputeScreenPos( ase_positionCS );
				o.ase_texcoord3 = screenPos;
				
				#ifdef ASE_ABSOLUTE_VERTEX_POS
					float3 defaultVertexValue = v.positionOS.xyz;
				#else
					float3 defaultVertexValue = float3( 0, 0, 0 );
				#endif
				float3 vertexValue = defaultVertexValue;
				#ifdef ASE_ABSOLUTE_VERTEX_POS
					v.positionOS.xyz = vertexValue;
				#else
					v.positionOS.xyz += vertexValue;
				#endif
				v.normal = v.normal;
				v.tangent.xyz = v.tangent.xyz;

				VertexPositionInputs vertexInput = GetVertexPositionInputs(v.positionOS.xyz);

				o.positionCS = vertexInput.positionCS;
				o.positionWS = vertexInput.positionWS;
				o.texCoord0 = v.uv0;
				o.color = v.color;

				return o;
			}

			half4 frag( VertexOutput IN  ) : SV_Target
			{
				UNITY_SETUP_INSTANCE_ID(IN);
				UNITY_SETUP_STEREO_EYE_INDEX_POST_VERTEX(IN);

				float4 _MainTex_ST_Instance = UNITY_ACCESS_INSTANCED_PROP(AmplifyShadersSpriteExpandCircle,_MainTex_ST);
				float2 uv_MainTex = IN.texCoord0.xy * _MainTex_ST_Instance.xy + _MainTex_ST_Instance.zw;
				float4 tex2DNode8 = tex2D( _MainTex, uv_MainTex );
				float2 texCoord4 = IN.texCoord0.xy * float2( 1,1 ) + float2( 0,0 );
				float temp_output_23_0 = length( ( texCoord4 - float2( 0.5,0.5 ) ) );
				float _ExternalNoiseMovement_Instance = UNITY_ACCESS_INSTANCED_PROP(AmplifyShadersSpriteExpandCircle,_ExternalNoiseMovement);
				float2 appendResult100 = (float2(_ExternalNoiseMovement_Instance , 0.0));
				float2 appendResult73 = (float2(_NoisePan.x , _NoisePan.y));
				float4 _NoiseMap_ST_Instance = UNITY_ACCESS_INSTANCED_PROP(AmplifyShadersSpriteExpandCircle,_NoiseMap_ST);
				float2 uv_NoiseMap = IN.texCoord0.xy * _NoiseMap_ST_Instance.xy + _NoiseMap_ST_Instance.zw;
				float2 temp_output_34_0_g3 = ( uv_NoiseMap - float2( 0.5,0.5 ) );
				float2 break39_g3 = temp_output_34_0_g3;
				float2 appendResult50_g3 = (float2(( _RadialScale * ( length( temp_output_34_0_g3 ) * 2.0 ) ) , ( ( atan2( break39_g3.x , break39_g3.y ) * ( 1.0 / TWO_PI ) ) * _LengthScale )));
				float2 panner37 = ( 1.0 * _Time.y * appendResult73 + appendResult50_g3);
				float smoothstepResult94 = smoothstep( _NoiseMin , ( _NoiseMin + _NoiseSmooth ) , tex2D( _NoiseMap, ( appendResult100 + panner37 ) ).r);
				float temp_output_42_0 = ( -( temp_output_23_0 - _CircleRadius ) + ( smoothstepResult94 * _NoiseStrength ) );
				float smoothstepResult12 = smoothstep( 0.0 , ( 0.0 + _CircleSmooth ) , temp_output_42_0);
				float smoothstepResult43 = smoothstep( _Glow , ( _Glow + _GlowSmooth ) , temp_output_42_0);
				float2 appendResult74 = (float2(_NoisePan.z , _NoisePan.w));
				float2 dissolvePan75 = appendResult74;
				float4 _DissolveNoise_ST_Instance = UNITY_ACCESS_INSTANCED_PROP(AmplifyShadersSpriteExpandCircle,_DissolveNoise_ST);
				float2 uv_DissolveNoise = IN.texCoord0.xy * _DissolveNoise_ST_Instance.xy + _DissolveNoise_ST_Instance.zw;
				float2 panner76 = ( 1.0 * _Time.y * dissolvePan75 + uv_DissolveNoise);
				float dissolveGuide53 = ( tex2D( _DissolveNoise, panner76 ).r * _DissolveNoiseStrength );
				float4 screenPos = IN.ase_texcoord3;
				float4 ase_positionSSNorm = screenPos / screenPos.w;
				ase_positionSSNorm.z = ( UNITY_NEAR_CLIP_VALUE >= 0 ) ? ase_positionSSNorm.z : ase_positionSSNorm.z * 0.5 + 0.5;
				float2 appendResult82 = (float2(ase_positionSSNorm.x , ase_positionSSNorm.y));
				float dotResult4_g4 = dot( ( ( appendResult82 / ase_positionSSNorm.w ) * _HurScale ) , float2( 12.9898,78.233 ) );
				float lerpResult10_g4 = lerp( 0.0 , 1.0 , frac( ( sin( dotResult4_g4 ) * 43758.55 ) ));
				float hurNoise88 = ( lerpResult10_g4 * _HurlStrength );
				float smoothstepResult61 = smoothstep( _Dissolve , ( _Dissolve + _DissolveSmooth ) , -( ( temp_output_23_0 - ( _CircleRadius - _DissolveDistance ) ) + dissolveGuide53 + hurNoise88 ));
				float dissolve67 = smoothstepResult61;
				float circle15 = saturate( ( saturate( ( smoothstepResult12 + ( smoothstepResult43 * _GlowStrength ) ) ) - dissolve67 ) );
				float4 appendResult9 = (float4(tex2DNode8.rgb , ( tex2DNode8.a * circle15 )));
				
				float4 Color = ( IN.color * appendResult9 );

				#if ETC1_EXTERNAL_ALPHA
					float4 alpha = SAMPLE_TEXTURE2D( _AlphaTex, sampler_AlphaTex, IN.texCoord0.xy );
					Color.a = lerp( Color.a, alpha.r, _EnableAlphaTexture );
				#endif


				#if defined(DEBUG_DISPLAY)
					SurfaceData2D surfaceData;
					InitializeSurfaceData(Color.rgb, Color.a, surfaceData);
					InputData2D inputData;
					InitializeInputData(IN.positionWS.xy, half2(IN.texCoord0.xy), inputData);
					half4 debugColor = 0;

					SETUP_DEBUG_DATA_2D(inputData, IN.positionWS);

					if (CanDebugOverrideOutputColor(surfaceData, inputData, debugColor))
					{
						return debugColor;
					}
				#endif

				Color *= IN.color * _RendererColor;
				return Color;
			}

			ENDHLSL
		}
		
        Pass
        {
			
            Name "SceneSelectionPass"
            Tags { "LightMode"="SceneSelectionPass" }

            Cull Off

            HLSLPROGRAM

			#define ASE_VERSION 19801
			#define ASE_SRP_VERSION 120112


			#pragma vertex vert
			#pragma fragment frag

            #define _SURFACE_TYPE_TRANSPARENT 1
            #define ATTRIBUTES_NEED_NORMAL
            #define ATTRIBUTES_NEED_TANGENT
            #define FEATURES_GRAPH_VERTEX

            #define SHADERPASS SHADERPASS_DEPTHONLY
			#define SCENESELECTIONPASS 1

			#include "Packages/com.unity.render-pipelines.core/ShaderLibrary/Color.hlsl"
			#include "Packages/com.unity.render-pipelines.core/ShaderLibrary/Texture.hlsl"
			#include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
			#include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Lighting.hlsl"
			#include "Packages/com.unity.render-pipelines.core/ShaderLibrary/TextureStack.hlsl"
			#include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/ShaderGraphFunctions.hlsl"
			#include "Packages/com.unity.render-pipelines.universal/Editor/ShaderGraph/Includes/ShaderPass.hlsl"

			#pragma multi_compile_instancing


			sampler2D _MainTex;
			sampler2D _NoiseMap;
			sampler2D _DissolveNoise;
			UNITY_INSTANCING_BUFFER_START(AmplifyShadersSpriteExpandCircle)
				UNITY_DEFINE_INSTANCED_PROP(float4, _MainTex_ST)
				UNITY_DEFINE_INSTANCED_PROP(float4, _NoiseMap_ST)
				UNITY_DEFINE_INSTANCED_PROP(float4, _DissolveNoise_ST)
				UNITY_DEFINE_INSTANCED_PROP(float, _ExternalNoiseMovement)
			UNITY_INSTANCING_BUFFER_END(AmplifyShadersSpriteExpandCircle)
			CBUFFER_START( UnityPerMaterial )
			float4 _NoisePan;
			float _CircleSmooth;
			float _DissolveNoiseStrength;
			float _DissolveDistance;
			float _DissolveSmooth;
			float _Dissolve;
			float _GlowStrength;
			float _GlowSmooth;
			float _Glow;
			float _NoiseStrength;
			float _LengthScale;
			float _RadialScale;
			float _NoiseSmooth;
			float _NoiseMin;
			float _CircleRadius;
			float _HurScale;
			float _HurlStrength;
			CBUFFER_END


            struct VertexInput
			{
				float3 positionOS : POSITION;
				float3 normal : NORMAL;
				float4 tangent : TANGENT;
				float4 ase_color : COLOR;
				float4 ase_texcoord : TEXCOORD0;
				UNITY_VERTEX_INPUT_INSTANCE_ID
			};

			struct VertexOutput
			{
				float4 positionCS : SV_POSITION;
				float4 ase_color : COLOR;
				float4 ase_texcoord : TEXCOORD0;
				float4 ase_texcoord1 : TEXCOORD1;
				UNITY_VERTEX_INPUT_INSTANCE_ID
			};

            int _ObjectId;
            int _PassValue;

			
			VertexOutput vert(VertexInput v )
			{
				VertexOutput o = (VertexOutput)0;

				UNITY_SETUP_INSTANCE_ID(v);
				UNITY_TRANSFER_INSTANCE_ID(v, o);
				UNITY_INITIALIZE_VERTEX_OUTPUT_STEREO(o);

				float4 ase_positionCS = TransformObjectToHClip( ( v.positionOS ).xyz );
				float4 screenPos = ComputeScreenPos( ase_positionCS );
				o.ase_texcoord1 = screenPos;
				
				o.ase_color = v.ase_color;
				o.ase_texcoord.xy = v.ase_texcoord.xy;
				
				//setting value to unused interpolator channels and avoid initialization warnings
				o.ase_texcoord.zw = 0;
				#ifdef ASE_ABSOLUTE_VERTEX_POS
					float3 defaultVertexValue = v.positionOS.xyz;
				#else
					float3 defaultVertexValue = float3(0, 0, 0);
				#endif
				float3 vertexValue = defaultVertexValue;
				#ifdef ASE_ABSOLUTE_VERTEX_POS
					v.positionOS.xyz = vertexValue;
				#else
					v.positionOS.xyz += vertexValue;
				#endif

				VertexPositionInputs vertexInput = GetVertexPositionInputs(v.positionOS.xyz);
				float3 positionWS = TransformObjectToWorld(v.positionOS);
				o.positionCS = TransformWorldToHClip(positionWS);

				return o;
			}

			half4 frag(VertexOutput IN) : SV_TARGET
			{
				float4 _MainTex_ST_Instance = UNITY_ACCESS_INSTANCED_PROP(AmplifyShadersSpriteExpandCircle,_MainTex_ST);
				float2 uv_MainTex = IN.ase_texcoord.xy * _MainTex_ST_Instance.xy + _MainTex_ST_Instance.zw;
				float4 tex2DNode8 = tex2D( _MainTex, uv_MainTex );
				float2 texCoord4 = IN.ase_texcoord.xy * float2( 1,1 ) + float2( 0,0 );
				float temp_output_23_0 = length( ( texCoord4 - float2( 0.5,0.5 ) ) );
				float _ExternalNoiseMovement_Instance = UNITY_ACCESS_INSTANCED_PROP(AmplifyShadersSpriteExpandCircle,_ExternalNoiseMovement);
				float2 appendResult100 = (float2(_ExternalNoiseMovement_Instance , 0.0));
				float2 appendResult73 = (float2(_NoisePan.x , _NoisePan.y));
				float4 _NoiseMap_ST_Instance = UNITY_ACCESS_INSTANCED_PROP(AmplifyShadersSpriteExpandCircle,_NoiseMap_ST);
				float2 uv_NoiseMap = IN.ase_texcoord.xy * _NoiseMap_ST_Instance.xy + _NoiseMap_ST_Instance.zw;
				float2 temp_output_34_0_g3 = ( uv_NoiseMap - float2( 0.5,0.5 ) );
				float2 break39_g3 = temp_output_34_0_g3;
				float2 appendResult50_g3 = (float2(( _RadialScale * ( length( temp_output_34_0_g3 ) * 2.0 ) ) , ( ( atan2( break39_g3.x , break39_g3.y ) * ( 1.0 / TWO_PI ) ) * _LengthScale )));
				float2 panner37 = ( 1.0 * _Time.y * appendResult73 + appendResult50_g3);
				float smoothstepResult94 = smoothstep( _NoiseMin , ( _NoiseMin + _NoiseSmooth ) , tex2D( _NoiseMap, ( appendResult100 + panner37 ) ).r);
				float temp_output_42_0 = ( -( temp_output_23_0 - _CircleRadius ) + ( smoothstepResult94 * _NoiseStrength ) );
				float smoothstepResult12 = smoothstep( 0.0 , ( 0.0 + _CircleSmooth ) , temp_output_42_0);
				float smoothstepResult43 = smoothstep( _Glow , ( _Glow + _GlowSmooth ) , temp_output_42_0);
				float2 appendResult74 = (float2(_NoisePan.z , _NoisePan.w));
				float2 dissolvePan75 = appendResult74;
				float4 _DissolveNoise_ST_Instance = UNITY_ACCESS_INSTANCED_PROP(AmplifyShadersSpriteExpandCircle,_DissolveNoise_ST);
				float2 uv_DissolveNoise = IN.ase_texcoord.xy * _DissolveNoise_ST_Instance.xy + _DissolveNoise_ST_Instance.zw;
				float2 panner76 = ( 1.0 * _Time.y * dissolvePan75 + uv_DissolveNoise);
				float dissolveGuide53 = ( tex2D( _DissolveNoise, panner76 ).r * _DissolveNoiseStrength );
				float4 screenPos = IN.ase_texcoord1;
				float4 ase_positionSSNorm = screenPos / screenPos.w;
				ase_positionSSNorm.z = ( UNITY_NEAR_CLIP_VALUE >= 0 ) ? ase_positionSSNorm.z : ase_positionSSNorm.z * 0.5 + 0.5;
				float2 appendResult82 = (float2(ase_positionSSNorm.x , ase_positionSSNorm.y));
				float dotResult4_g4 = dot( ( ( appendResult82 / ase_positionSSNorm.w ) * _HurScale ) , float2( 12.9898,78.233 ) );
				float lerpResult10_g4 = lerp( 0.0 , 1.0 , frac( ( sin( dotResult4_g4 ) * 43758.55 ) ));
				float hurNoise88 = ( lerpResult10_g4 * _HurlStrength );
				float smoothstepResult61 = smoothstep( _Dissolve , ( _Dissolve + _DissolveSmooth ) , -( ( temp_output_23_0 - ( _CircleRadius - _DissolveDistance ) ) + dissolveGuide53 + hurNoise88 ));
				float dissolve67 = smoothstepResult61;
				float circle15 = saturate( ( saturate( ( smoothstepResult12 + ( smoothstepResult43 * _GlowStrength ) ) ) - dissolve67 ) );
				float4 appendResult9 = (float4(tex2DNode8.rgb , ( tex2DNode8.a * circle15 )));
				
				float4 Color = ( IN.ase_color * appendResult9 );

				half4 outColor = half4(_ObjectId, _PassValue, 1.0, 1.0);
				return outColor;
			}

            ENDHLSL
        }

		
        Pass
        {
			
            Name "ScenePickingPass"
            Tags { "LightMode"="Picking" }

			Cull Off

            HLSLPROGRAM

			#define ASE_VERSION 19801
			#define ASE_SRP_VERSION 120112


			#pragma vertex vert
			#pragma fragment frag

            #define _SURFACE_TYPE_TRANSPARENT 1
            #define ATTRIBUTES_NEED_NORMAL
            #define ATTRIBUTES_NEED_TANGENT
            #define FEATURES_GRAPH_VERTEX

            #define SHADERPASS SHADERPASS_DEPTHONLY
			#define SCENEPICKINGPASS 1

			#include "Packages/com.unity.render-pipelines.core/ShaderLibrary/Color.hlsl"
			#include "Packages/com.unity.render-pipelines.core/ShaderLibrary/Texture.hlsl"
			#include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
			#include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Lighting.hlsl"
			#include "Packages/com.unity.render-pipelines.core/ShaderLibrary/TextureStack.hlsl"
			#include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/ShaderGraphFunctions.hlsl"
			#include "Packages/com.unity.render-pipelines.universal/Editor/ShaderGraph/Includes/ShaderPass.hlsl"

        	#pragma multi_compile_instancing


			sampler2D _MainTex;
			sampler2D _NoiseMap;
			sampler2D _DissolveNoise;
			UNITY_INSTANCING_BUFFER_START(AmplifyShadersSpriteExpandCircle)
				UNITY_DEFINE_INSTANCED_PROP(float4, _MainTex_ST)
				UNITY_DEFINE_INSTANCED_PROP(float4, _NoiseMap_ST)
				UNITY_DEFINE_INSTANCED_PROP(float4, _DissolveNoise_ST)
				UNITY_DEFINE_INSTANCED_PROP(float, _ExternalNoiseMovement)
			UNITY_INSTANCING_BUFFER_END(AmplifyShadersSpriteExpandCircle)
			CBUFFER_START( UnityPerMaterial )
			float4 _NoisePan;
			float _CircleSmooth;
			float _DissolveNoiseStrength;
			float _DissolveDistance;
			float _DissolveSmooth;
			float _Dissolve;
			float _GlowStrength;
			float _GlowSmooth;
			float _Glow;
			float _NoiseStrength;
			float _LengthScale;
			float _RadialScale;
			float _NoiseSmooth;
			float _NoiseMin;
			float _CircleRadius;
			float _HurScale;
			float _HurlStrength;
			CBUFFER_END


            struct VertexInput
			{
				float3 positionOS : POSITION;
				float3 normal : NORMAL;
				float4 tangent : TANGENT;
				float4 ase_color : COLOR;
				float4 ase_texcoord : TEXCOORD0;
				UNITY_VERTEX_INPUT_INSTANCE_ID
			};

			struct VertexOutput
			{
				float4 positionCS : SV_POSITION;
				float4 ase_color : COLOR;
				float4 ase_texcoord : TEXCOORD0;
				float4 ase_texcoord1 : TEXCOORD1;
				UNITY_VERTEX_INPUT_INSTANCE_ID
			};

            float4 _SelectionID;

			
			VertexOutput vert(VertexInput v  )
			{
				VertexOutput o = (VertexOutput)0;

				UNITY_SETUP_INSTANCE_ID(v);
				UNITY_TRANSFER_INSTANCE_ID(v, o);
				UNITY_INITIALIZE_VERTEX_OUTPUT_STEREO(o);

				float4 ase_positionCS = TransformObjectToHClip( ( v.positionOS ).xyz );
				float4 screenPos = ComputeScreenPos( ase_positionCS );
				o.ase_texcoord1 = screenPos;
				
				o.ase_color = v.ase_color;
				o.ase_texcoord.xy = v.ase_texcoord.xy;
				
				//setting value to unused interpolator channels and avoid initialization warnings
				o.ase_texcoord.zw = 0;
				#ifdef ASE_ABSOLUTE_VERTEX_POS
					float3 defaultVertexValue = v.positionOS.xyz;
				#else
					float3 defaultVertexValue = float3(0, 0, 0);
				#endif
				float3 vertexValue = defaultVertexValue;
				#ifdef ASE_ABSOLUTE_VERTEX_POS
					v.positionOS.xyz = vertexValue;
				#else
					v.positionOS.xyz += vertexValue;
				#endif

				VertexPositionInputs vertexInput = GetVertexPositionInputs(v.positionOS.xyz);
				float3 positionWS = TransformObjectToWorld(v.positionOS);
				o.positionCS = TransformWorldToHClip(positionWS);

				return o;
			}

			half4 frag(VertexOutput IN ) : SV_TARGET
			{
				float4 _MainTex_ST_Instance = UNITY_ACCESS_INSTANCED_PROP(AmplifyShadersSpriteExpandCircle,_MainTex_ST);
				float2 uv_MainTex = IN.ase_texcoord.xy * _MainTex_ST_Instance.xy + _MainTex_ST_Instance.zw;
				float4 tex2DNode8 = tex2D( _MainTex, uv_MainTex );
				float2 texCoord4 = IN.ase_texcoord.xy * float2( 1,1 ) + float2( 0,0 );
				float temp_output_23_0 = length( ( texCoord4 - float2( 0.5,0.5 ) ) );
				float _ExternalNoiseMovement_Instance = UNITY_ACCESS_INSTANCED_PROP(AmplifyShadersSpriteExpandCircle,_ExternalNoiseMovement);
				float2 appendResult100 = (float2(_ExternalNoiseMovement_Instance , 0.0));
				float2 appendResult73 = (float2(_NoisePan.x , _NoisePan.y));
				float4 _NoiseMap_ST_Instance = UNITY_ACCESS_INSTANCED_PROP(AmplifyShadersSpriteExpandCircle,_NoiseMap_ST);
				float2 uv_NoiseMap = IN.ase_texcoord.xy * _NoiseMap_ST_Instance.xy + _NoiseMap_ST_Instance.zw;
				float2 temp_output_34_0_g3 = ( uv_NoiseMap - float2( 0.5,0.5 ) );
				float2 break39_g3 = temp_output_34_0_g3;
				float2 appendResult50_g3 = (float2(( _RadialScale * ( length( temp_output_34_0_g3 ) * 2.0 ) ) , ( ( atan2( break39_g3.x , break39_g3.y ) * ( 1.0 / TWO_PI ) ) * _LengthScale )));
				float2 panner37 = ( 1.0 * _Time.y * appendResult73 + appendResult50_g3);
				float smoothstepResult94 = smoothstep( _NoiseMin , ( _NoiseMin + _NoiseSmooth ) , tex2D( _NoiseMap, ( appendResult100 + panner37 ) ).r);
				float temp_output_42_0 = ( -( temp_output_23_0 - _CircleRadius ) + ( smoothstepResult94 * _NoiseStrength ) );
				float smoothstepResult12 = smoothstep( 0.0 , ( 0.0 + _CircleSmooth ) , temp_output_42_0);
				float smoothstepResult43 = smoothstep( _Glow , ( _Glow + _GlowSmooth ) , temp_output_42_0);
				float2 appendResult74 = (float2(_NoisePan.z , _NoisePan.w));
				float2 dissolvePan75 = appendResult74;
				float4 _DissolveNoise_ST_Instance = UNITY_ACCESS_INSTANCED_PROP(AmplifyShadersSpriteExpandCircle,_DissolveNoise_ST);
				float2 uv_DissolveNoise = IN.ase_texcoord.xy * _DissolveNoise_ST_Instance.xy + _DissolveNoise_ST_Instance.zw;
				float2 panner76 = ( 1.0 * _Time.y * dissolvePan75 + uv_DissolveNoise);
				float dissolveGuide53 = ( tex2D( _DissolveNoise, panner76 ).r * _DissolveNoiseStrength );
				float4 screenPos = IN.ase_texcoord1;
				float4 ase_positionSSNorm = screenPos / screenPos.w;
				ase_positionSSNorm.z = ( UNITY_NEAR_CLIP_VALUE >= 0 ) ? ase_positionSSNorm.z : ase_positionSSNorm.z * 0.5 + 0.5;
				float2 appendResult82 = (float2(ase_positionSSNorm.x , ase_positionSSNorm.y));
				float dotResult4_g4 = dot( ( ( appendResult82 / ase_positionSSNorm.w ) * _HurScale ) , float2( 12.9898,78.233 ) );
				float lerpResult10_g4 = lerp( 0.0 , 1.0 , frac( ( sin( dotResult4_g4 ) * 43758.55 ) ));
				float hurNoise88 = ( lerpResult10_g4 * _HurlStrength );
				float smoothstepResult61 = smoothstep( _Dissolve , ( _Dissolve + _DissolveSmooth ) , -( ( temp_output_23_0 - ( _CircleRadius - _DissolveDistance ) ) + dissolveGuide53 + hurNoise88 ));
				float dissolve67 = smoothstepResult61;
				float circle15 = saturate( ( saturate( ( smoothstepResult12 + ( smoothstepResult43 * _GlowStrength ) ) ) - dissolve67 ) );
				float4 appendResult9 = (float4(tex2DNode8.rgb , ( tex2DNode8.a * circle15 )));
				
				float4 Color = ( IN.ase_color * appendResult9 );
				half4 outColor = _SelectionID;
				return outColor;
			}

            ENDHLSL
        }
		
	}
	CustomEditor "UnityEditor.ShaderGraph.GenericShaderGraphMaterialGUI"
	FallBack "Hidden/Shader Graph/FallbackError"
	
	Fallback Off
}
/*ASEBEGIN
Version=19801
Node;AmplifyShaderEditor.Vector4Node;72;-1632,1040;Inherit;False;Property;_NoisePan;NoisePan;10;0;Create;True;0;0;0;False;0;False;0,0,0,0;0,0,0,0;0;5;FLOAT4;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.ScreenPosInputsNode;81;-1696,1920;Float;False;0;False;0;5;FLOAT4;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.DynamicAppendNode;74;-1440,1152;Inherit;False;FLOAT2;4;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;0;False;3;FLOAT;0;False;1;FLOAT2;0
Node;AmplifyShaderEditor.RangedFloatNode;35;-1552,816;Inherit;False;Property;_RadialScale;RadialScale;12;0;Create;True;0;0;0;False;0;False;1;1;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;36;-1552,896;Inherit;False;Property;_LengthScale;LengthScale;13;0;Create;True;0;0;0;False;0;False;1;1;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.TextureCoordinatesNode;27;-1584,688;Inherit;False;0;26;2;3;2;SAMPLER2D;;False;0;FLOAT2;1,1;False;1;FLOAT2;0,0;False;5;FLOAT2;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.DynamicAppendNode;82;-1488,1920;Inherit;False;FLOAT2;4;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;0;False;3;FLOAT;0;False;1;FLOAT2;0
Node;AmplifyShaderEditor.DynamicAppendNode;73;-1440,1040;Inherit;False;FLOAT2;4;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;0;False;3;FLOAT;0;False;1;FLOAT2;0
Node;AmplifyShaderEditor.RegisterLocalVarNode;75;-1296,1152;Inherit;False;dissolvePan;-1;True;1;0;FLOAT2;0,0;False;1;FLOAT2;0
Node;AmplifyShaderEditor.FunctionNode;30;-1344,768;Inherit;False;Polar Coordinates;-1;;3;7dab8e02884cf104ebefaa2e788e4162;0;4;1;FLOAT2;0,0;False;2;FLOAT2;0.5,0.5;False;3;FLOAT;1;False;4;FLOAT;1;False;3;FLOAT2;0;FLOAT;55;FLOAT;56
Node;AmplifyShaderEditor.RangedFloatNode;99;-1168,688;Inherit;False;InstancedProperty;_ExternalNoiseMovement;ExternalNoiseMovement;11;0;Create;True;0;0;0;False;0;False;0;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.TextureCoordinatesNode;51;-1600,1504;Inherit;False;0;52;2;3;2;SAMPLER2D;;False;0;FLOAT2;1,1;False;1;FLOAT2;0,0;False;5;FLOAT2;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.GetLocalVarNode;77;-1568,1632;Inherit;False;75;dissolvePan;1;0;OBJECT;;False;1;FLOAT2;0
Node;AmplifyShaderEditor.TextureCoordinatesNode;4;-1280,384;Inherit;False;0;-1;2;3;2;SAMPLER2D;;False;0;FLOAT2;1,1;False;1;FLOAT2;0,0;False;5;FLOAT2;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.Vector2Node;7;-1248,512;Inherit;False;Constant;_Vector0;Vector 0;0;0;Create;True;0;0;0;False;0;False;0.5,0.5;0,0;0;3;FLOAT2;0;FLOAT;1;FLOAT;2
Node;AmplifyShaderEditor.SimpleDivideOpNode;83;-1328,1984;Inherit;False;2;0;FLOAT2;0,0;False;1;FLOAT;0;False;1;FLOAT2;0
Node;AmplifyShaderEditor.RangedFloatNode;87;-1328,2128;Inherit;False;Property;_HurScale;HurScale;19;0;Create;True;0;0;0;False;0;False;1;1;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.PannerNode;37;-1008,848;Inherit;False;3;0;FLOAT2;0,0;False;2;FLOAT2;0,0;False;1;FLOAT;1;False;1;FLOAT2;0
Node;AmplifyShaderEditor.DynamicAppendNode;100;-928,688;Inherit;False;FLOAT2;4;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;0;False;3;FLOAT;0;False;1;FLOAT2;0
Node;AmplifyShaderEditor.SimpleSubtractOpNode;22;-1024,448;Inherit;False;2;0;FLOAT2;0,0;False;1;FLOAT2;0,0;False;1;FLOAT2;0
Node;AmplifyShaderEditor.PannerNode;76;-1360,1504;Inherit;False;3;0;FLOAT2;0,0;False;2;FLOAT2;0,0;False;1;FLOAT;1;False;1;FLOAT2;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;85;-1168,2080;Inherit;False;2;2;0;FLOAT2;0,0;False;1;FLOAT;0;False;1;FLOAT2;0
Node;AmplifyShaderEditor.RangedFloatNode;95;-688,1040;Inherit;False;Property;_NoiseMin;NoiseMin;7;0;Create;True;0;0;0;False;0;False;0;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;97;-720,1120;Inherit;False;Property;_NoiseSmooth;NoiseSmooth;8;0;Create;True;0;0;0;False;0;False;1;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleAddOpNode;101;-784,688;Inherit;False;2;2;0;FLOAT2;0,0;False;1;FLOAT2;0,0;False;1;FLOAT2;0
Node;AmplifyShaderEditor.RangedFloatNode;17;-880,544;Inherit;False;Property;_CircleRadius;CircleRadius;1;0;Create;True;0;0;0;False;0;False;0.3;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.LengthOpNode;23;-848,448;Inherit;False;1;0;FLOAT2;0,0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SamplerNode;52;-1168,1504;Inherit;True;Property;_DissolveNoise;DissolveNoise;15;0;Create;True;0;0;0;False;0;False;-1;184854303c9362c43b86499202498085;184854303c9362c43b86499202498085;True;0;False;white;Auto;False;Object;-1;Auto;Texture2D;8;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;6;FLOAT;0;False;7;SAMPLERSTATE;;False;6;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4;FLOAT3;5
Node;AmplifyShaderEditor.RangedFloatNode;60;-1136,1760;Inherit;False;Property;_DissolveNoiseStrength;DissolveNoiseStrength;16;0;Create;True;0;0;0;False;0;False;0.1;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;93;-960,2304;Inherit;False;Property;_HurlStrength;HurlStrength;20;0;Create;True;0;0;0;False;0;False;0.5;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.FunctionNode;89;-1008,2080;Inherit;True;Random Range;-1;;4;7b754edb8aebbfb4a9ace907af661cfc;0;3;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT;1;False;1;FLOAT;0
Node;AmplifyShaderEditor.SamplerNode;26;-704,848;Inherit;True;Property;_NoiseMap;NoiseMap;6;0;Create;True;0;0;0;False;0;False;-1;1c9c12e29dba65f489a1e60d73635451;a2f128330bdd0984aabbc6d45c920f6e;True;0;False;white;Auto;False;Object;-1;Auto;Texture2D;8;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;6;FLOAT;0;False;7;SAMPLERSTATE;;False;6;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4;FLOAT3;5
Node;AmplifyShaderEditor.SimpleAddOpNode;96;-528,1040;Inherit;False;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;55;-832,224;Inherit;False;Property;_DissolveDistance;DissolveDistance;14;0;Create;True;0;0;0;False;0;False;0;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleSubtractOpNode;16;-656,448;Inherit;True;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;59;-864,1664;Inherit;False;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;92;-722.4958,2230.331;Inherit;False;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;34;-464,1168;Inherit;False;Property;_NoiseStrength;NoiseStrength;9;0;Create;True;0;0;0;False;0;False;0.2;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.SmoothstepOpNode;94;-400,896;Inherit;False;3;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;1;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleSubtractOpNode;78;-608,192;Inherit;False;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.NegateNode;18;-400,448;Inherit;False;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.RegisterLocalVarNode;53;-720,1648;Inherit;False;dissolveGuide;-1;True;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.RegisterLocalVarNode;88;-528,2080;Inherit;False;hurNoise;-1;True;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;33;-272,1072;Inherit;False;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleSubtractOpNode;54;-464,192;Inherit;False;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleAddOpNode;42;-80,448;Inherit;False;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;46;-16,832;Inherit;False;Property;_Glow;Glow;3;0;Create;True;0;0;0;False;0;False;0;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;44;-48,912;Inherit;False;Property;_GlowSmooth;GlowSmooth;4;0;Create;True;0;0;0;False;0;False;1;1;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.GetLocalVarNode;57;-528,288;Inherit;False;53;dissolveGuide;1;0;OBJECT;;False;1;FLOAT;0
Node;AmplifyShaderEditor.GetLocalVarNode;91;-496,368;Inherit;False;88;hurNoise;1;0;OBJECT;;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleAddOpNode;66;-288,192;Inherit;True;3;3;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;14;208,608;Inherit;False;Property;_CircleSmooth;CircleSmooth;2;0;Create;True;0;0;0;False;0;False;0.05;1;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;13;240,528;Inherit;False;Constant;_Circle;Circle;2;0;Create;True;0;0;0;False;0;False;0;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleAddOpNode;45;144,880;Inherit;False;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.WireNode;71;-128,768;Inherit;False;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;64;-16,352;Inherit;False;Property;_DissolveSmooth;DissolveSmooth;18;0;Create;True;0;0;0;False;0;False;0.01;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;63;16,272;Inherit;False;Property;_Dissolve;Dissolve;17;0;Create;True;0;0;0;False;0;False;0;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleAddOpNode;19;400,576;Inherit;False;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SmoothstepOpNode;43;368,816;Inherit;False;3;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;1;False;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;50;272,944;Inherit;False;Property;_GlowStrength;GlowStrength;5;0;Create;True;0;0;0;False;0;False;1;1;0;1;0;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleAddOpNode;65;208,320;Inherit;False;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.NegateNode;62;16,192;Inherit;False;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SmoothstepOpNode;12;528,448;Inherit;False;3;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;1;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;49;560,816;Inherit;False;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SmoothstepOpNode;61;368,192;Inherit;True;3;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;1;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleAddOpNode;47;768,624;Inherit;False;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.RegisterLocalVarNode;67;592,192;Inherit;False;dissolve;-1;True;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.GetLocalVarNode;68;832,752;Inherit;False;67;dissolve;1;0;OBJECT;;False;1;FLOAT;0
Node;AmplifyShaderEditor.SaturateNode;80;880,624;Inherit;False;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleSubtractOpNode;79;1040,624;Inherit;True;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SaturateNode;48;1248,624;Inherit;False;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.RegisterLocalVarNode;15;1408,624;Inherit;False;circle;-1;True;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.GetLocalVarNode;20;-784,48;Inherit;False;15;circle;1;0;OBJECT;;False;1;FLOAT;0
Node;AmplifyShaderEditor.SamplerNode;8;-896,-160;Inherit;True;Property;_MainTex;MainTex;0;0;Create;True;0;0;0;False;0;False;-1;None;None;True;0;False;white;Auto;False;Object;-1;Auto;Texture2D;8;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;6;FLOAT;0;False;7;SAMPLERSTATE;;False;6;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4;FLOAT3;5
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;21;-608,0;Inherit;False;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.DynamicAppendNode;9;-464,-144;Inherit;False;FLOAT4;4;0;FLOAT3;0,0,0;False;1;FLOAT;0;False;2;FLOAT;0;False;3;FLOAT;0;False;1;FLOAT4;0
Node;AmplifyShaderEditor.VertexColorNode;102;-496,-336;Inherit;False;0;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;103;-304,-240;Inherit;False;2;2;0;COLOR;0,0,0,0;False;1;FLOAT4;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.TemplateMultiPassMasterNode;1;0,0;Float;False;False;-1;2;UnityEditor.ShaderGraph.GenericShaderGraphMaterialGUI;0;1;New Amplify Shader;cf964e524c8e69742b1d21fbe2ebcc4a;True;Sprite Unlit Forward;0;1;Sprite Unlit Forward;0;False;False;False;False;False;False;False;False;False;False;False;False;False;False;True;2;False;;False;False;False;False;False;False;False;False;False;False;False;False;False;False;True;3;RenderPipeline=UniversalPipeline;RenderType=Transparent=RenderType;Queue=Transparent=Queue=0;True;0;True;12;all;0;False;True;2;5;False;;10;False;;3;1;False;;10;False;;False;False;False;False;False;False;False;False;False;False;False;False;False;False;True;True;True;True;True;0;False;;False;False;False;False;False;False;False;True;False;0;False;;255;False;;255;False;;0;False;;0;False;;0;False;;0;False;;0;False;;0;False;;0;False;;0;False;;False;True;2;False;;True;3;False;;True;True;0;False;;0;False;;True;1;LightMode=UniversalForward;False;False;0;;0;0;Standard;0;False;0
Node;AmplifyShaderEditor.TemplateMultiPassMasterNode;2;0,0;Float;False;False;-1;2;UnityEditor.ShaderGraph.GenericShaderGraphMaterialGUI;0;1;New Amplify Shader;cf964e524c8e69742b1d21fbe2ebcc4a;True;SceneSelectionPass;0;2;SceneSelectionPass;0;False;False;False;False;False;False;False;False;False;False;False;False;False;False;True;2;False;;False;False;False;False;False;False;False;False;False;False;False;False;False;False;True;3;RenderPipeline=UniversalPipeline;RenderType=Transparent=RenderType;Queue=Transparent=Queue=0;True;0;True;12;all;0;False;False;False;False;False;False;False;False;False;False;False;False;False;False;True;2;False;;False;False;False;False;False;False;False;False;False;False;False;False;False;False;True;1;LightMode=SceneSelectionPass;False;False;0;;0;0;Standard;0;False;0
Node;AmplifyShaderEditor.TemplateMultiPassMasterNode;3;0,0;Float;False;False;-1;2;UnityEditor.ShaderGraph.GenericShaderGraphMaterialGUI;0;1;New Amplify Shader;cf964e524c8e69742b1d21fbe2ebcc4a;True;ScenePickingPass;0;3;ScenePickingPass;0;False;False;False;False;False;False;False;False;False;False;False;False;False;False;True;2;False;;False;False;False;False;False;False;False;False;False;False;False;False;False;False;True;3;RenderPipeline=UniversalPipeline;RenderType=Transparent=RenderType;Queue=Transparent=Queue=0;True;0;True;12;all;0;False;False;False;False;False;False;False;False;False;False;False;False;False;False;True;2;False;;False;False;False;False;False;False;False;False;False;False;False;False;False;False;True;1;LightMode=Picking;False;False;0;;0;0;Standard;0;False;0
Node;AmplifyShaderEditor.TemplateMultiPassMasterNode;0;-144,-144;Float;False;True;-1;2;UnityEditor.ShaderGraph.GenericShaderGraphMaterialGUI;0;15;AmplifyShaders/Sprite ExpandCircle;cf964e524c8e69742b1d21fbe2ebcc4a;True;Sprite Unlit;0;0;Sprite Unlit;4;False;False;False;False;False;False;False;False;False;False;False;False;False;False;True;2;False;;False;False;False;False;False;False;False;False;False;False;False;False;False;False;True;3;RenderPipeline=UniversalPipeline;RenderType=Transparent=RenderType;Queue=Transparent=Queue=0;True;0;True;12;all;0;False;True;2;5;False;;10;False;;3;1;False;;10;False;;False;False;False;False;False;False;False;False;False;False;False;False;False;False;True;True;True;True;True;0;False;;False;False;False;False;False;False;False;True;False;0;False;;255;False;;255;False;;0;False;;0;False;;0;False;;0;False;;0;False;;0;False;;0;False;;0;False;;False;True;2;False;;True;3;False;;True;True;0;False;;0;False;;True;1;LightMode=Universal2D;False;False;0;;0;0;Standard;3;Vertex Position;1;0;Debug Display;0;0;External Alpha;0;0;0;4;True;True;True;True;False;;False;0
WireConnection;74;0;72;3
WireConnection;74;1;72;4
WireConnection;82;0;81;1
WireConnection;82;1;81;2
WireConnection;73;0;72;1
WireConnection;73;1;72;2
WireConnection;75;0;74;0
WireConnection;30;1;27;0
WireConnection;30;3;35;0
WireConnection;30;4;36;0
WireConnection;83;0;82;0
WireConnection;83;1;81;4
WireConnection;37;0;30;0
WireConnection;37;2;73;0
WireConnection;100;0;99;0
WireConnection;22;0;4;0
WireConnection;22;1;7;0
WireConnection;76;0;51;0
WireConnection;76;2;77;0
WireConnection;85;0;83;0
WireConnection;85;1;87;0
WireConnection;101;0;100;0
WireConnection;101;1;37;0
WireConnection;23;0;22;0
WireConnection;52;1;76;0
WireConnection;89;1;85;0
WireConnection;26;1;101;0
WireConnection;96;0;95;0
WireConnection;96;1;97;0
WireConnection;16;0;23;0
WireConnection;16;1;17;0
WireConnection;59;0;52;1
WireConnection;59;1;60;0
WireConnection;92;0;89;0
WireConnection;92;1;93;0
WireConnection;94;0;26;1
WireConnection;94;1;95;0
WireConnection;94;2;96;0
WireConnection;78;0;17;0
WireConnection;78;1;55;0
WireConnection;18;0;16;0
WireConnection;53;0;59;0
WireConnection;88;0;92;0
WireConnection;33;0;94;0
WireConnection;33;1;34;0
WireConnection;54;0;23;0
WireConnection;54;1;78;0
WireConnection;42;0;18;0
WireConnection;42;1;33;0
WireConnection;66;0;54;0
WireConnection;66;1;57;0
WireConnection;66;2;91;0
WireConnection;45;0;46;0
WireConnection;45;1;44;0
WireConnection;71;0;42;0
WireConnection;19;0;13;0
WireConnection;19;1;14;0
WireConnection;43;0;71;0
WireConnection;43;1;46;0
WireConnection;43;2;45;0
WireConnection;65;0;63;0
WireConnection;65;1;64;0
WireConnection;62;0;66;0
WireConnection;12;0;42;0
WireConnection;12;1;13;0
WireConnection;12;2;19;0
WireConnection;49;0;43;0
WireConnection;49;1;50;0
WireConnection;61;0;62;0
WireConnection;61;1;63;0
WireConnection;61;2;65;0
WireConnection;47;0;12;0
WireConnection;47;1;49;0
WireConnection;67;0;61;0
WireConnection;80;0;47;0
WireConnection;79;0;80;0
WireConnection;79;1;68;0
WireConnection;48;0;79;0
WireConnection;15;0;48;0
WireConnection;21;0;8;4
WireConnection;21;1;20;0
WireConnection;9;0;8;5
WireConnection;9;3;21;0
WireConnection;103;0;102;0
WireConnection;103;1;9;0
WireConnection;0;1;103;0
ASEEND*/
//CHKSM=3C1D4F864F169F9B7C2BD7FA518F69CCB2039BD8