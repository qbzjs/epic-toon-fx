#ifndef UNIVERSAL_PBR_GRASS_SURFACE
#define UNIVERSAL_PBR_GRASS_SURFACE

#include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Lighting.hlsl"

uniform half4 _GrassColorTint;
uniform float _GrassSpecularTresholdHeight;
uniform float _GrassSpecularTresholdSmoothness;
uniform half _GrassMetallic;
uniform half _GrassSmoothness;
uniform half4 _GrassEmission;

// Used for StandardSimpleLighting shader
half4 surf(GrassVertexOutput input) : SV_Target
{
    UNITY_SETUP_INSTANCE_ID(input);
    UNITY_SETUP_STEREO_EYE_INDEX_POST_VERTEX(input);

    float2 uv = input.uv;
    half4 diffuseAlpha = SampleAlbedoAlpha(uv, TEXTURE2D_ARGS(_MainTex, sampler_MainTex));
	half3 diffuse = diffuseAlpha.rgb * input.color.rgb * _GrassColorTint.rgb;

    half alpha = diffuseAlpha.a;
    AlphaDiscard(alpha, _Cutoff);
    alpha *= input.color.a;

    half3 emission = _GrassEmission.rgb;
	half3 specularGloss = 0.1;// SampleSpecularSmoothness(uv, diffuseAlpha.a, _SpecColor, TEXTURE2D_ARGS(_SpecGlossMap, sampler_SpecGlossMap));
	half shininess = input.posWSShininess.w;

    InputData inputData;
    InitializeInputData(input, inputData);

	half metallic = _GrassMetallic;

	float spec = ((_GrassSpecularTresholdHeight + input.color.a - 0.5f) * _GrassSpecularTresholdSmoothness) + 0.5f;
	spec = clamp(spec, 0, 1);
	half smoothness = spec * _GrassSmoothness;
	half occlusion = spec;
	half4 color = UniversalFragmentPBR(inputData, diffuse, metallic, specularGloss, smoothness, occlusion, emission, alpha);
	//half4 color = UniversalFragmentBlinnPhong(inputData, diffuse, specularGloss, shininess, emission, alpha);
    color.rgb = MixFog(color.rgb, inputData.fogCoord);
    return color;
};

#endif
