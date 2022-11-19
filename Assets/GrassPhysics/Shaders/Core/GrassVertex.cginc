#ifndef GRASS_VERTEX_INCLUDED
#define GRASS_VERTEX_INCLUDED

#include "GrassWind.cginc"
#include "GrassPhysics.cginc"

void PhysicsWavingGrassVert(inout appdata_full v)
{
	//How much has deformed
	float deform = 0;

	//Do physics if vertex can move
	if (v.color.a > 0.01f)
	{
		deform = GrassDoPhysics(v.vertex);
	}

	//Do wind
	float2 move = deform * GrassDoWind(v.color, v.vertex);

#ifdef DEBUG_GRASS_WIND
	v.color.rgb = float3(move, 1);
#endif

	//Move vertex
	v.vertex -= mul(unity_WorldToObject, float4(move.x, 0, move.y, 0));
}

float _WaveSpeed;
float _WaveScale;
float _WaveInfluence;

void PhysicsWavingMeshVert(inout appdata_full v)
{
	SetWindProperties(_WaveSpeed, _WaveScale, _WaveInfluence);
	PhysicsWavingGrassVert(v);
}

#endif