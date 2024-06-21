shader_type spatial;
render_mode blend_mix, depth_draw_opaque, cull_back, diffuse_burley, specular_schlick_ggx;


// ====================
// Uniforms
// ====================

uniform int seed;
uniform vec2 offset;
uniform float scale : hint_range(0.1, 10.0) = 1.0;

group_uniforms Thresholds;
uniform float road_threshold : hint_range(0.0, 5.0) = 0.05;
uniform float berm_threshold : hint_range(0.0, 5.0) = 0.15;

group_uniforms Heights;
uniform float road_height : hint_range(0.0, 5.0)  = 0.0;
uniform float berm_height : hint_range(0.0, 5.0)  = 0.5;
uniform float base_height : hint_range(0.0, 50.0) = 15.0;

group_uniforms Colors;
//uniform vec3 road_color : source_color = vec3(0.77f, 0.64f, 0.35f);	// #C4A35A	Bright warm sandstone — worn road surface
//uniform vec3 berm_color : source_color = vec3(0.55f, 0.48f, 0.32f);	// #8C7A52	Muted stone — raised walkways/curbs
//uniform vec3 base_color : source_color = vec3(0.42f, 0.26f, 0.15f);	// #6B4226	Dark clay — mudbrick structures

uniform vec3 road_color : source_color = vec3(1.0, 0.0, 0.0);	// R
uniform vec3 berm_color : source_color = vec3(0.0, 1.0, 0.0);	// G
uniform vec3 base_color : source_color = vec3(0.0, 0.0, 1.0);	// B

group_uniforms Textures;
uniform sampler2DArray albedo_array : source_color, filter_linear_mipmap_anisotropic, repeat_enable;
uniform sampler2DArray normal_array : hint_roughness_normal, filter_linear_mipmap_anisotropic, repeat_enable;
uniform sampler2DArray detail_array : hint_default_white, filter_linear_mipmap_anisotropic, repeat_enable;

group_uniforms Tuning;
uniform float texture_scale   : hint_range(0.0, 10)  = 0.2;
uniform float depth_strength  : hint_range(0.0, 0.1) = 0.03;
uniform float normal_strength : hint_range(0.0, 2.0) = 1.0;
uniform float min_detail_dist : hint_range(0.0, 5.0) = 0.5;
uniform float max_detail_dist : hint_range(5.0, 50)  = 25.0;


// ====================
// Constants
// ====================

const float road_idx = 0.0;
const float berm_idx = 1.0;
const float base_idx = 2.0;

struct City {
    float is_road;
    float is_berm;
    float is_base;
    float height;
    float idx;
};

struct PBR {
    vec3 ALBEDO;
    vec3 NORMAL_MAP;
    float NORMAL_MAP_DEPTH;

    float METALLIC;
    float ROUGHNESS;
    float AO;
};


// ====================
// Utilities
// ====================

#include "res://addons/F00F.Core/Shaders/Utils/Utils.gdshaderinc"
#include "res://addons/F00F.Core/Shaders/Utils/Utils.Hash.gdshaderinc"
#include "res://addons/F00F.Core/Shaders/Utils/Utils.Texture.gdshaderinc"
#include "res://addons/F00F.Core/Shaders/Utils/fnl/fnl.voronoi.gdshaderinc"


// ====================
// Functions
// ====================

City GetCityHeight(vec2 p) {
    p = (p + offset) * scale;

    voronoi v = fnlVoronoi2D(Voronoi(seed), p.x, p.y);

    float edge = 1.0 + v.d_sub; // [0..1]
    float is_above_road = step(road_threshold, edge);
    float is_above_berm = step(berm_threshold, edge);

    float is_road = 1.0 - is_above_road;
    float is_berm = is_above_road - is_above_berm;
    float is_base = is_above_berm;
    return c.is_road * road_height
         + c.is_berm * berm_height
         + c.is_base * base_height;
}

City GetCityState(vec2 p) {
    p = (p + offset) * scale;

    voronoi v = fnlVoronoi2D(Voronoi(seed), p.x, p.y);

    float edge = 1.0 + v.d_sub; // [0..1]
    float is_above_road = step(road_threshold, edge);
    float is_above_berm = step(berm_threshold, edge);

    City c;
    c.is_road = 1.0 - is_above_road;
    c.is_berm = is_above_road - is_above_berm;
    c.is_base = is_above_berm;
    c.height  = c.is_road * road_height
              + c.is_berm * berm_height
              + c.is_base * base_height;
    c.idx     = c.is_road * road_idx
              + c.is_berm * berm_idx
              + c.is_base * base_idx;
    return c;
}

PBR DrawCity() {

}

float CityHeight(vec2 world_pos) {
    p = (p + offset) * scale;

    voronoi v = fnlVoronoi2D(Voronoi(seed), p.x, p.y);

    float edge = 1.0 + v.d_sub; // [0..1]
    float is_above_road = step(road_threshold, edge);
    float is_above_berm = step(berm_threshold, edge);

    float is_road = 1.0 - is_above_road;
    float is_berm = is_above_road - is_above_berm;
    float is_base = is_above_berm;

    return c.is_road * road_height
         + c.is_berm * berm_height
         + c.is_base * base_height;
}

PBR CityState(vec2 world_pos, float view_dist, vec3 vertex) {
    p = (p + offset) * scale;

    voronoi v = fnlVoronoi2D(Voronoi(seed), p.x, p.y);

    float edge = 1.0 + v.d_sub; // [0..1]
    float is_above_road = step(road_threshold, edge);
    float is_above_berm = step(berm_threshold, edge);

    float is_road = 1.0 - is_above_road;
    float is_berm = is_above_road - is_above_berm;
    float is_base = is_above_berm;

    return c.is_road * road_height
         + c.is_berm * berm_height
         + c.is_base * base_height;
}

// ====================
// MAIN
// ====================

varying vec3 view_pos;
varying vec3 world_pos;

void vertex() {
    view_pos = ViewPos(VIEW_MATRIX, VERTEX);
    world_pos = WorldPos(MODEL_MATRIX, VERTEX);
    VERTEX.y = CityHeight(world_pos.xz);
}

void fragment() {
    PBR city = CityState(world_pos.xz, length(view_pos), VERTEX);

    ALBEDO           = city.ALBEDO;
    NORMAL           = city.NORMAL;
    NORMAL_MAP       = city.NORMAL_MAP;
    NORMAL_MAP_DEPTH = city.NORMAL_MAP_DEPTH;
    AO               = city.AO;
    METALLIC         = city.METALLIC;
    ROUGHNESS        = city.ROUGHNESS;
}
 
    vec3 blend = pow(abs(NORMAL), vec3(4.0));
    blend /= max(blend.x + blend.y + blend.z, epsilon);

    float dist = length(view_pos);
    float fade = smoothstep(min_detail_dist, min_detail_dist + 0.5, dist)
        * (1.0 - smoothstep(max_detail_dist - 5.0, max_detail_dist, dist));

    float height = sample_triplanar_texture(detail_array, world_pos, blend, c.idx).a;
    vec4 albedo = sample_parallax_triplanar_texture(albedo_array, height, world_pos, blend, c.idx);
    vec4 normal = sample_parallax_triplanar_texture(normal_array, height, world_pos, blend, c.idx);
    vec4 detail = sample_parallax_triplanar_texture(detail_array, height, world_pos, blend, c.idx);

    ALBEDO           = albedo.rgb;
    NORMAL_MAP       = normal.rgb;
    NORMAL_MAP_DEPTH = normal_strength * fade;
    AO               = detail.r;
    ROUGHNESS        = detail.g;
    METALLIC         = detail.b;


    // If color
    //ALBEDO = c.is_road * road_color
    //       + c.is_berm * berm_color
    //       + c.is_base * base_color;
    //NORMAL = normalize(cross(dFdx(VERTEX), dFdy(VERTEX)));
    //ROUGHNESS = mix(0.4, 0.8, c.is_base);
    //METALLIC = 0.0;
}
