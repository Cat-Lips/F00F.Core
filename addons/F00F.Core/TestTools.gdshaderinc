#ifdef DEBUG

#include "res://addons/F00F.Core/TestTools/TestTools.gdshaderinc"

#define DBG_DECLARE(x) varying x;
#define DBG_LINE(x) x;
#define DBG_ARG(x) x,

#define DBG_INIT Debug dbg = debug_construct(UV);
#define DBG_IN dbg,
#define DBG_OUT inout Debug dbg,
#define DBG_FLT(x) dbgFloat(dbg, x);
#define DBG_FLR(x) dbgFloor(dbg, x);
#define DBG_FINISH debug_finish(ALBEDO, dbg);

#else

#define DBG_DECLARE(x)
#define DBG_LINE(x)
#define DBG_ARG(x)

#define DBG_INIT
#define DBG_IN
#define DBG_OUT
#define DBG_FLT(x)
#define DBG_FLR(x)
#define DBG_FINISH

#endif
