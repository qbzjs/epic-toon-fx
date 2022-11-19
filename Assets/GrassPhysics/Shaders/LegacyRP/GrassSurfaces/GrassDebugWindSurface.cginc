#define DEBUG_GRASS_WIND

void surf(Input IN, inout SurfaceOutput o) {
	fixed4 c = tex2D(_MainTex, IN.uv_MainTex);
	o.Albedo = IN.color.rgb;
	o.Alpha = c.a;
	clip(o.Alpha - _Cutoff);
}