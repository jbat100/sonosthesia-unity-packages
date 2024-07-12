#ifndef NOISE_WU_SIMPLEX_4D_FUNC
#define NOISE_WU_SIMPLEX_4D_FUNC

// source : https://github.com/atyuwen/bitangent_noise/blob/main/Develop/SimplexNoise.hlsl

//	--------------------------------------------------------------------
//	Optimized implementation of simplex noise.
//	Based on stegu's simplex noise: https://github.com/stegu/webgl-noise.
//	Contact : atyuwen@gmail.com
//	Author : Yuwen Wu (https://atyuwen.github.io/)
//	License : Distributed under the MIT License.
//	--------------------------------------------------------------------

// Permuted congruential generator (only top 16 bits are well shuffled).
// References: 1. Mark Jarzynski and Marc Olano, "Hash Functions for GPU Rendering".
//             2. UnrealEngine/Random.ush. https://github.com/EpicGames/UnrealEngine
uint pcg3d16(uint3 p)
{
	uint3 v = p * 1664525u + 1013904223u;
	v.x += v.y*v.z; v.y += v.z*v.x; v.z += v.x*v.y;
	v.x += v.y*v.z;
	return v.x;
}
uint pcg4d16(uint4 p)
{
	uint4 v = p * 1664525u + 1013904223u;
	v.x += v.y*v.w; v.y += v.z*v.x; v.z += v.x*v.y; v.w += v.y*v.z;
	v.x += v.y*v.w;
	return v.x;
}


// Get random gradient from hash value.
float3 gradient3d(uint hash)
{
	static const uint3 hash_mask = uint3(0x80000, 0x40000, 0x20000);
	static const float3 multiplier = float3(1.0 / 0x40000, 1.0 / 0x20000, 1.0 / 0x10000);
	
	const float3 g = float3(hash.xxx & hash_mask);
	return g * multiplier - 1.0;
}

float4 gradient4d(uint hash)
{
	static const uint4 hash_mask = uint4(0x80000, 0x40000, 0x20000, 0x10000);
	static const float4 multiplier = float4(1.0 / 0x40000, 1.0 / 0x20000, 1.0 / 0x10000, 1.0 / 0x8000);
	
	const float4 g = float4(hash.xxxx & hash_mask);
	return g * multiplier - 1.0;
}

// 3D Simplex Noise. Approximately 71 instruction slots used.
// Assume p is in the range [-32768, 32767].
float WuSimplexNoise3D(float3 p)
{
	static const float2 C = float2(1.0 / 6.0, 1.0 / 3.0);
	static const float4 D = float4(0.0, 0.5, 1.0, 2.0);

	// First corner
	float3 i = floor(p + dot(p, C.yyy));
	float3 x0 = p - i + dot(i, C.xxx);

	// Other corners
	float3 g = step(x0.yzx, x0.xyz);
	float3 l = 1.0 - g;
	float3 i1 = min(g.xyz, l.zxy);
	float3 i2 = max(g.xyz, l.zxy);

	// x0 = x0 - 0.0 + 0.0 * C.xxx;
	// x1 = x0 - i1  + 1.0 * C.xxx;
	// x2 = x0 - i2  + 2.0 * C.xxx;
	// x3 = x0 - 1.0 + 3.0 * C.xxx;
	float3 x1 = x0 - i1 + C.xxx;
	float3 x2 = x0 - i2 + C.yyy; // 2.0*C.x = 1/3 = C.y
	float3 x3 = x0 - D.yyy;      // -1.0+3.0*C.x = -0.5 = -D.y

	i = i + 32768.5;
	uint hash0 = pcg3d16((uint3)i);
	uint hash1 = pcg3d16((uint3)(i + i1));
	uint hash2 = pcg3d16((uint3)(i + i2));
	uint hash3 = pcg3d16((uint3)(i + 1 ));

	float3 p0 = gradient3d(hash0);
	float3 p1 = gradient3d(hash1);
	float3 p2 = gradient3d(hash2);
	float3 p3 = gradient3d(hash3);

	// Mix final noise value.
	float4 m = saturate(0.5 - float4(dot(x0, x0), dot(x1, x1), dot(x2, x2), dot(x3, x3)));
	float4 mt = m * m;
	float4 m4 = mt * mt;
	return 62.6 * dot(m4, float4(dot(x0, p0), dot(x1, p1), dot(x2, p2), dot(x3, p3)));
}

// 4D Simplex Noise. Approximately 113 instruction slots used.
// Assume p is in the range [-32768, 32767].
float WuSimplexNoise4D_float(float4 p)
{
	static const float4 F4 = 0.309016994374947451;
	static const float4  C = float4( 0.138196601125011,  // (5 - sqrt(5))/20  G4
	                          0.276393202250021,  // 2 * G4
	                          0.414589803375032,  // 3 * G4
	                         -0.447213595499958); // -1 + 4 * G4

	// First corner
	float4 i  = floor(p + dot(p, F4) );
	float4 x0 = p -   i + dot(i, C.xxxx);

	// Other corners

	// Rank sorting originally contributed by Bill Licea-Kane, AMD (formerly ATI)
	float4 i0;
	float3 isX = step( x0.yzw, x0.xxx );
	float3 isYZ = step( x0.zww, x0.yyz );
	// i0.x = dot( isX, float3( 1.0 ) );
	i0.x = isX.x + isX.y + isX.z;
	i0.yzw = 1.0 - isX;
	// i0.y += dot( isYZ.xy, float2( 1.0 ) );
	i0.y += isYZ.x + isYZ.y;
	i0.zw += 1.0 - isYZ.xy;
	i0.z += isYZ.z;
	i0.w += 1.0 - isYZ.z;

	// i0 now contains the unique values 0,1,2,3 in each channel
	float4 i3 = saturate( i0 );
	float4 i2 = saturate( i0 - 1.0 );
	float4 i1 = saturate( i0 - 2.0 );

	// x0 = x0 - 0.0 + 0.0 * C.xxxx
	// x1 = x0 - i1  + 1.0 * C.xxxx
	// x2 = x0 - i2  + 2.0 * C.xxxx
	// x3 = x0 - i3  + 3.0 * C.xxxx
	// x4 = x0 - 1.0 + 4.0 * C.xxxx
	float4 x1 = x0 - i1 + C.xxxx;
	float4 x2 = x0 - i2 + C.yyyy;
	float4 x3 = x0 - i3 + C.zzzz;
	float4 x4 = x0 + C.wwww;

	i = i + 32768.5;
	uint hash0 = pcg4d16((uint4)i);
	uint hash1 = pcg4d16((uint4)(i + i1));
	uint hash2 = pcg4d16((uint4)(i + i2));
	uint hash3 = pcg4d16((uint4)(i + i3));
	uint hash4 = pcg4d16((uint4)(i + 1 ));

	float4 p0 = gradient4d(hash0);
	float4 p1 = gradient4d(hash1);
	float4 p2 = gradient4d(hash2);
	float4 p3 = gradient4d(hash3);
	float4 p4 = gradient4d(hash4);

	// Mix contributions from the five corners
	float3 m0 = saturate(0.6 - float3(dot(x0,x0), dot(x1,x1), dot(x2,x2)));
	float2 m1 = saturate(0.6 - float2(dot(x3,x3), dot(x4,x4)            ));
	float3 m03 = m0 * m0 * m0;
	float2 m13 = m1 * m1 * m1;
	return (dot(m03, float3(dot(p0, x0), dot(p1, x1), dot(p2, x2)))
	      + dot(m13, float2(dot(p3, x3), dot(p4, x4)))) * 9.0;
}


// 4D Simplex Noise. Approximately 113 instruction slots used.
// Assume p is in the range [-32768, 32767].
half WuSimplexNoise4D_half(half4 p)
{
	static const half4 F4 = 0.309016994374947451;
	static const half4  C = half4( 0.138196601125011,  // (5 - sqrt(5))/20  G4
	                          0.276393202250021,  // 2 * G4
	                          0.414589803375032,  // 3 * G4
	                         -0.447213595499958); // -1 + 4 * G4

	// First corner
	half4 i  = floor(p + dot(p, F4) );
	half4 x0 = p -   i + dot(i, C.xxxx);

	// Other corners

	// Rank sorting originally contributed by Bill Licea-Kane, AMD (formerly ATI)
	half4 i0;
	half3 isX = step( x0.yzw, x0.xxx );
	half3 isYZ = step( x0.zww, x0.yyz );
	// i0.x = dot( isX, half3( 1.0 ) );
	i0.x = isX.x + isX.y + isX.z;
	i0.yzw = 1.0 - isX;
	// i0.y += dot( isYZ.xy, half2( 1.0 ) );
	i0.y += isYZ.x + isYZ.y;
	i0.zw += 1.0 - isYZ.xy;
	i0.z += isYZ.z;
	i0.w += 1.0 - isYZ.z;

	// i0 now contains the unique values 0,1,2,3 in each channel
	half4 i3 = saturate( i0 );
	half4 i2 = saturate( i0 - 1.0 );
	half4 i1 = saturate( i0 - 2.0 );

	// x0 = x0 - 0.0 + 0.0 * C.xxxx
	// x1 = x0 - i1  + 1.0 * C.xxxx
	// x2 = x0 - i2  + 2.0 * C.xxxx
	// x3 = x0 - i3  + 3.0 * C.xxxx
	// x4 = x0 - 1.0 + 4.0 * C.xxxx
	half4 x1 = x0 - i1 + C.xxxx;
	half4 x2 = x0 - i2 + C.yyyy;
	half4 x3 = x0 - i3 + C.zzzz;
	half4 x4 = x0 + C.wwww;

	i = i + 32768.5;
	uint hash0 = pcg4d16((uint4)i);
	uint hash1 = pcg4d16((uint4)(i + i1));
	uint hash2 = pcg4d16((uint4)(i + i2));
	uint hash3 = pcg4d16((uint4)(i + i3));
	uint hash4 = pcg4d16((uint4)(i + 1 ));

	half4 p0 = gradient4d(hash0);
	half4 p1 = gradient4d(hash1);
	half4 p2 = gradient4d(hash2);
	half4 p3 = gradient4d(hash3);
	half4 p4 = gradient4d(hash4);

	// Mix contributions from the five corners
	half3 m0 = saturate(0.6 - half3(dot(x0,x0), dot(x1,x1), dot(x2,x2)));
	half2 m1 = saturate(0.6 - half2(dot(x3,x3), dot(x4,x4)            ));
	half3 m03 = m0 * m0 * m0;
	half2 m13 = m1 * m1 * m1;
	return (dot(m03, half3(dot(p0, x0), dot(p1, x1), dot(p2, x2)))
	      + dot(m13, half2(dot(p3, x3), dot(p4, x4)))) * 9.0;
}

//----------------------- Settings -------------------------

// spark settings from https://alteredqualia.com/three/examples/webgl_shader_sparks.html

void WuDynamicSparkNoise_float(float3 coordinates, float time, out float Out)
{
    float4 coord = float4(coordinates, time);
    float n = 0.0;

    n += 1.0 * abs(WuSimplexNoise4D_float(coord));
    n += 0.5 * abs(WuSimplexNoise4D_float(coord * 2.0));
    n += 0.25 * abs(WuSimplexNoise4D_float(coord * 4.0));
    n += 0.125 * abs(WuSimplexNoise4D_float(coord * 8.0));

    const float rn = 1.0 - n;
    Out = rn * rn;
}

void WuDynamicSparkNoise_half(half3 coordinates, half time, out half Out)
{
    half4 coord = half4(coordinates, time);
    half n = 0.0;

    n += 1.0 * abs(WuSimplexNoise4D_half(coord));
    n += 0.5 * abs(WuSimplexNoise4D_half(coord * 2.0));
    n += 0.25 * abs(WuSimplexNoise4D_half(coord * 4.0));
    n += 0.125 * abs(WuSimplexNoise4D_half(coord * 8.0));

    const half rn = 1.0 - n;
    Out = rn * rn;
}

void WuLDDynamicSparkNoise_float(float3 coordinates, float time, out float Out)
{
	float4 coord = float4(coordinates, time);
	float n = 0.0;

	n += 1.0 * abs(WuSimplexNoise4D_float(coord));
	n += 0.5 * abs(WuSimplexNoise4D_float(coord * 2.0));
	n += 0.25 * abs(WuSimplexNoise4D_float(coord * 4.0));

	const float rn = 1.0 - n;
	Out = rn * rn;
}

void WuLDDynamicSparkNoise_half(half3 coordinates, half time, out half Out)
{
	half4 coord = half4(coordinates, time);
	half n = 0.0;

	n += 1.0 * abs(WuSimplexNoise4D_half(coord));
	n += 0.5 * abs(WuSimplexNoise4D_half(coord * 2.0));
	n += 0.25 * abs(WuSimplexNoise4D_half(coord * 4.0));

	const half rn = 1.0 - n;
	Out = rn * rn;
}

// amber settings from https://alteredqualia.com/three/examples/webgl_shader_amber.html

#endif