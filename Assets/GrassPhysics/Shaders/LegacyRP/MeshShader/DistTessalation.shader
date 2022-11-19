Shader "GrassPhysics/Physics Local Settings/Distance Based Tessalation" {
	Properties{
			_Tess("Tessellation", Range(1,32)) = 7
			_MinDist("Min Distance", float) = 10
			_MaxDist("Max Distance", float) = 25

			[Space]
			_Color("Color", color) = (1,1,1,1)
			_GroundColor("Ground Color", color) = (1,1,1,1)
			_GroundColorThreshold("Ground Color Threshold", Range(0,2)) = 0.5
			_MainTex("Base (RGB)", 2D) = "white" {}
			_NormalMap("Normalmap", 2D) = "bump" {}

			[Space]
			_SpecColor("Spec Color", color) = (0.5,0.5,0.5,0.5)
			_SpecularStren("Specular Strength", Range(0,1)) = 0.2
			_GlossStren("Gloss Strength", Range(0,1)) = 1

			[Space]
			_DispTex("Displacement Texture", 2D) = "gray" {}
			_Displacement("Displacement", Range(0, 1.0)) = 0.3
			_TessHeight("Displacement Height", float) = 1

			[Space]
			_TessNormalSize("Normal Sampling Size", Range(0,1)) = 0.4

			[Space]
			_GrassPhysicsAreaPos("Physics Area Position", Vector) = (0,0,0,0)			//comment or delete this line if you want to use global value of this variable

			[Space]
			_GrassPhysicsAreaSize("Physics Area Size", vector) = (40, 40, 40, 0)	//comment or delete this line if you want to use global value of this variable
			_GrassPhysicsOffset("Physics Impact Offset", float) = 0					//comment or delete this line if you want to use global value of this variable
			_GrassTexEnlargement("Physics Impact Enlargement", float) = 0.2			//comment or delete this line if you want to use global value of this variable
			_GrassDepthTex("Physics Depth Texture", 2D) = "black" {}				//comment or delete this line if you want to use global value of this variable
	}
		SubShader{
			Tags { "RenderType" = "Opaque" }
			LOD 300

			CGPROGRAM
					#pragma surface surf BlinnPhong vertex:disp tessellate:tessDistance nolightmap addshadow fullforwardshadows 
					#pragma target 4.6
					#define PHYSICS_TESS 1
					#include "Tessellation.cginc"
					#include "../../Core/GrassPhysics.cginc"

					struct appdata {
						float4 vertex : POSITION;
						float4 tangent : TANGENT;
						float3 normal : NORMAL;
						float2 texcoord : TEXCOORD0;
						float4 color : COLOR;
					};

					float _Tess;
					float _MinDist;
					float _MaxDist;

					float4 tessDistance(appdata v0, appdata v1, appdata v2) {
						float minDist = _MinDist;
						float maxDist = _MaxDist;

						return UnityDistanceBasedTess(v0.vertex, v1.vertex, v2.vertex, minDist, maxDist, _Tess);
					}

					sampler2D _DispTex;
					float4 _DispTex_ST;
					float _Displacement;
					float _TessHeight;

					float _GroundColorThreshold;
					float4 _GroundColor;
					fixed4 _Color;

					void disp(inout appdata v)
					{
						float d = tex2Dlod(_DispTex, float4((v.texcoord.xy * _DispTex_ST.xy) + _DispTex_ST.zw, 0, 0)).r * _Displacement;
						v.vertex.xyz += v.normal * (d + _TessHeight);
						float displacement = GrassTessPhysics(v.vertex, v.normal);
						v.color = lerp(_Color, _GroundColor, displacement * _GroundColorThreshold);
					}

					struct Input {
						float2 uv_MainTex;
						float4 color : COLOR;
					};

					sampler2D _MainTex;
					sampler2D _NormalMap;
					float4 _NormalMap_ST;
					float _SpecularStren;
					float _GlossStren;

					void surf(Input IN, inout SurfaceOutput o) {
						half4 c = tex2D(_MainTex, IN.uv_MainTex) * IN.color;
						o.Albedo = c.rgb;
						o.Specular = _SpecularStren;
						o.Gloss = _GlossStren;
						o.Normal = UnpackNormal(tex2D(_NormalMap, (IN.uv_MainTex * _NormalMap_ST.xy) + _NormalMap_ST.zw));
					}
					ENDCG
	}

		FallBack "Diffuse"
}