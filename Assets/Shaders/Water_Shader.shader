// Made with Amplify Shader Editor
// Available at the Unity Asset Store - http://u3d.as/y3X 
Shader "Water_Shader"
{
	Properties
	{
		[HideInInspector] _EmissionColor("Emission Color", Color) = (1,1,1,1)
		[HideInInspector] _AlphaCutoff("Alpha Cutoff ", Range(0, 1)) = 0.5
		[ASEBegin]_NoiseTexture("Noise Texture", 2D) = "white" {}
		_Direction("Direction", Vector) = (1,0,0,0)
		_Distance("Distance", Float) = 10
		_windDirection("windDirection", Vector) = (0,0,0,0)
		_Speed("Speed", Range( 0 , 0.02)) = 0.02
		_Alpha("Alpha", Range( 0 , 1)) = 0.5
		_WaterNoiseMultiplyer("Water Noise Multiplyer", Float) = 0.03
		_textureSize("Texture Size", Float) = 0
		_SurfaceColor("SurfaceColor", Color) = (0.07058824,0.8666667,0.8862746,1)
		_DeepColor("DeepColor", Color) = (0.0627451,0.3607843,0.5647059,1)
		_SoftSelect("Soft Select", Vector) = (0.47,3.32,0,0)
		_EdgeFoamScale("EdgeFoamScale", Float) = 0
		[HDR]_EdgeFoamColor("EdgeFoamColor", Color) = (1,1,1,1)
		_EdgePower("EdgePower", Float) = 1
		[HDR]_FoamColor("FoamColor", Color) = (1,1,1,1)
		_FoamAlpha("FoamAlpha", Float) = 0
		_FoamCuttoff("FoamCuttoff", Float) = 1
		_IceColor("IceColor", Color) = (1,1,1,1)
		_IceTextureScale("IceTextureScale", Float) = 0
		_IceStrength("IceStrength", Range( 0 , 1)) = 0
		_IceSmoothStep("IceSmoothStep", Vector) = (-1,1,0,0)
		_DepthSmoothStep("DepthSmoothStep", Vector) = (0,0,0,0)
		[ASEEnd]_MainTexture("MainTexture", 2D) = "white" {}

		//_TransmissionShadow( "Transmission Shadow", Range( 0, 1 ) ) = 0.5
		//_TransStrength( "Trans Strength", Range( 0, 50 ) ) = 1
		//_TransNormal( "Trans Normal Distortion", Range( 0, 1 ) ) = 0.5
		//_TransScattering( "Trans Scattering", Range( 1, 50 ) ) = 2
		//_TransDirect( "Trans Direct", Range( 0, 1 ) ) = 0.9
		//_TransAmbient( "Trans Ambient", Range( 0, 1 ) ) = 0.1
		//_TransShadow( "Trans Shadow", Range( 0, 1 ) ) = 0.5
		//_TessPhongStrength( "Tess Phong Strength", Range( 0, 1 ) ) = 0.5
		//_TessValue( "Tess Max Tessellation", Range( 1, 32 ) ) = 16
		//_TessMin( "Tess Min Distance", Float ) = 10
		//_TessMax( "Tess Max Distance", Float ) = 25
		//_TessEdgeLength ( "Tess Edge length", Range( 2, 50 ) ) = 16
		//_TessMaxDisp( "Tess Max Displacement", Float ) = 25
	}

	SubShader
	{
		LOD 0

		

		Tags { "RenderPipeline"="UniversalPipeline" "RenderType"="Transparent" "Queue"="Transparent" }
		Cull Back
		AlphaToMask Off
		
		HLSLINCLUDE
		#pragma target 2.0

		#pragma prefer_hlslcc gles
		#pragma exclude_renderers d3d11_9x 

		#ifndef ASE_TESS_FUNCS
		#define ASE_TESS_FUNCS
		float4 FixedTess( float tessValue )
		{
			return tessValue;
		}
		
		float CalcDistanceTessFactor (float4 vertex, float minDist, float maxDist, float tess, float4x4 o2w, float3 cameraPos )
		{
			float3 wpos = mul(o2w,vertex).xyz;
			float dist = distance (wpos, cameraPos);
			float f = clamp(1.0 - (dist - minDist) / (maxDist - minDist), 0.01, 1.0) * tess;
			return f;
		}

		float4 CalcTriEdgeTessFactors (float3 triVertexFactors)
		{
			float4 tess;
			tess.x = 0.5 * (triVertexFactors.y + triVertexFactors.z);
			tess.y = 0.5 * (triVertexFactors.x + triVertexFactors.z);
			tess.z = 0.5 * (triVertexFactors.x + triVertexFactors.y);
			tess.w = (triVertexFactors.x + triVertexFactors.y + triVertexFactors.z) / 3.0f;
			return tess;
		}

		float CalcEdgeTessFactor (float3 wpos0, float3 wpos1, float edgeLen, float3 cameraPos, float4 scParams )
		{
			float dist = distance (0.5 * (wpos0+wpos1), cameraPos);
			float len = distance(wpos0, wpos1);
			float f = max(len * scParams.y / (edgeLen * dist), 1.0);
			return f;
		}

		float DistanceFromPlane (float3 pos, float4 plane)
		{
			float d = dot (float4(pos,1.0f), plane);
			return d;
		}

		bool WorldViewFrustumCull (float3 wpos0, float3 wpos1, float3 wpos2, float cullEps, float4 planes[6] )
		{
			float4 planeTest;
			planeTest.x = (( DistanceFromPlane(wpos0, planes[0]) > -cullEps) ? 1.0f : 0.0f ) +
						  (( DistanceFromPlane(wpos1, planes[0]) > -cullEps) ? 1.0f : 0.0f ) +
						  (( DistanceFromPlane(wpos2, planes[0]) > -cullEps) ? 1.0f : 0.0f );
			planeTest.y = (( DistanceFromPlane(wpos0, planes[1]) > -cullEps) ? 1.0f : 0.0f ) +
						  (( DistanceFromPlane(wpos1, planes[1]) > -cullEps) ? 1.0f : 0.0f ) +
						  (( DistanceFromPlane(wpos2, planes[1]) > -cullEps) ? 1.0f : 0.0f );
			planeTest.z = (( DistanceFromPlane(wpos0, planes[2]) > -cullEps) ? 1.0f : 0.0f ) +
						  (( DistanceFromPlane(wpos1, planes[2]) > -cullEps) ? 1.0f : 0.0f ) +
						  (( DistanceFromPlane(wpos2, planes[2]) > -cullEps) ? 1.0f : 0.0f );
			planeTest.w = (( DistanceFromPlane(wpos0, planes[3]) > -cullEps) ? 1.0f : 0.0f ) +
						  (( DistanceFromPlane(wpos1, planes[3]) > -cullEps) ? 1.0f : 0.0f ) +
						  (( DistanceFromPlane(wpos2, planes[3]) > -cullEps) ? 1.0f : 0.0f );
			return !all (planeTest);
		}

		float4 DistanceBasedTess( float4 v0, float4 v1, float4 v2, float tess, float minDist, float maxDist, float4x4 o2w, float3 cameraPos )
		{
			float3 f;
			f.x = CalcDistanceTessFactor (v0,minDist,maxDist,tess,o2w,cameraPos);
			f.y = CalcDistanceTessFactor (v1,minDist,maxDist,tess,o2w,cameraPos);
			f.z = CalcDistanceTessFactor (v2,minDist,maxDist,tess,o2w,cameraPos);

			return CalcTriEdgeTessFactors (f);
		}

		float4 EdgeLengthBasedTess( float4 v0, float4 v1, float4 v2, float edgeLength, float4x4 o2w, float3 cameraPos, float4 scParams )
		{
			float3 pos0 = mul(o2w,v0).xyz;
			float3 pos1 = mul(o2w,v1).xyz;
			float3 pos2 = mul(o2w,v2).xyz;
			float4 tess;
			tess.x = CalcEdgeTessFactor (pos1, pos2, edgeLength, cameraPos, scParams);
			tess.y = CalcEdgeTessFactor (pos2, pos0, edgeLength, cameraPos, scParams);
			tess.z = CalcEdgeTessFactor (pos0, pos1, edgeLength, cameraPos, scParams);
			tess.w = (tess.x + tess.y + tess.z) / 3.0f;
			return tess;
		}

		float4 EdgeLengthBasedTessCull( float4 v0, float4 v1, float4 v2, float edgeLength, float maxDisplacement, float4x4 o2w, float3 cameraPos, float4 scParams, float4 planes[6] )
		{
			float3 pos0 = mul(o2w,v0).xyz;
			float3 pos1 = mul(o2w,v1).xyz;
			float3 pos2 = mul(o2w,v2).xyz;
			float4 tess;

			if (WorldViewFrustumCull(pos0, pos1, pos2, maxDisplacement, planes))
			{
				tess = 0.0f;
			}
			else
			{
				tess.x = CalcEdgeTessFactor (pos1, pos2, edgeLength, cameraPos, scParams);
				tess.y = CalcEdgeTessFactor (pos2, pos0, edgeLength, cameraPos, scParams);
				tess.z = CalcEdgeTessFactor (pos0, pos1, edgeLength, cameraPos, scParams);
				tess.w = (tess.x + tess.y + tess.z) / 3.0f;
			}
			return tess;
		}
		#endif //ASE_TESS_FUNCS

		ENDHLSL

		
		Pass
		{
			
			Name "Forward"
			Tags { "LightMode"="UniversalForward" }
			
			Blend SrcAlpha OneMinusSrcAlpha, One OneMinusSrcAlpha
			ZWrite Off
			ZTest LEqual
			Offset 0 , 0
			ColorMask RGBA
			

			HLSLPROGRAM
			
			#define _NORMAL_DROPOFF_TS 1
			#pragma multi_compile _ LOD_FADE_CROSSFADE
			#pragma multi_compile_fog
			#define ASE_FOG 1
			#define ASE_SRP_VERSION 100400
			#define REQUIRE_DEPTH_TEXTURE 1

			
			#pragma multi_compile _ _SCREEN_SPACE_OCCLUSION
			#pragma multi_compile _ _MAIN_LIGHT_SHADOWS
			#pragma multi_compile _ _MAIN_LIGHT_SHADOWS_CASCADE
			#pragma multi_compile _ _ADDITIONAL_LIGHTS_VERTEX _ADDITIONAL_LIGHTS _ADDITIONAL_OFF
			#pragma multi_compile _ _ADDITIONAL_LIGHT_SHADOWS
			#pragma multi_compile _ _SHADOWS_SOFT
			#pragma multi_compile _ _MIXED_LIGHTING_SUBTRACTIVE
			
			#pragma multi_compile _ LIGHTMAP_SHADOW_MIXING
			#pragma multi_compile _ SHADOWS_SHADOWMASK

			#pragma multi_compile _ DIRLIGHTMAP_COMBINED
			#pragma multi_compile _ LIGHTMAP_ON

			#pragma vertex vert
			#pragma fragment frag

			#define SHADERPASS_FORWARD

			#include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
			#include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Lighting.hlsl"
			#include "Packages/com.unity.render-pipelines.core/ShaderLibrary/Color.hlsl"
			#include "Packages/com.unity.render-pipelines.core/ShaderLibrary/UnityInstancing.hlsl"
			#include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/ShaderGraphFunctions.hlsl"
			
			#if ASE_SRP_VERSION <= 70108
			#define REQUIRES_VERTEX_SHADOW_COORD_INTERPOLATOR
			#endif

			#if defined(UNITY_INSTANCING_ENABLED) && defined(_TERRAIN_INSTANCED_PERPIXEL_NORMAL)
			    #define ENABLE_TERRAIN_PERPIXEL_NORMAL
			#endif

			#define ASE_NEEDS_FRAG_SCREEN_POSITION
			#define ASE_NEEDS_FRAG_WORLD_POSITION
			#pragma multi_compile_instancing


			struct VertexInput
			{
				float4 vertex : POSITION;
				float3 ase_normal : NORMAL;
				float4 ase_tangent : TANGENT;
				float4 texcoord1 : TEXCOORD1;
				float4 texcoord : TEXCOORD0;
				
				UNITY_VERTEX_INPUT_INSTANCE_ID
			};

			struct VertexOutput
			{
				float4 clipPos : SV_POSITION;
				float4 lightmapUVOrVertexSH : TEXCOORD0;
				half4 fogFactorAndVertexLight : TEXCOORD1;
				#if defined(REQUIRES_VERTEX_SHADOW_COORD_INTERPOLATOR)
				float4 shadowCoord : TEXCOORD2;
				#endif
				float4 tSpace0 : TEXCOORD3;
				float4 tSpace1 : TEXCOORD4;
				float4 tSpace2 : TEXCOORD5;
				#if defined(ASE_NEEDS_FRAG_SCREEN_POSITION)
				float4 screenPos : TEXCOORD6;
				#endif
				float4 ase_texcoord7 : TEXCOORD7;
				UNITY_VERTEX_INPUT_INSTANCE_ID
				UNITY_VERTEX_OUTPUT_STEREO
			};

			CBUFFER_START(UnityPerMaterial)
						#ifdef _TRANSMISSION_ASE
				float _TransmissionShadow;
			#endif
			#ifdef _TRANSLUCENCY_ASE
				float _TransStrength;
				float _TransNormal;
				float _TransScattering;
				float _TransDirect;
				float _TransAmbient;
				float _TransShadow;
			#endif
			#ifdef TESSELLATION_ON
				float _TessPhongStrength;
				float _TessValue;
				float _TessMin;
				float _TessMax;
				float _TessEdgeLength;
				float _TessMaxDisp;
			#endif
			CBUFFER_END
			uniform float4 _CameraDepthTexture_TexelSize;
			sampler2D _MainTexture;
			sampler2D _NoiseTexture;
			UNITY_INSTANCING_BUFFER_START(Water_Shader)
				UNITY_DEFINE_INSTANCED_PROP(float4, _SurfaceColor)
				UNITY_DEFINE_INSTANCED_PROP(float4, _DeepColor)
				UNITY_DEFINE_INSTANCED_PROP(float4, _IceColor)
				UNITY_DEFINE_INSTANCED_PROP(float4, _FoamColor)
				UNITY_DEFINE_INSTANCED_PROP(float4, _EdgeFoamColor)
				UNITY_DEFINE_INSTANCED_PROP(float2, _DepthSmoothStep)
				UNITY_DEFINE_INSTANCED_PROP(float2, _SoftSelect)
				UNITY_DEFINE_INSTANCED_PROP(float2, _windDirection)
				UNITY_DEFINE_INSTANCED_PROP(float2, _IceSmoothStep)
				UNITY_DEFINE_INSTANCED_PROP(float2, _Direction)
				UNITY_DEFINE_INSTANCED_PROP(float, _IceStrength)
				UNITY_DEFINE_INSTANCED_PROP(float, _IceTextureScale)
				UNITY_DEFINE_INSTANCED_PROP(float, _Speed)
				UNITY_DEFINE_INSTANCED_PROP(float, _FoamCuttoff)
				UNITY_DEFINE_INSTANCED_PROP(float, _Alpha)
				UNITY_DEFINE_INSTANCED_PROP(float, _WaterNoiseMultiplyer)
				UNITY_DEFINE_INSTANCED_PROP(float, _textureSize)
				UNITY_DEFINE_INSTANCED_PROP(float, _EdgePower)
				UNITY_DEFINE_INSTANCED_PROP(float, _Distance)
				UNITY_DEFINE_INSTANCED_PROP(float, _EdgeFoamScale)
				UNITY_DEFINE_INSTANCED_PROP(float, _FoamAlpha)
			UNITY_INSTANCING_BUFFER_END(Water_Shader)


			
			VertexOutput VertexFunction( VertexInput v  )
			{
				VertexOutput o = (VertexOutput)0;
				UNITY_SETUP_INSTANCE_ID(v);
				UNITY_TRANSFER_INSTANCE_ID(v, o);
				UNITY_INITIALIZE_VERTEX_OUTPUT_STEREO(o);

				o.ase_texcoord7.xy = v.texcoord.xy;
				
				//setting value to unused interpolator channels and avoid initialization warnings
				o.ase_texcoord7.zw = 0;
				#ifdef ASE_ABSOLUTE_VERTEX_POS
					float3 defaultVertexValue = v.vertex.xyz;
				#else
					float3 defaultVertexValue = float3(0, 0, 0);
				#endif
				float3 vertexValue = defaultVertexValue;
				#ifdef ASE_ABSOLUTE_VERTEX_POS
					v.vertex.xyz = vertexValue;
				#else
					v.vertex.xyz += vertexValue;
				#endif
				v.ase_normal = v.ase_normal;

				float3 positionWS = TransformObjectToWorld( v.vertex.xyz );
				float3 positionVS = TransformWorldToView( positionWS );
				float4 positionCS = TransformWorldToHClip( positionWS );

				VertexNormalInputs normalInput = GetVertexNormalInputs( v.ase_normal, v.ase_tangent );

				o.tSpace0 = float4( normalInput.normalWS, positionWS.x);
				o.tSpace1 = float4( normalInput.tangentWS, positionWS.y);
				o.tSpace2 = float4( normalInput.bitangentWS, positionWS.z);

				OUTPUT_LIGHTMAP_UV( v.texcoord1, unity_LightmapST, o.lightmapUVOrVertexSH.xy );
				OUTPUT_SH( normalInput.normalWS.xyz, o.lightmapUVOrVertexSH.xyz );

				#if defined(ENABLE_TERRAIN_PERPIXEL_NORMAL)
					o.lightmapUVOrVertexSH.zw = v.texcoord;
					o.lightmapUVOrVertexSH.xy = v.texcoord * unity_LightmapST.xy + unity_LightmapST.zw;
				#endif

				half3 vertexLight = VertexLighting( positionWS, normalInput.normalWS );
				#ifdef ASE_FOG
					half fogFactor = ComputeFogFactor( positionCS.z );
				#else
					half fogFactor = 0;
				#endif
				o.fogFactorAndVertexLight = half4(fogFactor, vertexLight);
				
				#if defined(REQUIRES_VERTEX_SHADOW_COORD_INTERPOLATOR)
				VertexPositionInputs vertexInput = (VertexPositionInputs)0;
				vertexInput.positionWS = positionWS;
				vertexInput.positionCS = positionCS;
				o.shadowCoord = GetShadowCoord( vertexInput );
				#endif
				
				o.clipPos = positionCS;
				#if defined(ASE_NEEDS_FRAG_SCREEN_POSITION)
				o.screenPos = ComputeScreenPos(positionCS);
				#endif
				return o;
			}
			
			#if defined(TESSELLATION_ON)
			struct VertexControl
			{
				float4 vertex : INTERNALTESSPOS;
				float3 ase_normal : NORMAL;
				float4 ase_tangent : TANGENT;
				float4 texcoord : TEXCOORD0;
				float4 texcoord1 : TEXCOORD1;
				
				UNITY_VERTEX_INPUT_INSTANCE_ID
			};

			struct TessellationFactors
			{
				float edge[3] : SV_TessFactor;
				float inside : SV_InsideTessFactor;
			};

			VertexControl vert ( VertexInput v )
			{
				VertexControl o;
				UNITY_SETUP_INSTANCE_ID(v);
				UNITY_TRANSFER_INSTANCE_ID(v, o);
				o.vertex = v.vertex;
				o.ase_normal = v.ase_normal;
				o.ase_tangent = v.ase_tangent;
				o.texcoord = v.texcoord;
				o.texcoord1 = v.texcoord1;
				
				return o;
			}

			TessellationFactors TessellationFunction (InputPatch<VertexControl,3> v)
			{
				TessellationFactors o;
				float4 tf = 1;
				float tessValue = _TessValue; float tessMin = _TessMin; float tessMax = _TessMax;
				float edgeLength = _TessEdgeLength; float tessMaxDisp = _TessMaxDisp;
				#if defined(ASE_FIXED_TESSELLATION)
				tf = FixedTess( tessValue );
				#elif defined(ASE_DISTANCE_TESSELLATION)
				tf = DistanceBasedTess(v[0].vertex, v[1].vertex, v[2].vertex, tessValue, tessMin, tessMax, GetObjectToWorldMatrix(), _WorldSpaceCameraPos );
				#elif defined(ASE_LENGTH_TESSELLATION)
				tf = EdgeLengthBasedTess(v[0].vertex, v[1].vertex, v[2].vertex, edgeLength, GetObjectToWorldMatrix(), _WorldSpaceCameraPos, _ScreenParams );
				#elif defined(ASE_LENGTH_CULL_TESSELLATION)
				tf = EdgeLengthBasedTessCull(v[0].vertex, v[1].vertex, v[2].vertex, edgeLength, tessMaxDisp, GetObjectToWorldMatrix(), _WorldSpaceCameraPos, _ScreenParams, unity_CameraWorldClipPlanes );
				#endif
				o.edge[0] = tf.x; o.edge[1] = tf.y; o.edge[2] = tf.z; o.inside = tf.w;
				return o;
			}

			[domain("tri")]
			[partitioning("fractional_odd")]
			[outputtopology("triangle_cw")]
			[patchconstantfunc("TessellationFunction")]
			[outputcontrolpoints(3)]
			VertexControl HullFunction(InputPatch<VertexControl, 3> patch, uint id : SV_OutputControlPointID)
			{
			   return patch[id];
			}

			[domain("tri")]
			VertexOutput DomainFunction(TessellationFactors factors, OutputPatch<VertexControl, 3> patch, float3 bary : SV_DomainLocation)
			{
				VertexInput o = (VertexInput) 0;
				o.vertex = patch[0].vertex * bary.x + patch[1].vertex * bary.y + patch[2].vertex * bary.z;
				o.ase_normal = patch[0].ase_normal * bary.x + patch[1].ase_normal * bary.y + patch[2].ase_normal * bary.z;
				o.ase_tangent = patch[0].ase_tangent * bary.x + patch[1].ase_tangent * bary.y + patch[2].ase_tangent * bary.z;
				o.texcoord = patch[0].texcoord * bary.x + patch[1].texcoord * bary.y + patch[2].texcoord * bary.z;
				o.texcoord1 = patch[0].texcoord1 * bary.x + patch[1].texcoord1 * bary.y + patch[2].texcoord1 * bary.z;
				
				#if defined(ASE_PHONG_TESSELLATION)
				float3 pp[3];
				for (int i = 0; i < 3; ++i)
					pp[i] = o.vertex.xyz - patch[i].ase_normal * (dot(o.vertex.xyz, patch[i].ase_normal) - dot(patch[i].vertex.xyz, patch[i].ase_normal));
				float phongStrength = _TessPhongStrength;
				o.vertex.xyz = phongStrength * (pp[0]*bary.x + pp[1]*bary.y + pp[2]*bary.z) + (1.0f-phongStrength) * o.vertex.xyz;
				#endif
				UNITY_TRANSFER_INSTANCE_ID(patch[0], o);
				return VertexFunction(o);
			}
			#else
			VertexOutput vert ( VertexInput v )
			{
				return VertexFunction( v );
			}
			#endif

			#if defined(ASE_EARLY_Z_DEPTH_OPTIMIZE)
				#define ASE_SV_DEPTH SV_DepthLessEqual  
			#else
				#define ASE_SV_DEPTH SV_Depth
			#endif

			half4 frag ( VertexOutput IN 
						#ifdef ASE_DEPTH_WRITE_ON
						,out float outputDepth : ASE_SV_DEPTH
						#endif
						 ) : SV_Target
			{
				UNITY_SETUP_INSTANCE_ID(IN);
				UNITY_SETUP_STEREO_EYE_INDEX_POST_VERTEX(IN);

				#ifdef LOD_FADE_CROSSFADE
					LODDitheringTransition( IN.clipPos.xyz, unity_LODFade.x );
				#endif

				#if defined(ENABLE_TERRAIN_PERPIXEL_NORMAL)
					float2 sampleCoords = (IN.lightmapUVOrVertexSH.zw / _TerrainHeightmapRecipSize.zw + 0.5f) * _TerrainHeightmapRecipSize.xy;
					float3 WorldNormal = TransformObjectToWorldNormal(normalize(SAMPLE_TEXTURE2D(_TerrainNormalmapTexture, sampler_TerrainNormalmapTexture, sampleCoords).rgb * 2 - 1));
					float3 WorldTangent = -cross(GetObjectToWorldMatrix()._13_23_33, WorldNormal);
					float3 WorldBiTangent = cross(WorldNormal, -WorldTangent);
				#else
					float3 WorldNormal = normalize( IN.tSpace0.xyz );
					float3 WorldTangent = IN.tSpace1.xyz;
					float3 WorldBiTangent = IN.tSpace2.xyz;
				#endif
				float3 WorldPosition = float3(IN.tSpace0.w,IN.tSpace1.w,IN.tSpace2.w);
				float3 WorldViewDirection = _WorldSpaceCameraPos.xyz  - WorldPosition;
				float4 ShadowCoords = float4( 0, 0, 0, 0 );
				#if defined(ASE_NEEDS_FRAG_SCREEN_POSITION)
				float4 ScreenPos = IN.screenPos;
				#endif

				#if defined(REQUIRES_VERTEX_SHADOW_COORD_INTERPOLATOR)
					ShadowCoords = IN.shadowCoord;
				#elif defined(MAIN_LIGHT_CALCULATE_SHADOWS)
					ShadowCoords = TransformWorldToShadowCoord( WorldPosition );
				#endif
	
				WorldViewDirection = SafeNormalize( WorldViewDirection );

				float4 _SurfaceColor_Instance = UNITY_ACCESS_INSTANCED_PROP(Water_Shader,_SurfaceColor);
				float4 _DeepColor_Instance = UNITY_ACCESS_INSTANCED_PROP(Water_Shader,_DeepColor);
				float2 _DepthSmoothStep_Instance = UNITY_ACCESS_INSTANCED_PROP(Water_Shader,_DepthSmoothStep);
				float4 ase_screenPosNorm = ScreenPos / ScreenPos.w;
				ase_screenPosNorm.z = ( UNITY_NEAR_CLIP_VALUE >= 0 ) ? ase_screenPosNorm.z : ase_screenPosNorm.z * 0.5 + 0.5;
				float _Distance_Instance = UNITY_ACCESS_INSTANCED_PROP(Water_Shader,_Distance);
				float screenDepth17 = LinearEyeDepth(SHADERGRAPH_SAMPLE_SCENE_DEPTH( ase_screenPosNorm.xy ),_ZBufferParams);
				float distanceDepth17 = abs( ( screenDepth17 - LinearEyeDepth( ase_screenPosNorm.z,_ZBufferParams ) ) / ( _Distance_Instance ) );
				float _EdgePower_Instance = UNITY_ACCESS_INSTANCED_PROP(Water_Shader,_EdgePower);
				float clampResult40 = clamp( ( ( 1.0 - distanceDepth17 ) * _EdgePower_Instance ) , 0.0 , 1.0 );
				float smoothstepResult132 = smoothstep( _DepthSmoothStep_Instance.x , _DepthSmoothStep_Instance.y , clampResult40);
				float DepthFadeBasic52 = smoothstepResult132;
				float4 lerpResult91 = lerp( _SurfaceColor_Instance , _DeepColor_Instance , ( 1.0 - DepthFadeBasic52 ));
				float2 _SoftSelect_Instance = UNITY_ACCESS_INSTANCED_PROP(Water_Shader,_SoftSelect);
				float2 _windDirection_Instance = UNITY_ACCESS_INSTANCED_PROP(Water_Shader,_windDirection);
				float2 _windDirection180 = _windDirection_Instance;
				float _textureSize_Instance = UNITY_ACCESS_INSTANCED_PROP(Water_Shader,_textureSize);
				float2 panner51 = ( 1.0 * _Time.y * _windDirection180 + (( WorldPosition / _textureSize_Instance )).xz);
				float smoothstepResult65 = smoothstep( _SoftSelect_Instance.x , _SoftSelect_Instance.y , tex2D( _MainTexture, panner51 ).a);
				float2 panner50 = ( 1.0 * _Time.y * ( ( 1.0 - _windDirection180 ) * 0.01 ) + (( WorldPosition / 94.0 )).xz);
				float lerpResult75 = lerp( smoothstepResult65 , 0.0 , ( tex2D( _MainTexture, panner50 ).a * 3.0 ));
				float _WaterNoiseMultiplyer_Instance = UNITY_ACCESS_INSTANCED_PROP(Water_Shader,_WaterNoiseMultiplyer);
				float _Wind85 = ( lerpResult75 * _WaterNoiseMultiplyer_Instance );
				float4 lerpResult99 = lerp( lerpResult91 , float4( 1,1,1,0 ) , _Wind85);
				float4 _FoamColor_Instance = UNITY_ACCESS_INSTANCED_PROP(Water_Shader,_FoamColor);
				float _depthNonSmoothStepped134 = clampResult40;
				float _FoamCuttoff_Instance = UNITY_ACCESS_INSTANCED_PROP(Water_Shader,_FoamCuttoff);
				float _EdgeFoamScale_Instance = UNITY_ACCESS_INSTANCED_PROP(Water_Shader,_EdgeFoamScale);
				float2 temp_cast_0 = (_EdgeFoamScale_Instance).xx;
				float2 _Direction_Instance = UNITY_ACCESS_INSTANCED_PROP(Water_Shader,_Direction);
				float2 _direction13 = _Direction_Instance;
				float _Speed_Instance = UNITY_ACCESS_INSTANCED_PROP(Water_Shader,_Speed);
				float _speed14 = _Speed_Instance;
				float2 texCoord56 = IN.ase_texcoord7.xy * temp_cast_0 + ( ( _direction13 * ( _speed14 / 10.0 ) ) * _TimeParameters.x );
				float2 temp_cast_1 = (_EdgeFoamScale_Instance).xx;
				float2 texCoord60 = IN.ase_texcoord7.xy * temp_cast_1 + ( 1.0 - ( ( _direction13 * _speed14 ) * _TimeParameters.x ) );
				float _foam82 = step( ( ( 1.0 - _depthNonSmoothStepped134 ) * _FoamCuttoff_Instance ) , ( tex2D( _NoiseTexture, texCoord56 ).a + tex2D( _NoiseTexture, texCoord60 ).a ) );
				float4 _EdgeFoamColor_Instance = UNITY_ACCESS_INSTANCED_PROP(Water_Shader,_EdgeFoamColor);
				float4 lerpResult104 = lerp( lerpResult99 , _FoamColor_Instance , ( _foam82 * _EdgeFoamColor_Instance ));
				float4 _colour110 = lerpResult104;
				float4 _IceColor_Instance = UNITY_ACCESS_INSTANCED_PROP(Water_Shader,_IceColor);
				float4 appendResult98 = (float4(_IceColor_Instance.r , _IceColor_Instance.g , _IceColor_Instance.b , 0.0));
				float2 _IceSmoothStep_Instance = UNITY_ACCESS_INSTANCED_PROP(Water_Shader,_IceSmoothStep);
				float _IceTextureScale_Instance = UNITY_ACCESS_INSTANCED_PROP(Water_Shader,_IceTextureScale);
				float2 temp_cast_2 = (_IceTextureScale_Instance).xx;
				float2 texCoord70 = IN.ase_texcoord7.xy * temp_cast_2 + float2( 0,0 );
				float smoothstepResult83 = smoothstep( -1.0 , 1.0 , tex2D( _NoiseTexture, texCoord70 ).b);
				float smoothstepResult101 = smoothstep( _IceSmoothStep_Instance.x , _IceSmoothStep_Instance.y , ( smoothstepResult83 - tex2D( _NoiseTexture, ( texCoord70 * float2( 2,2 ) ) ).b ));
				float4 clampResult108 = clamp( ( appendResult98 * smoothstepResult101 ) , float4( 0,0,0,0 ) , float4( 1,1,1,0 ) );
				float _IceStrength_Instance = UNITY_ACCESS_INSTANCED_PROP(Water_Shader,_IceStrength);
				float4 _albedo118 = ( _colour110 + ( clampResult108 * _IceStrength_Instance ) );
				
				float _Alpha_Instance = UNITY_ACCESS_INSTANCED_PROP(Water_Shader,_Alpha);
				float lerpResult106 = lerp( 1.0 , _Alpha_Instance , DepthFadeBasic52);
				float lerpResult111 = lerp( lerpResult106 , 0.0 , _Wind85);
				float _FoamAlpha_Instance = UNITY_ACCESS_INSTANCED_PROP(Water_Shader,_FoamAlpha);
				float lerpResult116 = lerp( lerpResult111 , _FoamAlpha_Instance , _foam82);
				float _Alpha119 = lerpResult116;
				
				float3 Albedo = _albedo118.rgb;
				float3 Normal = float3(0, 0, 1);
				float3 Emission = 0;
				float3 Specular = 0.5;
				float Metallic = 0.0;
				float Smoothness = 0.0;
				float Occlusion = 1;
				float Alpha = _Alpha119;
				float AlphaClipThreshold = 0.5;
				float AlphaClipThresholdShadow = 0.5;
				float3 BakedGI = 0;
				float3 RefractionColor = 1;
				float RefractionIndex = 1;
				float3 Transmission = 1;
				float3 Translucency = 1;
				#ifdef ASE_DEPTH_WRITE_ON
				float DepthValue = 0;
				#endif

				#ifdef _ALPHATEST_ON
					clip(Alpha - AlphaClipThreshold);
				#endif

				InputData inputData;
				inputData.positionWS = WorldPosition;
				inputData.viewDirectionWS = WorldViewDirection;
				inputData.shadowCoord = ShadowCoords;

				#ifdef _NORMALMAP
					#if _NORMAL_DROPOFF_TS
					inputData.normalWS = TransformTangentToWorld(Normal, half3x3( WorldTangent, WorldBiTangent, WorldNormal ));
					#elif _NORMAL_DROPOFF_OS
					inputData.normalWS = TransformObjectToWorldNormal(Normal);
					#elif _NORMAL_DROPOFF_WS
					inputData.normalWS = Normal;
					#endif
					inputData.normalWS = NormalizeNormalPerPixel(inputData.normalWS);
				#else
					inputData.normalWS = WorldNormal;
				#endif

				#ifdef ASE_FOG
					inputData.fogCoord = IN.fogFactorAndVertexLight.x;
				#endif

				inputData.vertexLighting = IN.fogFactorAndVertexLight.yzw;
				#if defined(ENABLE_TERRAIN_PERPIXEL_NORMAL)
					float3 SH = SampleSH(inputData.normalWS.xyz);
				#else
					float3 SH = IN.lightmapUVOrVertexSH.xyz;
				#endif

				inputData.bakedGI = SAMPLE_GI( IN.lightmapUVOrVertexSH.xy, SH, inputData.normalWS );
				#ifdef _ASE_BAKEDGI
					inputData.bakedGI = BakedGI;
				#endif
				
				inputData.normalizedScreenSpaceUV = GetNormalizedScreenSpaceUV(IN.clipPos);
				inputData.shadowMask = SAMPLE_SHADOWMASK(IN.lightmapUVOrVertexSH.xy);

				half4 color = UniversalFragmentPBR(
					inputData, 
					Albedo, 
					Metallic, 
					Specular, 
					Smoothness, 
					Occlusion, 
					Emission, 
					Alpha);

				#ifdef _TRANSMISSION_ASE
				{
					float shadow = _TransmissionShadow;

					Light mainLight = GetMainLight( inputData.shadowCoord );
					float3 mainAtten = mainLight.color * mainLight.distanceAttenuation;
					mainAtten = lerp( mainAtten, mainAtten * mainLight.shadowAttenuation, shadow );
					half3 mainTransmission = max(0 , -dot(inputData.normalWS, mainLight.direction)) * mainAtten * Transmission;
					color.rgb += Albedo * mainTransmission;

					#ifdef _ADDITIONAL_LIGHTS
						int transPixelLightCount = GetAdditionalLightsCount();
						for (int i = 0; i < transPixelLightCount; ++i)
						{
							Light light = GetAdditionalLight(i, inputData.positionWS);
							float3 atten = light.color * light.distanceAttenuation;
							atten = lerp( atten, atten * light.shadowAttenuation, shadow );

							half3 transmission = max(0 , -dot(inputData.normalWS, light.direction)) * atten * Transmission;
							color.rgb += Albedo * transmission;
						}
					#endif
				}
				#endif

				#ifdef _TRANSLUCENCY_ASE
				{
					float shadow = _TransShadow;
					float normal = _TransNormal;
					float scattering = _TransScattering;
					float direct = _TransDirect;
					float ambient = _TransAmbient;
					float strength = _TransStrength;

					Light mainLight = GetMainLight( inputData.shadowCoord );
					float3 mainAtten = mainLight.color * mainLight.distanceAttenuation;
					mainAtten = lerp( mainAtten, mainAtten * mainLight.shadowAttenuation, shadow );

					half3 mainLightDir = mainLight.direction + inputData.normalWS * normal;
					half mainVdotL = pow( saturate( dot( inputData.viewDirectionWS, -mainLightDir ) ), scattering );
					half3 mainTranslucency = mainAtten * ( mainVdotL * direct + inputData.bakedGI * ambient ) * Translucency;
					color.rgb += Albedo * mainTranslucency * strength;

					#ifdef _ADDITIONAL_LIGHTS
						int transPixelLightCount = GetAdditionalLightsCount();
						for (int i = 0; i < transPixelLightCount; ++i)
						{
							Light light = GetAdditionalLight(i, inputData.positionWS);
							float3 atten = light.color * light.distanceAttenuation;
							atten = lerp( atten, atten * light.shadowAttenuation, shadow );

							half3 lightDir = light.direction + inputData.normalWS * normal;
							half VdotL = pow( saturate( dot( inputData.viewDirectionWS, -lightDir ) ), scattering );
							half3 translucency = atten * ( VdotL * direct + inputData.bakedGI * ambient ) * Translucency;
							color.rgb += Albedo * translucency * strength;
						}
					#endif
				}
				#endif

				#ifdef _REFRACTION_ASE
					float4 projScreenPos = ScreenPos / ScreenPos.w;
					float3 refractionOffset = ( RefractionIndex - 1.0 ) * mul( UNITY_MATRIX_V, float4( WorldNormal,0 ) ).xyz * ( 1.0 - dot( WorldNormal, WorldViewDirection ) );
					projScreenPos.xy += refractionOffset.xy;
					float3 refraction = SHADERGRAPH_SAMPLE_SCENE_COLOR( projScreenPos.xy ) * RefractionColor;
					color.rgb = lerp( refraction, color.rgb, color.a );
					color.a = 1;
				#endif

				#ifdef ASE_FINAL_COLOR_ALPHA_MULTIPLY
					color.rgb *= color.a;
				#endif

				#ifdef ASE_FOG
					#ifdef TERRAIN_SPLAT_ADDPASS
						color.rgb = MixFogColor(color.rgb, half3( 0, 0, 0 ), IN.fogFactorAndVertexLight.x );
					#else
						color.rgb = MixFog(color.rgb, IN.fogFactorAndVertexLight.x);
					#endif
				#endif

				#ifdef ASE_DEPTH_WRITE_ON
					outputDepth = DepthValue;
				#endif

				return color;
			}

			ENDHLSL
		}

		
		Pass
		{
			
			Name "ShadowCaster"
			Tags { "LightMode"="ShadowCaster" }

			ZWrite On
			ZTest LEqual
			AlphaToMask Off
			ColorMask 0

			HLSLPROGRAM
			
			#define _NORMAL_DROPOFF_TS 1
			#pragma multi_compile _ LOD_FADE_CROSSFADE
			#pragma multi_compile_fog
			#define ASE_FOG 1
			#define ASE_SRP_VERSION 100400
			#define REQUIRE_DEPTH_TEXTURE 1

			
			#pragma vertex vert
			#pragma fragment frag
#if ASE_SRP_VERSION >= 110000
			#pragma multi_compile _ _CASTING_PUNCTUAL_LIGHT_SHADOW
#endif
			#define SHADERPASS_SHADOWCASTER

			#include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
			#include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Lighting.hlsl"
			#include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/ShaderGraphFunctions.hlsl"
			#include "Packages/com.unity.render-pipelines.core/ShaderLibrary/Color.hlsl"

			#define ASE_NEEDS_FRAG_WORLD_POSITION
			#pragma multi_compile_instancing


			struct VertexInput
			{
				float4 vertex : POSITION;
				float3 ase_normal : NORMAL;
				float4 ase_texcoord : TEXCOORD0;
				UNITY_VERTEX_INPUT_INSTANCE_ID
			};

			struct VertexOutput
			{
				float4 clipPos : SV_POSITION;
				#if defined(ASE_NEEDS_FRAG_WORLD_POSITION)
				float3 worldPos : TEXCOORD0;
				#endif
				#if defined(REQUIRES_VERTEX_SHADOW_COORD_INTERPOLATOR) && defined(ASE_NEEDS_FRAG_SHADOWCOORDS)
				float4 shadowCoord : TEXCOORD1;
				#endif
				float4 ase_texcoord2 : TEXCOORD2;
				float4 ase_texcoord3 : TEXCOORD3;
				UNITY_VERTEX_INPUT_INSTANCE_ID
				UNITY_VERTEX_OUTPUT_STEREO
			};

			CBUFFER_START(UnityPerMaterial)
						#ifdef _TRANSMISSION_ASE
				float _TransmissionShadow;
			#endif
			#ifdef _TRANSLUCENCY_ASE
				float _TransStrength;
				float _TransNormal;
				float _TransScattering;
				float _TransDirect;
				float _TransAmbient;
				float _TransShadow;
			#endif
			#ifdef TESSELLATION_ON
				float _TessPhongStrength;
				float _TessValue;
				float _TessMin;
				float _TessMax;
				float _TessEdgeLength;
				float _TessMaxDisp;
			#endif
			CBUFFER_END
			uniform float4 _CameraDepthTexture_TexelSize;
			sampler2D _MainTexture;
			sampler2D _NoiseTexture;
			UNITY_INSTANCING_BUFFER_START(Water_Shader)
				UNITY_DEFINE_INSTANCED_PROP(float2, _DepthSmoothStep)
				UNITY_DEFINE_INSTANCED_PROP(float2, _SoftSelect)
				UNITY_DEFINE_INSTANCED_PROP(float2, _windDirection)
				UNITY_DEFINE_INSTANCED_PROP(float2, _Direction)
				UNITY_DEFINE_INSTANCED_PROP(float, _Alpha)
				UNITY_DEFINE_INSTANCED_PROP(float, _Distance)
				UNITY_DEFINE_INSTANCED_PROP(float, _EdgePower)
				UNITY_DEFINE_INSTANCED_PROP(float, _textureSize)
				UNITY_DEFINE_INSTANCED_PROP(float, _WaterNoiseMultiplyer)
				UNITY_DEFINE_INSTANCED_PROP(float, _FoamAlpha)
				UNITY_DEFINE_INSTANCED_PROP(float, _FoamCuttoff)
				UNITY_DEFINE_INSTANCED_PROP(float, _EdgeFoamScale)
				UNITY_DEFINE_INSTANCED_PROP(float, _Speed)
			UNITY_INSTANCING_BUFFER_END(Water_Shader)


			
			float3 _LightDirection;
#if ASE_SRP_VERSION >= 110000 
			float3 _LightPosition;
#endif
			VertexOutput VertexFunction( VertexInput v )
			{
				VertexOutput o;
				UNITY_SETUP_INSTANCE_ID(v);
				UNITY_TRANSFER_INSTANCE_ID(v, o);
				UNITY_INITIALIZE_VERTEX_OUTPUT_STEREO( o );

				float4 ase_clipPos = TransformObjectToHClip((v.vertex).xyz);
				float4 screenPos = ComputeScreenPos(ase_clipPos);
				o.ase_texcoord2 = screenPos;
				
				o.ase_texcoord3.xy = v.ase_texcoord.xy;
				
				//setting value to unused interpolator channels and avoid initialization warnings
				o.ase_texcoord3.zw = 0;
				#ifdef ASE_ABSOLUTE_VERTEX_POS
					float3 defaultVertexValue = v.vertex.xyz;
				#else
					float3 defaultVertexValue = float3(0, 0, 0);
				#endif
				float3 vertexValue = defaultVertexValue;
				#ifdef ASE_ABSOLUTE_VERTEX_POS
					v.vertex.xyz = vertexValue;
				#else
					v.vertex.xyz += vertexValue;
				#endif

				v.ase_normal = v.ase_normal;

				float3 positionWS = TransformObjectToWorld( v.vertex.xyz );
				#if defined(ASE_NEEDS_FRAG_WORLD_POSITION)
				o.worldPos = positionWS;
				#endif
				float3 normalWS = TransformObjectToWorldDir(v.ase_normal);

		#if ASE_SRP_VERSION >= 110000 
			#if _CASTING_PUNCTUAL_LIGHT_SHADOW
				float3 lightDirectionWS = normalize(_LightPosition - positionWS);
			#else
				float3 lightDirectionWS = _LightDirection;
			#endif
				float4 clipPos = TransformWorldToHClip(ApplyShadowBias(positionWS, normalWS, lightDirectionWS));
			#if UNITY_REVERSED_Z
				clipPos.z = min(clipPos.z, UNITY_NEAR_CLIP_VALUE);
			#else
				clipPos.z = max(clipPos.z, UNITY_NEAR_CLIP_VALUE);
			#endif
		#else
				float4 clipPos = TransformWorldToHClip(ApplyShadowBias(positionWS, normalWS, _LightDirection));
			#if UNITY_REVERSED_Z
				clipPos.z = min(clipPos.z, clipPos.w * UNITY_NEAR_CLIP_VALUE);
			#else
				clipPos.z = max(clipPos.z, clipPos.w * UNITY_NEAR_CLIP_VALUE);
			#endif
		#endif

				#if defined(REQUIRES_VERTEX_SHADOW_COORD_INTERPOLATOR) && defined(ASE_NEEDS_FRAG_SHADOWCOORDS)
					VertexPositionInputs vertexInput = (VertexPositionInputs)0;
					vertexInput.positionWS = positionWS;
					vertexInput.positionCS = clipPos;
					o.shadowCoord = GetShadowCoord( vertexInput );
				#endif
				o.clipPos = clipPos;
				return o;
			}

			#if defined(TESSELLATION_ON)
			struct VertexControl
			{
				float4 vertex : INTERNALTESSPOS;
				float3 ase_normal : NORMAL;
				float4 ase_texcoord : TEXCOORD0;

				UNITY_VERTEX_INPUT_INSTANCE_ID
			};

			struct TessellationFactors
			{
				float edge[3] : SV_TessFactor;
				float inside : SV_InsideTessFactor;
			};

			VertexControl vert ( VertexInput v )
			{
				VertexControl o;
				UNITY_SETUP_INSTANCE_ID(v);
				UNITY_TRANSFER_INSTANCE_ID(v, o);
				o.vertex = v.vertex;
				o.ase_normal = v.ase_normal;
				o.ase_texcoord = v.ase_texcoord;
				return o;
			}

			TessellationFactors TessellationFunction (InputPatch<VertexControl,3> v)
			{
				TessellationFactors o;
				float4 tf = 1;
				float tessValue = _TessValue; float tessMin = _TessMin; float tessMax = _TessMax;
				float edgeLength = _TessEdgeLength; float tessMaxDisp = _TessMaxDisp;
				#if defined(ASE_FIXED_TESSELLATION)
				tf = FixedTess( tessValue );
				#elif defined(ASE_DISTANCE_TESSELLATION)
				tf = DistanceBasedTess(v[0].vertex, v[1].vertex, v[2].vertex, tessValue, tessMin, tessMax, GetObjectToWorldMatrix(), _WorldSpaceCameraPos );
				#elif defined(ASE_LENGTH_TESSELLATION)
				tf = EdgeLengthBasedTess(v[0].vertex, v[1].vertex, v[2].vertex, edgeLength, GetObjectToWorldMatrix(), _WorldSpaceCameraPos, _ScreenParams );
				#elif defined(ASE_LENGTH_CULL_TESSELLATION)
				tf = EdgeLengthBasedTessCull(v[0].vertex, v[1].vertex, v[2].vertex, edgeLength, tessMaxDisp, GetObjectToWorldMatrix(), _WorldSpaceCameraPos, _ScreenParams, unity_CameraWorldClipPlanes );
				#endif
				o.edge[0] = tf.x; o.edge[1] = tf.y; o.edge[2] = tf.z; o.inside = tf.w;
				return o;
			}

			[domain("tri")]
			[partitioning("fractional_odd")]
			[outputtopology("triangle_cw")]
			[patchconstantfunc("TessellationFunction")]
			[outputcontrolpoints(3)]
			VertexControl HullFunction(InputPatch<VertexControl, 3> patch, uint id : SV_OutputControlPointID)
			{
			   return patch[id];
			}

			[domain("tri")]
			VertexOutput DomainFunction(TessellationFactors factors, OutputPatch<VertexControl, 3> patch, float3 bary : SV_DomainLocation)
			{
				VertexInput o = (VertexInput) 0;
				o.vertex = patch[0].vertex * bary.x + patch[1].vertex * bary.y + patch[2].vertex * bary.z;
				o.ase_normal = patch[0].ase_normal * bary.x + patch[1].ase_normal * bary.y + patch[2].ase_normal * bary.z;
				o.ase_texcoord = patch[0].ase_texcoord * bary.x + patch[1].ase_texcoord * bary.y + patch[2].ase_texcoord * bary.z;
				#if defined(ASE_PHONG_TESSELLATION)
				float3 pp[3];
				for (int i = 0; i < 3; ++i)
					pp[i] = o.vertex.xyz - patch[i].ase_normal * (dot(o.vertex.xyz, patch[i].ase_normal) - dot(patch[i].vertex.xyz, patch[i].ase_normal));
				float phongStrength = _TessPhongStrength;
				o.vertex.xyz = phongStrength * (pp[0]*bary.x + pp[1]*bary.y + pp[2]*bary.z) + (1.0f-phongStrength) * o.vertex.xyz;
				#endif
				UNITY_TRANSFER_INSTANCE_ID(patch[0], o);
				return VertexFunction(o);
			}
			#else
			VertexOutput vert ( VertexInput v )
			{
				return VertexFunction( v );
			}
			#endif

			#if defined(ASE_EARLY_Z_DEPTH_OPTIMIZE)
				#define ASE_SV_DEPTH SV_DepthLessEqual  
			#else
				#define ASE_SV_DEPTH SV_Depth
			#endif

			half4 frag(	VertexOutput IN 
						#ifdef ASE_DEPTH_WRITE_ON
						,out float outputDepth : ASE_SV_DEPTH
						#endif
						 ) : SV_TARGET
			{
				UNITY_SETUP_INSTANCE_ID( IN );
				UNITY_SETUP_STEREO_EYE_INDEX_POST_VERTEX( IN );
				
				#if defined(ASE_NEEDS_FRAG_WORLD_POSITION)
				float3 WorldPosition = IN.worldPos;
				#endif
				float4 ShadowCoords = float4( 0, 0, 0, 0 );

				#if defined(ASE_NEEDS_FRAG_SHADOWCOORDS)
					#if defined(REQUIRES_VERTEX_SHADOW_COORD_INTERPOLATOR)
						ShadowCoords = IN.shadowCoord;
					#elif defined(MAIN_LIGHT_CALCULATE_SHADOWS)
						ShadowCoords = TransformWorldToShadowCoord( WorldPosition );
					#endif
				#endif

				float _Alpha_Instance = UNITY_ACCESS_INSTANCED_PROP(Water_Shader,_Alpha);
				float2 _DepthSmoothStep_Instance = UNITY_ACCESS_INSTANCED_PROP(Water_Shader,_DepthSmoothStep);
				float4 screenPos = IN.ase_texcoord2;
				float4 ase_screenPosNorm = screenPos / screenPos.w;
				ase_screenPosNorm.z = ( UNITY_NEAR_CLIP_VALUE >= 0 ) ? ase_screenPosNorm.z : ase_screenPosNorm.z * 0.5 + 0.5;
				float _Distance_Instance = UNITY_ACCESS_INSTANCED_PROP(Water_Shader,_Distance);
				float screenDepth17 = LinearEyeDepth(SHADERGRAPH_SAMPLE_SCENE_DEPTH( ase_screenPosNorm.xy ),_ZBufferParams);
				float distanceDepth17 = abs( ( screenDepth17 - LinearEyeDepth( ase_screenPosNorm.z,_ZBufferParams ) ) / ( _Distance_Instance ) );
				float _EdgePower_Instance = UNITY_ACCESS_INSTANCED_PROP(Water_Shader,_EdgePower);
				float clampResult40 = clamp( ( ( 1.0 - distanceDepth17 ) * _EdgePower_Instance ) , 0.0 , 1.0 );
				float smoothstepResult132 = smoothstep( _DepthSmoothStep_Instance.x , _DepthSmoothStep_Instance.y , clampResult40);
				float DepthFadeBasic52 = smoothstepResult132;
				float lerpResult106 = lerp( 1.0 , _Alpha_Instance , DepthFadeBasic52);
				float2 _SoftSelect_Instance = UNITY_ACCESS_INSTANCED_PROP(Water_Shader,_SoftSelect);
				float2 _windDirection_Instance = UNITY_ACCESS_INSTANCED_PROP(Water_Shader,_windDirection);
				float2 _windDirection180 = _windDirection_Instance;
				float _textureSize_Instance = UNITY_ACCESS_INSTANCED_PROP(Water_Shader,_textureSize);
				float2 panner51 = ( 1.0 * _Time.y * _windDirection180 + (( WorldPosition / _textureSize_Instance )).xz);
				float smoothstepResult65 = smoothstep( _SoftSelect_Instance.x , _SoftSelect_Instance.y , tex2D( _MainTexture, panner51 ).a);
				float2 panner50 = ( 1.0 * _Time.y * ( ( 1.0 - _windDirection180 ) * 0.01 ) + (( WorldPosition / 94.0 )).xz);
				float lerpResult75 = lerp( smoothstepResult65 , 0.0 , ( tex2D( _MainTexture, panner50 ).a * 3.0 ));
				float _WaterNoiseMultiplyer_Instance = UNITY_ACCESS_INSTANCED_PROP(Water_Shader,_WaterNoiseMultiplyer);
				float _Wind85 = ( lerpResult75 * _WaterNoiseMultiplyer_Instance );
				float lerpResult111 = lerp( lerpResult106 , 0.0 , _Wind85);
				float _FoamAlpha_Instance = UNITY_ACCESS_INSTANCED_PROP(Water_Shader,_FoamAlpha);
				float _depthNonSmoothStepped134 = clampResult40;
				float _FoamCuttoff_Instance = UNITY_ACCESS_INSTANCED_PROP(Water_Shader,_FoamCuttoff);
				float _EdgeFoamScale_Instance = UNITY_ACCESS_INSTANCED_PROP(Water_Shader,_EdgeFoamScale);
				float2 temp_cast_0 = (_EdgeFoamScale_Instance).xx;
				float2 _Direction_Instance = UNITY_ACCESS_INSTANCED_PROP(Water_Shader,_Direction);
				float2 _direction13 = _Direction_Instance;
				float _Speed_Instance = UNITY_ACCESS_INSTANCED_PROP(Water_Shader,_Speed);
				float _speed14 = _Speed_Instance;
				float2 texCoord56 = IN.ase_texcoord3.xy * temp_cast_0 + ( ( _direction13 * ( _speed14 / 10.0 ) ) * _TimeParameters.x );
				float2 temp_cast_1 = (_EdgeFoamScale_Instance).xx;
				float2 texCoord60 = IN.ase_texcoord3.xy * temp_cast_1 + ( 1.0 - ( ( _direction13 * _speed14 ) * _TimeParameters.x ) );
				float _foam82 = step( ( ( 1.0 - _depthNonSmoothStepped134 ) * _FoamCuttoff_Instance ) , ( tex2D( _NoiseTexture, texCoord56 ).a + tex2D( _NoiseTexture, texCoord60 ).a ) );
				float lerpResult116 = lerp( lerpResult111 , _FoamAlpha_Instance , _foam82);
				float _Alpha119 = lerpResult116;
				
				float Alpha = _Alpha119;
				float AlphaClipThreshold = 0.5;
				float AlphaClipThresholdShadow = 0.5;
				#ifdef ASE_DEPTH_WRITE_ON
				float DepthValue = 0;
				#endif

				#ifdef _ALPHATEST_ON
					#ifdef _ALPHATEST_SHADOW_ON
						clip(Alpha - AlphaClipThresholdShadow);
					#else
						clip(Alpha - AlphaClipThreshold);
					#endif
				#endif

				#ifdef LOD_FADE_CROSSFADE
					LODDitheringTransition( IN.clipPos.xyz, unity_LODFade.x );
				#endif
				#ifdef ASE_DEPTH_WRITE_ON
					outputDepth = DepthValue;
				#endif
				return 0;
			}

			ENDHLSL
		}

		
		Pass
		{
			
			Name "DepthOnly"
			Tags { "LightMode"="DepthOnly" }

			ZWrite On
			ColorMask 0
			AlphaToMask Off

			HLSLPROGRAM
			
			#define _NORMAL_DROPOFF_TS 1
			#pragma multi_compile _ LOD_FADE_CROSSFADE
			#pragma multi_compile_fog
			#define ASE_FOG 1
			#define ASE_SRP_VERSION 100400
			#define REQUIRE_DEPTH_TEXTURE 1

			
			#pragma vertex vert
			#pragma fragment frag

			#define SHADERPASS_DEPTHONLY

			#include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
			#include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Lighting.hlsl"
			#include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/ShaderGraphFunctions.hlsl"
			#include "Packages/com.unity.render-pipelines.core/ShaderLibrary/Color.hlsl"

			#define ASE_NEEDS_FRAG_WORLD_POSITION
			#pragma multi_compile_instancing


			struct VertexInput
			{
				float4 vertex : POSITION;
				float3 ase_normal : NORMAL;
				float4 ase_texcoord : TEXCOORD0;
				UNITY_VERTEX_INPUT_INSTANCE_ID
			};

			struct VertexOutput
			{
				float4 clipPos : SV_POSITION;
				#if defined(ASE_NEEDS_FRAG_WORLD_POSITION)
				float3 worldPos : TEXCOORD0;
				#endif
				#if defined(REQUIRES_VERTEX_SHADOW_COORD_INTERPOLATOR) && defined(ASE_NEEDS_FRAG_SHADOWCOORDS)
				float4 shadowCoord : TEXCOORD1;
				#endif
				float4 ase_texcoord2 : TEXCOORD2;
				float4 ase_texcoord3 : TEXCOORD3;
				UNITY_VERTEX_INPUT_INSTANCE_ID
				UNITY_VERTEX_OUTPUT_STEREO
			};

			CBUFFER_START(UnityPerMaterial)
						#ifdef _TRANSMISSION_ASE
				float _TransmissionShadow;
			#endif
			#ifdef _TRANSLUCENCY_ASE
				float _TransStrength;
				float _TransNormal;
				float _TransScattering;
				float _TransDirect;
				float _TransAmbient;
				float _TransShadow;
			#endif
			#ifdef TESSELLATION_ON
				float _TessPhongStrength;
				float _TessValue;
				float _TessMin;
				float _TessMax;
				float _TessEdgeLength;
				float _TessMaxDisp;
			#endif
			CBUFFER_END
			uniform float4 _CameraDepthTexture_TexelSize;
			sampler2D _MainTexture;
			sampler2D _NoiseTexture;
			UNITY_INSTANCING_BUFFER_START(Water_Shader)
				UNITY_DEFINE_INSTANCED_PROP(float2, _DepthSmoothStep)
				UNITY_DEFINE_INSTANCED_PROP(float2, _SoftSelect)
				UNITY_DEFINE_INSTANCED_PROP(float2, _windDirection)
				UNITY_DEFINE_INSTANCED_PROP(float2, _Direction)
				UNITY_DEFINE_INSTANCED_PROP(float, _Alpha)
				UNITY_DEFINE_INSTANCED_PROP(float, _Distance)
				UNITY_DEFINE_INSTANCED_PROP(float, _EdgePower)
				UNITY_DEFINE_INSTANCED_PROP(float, _textureSize)
				UNITY_DEFINE_INSTANCED_PROP(float, _WaterNoiseMultiplyer)
				UNITY_DEFINE_INSTANCED_PROP(float, _FoamAlpha)
				UNITY_DEFINE_INSTANCED_PROP(float, _FoamCuttoff)
				UNITY_DEFINE_INSTANCED_PROP(float, _EdgeFoamScale)
				UNITY_DEFINE_INSTANCED_PROP(float, _Speed)
			UNITY_INSTANCING_BUFFER_END(Water_Shader)


			
			VertexOutput VertexFunction( VertexInput v  )
			{
				VertexOutput o = (VertexOutput)0;
				UNITY_SETUP_INSTANCE_ID(v);
				UNITY_TRANSFER_INSTANCE_ID(v, o);
				UNITY_INITIALIZE_VERTEX_OUTPUT_STEREO(o);

				float4 ase_clipPos = TransformObjectToHClip((v.vertex).xyz);
				float4 screenPos = ComputeScreenPos(ase_clipPos);
				o.ase_texcoord2 = screenPos;
				
				o.ase_texcoord3.xy = v.ase_texcoord.xy;
				
				//setting value to unused interpolator channels and avoid initialization warnings
				o.ase_texcoord3.zw = 0;
				#ifdef ASE_ABSOLUTE_VERTEX_POS
					float3 defaultVertexValue = v.vertex.xyz;
				#else
					float3 defaultVertexValue = float3(0, 0, 0);
				#endif
				float3 vertexValue = defaultVertexValue;
				#ifdef ASE_ABSOLUTE_VERTEX_POS
					v.vertex.xyz = vertexValue;
				#else
					v.vertex.xyz += vertexValue;
				#endif

				v.ase_normal = v.ase_normal;
				float3 positionWS = TransformObjectToWorld( v.vertex.xyz );
				float4 positionCS = TransformWorldToHClip( positionWS );

				#if defined(ASE_NEEDS_FRAG_WORLD_POSITION)
				o.worldPos = positionWS;
				#endif

				#if defined(REQUIRES_VERTEX_SHADOW_COORD_INTERPOLATOR) && defined(ASE_NEEDS_FRAG_SHADOWCOORDS)
					VertexPositionInputs vertexInput = (VertexPositionInputs)0;
					vertexInput.positionWS = positionWS;
					vertexInput.positionCS = positionCS;
					o.shadowCoord = GetShadowCoord( vertexInput );
				#endif
				o.clipPos = positionCS;
				return o;
			}

			#if defined(TESSELLATION_ON)
			struct VertexControl
			{
				float4 vertex : INTERNALTESSPOS;
				float3 ase_normal : NORMAL;
				float4 ase_texcoord : TEXCOORD0;

				UNITY_VERTEX_INPUT_INSTANCE_ID
			};

			struct TessellationFactors
			{
				float edge[3] : SV_TessFactor;
				float inside : SV_InsideTessFactor;
			};

			VertexControl vert ( VertexInput v )
			{
				VertexControl o;
				UNITY_SETUP_INSTANCE_ID(v);
				UNITY_TRANSFER_INSTANCE_ID(v, o);
				o.vertex = v.vertex;
				o.ase_normal = v.ase_normal;
				o.ase_texcoord = v.ase_texcoord;
				return o;
			}

			TessellationFactors TessellationFunction (InputPatch<VertexControl,3> v)
			{
				TessellationFactors o;
				float4 tf = 1;
				float tessValue = _TessValue; float tessMin = _TessMin; float tessMax = _TessMax;
				float edgeLength = _TessEdgeLength; float tessMaxDisp = _TessMaxDisp;
				#if defined(ASE_FIXED_TESSELLATION)
				tf = FixedTess( tessValue );
				#elif defined(ASE_DISTANCE_TESSELLATION)
				tf = DistanceBasedTess(v[0].vertex, v[1].vertex, v[2].vertex, tessValue, tessMin, tessMax, GetObjectToWorldMatrix(), _WorldSpaceCameraPos );
				#elif defined(ASE_LENGTH_TESSELLATION)
				tf = EdgeLengthBasedTess(v[0].vertex, v[1].vertex, v[2].vertex, edgeLength, GetObjectToWorldMatrix(), _WorldSpaceCameraPos, _ScreenParams );
				#elif defined(ASE_LENGTH_CULL_TESSELLATION)
				tf = EdgeLengthBasedTessCull(v[0].vertex, v[1].vertex, v[2].vertex, edgeLength, tessMaxDisp, GetObjectToWorldMatrix(), _WorldSpaceCameraPos, _ScreenParams, unity_CameraWorldClipPlanes );
				#endif
				o.edge[0] = tf.x; o.edge[1] = tf.y; o.edge[2] = tf.z; o.inside = tf.w;
				return o;
			}

			[domain("tri")]
			[partitioning("fractional_odd")]
			[outputtopology("triangle_cw")]
			[patchconstantfunc("TessellationFunction")]
			[outputcontrolpoints(3)]
			VertexControl HullFunction(InputPatch<VertexControl, 3> patch, uint id : SV_OutputControlPointID)
			{
			   return patch[id];
			}

			[domain("tri")]
			VertexOutput DomainFunction(TessellationFactors factors, OutputPatch<VertexControl, 3> patch, float3 bary : SV_DomainLocation)
			{
				VertexInput o = (VertexInput) 0;
				o.vertex = patch[0].vertex * bary.x + patch[1].vertex * bary.y + patch[2].vertex * bary.z;
				o.ase_normal = patch[0].ase_normal * bary.x + patch[1].ase_normal * bary.y + patch[2].ase_normal * bary.z;
				o.ase_texcoord = patch[0].ase_texcoord * bary.x + patch[1].ase_texcoord * bary.y + patch[2].ase_texcoord * bary.z;
				#if defined(ASE_PHONG_TESSELLATION)
				float3 pp[3];
				for (int i = 0; i < 3; ++i)
					pp[i] = o.vertex.xyz - patch[i].ase_normal * (dot(o.vertex.xyz, patch[i].ase_normal) - dot(patch[i].vertex.xyz, patch[i].ase_normal));
				float phongStrength = _TessPhongStrength;
				o.vertex.xyz = phongStrength * (pp[0]*bary.x + pp[1]*bary.y + pp[2]*bary.z) + (1.0f-phongStrength) * o.vertex.xyz;
				#endif
				UNITY_TRANSFER_INSTANCE_ID(patch[0], o);
				return VertexFunction(o);
			}
			#else
			VertexOutput vert ( VertexInput v )
			{
				return VertexFunction( v );
			}
			#endif

			#if defined(ASE_EARLY_Z_DEPTH_OPTIMIZE)
				#define ASE_SV_DEPTH SV_DepthLessEqual  
			#else
				#define ASE_SV_DEPTH SV_Depth
			#endif
			half4 frag(	VertexOutput IN 
						#ifdef ASE_DEPTH_WRITE_ON
						,out float outputDepth : ASE_SV_DEPTH
						#endif
						 ) : SV_TARGET
			{
				UNITY_SETUP_INSTANCE_ID(IN);
				UNITY_SETUP_STEREO_EYE_INDEX_POST_VERTEX( IN );

				#if defined(ASE_NEEDS_FRAG_WORLD_POSITION)
				float3 WorldPosition = IN.worldPos;
				#endif
				float4 ShadowCoords = float4( 0, 0, 0, 0 );

				#if defined(ASE_NEEDS_FRAG_SHADOWCOORDS)
					#if defined(REQUIRES_VERTEX_SHADOW_COORD_INTERPOLATOR)
						ShadowCoords = IN.shadowCoord;
					#elif defined(MAIN_LIGHT_CALCULATE_SHADOWS)
						ShadowCoords = TransformWorldToShadowCoord( WorldPosition );
					#endif
				#endif

				float _Alpha_Instance = UNITY_ACCESS_INSTANCED_PROP(Water_Shader,_Alpha);
				float2 _DepthSmoothStep_Instance = UNITY_ACCESS_INSTANCED_PROP(Water_Shader,_DepthSmoothStep);
				float4 screenPos = IN.ase_texcoord2;
				float4 ase_screenPosNorm = screenPos / screenPos.w;
				ase_screenPosNorm.z = ( UNITY_NEAR_CLIP_VALUE >= 0 ) ? ase_screenPosNorm.z : ase_screenPosNorm.z * 0.5 + 0.5;
				float _Distance_Instance = UNITY_ACCESS_INSTANCED_PROP(Water_Shader,_Distance);
				float screenDepth17 = LinearEyeDepth(SHADERGRAPH_SAMPLE_SCENE_DEPTH( ase_screenPosNorm.xy ),_ZBufferParams);
				float distanceDepth17 = abs( ( screenDepth17 - LinearEyeDepth( ase_screenPosNorm.z,_ZBufferParams ) ) / ( _Distance_Instance ) );
				float _EdgePower_Instance = UNITY_ACCESS_INSTANCED_PROP(Water_Shader,_EdgePower);
				float clampResult40 = clamp( ( ( 1.0 - distanceDepth17 ) * _EdgePower_Instance ) , 0.0 , 1.0 );
				float smoothstepResult132 = smoothstep( _DepthSmoothStep_Instance.x , _DepthSmoothStep_Instance.y , clampResult40);
				float DepthFadeBasic52 = smoothstepResult132;
				float lerpResult106 = lerp( 1.0 , _Alpha_Instance , DepthFadeBasic52);
				float2 _SoftSelect_Instance = UNITY_ACCESS_INSTANCED_PROP(Water_Shader,_SoftSelect);
				float2 _windDirection_Instance = UNITY_ACCESS_INSTANCED_PROP(Water_Shader,_windDirection);
				float2 _windDirection180 = _windDirection_Instance;
				float _textureSize_Instance = UNITY_ACCESS_INSTANCED_PROP(Water_Shader,_textureSize);
				float2 panner51 = ( 1.0 * _Time.y * _windDirection180 + (( WorldPosition / _textureSize_Instance )).xz);
				float smoothstepResult65 = smoothstep( _SoftSelect_Instance.x , _SoftSelect_Instance.y , tex2D( _MainTexture, panner51 ).a);
				float2 panner50 = ( 1.0 * _Time.y * ( ( 1.0 - _windDirection180 ) * 0.01 ) + (( WorldPosition / 94.0 )).xz);
				float lerpResult75 = lerp( smoothstepResult65 , 0.0 , ( tex2D( _MainTexture, panner50 ).a * 3.0 ));
				float _WaterNoiseMultiplyer_Instance = UNITY_ACCESS_INSTANCED_PROP(Water_Shader,_WaterNoiseMultiplyer);
				float _Wind85 = ( lerpResult75 * _WaterNoiseMultiplyer_Instance );
				float lerpResult111 = lerp( lerpResult106 , 0.0 , _Wind85);
				float _FoamAlpha_Instance = UNITY_ACCESS_INSTANCED_PROP(Water_Shader,_FoamAlpha);
				float _depthNonSmoothStepped134 = clampResult40;
				float _FoamCuttoff_Instance = UNITY_ACCESS_INSTANCED_PROP(Water_Shader,_FoamCuttoff);
				float _EdgeFoamScale_Instance = UNITY_ACCESS_INSTANCED_PROP(Water_Shader,_EdgeFoamScale);
				float2 temp_cast_0 = (_EdgeFoamScale_Instance).xx;
				float2 _Direction_Instance = UNITY_ACCESS_INSTANCED_PROP(Water_Shader,_Direction);
				float2 _direction13 = _Direction_Instance;
				float _Speed_Instance = UNITY_ACCESS_INSTANCED_PROP(Water_Shader,_Speed);
				float _speed14 = _Speed_Instance;
				float2 texCoord56 = IN.ase_texcoord3.xy * temp_cast_0 + ( ( _direction13 * ( _speed14 / 10.0 ) ) * _TimeParameters.x );
				float2 temp_cast_1 = (_EdgeFoamScale_Instance).xx;
				float2 texCoord60 = IN.ase_texcoord3.xy * temp_cast_1 + ( 1.0 - ( ( _direction13 * _speed14 ) * _TimeParameters.x ) );
				float _foam82 = step( ( ( 1.0 - _depthNonSmoothStepped134 ) * _FoamCuttoff_Instance ) , ( tex2D( _NoiseTexture, texCoord56 ).a + tex2D( _NoiseTexture, texCoord60 ).a ) );
				float lerpResult116 = lerp( lerpResult111 , _FoamAlpha_Instance , _foam82);
				float _Alpha119 = lerpResult116;
				
				float Alpha = _Alpha119;
				float AlphaClipThreshold = 0.5;
				#ifdef ASE_DEPTH_WRITE_ON
				float DepthValue = 0;
				#endif

				#ifdef _ALPHATEST_ON
					clip(Alpha - AlphaClipThreshold);
				#endif

				#ifdef LOD_FADE_CROSSFADE
					LODDitheringTransition( IN.clipPos.xyz, unity_LODFade.x );
				#endif
				#ifdef ASE_DEPTH_WRITE_ON
				outputDepth = DepthValue;
				#endif

				return 0;
			}
			ENDHLSL
		}
		
		
		Pass
		{
			
			Name "Meta"
			Tags { "LightMode"="Meta" }

			Cull Off

			HLSLPROGRAM
			
			#define _NORMAL_DROPOFF_TS 1
			#pragma multi_compile _ LOD_FADE_CROSSFADE
			#pragma multi_compile_fog
			#define ASE_FOG 1
			#define ASE_SRP_VERSION 100400
			#define REQUIRE_DEPTH_TEXTURE 1

			
			#pragma vertex vert
			#pragma fragment frag

			#define SHADERPASS_META

			#include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
			#include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/MetaInput.hlsl"
			#include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/ShaderGraphFunctions.hlsl"
			#include "Packages/com.unity.render-pipelines.core/ShaderLibrary/Color.hlsl"

			#define ASE_NEEDS_FRAG_WORLD_POSITION
			#pragma multi_compile_instancing


			#pragma shader_feature _ _SMOOTHNESS_TEXTURE_ALBEDO_CHANNEL_A

			struct VertexInput
			{
				float4 vertex : POSITION;
				float3 ase_normal : NORMAL;
				float4 texcoord1 : TEXCOORD1;
				float4 texcoord2 : TEXCOORD2;
				float4 ase_texcoord : TEXCOORD0;
				UNITY_VERTEX_INPUT_INSTANCE_ID
			};

			struct VertexOutput
			{
				float4 clipPos : SV_POSITION;
				#if defined(ASE_NEEDS_FRAG_WORLD_POSITION)
				float3 worldPos : TEXCOORD0;
				#endif
				#if defined(REQUIRES_VERTEX_SHADOW_COORD_INTERPOLATOR) && defined(ASE_NEEDS_FRAG_SHADOWCOORDS)
				float4 shadowCoord : TEXCOORD1;
				#endif
				float4 ase_texcoord2 : TEXCOORD2;
				float4 ase_texcoord3 : TEXCOORD3;
				UNITY_VERTEX_INPUT_INSTANCE_ID
				UNITY_VERTEX_OUTPUT_STEREO
			};

			CBUFFER_START(UnityPerMaterial)
						#ifdef _TRANSMISSION_ASE
				float _TransmissionShadow;
			#endif
			#ifdef _TRANSLUCENCY_ASE
				float _TransStrength;
				float _TransNormal;
				float _TransScattering;
				float _TransDirect;
				float _TransAmbient;
				float _TransShadow;
			#endif
			#ifdef TESSELLATION_ON
				float _TessPhongStrength;
				float _TessValue;
				float _TessMin;
				float _TessMax;
				float _TessEdgeLength;
				float _TessMaxDisp;
			#endif
			CBUFFER_END
			uniform float4 _CameraDepthTexture_TexelSize;
			sampler2D _MainTexture;
			sampler2D _NoiseTexture;
			UNITY_INSTANCING_BUFFER_START(Water_Shader)
				UNITY_DEFINE_INSTANCED_PROP(float4, _SurfaceColor)
				UNITY_DEFINE_INSTANCED_PROP(float4, _DeepColor)
				UNITY_DEFINE_INSTANCED_PROP(float4, _IceColor)
				UNITY_DEFINE_INSTANCED_PROP(float4, _FoamColor)
				UNITY_DEFINE_INSTANCED_PROP(float4, _EdgeFoamColor)
				UNITY_DEFINE_INSTANCED_PROP(float2, _DepthSmoothStep)
				UNITY_DEFINE_INSTANCED_PROP(float2, _SoftSelect)
				UNITY_DEFINE_INSTANCED_PROP(float2, _windDirection)
				UNITY_DEFINE_INSTANCED_PROP(float2, _IceSmoothStep)
				UNITY_DEFINE_INSTANCED_PROP(float2, _Direction)
				UNITY_DEFINE_INSTANCED_PROP(float, _IceStrength)
				UNITY_DEFINE_INSTANCED_PROP(float, _IceTextureScale)
				UNITY_DEFINE_INSTANCED_PROP(float, _Speed)
				UNITY_DEFINE_INSTANCED_PROP(float, _FoamCuttoff)
				UNITY_DEFINE_INSTANCED_PROP(float, _Alpha)
				UNITY_DEFINE_INSTANCED_PROP(float, _WaterNoiseMultiplyer)
				UNITY_DEFINE_INSTANCED_PROP(float, _textureSize)
				UNITY_DEFINE_INSTANCED_PROP(float, _EdgePower)
				UNITY_DEFINE_INSTANCED_PROP(float, _Distance)
				UNITY_DEFINE_INSTANCED_PROP(float, _EdgeFoamScale)
				UNITY_DEFINE_INSTANCED_PROP(float, _FoamAlpha)
			UNITY_INSTANCING_BUFFER_END(Water_Shader)


			
			VertexOutput VertexFunction( VertexInput v  )
			{
				VertexOutput o = (VertexOutput)0;
				UNITY_SETUP_INSTANCE_ID(v);
				UNITY_TRANSFER_INSTANCE_ID(v, o);
				UNITY_INITIALIZE_VERTEX_OUTPUT_STEREO(o);

				float4 ase_clipPos = TransformObjectToHClip((v.vertex).xyz);
				float4 screenPos = ComputeScreenPos(ase_clipPos);
				o.ase_texcoord2 = screenPos;
				
				o.ase_texcoord3.xy = v.ase_texcoord.xy;
				
				//setting value to unused interpolator channels and avoid initialization warnings
				o.ase_texcoord3.zw = 0;
				
				#ifdef ASE_ABSOLUTE_VERTEX_POS
					float3 defaultVertexValue = v.vertex.xyz;
				#else
					float3 defaultVertexValue = float3(0, 0, 0);
				#endif
				float3 vertexValue = defaultVertexValue;
				#ifdef ASE_ABSOLUTE_VERTEX_POS
					v.vertex.xyz = vertexValue;
				#else
					v.vertex.xyz += vertexValue;
				#endif

				v.ase_normal = v.ase_normal;

				float3 positionWS = TransformObjectToWorld( v.vertex.xyz );
				#if defined(ASE_NEEDS_FRAG_WORLD_POSITION)
				o.worldPos = positionWS;
				#endif

				o.clipPos = MetaVertexPosition( v.vertex, v.texcoord1.xy, v.texcoord1.xy, unity_LightmapST, unity_DynamicLightmapST );
				#if defined(REQUIRES_VERTEX_SHADOW_COORD_INTERPOLATOR) && defined(ASE_NEEDS_FRAG_SHADOWCOORDS)
					VertexPositionInputs vertexInput = (VertexPositionInputs)0;
					vertexInput.positionWS = positionWS;
					vertexInput.positionCS = o.clipPos;
					o.shadowCoord = GetShadowCoord( vertexInput );
				#endif
				return o;
			}

			#if defined(TESSELLATION_ON)
			struct VertexControl
			{
				float4 vertex : INTERNALTESSPOS;
				float3 ase_normal : NORMAL;
				float4 texcoord1 : TEXCOORD1;
				float4 texcoord2 : TEXCOORD2;
				float4 ase_texcoord : TEXCOORD0;

				UNITY_VERTEX_INPUT_INSTANCE_ID
			};

			struct TessellationFactors
			{
				float edge[3] : SV_TessFactor;
				float inside : SV_InsideTessFactor;
			};

			VertexControl vert ( VertexInput v )
			{
				VertexControl o;
				UNITY_SETUP_INSTANCE_ID(v);
				UNITY_TRANSFER_INSTANCE_ID(v, o);
				o.vertex = v.vertex;
				o.ase_normal = v.ase_normal;
				o.texcoord1 = v.texcoord1;
				o.texcoord2 = v.texcoord2;
				o.ase_texcoord = v.ase_texcoord;
				return o;
			}

			TessellationFactors TessellationFunction (InputPatch<VertexControl,3> v)
			{
				TessellationFactors o;
				float4 tf = 1;
				float tessValue = _TessValue; float tessMin = _TessMin; float tessMax = _TessMax;
				float edgeLength = _TessEdgeLength; float tessMaxDisp = _TessMaxDisp;
				#if defined(ASE_FIXED_TESSELLATION)
				tf = FixedTess( tessValue );
				#elif defined(ASE_DISTANCE_TESSELLATION)
				tf = DistanceBasedTess(v[0].vertex, v[1].vertex, v[2].vertex, tessValue, tessMin, tessMax, GetObjectToWorldMatrix(), _WorldSpaceCameraPos );
				#elif defined(ASE_LENGTH_TESSELLATION)
				tf = EdgeLengthBasedTess(v[0].vertex, v[1].vertex, v[2].vertex, edgeLength, GetObjectToWorldMatrix(), _WorldSpaceCameraPos, _ScreenParams );
				#elif defined(ASE_LENGTH_CULL_TESSELLATION)
				tf = EdgeLengthBasedTessCull(v[0].vertex, v[1].vertex, v[2].vertex, edgeLength, tessMaxDisp, GetObjectToWorldMatrix(), _WorldSpaceCameraPos, _ScreenParams, unity_CameraWorldClipPlanes );
				#endif
				o.edge[0] = tf.x; o.edge[1] = tf.y; o.edge[2] = tf.z; o.inside = tf.w;
				return o;
			}

			[domain("tri")]
			[partitioning("fractional_odd")]
			[outputtopology("triangle_cw")]
			[patchconstantfunc("TessellationFunction")]
			[outputcontrolpoints(3)]
			VertexControl HullFunction(InputPatch<VertexControl, 3> patch, uint id : SV_OutputControlPointID)
			{
			   return patch[id];
			}

			[domain("tri")]
			VertexOutput DomainFunction(TessellationFactors factors, OutputPatch<VertexControl, 3> patch, float3 bary : SV_DomainLocation)
			{
				VertexInput o = (VertexInput) 0;
				o.vertex = patch[0].vertex * bary.x + patch[1].vertex * bary.y + patch[2].vertex * bary.z;
				o.ase_normal = patch[0].ase_normal * bary.x + patch[1].ase_normal * bary.y + patch[2].ase_normal * bary.z;
				o.texcoord1 = patch[0].texcoord1 * bary.x + patch[1].texcoord1 * bary.y + patch[2].texcoord1 * bary.z;
				o.texcoord2 = patch[0].texcoord2 * bary.x + patch[1].texcoord2 * bary.y + patch[2].texcoord2 * bary.z;
				o.ase_texcoord = patch[0].ase_texcoord * bary.x + patch[1].ase_texcoord * bary.y + patch[2].ase_texcoord * bary.z;
				#if defined(ASE_PHONG_TESSELLATION)
				float3 pp[3];
				for (int i = 0; i < 3; ++i)
					pp[i] = o.vertex.xyz - patch[i].ase_normal * (dot(o.vertex.xyz, patch[i].ase_normal) - dot(patch[i].vertex.xyz, patch[i].ase_normal));
				float phongStrength = _TessPhongStrength;
				o.vertex.xyz = phongStrength * (pp[0]*bary.x + pp[1]*bary.y + pp[2]*bary.z) + (1.0f-phongStrength) * o.vertex.xyz;
				#endif
				UNITY_TRANSFER_INSTANCE_ID(patch[0], o);
				return VertexFunction(o);
			}
			#else
			VertexOutput vert ( VertexInput v )
			{
				return VertexFunction( v );
			}
			#endif

			half4 frag(VertexOutput IN  ) : SV_TARGET
			{
				UNITY_SETUP_INSTANCE_ID(IN);
				UNITY_SETUP_STEREO_EYE_INDEX_POST_VERTEX( IN );

				#if defined(ASE_NEEDS_FRAG_WORLD_POSITION)
				float3 WorldPosition = IN.worldPos;
				#endif
				float4 ShadowCoords = float4( 0, 0, 0, 0 );

				#if defined(ASE_NEEDS_FRAG_SHADOWCOORDS)
					#if defined(REQUIRES_VERTEX_SHADOW_COORD_INTERPOLATOR)
						ShadowCoords = IN.shadowCoord;
					#elif defined(MAIN_LIGHT_CALCULATE_SHADOWS)
						ShadowCoords = TransformWorldToShadowCoord( WorldPosition );
					#endif
				#endif

				float4 _SurfaceColor_Instance = UNITY_ACCESS_INSTANCED_PROP(Water_Shader,_SurfaceColor);
				float4 _DeepColor_Instance = UNITY_ACCESS_INSTANCED_PROP(Water_Shader,_DeepColor);
				float2 _DepthSmoothStep_Instance = UNITY_ACCESS_INSTANCED_PROP(Water_Shader,_DepthSmoothStep);
				float4 screenPos = IN.ase_texcoord2;
				float4 ase_screenPosNorm = screenPos / screenPos.w;
				ase_screenPosNorm.z = ( UNITY_NEAR_CLIP_VALUE >= 0 ) ? ase_screenPosNorm.z : ase_screenPosNorm.z * 0.5 + 0.5;
				float _Distance_Instance = UNITY_ACCESS_INSTANCED_PROP(Water_Shader,_Distance);
				float screenDepth17 = LinearEyeDepth(SHADERGRAPH_SAMPLE_SCENE_DEPTH( ase_screenPosNorm.xy ),_ZBufferParams);
				float distanceDepth17 = abs( ( screenDepth17 - LinearEyeDepth( ase_screenPosNorm.z,_ZBufferParams ) ) / ( _Distance_Instance ) );
				float _EdgePower_Instance = UNITY_ACCESS_INSTANCED_PROP(Water_Shader,_EdgePower);
				float clampResult40 = clamp( ( ( 1.0 - distanceDepth17 ) * _EdgePower_Instance ) , 0.0 , 1.0 );
				float smoothstepResult132 = smoothstep( _DepthSmoothStep_Instance.x , _DepthSmoothStep_Instance.y , clampResult40);
				float DepthFadeBasic52 = smoothstepResult132;
				float4 lerpResult91 = lerp( _SurfaceColor_Instance , _DeepColor_Instance , ( 1.0 - DepthFadeBasic52 ));
				float2 _SoftSelect_Instance = UNITY_ACCESS_INSTANCED_PROP(Water_Shader,_SoftSelect);
				float2 _windDirection_Instance = UNITY_ACCESS_INSTANCED_PROP(Water_Shader,_windDirection);
				float2 _windDirection180 = _windDirection_Instance;
				float _textureSize_Instance = UNITY_ACCESS_INSTANCED_PROP(Water_Shader,_textureSize);
				float2 panner51 = ( 1.0 * _Time.y * _windDirection180 + (( WorldPosition / _textureSize_Instance )).xz);
				float smoothstepResult65 = smoothstep( _SoftSelect_Instance.x , _SoftSelect_Instance.y , tex2D( _MainTexture, panner51 ).a);
				float2 panner50 = ( 1.0 * _Time.y * ( ( 1.0 - _windDirection180 ) * 0.01 ) + (( WorldPosition / 94.0 )).xz);
				float lerpResult75 = lerp( smoothstepResult65 , 0.0 , ( tex2D( _MainTexture, panner50 ).a * 3.0 ));
				float _WaterNoiseMultiplyer_Instance = UNITY_ACCESS_INSTANCED_PROP(Water_Shader,_WaterNoiseMultiplyer);
				float _Wind85 = ( lerpResult75 * _WaterNoiseMultiplyer_Instance );
				float4 lerpResult99 = lerp( lerpResult91 , float4( 1,1,1,0 ) , _Wind85);
				float4 _FoamColor_Instance = UNITY_ACCESS_INSTANCED_PROP(Water_Shader,_FoamColor);
				float _depthNonSmoothStepped134 = clampResult40;
				float _FoamCuttoff_Instance = UNITY_ACCESS_INSTANCED_PROP(Water_Shader,_FoamCuttoff);
				float _EdgeFoamScale_Instance = UNITY_ACCESS_INSTANCED_PROP(Water_Shader,_EdgeFoamScale);
				float2 temp_cast_0 = (_EdgeFoamScale_Instance).xx;
				float2 _Direction_Instance = UNITY_ACCESS_INSTANCED_PROP(Water_Shader,_Direction);
				float2 _direction13 = _Direction_Instance;
				float _Speed_Instance = UNITY_ACCESS_INSTANCED_PROP(Water_Shader,_Speed);
				float _speed14 = _Speed_Instance;
				float2 texCoord56 = IN.ase_texcoord3.xy * temp_cast_0 + ( ( _direction13 * ( _speed14 / 10.0 ) ) * _TimeParameters.x );
				float2 temp_cast_1 = (_EdgeFoamScale_Instance).xx;
				float2 texCoord60 = IN.ase_texcoord3.xy * temp_cast_1 + ( 1.0 - ( ( _direction13 * _speed14 ) * _TimeParameters.x ) );
				float _foam82 = step( ( ( 1.0 - _depthNonSmoothStepped134 ) * _FoamCuttoff_Instance ) , ( tex2D( _NoiseTexture, texCoord56 ).a + tex2D( _NoiseTexture, texCoord60 ).a ) );
				float4 _EdgeFoamColor_Instance = UNITY_ACCESS_INSTANCED_PROP(Water_Shader,_EdgeFoamColor);
				float4 lerpResult104 = lerp( lerpResult99 , _FoamColor_Instance , ( _foam82 * _EdgeFoamColor_Instance ));
				float4 _colour110 = lerpResult104;
				float4 _IceColor_Instance = UNITY_ACCESS_INSTANCED_PROP(Water_Shader,_IceColor);
				float4 appendResult98 = (float4(_IceColor_Instance.r , _IceColor_Instance.g , _IceColor_Instance.b , 0.0));
				float2 _IceSmoothStep_Instance = UNITY_ACCESS_INSTANCED_PROP(Water_Shader,_IceSmoothStep);
				float _IceTextureScale_Instance = UNITY_ACCESS_INSTANCED_PROP(Water_Shader,_IceTextureScale);
				float2 temp_cast_2 = (_IceTextureScale_Instance).xx;
				float2 texCoord70 = IN.ase_texcoord3.xy * temp_cast_2 + float2( 0,0 );
				float smoothstepResult83 = smoothstep( -1.0 , 1.0 , tex2D( _NoiseTexture, texCoord70 ).b);
				float smoothstepResult101 = smoothstep( _IceSmoothStep_Instance.x , _IceSmoothStep_Instance.y , ( smoothstepResult83 - tex2D( _NoiseTexture, ( texCoord70 * float2( 2,2 ) ) ).b ));
				float4 clampResult108 = clamp( ( appendResult98 * smoothstepResult101 ) , float4( 0,0,0,0 ) , float4( 1,1,1,0 ) );
				float _IceStrength_Instance = UNITY_ACCESS_INSTANCED_PROP(Water_Shader,_IceStrength);
				float4 _albedo118 = ( _colour110 + ( clampResult108 * _IceStrength_Instance ) );
				
				float _Alpha_Instance = UNITY_ACCESS_INSTANCED_PROP(Water_Shader,_Alpha);
				float lerpResult106 = lerp( 1.0 , _Alpha_Instance , DepthFadeBasic52);
				float lerpResult111 = lerp( lerpResult106 , 0.0 , _Wind85);
				float _FoamAlpha_Instance = UNITY_ACCESS_INSTANCED_PROP(Water_Shader,_FoamAlpha);
				float lerpResult116 = lerp( lerpResult111 , _FoamAlpha_Instance , _foam82);
				float _Alpha119 = lerpResult116;
				
				
				float3 Albedo = _albedo118.rgb;
				float3 Emission = 0;
				float Alpha = _Alpha119;
				float AlphaClipThreshold = 0.5;

				#ifdef _ALPHATEST_ON
					clip(Alpha - AlphaClipThreshold);
				#endif

				MetaInput metaInput = (MetaInput)0;
				metaInput.Albedo = Albedo;
				metaInput.Emission = Emission;
				
				return MetaFragment(metaInput);
			}
			ENDHLSL
		}

		
		Pass
		{
			
			Name "Universal2D"
			Tags { "LightMode"="Universal2D" }

			Blend SrcAlpha OneMinusSrcAlpha, One OneMinusSrcAlpha
			ZWrite Off
			ZTest LEqual
			Offset 0 , 0
			ColorMask RGBA

			HLSLPROGRAM
			
			#define _NORMAL_DROPOFF_TS 1
			#pragma multi_compile _ LOD_FADE_CROSSFADE
			#pragma multi_compile_fog
			#define ASE_FOG 1
			#define ASE_SRP_VERSION 100400
			#define REQUIRE_DEPTH_TEXTURE 1

			
			#pragma vertex vert
			#pragma fragment frag

			#define SHADERPASS_2D

			#include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
			#include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Lighting.hlsl"
			#include "Packages/com.unity.render-pipelines.core/ShaderLibrary/Color.hlsl"
			#include "Packages/com.unity.render-pipelines.core/ShaderLibrary/UnityInstancing.hlsl"
			#include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/ShaderGraphFunctions.hlsl"
			
			#define ASE_NEEDS_FRAG_WORLD_POSITION
			#pragma multi_compile_instancing


			#pragma shader_feature _ _SMOOTHNESS_TEXTURE_ALBEDO_CHANNEL_A

			struct VertexInput
			{
				float4 vertex : POSITION;
				float3 ase_normal : NORMAL;
				float4 ase_texcoord : TEXCOORD0;
				UNITY_VERTEX_INPUT_INSTANCE_ID
			};

			struct VertexOutput
			{
				float4 clipPos : SV_POSITION;
				#if defined(ASE_NEEDS_FRAG_WORLD_POSITION)
				float3 worldPos : TEXCOORD0;
				#endif
				#if defined(REQUIRES_VERTEX_SHADOW_COORD_INTERPOLATOR) && defined(ASE_NEEDS_FRAG_SHADOWCOORDS)
				float4 shadowCoord : TEXCOORD1;
				#endif
				float4 ase_texcoord2 : TEXCOORD2;
				float4 ase_texcoord3 : TEXCOORD3;
				UNITY_VERTEX_INPUT_INSTANCE_ID
				UNITY_VERTEX_OUTPUT_STEREO
			};

			CBUFFER_START(UnityPerMaterial)
						#ifdef _TRANSMISSION_ASE
				float _TransmissionShadow;
			#endif
			#ifdef _TRANSLUCENCY_ASE
				float _TransStrength;
				float _TransNormal;
				float _TransScattering;
				float _TransDirect;
				float _TransAmbient;
				float _TransShadow;
			#endif
			#ifdef TESSELLATION_ON
				float _TessPhongStrength;
				float _TessValue;
				float _TessMin;
				float _TessMax;
				float _TessEdgeLength;
				float _TessMaxDisp;
			#endif
			CBUFFER_END
			uniform float4 _CameraDepthTexture_TexelSize;
			sampler2D _MainTexture;
			sampler2D _NoiseTexture;
			UNITY_INSTANCING_BUFFER_START(Water_Shader)
				UNITY_DEFINE_INSTANCED_PROP(float4, _SurfaceColor)
				UNITY_DEFINE_INSTANCED_PROP(float4, _DeepColor)
				UNITY_DEFINE_INSTANCED_PROP(float4, _IceColor)
				UNITY_DEFINE_INSTANCED_PROP(float4, _FoamColor)
				UNITY_DEFINE_INSTANCED_PROP(float4, _EdgeFoamColor)
				UNITY_DEFINE_INSTANCED_PROP(float2, _DepthSmoothStep)
				UNITY_DEFINE_INSTANCED_PROP(float2, _SoftSelect)
				UNITY_DEFINE_INSTANCED_PROP(float2, _windDirection)
				UNITY_DEFINE_INSTANCED_PROP(float2, _IceSmoothStep)
				UNITY_DEFINE_INSTANCED_PROP(float2, _Direction)
				UNITY_DEFINE_INSTANCED_PROP(float, _IceStrength)
				UNITY_DEFINE_INSTANCED_PROP(float, _IceTextureScale)
				UNITY_DEFINE_INSTANCED_PROP(float, _Speed)
				UNITY_DEFINE_INSTANCED_PROP(float, _FoamCuttoff)
				UNITY_DEFINE_INSTANCED_PROP(float, _Alpha)
				UNITY_DEFINE_INSTANCED_PROP(float, _WaterNoiseMultiplyer)
				UNITY_DEFINE_INSTANCED_PROP(float, _textureSize)
				UNITY_DEFINE_INSTANCED_PROP(float, _EdgePower)
				UNITY_DEFINE_INSTANCED_PROP(float, _Distance)
				UNITY_DEFINE_INSTANCED_PROP(float, _EdgeFoamScale)
				UNITY_DEFINE_INSTANCED_PROP(float, _FoamAlpha)
			UNITY_INSTANCING_BUFFER_END(Water_Shader)


			
			VertexOutput VertexFunction( VertexInput v  )
			{
				VertexOutput o = (VertexOutput)0;
				UNITY_SETUP_INSTANCE_ID( v );
				UNITY_TRANSFER_INSTANCE_ID( v, o );
				UNITY_INITIALIZE_VERTEX_OUTPUT_STEREO( o );

				float4 ase_clipPos = TransformObjectToHClip((v.vertex).xyz);
				float4 screenPos = ComputeScreenPos(ase_clipPos);
				o.ase_texcoord2 = screenPos;
				
				o.ase_texcoord3.xy = v.ase_texcoord.xy;
				
				//setting value to unused interpolator channels and avoid initialization warnings
				o.ase_texcoord3.zw = 0;
				
				#ifdef ASE_ABSOLUTE_VERTEX_POS
					float3 defaultVertexValue = v.vertex.xyz;
				#else
					float3 defaultVertexValue = float3(0, 0, 0);
				#endif
				float3 vertexValue = defaultVertexValue;
				#ifdef ASE_ABSOLUTE_VERTEX_POS
					v.vertex.xyz = vertexValue;
				#else
					v.vertex.xyz += vertexValue;
				#endif

				v.ase_normal = v.ase_normal;

				float3 positionWS = TransformObjectToWorld( v.vertex.xyz );
				float4 positionCS = TransformWorldToHClip( positionWS );

				#if defined(ASE_NEEDS_FRAG_WORLD_POSITION)
				o.worldPos = positionWS;
				#endif

				#if defined(REQUIRES_VERTEX_SHADOW_COORD_INTERPOLATOR) && defined(ASE_NEEDS_FRAG_SHADOWCOORDS)
					VertexPositionInputs vertexInput = (VertexPositionInputs)0;
					vertexInput.positionWS = positionWS;
					vertexInput.positionCS = positionCS;
					o.shadowCoord = GetShadowCoord( vertexInput );
				#endif

				o.clipPos = positionCS;
				return o;
			}

			#if defined(TESSELLATION_ON)
			struct VertexControl
			{
				float4 vertex : INTERNALTESSPOS;
				float3 ase_normal : NORMAL;
				float4 ase_texcoord : TEXCOORD0;

				UNITY_VERTEX_INPUT_INSTANCE_ID
			};

			struct TessellationFactors
			{
				float edge[3] : SV_TessFactor;
				float inside : SV_InsideTessFactor;
			};

			VertexControl vert ( VertexInput v )
			{
				VertexControl o;
				UNITY_SETUP_INSTANCE_ID(v);
				UNITY_TRANSFER_INSTANCE_ID(v, o);
				o.vertex = v.vertex;
				o.ase_normal = v.ase_normal;
				o.ase_texcoord = v.ase_texcoord;
				return o;
			}

			TessellationFactors TessellationFunction (InputPatch<VertexControl,3> v)
			{
				TessellationFactors o;
				float4 tf = 1;
				float tessValue = _TessValue; float tessMin = _TessMin; float tessMax = _TessMax;
				float edgeLength = _TessEdgeLength; float tessMaxDisp = _TessMaxDisp;
				#if defined(ASE_FIXED_TESSELLATION)
				tf = FixedTess( tessValue );
				#elif defined(ASE_DISTANCE_TESSELLATION)
				tf = DistanceBasedTess(v[0].vertex, v[1].vertex, v[2].vertex, tessValue, tessMin, tessMax, GetObjectToWorldMatrix(), _WorldSpaceCameraPos );
				#elif defined(ASE_LENGTH_TESSELLATION)
				tf = EdgeLengthBasedTess(v[0].vertex, v[1].vertex, v[2].vertex, edgeLength, GetObjectToWorldMatrix(), _WorldSpaceCameraPos, _ScreenParams );
				#elif defined(ASE_LENGTH_CULL_TESSELLATION)
				tf = EdgeLengthBasedTessCull(v[0].vertex, v[1].vertex, v[2].vertex, edgeLength, tessMaxDisp, GetObjectToWorldMatrix(), _WorldSpaceCameraPos, _ScreenParams, unity_CameraWorldClipPlanes );
				#endif
				o.edge[0] = tf.x; o.edge[1] = tf.y; o.edge[2] = tf.z; o.inside = tf.w;
				return o;
			}

			[domain("tri")]
			[partitioning("fractional_odd")]
			[outputtopology("triangle_cw")]
			[patchconstantfunc("TessellationFunction")]
			[outputcontrolpoints(3)]
			VertexControl HullFunction(InputPatch<VertexControl, 3> patch, uint id : SV_OutputControlPointID)
			{
			   return patch[id];
			}

			[domain("tri")]
			VertexOutput DomainFunction(TessellationFactors factors, OutputPatch<VertexControl, 3> patch, float3 bary : SV_DomainLocation)
			{
				VertexInput o = (VertexInput) 0;
				o.vertex = patch[0].vertex * bary.x + patch[1].vertex * bary.y + patch[2].vertex * bary.z;
				o.ase_normal = patch[0].ase_normal * bary.x + patch[1].ase_normal * bary.y + patch[2].ase_normal * bary.z;
				o.ase_texcoord = patch[0].ase_texcoord * bary.x + patch[1].ase_texcoord * bary.y + patch[2].ase_texcoord * bary.z;
				#if defined(ASE_PHONG_TESSELLATION)
				float3 pp[3];
				for (int i = 0; i < 3; ++i)
					pp[i] = o.vertex.xyz - patch[i].ase_normal * (dot(o.vertex.xyz, patch[i].ase_normal) - dot(patch[i].vertex.xyz, patch[i].ase_normal));
				float phongStrength = _TessPhongStrength;
				o.vertex.xyz = phongStrength * (pp[0]*bary.x + pp[1]*bary.y + pp[2]*bary.z) + (1.0f-phongStrength) * o.vertex.xyz;
				#endif
				UNITY_TRANSFER_INSTANCE_ID(patch[0], o);
				return VertexFunction(o);
			}
			#else
			VertexOutput vert ( VertexInput v )
			{
				return VertexFunction( v );
			}
			#endif

			half4 frag(VertexOutput IN  ) : SV_TARGET
			{
				UNITY_SETUP_INSTANCE_ID( IN );
				UNITY_SETUP_STEREO_EYE_INDEX_POST_VERTEX( IN );

				#if defined(ASE_NEEDS_FRAG_WORLD_POSITION)
				float3 WorldPosition = IN.worldPos;
				#endif
				float4 ShadowCoords = float4( 0, 0, 0, 0 );

				#if defined(ASE_NEEDS_FRAG_SHADOWCOORDS)
					#if defined(REQUIRES_VERTEX_SHADOW_COORD_INTERPOLATOR)
						ShadowCoords = IN.shadowCoord;
					#elif defined(MAIN_LIGHT_CALCULATE_SHADOWS)
						ShadowCoords = TransformWorldToShadowCoord( WorldPosition );
					#endif
				#endif

				float4 _SurfaceColor_Instance = UNITY_ACCESS_INSTANCED_PROP(Water_Shader,_SurfaceColor);
				float4 _DeepColor_Instance = UNITY_ACCESS_INSTANCED_PROP(Water_Shader,_DeepColor);
				float2 _DepthSmoothStep_Instance = UNITY_ACCESS_INSTANCED_PROP(Water_Shader,_DepthSmoothStep);
				float4 screenPos = IN.ase_texcoord2;
				float4 ase_screenPosNorm = screenPos / screenPos.w;
				ase_screenPosNorm.z = ( UNITY_NEAR_CLIP_VALUE >= 0 ) ? ase_screenPosNorm.z : ase_screenPosNorm.z * 0.5 + 0.5;
				float _Distance_Instance = UNITY_ACCESS_INSTANCED_PROP(Water_Shader,_Distance);
				float screenDepth17 = LinearEyeDepth(SHADERGRAPH_SAMPLE_SCENE_DEPTH( ase_screenPosNorm.xy ),_ZBufferParams);
				float distanceDepth17 = abs( ( screenDepth17 - LinearEyeDepth( ase_screenPosNorm.z,_ZBufferParams ) ) / ( _Distance_Instance ) );
				float _EdgePower_Instance = UNITY_ACCESS_INSTANCED_PROP(Water_Shader,_EdgePower);
				float clampResult40 = clamp( ( ( 1.0 - distanceDepth17 ) * _EdgePower_Instance ) , 0.0 , 1.0 );
				float smoothstepResult132 = smoothstep( _DepthSmoothStep_Instance.x , _DepthSmoothStep_Instance.y , clampResult40);
				float DepthFadeBasic52 = smoothstepResult132;
				float4 lerpResult91 = lerp( _SurfaceColor_Instance , _DeepColor_Instance , ( 1.0 - DepthFadeBasic52 ));
				float2 _SoftSelect_Instance = UNITY_ACCESS_INSTANCED_PROP(Water_Shader,_SoftSelect);
				float2 _windDirection_Instance = UNITY_ACCESS_INSTANCED_PROP(Water_Shader,_windDirection);
				float2 _windDirection180 = _windDirection_Instance;
				float _textureSize_Instance = UNITY_ACCESS_INSTANCED_PROP(Water_Shader,_textureSize);
				float2 panner51 = ( 1.0 * _Time.y * _windDirection180 + (( WorldPosition / _textureSize_Instance )).xz);
				float smoothstepResult65 = smoothstep( _SoftSelect_Instance.x , _SoftSelect_Instance.y , tex2D( _MainTexture, panner51 ).a);
				float2 panner50 = ( 1.0 * _Time.y * ( ( 1.0 - _windDirection180 ) * 0.01 ) + (( WorldPosition / 94.0 )).xz);
				float lerpResult75 = lerp( smoothstepResult65 , 0.0 , ( tex2D( _MainTexture, panner50 ).a * 3.0 ));
				float _WaterNoiseMultiplyer_Instance = UNITY_ACCESS_INSTANCED_PROP(Water_Shader,_WaterNoiseMultiplyer);
				float _Wind85 = ( lerpResult75 * _WaterNoiseMultiplyer_Instance );
				float4 lerpResult99 = lerp( lerpResult91 , float4( 1,1,1,0 ) , _Wind85);
				float4 _FoamColor_Instance = UNITY_ACCESS_INSTANCED_PROP(Water_Shader,_FoamColor);
				float _depthNonSmoothStepped134 = clampResult40;
				float _FoamCuttoff_Instance = UNITY_ACCESS_INSTANCED_PROP(Water_Shader,_FoamCuttoff);
				float _EdgeFoamScale_Instance = UNITY_ACCESS_INSTANCED_PROP(Water_Shader,_EdgeFoamScale);
				float2 temp_cast_0 = (_EdgeFoamScale_Instance).xx;
				float2 _Direction_Instance = UNITY_ACCESS_INSTANCED_PROP(Water_Shader,_Direction);
				float2 _direction13 = _Direction_Instance;
				float _Speed_Instance = UNITY_ACCESS_INSTANCED_PROP(Water_Shader,_Speed);
				float _speed14 = _Speed_Instance;
				float2 texCoord56 = IN.ase_texcoord3.xy * temp_cast_0 + ( ( _direction13 * ( _speed14 / 10.0 ) ) * _TimeParameters.x );
				float2 temp_cast_1 = (_EdgeFoamScale_Instance).xx;
				float2 texCoord60 = IN.ase_texcoord3.xy * temp_cast_1 + ( 1.0 - ( ( _direction13 * _speed14 ) * _TimeParameters.x ) );
				float _foam82 = step( ( ( 1.0 - _depthNonSmoothStepped134 ) * _FoamCuttoff_Instance ) , ( tex2D( _NoiseTexture, texCoord56 ).a + tex2D( _NoiseTexture, texCoord60 ).a ) );
				float4 _EdgeFoamColor_Instance = UNITY_ACCESS_INSTANCED_PROP(Water_Shader,_EdgeFoamColor);
				float4 lerpResult104 = lerp( lerpResult99 , _FoamColor_Instance , ( _foam82 * _EdgeFoamColor_Instance ));
				float4 _colour110 = lerpResult104;
				float4 _IceColor_Instance = UNITY_ACCESS_INSTANCED_PROP(Water_Shader,_IceColor);
				float4 appendResult98 = (float4(_IceColor_Instance.r , _IceColor_Instance.g , _IceColor_Instance.b , 0.0));
				float2 _IceSmoothStep_Instance = UNITY_ACCESS_INSTANCED_PROP(Water_Shader,_IceSmoothStep);
				float _IceTextureScale_Instance = UNITY_ACCESS_INSTANCED_PROP(Water_Shader,_IceTextureScale);
				float2 temp_cast_2 = (_IceTextureScale_Instance).xx;
				float2 texCoord70 = IN.ase_texcoord3.xy * temp_cast_2 + float2( 0,0 );
				float smoothstepResult83 = smoothstep( -1.0 , 1.0 , tex2D( _NoiseTexture, texCoord70 ).b);
				float smoothstepResult101 = smoothstep( _IceSmoothStep_Instance.x , _IceSmoothStep_Instance.y , ( smoothstepResult83 - tex2D( _NoiseTexture, ( texCoord70 * float2( 2,2 ) ) ).b ));
				float4 clampResult108 = clamp( ( appendResult98 * smoothstepResult101 ) , float4( 0,0,0,0 ) , float4( 1,1,1,0 ) );
				float _IceStrength_Instance = UNITY_ACCESS_INSTANCED_PROP(Water_Shader,_IceStrength);
				float4 _albedo118 = ( _colour110 + ( clampResult108 * _IceStrength_Instance ) );
				
				float _Alpha_Instance = UNITY_ACCESS_INSTANCED_PROP(Water_Shader,_Alpha);
				float lerpResult106 = lerp( 1.0 , _Alpha_Instance , DepthFadeBasic52);
				float lerpResult111 = lerp( lerpResult106 , 0.0 , _Wind85);
				float _FoamAlpha_Instance = UNITY_ACCESS_INSTANCED_PROP(Water_Shader,_FoamAlpha);
				float lerpResult116 = lerp( lerpResult111 , _FoamAlpha_Instance , _foam82);
				float _Alpha119 = lerpResult116;
				
				
				float3 Albedo = _albedo118.rgb;
				float Alpha = _Alpha119;
				float AlphaClipThreshold = 0.5;

				half4 color = half4( Albedo, Alpha );

				#ifdef _ALPHATEST_ON
					clip(Alpha - AlphaClipThreshold);
				#endif

				return color;
			}
			ENDHLSL
		}

		
		Pass
		{
			
			Name "DepthNormals"
			Tags { "LightMode"="DepthNormals" }

			ZWrite On
			Blend One Zero
            ZTest LEqual
            ZWrite On

			HLSLPROGRAM
			
			#define _NORMAL_DROPOFF_TS 1
			#pragma multi_compile _ LOD_FADE_CROSSFADE
			#pragma multi_compile_fog
			#define ASE_FOG 1
			#define ASE_SRP_VERSION 100400
			#define REQUIRE_DEPTH_TEXTURE 1

			
			#pragma vertex vert
			#pragma fragment frag

			#define SHADERPASS_DEPTHNORMALSONLY

			#include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
			#include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Lighting.hlsl"
			#include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/ShaderGraphFunctions.hlsl"
			#include "Packages/com.unity.render-pipelines.core/ShaderLibrary/Color.hlsl"

			#define ASE_NEEDS_FRAG_WORLD_POSITION
			#pragma multi_compile_instancing


			struct VertexInput
			{
				float4 vertex : POSITION;
				float3 ase_normal : NORMAL;
				float4 ase_texcoord : TEXCOORD0;
				UNITY_VERTEX_INPUT_INSTANCE_ID
			};

			struct VertexOutput
			{
				float4 clipPos : SV_POSITION;
				#if defined(ASE_NEEDS_FRAG_WORLD_POSITION)
				float3 worldPos : TEXCOORD0;
				#endif
				#if defined(REQUIRES_VERTEX_SHADOW_COORD_INTERPOLATOR) && defined(ASE_NEEDS_FRAG_SHADOWCOORDS)
				float4 shadowCoord : TEXCOORD1;
				#endif
				float3 worldNormal : TEXCOORD2;
				float4 ase_texcoord3 : TEXCOORD3;
				float4 ase_texcoord4 : TEXCOORD4;
				UNITY_VERTEX_INPUT_INSTANCE_ID
				UNITY_VERTEX_OUTPUT_STEREO
			};

			CBUFFER_START(UnityPerMaterial)
						#ifdef _TRANSMISSION_ASE
				float _TransmissionShadow;
			#endif
			#ifdef _TRANSLUCENCY_ASE
				float _TransStrength;
				float _TransNormal;
				float _TransScattering;
				float _TransDirect;
				float _TransAmbient;
				float _TransShadow;
			#endif
			#ifdef TESSELLATION_ON
				float _TessPhongStrength;
				float _TessValue;
				float _TessMin;
				float _TessMax;
				float _TessEdgeLength;
				float _TessMaxDisp;
			#endif
			CBUFFER_END
			uniform float4 _CameraDepthTexture_TexelSize;
			sampler2D _MainTexture;
			sampler2D _NoiseTexture;
			UNITY_INSTANCING_BUFFER_START(Water_Shader)
				UNITY_DEFINE_INSTANCED_PROP(float2, _DepthSmoothStep)
				UNITY_DEFINE_INSTANCED_PROP(float2, _SoftSelect)
				UNITY_DEFINE_INSTANCED_PROP(float2, _windDirection)
				UNITY_DEFINE_INSTANCED_PROP(float2, _Direction)
				UNITY_DEFINE_INSTANCED_PROP(float, _Alpha)
				UNITY_DEFINE_INSTANCED_PROP(float, _Distance)
				UNITY_DEFINE_INSTANCED_PROP(float, _EdgePower)
				UNITY_DEFINE_INSTANCED_PROP(float, _textureSize)
				UNITY_DEFINE_INSTANCED_PROP(float, _WaterNoiseMultiplyer)
				UNITY_DEFINE_INSTANCED_PROP(float, _FoamAlpha)
				UNITY_DEFINE_INSTANCED_PROP(float, _FoamCuttoff)
				UNITY_DEFINE_INSTANCED_PROP(float, _EdgeFoamScale)
				UNITY_DEFINE_INSTANCED_PROP(float, _Speed)
			UNITY_INSTANCING_BUFFER_END(Water_Shader)


			
			VertexOutput VertexFunction( VertexInput v  )
			{
				VertexOutput o = (VertexOutput)0;
				UNITY_SETUP_INSTANCE_ID(v);
				UNITY_TRANSFER_INSTANCE_ID(v, o);
				UNITY_INITIALIZE_VERTEX_OUTPUT_STEREO(o);

				float4 ase_clipPos = TransformObjectToHClip((v.vertex).xyz);
				float4 screenPos = ComputeScreenPos(ase_clipPos);
				o.ase_texcoord3 = screenPos;
				
				o.ase_texcoord4.xy = v.ase_texcoord.xy;
				
				//setting value to unused interpolator channels and avoid initialization warnings
				o.ase_texcoord4.zw = 0;
				#ifdef ASE_ABSOLUTE_VERTEX_POS
					float3 defaultVertexValue = v.vertex.xyz;
				#else
					float3 defaultVertexValue = float3(0, 0, 0);
				#endif
				float3 vertexValue = defaultVertexValue;
				#ifdef ASE_ABSOLUTE_VERTEX_POS
					v.vertex.xyz = vertexValue;
				#else
					v.vertex.xyz += vertexValue;
				#endif

				v.ase_normal = v.ase_normal;
				float3 positionWS = TransformObjectToWorld( v.vertex.xyz );
				float3 normalWS = TransformObjectToWorldNormal( v.ase_normal );
				float4 positionCS = TransformWorldToHClip( positionWS );

				#if defined(ASE_NEEDS_FRAG_WORLD_POSITION)
				o.worldPos = positionWS;
				#endif

				o.worldNormal = normalWS;

				#if defined(REQUIRES_VERTEX_SHADOW_COORD_INTERPOLATOR) && defined(ASE_NEEDS_FRAG_SHADOWCOORDS)
					VertexPositionInputs vertexInput = (VertexPositionInputs)0;
					vertexInput.positionWS = positionWS;
					vertexInput.positionCS = positionCS;
					o.shadowCoord = GetShadowCoord( vertexInput );
				#endif
				o.clipPos = positionCS;
				return o;
			}

			#if defined(TESSELLATION_ON)
			struct VertexControl
			{
				float4 vertex : INTERNALTESSPOS;
				float3 ase_normal : NORMAL;
				float4 ase_texcoord : TEXCOORD0;

				UNITY_VERTEX_INPUT_INSTANCE_ID
			};

			struct TessellationFactors
			{
				float edge[3] : SV_TessFactor;
				float inside : SV_InsideTessFactor;
			};

			VertexControl vert ( VertexInput v )
			{
				VertexControl o;
				UNITY_SETUP_INSTANCE_ID(v);
				UNITY_TRANSFER_INSTANCE_ID(v, o);
				o.vertex = v.vertex;
				o.ase_normal = v.ase_normal;
				o.ase_texcoord = v.ase_texcoord;
				return o;
			}

			TessellationFactors TessellationFunction (InputPatch<VertexControl,3> v)
			{
				TessellationFactors o;
				float4 tf = 1;
				float tessValue = _TessValue; float tessMin = _TessMin; float tessMax = _TessMax;
				float edgeLength = _TessEdgeLength; float tessMaxDisp = _TessMaxDisp;
				#if defined(ASE_FIXED_TESSELLATION)
				tf = FixedTess( tessValue );
				#elif defined(ASE_DISTANCE_TESSELLATION)
				tf = DistanceBasedTess(v[0].vertex, v[1].vertex, v[2].vertex, tessValue, tessMin, tessMax, GetObjectToWorldMatrix(), _WorldSpaceCameraPos );
				#elif defined(ASE_LENGTH_TESSELLATION)
				tf = EdgeLengthBasedTess(v[0].vertex, v[1].vertex, v[2].vertex, edgeLength, GetObjectToWorldMatrix(), _WorldSpaceCameraPos, _ScreenParams );
				#elif defined(ASE_LENGTH_CULL_TESSELLATION)
				tf = EdgeLengthBasedTessCull(v[0].vertex, v[1].vertex, v[2].vertex, edgeLength, tessMaxDisp, GetObjectToWorldMatrix(), _WorldSpaceCameraPos, _ScreenParams, unity_CameraWorldClipPlanes );
				#endif
				o.edge[0] = tf.x; o.edge[1] = tf.y; o.edge[2] = tf.z; o.inside = tf.w;
				return o;
			}

			[domain("tri")]
			[partitioning("fractional_odd")]
			[outputtopology("triangle_cw")]
			[patchconstantfunc("TessellationFunction")]
			[outputcontrolpoints(3)]
			VertexControl HullFunction(InputPatch<VertexControl, 3> patch, uint id : SV_OutputControlPointID)
			{
			   return patch[id];
			}

			[domain("tri")]
			VertexOutput DomainFunction(TessellationFactors factors, OutputPatch<VertexControl, 3> patch, float3 bary : SV_DomainLocation)
			{
				VertexInput o = (VertexInput) 0;
				o.vertex = patch[0].vertex * bary.x + patch[1].vertex * bary.y + patch[2].vertex * bary.z;
				o.ase_normal = patch[0].ase_normal * bary.x + patch[1].ase_normal * bary.y + patch[2].ase_normal * bary.z;
				o.ase_texcoord = patch[0].ase_texcoord * bary.x + patch[1].ase_texcoord * bary.y + patch[2].ase_texcoord * bary.z;
				#if defined(ASE_PHONG_TESSELLATION)
				float3 pp[3];
				for (int i = 0; i < 3; ++i)
					pp[i] = o.vertex.xyz - patch[i].ase_normal * (dot(o.vertex.xyz, patch[i].ase_normal) - dot(patch[i].vertex.xyz, patch[i].ase_normal));
				float phongStrength = _TessPhongStrength;
				o.vertex.xyz = phongStrength * (pp[0]*bary.x + pp[1]*bary.y + pp[2]*bary.z) + (1.0f-phongStrength) * o.vertex.xyz;
				#endif
				UNITY_TRANSFER_INSTANCE_ID(patch[0], o);
				return VertexFunction(o);
			}
			#else
			VertexOutput vert ( VertexInput v )
			{
				return VertexFunction( v );
			}
			#endif

			#if defined(ASE_EARLY_Z_DEPTH_OPTIMIZE)
				#define ASE_SV_DEPTH SV_DepthLessEqual  
			#else
				#define ASE_SV_DEPTH SV_Depth
			#endif
			half4 frag(	VertexOutput IN 
						#ifdef ASE_DEPTH_WRITE_ON
						,out float outputDepth : ASE_SV_DEPTH
						#endif
						 ) : SV_TARGET
			{
				UNITY_SETUP_INSTANCE_ID(IN);
				UNITY_SETUP_STEREO_EYE_INDEX_POST_VERTEX( IN );

				#if defined(ASE_NEEDS_FRAG_WORLD_POSITION)
				float3 WorldPosition = IN.worldPos;
				#endif
				float4 ShadowCoords = float4( 0, 0, 0, 0 );

				#if defined(ASE_NEEDS_FRAG_SHADOWCOORDS)
					#if defined(REQUIRES_VERTEX_SHADOW_COORD_INTERPOLATOR)
						ShadowCoords = IN.shadowCoord;
					#elif defined(MAIN_LIGHT_CALCULATE_SHADOWS)
						ShadowCoords = TransformWorldToShadowCoord( WorldPosition );
					#endif
				#endif

				float _Alpha_Instance = UNITY_ACCESS_INSTANCED_PROP(Water_Shader,_Alpha);
				float2 _DepthSmoothStep_Instance = UNITY_ACCESS_INSTANCED_PROP(Water_Shader,_DepthSmoothStep);
				float4 screenPos = IN.ase_texcoord3;
				float4 ase_screenPosNorm = screenPos / screenPos.w;
				ase_screenPosNorm.z = ( UNITY_NEAR_CLIP_VALUE >= 0 ) ? ase_screenPosNorm.z : ase_screenPosNorm.z * 0.5 + 0.5;
				float _Distance_Instance = UNITY_ACCESS_INSTANCED_PROP(Water_Shader,_Distance);
				float screenDepth17 = LinearEyeDepth(SHADERGRAPH_SAMPLE_SCENE_DEPTH( ase_screenPosNorm.xy ),_ZBufferParams);
				float distanceDepth17 = abs( ( screenDepth17 - LinearEyeDepth( ase_screenPosNorm.z,_ZBufferParams ) ) / ( _Distance_Instance ) );
				float _EdgePower_Instance = UNITY_ACCESS_INSTANCED_PROP(Water_Shader,_EdgePower);
				float clampResult40 = clamp( ( ( 1.0 - distanceDepth17 ) * _EdgePower_Instance ) , 0.0 , 1.0 );
				float smoothstepResult132 = smoothstep( _DepthSmoothStep_Instance.x , _DepthSmoothStep_Instance.y , clampResult40);
				float DepthFadeBasic52 = smoothstepResult132;
				float lerpResult106 = lerp( 1.0 , _Alpha_Instance , DepthFadeBasic52);
				float2 _SoftSelect_Instance = UNITY_ACCESS_INSTANCED_PROP(Water_Shader,_SoftSelect);
				float2 _windDirection_Instance = UNITY_ACCESS_INSTANCED_PROP(Water_Shader,_windDirection);
				float2 _windDirection180 = _windDirection_Instance;
				float _textureSize_Instance = UNITY_ACCESS_INSTANCED_PROP(Water_Shader,_textureSize);
				float2 panner51 = ( 1.0 * _Time.y * _windDirection180 + (( WorldPosition / _textureSize_Instance )).xz);
				float smoothstepResult65 = smoothstep( _SoftSelect_Instance.x , _SoftSelect_Instance.y , tex2D( _MainTexture, panner51 ).a);
				float2 panner50 = ( 1.0 * _Time.y * ( ( 1.0 - _windDirection180 ) * 0.01 ) + (( WorldPosition / 94.0 )).xz);
				float lerpResult75 = lerp( smoothstepResult65 , 0.0 , ( tex2D( _MainTexture, panner50 ).a * 3.0 ));
				float _WaterNoiseMultiplyer_Instance = UNITY_ACCESS_INSTANCED_PROP(Water_Shader,_WaterNoiseMultiplyer);
				float _Wind85 = ( lerpResult75 * _WaterNoiseMultiplyer_Instance );
				float lerpResult111 = lerp( lerpResult106 , 0.0 , _Wind85);
				float _FoamAlpha_Instance = UNITY_ACCESS_INSTANCED_PROP(Water_Shader,_FoamAlpha);
				float _depthNonSmoothStepped134 = clampResult40;
				float _FoamCuttoff_Instance = UNITY_ACCESS_INSTANCED_PROP(Water_Shader,_FoamCuttoff);
				float _EdgeFoamScale_Instance = UNITY_ACCESS_INSTANCED_PROP(Water_Shader,_EdgeFoamScale);
				float2 temp_cast_0 = (_EdgeFoamScale_Instance).xx;
				float2 _Direction_Instance = UNITY_ACCESS_INSTANCED_PROP(Water_Shader,_Direction);
				float2 _direction13 = _Direction_Instance;
				float _Speed_Instance = UNITY_ACCESS_INSTANCED_PROP(Water_Shader,_Speed);
				float _speed14 = _Speed_Instance;
				float2 texCoord56 = IN.ase_texcoord4.xy * temp_cast_0 + ( ( _direction13 * ( _speed14 / 10.0 ) ) * _TimeParameters.x );
				float2 temp_cast_1 = (_EdgeFoamScale_Instance).xx;
				float2 texCoord60 = IN.ase_texcoord4.xy * temp_cast_1 + ( 1.0 - ( ( _direction13 * _speed14 ) * _TimeParameters.x ) );
				float _foam82 = step( ( ( 1.0 - _depthNonSmoothStepped134 ) * _FoamCuttoff_Instance ) , ( tex2D( _NoiseTexture, texCoord56 ).a + tex2D( _NoiseTexture, texCoord60 ).a ) );
				float lerpResult116 = lerp( lerpResult111 , _FoamAlpha_Instance , _foam82);
				float _Alpha119 = lerpResult116;
				
				float Alpha = _Alpha119;
				float AlphaClipThreshold = 0.5;
				#ifdef ASE_DEPTH_WRITE_ON
				float DepthValue = 0;
				#endif

				#ifdef _ALPHATEST_ON
					clip(Alpha - AlphaClipThreshold);
				#endif

				#ifdef LOD_FADE_CROSSFADE
					LODDitheringTransition( IN.clipPos.xyz, unity_LODFade.x );
				#endif
				
				#ifdef ASE_DEPTH_WRITE_ON
				outputDepth = DepthValue;
				#endif
				
				return float4(PackNormalOctRectEncode(TransformWorldToViewDir(IN.worldNormal, true)), 0.0, 0.0);
			}
			ENDHLSL
		}

		
		Pass
		{
			
			Name "GBuffer"
			Tags { "LightMode"="UniversalGBuffer" }
			
			Blend SrcAlpha OneMinusSrcAlpha, One OneMinusSrcAlpha
			ZWrite Off
			ZTest LEqual
			Offset 0 , 0
			ColorMask RGBA
			

			HLSLPROGRAM
			
			#define _NORMAL_DROPOFF_TS 1
			#pragma multi_compile _ LOD_FADE_CROSSFADE
			#pragma multi_compile_fog
			#define ASE_FOG 1
			#define ASE_SRP_VERSION 100400
			#define REQUIRE_DEPTH_TEXTURE 1

			
			#pragma multi_compile _ _MAIN_LIGHT_SHADOWS
			#pragma multi_compile _ _MAIN_LIGHT_SHADOWS_CASCADE
			#pragma multi_compile _ _ADDITIONAL_LIGHTS_VERTEX _ADDITIONAL_LIGHTS
			#pragma multi_compile _ _ADDITIONAL_LIGHT_SHADOWS
			#pragma multi_compile _ _SHADOWS_SOFT
			#pragma multi_compile _ _MIXED_LIGHTING_SUBTRACTIVE
			#pragma multi_compile _ _GBUFFER_NORMALS_OCT
			
			#pragma multi_compile _ DIRLIGHTMAP_COMBINED
			#pragma multi_compile _ LIGHTMAP_ON

			#pragma vertex vert
			#pragma fragment frag

			#define SHADERPASS SHADERPASS_GBUFFER

			#include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
			#include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Lighting.hlsl"
			#include "Packages/com.unity.render-pipelines.core/ShaderLibrary/Color.hlsl"
			#include "Packages/com.unity.render-pipelines.core/ShaderLibrary/UnityInstancing.hlsl"
			#include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/ShaderGraphFunctions.hlsl"
			#include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/UnityGBuffer.hlsl"

			#if ASE_SRP_VERSION <= 70108
			#define REQUIRES_VERTEX_SHADOW_COORD_INTERPOLATOR
			#endif

			#if defined(UNITY_INSTANCING_ENABLED) && defined(_TERRAIN_INSTANCED_PERPIXEL_NORMAL)
			    #define ENABLE_TERRAIN_PERPIXEL_NORMAL
			#endif

			#define ASE_NEEDS_FRAG_SCREEN_POSITION
			#define ASE_NEEDS_FRAG_WORLD_POSITION
			#pragma multi_compile_instancing


			struct VertexInput
			{
				float4 vertex : POSITION;
				float3 ase_normal : NORMAL;
				float4 ase_tangent : TANGENT;
				float4 texcoord1 : TEXCOORD1;
				float4 texcoord : TEXCOORD0;
				
				UNITY_VERTEX_INPUT_INSTANCE_ID
			};

			struct VertexOutput
			{
				float4 clipPos : SV_POSITION;
				float4 lightmapUVOrVertexSH : TEXCOORD0;
				half4 fogFactorAndVertexLight : TEXCOORD1;
				#if defined(REQUIRES_VERTEX_SHADOW_COORD_INTERPOLATOR)
				float4 shadowCoord : TEXCOORD2;
				#endif
				float4 tSpace0 : TEXCOORD3;
				float4 tSpace1 : TEXCOORD4;
				float4 tSpace2 : TEXCOORD5;
				#if defined(ASE_NEEDS_FRAG_SCREEN_POSITION)
				float4 screenPos : TEXCOORD6;
				#endif
				float4 ase_texcoord7 : TEXCOORD7;
				UNITY_VERTEX_INPUT_INSTANCE_ID
				UNITY_VERTEX_OUTPUT_STEREO
			};

			CBUFFER_START(UnityPerMaterial)
						#ifdef _TRANSMISSION_ASE
				float _TransmissionShadow;
			#endif
			#ifdef _TRANSLUCENCY_ASE
				float _TransStrength;
				float _TransNormal;
				float _TransScattering;
				float _TransDirect;
				float _TransAmbient;
				float _TransShadow;
			#endif
			#ifdef TESSELLATION_ON
				float _TessPhongStrength;
				float _TessValue;
				float _TessMin;
				float _TessMax;
				float _TessEdgeLength;
				float _TessMaxDisp;
			#endif
			CBUFFER_END
			uniform float4 _CameraDepthTexture_TexelSize;
			sampler2D _MainTexture;
			sampler2D _NoiseTexture;
			UNITY_INSTANCING_BUFFER_START(Water_Shader)
				UNITY_DEFINE_INSTANCED_PROP(float4, _SurfaceColor)
				UNITY_DEFINE_INSTANCED_PROP(float4, _DeepColor)
				UNITY_DEFINE_INSTANCED_PROP(float4, _IceColor)
				UNITY_DEFINE_INSTANCED_PROP(float4, _FoamColor)
				UNITY_DEFINE_INSTANCED_PROP(float4, _EdgeFoamColor)
				UNITY_DEFINE_INSTANCED_PROP(float2, _DepthSmoothStep)
				UNITY_DEFINE_INSTANCED_PROP(float2, _SoftSelect)
				UNITY_DEFINE_INSTANCED_PROP(float2, _windDirection)
				UNITY_DEFINE_INSTANCED_PROP(float2, _IceSmoothStep)
				UNITY_DEFINE_INSTANCED_PROP(float2, _Direction)
				UNITY_DEFINE_INSTANCED_PROP(float, _IceStrength)
				UNITY_DEFINE_INSTANCED_PROP(float, _IceTextureScale)
				UNITY_DEFINE_INSTANCED_PROP(float, _Speed)
				UNITY_DEFINE_INSTANCED_PROP(float, _FoamCuttoff)
				UNITY_DEFINE_INSTANCED_PROP(float, _Alpha)
				UNITY_DEFINE_INSTANCED_PROP(float, _WaterNoiseMultiplyer)
				UNITY_DEFINE_INSTANCED_PROP(float, _textureSize)
				UNITY_DEFINE_INSTANCED_PROP(float, _EdgePower)
				UNITY_DEFINE_INSTANCED_PROP(float, _Distance)
				UNITY_DEFINE_INSTANCED_PROP(float, _EdgeFoamScale)
				UNITY_DEFINE_INSTANCED_PROP(float, _FoamAlpha)
			UNITY_INSTANCING_BUFFER_END(Water_Shader)


			
			VertexOutput VertexFunction( VertexInput v  )
			{
				VertexOutput o = (VertexOutput)0;
				UNITY_SETUP_INSTANCE_ID(v);
				UNITY_TRANSFER_INSTANCE_ID(v, o);
				UNITY_INITIALIZE_VERTEX_OUTPUT_STEREO(o);

				o.ase_texcoord7.xy = v.texcoord.xy;
				
				//setting value to unused interpolator channels and avoid initialization warnings
				o.ase_texcoord7.zw = 0;
				#ifdef ASE_ABSOLUTE_VERTEX_POS
					float3 defaultVertexValue = v.vertex.xyz;
				#else
					float3 defaultVertexValue = float3(0, 0, 0);
				#endif
				float3 vertexValue = defaultVertexValue;
				#ifdef ASE_ABSOLUTE_VERTEX_POS
					v.vertex.xyz = vertexValue;
				#else
					v.vertex.xyz += vertexValue;
				#endif
				v.ase_normal = v.ase_normal;

				float3 positionWS = TransformObjectToWorld( v.vertex.xyz );
				float3 positionVS = TransformWorldToView( positionWS );
				float4 positionCS = TransformWorldToHClip( positionWS );

				VertexNormalInputs normalInput = GetVertexNormalInputs( v.ase_normal, v.ase_tangent );

				o.tSpace0 = float4( normalInput.normalWS, positionWS.x);
				o.tSpace1 = float4( normalInput.tangentWS, positionWS.y);
				o.tSpace2 = float4( normalInput.bitangentWS, positionWS.z);

				OUTPUT_LIGHTMAP_UV( v.texcoord1, unity_LightmapST, o.lightmapUVOrVertexSH.xy );
				OUTPUT_SH( normalInput.normalWS.xyz, o.lightmapUVOrVertexSH.xyz );

				#if defined(ENABLE_TERRAIN_PERPIXEL_NORMAL)
					o.lightmapUVOrVertexSH.zw = v.texcoord;
					o.lightmapUVOrVertexSH.xy = v.texcoord * unity_LightmapST.xy + unity_LightmapST.zw;
				#endif

				half3 vertexLight = VertexLighting( positionWS, normalInput.normalWS );
				#ifdef ASE_FOG
					half fogFactor = ComputeFogFactor( positionCS.z );
				#else
					half fogFactor = 0;
				#endif
				o.fogFactorAndVertexLight = half4(fogFactor, vertexLight);
				
				#if defined(REQUIRES_VERTEX_SHADOW_COORD_INTERPOLATOR)
				VertexPositionInputs vertexInput = (VertexPositionInputs)0;
				vertexInput.positionWS = positionWS;
				vertexInput.positionCS = positionCS;
				o.shadowCoord = GetShadowCoord( vertexInput );
				#endif
				
				o.clipPos = positionCS;
				#if defined(ASE_NEEDS_FRAG_SCREEN_POSITION)
				o.screenPos = ComputeScreenPos(positionCS);
				#endif
				return o;
			}
			
			#if defined(TESSELLATION_ON)
			struct VertexControl
			{
				float4 vertex : INTERNALTESSPOS;
				float3 ase_normal : NORMAL;
				float4 ase_tangent : TANGENT;
				float4 texcoord : TEXCOORD0;
				float4 texcoord1 : TEXCOORD1;
				
				UNITY_VERTEX_INPUT_INSTANCE_ID
			};

			struct TessellationFactors
			{
				float edge[3] : SV_TessFactor;
				float inside : SV_InsideTessFactor;
			};

			VertexControl vert ( VertexInput v )
			{
				VertexControl o;
				UNITY_SETUP_INSTANCE_ID(v);
				UNITY_TRANSFER_INSTANCE_ID(v, o);
				o.vertex = v.vertex;
				o.ase_normal = v.ase_normal;
				o.ase_tangent = v.ase_tangent;
				o.texcoord = v.texcoord;
				o.texcoord1 = v.texcoord1;
				
				return o;
			}

			TessellationFactors TessellationFunction (InputPatch<VertexControl,3> v)
			{
				TessellationFactors o;
				float4 tf = 1;
				float tessValue = _TessValue; float tessMin = _TessMin; float tessMax = _TessMax;
				float edgeLength = _TessEdgeLength; float tessMaxDisp = _TessMaxDisp;
				#if defined(ASE_FIXED_TESSELLATION)
				tf = FixedTess( tessValue );
				#elif defined(ASE_DISTANCE_TESSELLATION)
				tf = DistanceBasedTess(v[0].vertex, v[1].vertex, v[2].vertex, tessValue, tessMin, tessMax, GetObjectToWorldMatrix(), _WorldSpaceCameraPos );
				#elif defined(ASE_LENGTH_TESSELLATION)
				tf = EdgeLengthBasedTess(v[0].vertex, v[1].vertex, v[2].vertex, edgeLength, GetObjectToWorldMatrix(), _WorldSpaceCameraPos, _ScreenParams );
				#elif defined(ASE_LENGTH_CULL_TESSELLATION)
				tf = EdgeLengthBasedTessCull(v[0].vertex, v[1].vertex, v[2].vertex, edgeLength, tessMaxDisp, GetObjectToWorldMatrix(), _WorldSpaceCameraPos, _ScreenParams, unity_CameraWorldClipPlanes );
				#endif
				o.edge[0] = tf.x; o.edge[1] = tf.y; o.edge[2] = tf.z; o.inside = tf.w;
				return o;
			}

			[domain("tri")]
			[partitioning("fractional_odd")]
			[outputtopology("triangle_cw")]
			[patchconstantfunc("TessellationFunction")]
			[outputcontrolpoints(3)]
			VertexControl HullFunction(InputPatch<VertexControl, 3> patch, uint id : SV_OutputControlPointID)
			{
			   return patch[id];
			}

			[domain("tri")]
			VertexOutput DomainFunction(TessellationFactors factors, OutputPatch<VertexControl, 3> patch, float3 bary : SV_DomainLocation)
			{
				VertexInput o = (VertexInput) 0;
				o.vertex = patch[0].vertex * bary.x + patch[1].vertex * bary.y + patch[2].vertex * bary.z;
				o.ase_normal = patch[0].ase_normal * bary.x + patch[1].ase_normal * bary.y + patch[2].ase_normal * bary.z;
				o.ase_tangent = patch[0].ase_tangent * bary.x + patch[1].ase_tangent * bary.y + patch[2].ase_tangent * bary.z;
				o.texcoord = patch[0].texcoord * bary.x + patch[1].texcoord * bary.y + patch[2].texcoord * bary.z;
				o.texcoord1 = patch[0].texcoord1 * bary.x + patch[1].texcoord1 * bary.y + patch[2].texcoord1 * bary.z;
				
				#if defined(ASE_PHONG_TESSELLATION)
				float3 pp[3];
				for (int i = 0; i < 3; ++i)
					pp[i] = o.vertex.xyz - patch[i].ase_normal * (dot(o.vertex.xyz, patch[i].ase_normal) - dot(patch[i].vertex.xyz, patch[i].ase_normal));
				float phongStrength = _TessPhongStrength;
				o.vertex.xyz = phongStrength * (pp[0]*bary.x + pp[1]*bary.y + pp[2]*bary.z) + (1.0f-phongStrength) * o.vertex.xyz;
				#endif
				UNITY_TRANSFER_INSTANCE_ID(patch[0], o);
				return VertexFunction(o);
			}
			#else
			VertexOutput vert ( VertexInput v )
			{
				return VertexFunction( v );
			}
			#endif

			#if defined(ASE_EARLY_Z_DEPTH_OPTIMIZE)
				#define ASE_SV_DEPTH SV_DepthLessEqual  
			#else
				#define ASE_SV_DEPTH SV_Depth
			#endif
			FragmentOutput frag ( VertexOutput IN 
								#ifdef ASE_DEPTH_WRITE_ON
								,out float outputDepth : ASE_SV_DEPTH
								#endif
								 )
			{
				UNITY_SETUP_INSTANCE_ID(IN);
				UNITY_SETUP_STEREO_EYE_INDEX_POST_VERTEX(IN);

				#ifdef LOD_FADE_CROSSFADE
					LODDitheringTransition( IN.clipPos.xyz, unity_LODFade.x );
				#endif

				#if defined(ENABLE_TERRAIN_PERPIXEL_NORMAL)
					float2 sampleCoords = (IN.lightmapUVOrVertexSH.zw / _TerrainHeightmapRecipSize.zw + 0.5f) * _TerrainHeightmapRecipSize.xy;
					float3 WorldNormal = TransformObjectToWorldNormal(normalize(SAMPLE_TEXTURE2D(_TerrainNormalmapTexture, sampler_TerrainNormalmapTexture, sampleCoords).rgb * 2 - 1));
					float3 WorldTangent = -cross(GetObjectToWorldMatrix()._13_23_33, WorldNormal);
					float3 WorldBiTangent = cross(WorldNormal, -WorldTangent);
				#else
					float3 WorldNormal = normalize( IN.tSpace0.xyz );
					float3 WorldTangent = IN.tSpace1.xyz;
					float3 WorldBiTangent = IN.tSpace2.xyz;
				#endif
				float3 WorldPosition = float3(IN.tSpace0.w,IN.tSpace1.w,IN.tSpace2.w);
				float3 WorldViewDirection = _WorldSpaceCameraPos.xyz  - WorldPosition;
				float4 ShadowCoords = float4( 0, 0, 0, 0 );
				#if defined(ASE_NEEDS_FRAG_SCREEN_POSITION)
				float4 ScreenPos = IN.screenPos;
				#endif

				#if defined(REQUIRES_VERTEX_SHADOW_COORD_INTERPOLATOR)
					ShadowCoords = IN.shadowCoord;
				#elif defined(MAIN_LIGHT_CALCULATE_SHADOWS)
					ShadowCoords = TransformWorldToShadowCoord( WorldPosition );
				#endif
	
				WorldViewDirection = SafeNormalize( WorldViewDirection );

				float4 _SurfaceColor_Instance = UNITY_ACCESS_INSTANCED_PROP(Water_Shader,_SurfaceColor);
				float4 _DeepColor_Instance = UNITY_ACCESS_INSTANCED_PROP(Water_Shader,_DeepColor);
				float2 _DepthSmoothStep_Instance = UNITY_ACCESS_INSTANCED_PROP(Water_Shader,_DepthSmoothStep);
				float4 ase_screenPosNorm = ScreenPos / ScreenPos.w;
				ase_screenPosNorm.z = ( UNITY_NEAR_CLIP_VALUE >= 0 ) ? ase_screenPosNorm.z : ase_screenPosNorm.z * 0.5 + 0.5;
				float _Distance_Instance = UNITY_ACCESS_INSTANCED_PROP(Water_Shader,_Distance);
				float screenDepth17 = LinearEyeDepth(SHADERGRAPH_SAMPLE_SCENE_DEPTH( ase_screenPosNorm.xy ),_ZBufferParams);
				float distanceDepth17 = abs( ( screenDepth17 - LinearEyeDepth( ase_screenPosNorm.z,_ZBufferParams ) ) / ( _Distance_Instance ) );
				float _EdgePower_Instance = UNITY_ACCESS_INSTANCED_PROP(Water_Shader,_EdgePower);
				float clampResult40 = clamp( ( ( 1.0 - distanceDepth17 ) * _EdgePower_Instance ) , 0.0 , 1.0 );
				float smoothstepResult132 = smoothstep( _DepthSmoothStep_Instance.x , _DepthSmoothStep_Instance.y , clampResult40);
				float DepthFadeBasic52 = smoothstepResult132;
				float4 lerpResult91 = lerp( _SurfaceColor_Instance , _DeepColor_Instance , ( 1.0 - DepthFadeBasic52 ));
				float2 _SoftSelect_Instance = UNITY_ACCESS_INSTANCED_PROP(Water_Shader,_SoftSelect);
				float2 _windDirection_Instance = UNITY_ACCESS_INSTANCED_PROP(Water_Shader,_windDirection);
				float2 _windDirection180 = _windDirection_Instance;
				float _textureSize_Instance = UNITY_ACCESS_INSTANCED_PROP(Water_Shader,_textureSize);
				float2 panner51 = ( 1.0 * _Time.y * _windDirection180 + (( WorldPosition / _textureSize_Instance )).xz);
				float smoothstepResult65 = smoothstep( _SoftSelect_Instance.x , _SoftSelect_Instance.y , tex2D( _MainTexture, panner51 ).a);
				float2 panner50 = ( 1.0 * _Time.y * ( ( 1.0 - _windDirection180 ) * 0.01 ) + (( WorldPosition / 94.0 )).xz);
				float lerpResult75 = lerp( smoothstepResult65 , 0.0 , ( tex2D( _MainTexture, panner50 ).a * 3.0 ));
				float _WaterNoiseMultiplyer_Instance = UNITY_ACCESS_INSTANCED_PROP(Water_Shader,_WaterNoiseMultiplyer);
				float _Wind85 = ( lerpResult75 * _WaterNoiseMultiplyer_Instance );
				float4 lerpResult99 = lerp( lerpResult91 , float4( 1,1,1,0 ) , _Wind85);
				float4 _FoamColor_Instance = UNITY_ACCESS_INSTANCED_PROP(Water_Shader,_FoamColor);
				float _depthNonSmoothStepped134 = clampResult40;
				float _FoamCuttoff_Instance = UNITY_ACCESS_INSTANCED_PROP(Water_Shader,_FoamCuttoff);
				float _EdgeFoamScale_Instance = UNITY_ACCESS_INSTANCED_PROP(Water_Shader,_EdgeFoamScale);
				float2 temp_cast_0 = (_EdgeFoamScale_Instance).xx;
				float2 _Direction_Instance = UNITY_ACCESS_INSTANCED_PROP(Water_Shader,_Direction);
				float2 _direction13 = _Direction_Instance;
				float _Speed_Instance = UNITY_ACCESS_INSTANCED_PROP(Water_Shader,_Speed);
				float _speed14 = _Speed_Instance;
				float2 texCoord56 = IN.ase_texcoord7.xy * temp_cast_0 + ( ( _direction13 * ( _speed14 / 10.0 ) ) * _TimeParameters.x );
				float2 temp_cast_1 = (_EdgeFoamScale_Instance).xx;
				float2 texCoord60 = IN.ase_texcoord7.xy * temp_cast_1 + ( 1.0 - ( ( _direction13 * _speed14 ) * _TimeParameters.x ) );
				float _foam82 = step( ( ( 1.0 - _depthNonSmoothStepped134 ) * _FoamCuttoff_Instance ) , ( tex2D( _NoiseTexture, texCoord56 ).a + tex2D( _NoiseTexture, texCoord60 ).a ) );
				float4 _EdgeFoamColor_Instance = UNITY_ACCESS_INSTANCED_PROP(Water_Shader,_EdgeFoamColor);
				float4 lerpResult104 = lerp( lerpResult99 , _FoamColor_Instance , ( _foam82 * _EdgeFoamColor_Instance ));
				float4 _colour110 = lerpResult104;
				float4 _IceColor_Instance = UNITY_ACCESS_INSTANCED_PROP(Water_Shader,_IceColor);
				float4 appendResult98 = (float4(_IceColor_Instance.r , _IceColor_Instance.g , _IceColor_Instance.b , 0.0));
				float2 _IceSmoothStep_Instance = UNITY_ACCESS_INSTANCED_PROP(Water_Shader,_IceSmoothStep);
				float _IceTextureScale_Instance = UNITY_ACCESS_INSTANCED_PROP(Water_Shader,_IceTextureScale);
				float2 temp_cast_2 = (_IceTextureScale_Instance).xx;
				float2 texCoord70 = IN.ase_texcoord7.xy * temp_cast_2 + float2( 0,0 );
				float smoothstepResult83 = smoothstep( -1.0 , 1.0 , tex2D( _NoiseTexture, texCoord70 ).b);
				float smoothstepResult101 = smoothstep( _IceSmoothStep_Instance.x , _IceSmoothStep_Instance.y , ( smoothstepResult83 - tex2D( _NoiseTexture, ( texCoord70 * float2( 2,2 ) ) ).b ));
				float4 clampResult108 = clamp( ( appendResult98 * smoothstepResult101 ) , float4( 0,0,0,0 ) , float4( 1,1,1,0 ) );
				float _IceStrength_Instance = UNITY_ACCESS_INSTANCED_PROP(Water_Shader,_IceStrength);
				float4 _albedo118 = ( _colour110 + ( clampResult108 * _IceStrength_Instance ) );
				
				float _Alpha_Instance = UNITY_ACCESS_INSTANCED_PROP(Water_Shader,_Alpha);
				float lerpResult106 = lerp( 1.0 , _Alpha_Instance , DepthFadeBasic52);
				float lerpResult111 = lerp( lerpResult106 , 0.0 , _Wind85);
				float _FoamAlpha_Instance = UNITY_ACCESS_INSTANCED_PROP(Water_Shader,_FoamAlpha);
				float lerpResult116 = lerp( lerpResult111 , _FoamAlpha_Instance , _foam82);
				float _Alpha119 = lerpResult116;
				
				float3 Albedo = _albedo118.rgb;
				float3 Normal = float3(0, 0, 1);
				float3 Emission = 0;
				float3 Specular = 0.5;
				float Metallic = 0.0;
				float Smoothness = 0.0;
				float Occlusion = 1;
				float Alpha = _Alpha119;
				float AlphaClipThreshold = 0.5;
				float AlphaClipThresholdShadow = 0.5;
				float3 BakedGI = 0;
				float3 RefractionColor = 1;
				float RefractionIndex = 1;
				float3 Transmission = 1;
				float3 Translucency = 1;
				#ifdef ASE_DEPTH_WRITE_ON
				float DepthValue = 0;
				#endif

				#ifdef _ALPHATEST_ON
					clip(Alpha - AlphaClipThreshold);
				#endif

				InputData inputData;
				inputData.positionWS = WorldPosition;
				inputData.viewDirectionWS = WorldViewDirection;
				inputData.shadowCoord = ShadowCoords;

				#ifdef _NORMALMAP
					#if _NORMAL_DROPOFF_TS
					inputData.normalWS = TransformTangentToWorld(Normal, half3x3( WorldTangent, WorldBiTangent, WorldNormal ));
					#elif _NORMAL_DROPOFF_OS
					inputData.normalWS = TransformObjectToWorldNormal(Normal);
					#elif _NORMAL_DROPOFF_WS
					inputData.normalWS = Normal;
					#endif
					inputData.normalWS = NormalizeNormalPerPixel(inputData.normalWS);
				#else
					inputData.normalWS = WorldNormal;
				#endif

				#ifdef ASE_FOG
					inputData.fogCoord = IN.fogFactorAndVertexLight.x;
				#endif

				inputData.vertexLighting = IN.fogFactorAndVertexLight.yzw;
				#if defined(ENABLE_TERRAIN_PERPIXEL_NORMAL)
					float3 SH = SampleSH(inputData.normalWS.xyz);
				#else
					float3 SH = IN.lightmapUVOrVertexSH.xyz;
				#endif

				inputData.bakedGI = SAMPLE_GI( IN.lightmapUVOrVertexSH.xy, SH, inputData.normalWS );
				#ifdef _ASE_BAKEDGI
					inputData.bakedGI = BakedGI;
				#endif

				BRDFData brdfData;
				InitializeBRDFData( Albedo, Metallic, Specular, Smoothness, Alpha, brdfData);
				half4 color;
				color.rgb = GlobalIllumination( brdfData, inputData.bakedGI, Occlusion, inputData.normalWS, inputData.viewDirectionWS);
				color.a = Alpha;

				#ifdef _TRANSMISSION_ASE
				{
					float shadow = _TransmissionShadow;
				
					Light mainLight = GetMainLight( inputData.shadowCoord );
					float3 mainAtten = mainLight.color * mainLight.distanceAttenuation;
					mainAtten = lerp( mainAtten, mainAtten * mainLight.shadowAttenuation, shadow );
					half3 mainTransmission = max(0 , -dot(inputData.normalWS, mainLight.direction)) * mainAtten * Transmission;
					color.rgb += Albedo * mainTransmission;
				
					#ifdef _ADDITIONAL_LIGHTS
						int transPixelLightCount = GetAdditionalLightsCount();
						for (int i = 0; i < transPixelLightCount; ++i)
						{
							Light light = GetAdditionalLight(i, inputData.positionWS);
							float3 atten = light.color * light.distanceAttenuation;
							atten = lerp( atten, atten * light.shadowAttenuation, shadow );
				
							half3 transmission = max(0 , -dot(inputData.normalWS, light.direction)) * atten * Transmission;
							color.rgb += Albedo * transmission;
						}
					#endif
				}
				#endif
				
				#ifdef _TRANSLUCENCY_ASE
				{
					float shadow = _TransShadow;
					float normal = _TransNormal;
					float scattering = _TransScattering;
					float direct = _TransDirect;
					float ambient = _TransAmbient;
					float strength = _TransStrength;
				
					Light mainLight = GetMainLight( inputData.shadowCoord );
					float3 mainAtten = mainLight.color * mainLight.distanceAttenuation;
					mainAtten = lerp( mainAtten, mainAtten * mainLight.shadowAttenuation, shadow );
				
					half3 mainLightDir = mainLight.direction + inputData.normalWS * normal;
					half mainVdotL = pow( saturate( dot( inputData.viewDirectionWS, -mainLightDir ) ), scattering );
					half3 mainTranslucency = mainAtten * ( mainVdotL * direct + inputData.bakedGI * ambient ) * Translucency;
					color.rgb += Albedo * mainTranslucency * strength;
				
					#ifdef _ADDITIONAL_LIGHTS
						int transPixelLightCount = GetAdditionalLightsCount();
						for (int i = 0; i < transPixelLightCount; ++i)
						{
							Light light = GetAdditionalLight(i, inputData.positionWS);
							float3 atten = light.color * light.distanceAttenuation;
							atten = lerp( atten, atten * light.shadowAttenuation, shadow );
				
							half3 lightDir = light.direction + inputData.normalWS * normal;
							half VdotL = pow( saturate( dot( inputData.viewDirectionWS, -lightDir ) ), scattering );
							half3 translucency = atten * ( VdotL * direct + inputData.bakedGI * ambient ) * Translucency;
							color.rgb += Albedo * translucency * strength;
						}
					#endif
				}
				#endif
				
				#ifdef _REFRACTION_ASE
					float4 projScreenPos = ScreenPos / ScreenPos.w;
					float3 refractionOffset = ( RefractionIndex - 1.0 ) * mul( UNITY_MATRIX_V, float4( WorldNormal, 0 ) ).xyz * ( 1.0 - dot( WorldNormal, WorldViewDirection ) );
					projScreenPos.xy += refractionOffset.xy;
					float3 refraction = SHADERGRAPH_SAMPLE_SCENE_COLOR( projScreenPos.xy ) * RefractionColor;
					color.rgb = lerp( refraction, color.rgb, color.a );
					color.a = 1;
				#endif
				
				#ifdef ASE_FINAL_COLOR_ALPHA_MULTIPLY
					color.rgb *= color.a;
				#endif
				
				#ifdef ASE_FOG
					#ifdef TERRAIN_SPLAT_ADDPASS
						color.rgb = MixFogColor(color.rgb, half3( 0, 0, 0 ), IN.fogFactorAndVertexLight.x );
					#else
						color.rgb = MixFog(color.rgb, IN.fogFactorAndVertexLight.x);
					#endif
				#endif
				
				#ifdef ASE_DEPTH_WRITE_ON
					outputDepth = DepthValue;
				#endif
				
				return BRDFDataToGbuffer(brdfData, inputData, Smoothness, Emission + color.rgb);
			}

			ENDHLSL
		}
		
	}
	
	CustomEditor "UnityEditor.ShaderGraph.PBRMasterGUI"
	Fallback "Hidden/InternalErrorShader"
	
}
/*ASEBEGIN
Version=18935
0;73;898;441;-234.2752;1929.662;1.696036;True;False
Node;AmplifyShaderEditor.CommentaryNode;8;1322.869,-1109.85;Inherit;False;714.9635;1125.281;Vars;10;180;19;131;29;13;14;12;11;186;187;;1,1,1,1;0;0
Node;AmplifyShaderEditor.CommentaryNode;9;-1533.462,-461.3279;Inherit;False;1389.4;273.732;Depth;10;133;132;52;134;40;20;23;17;10;32;;1,1,1,1;0;0
Node;AmplifyShaderEditor.Vector2Node;19;1376,-224;Inherit;False;InstancedProperty;_windDirection;windDirection;3;0;Create;True;0;0;0;False;0;False;0,0;1,0;0;3;FLOAT2;0;FLOAT;1;FLOAT;2
Node;AmplifyShaderEditor.RangedFloatNode;10;-1520.673,-402.8022;Inherit;False;InstancedProperty;_Distance;Distance;2;0;Create;True;0;0;0;False;0;False;10;10;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.Vector2Node;11;1376.607,-1055.85;Inherit;False;InstancedProperty;_Direction;Direction;1;0;Create;True;0;0;0;False;0;False;1,0;1,0;0;3;FLOAT2;0;FLOAT;1;FLOAT;2
Node;AmplifyShaderEditor.RangedFloatNode;12;1377.87,-928.1861;Inherit;False;InstancedProperty;_Speed;Speed;4;0;Create;True;0;0;0;False;0;False;0.02;0.02;0;0.02;0;1;FLOAT;0
Node;AmplifyShaderEditor.CommentaryNode;16;-1304.463,-2215.292;Inherit;False;3325.924;1080.972;Wind;25;85;76;75;71;68;65;59;57;54;51;50;44;43;39;37;35;34;33;26;25;21;18;129;130;182;;1,1,1,1;0;0
Node;AmplifyShaderEditor.RegisterLocalVarNode;180;1840,-208;Inherit;False;_windDirection;-1;True;1;0;FLOAT2;0,0;False;1;FLOAT2;0
Node;AmplifyShaderEditor.DepthFade;17;-1362.119,-404.628;Inherit;False;True;False;True;2;1;FLOAT3;0,0,0;False;0;FLOAT;1;False;1;FLOAT;0
Node;AmplifyShaderEditor.RegisterLocalVarNode;14;1683.086,-928.8447;Inherit;False;_speed;-1;True;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.CommentaryNode;15;-1299.878,-184.0405;Inherit;False;2451.877;818.2622;Foam;31;82;81;74;72;67;66;64;63;60;56;55;53;48;47;45;41;38;36;31;30;28;27;24;22;7;6;5;4;3;2;128;;1,1,1,1;0;0
Node;AmplifyShaderEditor.RegisterLocalVarNode;13;1687.086,-1056.844;Inherit;False;_direction;-1;True;1;0;FLOAT2;0,0;False;1;FLOAT2;0
Node;AmplifyShaderEditor.WorldPosInputsNode;26;-1149.738,-2118.995;Inherit;True;0;4;FLOAT3;0;FLOAT;1;FLOAT;2;FLOAT;3
Node;AmplifyShaderEditor.RangedFloatNode;20;-1083.606,-315.8131;Inherit;False;InstancedProperty;_EdgePower;EdgePower;13;0;Create;True;0;0;0;False;0;False;1;1;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.GetLocalVarNode;27;-1059.082,239.1279;Inherit;False;13;_direction;1;0;OBJECT;;False;1;FLOAT2;0
Node;AmplifyShaderEditor.GetLocalVarNode;22;-1062.707,118.3597;Inherit;False;14;_speed;1;0;OBJECT;;False;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;25;-1152.02,-1931.077;Inherit;False;InstancedProperty;_textureSize;Texture Size;7;0;Create;False;0;0;0;False;0;False;0;5;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;18;-396.0793,-1614.476;Inherit;False;Constant;_Cloudscale;Cloud scale;12;0;Create;True;0;0;0;False;0;False;94;0.6;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.WorldPosInputsNode;21;-427.8477,-1765.781;Inherit;False;0;4;FLOAT3;0;FLOAT;1;FLOAT;2;FLOAT;3
Node;AmplifyShaderEditor.GetLocalVarNode;182;-760.7206,-1837.792;Inherit;False;180;_windDirection;1;0;OBJECT;;False;1;FLOAT2;0
Node;AmplifyShaderEditor.OneMinusNode;23;-1082.908,-402.7297;Inherit;False;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.GetLocalVarNode;24;-1065.878,329.9811;Inherit;False;14;_speed;1;0;OBJECT;;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleDivideOpNode;33;-184.1552,-1673.056;Inherit;False;2;0;FLOAT3;0,0,0;False;1;FLOAT;0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.RangedFloatNode;34;-398.0371,-1423.6;Inherit;False;Constant;_WindSpeed;WindSpeed;14;0;Create;True;0;0;0;False;0;False;0.01;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleDivideOpNode;35;-820.5458,-2020.388;Inherit;False;2;0;FLOAT3;0,0,0;False;1;FLOAT;0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.GetLocalVarNode;31;-1065.812,23.30566;Inherit;False;13;_direction;1;0;OBJECT;;False;1;FLOAT2;0
Node;AmplifyShaderEditor.OneMinusNode;37;-399.6012,-1530.149;Inherit;False;1;0;FLOAT2;0,0;False;1;FLOAT2;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;30;-798.1971,239.5869;Inherit;False;2;2;0;FLOAT2;0,0;False;1;FLOAT;0;False;1;FLOAT2;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;32;-918.9347,-392.8293;Inherit;False;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleTimeNode;28;-825.16,358.222;Inherit;False;1;0;FLOAT;1;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleDivideOpNode;36;-783.6631,120.9806;Inherit;False;2;0;FLOAT;0;False;1;FLOAT;10;False;1;FLOAT;0
Node;AmplifyShaderEditor.TexturePropertyNode;186;1385.555,-740.6116;Inherit;True;Property;_MainTexture;MainTexture;22;0;Create;True;0;0;0;False;0;False;None;1b30082104b5358478b36284b51ced67;False;white;Auto;Texture2D;-1;0;2;SAMPLER2D;0;SAMPLERSTATE;1
Node;AmplifyShaderEditor.SimpleTimeNode;41;-615.9891,149.6008;Inherit;False;1;0;FLOAT;1;False;1;FLOAT;0
Node;AmplifyShaderEditor.RegisterLocalVarNode;187;1737.415,-650.0521;Inherit;False;_noiseTexture;-1;True;1;0;SAMPLER2D;;False;1;SAMPLER2D;0
Node;AmplifyShaderEditor.ComponentMaskNode;44;-637.6744,-2027.684;Inherit;False;True;False;True;True;1;0;FLOAT3;0,0,0;False;1;FLOAT2;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;38;-580.0273,38.96465;Inherit;False;2;2;0;FLOAT2;0,0;False;1;FLOAT;0;False;1;FLOAT2;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;39;-44.2739,-1549.039;Inherit;True;2;2;0;FLOAT2;0,0;False;1;FLOAT;0;False;1;FLOAT2;0
Node;AmplifyShaderEditor.ClampOpNode;40;-762.5948,-424.4975;Inherit;False;3;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;1;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;45;-632.1581,307.222;Inherit;False;2;2;0;FLOAT2;0,0;False;1;FLOAT;0;False;1;FLOAT2;0
Node;AmplifyShaderEditor.ComponentMaskNode;43;-24.71748,-1667.843;Inherit;False;True;False;True;True;1;0;FLOAT3;0,0,0;False;1;FLOAT2;0
Node;AmplifyShaderEditor.TexturePropertyNode;29;1355.871,-459.6231;Inherit;True;Property;_NoiseTexture;Noise Texture;0;0;Create;True;0;0;0;False;0;False;1b30082104b5358478b36284b51ced67;854c813354f78c447a56eed49c7fc29c;False;white;Auto;Texture2D;-1;0;2;SAMPLER2D;0;SAMPLERSTATE;1
Node;AmplifyShaderEditor.PannerNode;51;-350.0219,-2021.723;Inherit;True;3;0;FLOAT2;0,0;False;2;FLOAT2;0,0;False;1;FLOAT;1;False;1;FLOAT2;0
Node;AmplifyShaderEditor.RangedFloatNode;48;-457.1401,260.2038;Inherit;False;InstancedProperty;_EdgeFoamScale;EdgeFoamScale;11;0;Create;True;0;0;0;False;0;False;0;350;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.RegisterLocalVarNode;134;-519.8088,-423.9857;Inherit;False;_depthNonSmoothStepped;-1;True;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.GetLocalVarNode;129;235.189,-1705.257;Inherit;False;187;_noiseTexture;1;0;OBJECT;;False;1;SAMPLER2D;0
Node;AmplifyShaderEditor.RegisterLocalVarNode;131;1796.92,-380.7503;Inherit;False;_mainTexture;-1;True;1;0;SAMPLER2D;;False;1;SAMPLER2D;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;47;-411.9892,56.60065;Inherit;False;2;2;0;FLOAT2;0,0;False;1;FLOAT;0;False;1;FLOAT2;0
Node;AmplifyShaderEditor.GetLocalVarNode;130;-336.6213,-2170.825;Inherit;True;187;_noiseTexture;1;0;OBJECT;;False;1;SAMPLER2D;0
Node;AmplifyShaderEditor.PannerNode;50;218.8719,-1606.172;Inherit;True;3;0;FLOAT2;0,0;False;2;FLOAT2;0.5,0;False;1;FLOAT;1;False;1;FLOAT2;0
Node;AmplifyShaderEditor.OneMinusNode;53;-447.4682,395.4898;Inherit;False;1;0;FLOAT2;0,0;False;1;FLOAT2;0
Node;AmplifyShaderEditor.SamplerNode;54;537.3069,-1664.717;Inherit;True;Property;_TextureSample1;Texture Sample 1;1;0;Create;True;0;0;0;False;0;False;-1;None;None;True;0;False;white;Auto;False;Object;-1;Auto;Texture2D;8;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;6;FLOAT;0;False;7;SAMPLERSTATE;;False;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.GetLocalVarNode;128;-160.3173,260.3092;Inherit;False;131;_mainTexture;1;0;OBJECT;;False;1;SAMPLER2D;0
Node;AmplifyShaderEditor.Vector2Node;57;15.54037,-1849.327;Inherit;False;InstancedProperty;_SoftSelect;Soft Select;10;0;Create;True;0;0;0;False;0;False;0.47,3.32;0.47,3.32;0;3;FLOAT2;0;FLOAT;1;FLOAT;2
Node;AmplifyShaderEditor.Vector2Node;133;-795.2715,-311.0076;Inherit;False;InstancedProperty;_DepthSmoothStep;DepthSmoothStep;21;0;Create;True;0;0;0;False;0;False;0,0;-0.08,2;0;3;FLOAT2;0;FLOAT;1;FLOAT;2
Node;AmplifyShaderEditor.SamplerNode;59;-1.319855,-2054.145;Inherit;True;Property;_TextureSample0;Texture Sample 0;1;0;Create;True;0;0;0;False;0;False;-1;None;None;True;0;False;white;Auto;False;Object;-1;Auto;Texture2D;8;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;6;FLOAT;0;False;7;SAMPLERSTATE;;False;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.GetLocalVarNode;55;-180.8619,-121.0405;Inherit;True;134;_depthNonSmoothStepped;1;0;OBJECT;;False;1;FLOAT;0
Node;AmplifyShaderEditor.CommentaryNode;61;-1307.672,-3086.822;Inherit;False;3330.897;847.7177;Ice;21;118;117;114;112;108;107;103;101;98;94;92;89;87;83;80;79;78;70;69;62;0;;1,1,1,1;0;0
Node;AmplifyShaderEditor.TextureCoordinatesNode;60;-182.3479,407.7728;Inherit;False;0;-1;2;3;2;SAMPLER2D;;False;0;FLOAT2;1,1;False;1;FLOAT2;0,0;False;5;FLOAT2;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.TextureCoordinatesNode;56;-213.1803,92.55054;Inherit;False;0;-1;2;3;2;SAMPLER2D;;False;0;FLOAT2;1,1;False;1;FLOAT2;0,0;False;5;FLOAT2;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.OneMinusNode;67;147.6991,16.73854;Inherit;False;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SamplerNode;66;79.57924,173.7886;Inherit;True;Property;_TextureSample2;Texture Sample 2;18;0;Create;True;0;0;0;False;0;False;-1;None;None;True;0;False;white;Auto;False;Object;-1;Auto;Texture2D;8;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;6;FLOAT;0;False;7;SAMPLERSTATE;;False;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.SmoothstepOpNode;65;343.2776,-1952.931;Inherit;True;3;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;1;False;1;FLOAT;0
Node;AmplifyShaderEditor.SamplerNode;64;104.6627,399.8209;Inherit;True;Property;_TextureSample3;Texture Sample 3;18;0;Create;True;0;0;0;False;0;False;-1;None;None;True;0;False;white;Auto;False;Object;-1;Auto;Texture2D;8;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;6;FLOAT;0;False;7;SAMPLERSTATE;;False;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;68;876.4528,-1568.221;Inherit;False;2;2;0;FLOAT;0;False;1;FLOAT;3;False;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;63;140.3414,90.06452;Inherit;False;InstancedProperty;_FoamCuttoff;FoamCuttoff;16;0;Create;True;0;0;0;False;0;False;1;7.68;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;62;-1179.673,-2702.386;Inherit;False;InstancedProperty;_IceTextureScale;IceTextureScale;18;0;Create;True;0;0;0;False;0;False;0;200;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.SmoothstepOpNode;132;-546.2715,-340.3261;Inherit;False;3;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;1;False;1;FLOAT;0
Node;AmplifyShaderEditor.TextureCoordinatesNode;70;-939.673,-2717.386;Inherit;False;0;-1;2;3;2;SAMPLER2D;;False;0;FLOAT2;1,1;False;1;FLOAT2;0,0;False;5;FLOAT2;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.LerpOp;75;1090.415,-1723.074;Inherit;False;3;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;71;1080.251,-1593.853;Inherit;False;InstancedProperty;_WaterNoiseMultiplyer;Water Noise Multiplyer;6;0;Create;True;0;0;0;False;0;False;0.03;0.03;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.CommentaryNode;73;-135.81,-1124.529;Inherit;False;1438.423;890.6193;Color;13;110;104;100;99;96;95;93;91;90;88;86;84;77;;1,1,1,1;0;0
Node;AmplifyShaderEditor.GetLocalVarNode;69;-779.6891,-3036.822;Inherit;True;131;_mainTexture;1;0;OBJECT;;False;1;SAMPLER2D;0
Node;AmplifyShaderEditor.RegisterLocalVarNode;52;-329.339,-352.7907;Inherit;False;DepthFadeBasic;-1;True;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleAddOpNode;72;441.5325,197.5907;Inherit;False;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;74;391.681,43.64159;Inherit;False;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;80;-532.634,-2508.683;Inherit;True;2;2;0;FLOAT2;0,0;False;1;FLOAT2;2,2;False;1;FLOAT2;0
Node;AmplifyShaderEditor.GetLocalVarNode;79;-537.753,-2722.164;Inherit;True;131;_mainTexture;1;0;OBJECT;;False;1;SAMPLER2D;0
Node;AmplifyShaderEditor.StepOpNode;81;621.7236,174.9256;Inherit;True;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.GetLocalVarNode;77;-64.18143,-714.398;Inherit;False;52;DepthFadeBasic;1;0;OBJECT;;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;76;1411.613,-1722.853;Inherit;True;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SamplerNode;78;-474.0039,-2938.516;Inherit;True;Property;_TextureSample4;Texture Sample 4;20;0;Create;True;0;0;0;False;0;False;-1;None;None;True;0;False;white;Auto;False;Object;-1;Auto;Texture2D;8;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;6;FLOAT;0;False;7;SAMPLERSTATE;;False;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.RegisterLocalVarNode;85;1680.651,-1718.561;Inherit;False;_Wind;-1;True;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.ColorNode;84;-64.69773,-1060.74;Inherit;False;InstancedProperty;_SurfaceColor;SurfaceColor;8;0;Create;True;0;0;0;False;0;False;0.07058824,0.8666667,0.8862746,1;0,0.7647059,1,1;True;0;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.OneMinusNode;88;145.049,-725.33;Inherit;False;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SmoothstepOpNode;83;-104.1468,-2809.181;Inherit;True;3;0;FLOAT;0;False;1;FLOAT;-1;False;2;FLOAT;1;False;1;FLOAT;0
Node;AmplifyShaderEditor.SamplerNode;87;-293.8759,-2478.656;Inherit;True;Property;_TextureSample6;Texture Sample 6;22;0;Create;True;0;0;0;False;0;False;-1;None;None;True;0;False;white;Auto;False;Object;-1;Auto;Texture2D;8;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;6;FLOAT;0;False;7;SAMPLERSTATE;;False;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.ColorNode;86;-69.72288,-890.4474;Inherit;False;InstancedProperty;_DeepColor;DeepColor;9;0;Create;True;0;0;0;False;0;False;0.0627451,0.3607843,0.5647059,1;0.2065232,0.3471635,0.4811321,1;True;0;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.RegisterLocalVarNode;82;887.9992,169.4156;Inherit;False;_foam;-1;True;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.ColorNode;93;270.3834,-422.1828;Inherit;False;InstancedProperty;_EdgeFoamColor;EdgeFoamColor;12;1;[HDR];Create;True;0;0;0;False;0;False;1,1,1,1;1,1,1,1;True;0;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.ColorNode;94;163.1242,-2768.101;Inherit;False;InstancedProperty;_IceColor;IceColor;17;0;Create;True;0;0;0;False;0;False;1,1,1,1;0.7971699,0.9271079,1,1;True;0;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.GetLocalVarNode;95;267.6988,-510.126;Inherit;False;82;_foam;1;0;OBJECT;;False;1;FLOAT;0
Node;AmplifyShaderEditor.GetLocalVarNode;90;354.494,-706.9228;Inherit;False;85;_Wind;1;0;OBJECT;;False;1;FLOAT;0
Node;AmplifyShaderEditor.Vector2Node;92;173.8533,-2376.181;Inherit;False;InstancedProperty;_IceSmoothStep;IceSmoothStep;20;0;Create;True;0;0;0;False;0;False;-1,1;-2,2;0;3;FLOAT2;0;FLOAT;1;FLOAT;2
Node;AmplifyShaderEditor.LerpOp;91;328.4809,-929.9809;Inherit;True;3;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0,0;False;2;FLOAT;0;False;1;COLOR;0
Node;AmplifyShaderEditor.SimpleSubtractOpNode;89;149.0084,-2589.951;Inherit;True;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.ColorNode;100;550.0652,-667.6354;Inherit;False;InstancedProperty;_FoamColor;FoamColor;14;1;[HDR];Create;True;0;0;0;False;0;False;1,1,1,1;1,1,1,1;True;0;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;96;594.3832,-489.1828;Inherit;False;2;2;0;FLOAT;0;False;1;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.DynamicAppendNode;98;446.9534,-2741.579;Inherit;False;COLOR;4;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;0;False;3;FLOAT;0;False;1;COLOR;0
Node;AmplifyShaderEditor.LerpOp;99;643.3102,-842.5923;Inherit;False;3;0;COLOR;0,0,0,0;False;1;COLOR;1,1,1,0;False;2;FLOAT;0;False;1;COLOR;0
Node;AmplifyShaderEditor.SmoothstepOpNode;101;509.0392,-2499.467;Inherit;True;3;0;FLOAT;0;False;1;FLOAT;-1;False;2;FLOAT;1;False;1;FLOAT;0
Node;AmplifyShaderEditor.CommentaryNode;97;-1298.981,-1122.276;Inherit;False;1147.813;655.6155;Alpha;9;119;116;115;113;111;109;106;105;102;;1,1,1,1;0;0
Node;AmplifyShaderEditor.RangedFloatNode;102;-1193.981,-945.2759;Inherit;False;InstancedProperty;_Alpha;Alpha;5;0;Create;True;0;0;0;False;0;False;0.5;0.5;0;1;0;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;103;863.2922,-2539.742;Inherit;True;2;2;0;COLOR;0,0,0,0;False;1;FLOAT;0;False;1;COLOR;0
Node;AmplifyShaderEditor.LerpOp;104;930.4943,-556.9227;Inherit;False;3;0;COLOR;0,0,0,0;False;1;COLOR;1,1,1,0;False;2;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.GetLocalVarNode;105;-1129.589,-860.8541;Inherit;False;52;DepthFadeBasic;1;0;OBJECT;;False;1;FLOAT;0
Node;AmplifyShaderEditor.RegisterLocalVarNode;110;1114.432,-562.4618;Inherit;False;_colour;-1;True;1;0;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.ClampOpNode;108;1155.99,-2559.147;Inherit;False;3;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0,0;False;2;COLOR;1,1,1,0;False;1;COLOR;0
Node;AmplifyShaderEditor.RangedFloatNode;107;1105.517,-2403.418;Inherit;False;InstancedProperty;_IceStrength;IceStrength;19;0;Create;True;0;0;0;False;0;False;0;0;0;1;0;1;FLOAT;0
Node;AmplifyShaderEditor.GetLocalVarNode;109;-950.6854,-719.5524;Inherit;False;85;_Wind;1;0;OBJECT;;False;1;FLOAT;0
Node;AmplifyShaderEditor.LerpOp;106;-900.5894,-935.8544;Inherit;False;3;0;FLOAT;1;False;1;FLOAT;1;False;2;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.GetLocalVarNode;114;1410.16,-2630.033;Inherit;False;110;_colour;1;0;OBJECT;;False;1;COLOR;0
Node;AmplifyShaderEditor.GetLocalVarNode;115;-746.8863,-595.1542;Inherit;False;82;_foam;1;0;OBJECT;;False;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;113;-739.7144,-672.1781;Inherit;False;InstancedProperty;_FoamAlpha;FoamAlpha;15;0;Create;True;0;0;0;False;0;False;0;0.5;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;112;1438.13,-2546.505;Inherit;False;2;2;0;COLOR;0,0,0,0;False;1;FLOAT;0;False;1;COLOR;0
Node;AmplifyShaderEditor.LerpOp;111;-735.6854,-803.5524;Inherit;False;3;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.LerpOp;116;-537.886,-725.1537;Inherit;False;3;0;FLOAT;0;False;1;FLOAT;1;False;2;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleAddOpNode;117;1623.984,-2574.022;Inherit;True;2;2;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.RegisterLocalVarNode;119;-349.7409,-728.3821;Inherit;False;_Alpha;-1;True;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.RegisterLocalVarNode;118;1842.465,-2578.676;Inherit;False;_albedo;-1;True;1;0;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.GetLocalVarNode;122;2387.156,-326.3995;Inherit;False;119;_Alpha;1;0;OBJECT;;False;1;FLOAT;0
Node;AmplifyShaderEditor.GetLocalVarNode;121;2433.512,-541.0892;Inherit;False;118;_albedo;1;0;OBJECT;;False;1;COLOR;0
Node;AmplifyShaderEditor.RangedFloatNode;136;2422.975,-474.2839;Inherit;False;Constant;_Float0;Float 0;24;0;Create;True;0;0;0;False;0;False;0;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.TemplateMultiPassMasterNode;6;0,0;Float;False;False;-1;2;UnityEditor.ShaderGraph.PBRMasterGUI;0;2;New Amplify Shader;94348b07e5e8bab40bd6c8a1e3df54cd;True;DepthNormals;0;6;DepthNormals;0;False;False;False;False;False;False;False;False;False;False;False;False;True;0;False;-1;False;True;0;False;-1;False;False;False;False;False;False;False;False;False;True;False;255;False;-1;255;False;-1;255;False;-1;7;False;-1;1;False;-1;1;False;-1;1;False;-1;7;False;-1;1;False;-1;1;False;-1;1;False;-1;False;False;False;False;True;3;RenderPipeline=UniversalPipeline;RenderType=Opaque=RenderType;Queue=Geometry=Queue=0;True;0;True;17;d3d9;d3d11;glcore;gles;gles3;metal;vulkan;xbox360;xboxone;xboxseries;ps4;playstation;psp2;n3ds;wiiu;switch;nomrt;0;False;True;1;1;False;-1;0;False;-1;0;1;False;-1;0;False;-1;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;True;1;False;-1;True;3;False;-1;False;True;1;LightMode=DepthNormals;False;False;0;Hidden/InternalErrorShader;0;0;Standard;0;False;0
Node;AmplifyShaderEditor.TemplateMultiPassMasterNode;5;0,0;Float;False;False;-1;2;UnityEditor.ShaderGraph.PBRMasterGUI;0;2;New Amplify Shader;94348b07e5e8bab40bd6c8a1e3df54cd;True;Universal2D;0;5;Universal2D;0;False;False;False;False;False;False;False;False;False;False;False;False;True;0;False;-1;False;True;0;False;-1;False;False;False;False;False;False;False;False;False;True;False;255;False;-1;255;False;-1;255;False;-1;7;False;-1;1;False;-1;1;False;-1;1;False;-1;7;False;-1;1;False;-1;1;False;-1;1;False;-1;False;False;False;False;True;3;RenderPipeline=UniversalPipeline;RenderType=Opaque=RenderType;Queue=Geometry=Queue=0;True;0;True;17;d3d9;d3d11;glcore;gles;gles3;metal;vulkan;xbox360;xboxone;xboxseries;ps4;playstation;psp2;n3ds;wiiu;switch;nomrt;0;False;True;1;5;False;-1;10;False;-1;1;1;False;-1;10;False;-1;False;False;False;False;False;False;False;False;False;False;False;False;False;False;True;True;True;True;True;0;False;-1;False;False;False;False;False;False;False;False;False;True;2;False;-1;True;3;False;-1;True;True;0;False;-1;0;False;-1;True;1;LightMode=Universal2D;False;False;0;Hidden/InternalErrorShader;0;0;Standard;0;False;0
Node;AmplifyShaderEditor.TemplateMultiPassMasterNode;3;0,0;Float;False;False;-1;2;UnityEditor.ShaderGraph.PBRMasterGUI;0;2;New Amplify Shader;94348b07e5e8bab40bd6c8a1e3df54cd;True;DepthOnly;0;3;DepthOnly;0;False;False;False;False;False;False;False;False;False;False;False;False;True;0;False;-1;False;True;0;False;-1;False;False;False;False;False;False;False;False;False;True;False;255;False;-1;255;False;-1;255;False;-1;7;False;-1;1;False;-1;1;False;-1;1;False;-1;7;False;-1;1;False;-1;1;False;-1;1;False;-1;False;False;False;False;True;3;RenderPipeline=UniversalPipeline;RenderType=Opaque=RenderType;Queue=Geometry=Queue=0;True;0;True;17;d3d9;d3d11;glcore;gles;gles3;metal;vulkan;xbox360;xboxone;xboxseries;ps4;playstation;psp2;n3ds;wiiu;switch;nomrt;0;False;False;False;False;False;False;False;False;False;False;False;False;True;0;False;-1;False;False;False;True;False;False;False;False;0;False;-1;False;False;False;False;False;False;False;False;False;True;1;False;-1;False;False;True;1;LightMode=DepthOnly;False;False;0;Hidden/InternalErrorShader;0;0;Standard;0;False;0
Node;AmplifyShaderEditor.TemplateMultiPassMasterNode;4;0,0;Float;False;False;-1;2;UnityEditor.ShaderGraph.PBRMasterGUI;0;2;New Amplify Shader;94348b07e5e8bab40bd6c8a1e3df54cd;True;Meta;0;4;Meta;0;False;False;False;False;False;False;False;False;False;False;False;False;True;0;False;-1;False;True;0;False;-1;False;False;False;False;False;False;False;False;False;True;False;255;False;-1;255;False;-1;255;False;-1;7;False;-1;1;False;-1;1;False;-1;1;False;-1;7;False;-1;1;False;-1;1;False;-1;1;False;-1;False;False;False;False;True;3;RenderPipeline=UniversalPipeline;RenderType=Opaque=RenderType;Queue=Geometry=Queue=0;True;0;True;17;d3d9;d3d11;glcore;gles;gles3;metal;vulkan;xbox360;xboxone;xboxseries;ps4;playstation;psp2;n3ds;wiiu;switch;nomrt;0;False;False;False;False;False;False;False;False;False;False;False;False;False;False;True;2;False;-1;False;False;False;False;False;False;False;False;False;False;False;False;False;False;True;1;LightMode=Meta;False;False;0;Hidden/InternalErrorShader;0;0;Standard;0;False;0
Node;AmplifyShaderEditor.TemplateMultiPassMasterNode;7;0,0;Float;False;False;-1;2;UnityEditor.ShaderGraph.PBRMasterGUI;0;2;New Amplify Shader;94348b07e5e8bab40bd6c8a1e3df54cd;True;GBuffer;0;7;GBuffer;0;False;False;False;False;False;False;False;False;False;False;False;False;True;0;False;-1;False;True;0;False;-1;False;False;False;False;False;False;False;False;False;True;False;255;False;-1;255;False;-1;255;False;-1;7;False;-1;1;False;-1;1;False;-1;1;False;-1;7;False;-1;1;False;-1;1;False;-1;1;False;-1;False;False;False;False;True;3;RenderPipeline=UniversalPipeline;RenderType=Opaque=RenderType;Queue=Geometry=Queue=0;True;0;True;17;d3d9;d3d11;glcore;gles;gles3;metal;vulkan;xbox360;xboxone;xboxseries;ps4;playstation;psp2;n3ds;wiiu;switch;nomrt;0;False;True;1;5;False;-1;10;False;-1;1;1;False;-1;10;False;-1;False;False;False;False;False;False;False;False;False;False;False;False;False;False;True;True;True;True;True;0;False;-1;False;False;False;False;False;False;False;True;False;255;False;-1;255;False;-1;255;False;-1;7;False;-1;1;False;-1;1;False;-1;1;False;-1;7;False;-1;1;False;-1;1;False;-1;1;False;-1;False;True;2;False;-1;True;3;False;-1;True;True;0;False;-1;0;False;-1;True;1;LightMode=UniversalGBuffer;False;False;0;Hidden/InternalErrorShader;0;0;Standard;0;False;0
Node;AmplifyShaderEditor.TemplateMultiPassMasterNode;1;2627.21,-544.6473;Float;False;True;-1;2;UnityEditor.ShaderGraph.PBRMasterGUI;0;2;Water_Shader;94348b07e5e8bab40bd6c8a1e3df54cd;True;Forward;0;1;Forward;18;False;False;False;False;False;False;False;False;False;False;False;False;True;0;False;-1;False;True;0;False;-1;False;False;False;False;False;False;False;False;False;True;False;255;False;-1;255;False;-1;255;False;-1;7;False;-1;1;False;-1;1;False;-1;1;False;-1;7;False;-1;1;False;-1;1;False;-1;1;False;-1;False;False;False;False;True;3;RenderPipeline=UniversalPipeline;RenderType=Transparent=RenderType;Queue=Transparent=Queue=0;True;0;True;17;d3d9;d3d11;glcore;gles;gles3;metal;vulkan;xbox360;xboxone;xboxseries;ps4;playstation;psp2;n3ds;wiiu;switch;nomrt;0;False;True;1;5;False;-1;10;False;-1;1;1;False;-1;10;False;-1;False;False;False;False;False;False;False;False;False;False;False;False;False;False;True;True;True;True;True;0;False;-1;False;False;False;False;False;False;False;True;False;255;False;-1;255;False;-1;255;False;-1;7;False;-1;1;False;-1;1;False;-1;1;False;-1;7;False;-1;1;False;-1;1;False;-1;1;False;-1;False;True;2;False;-1;True;3;False;-1;True;True;0;False;-1;0;False;-1;True;1;LightMode=UniversalForward;False;False;0;Hidden/InternalErrorShader;0;0;Standard;38;Workflow;1;0;Surface;1;637956951908808134;  Refraction Model;0;0;  Blend;0;0;Two Sided;1;0;Fragment Normal Space,InvertActionOnDeselection;0;0;Transmission;0;0;  Transmission Shadow;0.5,False,-1;0;Translucency;0;0;  Translucency Strength;1,False,-1;0;  Normal Distortion;0.5,False,-1;0;  Scattering;2,False,-1;0;  Direct;0.9,False,-1;0;  Ambient;0.1,False,-1;0;  Shadow;0.5,False,-1;0;Cast Shadows;1;0;  Use Shadow Threshold;0;0;Receive Shadows;1;0;GPU Instancing;1;0;LOD CrossFade;1;0;Built-in Fog;1;0;_FinalColorxAlpha;0;0;Meta Pass;1;0;Override Baked GI;0;0;Extra Pre Pass;0;0;DOTS Instancing;0;0;Tessellation;0;637967388002554674;  Phong;0;637967325782332044;  Strength;0.5,False,-1;0;  Type;0;637967365475393393;  Tess;16,False,-1;637967365487059122;  Min;10,False,-1;637967325393012477;  Max;25,False,-1;637967365294563734;  Edge Length;16,False,-1;0;  Max Displacement;25,False,-1;0;Write Depth;0;637967323170491116;  Early Z;0;0;Vertex Position,InvertActionOnDeselection;1;0;0;8;False;True;True;True;True;True;True;True;False;;False;0
Node;AmplifyShaderEditor.TemplateMultiPassMasterNode;2;0,0;Float;False;False;-1;2;UnityEditor.ShaderGraph.PBRMasterGUI;0;2;New Amplify Shader;94348b07e5e8bab40bd6c8a1e3df54cd;True;ShadowCaster;0;2;ShadowCaster;0;False;False;False;False;False;False;False;False;False;False;False;False;True;0;False;-1;False;True;0;False;-1;False;False;False;False;False;False;False;False;False;True;False;255;False;-1;255;False;-1;255;False;-1;7;False;-1;1;False;-1;1;False;-1;1;False;-1;7;False;-1;1;False;-1;1;False;-1;1;False;-1;False;False;False;False;True;3;RenderPipeline=UniversalPipeline;RenderType=Opaque=RenderType;Queue=Geometry=Queue=0;True;0;True;17;d3d9;d3d11;glcore;gles;gles3;metal;vulkan;xbox360;xboxone;xboxseries;ps4;playstation;psp2;n3ds;wiiu;switch;nomrt;0;False;False;False;False;False;False;False;False;False;False;False;False;True;0;False;-1;False;False;False;True;False;False;False;False;0;False;-1;False;False;False;False;False;False;False;False;False;True;1;False;-1;True;3;False;-1;False;True;1;LightMode=ShadowCaster;False;False;0;Hidden/InternalErrorShader;0;0;Standard;0;False;0
Node;AmplifyShaderEditor.TemplateMultiPassMasterNode;0;-256.8051,-2402.71;Float;False;False;-1;2;UnityEditor.ShaderGraph.PBRMasterGUI;0;2;New Amplify Shader;94348b07e5e8bab40bd6c8a1e3df54cd;True;ExtraPrePass;0;0;ExtraPrePass;5;False;False;False;False;False;False;False;False;False;False;False;False;True;0;False;-1;False;True;0;False;-1;False;False;False;False;False;False;False;False;False;True;False;255;False;-1;255;False;-1;255;False;-1;7;False;-1;1;False;-1;1;False;-1;1;False;-1;7;False;-1;1;False;-1;1;False;-1;1;False;-1;False;False;False;False;True;3;RenderPipeline=UniversalPipeline;RenderType=Opaque=RenderType;Queue=Geometry=Queue=0;True;0;True;17;d3d9;d3d11;glcore;gles;gles3;metal;vulkan;xbox360;xboxone;xboxseries;ps4;playstation;psp2;n3ds;wiiu;switch;nomrt;0;False;True;1;1;False;-1;0;False;-1;0;1;False;-1;0;False;-1;False;False;False;False;False;False;False;False;False;False;False;False;True;0;False;-1;False;True;True;True;True;True;0;False;-1;False;False;False;False;False;False;False;True;False;255;False;-1;255;False;-1;255;False;-1;7;False;-1;1;False;-1;1;False;-1;1;False;-1;7;False;-1;1;False;-1;1;False;-1;1;False;-1;False;True;1;False;-1;True;3;False;-1;True;True;0;False;-1;0;False;-1;True;0;False;False;0;Hidden/InternalErrorShader;0;0;Standard;0;False;0
WireConnection;180;0;19;0
WireConnection;17;0;10;0
WireConnection;14;0;12;0
WireConnection;13;0;11;0
WireConnection;23;0;17;0
WireConnection;33;0;21;0
WireConnection;33;1;18;0
WireConnection;35;0;26;0
WireConnection;35;1;25;0
WireConnection;37;0;182;0
WireConnection;30;0;27;0
WireConnection;30;1;24;0
WireConnection;32;0;23;0
WireConnection;32;1;20;0
WireConnection;36;0;22;0
WireConnection;187;0;186;0
WireConnection;44;0;35;0
WireConnection;38;0;31;0
WireConnection;38;1;36;0
WireConnection;39;0;37;0
WireConnection;39;1;34;0
WireConnection;40;0;32;0
WireConnection;45;0;30;0
WireConnection;45;1;28;0
WireConnection;43;0;33;0
WireConnection;51;0;44;0
WireConnection;51;2;182;0
WireConnection;134;0;40;0
WireConnection;131;0;29;0
WireConnection;47;0;38;0
WireConnection;47;1;41;0
WireConnection;50;0;43;0
WireConnection;50;2;39;0
WireConnection;53;0;45;0
WireConnection;54;0;129;0
WireConnection;54;1;50;0
WireConnection;59;0;130;0
WireConnection;59;1;51;0
WireConnection;60;0;48;0
WireConnection;60;1;53;0
WireConnection;56;0;48;0
WireConnection;56;1;47;0
WireConnection;67;0;55;0
WireConnection;66;0;128;0
WireConnection;66;1;56;0
WireConnection;65;0;59;4
WireConnection;65;1;57;1
WireConnection;65;2;57;2
WireConnection;64;0;128;0
WireConnection;64;1;60;0
WireConnection;68;0;54;4
WireConnection;132;0;40;0
WireConnection;132;1;133;1
WireConnection;132;2;133;2
WireConnection;70;0;62;0
WireConnection;75;0;65;0
WireConnection;75;2;68;0
WireConnection;52;0;132;0
WireConnection;72;0;66;4
WireConnection;72;1;64;4
WireConnection;74;0;67;0
WireConnection;74;1;63;0
WireConnection;80;0;70;0
WireConnection;81;0;74;0
WireConnection;81;1;72;0
WireConnection;76;0;75;0
WireConnection;76;1;71;0
WireConnection;78;0;69;0
WireConnection;78;1;70;0
WireConnection;85;0;76;0
WireConnection;88;0;77;0
WireConnection;83;0;78;3
WireConnection;87;0;79;0
WireConnection;87;1;80;0
WireConnection;82;0;81;0
WireConnection;91;0;84;0
WireConnection;91;1;86;0
WireConnection;91;2;88;0
WireConnection;89;0;83;0
WireConnection;89;1;87;3
WireConnection;96;0;95;0
WireConnection;96;1;93;0
WireConnection;98;0;94;1
WireConnection;98;1;94;2
WireConnection;98;2;94;3
WireConnection;99;0;91;0
WireConnection;99;2;90;0
WireConnection;101;0;89;0
WireConnection;101;1;92;1
WireConnection;101;2;92;2
WireConnection;103;0;98;0
WireConnection;103;1;101;0
WireConnection;104;0;99;0
WireConnection;104;1;100;0
WireConnection;104;2;96;0
WireConnection;110;0;104;0
WireConnection;108;0;103;0
WireConnection;106;1;102;0
WireConnection;106;2;105;0
WireConnection;112;0;108;0
WireConnection;112;1;107;0
WireConnection;111;0;106;0
WireConnection;111;2;109;0
WireConnection;116;0;111;0
WireConnection;116;1;113;0
WireConnection;116;2;115;0
WireConnection;117;0;114;0
WireConnection;117;1;112;0
WireConnection;119;0;116;0
WireConnection;118;0;117;0
WireConnection;1;0;121;0
WireConnection;1;3;136;0
WireConnection;1;4;136;0
WireConnection;1;6;122;0
ASEEND*/
//CHKSM=C5CE71021F67E5DBE2EE06B5E70DE5ABB28242F9