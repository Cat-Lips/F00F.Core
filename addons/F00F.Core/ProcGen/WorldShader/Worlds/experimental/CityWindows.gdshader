shader_type spatial;
render_mode blend_mix, depth_draw_opaque, cull_back, diffuse_burley, specular_schlick_ggx;

uniform vec3 building_color : source_color = vec3(0.05, 0.05, 0.07);
uniform vec3 window_color_on : source_color = vec3(1.0, 0.85, 0.5);
uniform vec3 window_color_off : source_color = vec3(0.1, 0.1, 0.15);

uniform vec2 window_scale = vec2(20.0, 50.0); // Total grid rows and columns
uniform vec2 window_size = vec2(0.6, 0.7);    // Width/height of window inside its grid cell (0.0 to 1.0)
uniform float light_probability : hint_range(0.0, 1.0) = 0.4; // Percentage of lit windows
uniform float emission_intensity = 3.0;        // Controls bloom/glow intensity

// Simple pseudo-random function based on grid coordinates
float hash2d(vec2 p) {
    return fract(sin(dot(p, vec2(127.1, 311.7))) * 43758.5453123);
}

void fragment() {
    // 1. Base building color
    ALBEDO = building_color;
    ROUGHNESS = 0.8;
    METALLIC = 0.1;

    // 2. Scale UV coordinates to create the grid mesh
    vec2 remembered_uv = UV * window_scale;
    vec2 grid_id = floor(remembered_uv);    // Unique ID for each window cell
    vec2 local_uv = fract(remembered_uv);   // Local UV coordinates inside one cell (0.0 to 1.0)

    // 3. Determine if the window inside this cell is "on"
    float rand_val = hash2d(grid_id);
    bool is_light_on = rand_val < light_probability;

    // 4. Create sharp window box boundaries using step()
    // Calculates margins on both sides of the window cell
    vec2 margin = (vec2(1.0) - window_size) * 0.5;
    vec2 window_box = step(margin, local_uv) * step(local_uv, vec2(1.0) - margin);
    float window_mask = window_box.x * window_box.y;

    // 5. Apply colors and emission
    if (window_mask > 0.5) {
        if (is_light_on) {
            ALBEDO = window_color_on;
            EMISSION = window_color_on * emission_intensity;
        } else {
            ALBEDO = window_color_off;
        }
        // Windows are usually made of reflective glass
        ROUGHNESS = 0.1;
        METALLIC = 0.9;
    }
}
