using System;
using System.IO;

namespace OwLivPatcher
{
    public static class OwLivPatcherMain
    {
        //Called by OWML
        public static void Main(string[] args)
        {
            var basePath = args.Length > 0 ? args[0] : ".";
            var gamePath = AppDomain.CurrentDomain.BaseDirectory;

            CopyGameFiles(gamePath, Path.Combine(basePath, "files"));
        }

        private static string GetExecutableName(string gamePath)
        {
            var executableNames = new[] {"Outer Wilds.exe", "OuterWilds.exe"};
            foreach (var executableName in executableNames)
            {
                var executablePath = Path.Combine(gamePath, executableName);
                if (File.Exists(executablePath))
                {
                    return Path.GetFileNameWithoutExtension(executablePath);
                }
            }

            throw new FileNotFoundException($"Outer Wilds exe file not found in {gamePath}");
        }

        private static string GetDataDirectoryName()
        {
            var gamePath = AppDomain.CurrentDomain.BaseDirectory;
            return $"{GetExecutableName(gamePath)}_Data";
        }

        private static void CopyGameFiles(string gamePath, string filesPath)
        {
            // Get the subdirectories for the specified directory.
            var dir = new DirectoryInfo(filesPath);

            if (!dir.Exists)
            {
                throw new DirectoryNotFoundException(
                    "Source directory does not exist or could not be found: "
                    + filesPath);
            }

            var dirs = dir.GetDirectories();

            // If the destination directory doesn't exist, create it.       
            Directory.CreateDirectory(gamePath);

            // Get the files in the directory and copy them to the new location.
            var files = dir.GetFiles();
            foreach (var file in files)
            {
                var tempPath = Path.Combine(gamePath, file.Name);
                file.CopyTo(tempPath, true);
            }

            foreach (var subdir in dirs)
            {
                var tempPath = Path.Combine(gamePath, subdir.Name);
                CopyGameFiles(tempPath.Replace("OuterWilds_Data", GetDataDirectoryName()), subdir.FullName);
            }
        }
    }
}
