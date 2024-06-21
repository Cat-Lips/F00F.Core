shader_type spatial;

uniform float amplitude = 0.0;
uniform sampler2D heightmap;
uniform sampler2D colormap : source_color;

void vertex() {
    float height = texture(heightmap, UV).r;
    VERTEX.y += height * amplitude;
}

void fragment() {
    ALBEDO = texture(colormap, UV).rgb;
}
