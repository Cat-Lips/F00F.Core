#ifndef _F00F_UTILS_INC_
#define _F00F_UTILS_INC_

//
// Usage:
//   #include "res://addons/F00F.Core/Shaders/Utils/Utils.gdshaderinc"
//

#define epsilon 0.0001

#define WorldPos(model_matrix, vertex) (model_matrix * vec4(vertex, 1.0)).xyz
#define ViewPos(view_matrix, vertex) (view_matrix * vec4(vertex, 1.0)).xyz

#endif
