using System.Linq;
using Godot;

namespace F00F
{
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
                options._Name = config.GetValue("Name", @default: options.Name);
                options._Scene = config.GetValue("Scene", @default: options.Scene);
                options._BodyType = config.GetEnum("BodyType", @default: options.BodyType);
                options._FrontFace = config.GetEnum("FrontFace", @default: options.FrontFace);
                options._MassMultiplier = config.GetValue("MassMultiplier", @default: options.MassMultiplier);
                options._ScaleMultiplier = config.GetValue("ScaleMultiplier", @default: options.ScaleMultiplier);
                options._Nodes = config.GetSections().Except([""]).Select(LoadNode).ToArray();
                options._ImportOriginal = config.GetValue("ImportOriginal", @default: options.ImportOriginal);
                return options;

                GlbNode LoadNode(string section)
                {
                    var options = new GlbNode { Name = section };
                    options.ShapeType = config.GetEnum(section, "ShapeType", @default: options.ShapeType);
                    options.MultiConvexLimit = config.GetValue<int>(section, "MultiConvexLimit", @default: options.MultiConvexLimit);
                    return options;
                }
            }
        }

        public void Save(string path)
        {
            var config = new ConfigFile();

            config.SetValue("Name", Name);
            config.SetValue("Scene", Scene);
            config.SetEnum("BodyType", BodyType);
            config.SetEnum("FrontFace", FrontFace);
            config.SetValue("MassMultiplier", MassMultiplier);
            config.SetValue("ScaleMultiplier", ScaleMultiplier);
            Nodes.ForEach(x =>
            {
                config.SetEnum(x.Name, "ShapeType", x.ShapeType);
                config.SetValue(x.Name, "MultiConvexLimit", x.MultiConvexLimit);
            });
            config.SetValue("ImportOriginal", ImportOriginal);

            config.Save(path);
        }
    }
}
