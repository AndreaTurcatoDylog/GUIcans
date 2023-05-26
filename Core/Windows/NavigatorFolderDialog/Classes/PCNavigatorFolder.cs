using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Core
{
    public class PCNavigatorFolder: NavigatorFolderBase
    {
        #region Properties

        public override string RootName
        {
            get { return "PC"; }
        }

        #endregion

        #region Methods

        /// <summary>
        /// Returns the information about the path exists or not
        /// </summary>
        public override DirectoryExistsResult DirectoryExists(string path, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();

            if ((!string.IsNullOrEmpty(path) && Directory.Exists(path)))
            {
                return DirectoryExistsResult.Exists;
            }

            cancellationToken.ThrowIfCancellationRequested();

            return DirectoryExistsResult.NotExists;
        }

        /// <summary>
        /// Returns the Root in path
        /// </summary>
        public override string GetRoot(string currentPath)
        {
            if (string.IsNullOrEmpty(currentPath))
            {
                return Path.GetPathRoot(currentPath);
            }

            return string.Empty;
        }

        /// <summary>
        /// Returns all the Sub Folders
        /// </summary>
        public override IList<NavigationDialogItem> GetFolders(string path, CancellationToken token, ref bool error)
        {
            IList<NavigationDialogItem> list = null;

            token.ThrowIfCancellationRequested();

            // Determines whether the path is ThisPc or not
            var isThisPc = string.IsNullOrEmpty(path);
            if (isThisPc)
            {
                token.ThrowIfCancellationRequested();

                list = GetPaginatedDrives(out error);

                token.ThrowIfCancellationRequested();
            }
            else if (!string.IsNullOrEmpty(path))
            {
                token.ThrowIfCancellationRequested();

                var rootDirectory = new DirectoryInfo(path);
                list = GetPaginatedSubFolders(rootDirectory, out error);

                token.ThrowIfCancellationRequested();
            }

            if (!error && list != null)
            {
                return list;
            }

            return null;
        }

        /// <summary>
        /// Returns a paginated list of Sub folders
        /// </summary>
        private IList<NavigationDialogItem> GetPaginatedSubFolders(DirectoryInfo root, out bool error)
        {
            var result = new List<NavigationDialogItem>();
            error = false;

            try
            {
                if (root != null)
                {
                    DirectoryInfo[] subDirs = null;

                    // Find all the subdirectories under this directory.
                    subDirs = root.GetDirectories();
                    foreach (var dirInfo in subDirs)
                    {
                        // Create the item
                        result.Add(new NavigationDialogItem(_FolderBitmapImage, dirInfo.Name, dirInfo.FullName));
                    }
                }

                return result;
            }
            catch(UnauthorizedAccessException ex)
            {
                error = true;
                CoreLog.Instance.Append($"[{ex.Source}] - {ex.Message} - {ex.StackTrace}", CoreLogType.Error);
            }
            catch (Exception ex)
            {
                error = true;
                CoreLog.Instance.Append($"[{ex.Source}] - {ex.Message} - {ex.StackTrace}", CoreLogType.Error);
            }

            return result;
        }

        /// <summary>
        /// Returns a paginated list of Drives
        /// </summary>
        private IList<NavigationDialogItem> GetPaginatedDrives(out bool error)
        {
            var result = new List<NavigationDialogItem>();
            error = false;

            try
            {
                DriveInfo[] allDrives = DriveInfo.GetDrives();

                foreach (DriveInfo drive in allDrives)
                {
                    result.Add(new NavigationDialogItem(_FolderBitmapImage, drive.Name, drive.Name));
                }

                return result;
            }
            catch (Exception)
            {
                error = true;
            }

            return result;
        }

        /// <summary>
        /// Create a sub Path from 0 to limit.
        /// "0": position of first part of path
        /// "limit": position of the last part of sub path 
        /// </summary>
        public override string CreateSubPath(string currentPath, int limit)
        {
            var subPath = string.Empty;

            // Update the CurrentPath
            if (!string.IsNullOrEmpty(currentPath))
            {
                var splittedString = currentPath.Split('\\');
                splittedString = splittedString.Where(s => !string.IsNullOrEmpty(s)).ToArray();

                if (splittedString.Count() > 0)
                {
                    splittedString[0] = $@"{splittedString[0]}\";
                    for (var index = 0; index < limit; index++)
                    {
                        subPath = System.IO.Path.Combine(subPath, splittedString[index]);
                    }
                }
            }

            return subPath;
        }

        /// <summary>
        /// Returns the Path going Up in the Tree 
        /// </summary>
        public override string GoUpInTreeFolder(string currentPath)
        {
            if (!string.IsNullOrEmpty(currentPath))
            {
                return System.IO.Path.GetFullPath(System.IO.Path.Combine(currentPath, @"..\"));
            }

            return string.Empty;
        }

        #endregion
    }
}
