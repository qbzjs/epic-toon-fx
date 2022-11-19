Shader "GrassPhysics/Physics Local Settings/Standard Specular (double sided)"
{

	Properties {
		_Color("Color", Color) = (1,1,1,1)
		_MainTex ("Base (RGB) Alpha (A)", 2D) = "white" {}
		_Cutoff("Cutoff", Range(0,1)) = 0.5

		[Space]
		_GlossMap("Smoothness map", 2D) = "white" {}
		_GlossStren("Smoothness strength", Range(0,1)) = 0

		[Space]
		_SpecularColor("Specular color", Color) = (1,1,1,1)
		_SpecularStren("Specular strength", Range(0,1)) = 0

		[Space]
		_WavingTint("Wind Fade Color", Color) = (.7,.6,.5, 0)
		_WaveSpeed("Wind Speed", Range(0,1)) = 0.5
		_WaveScale("Wind Scale", Range(0,1)) = 0.5
		_WaveInfluence("Wind Influence", Range(0,1)) = 0.5

		[Space]
		[Toggle]
		_CanDeformUp("Can Deform Upwards",int) = 0

		[Space]
		_GrassPhysicsAreaPos("Physics Area position", Vector) = (0,0,0,0)		//comment or delete this line if you want to use global value of this variable

		[HideInInspector] _TargetsCount("Grass actors count", float) = 0
		[HideInInspector] _TargetsPos("Grass actors positions", float) = 0

		_GrassPhysicsAreaSize("Physics area size", vector) = (40, 40, 40, 40)	//comment or delete this line if you want to use global value of this variable
		_GrassPhysicsOffset("Physics impact offset", float) = 0					//comment or delete this line if you want to use global value of this variable
		_GrassTexEnlargement("Physics impact enlargement", float) = 0.2			//comment or delete this line if you want to use global value of this variable
		_GrassDepthTex("Physics depth texture", 2D) = "black" {}				//comment or delete this line if you want to use global value of this variable

		_DisplacementLimits("Vertex displacement limits (downwards, upwards, sideway) works if greater than zero", Vector) = (0, 0, 0, 0)
		[Toggle] _CanTilt("Enable grass tilting", int) = 1						//comment or delete this line if you want to use global value of this variable
	}

	SubShader {
		Tags {
			"Queue" = "Geometry"
			"RenderType"="Opaque"
		}
		Cull Off
		LOD 200
		ColorMask RGB

		CGPROGRAM
		#pragma surface surf StandardSpecular vertex:PhysicsWavingMeshVert addshadow exclude_path:deferred
		#pragma multi_compile PHYSICS_SIMPLE PHYSICS_FULL
		#pragma multi_compile DEBUG_GRASS_WIND NO_DEBUG
		#include "../../Core/GrassVertex.cginc"

		sampler2D _MainTex;
		fixed _Cutoff;

		fixed4 _Color;
		sampler2D _GlossMap;
		float _GlossStren;
		float _SpecularStren;
		fixed4 _SpecularColor;

		struct Input {
			float2 uv_MainTex;
			float2 uv_GlossMap;
			fixed4 color : COLOR;
		};

		void surf (Input IN, inout SurfaceOutputStandardSpecular o) {
			fixed4 c = tex2D(_MainTex, IN.uv_MainTex) * _Color;
			o.Albedo = c.rgb * IN.color.rgb;
#if DEBUG_GRASS_WIND
			o.Albedo = IN.color.rgb;
#endif
			o.Alpha = c.a;
			float gloss = tex2D(_GlossMap, IN.uv_GlossMap).r;
			o.Smoothness = gloss * _GlossStren;
			o.Specular = (gloss + 0.000001) * _SpecularStren * _SpecularColor;
			clip (o.Alpha - _Cutoff);
		}
		ENDCG
	}

	CustomEditor "ShadedTechnology.GrassPhysics.PhysicsMaterialEditor"
    Fallback Off
}
