﻿using System;

namespace F00F;

public static class EnumExtensions
{
    public static string Str<T>(this T source) where T : struct, Enum
        => $"{source}".Capitalise();
}
