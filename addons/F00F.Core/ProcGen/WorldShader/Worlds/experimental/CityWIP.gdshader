shader_type spatial;
render_mode blend_mix, depth_draw_opaque, cull_back, diffuse_burley, specular_schlick_ggx;

uniform int seed;
uniform float scale : hint_range(0.001, 1.0) = 0.05; // Lower values fit world units better

group_uniforms Thresholds;
uniform float road_threshold : hint_range(0.0, 1.0) = 0.05;
uniform float berm_threshold : hint_range(0.0, 1.0) = 0.15;

group_uniforms Heights;
uniform float road_height : hint_range(0.0, 5.0) = 0.0;
uniform float berm_height : hint_range(0.0, 5.0) = 0.5;
uniform float base_height : hint_range(0.0, 50.0) = 15.0;

group_uniforms Colors;
uniform vec3 road_color : source_color = vec3(1.0, 0.0, 0.0); // R
uniform vec3 berm_color : source_color = vec3(0.0, 1.0, 0.0); // G
uniform vec3 base_color : source_color = vec3(0.0, 0.0, 1.0); // B

struct City {
    float road;
    float berm;
    float base;
    float height;
};

///////////
// UTILS //
///////////
#include "res://addons/F00F.Core/Shaders/Utils/Utils.gdshaderinc"
#include "res://addons/F00F.Core/Shaders/Utils/Utils.Hash.gdshaderinc"
#include "res://addons/F00F.Core/Shaders/Utils/fnl/fnl.voronoi.gdshaderinc"

///////////
// FUNCS //
///////////
City GetCity(vec2 p) {
    // Process coordinates purely based on global positioning scale
    p = p * scale;

    voronoi v = fnlVoronoi2D(Voronoi(seed), p.x, p.y);
    float edge = 1.0 + v.d_sub; // Realigned [0..1] range from include file

    // Sharp binary transitions using step() to force vertical walls
    float is_above_road = step(road_threshold, edge);
    float is_above_berm = step(berm_threshold, edge);

    City c;
    // Mutually exclusive masks (values are strictly 0.0 or 1.0)
    c.road = 1.0 - is_above_road;
    c.berm = is_above_road - is_above_berm;
    c.base = is_above_berm;

    c.height = (c.road * road_height)
             + (c.berm * berm_height)
             + (c.base * base_height);
    return c;
}

//////////
// MAIN //
//////////
varying vec2 world_pos;

void vertex() {
    // Rely strictly on your include's WorldPos function for endless chunk streaming
    world_pos = WorldPos(MODEL_MATRIX, VERTEX).xz;

    // Assign vertical displacement directly
    VERTEX.y = GetCity(world_pos).height;
}

void fragment() {
    City c = GetCity(world_pos);

    // Apply clean procedural colors
    ALBEDO = (c.road * road_color)
           + (c.berm * berm_color)
           + (c.base * base_color);

    // Keep your screenspace derivative normal generation for clean flat-shaded faces
    vec3 dx = dFdx(VERTEX);
    vec3 dy = dFdy(VERTEX);
    NORMAL = normalize(cross(dx, dy));
    
    // Add physical material traits
    ROUGHNESS = mix(0.4, 0.8, c.base); // Buildings are matte, roads are slightly slicker
    METALLIC = 0.0;
}
