shader_type spatial;
render_mode blend_mix, depth_draw_opaque, cull_back, diffuse_burley, specular_schlick_ggx;

// Include your exact Voronoi configuration
#define FNL_VORONOI_EUCLIDEAN
#include "res://addons/F00F.Core/Shaders/Utils/fnl/fnl.voronoi.gdshaderinc"

group_uniforms Noise_System;
uniform int seed = 42;
uniform float world_scale = 0.025;         // Lower frequency to account for massive world units
uniform float jitter : hint_range(0.0, 1.0) = 0.85;

group_uniforms City_Center_Settings;
uniform vec2 city_origin = vec2(0.0, 0.0); // Global world coordinates (X, Z) of downtown
uniform float city_radius = 500.0;         // Size of city core in world units before fading to farms

group_uniforms Building_Generation;
uniform float max_building_height = 40.0;  // Real-world height metric scaling
uniform float building_steepness = 2.0;

group_uniforms Palette_and_Textures;
uniform vec3 road_color : source_color = vec3(0.08, 0.08, 0.09);
uniform vec3 pavement_color : source_color = vec3(0.35, 0.35, 0.37);
uniform vec3 building_color : source_color = vec3(0.25, 0.28, 0.32);
uniform vec3 farm_color_a : source_color = vec3(0.18, 0.30, 0.12);
uniform vec3 farm_color_b : source_color = vec3(0.35, 0.32, 0.18);
uniform vec3 grass_verge : source_color = vec3(0.15, 0.32, 0.14);

// Varyings to move data safely from Vertex to Fragment pipelines
varying float v_zone_density;
varying float v_road_mask;
varying float v_pavement_mask;
varying float v_is_building;

// Branchless calculation for urban density relative to global world center
float get_world_density(vec2 world_xz) {
    float dist = distance(world_xz, city_origin);
    return smoothstep(city_radius, 0.0, dist);
}

void vertex() {
    // 1. Calculate the exact world space positions across chunk bounds
    // We construct the world coordinate space from the mesh transformations
    vec4 world_pos = MODEL_MATRIX * vec4(VERTEX, 1.0);
    vec2 xz_coords = world_pos.xz;

    float density = get_world_density(xz_coords);
    v_zone_density = density;

    // 2. Fetch procedural Voronoi arrays using global positioning vectors
    _voronoi config = _voronoi(seed, 1.0, jitter);
    vec2 noise_pos = xz_coords * world_scale;
    voronoi v = fnlVoronoi2D(config, noise_pos.x, noise_pos.y);

    float border_dist = v.d_sub + 1.0; 

    // Compute widths mathematically instead of using logic branches
    float target_road_width = mix(0.03, 0.08, density);
    float road_mask = 1.0 - smoothstep(target_road_width, target_road_width + 0.005, border_dist);
    v_road_mask = road_mask;

    float target_pavement_width = mix(0.0, 0.20, density);
    float pavement_mask = 1.0 - smoothstep(target_pavement_width, target_pavement_width + 0.005, border_dist);
    v_pavement_mask = clamp(pavement_mask - road_mask, 0.0, 1.0);

    // 3. Mathematical height extrusion
    float building_eligible = step(target_pavement_width + 0.005, border_dist);
    float plot_random = fract(v.cell * 45.321);
    
    float height_modifier = pow(density, building_steepness) * max_building_height;
    float final_extrusion = plot_random * height_modifier * building_eligible;

    // Isolate upward facing surfaces cleanly using step functions
    float is_up_face = step(0.5, NORMAL.y);
    float extrusion_applied = final_extrusion * is_up_face;
    
    VERTEX.y += extrusion_applied;
    v_is_building = step(0.1, extrusion_applied);
}

void fragment() {
    // 1. Regenerate global positioning vectors for micro-detailing maps
    vec4 world_pos = INV_VIEW_MATRIX * vec4(VERTEX, 1.0);
    vec2 macro_pos = world_pos.xz * world_scale * 15.0; 

    // 2. Mathematical computation of underlying zone profiles (Branchless)
    // Farmland crop fields pattern
    float farm_lines = step(0.5, sin(macro_pos.x * 2.0) * sin(macro_pos.y * 0.5));
    vec3 farmland_texture = mix(farm_color_a, farm_color_b, farm_lines);

    // Marketplace squares pattern
    vec2 market_uv = fract(macro_pos * 2.0);
    float market_stalls = step(0.25, market_uv.x) * step(0.25, market_uv.y);
    vec3 market_texture = mix(pavement_color, vec3(0.55, 0.20, 0.18), market_stalls * 0.5);

    // 3. Define background landscape context based purely on city core range
    vec3 background_terrain = mix(farmland_texture, grass_verge, v_zone_density);
    vec3 ground_pavement = mix(pavement_color, market_texture, smoothstep(0.5, 0.85, v_zone_density));

    // 4. Fragment Combination using linear step mixtures (Zero Branching)
    // We compose the surface iteratively from the lowest layer to the highest
    vec3 final_surface = background_terrain;
    
    // If pavement mask is active, overlay sidewalk layers
    final_surface = mix(final_surface, ground_pavement, v_pavement_mask);
    // If road mask is active, overlay pavement network layers
    final_surface = mix(final_surface, road_color, v_road_mask);
    // If building mask is active, layer skyscraper configurations over the terrain
    final_surface = mix(final_surface, building_color * 0.85, v_is_building);

    // Assign final values to physical fragment render variables
    ALBEDO = final_surface;
    METALLIC = 0.0;
    ROUGHNESS = mix(0.85, 0.25, v_road_mask);
}
