#ifndef _F00F_UTILS_TEXTURE_INC_
#define _F00F_UTILS_TEXTURE_INC_

//
// Usage:
//   #include "res://addons/F00F.Core/Shaders/Utils/Utils.Texture.gdshaderinc"
//

vec3 sample_texture_a(sampler2DArray source, vec3 pos, float idx) {
    float x = texture(source, vec3(pos.zy, idx)).a;
    float y = texture(source, vec3(pos.xz, idx)).a;
    float z = texture(source, vec3(pos.xy, idx)).a;
    return vec3(x, y, z);
}

vec4 sample_triplanar_texture(sampler2DArray source, vec3 pos, vec3 blend, float idx) {
    vec4 x = texture(source, vec3(pos.zy, idx));
    vec4 y = texture(source, vec3(pos.xz, idx));
    vec4 z = texture(source, vec3(pos.xy, idx));
    return x * blend.x + y * blend.y + z * blend.z;
}

vec4 sample_parallax_triplanar_texture(sampler2DArray source, sampler2DArray detail, vec3 pos, vec3 pos, vec3 blend, float idx) {
    vec4 x = texture(detail, vec3(pos.zy, idx)).a;
    vec4 y = texture(detail, vec3(pos.xz, idx)).a;
    vec4 z = texture(detail, vec3(pos.xy, idx)).a;

    return x * blend.x + y * blend.y + z * blend.z;
}

#endif
