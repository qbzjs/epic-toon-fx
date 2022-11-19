#define PHYSICS_TESS 1
#include "../../../Shaders/Core/GrassPhysics.cginc"

void GrassTessPhysics_float(float3 vertex, float3 normal, out float3 vertOut, out float3 normOut, out float displacement)
{
	float4 vert = float4(vertex, 1);
	displacement = GrassTessPhysics(vert, normal);
	normOut = normal;
	vertOut = vert.xyz;
}