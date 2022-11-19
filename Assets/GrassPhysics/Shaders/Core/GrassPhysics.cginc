#ifndef GRASS_PHYSICS_INCLUDED
#define GRASS_PHYSICS_INCLUDED

#include "GrassPhysicsSettings.cginc"

#define IS_TRUE(x) (x != 0)

#if PHYSICS_MULTI
uniform int _GrassPhysicsMode;
#endif

#if PHYSICS_SIMPLE
uniform bool _CanDeformUp;
uniform bool _CanTilt;
uniform uint _TargetsCount;
uniform half4 _TargetsPos[PHYSICS_SIMPLE_MAX_COUNT];
uniform half3 _DisplacementLimits;  // x - upward limit, y - downward limit, z - sideway limit
#endif

#if PHYSICS_FULL
uniform half _GrassTexEnlargement;
#endif

#if PHYSICS_TESS
uniform half _TessNormalSize;
#endif

#if PHYSICS_FULL || PHYSICS_TESS
uniform float3 _GrassPhysicsAreaPos;
uniform half _GrassPhysicsOffset;	//Depth Offset
uniform half3 _GrassPhysicsAreaSize;
uniform sampler2D _GrassDepthTex;

half2 getTexOffset(float2 vertexPosXZ)
{
	half2 texOffset = vertexPosXZ - _GrassPhysicsAreaPos.xz;
	texOffset.x /= _GrassPhysicsAreaSize.x;
	texOffset.y /= _GrassPhysicsAreaSize.z;
	texOffset += half2(0.5, 0.5);
	texOffset.y = 1 - texOffset.y;
	return texOffset;
}

float textureToWorldDisplacement(float texDisp, half vertexPosY)
{
	return (texDisp * _GrassPhysicsAreaSize.y + vertexPosY - _GrassPhysicsAreaPos.y - _GrassPhysicsAreaSize.y + _GrassPhysicsOffset);
}

float DoDisplacement(inout float4 vertex, float texDisp, half3 vertexPos, half2 texOffset)
{
	if (texDisp <= 0) return 0;

	half displacement = textureToWorldDisplacement(texDisp, vertexPos.y);

	if (displacement > 0 && texOffset.x >= 0 && texOffset.x < 1 && texOffset.y >= 0 && texOffset.y < 1)
	{
		vertex -= mul(unity_WorldToObject, float4(0, displacement, 0, 0));
		return displacement;
	}

	return 0;
}
#endif

#if PHYSICS_FULL
void GetMaxValueFromNearbyPixels(half2 texOffset, inout float texDisp)
{
	half accuracyZ = _GrassTexEnlargement / _GrassPhysicsAreaSize.z;
	half accuracyX = _GrassTexEnlargement / _GrassPhysicsAreaSize.x;
	texDisp = max(texDisp, tex2Dlod(_GrassDepthTex, half4(texOffset.xy + half2(0, accuracyZ), 0, 0)).r);
	texDisp = max(texDisp, tex2Dlod(_GrassDepthTex, half4(texOffset.xy - half2(0, accuracyZ), 0, 0)).r);
	texDisp = max(texDisp, tex2Dlod(_GrassDepthTex, half4(texOffset.xy + half2(accuracyX, 0), 0, 0)).r);
	texDisp = max(texDisp, tex2Dlod(_GrassDepthTex, half4(texOffset.xy - half2(accuracyX, 0), 0, 0)).r);
	texDisp = max(texDisp, tex2Dlod(_GrassDepthTex, half4(texOffset.xy + half2(accuracyX, accuracyZ), 0, 0)).r);
	texDisp = max(texDisp, tex2Dlod(_GrassDepthTex, half4(texOffset.xy - half2(accuracyX, accuracyZ), 0, 0)).r);
	texDisp = max(texDisp, tex2Dlod(_GrassDepthTex, half4(texOffset.xy + half2(accuracyX, -accuracyZ), 0, 0)).r);
	texDisp = max(texDisp, tex2Dlod(_GrassDepthTex, half4(texOffset.xy - half2(accuracyX, -accuracyZ), 0, 0)).r);
}

float GrassFullPhysics(inout float4 vertex)
{
	half3 vertexPos = mul(unity_ObjectToWorld, vertex).xyz;
	half2 texOffset = getTexOffset(vertexPos.xz);
	if (texOffset.x < 0 || texOffset.x > 1 || texOffset.y < 0 || texOffset.y > 1)
	{
		return 1;
	}
	float texDisp = tex2Dlod(_GrassDepthTex, half4(texOffset.xy, 0, 0)).r;

	//Optional enlargement (gets max value from nearby pixels)
	if (0 < _GrassTexEnlargement)
	{
		GetMaxValueFromNearbyPixels(texOffset, texDisp);
	}
	//

#ifndef UNITY_REVERSED_Z
	texDisp = 1 - texDisp;
#endif

	return max(0, 1 - DoDisplacement(vertex, texDisp, vertexPos, texOffset));
}
#endif

#if PHYSICS_TESS
void CalculateNormal(half2 texOffset, half vertexPosY, inout float texDisp, inout float3 normal)
{
	//Distance value to calculate normal
	half accuracyZ = _TessNormalSize / _GrassPhysicsAreaSize.z;
	half accuracyX = _TessNormalSize / _GrassPhysicsAreaSize.x;

	half n1, n2, n3, n4;

	n1 = tex2Dlod(_GrassDepthTex, half4(texOffset.xy + half2(0, accuracyZ), 0, 0)).r;
	n2 = tex2Dlod(_GrassDepthTex, half4(texOffset.xy - half2(0, accuracyZ), 0, 0)).r;
	n3 = tex2Dlod(_GrassDepthTex, half4(texOffset.xy + half2(accuracyX, 0), 0, 0)).r;
	n4 = tex2Dlod(_GrassDepthTex, half4(texOffset.xy - half2(accuracyX, 0), 0, 0)).r;

	texDisp = max(texDisp, (n1 + n2 + n3 + n4) / 4.0);

	n1 = max(0, textureToWorldDisplacement(n1, vertexPosY));
	n2 = max(0, textureToWorldDisplacement(n2, vertexPosY));
	n3 = max(0, textureToWorldDisplacement(n3, vertexPosY));
	n4 = max(0, textureToWorldDisplacement(n4, vertexPosY));

	half3 tan1 = normalize(half3(0, -n1 + n2, -accuracyZ * 2));
	half3 tan2 = normalize(half3(-accuracyX * 2, -n3 + n4, 0));
	half3 deform_normal = normalize(cross(tan1, tan2));
	half3 world_normal = mul(unity_ObjectToWorld, float4(normal, 0)).xyz;
	half3 world_forward = normalize(cross(half3(1, 0, 0), world_normal));
	half3 world_right = normalize(cross(world_forward, world_normal));
	normal = normalize(deform_normal.x * world_right + deform_normal.y * world_normal + deform_normal.z * world_forward);
	normal = normalize(mul(unity_WorldToObject, float4(normal, 0)).xyz);
}

float GrassTessPhysics(inout float4 vertex, inout float3 normal)
{
	half3 vertexPos = mul(unity_ObjectToWorld, vertex).xyz;
	half2 texOffset = getTexOffset(vertexPos.xz);
	if (texOffset.x < 0 || texOffset.x > 1 || texOffset.y < 0 || texOffset.y > 1)
	{
		return 0;
	}
	float texDisp = tex2Dlod(_GrassDepthTex, half4(texOffset.xy, 0, 0)).r;

	if (0 < _TessNormalSize && texOffset.x >= 0 && texOffset.x < 1 && texOffset.y >= 0 && texOffset.y < 1)
	{
		CalculateNormal(texOffset, vertexPos.y, texDisp, normal);
	}

#ifndef UNITY_REVERSED_Z
	texDisp = 1 - texDisp;
#endif

	return DoDisplacement(vertex, texDisp, vertexPos, texOffset);
}
#endif

#if PHYSICS_SIMPLE
float GrassSimplePhysics(inout float4 vertex)
{
	half3 vertPos = mul(unity_ObjectToWorld, vertex).xyz;
	half3 deformRes = half3(0, 0, 0);
	half deformAmount = 0;
	for (uint i=0; i<_TargetsCount; ++i) 
	{
		half3 dir = vertPos - _TargetsPos[i].xyz;
		half dirY = dir.y;

		//Check if vertex is in range
		if (!IS_TRUE(_CanDeformUp))
		{
			dir.y = min(0, dir.y);
		}
		half sqrHorizLen = dir.x*dir.x + dir.z*dir.z;
		half sqrLen = sqrHorizLen + dir.y*dir.y;
		half sqrRadius = _TargetsPos[i].w*_TargetsPos[i].w;
		if (sqrLen > sqrRadius) 
		{
			continue;
		}

		//Check if greater than max deform
		dir = normalize(dir) * (_TargetsPos[i].w - sqrt(sqrLen));
		if (length(dir) < length(deformRes))
		{
			continue;
		}

		//Calculate deform
		if (IS_TRUE(_CanTilt))
		{
			dir.y = ((dir.y > 0 && _CanDeformUp) ? -1 : 1) * -length(dir) * _TargetsPos[i].w;
		}
		else
		{
			dir.xz = half2(0, 0);
			dir.y = (((dir.y > 0 && _CanDeformUp) ? -1 : 1) * -sqrt(sqrRadius - sqrHorizLen)) - dirY;
		}

		//Return results
		deformRes = dir;
		if (_DisplacementLimits.x > 0) deformRes.y = min(deformRes.y, _DisplacementLimits.x); //upward limit
		if (_DisplacementLimits.y > 0) deformRes.y = max(deformRes.y, -_DisplacementLimits.y); //downward limit
		if (_DisplacementLimits.z > 0) deformRes.xz = normalize(deformRes.xz) * min(length(deformRes.xz), _DisplacementLimits.z); //sideway limit
		deformAmount = (length(dir) / _TargetsPos[i].w);
	}
	
	vertex.xyz += mul(unity_WorldToObject, half4(deformRes, 0)).xyz;

	return clamp(1 - deformAmount, 0, 1);
}
#endif

float GrassDoPhysics(inout float4 vertex)
{

#if PHYSICS_MULTI
	switch (_GrassPhysicsMode)
	{
	case 0:
		return 1;
		break;
	case 1:
#endif
#if PHYSICS_SIMPLE
		return GrassSimplePhysics(vertex);
#endif
#if PHYSICS_MULTI
		break;
	case 2:
#endif
#if PHYSICS_FULL
		return GrassFullPhysics(vertex);
#endif
#if PHYSICS_MULTI
		break;
	}
#endif
	return 1;

}

#endif