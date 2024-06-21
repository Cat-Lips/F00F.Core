using System;
using System.Linq;
using Godot;

namespace F00F;

public partial class GlbOptions
{
    public static GlbOptions Load(string path)
    {
        var config = new ConfigFile();
        if (FS.FileExists(path))
            config.Load(path);
        return Load();

        GlbOptions Load()
        {
            var options = new GlbOptions();
            options._Name = (string)config.GetValue("", "Name", @default: options.Name);
            options._Scene = (string)config.GetValue("", "Scene", @default: options.Scene);
            options._Rotate = Enum.Parse<GlbRotate>((string)config.GetValue("", "Rotate", @default: $"{options.Rotate}"));
            options._BodyType = Enum.Parse<GlbBodyType>((string)config.GetValue("", "BodyType", @default: $"{options.BodyType}"));
            options._MassMultiplier = (float)config.GetValue("", "MassMultiplier", @default: options.MassMultiplier);
            options._ScaleMultiplier = (float)config.GetValue("", "ScaleMultiplier", @default: options.ScaleMultiplier);
            options._Nodes = config.GetSections().Except([""]).Select(LoadNode).ToArray();
            options._ImportOriginal = (bool)config.GetValue("", "ImportOriginal", @default: options.ImportOriginal);
            return options;

            GlbNode LoadNode(string section)
            {
                var options = new GlbNode { Name = section };
                options.ShapeType = Enum.Parse<GlbShapeType>((string)config.GetValue(section, "ShapeType", @default: $"{options.ShapeType}"));
                options.MultiConvexLimit = (int)config.GetValue(section, "MultiConvexLimit", @default: options.MultiConvexLimit);
                return options;
            }
        }
    }

    public void Save(string path)
    {
        var config = new ConfigFile();

        config.SetValue("", "Name", Name);
        config.SetValue("", "Scene", Scene);
        config.SetValue("", "Rotate", $"{Rotate}");
        config.SetValue("", "BodyType", $"{BodyType}");
        config.SetValue("", "MassMultiplier", MassMultiplier);
        config.SetValue("", "ScaleMultiplier", ScaleMultiplier);
        Nodes.ForEach(x =>
        {
            config.SetValue(x.Name, "ShapeType", $"{x.ShapeType}");
            config.SetValue(x.Name, "MultiConvexLimit", x.MultiConvexLimit);
        });
        config.SetValue("", "ImportOriginal", ImportOriginal);

        config.Save(path);
    }
}
