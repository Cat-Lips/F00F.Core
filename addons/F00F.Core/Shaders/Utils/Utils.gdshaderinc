//
// Usage:
//   #include "res://addons/F00F.Core/Shaders/Utils/Utils.gdshaderinc"
//

vec3 WorldPos(mat4 model_matrix, vec3 vertex) {
    return (model_matrix * vec4(vertex, 1.0)).xyz;
}

vec3 ViewPos(mat4 view_matrix, vec3 vertex) {
  return (view_matrix * vec4(vertex, 1.0)).xyz;
}
