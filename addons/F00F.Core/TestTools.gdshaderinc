#ifdef DEBUG

#include "res://addons/F00F.Core/Common/3D/TestTools/TestTools.gdshaderinc"

#define DBG_INIT Debug dbg = debug_construct(UV);
#define DBG dbg,
#define DBG_ inout Debug dbg,
#define DBG_FLT(x) dbgFloat(dbg, x);
#define DBG_FINISH debug_finish(ALBEDO, dbg);

#else

#define DBG_INIT
#define DBG
#define DBG_
#define DBG_FLT(x)
#define DBG_FINISH

#endif
