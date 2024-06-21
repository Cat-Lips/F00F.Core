//
// Usage:
//   [#define {2D/3D}]
//   #define {NOISE TYPE}
//   [#define {FRACTAL TYPE}]
//   [#define {DOMAIN WARP TYPE}]
//   [#define {DOMAIN WARP STYLE}]
//   #include "res://addons/F00F.Core/Shaders/Utils/ShaderNoise.gdshaderinc"
//
// 2D/3D:
//  FNL_USE_3D
//  FNL_USE_2D (default)
//
// NOISE TYPES:
//  FNL_USE_VALUE
//  FNL_USE_PERLIN
//  FNL_USE_CELLULAR
//  FNL_USE_SIMPLEX2
//  FNL_USE_SIMPLEX2S
//  FNL_USE_VALUECUBIC
//
// FRACTAL TYPES:
//  FNL_USE_FBM
//  FNL_USE_RIDGED
//  FNL_USE_PINGPONG
//
// DOMAIN WARP TYPES:
//  FNL_USE_DOMAINWARP_SIMPLEX2
//  FNL_USE_DOMAINWARP_SIMPLEX2S
//  FNL_USE_DOMAINWARP_BASICGRID
//
// DOMAIN WARP STYLES:
//  FNL_USE_DOMAINWARP_PROGRESSIVE
//  FNL_USE_DOMAINWARP_INDEPENDENT
//

#include "_fnl_/FastNoiseLite.gdshaderinc"

uniform int     Seed;
uniform float   Frequency : hint_range(0,1) = 0.01;
uniform int     NoiseType : hint_enum("Simplex2", "Simplex2S", "Cellular", "Perlin", "ValueCubic", "Value");

#if defined(FNL_USE_FBM) || defined(FNL_USE_RIDGED) || defined(FNL_USE_PINGPONG)
group_uniforms  Fractal;
uniform int     FractalType : hint_enum("None", "Fbm", "Ridged", "PingPong");
uniform int     FractalOctaves = 3;
uniform float   FractalLacunarity = 2.0;
uniform float   FractalGain = 0.5;
uniform float   FractalWeightedStrength = 0.0;
#ifdef FNL_USE_PINGPONG
uniform float   FractalPingPongStrength = 2.0;
#endif
#endif

#if defined(FNL_USE_DOMAINWARP_BASICGRID) || defined(FNL_USE_DOMAINWARP_SIMPLEX2) || defined(FNL_USE_DOMAINWARP_SIMPLEX2S)
group_uniforms  DomainWarp;
uniform int     DomainWarpType : hint_enum("None:-1", "Simplex2", "Simplex2S", "BasicGrid") = -1;
#if defined(FNL_USE_DOMAINWARP_PROGRESSIVE) || defined(FNL_USE_DOMAINWARP_INDEPENDENT)
uniform int     DomainWarpStyle : hint_enum("None:0", "Progressive:4", "Independent:5");
#endif
uniform float   DomainWarpAmplitude = 1.0;
uniform float   DomainWarpFrequency : hint_range(0,1) = 0.01;
uniform int     DomainWarpOctaves = 3;
#if defined(FNL_USE_DOMAINWARP_PROGRESSIVE) || defined(FNL_USE_DOMAINWARP_INDEPENDENT)
uniform float   DomainWarpLacunarity = 2.0;
#endif
uniform float   DomainWarpGain = 0.5;
#endif

#ifdef FNL_USE_CELLULAR
group_uniforms  Cellular;
uniform float   CellularJitter : hint_range(0,1) = 1.0;
uniform int     CellularReturnType : hint_enum("CellValue", "Distance", "Distance2", "Distance2Add", "Distance2Sub", "Distance2Mul", "Distance2Div") = FNL_CELLULAR_RETURN_TYPE_DISTANCE;
uniform int     CellularDistanceFunction : hint_enum("Euclidean", "EuclideanSquared", "Manhattan", "Hybrid") = FNL_CELLULAR_DISTANCE_EUCLIDEANSQ;
#endif

group_uniforms Sampling;
#if defined(FNL_USE_2D) || !defined(FNL_USE_3D)
uniform vec2    Offset;
#endif
#ifdef FNL_USE_3D
uniform vec3    Offset3D;
uniform int     RotationType3D : hint_enum("None", "ImproveXYPlanes", "ImproveXZPlanes");
#endif

fnl_state FNL()
{
    fnl_state fnl = fnlCreateState(Seed);
    fnl.frequency = Frequency;
    fnl.noise_type = NoiseType;
#if defined(FNL_USE_FBM) || defined(FNL_USE_RIDGED) || defined(FNL_USE_PINGPONG)
    fnl.fractal_type = FractalType;
    fnl.octaves = FractalOctaves;
    fnl.lacunarity = FractalLacunarity;
    fnl.gain = FractalGain;
    fnl.weighted_strength = FractalWeightedStrength;
#ifdef FNL_USE_PINGPONG
    fnl.ping_pong_strength = FractalPingPongStrength;
#endif
#endif
#ifdef FNL_USE_CELLULAR
    fnl.cellular_jitter_mod = CellularJitter;
    fnl.cellular_return_type = CellularReturnType;
    fnl.cellular_distance_func = CellularDistanceFunction;
#endif
#ifdef FNL_USE_3D
    fnl.rotation_type_3d = RotationType3D;
#endif
    return fnl;
}

#if defined(FNL_USE_DOMAINWARP_BASICGRID) || defined(FNL_USE_DOMAINWARP_SIMPLEX2) || defined(FNL_USE_DOMAINWARP_SIMPLEX2S)
fnl_state WARP()
{
    fnl_state fnl = fnlCreateState(Seed);
    fnl.domain_warp_type = DomainWarpType;
#if defined(FNL_USE_DOMAINWARP_PROGRESSIVE) || defined(FNL_USE_DOMAINWARP_INDEPENDENT)
    fnl.fractal_type = DomainWarpStyle;
#endif
    fnl.domain_warp_amp = DomainWarpAmplitude;
    fnl.frequency = DomainWarpFrequency;
    fnl.octaves = DomainWarpOctaves;
#if defined(FNL_USE_DOMAINWARP_PROGRESSIVE) || defined(FNL_USE_DOMAINWARP_INDEPENDENT)
    fnl.lacunarity = DomainWarpLacunarity;
#endif
    fnl.gain = DomainWarpGain;
#ifdef FNL_USE_3D
    fnl.rotation_type_3d = RotationType3D;
#endif
    return fnl;
}
#endif

// ====================
// Public API
// ====================

#ifdef FNL_USE_3D
float GetNoise3D(vec3 p) {
    p += Offset3D;
#if defined(FNL_USE_DOMAINWARP_BASICGRID) || defined(FNL_USE_DOMAINWARP_SIMPLEX2) || defined(FNL_USE_DOMAINWARP_SIMPLEX2S)
    if (DomainWarpType >= 0) {
        fnlDomainWarp3D(WARP(), p.x, p.y, p.z);
    }
#endif
    return fnlGetNoise3D(FNL(), p.x, p.y, p.z);
}
#endif

#if defined(FNL_USE_2D) || !defined(FNL_USE_3D)
float GetNoise2D(vec2 p) {
    p += Offset;
#if defined(FNL_USE_DOMAINWARP_BASICGRID) || defined(FNL_USE_DOMAINWARP_SIMPLEX2) || defined(FNL_USE_DOMAINWARP_SIMPLEX2S)
    if (DomainWarpType >= 0) {
        fnlDomainWarp2D(WARP(), p.x, p.y);
    }
#endif
    return fnlGetNoise2D(FNL(), p.x, p.y);
}
#endif
