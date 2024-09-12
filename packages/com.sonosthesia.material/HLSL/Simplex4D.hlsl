#ifndef NOISE_SIMPLEX_4D_FUNC
#define NOISE_SIMPLEX_4D_FUNC

/*
 
Description:
    Array- and textureless CgFx/HLSL 2D, 3D and 4D simplex noise functions.
    a.k.a. simplified and optimized Perlin noise.
 
    The functions have very good performance
    and no dependencies on external data.
 
    2D - Very fast, very compact code.
    3D - Fast, compact code.
    4D - Reasonably fast, reasonably compact code.
 
------------------------------------------------------------------
 
Ported by:
    Lex-DRL
    I've ported the code from GLSL to CgFx/HLSL for Unity,
    added a couple more optimisations (to speed it up even further)
    and slightly reformatted the code to make it more readable.
 
Original GLSL functions:
    https://github.com/ashima/webgl-noise
    Credits from original glsl file are at the end of this cginc.
 
------------------------------------------------------------------
 
Usage:
 
    float ns = snoise(v);
    // v is any of: float2, float3, float4
 
    Return type is float.
    To generate 2 or more components of noise (colorful noise),
    call these functions several times with different
    constant offsets for the arguments.
    E.g.:
 
    float3 colorNs = float3(
        snoise(v),
        snoise(v + 17.0),
        snoise(v - 43.0),
    );
 
 
Remark about those offsets from the original author:
 
    People have different opinions on whether these offsets should be integers
    for the classic noise functions to match the spacing of the zeroes,
    so we have left that for you to decide for yourself.
    For most applications, the exact offsets don't really matter as long
    as they are not too small or too close to the noise lattice period
    (289 in this implementation).
 
*/

//                 Credits from source glsl file:
//
// Description : Array and textureless GLSL 2D/3D/4D simplex
//               noise functions.
//      Author : Ian McEwan, Ashima Arts.
//  Maintainer : ijm
//     Lastmod : 20110822 (ijm)
//     License : Copyright (C) 2011 Ashima Arts. All rights reserved.
//               Distributed under the MIT License. See LICENSE file.
//               https://github.com/ashima/webgl-noise
//
//
//           The text from LICENSE file:
//
//
// Copyright (C) 2011 by Ashima Arts (Simplex noise)
// Copyright (C) 2011 by Stefan Gustavson (Classic noise)
//
// Permission is hereby granted, free of charge, to any person obtaining a copy
// of this software and associated documentation files (the "Software"), to deal
// in the Software without restriction, including without limitation the rights
// to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
// copies of the Software, and to permit persons to whom the Software is
// furnished to do so, subject to the following conditions:
//
// The above copyright notice and this permission notice shall be included in
// all copies or substantial portions of the Software.
//
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
// IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
// FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
// AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
// LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
// OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN
// THE SOFTWARE.

//----------------------- snoise -------------------------

// 1 / 289
#define NOISE_SIMPLEX_1_DIV_289 0.00346020761245674740484429065744f

float mod289(float x) {
    return x - floor(x * NOISE_SIMPLEX_1_DIV_289) * 289.0;
}

float4 mod289(float4 x) {
    return x - floor(x * NOISE_SIMPLEX_1_DIV_289) * 289.0;
}

float permute(float x){
    return mod289(
        x*x*34.0 + x
    );
}

float4 permute(float4 x){
    return mod289(
        x*x*34.0 + x
    );
}
 
float4 taylorInvSqrt(float4 r)
{
    return 1.79284291400159 - 0.85373472095314 * r;
}
 
float4 grad4(float j, float4 ip)
{
    const float4 ones = float4(1.0, 1.0, 1.0, -1.0);
    float4 p, s;
    p.xyz = floor( frac(j * ip.xyz) * 7.0) * ip.z - 1.0;
    p.w = 1.5 - dot( abs(p.xyz), ones.xyz );
 
    // GLSL: lessThan(x, y) = x < y
    // HLSL: 1 - step(y, x) = x < y
    s = float4(
        1 - step(0.0, p)
    );
 
    // Optimization hint Dolkar
    // p.xyz = p.xyz + (s.xyz * 2 - 1) * s.www;
    p.xyz -= sign(p.xyz) * (p.w < 0);
 
    return p;
}

float snoise(float4 v)
{
    const float4 C = float4(
        0.138196601125011, // (5 - sqrt(5))/20 G4
        0.276393202250021, // 2 * G4
        0.414589803375032, // 3 * G4
     -0.447213595499958  // -1 + 4 * G4
    );
 
// First corner
    float4 i = floor(
        v +
        dot(
            v,
            0.309016994374947451 // (sqrt(5) - 1) / 4
        )
    );
    float4 x0 = v - i + dot(i, C.xxxx);
 
// Other corners
 
// Rank sorting originally contributed by Bill Licea-Kane, AMD (formerly ATI)
    float4 i0;
    float3 isX = step( x0.yzw, x0.xxx );
    float3 isYZ = step( x0.zww, x0.yyz );
    i0.x = isX.x + isX.y + isX.z;
    i0.yzw = 1.0 - isX;
    i0.y += isYZ.x + isYZ.y;
    i0.zw += 1.0 - isYZ.xy;
    i0.z += isYZ.z;
    i0.w += 1.0 - isYZ.z;
 
    // i0 now contains the unique values 0,1,2,3 in each channel
    float4 i3 = saturate(i0);
    float4 i2 = saturate(i0-1.0);
    float4 i1 = saturate(i0-2.0);
 
    //    x0 = x0 - 0.0 + 0.0 * C.xxxx
    //    x1 = x0 - i1  + 1.0 * C.xxxx
    //    x2 = x0 - i2  + 2.0 * C.xxxx
    //    x3 = x0 - i3  + 3.0 * C.xxxx
    //    x4 = x0 - 1.0 + 4.0 * C.xxxx
    float4 x1 = x0 - i1 + C.xxxx;
    float4 x2 = x0 - i2 + C.yyyy;
    float4 x3 = x0 - i3 + C.zzzz;
    float4 x4 = x0 + C.wwww;
 
// Permutations
    i = mod289(i);
    float j0 = permute(
        permute(
            permute(
                permute(i.w) + i.z
            ) + i.y
        ) + i.x
    );
    float4 j1 = permute(
        permute(
            permute(
                permute (
                    i.w + float4(i1.w, i2.w, i3.w, 1.0 )
                ) + i.z + float4(i1.z, i2.z, i3.z, 1.0 )
            ) + i.y + float4(i1.y, i2.y, i3.y, 1.0 )
        ) + i.x + float4(i1.x, i2.x, i3.x, 1.0 )
    );
 
// Gradients: 7x7x6 points over a cube, mapped onto a 4-cross polytope
// 7*7*6 = 294, which is close to the ring size 17*17 = 289.
    const float4 ip = float4(
        0.003401360544217687075, // 1/294
        0.020408163265306122449, // 1/49
        0.142857142857142857143, // 1/7
        0.0
    );
 
    float4 p0 = grad4(j0, ip);
    float4 p1 = grad4(j1.x, ip);
    float4 p2 = grad4(j1.y, ip);
    float4 p3 = grad4(j1.z, ip);
    float4 p4 = grad4(j1.w, ip);
 
// Normalise gradients
    float4 norm = taylorInvSqrt(float4(
        dot(p0, p0),
        dot(p1, p1),
        dot(p2, p2),
        dot(p3, p3)
    ));
    p0 *= norm.x;
    p1 *= norm.y;
    p2 *= norm.z;
    p3 *= norm.w;
    p4 *= taylorInvSqrt( dot(p4, p4) );
 
// Mix contributions from the five corners
    float3 m0 = max(
        0.6 - float3(
            dot(x0, x0),
            dot(x1, x1),
            dot(x2, x2)
        ),
        0.0
    );
    float2 m1 = max(
        0.6 - float2(
            dot(x3, x3),
            dot(x4, x4)
        ),
        0.0
    );
    m0 = m0 * m0;
    m1 = m1 * m1;
 
    return 49.0 * (
        dot(
            m0*m0,
            float3(
                dot(p0, x0),
                dot(p1, x1),
                dot(p2, x2)
            )
        ) + dot(
            m1*m1,
            float2(
                dot(p3, x3),
                dot(p4, x4)
            )
        )
    );
}

//----------------------- Simplex -------------------------

void SimplexNoise4D_float(float4 coordinates, out float Out)
{
    Out = snoise(coordinates);
}

// uses 4D simplex noise, using time as the 4th input component
void DynamicSimplexNoise3D_float(float3 coordinates, float time, out float Out)
{
    Out = snoise(float4(coordinates, time));
}

void SimplexNoise4D_half(half4 coordinates, out half Out)
{
    Out = snoise(coordinates);
}

// uses 4D simplex noise, using time as the 4th input component
void DynamicSimplexNoise3D_half(half3 coordinates, half time, out half Out)
{
    Out = snoise(half4(coordinates, time));
}

//----------------------- Settings -------------------------

// spark settings from https://alteredqualia.com/three/examples/webgl_shader_sparks.html

void DynamicSparkNoise_float(float3 coordinates, float time, out float Out)
{
    float4 coord = float4(coordinates, time);
    float n = 0.0;

    n += 1.0 * abs(snoise(coord));
    n += 0.5 * abs(snoise(coord * 2.0));
    n += 0.25 * abs(snoise(coord * 4.0));
    n += 0.125 * abs(snoise(coord * 8.0));

    const float rn = 1.0 - n;
    Out = rn * rn;
}

void DynamicSparkNoise_half(half3 coordinates, half time, out half Out)
{
    half4 coord = half4(coordinates, time);
    half n = 0.0;

    n += 1.0 * abs(snoise(coord));
    n += 0.5 * abs(snoise(coord * 2.0));
    n += 0.25 * abs(snoise(coord * 4.0));
    n += 0.125 * abs(snoise(coord * 8.0));

    const half rn = 1.0 - n;
    Out = rn * rn;
}

void DynamicLDSparkNoise_float(float3 coordinates, float time, out float Out)
{
    float4 coord = float4(coordinates, time);
    float n = 0.0;

    n += 1.0 * abs(snoise(coord));
    n += 0.5 * abs(snoise(coord * 2.0));
    n += 0.25 * abs(snoise(coord * 4.0));

    const float rn = 1.0 - n;
    Out = rn * rn;
}

void DynamicLDSparkNoise_half(half3 coordinates, half time, out half Out)
{
    half4 coord = half4(coordinates, time);
    half n = 0.0;

    n += 1.0 * abs(snoise(coord));
    n += 0.5 * abs(snoise(coord * 2.0));
    n += 0.25 * abs(snoise(coord * 4.0));

    const half rn = 1.0 - n;
    Out = rn * rn;
}

// amber settings from https://alteredqualia.com/three/examples/webgl_shader_amber.html

#endif