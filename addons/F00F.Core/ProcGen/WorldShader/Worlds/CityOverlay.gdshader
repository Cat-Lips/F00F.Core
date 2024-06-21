shader_type spatial;
render_mode blend_mix, depth_draw_opaque, cull_back, diffuse_burley, specular_schlick_ggx;

uniform int seed;
uniform vec2 offset;
uniform float scale : hint_range(0.1, 10.0) = 1.0;

group_uniforms Thresholds;
uniform float road_threshold : hint_range(0.0, 1.0) = 0.1;
uniform float berm_threshold : hint_range(0.0, 1.0) = 0.3;
uniform float base_threshold : hint_range(0.0, 1.0) = 0.5;

group_uniforms Heights;
uniform float road_height : hint_range(0.0, 5.0) = 0.0;
uniform float berm_height : hint_range(0.0, 5.0) = 5.0;
uniform float base_height : hint_range(0.0, 50.0) = 50.0;

group_uniforms Colors;
//uniform vec3 road_color : source_color = vec3(0.77f, 0.64f, 0.35f);	// #C4A35A	Bright warm sandstone — worn road surface
//uniform vec3 berm_color : source_color = vec3(0.55f, 0.48f, 0.32f);	// #8C7A52	Muted stone — raised walkways/curbs
//uniform vec3 base_color : source_color = vec3(0.42f, 0.26f, 0.15f);	// #6B4226	Dark clay — mudbrick structures

uniform vec3 road_color : source_color = vec3(1.0, 0.0, 0.0);	// R
uniform vec3 berm_color : source_color = vec3(0.0, 1.0, 0.0);	// G
uniform vec3 base_color : source_color = vec3(0.0, 0.0, 1.0);	// B


struct City {
    float road;
    float berm;
    float base;
    float tone;
    float height;
    float building;
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
    p = (p + offset) * scale;

    voronoi v = fnlVoronoi2D(Voronoi(seed), p.x, p.y);

    float edge = 1.0 + v.d_sub; // [0..1]
    float grad = smoothstep(road_threshold, berm_threshold, edge);
    float base = smoothstep(berm_threshold, base_threshold, edge);
    float berm = grad - base;
    float road = 1.0 - grad;

    City c;
    c.road = road;
    c.berm = berm;
    c.base = base;
    c.height = road * road_height
             + berm * berm_height
             + base * base_height;
    return c;
}


//////////
// MAIN //
//////////

varying vec2 world_pos;

void vertex() {
    world_pos = WorldPos(MODEL_MATRIX, VERTEX).xz;
    VERTEX.y = GetCity(world_pos).height;
}

void fragment() {
    City c = GetCity(world_pos);

    ALBEDO = c.road * road_color
           + c.berm * berm_color
           + c.base * base_color;

    vec3 dx = dFdx(VERTEX);
    vec3 dy = dFdy(VERTEX);
    NORMAL = normalize(cross(dx, dy));
}
