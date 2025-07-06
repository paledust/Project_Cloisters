// Made with Amplify Shader Editor v1.9.8.1
// Available at the Unity Asset Store - http://u3d.as/y3X 
Shader "AmplifyShaders/Sprite ExperimentalShape Outside"
{
	Properties
	{
		[HideInInspector] _AlphaCutoff("Alpha Cutoff ", Range(0, 1)) = 0.5
		[HideInInspector] _EmissionColor("Emission Color", Color) = (1,1,1,1)
		[PerRendererData]_MainTex("MainTex", 2D) = "white" {}
		_Emissive("Emissive", Float) = 2
		_Saturation("Saturation", Range( 0 , 1)) = 0
		_Tile("Tile", Float) = 1
		_PanSpeed("PanSpeed", Vector) = (0.1,0,0,0)
		_StepMin("StepMin", Float) = 0.2
		_StepMax("StepMax", Float) = 0.2
		_StepSmooth("StepSmooth", Float) = 0.02
		_VoroSpeed("VoroSpeed", Float) = 1
		_VoroStrength("VoroStrength", Float) = 1
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

			#pragma multi_compile_instancing


			sampler2D _MainTex;
			UNITY_INSTANCING_BUFFER_START(AmplifyShadersSpriteExperimentalShapeOutside)
				UNITY_DEFINE_INSTANCED_PROP(float4, _MainTex_ST)
				UNITY_DEFINE_INSTANCED_PROP(float, _Emissive)
			UNITY_INSTANCING_BUFFER_END(AmplifyShadersSpriteExperimentalShapeOutside)
			CBUFFER_START( UnityPerMaterial )
			float2 _PanSpeed;
			float _Saturation;
			float _StepMin;
			float _StepMax;
			float _VoroSpeed;
			float _Tile;
			float _VoroStrength;
			float _StepSmooth;
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

					float2 voronoihash390( float2 p )
					{
						
						p = float2( dot( p, float2( 127.1, 311.7 ) ), dot( p, float2( 269.5, 183.3 ) ) );
						return frac( sin( p ) *43758.5453);
					}
			
					float voronoi390( float2 v, float time, inout float2 id, inout float2 mr, float smoothness, inout float2 smoothId )
					{
						float2 n = floor( v );
						float2 f = frac( v );
						float F1 = 8.0;
						float F2 = 8.0; float2 mg = 0;
						for ( int j = -1; j <= 1; j++ )
						{
							for ( int i = -1; i <= 1; i++ )
						 	{
						 		float2 g = float2( i, j );
						 		float2 o = voronoihash390( n + g );
								o = ( sin( time + o * 6.2831 ) * 0.5 + 0.5 ); float2 r = f - g - o;
								float d = 0.5 * dot( r, r );
						 		if( d<F1 ) {
						 			F2 = F1;
						 			F1 = d; mg = g; mr = r; id = o;
						 		} else if( d<F2 ) {
						 			F2 = d;
						
						 		}
						 	}
						}
						return F1;
					}
			

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

				float4 _MainTex_ST_Instance = UNITY_ACCESS_INSTANCED_PROP(AmplifyShadersSpriteExperimentalShapeOutside,_MainTex_ST);
				float2 uv_MainTex = IN.texCoord0.xy * _MainTex_ST_Instance.xy + _MainTex_ST_Instance.zw;
				float4 tex2DNode315 = tex2D( _MainTex, uv_MainTex );
				float3 temp_output_12_0_g1 = tex2DNode315.rgb;
				float dotResult28_g1 = dot( float3(0.2126729,0.7151522,0.072175) , temp_output_12_0_g1 );
				float3 temp_cast_0 = (dotResult28_g1).xxx;
				float temp_output_21_0_g1 = _Saturation;
				float3 lerpResult31_g1 = lerp( temp_cast_0 , temp_output_12_0_g1 , temp_output_21_0_g1);
				float _Emissive_Instance = UNITY_ACCESS_INSTANCED_PROP(AmplifyShadersSpriteExperimentalShapeOutside,_Emissive);
				float mulTime392 = _TimeParameters.x * _VoroSpeed;
				float time390 = mulTime392;
				float2 voronoiSmoothId390 = 0;
				float3 ase_positionWS = IN.ase_texcoord3.xyz;
				float2 panner371 = ( 1.0 * _Time.y * _PanSpeed + ( ase_positionWS * _Tile ).xy);
				float2 coords390 = ( floor( panner371 ) / _Tile ) * 1.0;
				float2 id390 = 0;
				float2 uv390 = 0;
				float voroi390 = voronoi390( coords390, time390, id390, uv390, 0, voronoiSmoothId390 );
				float lerpResult399 = lerp( _StepMin , _StepMax , ( voroi390 * _VoroStrength ));
				float smoothstepResult370 = smoothstep( lerpResult399 , ( lerpResult399 + _StepSmooth ) , distance( (frac( panner371 )).xy , float2( 0.5,0.5 ) ));
				float4 appendResult338 = (float4(( lerpResult31_g1 * _Emissive_Instance ) , saturate( ( tex2DNode315.a - smoothstepResult370 ) )));
				
				float4 Color = appendResult338;

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

			#pragma multi_compile_instancing


			sampler2D _MainTex;
			UNITY_INSTANCING_BUFFER_START(AmplifyShadersSpriteExperimentalShapeOutside)
				UNITY_DEFINE_INSTANCED_PROP(float4, _MainTex_ST)
				UNITY_DEFINE_INSTANCED_PROP(float, _Emissive)
			UNITY_INSTANCING_BUFFER_END(AmplifyShadersSpriteExperimentalShapeOutside)
			CBUFFER_START( UnityPerMaterial )
			float2 _PanSpeed;
			float _Saturation;
			float _StepMin;
			float _StepMax;
			float _VoroSpeed;
			float _Tile;
			float _VoroStrength;
			float _StepSmooth;
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

					float2 voronoihash390( float2 p )
					{
						
						p = float2( dot( p, float2( 127.1, 311.7 ) ), dot( p, float2( 269.5, 183.3 ) ) );
						return frac( sin( p ) *43758.5453);
					}
			
					float voronoi390( float2 v, float time, inout float2 id, inout float2 mr, float smoothness, inout float2 smoothId )
					{
						float2 n = floor( v );
						float2 f = frac( v );
						float F1 = 8.0;
						float F2 = 8.0; float2 mg = 0;
						for ( int j = -1; j <= 1; j++ )
						{
							for ( int i = -1; i <= 1; i++ )
						 	{
						 		float2 g = float2( i, j );
						 		float2 o = voronoihash390( n + g );
								o = ( sin( time + o * 6.2831 ) * 0.5 + 0.5 ); float2 r = f - g - o;
								float d = 0.5 * dot( r, r );
						 		if( d<F1 ) {
						 			F2 = F1;
						 			F1 = d; mg = g; mr = r; id = o;
						 		} else if( d<F2 ) {
						 			F2 = d;
						
						 		}
						 	}
						}
						return F1;
					}
			

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

				float4 _MainTex_ST_Instance = UNITY_ACCESS_INSTANCED_PROP(AmplifyShadersSpriteExperimentalShapeOutside,_MainTex_ST);
				float2 uv_MainTex = IN.texCoord0.xy * _MainTex_ST_Instance.xy + _MainTex_ST_Instance.zw;
				float4 tex2DNode315 = tex2D( _MainTex, uv_MainTex );
				float3 temp_output_12_0_g1 = tex2DNode315.rgb;
				float dotResult28_g1 = dot( float3(0.2126729,0.7151522,0.072175) , temp_output_12_0_g1 );
				float3 temp_cast_0 = (dotResult28_g1).xxx;
				float temp_output_21_0_g1 = _Saturation;
				float3 lerpResult31_g1 = lerp( temp_cast_0 , temp_output_12_0_g1 , temp_output_21_0_g1);
				float _Emissive_Instance = UNITY_ACCESS_INSTANCED_PROP(AmplifyShadersSpriteExperimentalShapeOutside,_Emissive);
				float mulTime392 = _TimeParameters.x * _VoroSpeed;
				float time390 = mulTime392;
				float2 voronoiSmoothId390 = 0;
				float3 ase_positionWS = IN.ase_texcoord3.xyz;
				float2 panner371 = ( 1.0 * _Time.y * _PanSpeed + ( ase_positionWS * _Tile ).xy);
				float2 coords390 = ( floor( panner371 ) / _Tile ) * 1.0;
				float2 id390 = 0;
				float2 uv390 = 0;
				float voroi390 = voronoi390( coords390, time390, id390, uv390, 0, voronoiSmoothId390 );
				float lerpResult399 = lerp( _StepMin , _StepMax , ( voroi390 * _VoroStrength ));
				float smoothstepResult370 = smoothstep( lerpResult399 , ( lerpResult399 + _StepSmooth ) , distance( (frac( panner371 )).xy , float2( 0.5,0.5 ) ));
				float4 appendResult338 = (float4(( lerpResult31_g1 * _Emissive_Instance ) , saturate( ( tex2DNode315.a - smoothstepResult370 ) )));
				
				float4 Color = appendResult338;

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
			UNITY_INSTANCING_BUFFER_START(AmplifyShadersSpriteExperimentalShapeOutside)
				UNITY_DEFINE_INSTANCED_PROP(float4, _MainTex_ST)
				UNITY_DEFINE_INSTANCED_PROP(float, _Emissive)
			UNITY_INSTANCING_BUFFER_END(AmplifyShadersSpriteExperimentalShapeOutside)
			CBUFFER_START( UnityPerMaterial )
			float2 _PanSpeed;
			float _Saturation;
			float _StepMin;
			float _StepMax;
			float _VoroSpeed;
			float _Tile;
			float _VoroStrength;
			float _StepSmooth;
			CBUFFER_END


            struct VertexInput
			{
				float3 positionOS : POSITION;
				float3 normal : NORMAL;
				float4 tangent : TANGENT;
				float4 ase_texcoord : TEXCOORD0;
				UNITY_VERTEX_INPUT_INSTANCE_ID
			};

			struct VertexOutput
			{
				float4 positionCS : SV_POSITION;
				float4 ase_texcoord : TEXCOORD0;
				float4 ase_texcoord1 : TEXCOORD1;
				UNITY_VERTEX_INPUT_INSTANCE_ID
			};

            int _ObjectId;
            int _PassValue;

					float2 voronoihash390( float2 p )
					{
						
						p = float2( dot( p, float2( 127.1, 311.7 ) ), dot( p, float2( 269.5, 183.3 ) ) );
						return frac( sin( p ) *43758.5453);
					}
			
					float voronoi390( float2 v, float time, inout float2 id, inout float2 mr, float smoothness, inout float2 smoothId )
					{
						float2 n = floor( v );
						float2 f = frac( v );
						float F1 = 8.0;
						float F2 = 8.0; float2 mg = 0;
						for ( int j = -1; j <= 1; j++ )
						{
							for ( int i = -1; i <= 1; i++ )
						 	{
						 		float2 g = float2( i, j );
						 		float2 o = voronoihash390( n + g );
								o = ( sin( time + o * 6.2831 ) * 0.5 + 0.5 ); float2 r = f - g - o;
								float d = 0.5 * dot( r, r );
						 		if( d<F1 ) {
						 			F2 = F1;
						 			F1 = d; mg = g; mr = r; id = o;
						 		} else if( d<F2 ) {
						 			F2 = d;
						
						 		}
						 	}
						}
						return F1;
					}
			

			VertexOutput vert(VertexInput v )
			{
				VertexOutput o = (VertexOutput)0;

				UNITY_SETUP_INSTANCE_ID(v);
				UNITY_TRANSFER_INSTANCE_ID(v, o);
				UNITY_INITIALIZE_VERTEX_OUTPUT_STEREO(o);

				float3 ase_positionWS = TransformObjectToWorld( ( v.positionOS ).xyz );
				o.ase_texcoord1.xyz = ase_positionWS;
				
				o.ase_texcoord.xy = v.ase_texcoord.xy;
				
				//setting value to unused interpolator channels and avoid initialization warnings
				o.ase_texcoord.zw = 0;
				o.ase_texcoord1.w = 0;
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
				float4 _MainTex_ST_Instance = UNITY_ACCESS_INSTANCED_PROP(AmplifyShadersSpriteExperimentalShapeOutside,_MainTex_ST);
				float2 uv_MainTex = IN.ase_texcoord.xy * _MainTex_ST_Instance.xy + _MainTex_ST_Instance.zw;
				float4 tex2DNode315 = tex2D( _MainTex, uv_MainTex );
				float3 temp_output_12_0_g1 = tex2DNode315.rgb;
				float dotResult28_g1 = dot( float3(0.2126729,0.7151522,0.072175) , temp_output_12_0_g1 );
				float3 temp_cast_0 = (dotResult28_g1).xxx;
				float temp_output_21_0_g1 = _Saturation;
				float3 lerpResult31_g1 = lerp( temp_cast_0 , temp_output_12_0_g1 , temp_output_21_0_g1);
				float _Emissive_Instance = UNITY_ACCESS_INSTANCED_PROP(AmplifyShadersSpriteExperimentalShapeOutside,_Emissive);
				float mulTime392 = _TimeParameters.x * _VoroSpeed;
				float time390 = mulTime392;
				float2 voronoiSmoothId390 = 0;
				float3 ase_positionWS = IN.ase_texcoord1.xyz;
				float2 panner371 = ( 1.0 * _Time.y * _PanSpeed + ( ase_positionWS * _Tile ).xy);
				float2 coords390 = ( floor( panner371 ) / _Tile ) * 1.0;
				float2 id390 = 0;
				float2 uv390 = 0;
				float voroi390 = voronoi390( coords390, time390, id390, uv390, 0, voronoiSmoothId390 );
				float lerpResult399 = lerp( _StepMin , _StepMax , ( voroi390 * _VoroStrength ));
				float smoothstepResult370 = smoothstep( lerpResult399 , ( lerpResult399 + _StepSmooth ) , distance( (frac( panner371 )).xy , float2( 0.5,0.5 ) ));
				float4 appendResult338 = (float4(( lerpResult31_g1 * _Emissive_Instance ) , saturate( ( tex2DNode315.a - smoothstepResult370 ) )));
				
				float4 Color = appendResult338;

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
			UNITY_INSTANCING_BUFFER_START(AmplifyShadersSpriteExperimentalShapeOutside)
				UNITY_DEFINE_INSTANCED_PROP(float4, _MainTex_ST)
				UNITY_DEFINE_INSTANCED_PROP(float, _Emissive)
			UNITY_INSTANCING_BUFFER_END(AmplifyShadersSpriteExperimentalShapeOutside)
			CBUFFER_START( UnityPerMaterial )
			float2 _PanSpeed;
			float _Saturation;
			float _StepMin;
			float _StepMax;
			float _VoroSpeed;
			float _Tile;
			float _VoroStrength;
			float _StepSmooth;
			CBUFFER_END


            struct VertexInput
			{
				float3 positionOS : POSITION;
				float3 normal : NORMAL;
				float4 tangent : TANGENT;
				float4 ase_texcoord : TEXCOORD0;
				UNITY_VERTEX_INPUT_INSTANCE_ID
			};

			struct VertexOutput
			{
				float4 positionCS : SV_POSITION;
				float4 ase_texcoord : TEXCOORD0;
				float4 ase_texcoord1 : TEXCOORD1;
				UNITY_VERTEX_INPUT_INSTANCE_ID
			};

            float4 _SelectionID;

					float2 voronoihash390( float2 p )
					{
						
						p = float2( dot( p, float2( 127.1, 311.7 ) ), dot( p, float2( 269.5, 183.3 ) ) );
						return frac( sin( p ) *43758.5453);
					}
			
					float voronoi390( float2 v, float time, inout float2 id, inout float2 mr, float smoothness, inout float2 smoothId )
					{
						float2 n = floor( v );
						float2 f = frac( v );
						float F1 = 8.0;
						float F2 = 8.0; float2 mg = 0;
						for ( int j = -1; j <= 1; j++ )
						{
							for ( int i = -1; i <= 1; i++ )
						 	{
						 		float2 g = float2( i, j );
						 		float2 o = voronoihash390( n + g );
								o = ( sin( time + o * 6.2831 ) * 0.5 + 0.5 ); float2 r = f - g - o;
								float d = 0.5 * dot( r, r );
						 		if( d<F1 ) {
						 			F2 = F1;
						 			F1 = d; mg = g; mr = r; id = o;
						 		} else if( d<F2 ) {
						 			F2 = d;
						
						 		}
						 	}
						}
						return F1;
					}
			

			VertexOutput vert(VertexInput v  )
			{
				VertexOutput o = (VertexOutput)0;

				UNITY_SETUP_INSTANCE_ID(v);
				UNITY_TRANSFER_INSTANCE_ID(v, o);
				UNITY_INITIALIZE_VERTEX_OUTPUT_STEREO(o);

				float3 ase_positionWS = TransformObjectToWorld( ( v.positionOS ).xyz );
				o.ase_texcoord1.xyz = ase_positionWS;
				
				o.ase_texcoord.xy = v.ase_texcoord.xy;
				
				//setting value to unused interpolator channels and avoid initialization warnings
				o.ase_texcoord.zw = 0;
				o.ase_texcoord1.w = 0;
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
				float4 _MainTex_ST_Instance = UNITY_ACCESS_INSTANCED_PROP(AmplifyShadersSpriteExperimentalShapeOutside,_MainTex_ST);
				float2 uv_MainTex = IN.ase_texcoord.xy * _MainTex_ST_Instance.xy + _MainTex_ST_Instance.zw;
				float4 tex2DNode315 = tex2D( _MainTex, uv_MainTex );
				float3 temp_output_12_0_g1 = tex2DNode315.rgb;
				float dotResult28_g1 = dot( float3(0.2126729,0.7151522,0.072175) , temp_output_12_0_g1 );
				float3 temp_cast_0 = (dotResult28_g1).xxx;
				float temp_output_21_0_g1 = _Saturation;
				float3 lerpResult31_g1 = lerp( temp_cast_0 , temp_output_12_0_g1 , temp_output_21_0_g1);
				float _Emissive_Instance = UNITY_ACCESS_INSTANCED_PROP(AmplifyShadersSpriteExperimentalShapeOutside,_Emissive);
				float mulTime392 = _TimeParameters.x * _VoroSpeed;
				float time390 = mulTime392;
				float2 voronoiSmoothId390 = 0;
				float3 ase_positionWS = IN.ase_texcoord1.xyz;
				float2 panner371 = ( 1.0 * _Time.y * _PanSpeed + ( ase_positionWS * _Tile ).xy);
				float2 coords390 = ( floor( panner371 ) / _Tile ) * 1.0;
				float2 id390 = 0;
				float2 uv390 = 0;
				float voroi390 = voronoi390( coords390, time390, id390, uv390, 0, voronoiSmoothId390 );
				float lerpResult399 = lerp( _StepMin , _StepMax , ( voroi390 * _VoroStrength ));
				float smoothstepResult370 = smoothstep( lerpResult399 , ( lerpResult399 + _StepSmooth ) , distance( (frac( panner371 )).xy , float2( 0.5,0.5 ) ));
				float4 appendResult338 = (float4(( lerpResult31_g1 * _Emissive_Instance ) , saturate( ( tex2DNode315.a - smoothstepResult370 ) )));
				
				float4 Color = appendResult338;
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
Node;AmplifyShaderEditor.WorldPosInputsNode;361;-80,-304;Inherit;True;0;4;FLOAT3;0;FLOAT;1;FLOAT;2;FLOAT;3
Node;AmplifyShaderEditor.RangedFloatNode;363;-16,-64;Inherit;False;Property;_Tile;Tile;10;0;Create;True;0;0;0;False;0;False;1;2.5;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;362;176,-304;Inherit;False;2;2;0;FLOAT3;0,0,0;False;1;FLOAT;0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.Vector2Node;373;208,-192;Inherit;False;Property;_PanSpeed;PanSpeed;11;0;Create;True;0;0;0;False;0;False;0.1,0;0.05,0.01;0;3;FLOAT2;0;FLOAT;1;FLOAT;2
Node;AmplifyShaderEditor.PannerNode;371;400,-256;Inherit;True;3;0;FLOAT2;0,0;False;2;FLOAT2;0,0;False;1;FLOAT;1;False;1;FLOAT2;0
Node;AmplifyShaderEditor.RangedFloatNode;391;256,288;Inherit;False;Property;_VoroSpeed;VoroSpeed;15;0;Create;True;0;0;0;False;0;False;1;1;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.FloorOpNode;395;656,-112;Inherit;False;1;0;FLOAT2;0,0;False;1;FLOAT2;0
Node;AmplifyShaderEditor.SimpleTimeNode;392;416,288;Inherit;False;1;0;FLOAT;1;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleDivideOpNode;396;768,-48;Inherit;False;2;0;FLOAT2;0,0;False;1;FLOAT;0;False;1;FLOAT2;0
Node;AmplifyShaderEditor.VoronoiNode;390;624,176;Inherit;True;0;0;1;0;1;False;1;False;False;False;4;0;FLOAT2;0,0;False;1;FLOAT;0;False;2;FLOAT;1;False;3;FLOAT;0;False;3;FLOAT;0;FLOAT2;1;FLOAT2;2
Node;AmplifyShaderEditor.RangedFloatNode;398;656,432;Inherit;False;Property;_VoroStrength;VoroStrength;16;0;Create;True;0;0;0;False;0;False;1;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.FractNode;387;656,-256;Inherit;False;1;0;FLOAT2;0,0;False;1;FLOAT2;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;397;832,240;Inherit;False;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;375;848,64;Inherit;False;Property;_StepMin;StepMin;12;0;Create;True;0;0;0;False;0;False;0.2;0.58;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;400;832,160;Inherit;False;Property;_StepMax;StepMax;13;0;Create;True;0;0;0;False;0;False;0.2;0.58;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;377;1024,272;Inherit;False;Property;_StepSmooth;StepSmooth;14;0;Create;True;0;0;0;False;0;False;0.02;0.01;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.ComponentMaskNode;389;800,-256;Inherit;False;True;True;False;True;1;0;FLOAT2;0,0;False;1;FLOAT2;0
Node;AmplifyShaderEditor.LerpOp;399;1056,128;Inherit;False;3;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleAddOpNode;376;1280,208;Inherit;True;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.DistanceOpNode;388;1072,-240;Inherit;True;2;0;FLOAT2;0,0;False;1;FLOAT2;0.5,0.5;False;1;FLOAT;0
Node;AmplifyShaderEditor.SamplerNode;315;1664,-352;Inherit;True;Property;_MainTex;MainTex;0;1;[PerRendererData];Create;True;0;0;0;False;0;False;-1;None;None;True;0;False;white;Auto;False;Object;-1;Auto;Texture2D;8;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;6;FLOAT;0;False;7;SAMPLERSTATE;;False;6;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4;FLOAT3;5
Node;AmplifyShaderEditor.RangedFloatNode;356;1664,-432;Inherit;False;Property;_Saturation;Saturation;9;0;Create;True;0;0;0;False;0;False;0;0;0;1;0;1;FLOAT;0
Node;AmplifyShaderEditor.SmoothstepOpNode;370;1536,96;Inherit;True;3;0;FLOAT;0;False;1;FLOAT;0.2;False;2;FLOAT;0.22;False;1;FLOAT;0
Node;AmplifyShaderEditor.FunctionNode;355;2000,-448;Inherit;False;Saturation;-1;;1;4f383aa3b2a7ef640be83276d286e709;1,51,0;2;12;FLOAT3;0,0,0;False;21;FLOAT;0.5;False;1;FLOAT3;0
Node;AmplifyShaderEditor.RangedFloatNode;360;2176,-336;Inherit;False;InstancedProperty;_Emissive;Emissive;8;0;Create;True;0;0;0;False;0;False;2;3;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleSubtractOpNode;385;2325.663,-184.4832;Inherit;False;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;359;2336,-448;Inherit;False;2;2;0;FLOAT3;0,0,0;False;1;FLOAT;0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.SaturateNode;386;2509.663,-92.48322;Inherit;False;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;320;1248,-576;Inherit;False;InstancedProperty;_UV_Rotate;UV_Rotate;1;0;Create;True;0;0;0;False;0;False;0;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.TextureCoordinatesNode;316;1312,-784;Inherit;False;0;-1;2;3;2;SAMPLER2D;;False;0;FLOAT2;1,1;False;1;FLOAT2;0,0;False;5;FLOAT2;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.PiNode;321;1392,-576;Inherit;False;1;0;FLOAT;1;False;1;FLOAT;0
Node;AmplifyShaderEditor.RotatorNode;318;1584,-784;Inherit;False;3;0;FLOAT2;0,0;False;1;FLOAT2;0.5,0.5;False;2;FLOAT;1;False;1;FLOAT2;0
Node;AmplifyShaderEditor.Vector2Node;343;1712,-624;Inherit;False;InstancedProperty;_Offset;Offset;4;0;Create;True;0;0;0;False;0;False;0,0;0,0;0;3;FLOAT2;0;FLOAT;1;FLOAT;2
Node;AmplifyShaderEditor.SimpleAddOpNode;322;1920,-784;Inherit;False;2;2;0;FLOAT2;0,0;False;1;FLOAT2;0,0;False;1;FLOAT2;0
Node;AmplifyShaderEditor.RangedFloatNode;350;1792,-1152;Inherit;False;InstancedProperty;_WaveSpeed;WaveSpeed;6;0;Create;True;0;0;0;False;0;False;0;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.BreakToComponentsNode;342;2112,-784;Inherit;False;FLOAT2;1;0;FLOAT2;0,0;False;16;FLOAT;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4;FLOAT;5;FLOAT;6;FLOAT;7;FLOAT;8;FLOAT;9;FLOAT;10;FLOAT;11;FLOAT;12;FLOAT;13;FLOAT;14;FLOAT;15
Node;AmplifyShaderEditor.SimpleTimeNode;349;1942.732,-1153.46;Inherit;False;1;0;FLOAT;1;False;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;346;2096,-1008;Inherit;False;InstancedProperty;_Float0;WaveScale;5;0;Create;True;0;0;0;False;0;False;1;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleAddOpNode;348;2192,-1184;Inherit;False;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;345;2272,-1008;Inherit;False;2;2;0;FLOAT;0;False;1;FLOAT;5;False;1;FLOAT;0
Node;AmplifyShaderEditor.SinOpNode;344;2432,-1008;Inherit;True;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.ScaleAndOffsetNode;353;2608,-1008;Inherit;False;3;0;FLOAT;0;False;1;FLOAT;0.5;False;2;FLOAT;0.5;False;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;352;2672,-880;Inherit;False;Property;_WaveStrength;WaveStrength;7;0;Create;True;0;0;0;False;0;False;0;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;351;2864,-944;Inherit;False;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleAddOpNode;347;3136,-800;Inherit;False;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.GetLocalVarNode;332;3072,-672;Inherit;False;330;noise;1;0;OBJECT;;False;1;FLOAT;0
Node;AmplifyShaderEditor.SmoothstepOpNode;331;3280,-720;Inherit;False;3;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;1;False;1;FLOAT;0
Node;AmplifyShaderEditor.RegisterLocalVarNode;333;3504,-720;Inherit;False;dim;-1;True;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.DynamicAppendNode;338;2544.527,-247.6526;Inherit;False;FLOAT4;4;0;FLOAT3;0,0,0;False;1;FLOAT;0;False;2;FLOAT;0;False;3;FLOAT;0;False;1;FLOAT4;0
Node;AmplifyShaderEditor.SamplerNode;323;1696,784;Inherit;True;Property;_NoiseTex;NoiseTex;2;0;Create;True;0;0;0;False;0;False;-1;2b7311c45ec18d14ca7aa0825cfcff7b;2b7311c45ec18d14ca7aa0825cfcff7b;True;0;False;red;Auto;False;Object;-1;Auto;Texture2D;8;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;6;FLOAT;0;False;7;SAMPLERSTATE;;False;6;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4;FLOAT3;5
Node;AmplifyShaderEditor.RangedFloatNode;326;1808,976;Inherit;False;InstancedProperty;_NoiseStrength;NoiseStrength;3;0;Create;True;0;0;0;False;0;False;0;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;329;2032,832;Inherit;False;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.RegisterLocalVarNode;330;2192,832;Inherit;False;noise;-1;True;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.TextureCoordinatesNode;324;1488,784;Inherit;False;0;323;2;3;2;SAMPLER2D;;False;0;FLOAT2;1,1;False;1;FLOAT2;0,0;False;5;FLOAT2;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.TemplateMultiPassMasterNode;72;2832,-272;Float;False;True;-1;2;AmplifyShaderEditor.MaterialInspector;0;15;AmplifyShaders/Sprite ExperimentalShape Outside;cf964e524c8e69742b1d21fbe2ebcc4a;True;Sprite Unlit;0;0;Sprite Unlit;4;False;False;False;False;False;False;False;False;False;False;False;False;False;False;True;2;False;;False;False;False;False;False;False;False;False;False;False;False;False;False;False;True;3;RenderPipeline=UniversalPipeline;RenderType=Transparent=RenderType;Queue=Transparent=Queue=0;True;0;True;12;all;0;False;True;2;5;False;;10;False;;3;1;False;;10;False;;False;False;False;False;False;False;False;False;False;False;False;False;False;False;True;True;True;True;True;0;False;;False;False;False;False;False;False;False;True;False;0;False;;255;False;;255;False;;0;False;;0;False;;0;False;;0;False;;0;False;;0;False;;0;False;;0;False;;False;True;2;False;;True;3;False;;True;True;0;False;;0;False;;True;1;LightMode=Universal2D;False;False;0;Hidden/InternalErrorShader;0;0;Standard;3;Vertex Position;1;0;Debug Display;0;0;External Alpha;0;0;0;4;True;True;True;True;False;;False;0
Node;AmplifyShaderEditor.TemplateMultiPassMasterNode;73;771.9996,-195.7002;Float;False;False;-1;2;AmplifyShaderEditor.MaterialInspector;0;15;New Amplify Shader;cf964e524c8e69742b1d21fbe2ebcc4a;True;Sprite Unlit Forward;0;1;Sprite Unlit Forward;0;False;False;False;False;False;False;False;False;False;False;False;False;False;False;True;2;False;;False;False;False;False;False;False;False;False;False;False;False;False;False;False;True;3;RenderPipeline=UniversalPipeline;RenderType=Transparent=RenderType;Queue=Transparent=Queue=0;True;0;True;12;all;0;False;True;2;5;False;;10;False;;3;1;False;;10;False;;False;False;False;False;False;False;False;False;False;False;False;False;False;False;True;True;True;True;True;0;False;;False;False;False;False;False;False;False;True;False;0;False;;255;False;;255;False;;0;False;;0;False;;0;False;;0;False;;0;False;;0;False;;0;False;;0;False;;False;True;2;False;;True;3;False;;True;True;0;False;;0;False;;True;1;LightMode=UniversalForward;False;False;0;Hidden/InternalErrorShader;0;0;Standard;0;False;0
Node;AmplifyShaderEditor.TemplateMultiPassMasterNode;74;771.9996,-195.7002;Float;False;False;-1;2;AmplifyShaderEditor.MaterialInspector;0;15;New Amplify Shader;cf964e524c8e69742b1d21fbe2ebcc4a;True;SceneSelectionPass;0;2;SceneSelectionPass;0;False;False;False;False;False;False;False;False;False;False;False;False;False;False;True;2;False;;False;False;False;False;False;False;False;False;False;False;False;False;False;False;True;3;RenderPipeline=UniversalPipeline;RenderType=Transparent=RenderType;Queue=Transparent=Queue=0;True;0;True;12;all;0;False;False;False;False;False;False;False;False;False;False;False;False;False;False;True;2;False;;False;False;False;False;False;False;False;False;False;False;False;False;False;False;True;1;LightMode=SceneSelectionPass;False;False;0;Hidden/InternalErrorShader;0;0;Standard;0;False;0
Node;AmplifyShaderEditor.TemplateMultiPassMasterNode;75;771.9996,-195.7002;Float;False;False;-1;2;AmplifyShaderEditor.MaterialInspector;0;15;New Amplify Shader;cf964e524c8e69742b1d21fbe2ebcc4a;True;ScenePickingPass;0;3;ScenePickingPass;0;False;False;False;False;False;False;False;False;False;False;False;False;False;False;True;2;False;;False;False;False;False;False;False;False;False;False;False;False;False;False;False;True;3;RenderPipeline=UniversalPipeline;RenderType=Transparent=RenderType;Queue=Transparent=Queue=0;True;0;True;12;all;0;False;False;False;False;False;False;False;False;False;False;False;False;False;False;True;2;False;;False;False;False;False;False;False;False;False;False;False;False;False;False;False;True;1;LightMode=Picking;False;False;0;Hidden/InternalErrorShader;0;0;Standard;0;False;0
WireConnection;362;0;361;0
WireConnection;362;1;363;0
WireConnection;371;0;362;0
WireConnection;371;2;373;0
WireConnection;395;0;371;0
WireConnection;392;0;391;0
WireConnection;396;0;395;0
WireConnection;396;1;363;0
WireConnection;390;0;396;0
WireConnection;390;1;392;0
WireConnection;387;0;371;0
WireConnection;397;0;390;0
WireConnection;397;1;398;0
WireConnection;389;0;387;0
WireConnection;399;0;375;0
WireConnection;399;1;400;0
WireConnection;399;2;397;0
WireConnection;376;0;399;0
WireConnection;376;1;377;0
WireConnection;388;0;389;0
WireConnection;370;0;388;0
WireConnection;370;1;399;0
WireConnection;370;2;376;0
WireConnection;355;12;315;5
WireConnection;355;21;356;0
WireConnection;385;0;315;4
WireConnection;385;1;370;0
WireConnection;359;0;355;0
WireConnection;359;1;360;0
WireConnection;386;0;385;0
WireConnection;321;0;320;0
WireConnection;318;0;316;0
WireConnection;318;2;321;0
WireConnection;322;0;318;0
WireConnection;322;1;343;0
WireConnection;342;0;322;0
WireConnection;349;0;350;0
WireConnection;348;0;342;0
WireConnection;348;1;349;0
WireConnection;345;0;348;0
WireConnection;345;1;346;0
WireConnection;344;0;345;0
WireConnection;353;0;344;0
WireConnection;351;0;353;0
WireConnection;351;1;352;0
WireConnection;347;0;351;0
WireConnection;347;1;342;1
WireConnection;331;0;347;0
WireConnection;331;1;332;0
WireConnection;333;0;331;0
WireConnection;338;0;359;0
WireConnection;338;3;386;0
WireConnection;323;1;324;0
WireConnection;329;0;323;1
WireConnection;329;1;326;0
WireConnection;330;0;329;0
WireConnection;72;1;338;0
ASEEND*/
//CHKSM=C9CCE9969444DFC52E094732512FD2078B2D7BFB