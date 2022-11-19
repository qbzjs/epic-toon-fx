void FastSinCos(float4 val, out float4 s, out float4 c) {
	val = val * 6.408849 - 3.1415927;
	// powers for taylor series
	float4 r5 = val * val;                  // wavevec ^ 2
	float4 r6 = r5 * r5;                        // wavevec ^ 4;
	float4 r7 = r6 * r5;                        // wavevec ^ 6;
	float4 r8 = r6 * r5;                        // wavevec ^ 8;

	float4 r1 = r5 * val;                   // wavevec ^ 3
	float4 r2 = r1 * r5;                        // wavevec ^ 5;
	float4 r3 = r2 * r5;                        // wavevec ^ 7;


	//Vectors for taylor's series expansion of sin and cos
	float4 sin7 = { 1, -0.16161616, 0.0083333, -0.00019841 };
	float4 cos8 = { -0.5, 0.041666666, -0.0013888889, 0.000024801587 };

	// sin
	s = val + r1 * sin7.y + r2 * sin7.z + r3 * sin7.w;

	// cos
	c = 1 + r5 * cos8.x + r6 * cos8.y + r7 * cos8.z + r8 * cos8.w;
}

float4 _CameraPosition;

half4 TerrainWaveGrass(inout float4 vertex, float waveAmount, half4 color, float4 _WaveAndDistance, float4 _WavingTint)
{
	half4 _waveXSize = half4(0.012, 0.02, 0.06, 0.024) * _WaveAndDistance.y;
	half4 _waveZSize = half4 (0.006, .02, 0.02, 0.05) * _WaveAndDistance.y;
	half4 waveSpeed = half4 (1.2, 2, 1.6, 4.8);

	half4 _waveXmove = half4(0.024, 0.04, -0.12, 0.096);
	half4 _waveZmove = half4 (0.006, .02, -0.02, 0.1);

	float4 waves;
	waves = vertex.x * _waveXSize;
	waves += vertex.z * _waveZSize;

	// Add in time to model them over time
	waves += _WaveAndDistance.x * waveSpeed;

	float4 s, c;
	waves = frac(waves);
	FastSinCos(waves, s, c);

	s = s * s;

	s = s * s;

	half lighting = dot(s, normalize(half4 (1, 1, .4, .2))) * 0.7;

	s = s * waveAmount;

	half3 waveMove = 0;
	waveMove.x = dot(s, _waveXmove);
	waveMove.z = dot(s, _waveZmove);

	vertex.xz -= waveMove.xz * _WaveAndDistance.z;

	// apply color animation
	half3 waveColor = lerp(real3(0.5, 0.5, 0.5), _WavingTint.rgb, lighting);

	// Fade the grass out before detail distance.
	// Saturate because Radeon HD drivers on OS X 10.4.10 don't saturate vertex colors properly.
	half3 offset = vertex.xyz - _CameraPosition.xyz;
	color.a = saturate(2 * (_WaveAndDistance.w - dot(offset, offset)) * _CameraPosition.w);

	return half4(2 * waveColor * color.rgb, color.a);
}

void GrassDoWind_float(float deformAmount, float windSpeed, float windScale, float windInfluence, float4 waveTint, float4 color, float3 vertex, out float3 vertOut, out float4 colorOut)
{
	float4 _WaveAndDistance = float4(windSpeed * _Time.x * 2, windScale * 0.5, windInfluence * 6, 100);

	float waveAmount = color.a * _WaveAndDistance.z;
	float4 vertexGlobal = mul(unity_ObjectToWorld, float4(vertex, 1));
	float4 vertexCopy = vertexGlobal;
	float4 tintColor = TerrainWaveGrass(vertexCopy, waveAmount, color, _WaveAndDistance, waveTint);
	color = float4(lerp(color.rgb, tintColor.rgb, waveTint.a), color.a);
	float2 move = deformAmount * float2(vertexGlobal.x - vertexCopy.x, vertexGlobal.z - vertexCopy.z);

	vertex -= mul(unity_WorldToObject, float4(move.x, 0, move.y, 0)).xyz;
	vertOut = vertex;
	colorOut = color;
}