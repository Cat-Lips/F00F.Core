shader_type spatial;

render_mode cull_back, depth_draw_opaque, diffuse_burley, specular_schlick_ggx;

uniform sampler2D height_map:source_color;
uniform sampler2D color_map:source_color;
uniform sampler2D gradient:source_color;
uniform float amplitude;

varying float raw_height;

/////////////////////
// -- FUNCTIONS -- //
/////////////////////

vec2 texture_size(sampler2D tex) {
    return vec2(textureSize(tex, 0));
}

vec4 texture_value(sampler2D tex, vec2 pos) {
    vec2 size = texture_size(tex);
    pos = (pos * (size - 1.0) + 0.5) / size;
    return texture(tex, pos);
}

float RawHeight(vec2 uv) {
    return texture_value(height_map, uv).r;
}

vec3 GetColor(float h) {
    return texture_value(color_map, vec2(h, 0.5)).rgb;
}

float GetGradient(float h) {
    return texture_value(gradient, vec2(h, 0.5)).r;
}

float RealHeight(float h) {
    return h * GetGradient(h) * amplitude;
}

vec3 GetNormal(vec2 uv, float h) {
    vec2 size = texture_size(height_map);
    float tx = 1.0 / size.x;
    float ty = 1.0 / size.y;

    float hL = RealHeight(RawHeight(uv + vec2(-tx, 0.0)));
    float hR = RealHeight(RawHeight(uv + vec2( tx, 0.0)));
    float hD = RealHeight(RawHeight(uv + vec2(0.0, -ty)));
    float hU = RealHeight(RawHeight(uv + vec2(0.0,  ty)));

    vec3 dx = vec3(2.0 * tx, hR - hL, 0.0);
    vec3 dz = vec3(0.0, hU - hD, 2.0 * ty);
    return normalize(cross(dz, dx));
}

///////////////////
// -- SHADERS -- //
///////////////////

void vertex() {
    raw_height = RawHeight(UV);
    VERTEX.y = RealHeight(raw_height);
}

void fragment() {
    ALBEDO = GetColor(raw_height);
    NORMAL = GetNormal(UV, raw_height);
}
