shader_type spatial;
render_mode blend_mix, depth_draw_opaque, cull_back, diffuse_burley, specular_schlick_ggx;

// Include your exact Voronoi configuration
#define FNL_VORONOI_EUCLIDEAN
#include "res://addons/F00F.Core/Shaders/Utils/fnl/fnl.voronoi.gdshaderinc"

group_uniforms Noise_System;
uniform int seed = 42;
uniform float world_scale = 30.0;
uniform float jitter : hint_range(0.0, 1.0) = 0.85;

group_uniforms City_Center_Settings;
uniform vec2 city_center = vec2(0.5, 0.5); // Center point in UV space
uniform float city_radius = 0.4;           // Transition radius from inner to outer city

group_uniforms Building_Generation;
uniform float max_building_height = 8.0;
uniform float building_steepness = 3.0;    // Sharpness of height decay towards edge

group_uniforms Palette_and_Textures;
uniform vec3 road_color : source_color = vec3(0.1, 0.1, 0.11);
uniform vec3 pavement_color : source_color = vec3(0.4, 0.4, 0.42);
uniform vec3 building_color : source_color = vec3(0.3, 0.35, 0.4);
uniform vec3 farm_color_a : source_color = vec3(0.2, 0.35, 0.15);
uniform vec3 farm_color_b : source_color = vec3(0.4, 0.38, 0.2);

// Varying to pass the calculated height profile from Vertex to Fragment
varying float v_zone_density;
varying float v_road_mask;
varying float v_pavement_mask;
varying float v_is_building;

// Helper to determine city density (1.0 = Downtown Core, 0.0 = Farmland Outer Edge)
float get_density(vec2 uv) {
    float dist = distance(uv, city_center);
    return smoothstep(city_radius, 0.0, dist);
}

void vertex() {
    // 1. Calculate Core Density Structure
    float density = get_density(UV);
    v_zone_density = density;

    // 2. Fetch Voronoi properties for structural features
    _voronoi config = _voronoi(seed, 1.0, jitter);
    vec2 noise_pos = UV * world_scale;
    voronoi v = fnlVoronoi2D(config, noise_pos.x, noise_pos.y);

    // Realign hardcoded negative offset from your library file
    float border_dist = v.d_sub + 1.0; 

    // Dynamic Road Widths: Roads widen where they converge in dense zones
    float target_road_width = mix(0.04, 0.09, density);
    float road_mask = 1.0 - smoothstep(target_road_width, target_road_width + 0.01, border_dist);
    v_road_mask = road_mask;

    // Dynamic Pavement Widths: Broad sidewalks in inner city, shrinking to non-existent at edge
    float target_pavement_width = mix(0.0, 0.22, density);
    float pavement_mask = 1.0 - smoothstep(target_pavement_width, target_pavement_width + 0.01, border_dist);
    v_pavement_mask = clamp(pavement_mask - road_mask, 0.0, 1.0);

    // 3. Extrude Building Geometry upwards
    // Buildings only appear if they aren't on roads or sidewalks
    float building_eligible = step(target_pavement_width + 0.01, border_dist);
    
    // Deterministic random check using your cell hash to create discrete plots
    float plot_random = fract(v.cell * 45.321);
    
    // Scale building heights intensely in the inner core, flat structures on the outskirts
    float height_modifier = pow(density, building_steepness) * max_building_height;
    float final_extrusion = plot_random * height_modifier * building_eligible;

    // Extrude vertices up along the Y-axis if the mesh faces up (Normal.y > 0.5)
    if (NORMAL.y > 0.5 && final_extrusion > 0.1) {
        VERTEX.y += final_extrusion;
        v_is_building = 1.0;
    } else {
        v_is_building = 0.0;
    }
}

void fragment() {
    // 1. Regenerate local coordinate data for macro-texturing textures
    vec2 macro_pos = UV * world_scale;
    
    // 2. Compute Farmland Texture & Grass Verges (Outer Zone, v_zone_density -> 0.0)
    // Create lined crop fields using a basic repeating sine pattern
    float farm_lines = step(0.5, sin(macro_pos.x * 8.0) * sin(macro_pos.y * 2.0));
    vec3 farmland_texture = mix(farm_color_a, farm_color_b, farm_lines);
    
    // Smooth green verges alongside roads on outskirts
    vec3 grass_verge_color = vec3(0.18, 0.38, 0.16);

    // 3. Compute Marketplace Texturing (Inner Zone convergence points)
    // We create structural market stalls using high frequency checkered textures
    vec2 market_uv = fract(macro_pos * 4.0);
    float market_stalls = step(0.3, market_uv.x) * step(0.3, market_uv.y);
    vec3 market_texture = mix(pavement_color, vec3(0.65, 0.25, 0.22), market_stalls * 0.6);

    // 4. Fragment Composition Logic
    vec3 surface_color = vec3(0.0);

    if (v_is_building > 0.5) {
        // Building Rooftops: Shaded slightly darker for skylines
        surface_color = building_color * 0.85;
    } else if (v_road_mask > 0.5) {
        // Road networks
        surface_color = road_color;
    } else if (v_pavement_mask > 0.5) {
        // If density is high, render vibrant plazas/markets, else render default concrete sidewalks
        surface_color = mix(pavement_color, market_texture, smoothstep(0.6, 0.9, v_zone_density));
    } else {
        // Terrain background: Blends smoothly from city center concrete to outer farmlands
        surface_color = mix(farmland_texture, grass_verge_color, v_zone_density);
    }

    // Apply material values
    ALBEDO = surface_color;
    METALLIC = 0.0;
    ROUGHNESS = mix(0.9, 0.3, v_road_mask); // Roads are smoother than organic dirt/farms
}
