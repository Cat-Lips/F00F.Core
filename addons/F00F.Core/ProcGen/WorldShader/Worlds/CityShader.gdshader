shader_type spatial;
render_mode cull_back, depth_draw_opaque, diffuse_burley, specular_schlick_ggx;

uniform int seed;
uniform vec2 offset;
uniform float scale : hint_range(0.1, 10.0) = 1.0;

group_uniforms Thresholds;
uniform float road_threshold : hint_range(0.0, 1.0) = 0.08;
uniform float berm_threshold : hint_range(0.0, 1.0) = 0.20;
uniform float base_threshold : hint_range(0.0, 1.0) = 0.45;

group_uniforms Heights;
uniform float road_height : hint_range(0.0, 5.0) = 0.0;
uniform float berm_height : hint_range(0.0, 5.0) = 0.2;
uniform float base_height : hint_range(0.0, 50.0) = 1.0;

group_uniforms Buildings;
uniform float building_inset : hint_range(0.0, 0.5) = 0.12;
uniform float building_density : hint_range(0.0, 1.0) = 0.9;
uniform float building_min_height : hint_range(0.0, 100.0) = 4.0;
uniform float building_max_height : hint_range(0.0, 200.0) = 40.0;

group_uniforms Colors;
uniform vec3 road_color : source_color = vec3(0.11, 0.11, 0.12);
uniform vec3 berm_color : source_color = vec3(0.33, 0.33, 0.35);
uniform vec3 base_color : source_color = vec3(0.45, 0.55, 0.35);
uniform vec3 building_color : source_color = vec3(0.72, 0.72, 0.76);
uniform vec3 building_tint : source_color = vec3(1.0, 0.98, 0.94);


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

    float hash = v.cell;
    float edge = 1.0 + v.d_sub; // [0..1]
    float grad = smoothstep(road_threshold, berm_threshold, edge);
    float base = smoothstep(berm_threshold, base_threshold, edge);
    float berm = grad - base;
    float road = 1.0 - grad;

    float inset = smoothstep(building_inset, building_inset + 0.15, base);
    float building = step(Hash1f(hash), building_density) * inset;
    float building_height = mix(building_min_height, building_max_height, Hash2f(hash));

    City c;
    c.road = road;
    c.berm = berm;
    c.base = base;
    c.tone = Hashf(hash);
    c.building = building;
    c.height = road * road_height
             + berm * berm_height
             + base * base_height
             + building * building_height;
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
           + c.base * base_color
           + c.building * mix(building_color, building_color * building_tint, c.tone);

    vec3 dx = dFdx(vec3(world_pos.x, c.height, world_pos.y));
    vec3 dy = dFdy(vec3(world_pos.x, c.height, world_pos.y));
    NORMAL = normalize(cross(dx, dy));
}
