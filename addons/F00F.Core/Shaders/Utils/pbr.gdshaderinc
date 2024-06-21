#ifndef _F00F_UTILS_PBR_INC_
#define _F00F_UTILS_PBR_INC_

//
// Usage:
//   #include "res://addons/F00F.Core/Shaders/Utils/pbr.gdshaderinc"
//

uniform sampler2DArray albedo_array : source_color, filter_linear_mipmap_anisotropic, repeat_enable;
uniform sampler2DArray normal_array : hint_roughness_normal, filter_linear_mipmap_anisotropic, repeat_enable;
uniform sampler2DArray height_array : hint_default_white, filter_linear_mipmap_anisotropic, repeat_enable;
uniform sampler2DArray metallic_array : hint_default_white, filter_linear_mipmap_anisotropic, repeat_enable;
uniform sampler2DArray occlusion_array : hint_default_white, filter_linear_mipmap_anisotropic, repeat_enable;

uniform float texture_tiling : hint_range(0.0, 100.0) = 50.0;
uniform float depth_strength : hint_range(0.0, 0.1) = 0.03;
uniform float normal_strength : hint_range(0.0, 2.0) = 1.0;

const float min_depth_dist = 0.5;
const float max_depth_dist = 25.0;

struct PBR {
  vec3 albedo;
  vec3 normal;
  float metallic;
  float roughness;
  float occlusion;
};


// ====================
// PRIVATE
// ====================



// ====================
// MAIN
// ====================



#endif
