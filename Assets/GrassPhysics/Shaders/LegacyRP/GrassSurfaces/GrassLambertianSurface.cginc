uniform fixed4 _GrassColorTint;

void surf(Input IN, inout SurfaceOutput o) {
	fixed4 c = tex2D(_MainTex, IN.uv_MainTex) * fixed4(IN.color.rgb,1) * _GrassColorTint;
	o.Albedo = c.rgb;
	o.Alpha = c.a;
	clip(o.Alpha - _Cutoff);
}