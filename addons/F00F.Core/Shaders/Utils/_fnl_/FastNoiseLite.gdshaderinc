// MIT License
//
// Copyright(c) 2023 Jordan Peck (jordan.me2@gmail.com)
// Copyright(c) 2023 Contributors
//
// Permission is hereby granted, free of charge, to any person obtaining a copy
// of this software and associated documentation files(the "Software"), to deal
// in the Software without restriction, including without limitation the rights
// to use, copy, modify, merge, publish, distribute, sublicense, and / or sell
// copies of the Software, and to permit persons to whom the Software is
// furnished to do so, subject to the following conditions :
//
// The above copyright notice and this permission notice shall be included in all
// copies or substantial portions of the Software.
//
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
// IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
// FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT.IN NO EVENT SHALL THE
// AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
// LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
// OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
// SOFTWARE.
//
// .'',;:cldxkO00KKXXNNWWWNNXKOkxdollcc::::::;:::ccllloooolllllllllooollc:,'...        ...........',;cldxkO000Okxdlc::;;;,,;;;::cclllllll
// ..',;:ldxO0KXXNNNNNNNNXXK0kxdolcc::::::;;;,,,,,,;;;;;;;;;;:::cclllllc:;'....       ...........',;:ldxO0KXXXK0Okxdolc::;;;;::cllodddddo
// ...',:loxO0KXNNNNNXXKK0Okxdolc::;::::::::;;;,,'''''.....''',;:clllllc:;,'............''''''''',;:loxO0KXNNNNNXK0Okxdollccccllodxxxxxxd
// ....';:ldkO0KXXXKK00Okxdolcc:;;;;;::cclllcc:;;,''..... ....',;clooddolcc:;;;;,,;;;;;::::;;;;;;:cloxk0KXNWWWWWWNXKK0Okxddoooddxxkkkkkxx
// .....';:ldxkOOOOOkxxdolcc:;;;,,,;;:cllooooolcc:;'...      ..,:codxkkkxddooollloooooooollcc:::::clodkO0KXNWWWWWWNNXK00Okxxxxxxxxkkkkxxx
// . ....';:cloddddo___________,,,,;;:clooddddoolc:,...      ..,:ldx__00OOOkkk___kkkkkkxxdollc::::cclodkO0KXXNNNNNNXXK0OOkxxxxxxxxxxxxddd
// .......',;:cccc:|           |,,,;;:cclooddddoll:;'..     ..';cox|  \KKK000|   |KK00OOkxdocc___;::clldxxkO0KKKKK00Okkxdddddddddddddddoo
// .......'',,,,,''|   ________|',,;;::cclloooooolc:;'......___:ldk|   \KK000|   |XKKK0Okxolc|   |;;::cclodxxkkkkxxdoolllcclllooodddooooo
// ''......''''....|   |  ....'',,,,;;;::cclloooollc:;,''.'|   |oxk|    \OOO0|   |KKK00Oxdoll|___|;;;;;::ccllllllcc::;;,,;;;:cclloooooooo
// ;;,''.......... |   |_____',,;;;____:___cllo________.___|   |___|     \xkk|   |KK_______ool___:::;________;;;_______...'',;;:ccclllloo
// c:;,''......... |         |:::/     '   |lo/        |           |      \dx|   |0/       \d|   |cc/        |'/       \......',,;;:ccllo
// ol:;,'..........|    _____|ll/    __    |o/   ______|____    ___|   |   \o|   |/   ___   \|   |o/   ______|/   ___   \ .......'',;:clo
// dlc;,...........|   |::clooo|    /  |   |x\___   \KXKKK0|   |dol|   |\   \|   |   |   |   |   |d\___   \..|   |  /   /       ....',:cl
// xoc;'...  .....'|   |llodddd|    \__|   |_____\   \KKK0O|   |lc:|   |'\       |   |___|   |   |_____\   \.|   |_/___/...      ...',;:c
// dlc;'... ....',;|   |oddddddo\          |          |Okkx|   |::;|   |..\      |\         /|   |          | \         |...    ....',;:c
// ol:,'.......',:c|___|xxxddollc\_____,___|_________/ddoll|___|,,,|___|...\_____|:\ ______/l|___|_________/...\________|'........',;::cc
// c:;'.......';:codxxkkkkxxolc::;::clodxkOO0OOkkxdollc::;;,,''''',,,,''''''''''',,'''''',;:loxkkOOkxol:;,'''',,;:ccllcc:;,'''''',;::ccll
// ;,'.......',:codxkOO0OOkxdlc:;,,;;:cldxxkkxxdolc:;;,,''.....'',;;:::;;,,,'''''........,;cldkO0KK0Okdoc::;;::cloodddoolc:;;;;;::ccllooo
// .........',;:lodxOO0000Okdoc:,,',,;:clloddoolc:;,''.......'',;:clooollc:;;,,''.......',:ldkOKXNNXX0Oxdolllloddxxxxxxdolccccccllooodddd
// .    .....';:cldxkO0000Okxol:;,''',,;::cccc:;,,'.......'',;:cldxxkkxxdolc:;;,'.......';coxOKXNWWWNXKOkxddddxxkkkkkkxdoollllooddxxxxkkk
//       ....',;:codxkO000OOxdoc:;,''',,,;;;;,''.......',,;:clodkO00000Okxolc::;,,''..',;:ldxOKXNWWWNNK0OkkkkkkkkkkkxxddooooodxxkOOOOO000
//       ....',;;clodxkkOOOkkdolc:;,,,,,,,,'..........,;:clodxkO0KKXKK0Okxdolcc::;;,,,;;:codkO0XXNNNNXKK0OOOOOkkkkxxdoollloodxkO0KKKXXXXX
//
// VERSION: 1.1.1
// https://github.com/Auburn/FastNoiseLite

// Switch between using floats or doubles for input position
#define FNLfloat float
//#define FNLfloat double

// Noise Type
#define FNL_NOISE_OPENSIMPLEX2 0
#define FNL_NOISE_OPENSIMPLEX2S 1
#define FNL_NOISE_CELLULAR 2
#define FNL_NOISE_PERLIN 3
#define FNL_NOISE_VALUE_CUBIC 4
#define FNL_NOISE_VALUE 5
#define fnl_noise_type int

// Rotation types
#define FNL_ROTATION_NONE 0
#define FNL_ROTATION_IMPROVE_XY_PLANES 1
#define FNL_ROTATION_IMPROVE_XZ_PLANES 2
#define fnl_rotation_type_3d int

// Fractal types
#define FNL_FRACTAL_NONE 0
#define FNL_FRACTAL_FBM 1
#define FNL_FRACTAL_RIDGED 2
#define FNL_FRACTAL_PINGPONG 3
#define FNL_FRACTAL_DOMAIN_WARP_PROGRESSIVE 4
#define FNL_FRACTAL_DOMAIN_WARP_INDEPENDENT 5
#define fnl_fractal_type int

#define FNL_CELLULAR_DISTANCE_EUCLIDEAN 0
#define FNL_CELLULAR_DISTANCE_EUCLIDEANSQ 1
#define FNL_CELLULAR_DISTANCE_MANHATTAN 2
#define FNL_CELLULAR_DISTANCE_HYBRID 3
#define fnl_cellular_distance_func int

#define FNL_CELLULAR_RETURN_TYPE_CELLVALUE 0
#define FNL_CELLULAR_RETURN_TYPE_DISTANCE 1
#define FNL_CELLULAR_RETURN_TYPE_DISTANCE2 2
#define FNL_CELLULAR_RETURN_TYPE_DISTANCE2ADD 3
#define FNL_CELLULAR_RETURN_TYPE_DISTANCE2SUB 4
#define FNL_CELLULAR_RETURN_TYPE_DISTANCE2MUL 5
#define FNL_CELLULAR_RETURN_TYPE_DISTANCE2DIV 6
#define fnl_cellular_return_type int

#define FNL_DOMAIN_WARP_OPENSIMPLEX2 0
#define FNL_DOMAIN_WARP_OPENSIMPLEX2_REDUCED 1
#define FNL_DOMAIN_WARP_BASICGRID 2
#define fnl_domain_warp_type int

// Structure containing entire noise system state.
// @note Must only be created using fnlCreateState(optional: seed). To ensure defaults are set.

struct fnl_state
{
    // Seed used for all noise types.
    // @remark Default: 1337
    int seed;

    // The frequency for all noise types.
    // @remark Default: 0.01
    float frequency;

    // The noise algorithm to be used by GetNoise(...).
    // @remark Default: FNL_NOISE_OPENSIMPLEX2
    fnl_noise_type noise_type;

    // Sets noise rotation type for 3D.
    // @remark Default: FNL_ROTATION_NONE
    fnl_rotation_type_3d rotation_type_3d;

    // The method used for combining octaves for all fractal noise types.
    // @remark Default: None
    // @remark FNL_FRACTAL_DOMAIN_WARP_... only effects fnlDomainWarp...
    fnl_fractal_type fractal_type;

    // The octave count for all fractal noise types.
    // @remark Default: 3
    int octaves;

    // The octave lacunarity for all fractal noise types.
    // @remark Default: 2.0
    float lacunarity;

    // The octave gain for all fractal noise types.
    // @remark Default: 0.5
    float gain;

    // The octave weighting for all none Domaain Warp fractal types.
    // @remark Default: 0.0
    // @remark 
    float weighted_strength;

    // The strength of the fractal ping pong effect.
    // @remark Default: 2.0
    float ping_pong_strength;

    // The distance function used in cellular noise calculations.
    // @remark Default: FNL_CELLULAR_DISTANCE_EUCLIDEANSQ
    fnl_cellular_distance_func cellular_distance_func;

    // The cellular return type from cellular noise calculations.
    // @remark Default: FNL_CELLULAR_RETURN_TYPE_DISTANCE
    fnl_cellular_return_type cellular_return_type;

    // The maximum distance a cellular point can move from it's grid position.
    // @remark Default: 1.0
    // @note Setting this higher than 1 will cause artifacts.
    float cellular_jitter_mod;

    // The warp algorithm when using fnlDomainWarp...
    // @remark Default: OpenSimplex2
    fnl_domain_warp_type domain_warp_type;

    // The maximum warp distance from original position when using fnlDomainWarp...
    // @remark Default: 1.0
    float domain_warp_amp;
};

// ====================
// PRIVATE
// ====================

float _fnlFastMin(float x, float y) { return min(x, y); }

float _fnlFastMax(float x, float y) { return max(x, y); }

float _fnlFastAbs(float f) { return abs(f); }

float _fnlFastSqrt(float a) { return sqrt(a); }

int _fnlFastFloor(FNLfloat f) { return int(floor(f)); }

int _fnlFastRound(FNLfloat f) { return int(round(f)); }

float _fnlLerp(float a, float b, float t) { return mix(a, b, t); }

float _fnlInterpHermite(float t) { return t * t * (3.f - 2.f * t); }

float _fnlInterpQuintic(float t) { return t * t * t * (t * (t * 6.f - 15.f) + 10.f); }

float _fnlCubicLerp(float a, float b, float c, float d, float t)
{
    float p = (d - c) - (a - b);
    return t * t * t * p + t * t * ((a - b) - p) + t * (c - a) + b;
}

#ifdef FNL_USE_PINGPONG
float _fnlPingPong(float t)
{
    t -= float(int(t * 0.5f)) * 2.f;
    return t < 1.f ? t : 2.f - t;
}
#endif

#if defined(FNL_USE_FBM) || defined(FNL_USE_RIDGED) || defined(FNL_USE_PINGPONG) || defined(FNL_USE_DOMAINWARP_BASICGRID) || defined(FNL_USE_DOMAINWARP_SIMPLEX2) || defined(FNL_USE_DOMAINWARP_SIMPLEX2S)
float _fnlCalculateFractalBounding(fnl_state state)
{
    float gain = _fnlFastAbs(state.gain);
    float amp = gain;
    float ampFractal = 1.f;
    for (int i = 1; i < state.octaves; i++)
    {
        ampFractal += amp;
        amp *= gain;
    }
    return 1.f / ampFractal;
}
#endif

const int PRIME_X = 501125321;
const int PRIME_Y = 1136930381;
const int PRIME_Z = 1720413743;

#ifdef FNL_USE_3D
#include "FastNoiseLite.3D.gdshaderinc"
#endif
#if defined(FNL_USE_2D) || !defined(FNL_USE_3D)
#include "FastNoiseLite.2D.gdshaderinc"
#endif

// ====================
// Public API
// ====================

fnl_state fnlCreateState(int seed)
{
    fnl_state newState;
    newState.seed = seed;
    newState.frequency = 0.01f;
    newState.noise_type = FNL_NOISE_OPENSIMPLEX2;
    newState.rotation_type_3d = FNL_ROTATION_NONE;
    newState.fractal_type = FNL_FRACTAL_NONE;
    newState.octaves = 3;
    newState.lacunarity = 2.f;
    newState.gain = 0.5f;
    newState.weighted_strength = 0.f;
    newState.ping_pong_strength = 2.f;
    newState.cellular_distance_func = FNL_CELLULAR_DISTANCE_EUCLIDEANSQ;
    newState.cellular_return_type = FNL_CELLULAR_RETURN_TYPE_DISTANCE;
    newState.cellular_jitter_mod = 1.f;
    newState.domain_warp_amp = 30.f;
    newState.domain_warp_type = FNL_DOMAIN_WARP_OPENSIMPLEX2;
    return newState;
}

