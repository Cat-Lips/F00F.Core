#ifndef _F00F_UTILS_HASH_INC_
#define _F00F_UTILS_HASH_INC_

//
// Usage:
//   #include "res://addons/F00F.Core/Shaders/Utils/Utils.Hash.gdshaderinc"
//

#define _hashf(p, s) fract(sin((p) * s) * 43758.5453123)
#define _hashv(p, s) fract(sin(dot(p, s)) * 43758.5453123)

#define Hashf(p) _hashf(p, 127.1)
#define Hash1f(p) _hashf(p, 269.5)
#define Hash2f(p) _hashf(p, 419.2)

#define Hashv(p) _hashv(p, vec2(127.1, 311.7))
#define Hash1v(p) _hashv(p, vec2(269.5, 183.3))
#define Hash2v(p) _hashv(p, vec2(419.2, 371.9))

#endif
