using System.Configuration;

namespace GoogleDriveFileNameConsolidator
{
    /// <summary>
    /// The main application class from which the name consolidation process should be carried out.
    /// </summary>
    internal class Program
    {
        private static void Main()
        {
            /* Determine the directory from which the name consolidation process should begin. */
            string rootDirectory = ConfigurationManager.AppSettings?["RootDirectory"] ?? string.Empty;

            /* Iterate through all of the child items in the root directory. For each:
             *     - Directory -> Recursively iterate through that directory.
             *     - File -> Rename the file to match the name of it's parent directory.
             */
            ConsolidateFileNames(rootDirectory);
        }

        /// <summary>
        /// Recursively process everything from the current directory downwards, consolidating the names of all files in the process.
        /// </summary>
        /// <param name="currentDir">The path of the current directory.</param>
        private static void ConsolidateFileNames(string currentDir)
        {
            string directoryName = Path.GetFileName(currentDir);
            int renameCounter = 1;

            /* Rename all of the files in this directory. */
            foreach (string file in Directory.EnumerateFiles(currentDir))
            {
                string extension = Path.GetExtension(file);
                string path = Path.GetDirectoryName(file) ?? string.Empty;
                string newFileName = $"{path}/{directoryName} ({renameCounter}){extension}";

                if (!File.Exists(newFileName))
                {
                    File.Move(file, newFileName);
                    renameCounter++;
                }
            }

            /* Recursively process all of the subdirectories in this directory. */
            new List<string>(Directory.GetDirectories(currentDir)).ForEach(ConsolidateFileNames);
        }
    }
}