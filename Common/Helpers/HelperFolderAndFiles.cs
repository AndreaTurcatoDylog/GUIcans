using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Common
{
    public class HelperFolderAndFiles
    {
        /// <summary>
        /// The method specify whether the target is excluded
        /// </summary>
        private bool isExcluded(List<string> excludedDirList, string target)
        {
            return excludedDirList.Any(d => new DirectoryInfo(target).Name.Equals(d.ToLower()));
        }

        /// <summary>
        /// Copy a directory from another
        /// </summary>
        public void DirectoryCopy(string sourceDirName, string destDirName, bool copySubDirs)
        {
            // Get the subdirectories for the specified directory.
            DirectoryInfo dir = new DirectoryInfo(sourceDirName);

            if (!dir.Exists)
            {
                throw new DirectoryNotFoundException(
                    "Source directory does not exist or could not be found: "
                    + sourceDirName);
            }

            DirectoryInfo[] dirs = dir.GetDirectories();

            // If the destination directory doesn't exist, create it.       
            Directory.CreateDirectory(destDirName);

            // Get the files in the directory and copy them to the new location.
            FileInfo[] files = dir.GetFiles();
            foreach (FileInfo file in files)
            {
                string tempPath = Path.Combine(destDirName, file.Name);
                file.CopyTo(tempPath, false);
            }

            // If copying subdirectories, copy them and their contents to new location.
            if (copySubDirs)
            {
                foreach (DirectoryInfo subdir in dirs)
                {
                    string tempPath = Path.Combine(destDirName, subdir.Name);
                    DirectoryCopy(subdir.FullName, tempPath, copySubDirs);
                }
            }
        }

        /// <summary>
        /// Copy file with some extensions from source to destination
        /// </summary>
        public void CopyFiles(string sourcePath, string targetPath, string[] extensions)
        {
            if (extensions != null && extensions.Count() > 0)
            {
                var files = (from file in Directory.EnumerateFiles(sourcePath)
                             where extensions.Contains(Path.GetExtension(file), StringComparer.InvariantCultureIgnoreCase) // comment this out if you don't want to filter extensions
                             select new
                             {
                                 Source = file,
                                 Destination = Path.Combine(targetPath, Path.GetFileName(file))
                             });

                foreach (var file in files)
                {
                    File.Copy(file.Source, file.Destination);
                }
            }
            else
            {
                var files = (from file in Directory.EnumerateFiles(sourcePath)
                             select new
                             {
                                 Source = file,
                                 Destination = Path.Combine(targetPath, Path.GetFileName(file))
                             });

                foreach (var file in files)
                {
                    File.Copy(file.Source, file.Destination);
                }
            }
        }

        /// <summary>
        /// Copy a directory from another excluding some
        /// </summary>
        public void DirectoryCopy(string sourceDirName, string destDirName, string[] excludedFolders)
        {
            // Get the subdirectories for the specified directory.
            DirectoryInfo dir = new DirectoryInfo(sourceDirName);

            if (!dir.Exists)
            {
                throw new DirectoryNotFoundException(
                    "Source directory does not exist or could not be found: "
                    + sourceDirName);
            }

            var dirs = dir.GetDirectories().Where(d => !isExcluded(excludedFolders.Select(x => x.ToLower()).ToList(), d.Name.ToLower()));

            // If the destination directory doesn't exist, create it.       
            Directory.CreateDirectory(destDirName);

            // Get the files in the directory and copy them to the new location.
            FileInfo[] files = dir.GetFiles();
            foreach (FileInfo file in files)
            {
                string tempPath = Path.Combine(destDirName, file.Name);
                file.CopyTo(tempPath, false);
            }

            // If copying subdirectories, copy them and their contents to new location.           
            foreach (DirectoryInfo subdir in dirs)
            {
                string tempPath = Path.Combine(destDirName, subdir.Name);
                DirectoryCopy(subdir.FullName, tempPath, excludedFolders);
            }
        }

        /// <summary>
        /// Returns all the file in the specificated directory
        /// </summary>
        /// <param name="searchFolder"> The directory where find the files </param>
        /// <param name="filters"> The extensions </param>
        /// <param name="isRecursive"> Specify whether the files must be searched in the sub directories</param>
        /// <example>
        /// new String[] { "jpg", "jpeg", "png", "gif", "tiff", "bmp", "svg" };
        /// var files = GetFilesFrom(searchFolder, filters, false);
        /// </example>
        public string[] GetFilesFrom(string searchFolder, string[] filters, bool isRecursive)
        {
            List<String> filesFound = new List<String>();
            var searchOption = isRecursive ? SearchOption.AllDirectories : SearchOption.TopDirectoryOnly;
            foreach (var filter in filters)
            {
                filesFound.AddRange(Directory.GetFiles(searchFolder, String.Format("*.{0}", filter), searchOption));
            }
            return filesFound.ToArray();
        }
    }
}
