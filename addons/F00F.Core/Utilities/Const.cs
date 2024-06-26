﻿using Godot;

namespace F00F
{
    public static class Const
    {
        public static readonly string AppName = (string)ProjectSettings.GetSetting("application/config/name");
        public static readonly float DefaultGravity = (float)ProjectSettings.GetSetting("physics/3d/default_gravity");
        public static readonly int AppHash = GD.Hash(AppName);

        public const float Deg30 = Mathf.Pi / 6f;
        public const float Deg45 = Mathf.Pi * .25f;
        public const float Deg90 = Mathf.Pi * .5f;
        public const float Deg180 = Mathf.Pi;
        public const float Deg360 = Mathf.Tau;
    }
}
