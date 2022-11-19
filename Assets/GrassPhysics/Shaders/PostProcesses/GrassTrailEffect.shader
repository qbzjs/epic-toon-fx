Shader "Hidden/GrassPhysics/GrassTrailEffect"
{
	Properties
	{
		_MainTex ("Texture", 2D) = "black" {}
		_Movement ("Movement", Vector) = (0,0,0,0)
		_PrevTex ("PreviousTex", 2D) = "black" {}
		_RecoverySpeed ("Recovery Speed", Range(0,1)) = 0.01
	}
	SubShader
	{
		Cull Off ZWrite Off ZTest Always

		Pass
		{
			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag
			
			#include "UnityCG.cginc"

			struct appdata
			{
				float4 vertex : POSITION;
				float2 uv : TEXCOORD0;
			};

			struct v2f
			{
				float2 uv : TEXCOORD0;
				float4 vertex : SV_POSITION;
			};

			sampler2D _MainTex;
			float4 _MainTex_ST;

			sampler2D _PrevTex;
			float3 _Movement;
			float _RecoverySpeed;
			
			v2f vert (appdata v)
			{
				v2f o;
				o.vertex = UnityObjectToClipPos(v.vertex);
				o.uv = TRANSFORM_TEX(v.uv, _MainTex);
				return o;
			}
			
			fixed4 frag (v2f i) : SV_Target
			{
				// sample the texture
				fixed col = tex2D(_MainTex, i.uv).r;
				fixed2 newUv = i.uv + fixed2(-_Movement.x, _Movement.z);
				fixed prev = (newUv.x >= 0 && newUv.x < 1 && newUv.y >= 0 && newUv.y < 1)?tex2D(_PrevTex, newUv).r:0;
				col = max(col, max(0,prev - _Movement.y - _RecoverySpeed));
				return fixed4(col,0,0,1);
			}
			ENDCG
		}
	}
}
