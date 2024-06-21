shader_type spatial;
render_mode blend_mix, depth_draw_opaque, cull_back, diffuse_burley, specular_schlick_ggx;

// Include your exact Voronoi configuration
#define FNL_VORONOI_EUCLIDEAN
#include "res://addons/F00F.Core/Shaders/Utils/fnl/fnl.voronoi.gdshaderinc"

group_uniforms Noise_Settings;
uniform int seed = 1337;
uniform float scale = 40.0;
uniform float jitter : hint_range(0.0, 1.0) = 0.9;

group_uniforms City_Layout_Tuning;
uniform float road_width : hint_range(0.01, 0.2) = 0.05;
uniform float block_margin : hint_range(0.01, 0.2) = 0.08;

group_uniforms Palette;
uniform vec3 road_color : source_color = vec3(0.12, 0.12, 0.14);
uniform vec3 residential_zone : source_color = vec3(0.22, 0.38, 0.24);
uniform vec3 commercial_zone : source_color = vec3(0.18, 0.32, 0.45);
uniform vec3 industrial_zone : source_color = vec3(0.42, 0.35, 0.22);

void fragment() {
    // 1. Initialise the Voronoi configurations using your library structs
    // Frequency is set to 1.0 because we scale global UV coordinates manually
    _voronoi config = _voronoi(seed, 1.0, jitter);
    
    // Scale UV coordinates to map out the grid sizes
    vec2 pos = UV * scale;
    voronoi v = fnlVoronoi2D(config, pos.x, pos.y);

    // 2. Identify road lines using the cell border subtraction metric
    // v.d_sub tracks (dist2 - dist1), which approaches zero near cell borders
    // We counteract the '- 1.f' offset hardcoded into your library return
    float border_dist = v.d_sub + 1.0; 
    
    // Create anti-aliased crisp road structures 
    float road_mask = 1.0 - smoothstep(road_width, road_width + 0.01, border_dist);

    // 3. Define city zone regions based on unique cell hashes
    // We isolate the unique cell ID and normalize it to a clean 0.0 - 1.0 index
    float zone_id = fract(v.cell * 15.234); 
    
    vec3 zone_color = residential_zone;
    if (zone_id > 0.66) {
        zone_color = commercial_zone;
    } else if (zone_id > 0.33) {
        zone_color = industrial_zone;
    }

    // 4. Carve out distinct city block styles 
    // Smooth stepping inside the blocks isolates them neatly from sidewalks
    float block_mask = smoothstep(block_margin, block_margin + 0.01, border_dist);
    
    // Procedural inner-block detail grid mimicking buildings / housing parcels
    vec2 block_grid = fract(pos * 3.0);
    float building_grid = step(0.15, block_grid.x) * step(0.15, block_grid.y);
    
    // Blend the inner building grid accents softly with the base zone color
    vec3 block_final_color = mix(zone_color * 0.75, zone_color, building_grid);

    // 5. Build final fragments outputs
    // Overlay the layout colors: Roads clip directly over building blocks
    vec3 final_albedo = mix(block_final_color, road_color, road_mask);
    
    ALBEDO = final_albedo;
    METALLIC = 0.0;
    ROUGHNESS = mix(0.2, 0.9, block_mask); // Make roads sleeker than matte green zones
}
