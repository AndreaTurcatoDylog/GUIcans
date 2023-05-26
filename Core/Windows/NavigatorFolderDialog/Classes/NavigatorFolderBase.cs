using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Media.Imaging;

namespace Core
{
    public abstract class NavigatorFolderBase: INavigatorFolder
    {
        #region Constants
        #endregion

        #region Members

        protected BitmapImage _FolderBitmapImage;

        #endregion

        #region Properties

        public abstract string RootName { get; }

        #endregion

        #region Constructor

        public NavigatorFolderBase()
        {
            // Create the folder image
            _FolderBitmapImage = Properties.Resources.Folder.ToBitmapImage();
        }

        #endregion

        #region Methods

        /// <summary>
        /// Get a subset of subfolders inside the specificated path
        /// </summary>
        public abstract IList<NavigationDialogItem> GetFolders(string path, CancellationToken token, ref bool error);

        /// <summary>
        /// Returns the Root in path
        /// </summary>
        public virtual string GetRoot(string currentPath) { return string.Empty; }

        /// <summary>
        /// Create a sub Path from 0 to limit.
        /// "0": position of first part of path
        /// "limit": position of the last part of sub path 
        /// </summary>
        public virtual string CreateSubPath(string currentPath, int limit) { return string.Empty; }

        /// <summary>
        /// Returns the Path going Up in the Tree 
        /// </summary>
        public virtual string GoUpInTreeFolder(string currentPath) { return string.Empty; }

        /// <summary>
        /// Returns the information about the path exists or not
        /// </summary>
        public virtual DirectoryExistsResult DirectoryExists(string path, CancellationToken cancellationToken)
        {
            return DirectoryExistsResult.NotExists;
        }

        #endregion
    }
}
