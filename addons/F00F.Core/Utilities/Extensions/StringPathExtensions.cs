using System.IO;

namespace F00F;

// FIXME: Can't use DirAccess/FileAccess for exported res files

public static class StringExtensions_Path
{
    public static string GetFileBaseName(this string source)
        => Path.GetFileNameWithoutExtension(source);

    public static string GetFileName(this string source)
        => Path.GetFileNameWithoutExtension(source);

    public static string GetFileExt(this string source)
        => Path.GetExtension(source);

    //public static IEnumerable<string> DirNames(this string path)
    //    => DirAccess.GetDirectoriesAt(path).Where(x => x.First() is not ('.' or '_'));

    //private static IEnumerable<string> DirFiles(this string path)
    //    => DirAccess.GetFilesAt(path).Where(file => file.GetExtension() is not "import");

    //public static string[] GetDirNames(this string path)
    //    => DirNames(path).ToArray();

    //public static string[] GetFiles(this string dir)
    //{
    //    return DirFiles(dir)
    //        .Select(dir.PathJoin)
    //        .ToArray();
    //}

    //public static string[] GetFiles(this string dir, params string[] ext)
    //{
    //    return DirFiles(dir)
    //        .Where(file => file.GetExtension().IsAnyOfN(ext))
    //        .Select(dir.PathJoin)
    //        .ToArray();
    //}

    //public static string[] GetFilesExcept(this string dir, params string[] ext)
    //{
    //    return DirFiles(dir)
    //        .Where(file => file.GetExtension().IsNotAnyOfN(ext))
    //        .Select(dir.PathJoin)
    //        .ToArray();
    //}

    //public static IDictionary<string, string> GetFilesWith(this string dir, params string[] parts)
    //{
    //    return MatchingFiles().ToDictionary(x => x.Part, x => x.Path);

    //    IEnumerable<(string Part, string Path)> MatchingFiles()
    //    {
    //        foreach (var file in DirFiles(dir))
    //        {
    //            if (!TryGetPart(file, out var part))
    //                continue;

    //            yield return (part, dir.PathJoin(file));
    //        }

    //        bool TryGetPart(string file, out string part)
    //            => (part = parts.Where(file.ContainsN).SingleOrDefault()) is not null;
    //    }
    //}
}
