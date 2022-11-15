namespace JoaInterface;

public static class StringExtensions
{
    public static bool IsDirectory(this string path)
    {
        return (File.GetAttributes(path) & FileAttributes.Directory) == FileAttributes.Directory;
    }
}