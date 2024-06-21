#ifndef _FNL_VORONOI_INC_
#define _FNL_VORONOI_INC_

#include "fnl.utils.gdshaderinc"

//
// Usage:
//  [#define FNL_VORONOI_EUCLIDEAN | FNL_VORONOI_EUCLIDEANSQ (default) | FNL_VORONOI_MANHATTAN | FNL_VORONOI_HYBRID]
//  #include "res://addons/F00F.Core/Shaders/Utils/fnl/fnl.voronoi.gdshaderinc"
//
//  voronoi v = fnlVoronoi2D(Voronoi(seed), x, y);
//  voronoi v = fnlVoronoi2D(_voronoi(seed, freq, jitter), x, y);
//

struct _voronoi
{
    int seed;
    float freq;
    float jitter;
};

struct voronoi
{
    float cell;
    float dist1;
    float dist2;
    float d_add;
    float d_sub;
    float d_mul;
    float d_div;
};

#define Voronoi(seed) _voronoi(seed, 0.01, 1.0)

// ====================
// PRIVATE
// ====================

voronoi _fnlVoronoi2D(_voronoi _v, float x, float y)
{
    int xr = _fnlFastRound(x);
    int yr = _fnlFastRound(y);

    float distance0 = 1e10f;
    float distance1 = 1e10f;
    int closestHash = 0;

    float _jitter = _v.jitter * 0.43701595f;

    int xPrimed = (xr - 1) * PRIME_X;
    int yPrimedBase = (yr - 1) * PRIME_Y;
    for (int xi = xr - 1; xi <= xr + 1; xi++)
    {
        int yPrimed = yPrimedBase;
        for (int yi = yr - 1; yi <= yr + 1; yi++)
        {
            int hash = _fnlHash2D(_v.seed, xPrimed, yPrimed);
            int idx = hash & (255 << 1);

            float vecX = float(xi) - x + RAND_VECS_2D[idx] * _jitter;
            float vecY = float(yi) - y + RAND_VECS_2D[idx | 1] * _jitter;

#if defined(FNL_VORONOI_MANHATTAN)
            float newDistance = _fnlFastAbs(vecX) + _fnlFastAbs(vecY);
#elif defined(FNL_VORONOI_HYBRID)
            float newDistance = (_fnlFastAbs(vecX) + _fnlFastAbs(vecY)) + (vecX * vecX + vecY * vecY);
#else
            float newDistance = vecX * vecX + vecY * vecY;
#endif
            distance1 = _fnlFastMax(_fnlFastMin(distance1, newDistance), distance0);
            if (newDistance < distance0)
            {
                distance0 = newDistance;
                closestHash = hash;
            }
            yPrimed += PRIME_Y;
        }
        xPrimed += PRIME_X;
    }
#ifdef FNL_VORONOI_EUCLIDEAN
    distance0 = _fnlFastSqrt(distance0);
    distance1 = _fnlFastSqrt(distance1);
#endif
    voronoi v;
    v.cell = float(closestHash) * (1.f / 2147483648.f);
    v.dist1 = distance0 - 1.f;
    v.dist2 = distance1 - 1.f;
    v.d_add = (distance1 + distance0) * 0.5f - 1.f;
    v.d_sub = distance1 - distance0 - 1.f;
    v.d_mul = distance1 * distance0 * 0.5f - 1.f;
    v.d_div = distance0 / distance1 - 1.f;
    return v;
}


// ====================
// MAIN
// ====================

voronoi fnlVoronoi2D(_voronoi v, float x, float y)
{
    x *= v.freq; y *= v.freq;
    return _fnlVoronoi2D(v, x, y);
}

#endif
