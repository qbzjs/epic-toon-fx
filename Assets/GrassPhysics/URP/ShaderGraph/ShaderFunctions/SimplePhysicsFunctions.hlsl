#include "../../../Shaders/Core/GrassPhysicsSettings.cginc"

#define IS_TRUE(x) (x != 0)

uint _TargetsCount;
uniform half4 _TargetsPos[PHYSICS_SIMPLE_MAX_COUNT];

float GrassSimplePhysics(bool canDeformUp, bool canTilt, half3 displacementLimits, inout float4 vertex)
{
	half3 vertPos = mul(unity_ObjectToWorld, vertex).xyz;
	half3 deformRes = half3(0, 0, 0);
	half deformAmount = 0;
	for (uint i = 0; i < _TargetsCount; ++i)
	{
		half3 dir = vertPos - _TargetsPos[i].xyz;
		half dirY = dir.y;

		//Check if vertex is in range
		if (!IS_TRUE(canDeformUp))
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
		if (IS_TRUE(canTilt))
		{
			dir.y = ((dir.y > 0 && canDeformUp) ? -1 : 1) * -length(dir) * _TargetsPos[i].w;
		}
		else
		{
			dir.xz = half2(0, 0);
			dir.y = (((dir.y > 0 && canDeformUp) ? -1 : 1) * -sqrt(sqrRadius - sqrHorizLen)) - dirY;
		}

		//Return results
		deformRes = dir;
		if (displacementLimits.x > 0) deformRes.y = min(deformRes.y, displacementLimits.x); //upward limit
		if (displacementLimits.y > 0) deformRes.y = max(deformRes.y, -displacementLimits.y); //downward limit
		if (displacementLimits.z > 0) deformRes.xz = normalize(deformRes.xz) * min(length(deformRes.xz), displacementLimits.z); //sideway limit
		deformAmount = (length(dir) / _TargetsPos[i].w);
	}

	vertex.xyz += mul(unity_WorldToObject, half4(deformRes, 0)).xyz;

	return clamp(1 - deformAmount, 0, 1);
}

void GrassSimplePhysics_float(float3 vertex, bool canDeformUp, bool canTilt, half3 displacementLimits, out float3 vertOut, out float deformAmount)
{
	float4 vert = float4(vertex, 1.0);
	deformAmount = GrassSimplePhysics(canDeformUp, canTilt, displacementLimits, vert);
	vertOut = vert.xyz;
}