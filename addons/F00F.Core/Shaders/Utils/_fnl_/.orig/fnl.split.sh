#!/bin/bash
set +H

############################################################
### - Splits FNL into separate files (2D/3D/common)      ###
### - Adds preprocessor definitions around noise types,  ###
###   fractals and domain warp functionality for optimal ###
###   compile time and shader resource management        ###
### 2D/3D:                                               ###
FNL_USE_3D="FNL_USE_3D"                                  ###
FNL_USE_2D="FNL_USE_2D"                                  ###
### NOISE TYPES:                                         ###
FNL_USE_VALUE="FNL_USE_VALUE"                            ###
FNL_USE_PERLIN="FNL_USE_PERLIN"                          ###
FNL_USE_CELLULAR="FNL_USE_CELLULAR"                      ###
FNL_USE_SIMPLEX2="FNL_USE_SIMPLEX2"                      ###
FNL_USE_SIMPLEX2S="FNL_USE_SIMPLEX2S"                    ###
FNL_USE_VALUECUBIC="FNL_USE_VALUECUBIC"                  ###
### FRACTAL TYPES:                                       ###
FNL_USE_FBM="FNL_USE_FBM"                                ###
FNL_USE_RIDGED="FNL_USE_RIDGED"                          ###
FNL_USE_PINGPONG="FNL_USE_PINGPONG"                       ###
### DOMAINWARP TYPES:                                      ###
FNL_USE_DOMAINWARP_SIMPLEX2="FNL_USE_DOMAINWARP_SIMPLEX2"   ###
FNL_USE_DOMAINWARP_SIMPLEX2S="FNL_USE_DOMAINWARP_SIMPLEX2S"  ###
FNL_USE_DOMAINWARP_BASICGRID="FNL_USE_DOMAINWARP_BASICGRID"   ###
### DOMAINWARP STYLES:                                         ###
FNL_USE_DOMAINWARP_PROGRESSIVE="FNL_USE_DOMAINWARP_PROGRESSIVE" ###
FNL_USE_DOMAINWARP_INDEPENDENT="FNL_USE_DOMAINWARP_INDEPENDENT" ###
###################################################################

fnl_input="FastNoiseLite.glsl.master.orig"
#fnl_input="FastNoiseLite.glsl.orig" # v 1.1.1

output_dir=".."

##################
##### UTILS ######
##################

DONE() {
  if [[ $- == *i* ]]; then
    read -n1 -rsp "DONE (press any key to exit)"
  fi
}

################
##### MAIN #####
################

main() {
  fnl_out_XX="$output_dir/FastNoiseLite.gdshaderinc"
  fnl_out_2D="$output_dir/FastNoiseLite.2D.gdshaderinc"
  fnl_out_3D="$output_dir/FastNoiseLite.3D.gdshaderinc"
  custom_out="$output_dir/../ShaderNoise.gdshaderinc"

  fnl_def=false
  fnl_fnc=false
  fnl_wip=false
  fnl_out=$fnl_out_XX;
  fnl_pub="// Public API"
  fnl_prv="// From here on, this is private implementation"

  declare -A SPLIT=(
    ["2D"]=$fnl_out_2D
    ["3D"]=$fnl_out_3D
    ["WarpSimplex"]=$fnl_out_2D
    ["WarpOpenSimplex"]=$fnl_out_3D
  )

  IFDEF_3D="#ifdef $FNL_USE_3D"
  IFDEF_2D="#if defined($FNL_USE_2D) || !defined($FNL_USE_3D)"
  DEF_FRACS="defined($FNL_USE_FBM) || defined($FNL_USE_RIDGED) || defined($FNL_USE_PINGPONG)"
  DEF_WARPS="defined($FNL_USE_DOMAINWARP_BASICGRID) || defined($FNL_USE_DOMAINWARP_SIMPLEX2) || defined($FNL_USE_DOMAINWARP_SIMPLEX2S)"
  DEF_WARP_STYLES="defined($FNL_USE_DOMAINWARP_PROGRESSIVE) || defined($FNL_USE_DOMAINWARP_INDEPENDENT)"
  declare -A DEFS=(
    ["Value"]="#ifdef $FNL_USE_VALUE"
    ["Perlin"]="#ifdef $FNL_USE_PERLIN"
    ["Simplex"]="#ifdef $FNL_USE_SIMPLEX2"
    ["Cellular"]="#ifdef $FNL_USE_CELLULAR"
    ["Simplex2S"]="#ifdef $FNL_USE_SIMPLEX2S"
    ["ValueCubic"]="#ifdef $FNL_USE_VALUECUBIC"

    ["FBM"]="#ifdef $FNL_USE_FBM"
    ["Ridged"]="#ifdef $FNL_USE_RIDGED"
    ["PingPong"]="#ifdef $FNL_USE_PINGPONG"

    ["BasicGrid"]="#ifdef $FNL_USE_DOMAINWARP_BASICGRID"
    ["Progressive"]="#if ($DEF_WARPS) && defined($FNL_USE_DOMAINWARP_PROGRESSIVE)"
    ["Independent"]="#if ($DEF_WARPS) && defined($FNL_USE_DOMAINWARP_INDEPENDENT)"
    ["WarpSimplex"]="#if defined($FNL_USE_DOMAINWARP_SIMPLEX2) || defined($FNL_USE_DOMAINWARP_SIMPLEX2S)" # 2D
    ["WarpOpenSimplex"]="#if defined($FNL_USE_DOMAINWARP_SIMPLEX2) || defined($FNL_USE_DOMAINWARP_SIMPLEX2S)" # 3D

    ["Warp"]="#if $DEF_WARPS"
    ["CalculateFractalBounding"]="#if $DEF_FRACS || $DEF_WARPS"
  )

  declare -a KEYS=(
    $(for key in ${!DEFS[@]}; do
      printf "%d %s\n" ${#key} $key
    done | sort -nr | awk '{print $2}')
  )
 
  declare -a FUNCS=(
    "fnlGetNoise" #2D/3D
    "fnlDomainWarp" #2D/3D
    "_fnlGenNoiseSingle" #2D/3D
    "_fnlDoSingleDomainWarp" #2D/3D
  )

  declare -A CASE=(
    ["FNL_NOISE_VALUE"]="#ifdef $FNL_USE_VALUE"
    ["FNL_NOISE_PERLIN"]="#ifdef $FNL_USE_PERLIN"
    ["FNL_NOISE_CELLULAR"]="#ifdef $FNL_USE_CELLULAR"
    ["FNL_NOISE_OPENSIMPLEX2"]="#ifdef $FNL_USE_SIMPLEX2"
    ["FNL_NOISE_OPENSIMPLEX2S"]="#ifdef $FNL_USE_SIMPLEX2S"
    ["FNL_NOISE_VALUE_CUBIC"]="#ifdef $FNL_USE_VALUECUBIC"

    ["FNL_FRACTAL_FBM"]="#ifdef $FNL_USE_FBM"
    ["FNL_FRACTAL_RIDGED"]="#ifdef $FNL_USE_RIDGED"
    ["FNL_FRACTAL_PINGPONG"]="#ifdef $FNL_USE_PINGPONG"

    ["FNL_DOMAIN_WARP_BASICGRID"]="#ifdef $FNL_USE_DOMAINWARP_BASICGRID"
    ["FNL_DOMAIN_WARP_OPENSIMPLEX2"]="#ifdef $FNL_USE_DOMAINWARP_SIMPLEX2"
    ["FNL_DOMAIN_WARP_OPENSIMPLEX2_REDUCED"]="#ifdef $FNL_USE_DOMAINWARP_SIMPLEX2S"
    ["FNL_FRACTAL_DOMAIN_WARP_PROGRESSIVE"]="#ifdef $FNL_USE_DOMAINWARP_PROGRESSIVE"
    ["FNL_FRACTAL_DOMAIN_WARP_INDEPENDENT"]="#ifdef $FNL_USE_DOMAINWARP_INDEPENDENT"
  )

  : > $fnl_out_XX
  : > $fnl_out_2D
  : > $fnl_out_3D

  while IFS= read -r line; do
    if [[ $fnl_wip == true ]]; then
      if [[ ${line:0:2} == // ]]; then
        if [[ $line == $fnl_pub ]]; then
          #echo "** Found $fnl_pub"
          echo "$IFDEF_3D" >> $fnl_out_XX
          echo '#include "FastNoiseLite.3D.gdshaderinc"' >> $fnl_out_XX
          echo "#endif" >> $fnl_out_XX
          echo "$IFDEF_2D" >> $fnl_out_XX
          echo '#include "FastNoiseLite.2D.gdshaderinc"' >> $fnl_out_XX
          echo "#endif" >> $fnl_out_XX
          echo "" >> $fnl_out_XX
          echo "// ====================" >> $fnl_out_XX
          echo "// Public API" >> $fnl_out_XX
          echo "// ====================" >> $fnl_out_XX
          echo "" >> $fnl_out_XX
          echo "// ====================" >> $fnl_out_2D
          echo "// Public API (2D)" >> $fnl_out_2D
          echo "// ====================" >> $fnl_out_2D
          echo "" >> $fnl_out_2D
          echo "// ====================" >> $fnl_out_3D
          echo "// Public API (3D)" >> $fnl_out_3D
          echo "// ====================" >> $fnl_out_3D
          echo "" >> $fnl_out_3D
        fi
        continue
      elif [[ ${line:0:1} == } ]]; then
        fnl_fnc=false
        if [[ $fnl_def == true ]]; then
          fnl_def=false
          fnl_add="#endif"
        fi
      elif [[ $line =~ ^[^[:space:]\{] ]]; then
        fnl_out=$fnl_out_XX
        for key in ${!SPLIT[@]}; do
          if [[ $line == *$key* ]]; then
            fnl_out=${SPLIT[$key]}
            #echo "** $fnl_out => $line"
            break
          fi
        done
        for key in ${KEYS[@]}; do
          if [[ $line == *$key* ]]; then
            fnl_def=true
            echo ${DEFS[$key]} >> $fnl_out
            break
          fi
        done
        func_name=${line%%(*}
        func_name=${func_name##*[[:space:]]}
        func_name=${func_name%[23]D}
        for key in ${FUNCS[@]}; do
          if [[ $func_name == $key ]]; then
            fnl_fnc=true
            break
          fi
        done
      elif [[ $fnl_fnc == true ]]; then
        if [[ $fnl_case == true ]]; then
          if [[ $line == *break* || $line == *return* ]]; then
            fnl_add="#endif"
            unset fnl_case
          fi
        elif [[ $line == *case* ]]; then
          case_name=${line%%:*}
          case_name=${case_name##*[[:space:]]}
          for key in ${!CASE[@]}; do
            if [[ $case_name == $key ]]; then
              echo ${CASE[$key]} >> $fnl_out
              fnl_case=true
              break
            fi
          done
        fi
      fi
    elif [[ $line == $fnl_prv ]]; then
      fnl_wip=true
      #echo "** Found $fnl_prv"
      echo "// ====================" >> $fnl_out_XX
      echo "// PRIVATE" >> $fnl_out_XX
      echo "// ====================" >> $fnl_out_XX
      echo "" >> $fnl_out_XX
      echo "// ====================" >> $fnl_out_2D
      echo "// PRIVATE (2D)" >> $fnl_out_2D
      echo "// ====================" >> $fnl_out_2D
      echo "" >> $fnl_out_2D
      echo "// ====================" >> $fnl_out_3D
      echo "// PRIVATE (3D)" >> $fnl_out_3D
      echo "// ====================" >> $fnl_out_3D
      echo "" >> $fnl_out_3D
      continue
    fi
    echo "$line" >> $fnl_out
    if [[ $fnl_add != "" ]]; then
      echo "$fnl_add" >> $fnl_out
      unset fnl_add
    fi
  done < $fnl_input

  cat > $custom_out << EOF
//
// Usage:
//   [#define {2D/3D}]
//   #define {NOISE TYPE}
//   [#define {FRACTAL TYPE}]
//   [#define {DOMAIN WARP TYPE}]
//   [#define {DOMAIN WARP STYLE}]
//   #include "res://addons/F00F.Core/Shaders/Utils/ShaderNoise.gdshaderinc"
//
// 2D/3D:
//  $FNL_USE_3D
//  $FNL_USE_2D (default)
//
// NOISE TYPES:
//  $FNL_USE_VALUE
//  $FNL_USE_PERLIN
//  $FNL_USE_CELLULAR
//  $FNL_USE_SIMPLEX2
//  $FNL_USE_SIMPLEX2S
//  $FNL_USE_VALUECUBIC
//
// FRACTAL TYPES:
//  $FNL_USE_FBM
//  $FNL_USE_RIDGED
//  $FNL_USE_PINGPONG
//
// DOMAIN WARP TYPES:
//  $FNL_USE_DOMAINWARP_SIMPLEX2
//  $FNL_USE_DOMAINWARP_SIMPLEX2S
//  $FNL_USE_DOMAINWARP_BASICGRID
//
// DOMAIN WARP STYLES:
//  $FNL_USE_DOMAINWARP_PROGRESSIVE
//  $FNL_USE_DOMAINWARP_INDEPENDENT
//

#include "_fnl_/FastNoiseLite.gdshaderinc"

uniform int     Seed;
uniform float   Frequency : hint_range(0,1) = 0.01;
uniform int     NoiseType : hint_enum("Simplex2", "Simplex2S", "Cellular", "Perlin", "ValueCubic", "Value");

#if $DEF_FRACS
group_uniforms  Fractal;
uniform int     FractalType : hint_enum("None", "Fbm", "Ridged", "PingPong");
uniform int     FractalOctaves = 3;
uniform float   FractalLacunarity = 2.0;
uniform float   FractalGain = 0.5;
uniform float   FractalWeightedStrength = 0.0;
#ifdef $FNL_USE_PINGPONG
uniform float   FractalPingPongStrength = 2.0;
#endif
#endif

#if $DEF_WARPS
group_uniforms  DomainWarp;
uniform int     DomainWarpType : hint_enum("None:-1", "Simplex2", "Simplex2S", "BasicGrid") = -1;
#if $DEF_WARP_STYLES
uniform int     DomainWarpStyle : hint_enum("None:0", "Progressive:4", "Independent:5");
#endif
uniform float   DomainWarpAmplitude = 1.0;
uniform float   DomainWarpFrequency : hint_range(0,1) = 0.01;
uniform int     DomainWarpOctaves = 3;
#if $DEF_WARP_STYLES
uniform float   DomainWarpLacunarity = 2.0;
#endif
uniform float   DomainWarpGain = 0.5;
#endif

#ifdef $FNL_USE_CELLULAR
group_uniforms  Cellular;
uniform float   CellularJitter : hint_range(0,1) = 1.0;
uniform int     CellularReturnType : hint_enum("CellValue", "Distance", "Distance2", "Distance2Add", "Distance2Sub", "Distance2Mul", "Distance2Div") = FNL_CELLULAR_RETURN_TYPE_DISTANCE;
uniform int     CellularDistanceFunction : hint_enum("Euclidean", "EuclideanSquared", "Manhattan", "Hybrid") = FNL_CELLULAR_DISTANCE_EUCLIDEANSQ;
#endif

group_uniforms Sampling;
$IFDEF_2D
uniform vec2    Offset;
#endif
$IFDEF_3D
uniform vec3    Offset3D;
uniform int     RotationType3D : hint_enum("None", "ImproveXYPlanes", "ImproveXZPlanes");
#endif

fnl_state FNL()
{
    fnl_state fnl = fnlCreateState(Seed);
    fnl.frequency = Frequency;
    fnl.noise_type = NoiseType;
#if $DEF_FRACS
    fnl.fractal_type = FractalType;
    fnl.octaves = FractalOctaves;
    fnl.lacunarity = FractalLacunarity;
    fnl.gain = FractalGain;
    fnl.weighted_strength = FractalWeightedStrength;
#ifdef $FNL_USE_PINGPONG
    fnl.ping_pong_strength = FractalPingPongStrength;
#endif
#endif
#ifdef $FNL_USE_CELLULAR
    fnl.cellular_jitter_mod = CellularJitter;
    fnl.cellular_return_type = CellularReturnType;
    fnl.cellular_distance_func = CellularDistanceFunction;
#endif
$IFDEF_3D
    fnl.rotation_type_3d = RotationType3D;
#endif
    return fnl;
}

#if $DEF_WARPS
fnl_state WARP()
{
    fnl_state fnl = fnlCreateState(Seed);
    fnl.domain_warp_type = DomainWarpType;
#if $DEF_WARP_STYLES
    fnl.fractal_type = DomainWarpStyle;
#endif
    fnl.domain_warp_amp = DomainWarpAmplitude;
    fnl.frequency = DomainWarpFrequency;
    fnl.octaves = DomainWarpOctaves;
#if $DEF_WARP_STYLES
    fnl.lacunarity = DomainWarpLacunarity;
#endif
    fnl.gain = DomainWarpGain;
$IFDEF_3D
    fnl.rotation_type_3d = RotationType3D;
#endif
    return fnl;
}
#endif

// ====================
// Public API
// ====================

$IFDEF_3D
float GetNoise3D(vec3 p) {
    p += Offset3D;
#if $DEF_WARPS
    if (DomainWarpType >= 0) {
        fnlDomainWarp3D(WARP(), p.x, p.y, p.z);
    }
#endif
    return fnlGetNoise3D(FNL(), p.x, p.y, p.z);
}
#endif

$IFDEF_2D
float GetNoise2D(vec2 p) {
    p += Offset;
#if $DEF_WARPS
    if (DomainWarpType >= 0) {
        fnlDomainWarp2D(WARP(), p.x, p.y);
    }
#endif
    return fnlGetNoise2D(FNL(), p.x, p.y);
}
#endif
EOF
}

###############
##### END #####
###############

main
DONE
