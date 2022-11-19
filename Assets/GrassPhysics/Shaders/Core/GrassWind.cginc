#ifndef GRASS_WIND_INCLUDED
#define GRASS_WIND_INCLUDED

#include "TerrainEngine.cginc"

void SetWindProperties(float windSpeed, float windScale, float windInfluence)
{
	_WaveAndDistance = float4(windSpeed * _Time.x * 2, windScale * 0.5, windInfluence * 6, 100);
}

float2 GrassDoWind(inout fixed4 color, float4 vertex)
{
	float waveAmount = color.a * _WaveAndDistance.z;
	float4 vertexGlobal = mul(unity_ObjectToWorld, vertex);
	float4 vertexCopy = vertexGlobal;
	fixed4 tintColor = TerrainWaveGrass(vertexCopy, waveAmount, color);
	color = fixed4(lerp(color.rgb, tintColor.rgb, _WavingTint.a), color.a);
	return float2(vertexGlobal.x - vertexCopy.x, vertexGlobal.z - vertexCopy.z);
}

#endif