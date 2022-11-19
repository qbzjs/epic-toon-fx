﻿Shader "GrassPhysics/Physics Global Settings/Standard" {
	Properties {
		_Color ("Color", Color) = (1,1,1,1)
		_MainTex ("Albedo (RGB)", 2D) = "white" {}
		_Cutoff("Cutoff", Range(0,1)) = 0.5

		[Space]
		_Glossiness ("Smoothness", Range(0,1)) = 0.5
		_Metallic ("Metallic", Range(0,1)) = 0.0
		[Normal] _BumpMap("Normal Map", 2D) = "bump" {}

		[Space]
		_WavingTint("Wind Fade Color", Color) = (.7,.6,.5, 0)
		_WaveSpeed("Wind Speed", Range(0,1)) = 0.5
		_WaveScale("Wind Scale", Range(0,1)) = 0.5
		_WaveInfluence("Wind Influence", Range(0,1)) = 0.5

		[Space]
		[Toggle]
		_CanDeformUp("Can Deform Upwards",int) = 0
	}

	SubShader{
		Tags {
			"Queue" = "Geometry"
			"RenderType" = "Opaque"
		}


		CGPROGRAM
		// Physically based Standard lighting model, and enable shadows on all light types
		#pragma surface surf Standard addshadow exclude_path:deferred
		#pragma vertex PhysicsWavingMeshVert
		#include "../../Core/GrassSettings.cginc"
		#include "../../Core/GrassVertex.cginc"

		// Use shader model 3.0 target, to get nicer looking lighting
		#pragma target 3.0

		sampler2D _MainTex;
		sampler2D _BumpMap;

		struct Input {
			float2 uv_MainTex;
			float2 uv_BumpMap;
			fixed4 color : COLOR;
		};

		half _Glossiness;
		half _Metallic;
		fixed4 _Color;
		fixed _Cutoff;

		// Add instancing support for this shader. You need to check 'Enable Instancing' on materials that use the shader.
		// See https://docs.unity3d.com/Manual/GPUInstancing.html for more information about instancing.
		// #pragma instancing_options assumeuniformscaling
		UNITY_INSTANCING_BUFFER_START(Props)
			// put more per-instance properties here
		UNITY_INSTANCING_BUFFER_END(Props)

		void surf (Input IN, inout SurfaceOutputStandard o) {
			// Albedo comes from a texture tinted by color
			fixed4 c = tex2D(_MainTex, IN.uv_MainTex) * _Color;
			o.Albedo = c.rgb * IN.color.rgb;
			// Metallic and smoothness come from slider variables
			o.Metallic = _Metallic;
			o.Smoothness = _Glossiness;
			o.Normal = UnpackNormal(tex2D(_BumpMap, IN.uv_BumpMap));
			o.Alpha = c.a;
			clip(o.Alpha - _Cutoff);
		}
		ENDCG
	}
	FallBack "Standard"
}
