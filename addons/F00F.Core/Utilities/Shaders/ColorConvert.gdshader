shader_type canvas_item;

uniform vec3 color_from : source_color = vec3(1, 1, 1);
uniform vec3 color_to : source_color = vec3(1, 1, 1);

void fragment() {
    vec4 cur_color = texture(TEXTURE, UV);
    if (cur_color.rgb == color_from) {
        COLOR.rgb = color_to;
    }
}
