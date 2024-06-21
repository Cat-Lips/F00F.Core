shader_type spatial;

///////////
// UTILS //
///////////

#include "res://addons/F00F.Core/Shaders/Utils/Utils.gdshaderinc"
#include "res://addons/F00F.Core/Shaders/Utils/ShaderNoise.gdshaderinc"

///////////
// FUNCS //
///////////



//////////
// MAIN //
//////////

varying vec2 world_pos;

void vertex() {
    world_pos = WorldPos(MODEL_MATRIX, VERTEX).xz;
    VERTEX.y = GetNoise2D(world_pos) * 50.0;
}

void fragment() {
    ALBEDO = vec3(GetNoise2D(world_pos) * 0.5 + 0.5);
}
