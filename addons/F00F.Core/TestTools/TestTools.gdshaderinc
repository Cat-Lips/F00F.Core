group_uniforms Debug;
// Click here to hide code

    uniform float dbg_text_scale : hint_range(0.2, 20.0, 0.2) = 4.0;
    uniform float dbg_text_offset : hint_range(0.0, 0.5, 0.02) = 0.08;
    uniform float dbg_alpha : hint_range(0.0, 1.0, 0.1) = 0.7;
    uniform float dbg_size_x : hint_range(0.0, 1.0, 0.05) = 0.9;
    uniform float dbg_size_y : hint_range(0.0, 1.0, 0.05) = 0.9;
    uniform float dbg_pos_x : hint_range(0.0, 1.0, 0.05) = 0.5;
    uniform float dbg_pos_y : hint_range(0.0, 1.0, 0.05) = 0.5;
    uniform bool dbg_fix_floats = false;
    struct Debug {
        vec2 uv;
        vec2 pos;
        float result;
        float scale;
        float addY;
        bool onUV;
    };

    const vec2 digit_divs = vec2(4.0, 5.0);
    const int digit_mask[10] = int[10](480599, 139810, 476951, 476999, 350020, 464711, 464727, 476228, 481111, 481095);
    const int cell_mask[20] = int[20](1, 2, 4, 8, 16, 32, 64, 128, 256, 512, 1024, 2048, 4096, 8192, 16384, 32768, 65536, 131072, 262144, 524288);
    const int min_mask = 1792;
    const int dot_mask = 2;
    const int remap_digits[7] = int[7](0,0,1,0,2,3,4);
    int getDigit(float fnum, int place) {
        float num = clamp(abs(fnum), 0.0001, 99.999);
        float sm_num = num / 100.0; // 3 decimal places
        int number = int(fract(sm_num * pow(10.0, float(place))) * 10.0);
        return clamp(number, 0, 9);
    }
    void _runFloat(inout Debug dbg, in float number, in vec2 addPos, in bool showDecimals) {
        float num_val = number;
        if (dbg_fix_floats) { num_val += num_val < 0.0 ? -0.001 : 0.001; }
        vec2 pos_uv = vec2(dbg.pos.x,1.0-dbg.pos.y) + vec2(addPos.x,-addPos.y);
        vec2 uv_base = vec2(dbg.uv.x,1.0-dbg.uv.y);
        float size = dbg.scale / 100.0;
        vec2 uv_sc = (uv_base - pos_uv) * vec2(1.0 / (size * 5.0), 1.0 / size) + pos_uv + vec2(0.142,1.0);    
        float box_stencil = 1.0;
        float bsx = uv_sc.x > 1.0 + pos_uv.x ? 0.0 :box_stencil;
        box_stencil = uv_sc.x < 0.0 + pos_uv.x ? 0.0 : bsx;
        float bsy = uv_sc.y > 1.0 + pos_uv.y ? 0.0 : box_stencil;
        box_stencil = uv_sc.y < 0.0 + pos_uv.y ? 0.0 : bsy;
        vec2 digit_split = clamp((uv_sc - pos_uv) * vec2(7.0,1.0), vec2(0.0, 0.0), vec2(7.0, 1.0));
        int digit_id = int(floor(digit_split.x));
        vec2 digit_uv = fract(digit_split);
        ivec2 uv_cells = ivec2(floor(digit_uv * digit_divs));
        int cell_id = clamp(int(uv_cells.y * int(digit_divs.x) + uv_cells.x), 0, 19);
        float result = 0.0;
        if (digit_id == 0 && num_val < 0.0) {
            result = float(cell_mask[cell_id] & min_mask);
        }
        else if (digit_id == 3 && showDecimals) {
            result = float(cell_mask[cell_id] & dot_mask);
        }
        else if (digit_id != 0 && digit_id != 3) {
            if (showDecimals || digit_id < 3) {
                int digit = getDigit(num_val, remap_digits[digit_id]);
                result = float(cell_mask[cell_id] & digit_mask[digit]);
            }
        }
        dbg.result += result * box_stencil;
    }
    void runFloat(inout Debug dbg, in float number, in vec2 addPos) { _runFloat(dbg, number, addPos, true); }
    void runFloor(inout Debug dbg, in float number, in vec2 addPos) { _runFloat(dbg, number, addPos, false); }

    void dbgFloor(inout Debug dbg, in float number) {
        if (dbg.onUV) {
            runFloor( dbg, number, vec2(0.0));
            dbg.pos.y += dbg.addY;
        }
    }
    void dbgFloat(inout Debug dbg, in float number) {
        if (dbg.onUV) {
            runFloat(dbg, number, vec2(0.0));
            dbg.pos.y += dbg.addY;
        }
    }
    void dbgVec2(inout Debug dbg, vec2 vector) {
        if (dbg.onUV) {
            float space = dbg.scale * 0.06;
            runFloat(dbg, vector.x, vec2(0.0));
            runFloat(dbg, vector.y, vec2(space,0.0));
            dbg.pos.y += dbg.addY;
        }
    }
    void dbgVec3(inout Debug dbg, vec3 vector) {
        if (dbg.onUV) {
            float space = dbg.scale * 0.06;
            runFloat(dbg, vector.x, vec2(0.0));
            runFloat(dbg, vector.y, vec2(space,0.0));
            runFloat(dbg, vector.z, vec2(space+space,0.0));
            dbg.pos.y += dbg.addY;
        }
    }
    void dbgVec4(inout Debug dbg, vec4 vector) {
        if (dbg.onUV) {
            float space = dbg.scale * 0.06;
            runFloat(dbg, vector.x, vec2(0.0));
            runFloat(dbg, vector.y, vec2(space,0.0));
            runFloat(dbg, vector.z, vec2(space+space,0.0));
            runFloat(dbg, vector.w, vec2(space+space+space,0.0));
            dbg.pos.y += dbg.addY;
        }
    }
    void dbgMat3(inout Debug dbg, in mat3 mat) {
        if (dbg.onUV) {
            vec3 m1 = mat[0];
            vec3 m2 = mat[1];
            vec3 m3 = mat[2];
            dbgVec3(dbg, m1);
            dbgVec3(dbg, m2);
            dbgVec3(dbg, m3);
        }
    }
    void dbgMat4(inout Debug dbg, in mat4 mat) {
        if (dbg.onUV) {
            vec4 m1 = mat[0];
            vec4 m2 = mat[1];
            vec4 m3 = mat[2];
            vec4 m4 = mat[2];
            dbgVec4(dbg, m1);
            dbgVec4(dbg, m2);
            dbgVec4(dbg, m3);
            dbgVec4(dbg, m4);
        }
    }
    void dbgSpacer(inout Debug dbg) {
        dbg.pos.y += dbg.addY;
    }



    void debug_finish( inout vec3 set, inout Debug dbg ) {
        if (dbg.onUV) {
            vec3 col = vec3(0.01);
            col.r = dbg.result;
            float m = dbg_alpha;
            m = -pow(2.0, -10.0 * dbg_alpha) + 1.0;
            set = mix(set,col,m);
        }
    }
    Debug debug_construct( in vec2 screen_uv ) {
        Debug dbg;
        dbg.uv = screen_uv;
        dbg.uv.x -= (1.0-dbg_size_x)*dbg_pos_x;
        dbg.uv.y -= (1.0-dbg_size_y)*dbg_pos_y;
        dbg.uv /= vec2(dbg_size_x,dbg_size_y);
        dbg.pos = vec2(dbg_text_offset);
        dbg.result = 0.0;
        dbg.scale = dbg_text_scale;
        dbg.addY = dbg_text_scale / 80.0;
        dbg.onUV = false;
        if (dbg.uv.x > 0.0 && dbg.uv.x < 1.0 && dbg.uv.y > 0.0 && dbg.uv.y < 1.0) {
            dbg.onUV = true;
        }
        return dbg;
    }

    /**
        * // Functions:
        * debug_construct
        * debug_finish
        * 
        * dbgFloat
        * dbgVec2
        * dbgVec3
        * dbgVec4
        * dbgMat3
        * dbgMat4
        * 
        * dbgSpacer
        * //
        * 
        * 
        * Godot Shader Debugger
        * Adapted from villain749 on reddit:
        * https://www.reddit.com/r/GraphicsProgramming/comments/12m1d36/comment/jg939n4
        * 
        * Displays values for floats, vectors and matricies
        * Values limited to -99.999 to 99.999
        * Float values are a little rough, use the Fix Floats toggle for integers
        * Obviously only works for per-frame rendered values
        * Easy to add to existing code
        * 
        *     To Use:
        * Debug dbg = debug_construct(UV);
        *        // Construct a dbg, passing a UV
        *        // Use SCREEN_UV for sky (ONLY WORKS in forward_plus rendering)
        * void my_func( inout Debug dbg ) {}
        *        // Pass the dbg to any function
        * dbgFloat(dbg, value);
        *        // Call the function for your value
        *        // See above for all available value functions
        * debug_finish(color, dbg);
        *        // Pass your color to the finisher before applying
    */

    //  Debug code can be folded (hidden) at top
