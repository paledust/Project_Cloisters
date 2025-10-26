// Made with Amplify Shader Editor v1.9.8.1
// Available at the Unity Asset Store - http://u3d.as/y3X 
Shader "AmplifyShaders/Sprite EvocativeDissolve Unlit"
{
	Properties
	{
		[HideInInspector] _EmissionColor("Emission Color", Color) = (1,1,1,1)
		[HideInInspector] _AlphaCutoff("Alpha Cutoff ", Range(0, 1)) = 0.5
		[HDR]_FillColor("FillColor", Color) = (0,0,0,1)
		_NoiseTex("NoiseTex", 2D) = "white" {}
		_DissolvePanSpeed("DissolvePanSpeed", Vector) = (0,0,0,0)
		_DissolveSoftness("DissolveSoftness", Float) = 1
		_PatternScale("PatternScale", Float) = 1
		_DissolveStart("DissolveStart", Float) = 0.5
		_DissolveLength("DissolveLength", Float) = 0.1
		_EdgeValue("EdgeValue", Float) = 0
		_EdgeSoftness("EdgeSoftness", Float) = 1
		[HDR]_EdgeColor("EdgeColor", Color) = (2.828427,2.029812,1.209014,1)
		_EmissionScale("EmissionScale", Float) = 1
		[PerRendererData]_NoiseUVOffset("NoiseUVOffset", Vector) = (0,0,0,0)
		_GlowPatternScale("GlowPatternScale", Float) = 0.1
		[HDR]_GlowColor("GlowColor", Color) = (1,1,1,1)
		_GlowValue("GlowValue", Float) = 0
		_GlowSoftness("GlowSoftness", Float) = 1
		_NoiseStrength("NoiseStrength", Float) = 0

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
			#define ASE_SRP_VERSION 140011


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
			#include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Input.hlsl"
			#include "Packages/com.unity.render-pipelines.core/ShaderLibrary/TextureStack.hlsl"

			
            #include "Packages/com.unity.render-pipelines.core/ShaderLibrary/FoveatedRendering.hlsl"
           

			
            #if ASE_SRP_VERSION >=140009
			#include_with_pragmas "Packages/com.unity.render-pipelines.core/ShaderLibrary/FoveatedRenderingKeywords.hlsl"
			#endif
		

			#include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/ShaderGraphFunctions.hlsl"
			#include "Packages/com.unity.render-pipelines.universal/Editor/ShaderGraph/Includes/ShaderPass.hlsl"

			#include "Packages/com.unity.render-pipelines.universal/Shaders/2D/Include/SurfaceData2D.hlsl"
			#include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Debug/Debugging2D.hlsl"

			#define ASE_NEEDS_FRAG_COLOR
			#pragma multi_compile_instancing


			sampler2D _NoiseTex;
			UNITY_INSTANCING_BUFFER_START(AmplifyShadersSpriteEvocativeDissolveUnlit)
				UNITY_DEFINE_INSTANCED_PROP(float2, _NoiseUVOffset)
				UNITY_DEFINE_INSTANCED_PROP(float, _DissolveStart)
				UNITY_DEFINE_INSTANCED_PROP(float, _DissolveLength)
				UNITY_DEFINE_INSTANCED_PROP(float, _EmissionScale)
			UNITY_INSTANCING_BUFFER_END(AmplifyShadersSpriteEvocativeDissolveUnlit)
			CBUFFER_START( UnityPerMaterial )
			float4 _FillColor;
			float4 _GlowColor;
			float4 _EdgeColor;
			float2 _DissolvePanSpeed;
			float _GlowValue;
			float _GlowSoftness;
			float _GlowPatternScale;
			float _DissolveSoftness;
			float _PatternScale;
			float _NoiseStrength;
			float _EdgeValue;
			float _EdgeSoftness;
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

				float3 ase_positionWS = TransformObjectToWorld( ( v.positionOS ).xyz );
				o.ase_texcoord3.xyz = ase_positionWS;
				
				
				//setting value to unused interpolator channels and avoid initialization warnings
				o.ase_texcoord3.w = 0;
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

				float2 _NoiseUVOffset_Instance = UNITY_ACCESS_INSTANCED_PROP(AmplifyShadersSpriteEvocativeDissolveUnlit,_NoiseUVOffset);
				float2 dissolvePanSpeed160 = _DissolvePanSpeed;
				float3 ase_positionWS = IN.ase_texcoord3.xyz;
				float2 appendResult90 = (float2(ase_positionWS.x , ase_positionWS.y));
				float2 panner126 = ( 1.0 * _Time.y * dissolvePanSpeed160 + appendResult90);
				float2 worldUV208 = ( _NoiseUVOffset_Instance + panner126 );
				float2 texCoord87 = IN.texCoord0.xy * float2( 1,1 ) + float2( 0,0 );
				float2 uv337 = texCoord87;
				float temp_output_325_0 = ( distance( uv337 , float2( 0.5,0.5 ) ) * 2.0 );
				float _DissolveStart_Instance = UNITY_ACCESS_INSTANCED_PROP(AmplifyShadersSpriteEvocativeDissolveUnlit,_DissolveStart);
				float dissolveRadius151 = _DissolveStart_Instance;
				float _DissolveLength_Instance = UNITY_ACCESS_INSTANCED_PROP(AmplifyShadersSpriteEvocativeDissolveUnlit,_DissolveLength);
				float dissolveLength152 = _DissolveLength_Instance;
				float temp_output_21_0_g31 = dissolveLength152;
				float temp_output_5_0_g31 = ( ( ( 1.0 - temp_output_325_0 ) - ( dissolveRadius151 - temp_output_21_0_g31 ) ) / temp_output_21_0_g31 );
				float smoothstepResult291 = smoothstep( _GlowValue , ( _GlowValue + _GlowSoftness ) , ( ( tex2D( _NoiseTex, ( _GlowPatternScale * worldUV208 ) ).r * 0.0 ) + -temp_output_5_0_g31 ));
				float glow292 = ( 1.0 - smoothstepResult291 );
				float4 lerpResult294 = lerp( _FillColor , _GlowColor , glow292);
				float patternScale162 = _PatternScale;
				float noiseStrength321 = _NoiseStrength;
				float temp_output_21_0_g30 = dissolveLength152;
				float temp_output_5_0_g30 = ( ( temp_output_325_0 - ( dissolveRadius151 - temp_output_21_0_g30 ) ) / temp_output_21_0_g30 );
				float temp_output_273_0 = ( ( tex2D( _NoiseTex, ( patternScale162 * worldUV208 ) ).r * noiseStrength321 ) + -temp_output_5_0_g30 );
				float smoothstepResult42 = smoothstep( 0.0 , _DissolveSoftness , temp_output_273_0);
				float temp_output_41_0 = saturate( smoothstepResult42 );
				float smoothstepResult49 = smoothstep( _EdgeValue , ( _EdgeValue + _EdgeSoftness ) , temp_output_273_0);
				float temp_output_53_0 = ( 1.0 - smoothstepResult49 );
				float4 appendResult40 = (float4(( lerpResult294 + ( saturate( ( temp_output_41_0 * ( temp_output_53_0 * temp_output_53_0 ) ) ) * _EdgeColor ) ).rgb , temp_output_41_0));
				float _EmissionScale_Instance = UNITY_ACCESS_INSTANCED_PROP(AmplifyShadersSpriteEvocativeDissolveUnlit,_EmissionScale);
				float4 appendResult122 = (float4(( (appendResult40).xyz * _EmissionScale_Instance ) , (appendResult40).w));
				
				float4 Color = ( IN.color * appendResult122 );

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
			#define ASE_SRP_VERSION 140011


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
			#include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Input.hlsl"
			#include "Packages/com.unity.render-pipelines.core/ShaderLibrary/TextureStack.hlsl"

			
            #include "Packages/com.unity.render-pipelines.core/ShaderLibrary/FoveatedRendering.hlsl"
           

			
            #if ASE_SRP_VERSION >=140009
			#include_with_pragmas "Packages/com.unity.render-pipelines.core/ShaderLibrary/FoveatedRenderingKeywords.hlsl"
			#endif
		

			#include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/ShaderGraphFunctions.hlsl"
			#include "Packages/com.unity.render-pipelines.universal/Editor/ShaderGraph/Includes/ShaderPass.hlsl"

			#include "Packages/com.unity.render-pipelines.universal/Shaders/2D/Include/SurfaceData2D.hlsl"
			#include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Debug/Debugging2D.hlsl"

			#define ASE_NEEDS_FRAG_COLOR
			#pragma multi_compile_instancing


			sampler2D _NoiseTex;
			UNITY_INSTANCING_BUFFER_START(AmplifyShadersSpriteEvocativeDissolveUnlit)
				UNITY_DEFINE_INSTANCED_PROP(float2, _NoiseUVOffset)
				UNITY_DEFINE_INSTANCED_PROP(float, _DissolveStart)
				UNITY_DEFINE_INSTANCED_PROP(float, _DissolveLength)
				UNITY_DEFINE_INSTANCED_PROP(float, _EmissionScale)
			UNITY_INSTANCING_BUFFER_END(AmplifyShadersSpriteEvocativeDissolveUnlit)
			CBUFFER_START( UnityPerMaterial )
			float4 _FillColor;
			float4 _GlowColor;
			float4 _EdgeColor;
			float2 _DissolvePanSpeed;
			float _GlowValue;
			float _GlowSoftness;
			float _GlowPatternScale;
			float _DissolveSoftness;
			float _PatternScale;
			float _NoiseStrength;
			float _EdgeValue;
			float _EdgeSoftness;
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

				float3 ase_positionWS = TransformObjectToWorld( ( v.positionOS ).xyz );
				o.ase_texcoord3.xyz = ase_positionWS;
				
				
				//setting value to unused interpolator channels and avoid initialization warnings
				o.ase_texcoord3.w = 0;
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

				float2 _NoiseUVOffset_Instance = UNITY_ACCESS_INSTANCED_PROP(AmplifyShadersSpriteEvocativeDissolveUnlit,_NoiseUVOffset);
				float2 dissolvePanSpeed160 = _DissolvePanSpeed;
				float3 ase_positionWS = IN.ase_texcoord3.xyz;
				float2 appendResult90 = (float2(ase_positionWS.x , ase_positionWS.y));
				float2 panner126 = ( 1.0 * _Time.y * dissolvePanSpeed160 + appendResult90);
				float2 worldUV208 = ( _NoiseUVOffset_Instance + panner126 );
				float2 texCoord87 = IN.texCoord0.xy * float2( 1,1 ) + float2( 0,0 );
				float2 uv337 = texCoord87;
				float temp_output_325_0 = ( distance( uv337 , float2( 0.5,0.5 ) ) * 2.0 );
				float _DissolveStart_Instance = UNITY_ACCESS_INSTANCED_PROP(AmplifyShadersSpriteEvocativeDissolveUnlit,_DissolveStart);
				float dissolveRadius151 = _DissolveStart_Instance;
				float _DissolveLength_Instance = UNITY_ACCESS_INSTANCED_PROP(AmplifyShadersSpriteEvocativeDissolveUnlit,_DissolveLength);
				float dissolveLength152 = _DissolveLength_Instance;
				float temp_output_21_0_g31 = dissolveLength152;
				float temp_output_5_0_g31 = ( ( ( 1.0 - temp_output_325_0 ) - ( dissolveRadius151 - temp_output_21_0_g31 ) ) / temp_output_21_0_g31 );
				float smoothstepResult291 = smoothstep( _GlowValue , ( _GlowValue + _GlowSoftness ) , ( ( tex2D( _NoiseTex, ( _GlowPatternScale * worldUV208 ) ).r * 0.0 ) + -temp_output_5_0_g31 ));
				float glow292 = ( 1.0 - smoothstepResult291 );
				float4 lerpResult294 = lerp( _FillColor , _GlowColor , glow292);
				float patternScale162 = _PatternScale;
				float noiseStrength321 = _NoiseStrength;
				float temp_output_21_0_g30 = dissolveLength152;
				float temp_output_5_0_g30 = ( ( temp_output_325_0 - ( dissolveRadius151 - temp_output_21_0_g30 ) ) / temp_output_21_0_g30 );
				float temp_output_273_0 = ( ( tex2D( _NoiseTex, ( patternScale162 * worldUV208 ) ).r * noiseStrength321 ) + -temp_output_5_0_g30 );
				float smoothstepResult42 = smoothstep( 0.0 , _DissolveSoftness , temp_output_273_0);
				float temp_output_41_0 = saturate( smoothstepResult42 );
				float smoothstepResult49 = smoothstep( _EdgeValue , ( _EdgeValue + _EdgeSoftness ) , temp_output_273_0);
				float temp_output_53_0 = ( 1.0 - smoothstepResult49 );
				float4 appendResult40 = (float4(( lerpResult294 + ( saturate( ( temp_output_41_0 * ( temp_output_53_0 * temp_output_53_0 ) ) ) * _EdgeColor ) ).rgb , temp_output_41_0));
				float _EmissionScale_Instance = UNITY_ACCESS_INSTANCED_PROP(AmplifyShadersSpriteEvocativeDissolveUnlit,_EmissionScale);
				float4 appendResult122 = (float4(( (appendResult40).xyz * _EmissionScale_Instance ) , (appendResult40).w));
				
				float4 Color = ( IN.color * appendResult122 );

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
			#define ASE_SRP_VERSION 140011


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
			#include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Input.hlsl"
			#include "Packages/com.unity.render-pipelines.core/ShaderLibrary/TextureStack.hlsl"

			
            #include "Packages/com.unity.render-pipelines.core/ShaderLibrary/FoveatedRendering.hlsl"
           

			
            #if ASE_SRP_VERSION >=140009
			#include_with_pragmas "Packages/com.unity.render-pipelines.core/ShaderLibrary/FoveatedRenderingKeywords.hlsl"
			#endif
		

			#include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/ShaderGraphFunctions.hlsl"
			#include_with_pragmas "Packages/com.unity.render-pipelines.universal/ShaderLibrary/DOTS.hlsl"
			#include "Packages/com.unity.render-pipelines.universal/Editor/ShaderGraph/Includes/ShaderPass.hlsl"

			#pragma multi_compile_instancing


			sampler2D _NoiseTex;
			UNITY_INSTANCING_BUFFER_START(AmplifyShadersSpriteEvocativeDissolveUnlit)
				UNITY_DEFINE_INSTANCED_PROP(float2, _NoiseUVOffset)
				UNITY_DEFINE_INSTANCED_PROP(float, _DissolveStart)
				UNITY_DEFINE_INSTANCED_PROP(float, _DissolveLength)
				UNITY_DEFINE_INSTANCED_PROP(float, _EmissionScale)
			UNITY_INSTANCING_BUFFER_END(AmplifyShadersSpriteEvocativeDissolveUnlit)
			CBUFFER_START( UnityPerMaterial )
			float4 _FillColor;
			float4 _GlowColor;
			float4 _EdgeColor;
			float2 _DissolvePanSpeed;
			float _GlowValue;
			float _GlowSoftness;
			float _GlowPatternScale;
			float _DissolveSoftness;
			float _PatternScale;
			float _NoiseStrength;
			float _EdgeValue;
			float _EdgeSoftness;
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

				float3 ase_positionWS = TransformObjectToWorld( ( v.positionOS ).xyz );
				o.ase_texcoord.xyz = ase_positionWS;
				
				o.ase_color = v.ase_color;
				o.ase_texcoord1.xy = v.ase_texcoord.xy;
				
				//setting value to unused interpolator channels and avoid initialization warnings
				o.ase_texcoord.w = 0;
				o.ase_texcoord1.zw = 0;
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
				float2 _NoiseUVOffset_Instance = UNITY_ACCESS_INSTANCED_PROP(AmplifyShadersSpriteEvocativeDissolveUnlit,_NoiseUVOffset);
				float2 dissolvePanSpeed160 = _DissolvePanSpeed;
				float3 ase_positionWS = IN.ase_texcoord.xyz;
				float2 appendResult90 = (float2(ase_positionWS.x , ase_positionWS.y));
				float2 panner126 = ( 1.0 * _Time.y * dissolvePanSpeed160 + appendResult90);
				float2 worldUV208 = ( _NoiseUVOffset_Instance + panner126 );
				float2 texCoord87 = IN.ase_texcoord1.xy * float2( 1,1 ) + float2( 0,0 );
				float2 uv337 = texCoord87;
				float temp_output_325_0 = ( distance( uv337 , float2( 0.5,0.5 ) ) * 2.0 );
				float _DissolveStart_Instance = UNITY_ACCESS_INSTANCED_PROP(AmplifyShadersSpriteEvocativeDissolveUnlit,_DissolveStart);
				float dissolveRadius151 = _DissolveStart_Instance;
				float _DissolveLength_Instance = UNITY_ACCESS_INSTANCED_PROP(AmplifyShadersSpriteEvocativeDissolveUnlit,_DissolveLength);
				float dissolveLength152 = _DissolveLength_Instance;
				float temp_output_21_0_g31 = dissolveLength152;
				float temp_output_5_0_g31 = ( ( ( 1.0 - temp_output_325_0 ) - ( dissolveRadius151 - temp_output_21_0_g31 ) ) / temp_output_21_0_g31 );
				float smoothstepResult291 = smoothstep( _GlowValue , ( _GlowValue + _GlowSoftness ) , ( ( tex2D( _NoiseTex, ( _GlowPatternScale * worldUV208 ) ).r * 0.0 ) + -temp_output_5_0_g31 ));
				float glow292 = ( 1.0 - smoothstepResult291 );
				float4 lerpResult294 = lerp( _FillColor , _GlowColor , glow292);
				float patternScale162 = _PatternScale;
				float noiseStrength321 = _NoiseStrength;
				float temp_output_21_0_g30 = dissolveLength152;
				float temp_output_5_0_g30 = ( ( temp_output_325_0 - ( dissolveRadius151 - temp_output_21_0_g30 ) ) / temp_output_21_0_g30 );
				float temp_output_273_0 = ( ( tex2D( _NoiseTex, ( patternScale162 * worldUV208 ) ).r * noiseStrength321 ) + -temp_output_5_0_g30 );
				float smoothstepResult42 = smoothstep( 0.0 , _DissolveSoftness , temp_output_273_0);
				float temp_output_41_0 = saturate( smoothstepResult42 );
				float smoothstepResult49 = smoothstep( _EdgeValue , ( _EdgeValue + _EdgeSoftness ) , temp_output_273_0);
				float temp_output_53_0 = ( 1.0 - smoothstepResult49 );
				float4 appendResult40 = (float4(( lerpResult294 + ( saturate( ( temp_output_41_0 * ( temp_output_53_0 * temp_output_53_0 ) ) ) * _EdgeColor ) ).rgb , temp_output_41_0));
				float _EmissionScale_Instance = UNITY_ACCESS_INSTANCED_PROP(AmplifyShadersSpriteEvocativeDissolveUnlit,_EmissionScale);
				float4 appendResult122 = (float4(( (appendResult40).xyz * _EmissionScale_Instance ) , (appendResult40).w));
				
				float4 Color = ( IN.ase_color * appendResult122 );

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
			#define ASE_SRP_VERSION 140011


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
			#include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Input.hlsl"
			#include "Packages/com.unity.render-pipelines.core/ShaderLibrary/TextureStack.hlsl"

			
            #include "Packages/com.unity.render-pipelines.core/ShaderLibrary/FoveatedRendering.hlsl"
           

			
            #if ASE_SRP_VERSION >=140009
			#include_with_pragmas "Packages/com.unity.render-pipelines.core/ShaderLibrary/FoveatedRenderingKeywords.hlsl"
			#endif
		

			#include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/ShaderGraphFunctions.hlsl"
			#include_with_pragmas "Packages/com.unity.render-pipelines.universal/ShaderLibrary/DOTS.hlsl"
			#include "Packages/com.unity.render-pipelines.universal/Editor/ShaderGraph/Includes/ShaderPass.hlsl"

        	#pragma multi_compile_instancing


			sampler2D _NoiseTex;
			UNITY_INSTANCING_BUFFER_START(AmplifyShadersSpriteEvocativeDissolveUnlit)
				UNITY_DEFINE_INSTANCED_PROP(float2, _NoiseUVOffset)
				UNITY_DEFINE_INSTANCED_PROP(float, _DissolveStart)
				UNITY_DEFINE_INSTANCED_PROP(float, _DissolveLength)
				UNITY_DEFINE_INSTANCED_PROP(float, _EmissionScale)
			UNITY_INSTANCING_BUFFER_END(AmplifyShadersSpriteEvocativeDissolveUnlit)
			CBUFFER_START( UnityPerMaterial )
			float4 _FillColor;
			float4 _GlowColor;
			float4 _EdgeColor;
			float2 _DissolvePanSpeed;
			float _GlowValue;
			float _GlowSoftness;
			float _GlowPatternScale;
			float _DissolveSoftness;
			float _PatternScale;
			float _NoiseStrength;
			float _EdgeValue;
			float _EdgeSoftness;
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

				float3 ase_positionWS = TransformObjectToWorld( ( v.positionOS ).xyz );
				o.ase_texcoord.xyz = ase_positionWS;
				
				o.ase_color = v.ase_color;
				o.ase_texcoord1.xy = v.ase_texcoord.xy;
				
				//setting value to unused interpolator channels and avoid initialization warnings
				o.ase_texcoord.w = 0;
				o.ase_texcoord1.zw = 0;
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
				float2 _NoiseUVOffset_Instance = UNITY_ACCESS_INSTANCED_PROP(AmplifyShadersSpriteEvocativeDissolveUnlit,_NoiseUVOffset);
				float2 dissolvePanSpeed160 = _DissolvePanSpeed;
				float3 ase_positionWS = IN.ase_texcoord.xyz;
				float2 appendResult90 = (float2(ase_positionWS.x , ase_positionWS.y));
				float2 panner126 = ( 1.0 * _Time.y * dissolvePanSpeed160 + appendResult90);
				float2 worldUV208 = ( _NoiseUVOffset_Instance + panner126 );
				float2 texCoord87 = IN.ase_texcoord1.xy * float2( 1,1 ) + float2( 0,0 );
				float2 uv337 = texCoord87;
				float temp_output_325_0 = ( distance( uv337 , float2( 0.5,0.5 ) ) * 2.0 );
				float _DissolveStart_Instance = UNITY_ACCESS_INSTANCED_PROP(AmplifyShadersSpriteEvocativeDissolveUnlit,_DissolveStart);
				float dissolveRadius151 = _DissolveStart_Instance;
				float _DissolveLength_Instance = UNITY_ACCESS_INSTANCED_PROP(AmplifyShadersSpriteEvocativeDissolveUnlit,_DissolveLength);
				float dissolveLength152 = _DissolveLength_Instance;
				float temp_output_21_0_g31 = dissolveLength152;
				float temp_output_5_0_g31 = ( ( ( 1.0 - temp_output_325_0 ) - ( dissolveRadius151 - temp_output_21_0_g31 ) ) / temp_output_21_0_g31 );
				float smoothstepResult291 = smoothstep( _GlowValue , ( _GlowValue + _GlowSoftness ) , ( ( tex2D( _NoiseTex, ( _GlowPatternScale * worldUV208 ) ).r * 0.0 ) + -temp_output_5_0_g31 ));
				float glow292 = ( 1.0 - smoothstepResult291 );
				float4 lerpResult294 = lerp( _FillColor , _GlowColor , glow292);
				float patternScale162 = _PatternScale;
				float noiseStrength321 = _NoiseStrength;
				float temp_output_21_0_g30 = dissolveLength152;
				float temp_output_5_0_g30 = ( ( temp_output_325_0 - ( dissolveRadius151 - temp_output_21_0_g30 ) ) / temp_output_21_0_g30 );
				float temp_output_273_0 = ( ( tex2D( _NoiseTex, ( patternScale162 * worldUV208 ) ).r * noiseStrength321 ) + -temp_output_5_0_g30 );
				float smoothstepResult42 = smoothstep( 0.0 , _DissolveSoftness , temp_output_273_0);
				float temp_output_41_0 = saturate( smoothstepResult42 );
				float smoothstepResult49 = smoothstep( _EdgeValue , ( _EdgeValue + _EdgeSoftness ) , temp_output_273_0);
				float temp_output_53_0 = ( 1.0 - smoothstepResult49 );
				float4 appendResult40 = (float4(( lerpResult294 + ( saturate( ( temp_output_41_0 * ( temp_output_53_0 * temp_output_53_0 ) ) ) * _EdgeColor ) ).rgb , temp_output_41_0));
				float _EmissionScale_Instance = UNITY_ACCESS_INSTANCED_PROP(AmplifyShadersSpriteEvocativeDissolveUnlit,_EmissionScale);
				float4 appendResult122 = (float4(( (appendResult40).xyz * _EmissionScale_Instance ) , (appendResult40).w));
				
				float4 Color = ( IN.ase_color * appendResult122 );
				half4 outColor = _SelectionID;
				return outColor;
			}

            ENDHLSL
        }
		
	}
	CustomEditor "AmplifyShaderEditor.MaterialInspector"
	FallBack "Hidden/Shader Graph/FallbackError"
	
	Fallback "Hidden/InternalErrorShader"
}
/*ASEBEGIN
Version=19801
Node;AmplifyShaderEditor.CommentaryNode;306;-7814.229,-386;Inherit;False;657.6773;1160.616;Comment;14;162;151;152;166;25;35;26;81;160;125;307;129;320;321;Parameter;1,1,1,1;0;0
Node;AmplifyShaderEditor.CommentaryNode;213;-7776,-1008;Inherit;False;1073.408;482.3657;Comment;7;206;207;126;90;165;89;208;world UV;1,1,1,1;0;0
Node;AmplifyShaderEditor.Vector2Node;125;-7760,96;Inherit;False;Property;_DissolvePanSpeed;DissolvePanSpeed;3;0;Create;True;0;0;0;False;0;False;0,0;0.5,0.3;0;3;FLOAT2;0;FLOAT;1;FLOAT;2
Node;AmplifyShaderEditor.WorldPosInputsNode;89;-7728,-784;Inherit;False;0;4;FLOAT3;0;FLOAT;1;FLOAT;2;FLOAT;3
Node;AmplifyShaderEditor.RegisterLocalVarNode;160;-7520,96;Inherit;False;dissolvePanSpeed;-1;True;1;0;FLOAT2;0,0;False;1;FLOAT2;0
Node;AmplifyShaderEditor.DynamicAppendNode;90;-7552,-752;Inherit;False;FLOAT2;4;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;0;False;3;FLOAT;0;False;1;FLOAT2;0
Node;AmplifyShaderEditor.GetLocalVarNode;165;-7600,-624;Inherit;False;160;dissolvePanSpeed;1;0;OBJECT;;False;1;FLOAT2;0
Node;AmplifyShaderEditor.TextureCoordinatesNode;87;-3744,-848;Inherit;True;0;-1;2;3;2;SAMPLER2D;;False;0;FLOAT2;1,1;False;1;FLOAT2;0,0;False;5;FLOAT2;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.PannerNode;126;-7312,-704;Inherit;False;3;0;FLOAT2;0,0;False;2;FLOAT2;0.2,0.5;False;1;FLOAT;1;False;1;FLOAT2;0
Node;AmplifyShaderEditor.Vector2Node;206;-7312,-960;Inherit;False;InstancedProperty;_NoiseUVOffset;NoiseUVOffset;12;1;[PerRendererData];Create;True;0;0;0;False;0;False;0,0;0,0;0;3;FLOAT2;0;FLOAT;1;FLOAT;2
Node;AmplifyShaderEditor.RegisterLocalVarNode;337;-3520,-848;Inherit;False;uv;-1;True;1;0;FLOAT2;0,0;False;1;FLOAT2;0
Node;AmplifyShaderEditor.SimpleAddOpNode;207;-7072,-832;Inherit;False;2;2;0;FLOAT2;0,0;False;1;FLOAT2;0,0;False;1;FLOAT2;0
Node;AmplifyShaderEditor.TexturePropertyNode;81;-7744,-336;Inherit;True;Property;_NoiseTex;NoiseTex;2;0;Create;True;0;0;0;False;0;False;97a921d248636704ead1a9dcc3b43211;184854303c9362c43b86499202498085;False;white;Auto;Texture2D;-1;0;2;SAMPLER2D;0;SAMPLERSTATE;1
Node;AmplifyShaderEditor.RangedFloatNode;26;-7744,320;Inherit;False;InstancedProperty;_DissolveLength;DissolveLength;7;0;Create;True;0;0;0;False;0;False;0.1;0.1;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;35;-7744,400;Inherit;False;Property;_PatternScale;PatternScale;5;0;Create;True;0;0;0;False;0;False;1;0.25;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;25;-7744,224;Inherit;False;InstancedProperty;_DissolveStart;DissolveStart;6;0;Create;True;0;0;0;False;0;False;0.5;0.49;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;320;-7744,480;Inherit;False;Property;_NoiseStrength;NoiseStrength;20;0;Create;True;0;0;0;False;0;False;0;0.25;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.GetLocalVarNode;144;-2976,-448;Inherit;True;337;uv;1;0;OBJECT;;False;1;FLOAT2;0
Node;AmplifyShaderEditor.RegisterLocalVarNode;166;-7456,-336;Inherit;False;noiseTex;-1;True;1;0;SAMPLER2D;;False;1;SAMPLER2D;0
Node;AmplifyShaderEditor.RegisterLocalVarNode;152;-7520,320;Inherit;False;dissolveLength;-1;True;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.RegisterLocalVarNode;151;-7520,224;Inherit;False;dissolveRadius;-1;True;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.RegisterLocalVarNode;208;-6944,-832;Inherit;False;worldUV;-1;True;1;0;FLOAT2;0,0;False;1;FLOAT2;0
Node;AmplifyShaderEditor.RegisterLocalVarNode;162;-7520,400;Inherit;False;patternScale;-1;True;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.RegisterLocalVarNode;321;-7520,480;Inherit;False;noiseStrength;-1;True;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.DistanceOpNode;318;-2752,-448;Inherit;True;2;0;FLOAT2;0,0;False;1;FLOAT2;0.5,0.5;False;1;FLOAT;0
Node;AmplifyShaderEditor.GetLocalVarNode;278;-2416,32;Inherit;False;208;worldUV;1;0;OBJECT;;False;1;FLOAT2;0
Node;AmplifyShaderEditor.GetLocalVarNode;322;-2338.495,-574.3788;Inherit;False;321;noiseStrength;1;0;OBJECT;;False;1;FLOAT;0
Node;AmplifyShaderEditor.GetLocalVarNode;274;-2400,112;Inherit;False;166;noiseTex;1;0;OBJECT;;False;1;SAMPLER2D;0
Node;AmplifyShaderEditor.GetLocalVarNode;275;-2400,192;Inherit;False;162;patternScale;1;0;OBJECT;;False;1;FLOAT;0
Node;AmplifyShaderEditor.GetLocalVarNode;277;-2336,368;Inherit;False;152;dissolveLength;1;0;OBJECT;;False;1;FLOAT;0
Node;AmplifyShaderEditor.GetLocalVarNode;276;-2368,272;Inherit;False;151;dissolveRadius;1;0;OBJECT;;False;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;52;-1488,176;Inherit;False;Property;_EdgeSoftness;EdgeSoftness;9;0;Create;True;0;0;0;False;0;False;1;1.8;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;51;-1456,112;Inherit;False;Property;_EdgeValue;EdgeValue;8;0;Create;True;0;0;0;False;0;False;0;-0.41;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;325;-2544,-448;Inherit;False;2;2;0;FLOAT;0;False;1;FLOAT;2;False;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;289;-1472,880;Inherit;False;Property;_GlowValue;GlowValue;18;0;Create;True;0;0;0;False;0;False;0;-2.8;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;288;-1504,960;Inherit;False;Property;_GlowSoftness;GlowSoftness;19;0;Create;True;0;0;0;False;0;False;1;0.1;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.FunctionNode;273;-1904,-224;Inherit;True;SGradientDissolve;-1;;30;e27ad990a2429b34d8f406fd90200dba;1,26,0;7;29;FLOAT;1;False;24;FLOAT;0;False;22;FLOAT2;0,0;False;17;SAMPLER2D;;False;18;FLOAT;1;False;20;FLOAT;0.5;False;21;FLOAT;0.1;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleAddOpNode;57;-1280,160;Inherit;False;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.OneMinusNode;327;-2384,-448;Inherit;False;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;282;-2304,1056;Inherit;False;Property;_GlowPatternScale;GlowPatternScale;16;0;Create;True;0;0;0;False;0;False;0.1;1;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.GetLocalVarNode;313;-2304,1136;Inherit;False;151;dissolveRadius;1;0;OBJECT;;False;1;FLOAT;0
Node;AmplifyShaderEditor.GetLocalVarNode;314;-2304,1216;Inherit;False;152;dissolveLength;1;0;OBJECT;;False;1;FLOAT;0
Node;AmplifyShaderEditor.GetLocalVarNode;311;-2272,976;Inherit;False;166;noiseTex;1;0;OBJECT;;False;1;SAMPLER2D;0
Node;AmplifyShaderEditor.GetLocalVarNode;310;-2272,896;Inherit;False;208;worldUV;1;0;OBJECT;;False;1;FLOAT2;0
Node;AmplifyShaderEditor.RangedFloatNode;43;-1328,-144;Inherit;False;Property;_DissolveSoftness;DissolveSoftness;4;0;Create;True;0;0;0;False;0;False;1;0.05;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleAddOpNode;290;-1269.51,928.3375;Inherit;False;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SmoothstepOpNode;49;-1120,96;Inherit;True;3;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;1;False;1;FLOAT;0
Node;AmplifyShaderEditor.FunctionNode;281;-1840,832;Inherit;False;SGradientDissolve;-1;;31;e27ad990a2429b34d8f406fd90200dba;1,26,0;7;29;FLOAT;0;False;24;FLOAT;0;False;22;FLOAT2;0,0;False;17;SAMPLER2D;;False;18;FLOAT;1;False;20;FLOAT;0.5;False;21;FLOAT;0.1;False;1;FLOAT;0
Node;AmplifyShaderEditor.SmoothstepOpNode;42;-1072,-224;Inherit;False;3;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;1;False;1;FLOAT;0
Node;AmplifyShaderEditor.SmoothstepOpNode;291;-1055.941,861.1998;Inherit;True;3;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;1;False;1;FLOAT;0
Node;AmplifyShaderEditor.OneMinusNode;53;-880,96;Inherit;True;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SaturateNode;41;-864,-224;Inherit;True;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.OneMinusNode;297;-818.042,871.6349;Inherit;False;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;329;-688,112;Inherit;True;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.RegisterLocalVarNode;292;-656,864;Inherit;False;glow;-1;True;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;58;-544,-32;Inherit;False;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.ColorNode;79;-560,-768;Inherit;False;Property;_FillColor;FillColor;1;1;[HDR];Create;True;0;0;0;False;0;False;0,0,0,1;1,0.4812231,0.1566037,1;True;True;0;6;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4;FLOAT3;5
Node;AmplifyShaderEditor.ColorNode;296;-544,-560;Inherit;False;Property;_GlowColor;GlowColor;17;1;[HDR];Create;True;0;0;0;False;0;False;1,1,1,1;2.118548,2.118548,2.118548,1;True;True;0;6;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4;FLOAT3;5
Node;AmplifyShaderEditor.ColorNode;54;-384,80;Inherit;False;Property;_EdgeColor;EdgeColor;10;1;[HDR];Create;True;0;0;0;False;0;False;2.828427,2.029812,1.209014,1;2,1.159775,1.007547,1;True;True;0;6;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4;FLOAT3;5
Node;AmplifyShaderEditor.SaturateNode;59;-320,-32;Inherit;False;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.GetLocalVarNode;295;-512,-352;Inherit;False;292;glow;1;0;OBJECT;;False;1;FLOAT;0
Node;AmplifyShaderEditor.LerpOp;294;-80,-592;Inherit;False;3;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0,0;False;2;FLOAT;0;False;1;COLOR;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;55;-160,0;Inherit;False;2;2;0;FLOAT;0;False;1;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.SimpleAddOpNode;56;222.8557,-594.6979;Inherit;False;2;2;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.DynamicAppendNode;40;977.6847,-288.4521;Inherit;False;FLOAT4;4;0;FLOAT3;1,0,0;False;1;FLOAT;1;False;2;FLOAT;1;False;3;FLOAT;0;False;1;FLOAT4;0
Node;AmplifyShaderEditor.ComponentMaskNode;120;1172.593,-383.2383;Inherit;False;True;True;True;False;1;0;FLOAT4;0,0,0,0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.RangedFloatNode;203;1197.449,-301.023;Inherit;False;InstancedProperty;_EmissionScale;EmissionScale;11;0;Create;True;0;0;0;False;0;False;1;1;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.ComponentMaskNode;121;1168,-192;Inherit;False;False;False;False;True;1;0;FLOAT4;0,0,0,0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;123;1398.373,-437.7367;Inherit;False;2;2;0;FLOAT3;0,0,0;False;1;FLOAT;0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.WireNode;326;1504,-192;Inherit;False;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.CommentaryNode;227;-6368,368;Inherit;False;1473.48;541.8779;Comment;10;224;215;220;222;221;214;219;223;217;218;Blink;1,1,1,1;0;0
Node;AmplifyShaderEditor.CommentaryNode;262;-6395.907,1233.859;Inherit;False;2424.785;713.9421;Comment;15;248;249;238;250;252;253;251;245;254;247;255;237;241;242;279;World Direction Dissolve;1,1,1,1;0;0
Node;AmplifyShaderEditor.DynamicAppendNode;122;1616,-320;Inherit;False;FLOAT4;4;0;FLOAT3;0,0,0;False;1;FLOAT;0;False;2;FLOAT;0;False;3;FLOAT;0;False;1;FLOAT4;0
Node;AmplifyShaderEditor.VertexColorNode;315;1600,-496;Inherit;False;0;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.TexturePropertyNode;129;-7744,-128;Inherit;True;Property;_MainTex;MainTex;0;0;Create;True;0;0;0;False;0;False;None;311925a002f4447b3a28927169b83ea6;False;white;Auto;Texture2D;-1;0;2;SAMPLER2D;0;SAMPLERSTATE;1
Node;AmplifyShaderEditor.RegisterLocalVarNode;307;-7456,-144;Inherit;False;mainTex;-1;True;1;0;SAMPLER2D;;False;1;SAMPLER2D;0
Node;AmplifyShaderEditor.RangedFloatNode;218;-6320,576;Inherit;False;Property;_BlinkSpeed;BlinkSpeed;14;0;Create;True;0;0;0;False;0;False;1;1;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleTimeNode;217;-6144,576;Inherit;False;1;0;FLOAT;1;False;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;223;-6096,416;Inherit;False;InstancedProperty;_RNDSeed;RNDSeed;13;1;[PerRendererData];Create;True;0;0;0;False;0;False;0;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleAddOpNode;219;-5920,480;Inherit;False;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SinOpNode;214;-5776,480;Inherit;True;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.NoiseGeneratorNode;221;-5744,768;Inherit;False;Simplex2D;True;False;2;0;FLOAT2;0,0;False;1;FLOAT;5;False;1;FLOAT;0
Node;AmplifyShaderEditor.ScaleNode;222;-5520,768;Inherit;False;0.2;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.ScaleAndOffsetNode;220;-5600,480;Inherit;True;3;0;FLOAT;0;False;1;FLOAT;0.5;False;2;FLOAT;0.5;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleAddOpNode;215;-5296,624;Inherit;False;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.RegisterLocalVarNode;224;-5152,624;Inherit;False;blink;-1;True;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;238;-6262.403,1492.29;Inherit;False;Property;_DissolveDirection;DissolveDirection;15;0;Create;True;0;0;0;False;0;False;0;0;0;360;0;1;FLOAT;0
Node;AmplifyShaderEditor.RadiansOpNode;250;-5965.071,1493.524;Inherit;False;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SinOpNode;249;-5768.094,1546.089;Inherit;False;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.CosOpNode;248;-5770.094,1444.09;Inherit;False;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.NegateNode;252;-5602.176,1545.131;Inherit;False;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.DynamicAppendNode;253;-5422.176,1596.132;Inherit;False;FLOAT3;4;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;1;False;3;FLOAT;0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.DynamicAppendNode;251;-5414.176,1447.132;Inherit;False;FLOAT3;4;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;1;False;3;FLOAT;0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.MatrixFromVectors;245;-5218.825,1514.306;Inherit;False;FLOAT3x3;True;4;0;FLOAT3;0,0,0;False;1;FLOAT3;0,0,0;False;2;FLOAT3;0,0,0;False;3;FLOAT3;0,0,0;False;1;FLOAT3x3;0
Node;AmplifyShaderEditor.Vector3Node;247;-5186.674,1647.569;Inherit;False;Constant;_Vector1;Vector 1;32;0;Create;True;0;0;0;False;0;False;1,0,0;0,0,0;0;4;FLOAT3;0;FLOAT;1;FLOAT;2;FLOAT;3
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;254;-4972.504,1550.082;Inherit;False;2;2;0;FLOAT3x3;0,0,0,1,1,1,1,0,1;False;1;FLOAT3;0,0,0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.WorldPosInputsNode;237;-4961.518,1341.669;Inherit;False;0;4;FLOAT3;0;FLOAT;1;FLOAT;2;FLOAT;3
Node;AmplifyShaderEditor.ComponentMaskNode;255;-4825.753,1549.034;Inherit;False;True;True;False;True;1;0;FLOAT3;0,0,0;False;1;FLOAT2;0
Node;AmplifyShaderEditor.DynamicAppendNode;241;-4757.061,1366.113;Inherit;False;FLOAT2;4;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;0;False;3;FLOAT;0;False;1;FLOAT2;0
Node;AmplifyShaderEditor.DotProductOpNode;242;-4567.245,1436.842;Inherit;False;2;0;FLOAT2;0,0;False;1;FLOAT2;0,0;False;1;FLOAT;0
Node;AmplifyShaderEditor.RegisterLocalVarNode;279;-4415.676,1443.181;Inherit;False;worldLineDissolve;-1;True;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;316;1792,-352;Inherit;False;2;2;0;COLOR;0,0,0,0;False;1;FLOAT4;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.SimpleSubtractOpNode;86;-3296,-848;Inherit;False;2;0;FLOAT2;0,0;False;1;FLOAT2;0.5,0.5;False;1;FLOAT2;0
Node;AmplifyShaderEditor.LengthOpNode;267;-3136,-848;Inherit;False;1;0;FLOAT2;0,0;False;1;FLOAT;0
Node;AmplifyShaderEditor.RegisterLocalVarNode;266;-2976,-848;Inherit;True;worldRadiusLength;-1;True;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.TemplateMultiPassMasterNode;73;771.9996,-195.7002;Float;False;False;-1;2;AmplifyShaderEditor.MaterialInspector;0;15;New Amplify Shader;cf964e524c8e69742b1d21fbe2ebcc4a;True;Sprite Unlit Forward;0;1;Sprite Unlit Forward;0;False;False;False;False;False;False;False;False;False;False;False;False;False;False;True;2;False;;False;False;False;False;False;False;False;False;False;False;False;False;False;False;True;3;RenderPipeline=UniversalPipeline;RenderType=Transparent=RenderType;Queue=Transparent=Queue=0;True;0;True;12;all;0;False;True;2;5;False;;10;False;;3;1;False;;10;False;;False;False;False;False;False;False;False;False;False;False;False;False;False;False;True;True;True;True;True;0;False;;False;False;False;False;False;False;False;True;False;0;False;;255;False;;255;False;;0;False;;0;False;;0;False;;0;False;;0;False;;0;False;;0;False;;0;False;;False;True;2;False;;True;3;False;;True;True;0;False;;0;False;;True;1;LightMode=UniversalForward;False;False;0;Hidden/InternalErrorShader;0;0;Standard;0;False;0
Node;AmplifyShaderEditor.TemplateMultiPassMasterNode;74;771.9996,-195.7002;Float;False;False;-1;2;AmplifyShaderEditor.MaterialInspector;0;15;New Amplify Shader;cf964e524c8e69742b1d21fbe2ebcc4a;True;SceneSelectionPass;0;2;SceneSelectionPass;0;False;False;False;False;False;False;False;False;False;False;False;False;False;False;True;2;False;;False;False;False;False;False;False;False;False;False;False;False;False;False;False;True;3;RenderPipeline=UniversalPipeline;RenderType=Transparent=RenderType;Queue=Transparent=Queue=0;True;0;True;12;all;0;False;False;False;False;False;False;False;False;False;False;False;False;False;False;True;2;False;;False;False;False;False;False;False;False;False;False;False;False;False;False;False;True;1;LightMode=SceneSelectionPass;False;False;0;Hidden/InternalErrorShader;0;0;Standard;0;False;0
Node;AmplifyShaderEditor.TemplateMultiPassMasterNode;75;771.9996,-195.7002;Float;False;False;-1;2;AmplifyShaderEditor.MaterialInspector;0;15;New Amplify Shader;cf964e524c8e69742b1d21fbe2ebcc4a;True;ScenePickingPass;0;3;ScenePickingPass;0;False;False;False;False;False;False;False;False;False;False;False;False;False;False;True;2;False;;False;False;False;False;False;False;False;False;False;False;False;False;False;False;True;3;RenderPipeline=UniversalPipeline;RenderType=Transparent=RenderType;Queue=Transparent=Queue=0;True;0;True;12;all;0;False;False;False;False;False;False;False;False;False;False;False;False;False;False;True;2;False;;False;False;False;False;False;False;False;False;False;False;False;False;False;False;True;1;LightMode=Picking;False;False;0;Hidden/InternalErrorShader;0;0;Standard;0;False;0
Node;AmplifyShaderEditor.TemplateMultiPassMasterNode;72;1952,-352;Float;False;True;-1;2;AmplifyShaderEditor.MaterialInspector;0;14;AmplifyShaders/Sprite EvocativeDissolve Unlit;cf964e524c8e69742b1d21fbe2ebcc4a;True;Sprite Unlit;0;0;Sprite Unlit;4;False;False;False;False;False;False;False;False;False;False;False;False;False;False;True;2;False;;False;False;False;False;False;False;False;False;False;False;False;False;False;False;True;3;RenderPipeline=UniversalPipeline;RenderType=Transparent=RenderType;Queue=Transparent=Queue=0;True;0;True;12;all;0;False;True;2;5;False;;10;False;;3;1;False;;10;False;;False;False;False;False;False;False;False;False;False;False;False;False;False;False;True;True;True;True;True;0;False;;False;False;False;False;False;False;False;True;False;0;False;;255;False;;255;False;;0;False;;0;False;;0;False;;0;False;;0;False;;0;False;;0;False;;0;False;;False;True;2;False;;True;3;False;;True;True;0;False;;0;False;;True;1;LightMode=Universal2D;False;False;0;Hidden/InternalErrorShader;0;0;Standard;3;Vertex Position;1;0;Debug Display;0;0;External Alpha;0;0;0;4;True;True;True;True;False;;False;0
WireConnection;160;0;125;0
WireConnection;90;0;89;1
WireConnection;90;1;89;2
WireConnection;126;0;90;0
WireConnection;126;2;165;0
WireConnection;337;0;87;0
WireConnection;207;0;206;0
WireConnection;207;1;126;0
WireConnection;166;0;81;0
WireConnection;152;0;26;0
WireConnection;151;0;25;0
WireConnection;208;0;207;0
WireConnection;162;0;35;0
WireConnection;321;0;320;0
WireConnection;318;0;144;0
WireConnection;325;0;318;0
WireConnection;273;29;322;0
WireConnection;273;24;325;0
WireConnection;273;22;278;0
WireConnection;273;17;274;0
WireConnection;273;18;275;0
WireConnection;273;20;276;0
WireConnection;273;21;277;0
WireConnection;57;0;51;0
WireConnection;57;1;52;0
WireConnection;327;0;325;0
WireConnection;290;0;289;0
WireConnection;290;1;288;0
WireConnection;49;0;273;0
WireConnection;49;1;51;0
WireConnection;49;2;57;0
WireConnection;281;24;327;0
WireConnection;281;22;310;0
WireConnection;281;17;311;0
WireConnection;281;18;282;0
WireConnection;281;20;313;0
WireConnection;281;21;314;0
WireConnection;42;0;273;0
WireConnection;42;2;43;0
WireConnection;291;0;281;0
WireConnection;291;1;289;0
WireConnection;291;2;290;0
WireConnection;53;0;49;0
WireConnection;41;0;42;0
WireConnection;297;0;291;0
WireConnection;329;0;53;0
WireConnection;329;1;53;0
WireConnection;292;0;297;0
WireConnection;58;0;41;0
WireConnection;58;1;329;0
WireConnection;59;0;58;0
WireConnection;294;0;79;0
WireConnection;294;1;296;0
WireConnection;294;2;295;0
WireConnection;55;0;59;0
WireConnection;55;1;54;0
WireConnection;56;0;294;0
WireConnection;56;1;55;0
WireConnection;40;0;56;0
WireConnection;40;3;41;0
WireConnection;120;0;40;0
WireConnection;121;0;40;0
WireConnection;123;0;120;0
WireConnection;123;1;203;0
WireConnection;326;0;121;0
WireConnection;122;0;123;0
WireConnection;122;3;326;0
WireConnection;307;0;129;0
WireConnection;217;0;218;0
WireConnection;219;0;223;0
WireConnection;219;1;217;0
WireConnection;214;0;219;0
WireConnection;221;0;219;0
WireConnection;222;0;221;0
WireConnection;220;0;214;0
WireConnection;215;0;220;0
WireConnection;215;1;222;0
WireConnection;224;0;215;0
WireConnection;250;0;238;0
WireConnection;249;0;250;0
WireConnection;248;0;250;0
WireConnection;252;0;249;0
WireConnection;253;0;249;0
WireConnection;253;1;248;0
WireConnection;251;0;248;0
WireConnection;251;1;252;0
WireConnection;245;0;251;0
WireConnection;245;1;253;0
WireConnection;254;0;245;0
WireConnection;254;1;247;0
WireConnection;255;0;254;0
WireConnection;241;0;237;1
WireConnection;241;1;237;2
WireConnection;242;0;241;0
WireConnection;242;1;255;0
WireConnection;279;0;242;0
WireConnection;316;0;315;0
WireConnection;316;1;122;0
WireConnection;86;0;337;0
WireConnection;267;0;86;0
WireConnection;266;0;267;0
WireConnection;72;1;316;0
ASEEND*/
//CHKSM=813051D67744414F875A009E005F16BD5D79DF6B