//Custom variables
uniform fixed4 _GrassColorTint;
uniform float _GrassSpecularTresholdHeight;
uniform float _GrassSpecularTresholdSmoothness;
uniform float _GrassGlossStren;
uniform float _GrassSpecStren;
uniform fixed4 _GrassSpecColor;

void surf(Input IN, inout SurfaceOutputStandardSpecular o) {
	fixed4 c = tex2D(_MainTex, IN.uv_MainTex) * fixed4(IN.color.rgb,1) * _GrassColorTint;
	o.Albedo = c.rgb;
	o.Alpha = c.a;
	float spec = ((_GrassSpecularTresholdHeight + IN.color.a - 0.5f) * _GrassSpecularTresholdSmoothness) + 0.5f;
	spec = clamp(spec, 0, 1);
	o.Smoothness = spec * _GrassGlossStren;
	o.Specular = (spec + 0.000001) * _GrassSpecStren * _GrassSpecColor;
	clip(o.Alpha - _Cutoff);
}