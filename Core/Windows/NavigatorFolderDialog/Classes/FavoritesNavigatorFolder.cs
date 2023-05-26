using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Core
{
    /// <summary>
    /// This class manages the Favorites items.
    /// Every item has a Friendly name and the associated path.
    /// This class:
    /// 1) Show a list of all created Favorites item
    /// 2) Permit to delete\modify an existing item
    /// 3) Permit to choose an existing item
    /// </summary>
    public class FavoritesNavigatorFolder : NavigatorFolderBase
    {
        #region Members

       
        #endregion

        #region Properties

        /// <summary>
        /// Get the RootName
        /// </summary>
        public override string RootName
        {
            get { return "SHORTCUT"; }
        }

        /// <summary>
        /// Get\Set the Favorite folders
        /// </summary>
        public IList<FavoriteFolder> FavoriteFolders { get; private set; }

        #endregion

        #region Constructor

        public FavoritesNavigatorFolder(IList<FavoriteFolder> favoriteFolders)
            : base()
        {
            FavoriteFolders = favoriteFolders;
        }

        #endregion

        #region Methods

        /// <summary>
        /// Returns the information about the path exists or not
        /// </summary>
        public override DirectoryExistsResult DirectoryExists(string path, CancellationToken token)
        {
            return DirectoryExistsResult.Exists;
        }

        /// <summary>
        /// Returns all the Favorite Folders. 
        /// The favorite folders are stored in json file specificated in the "path" parameter
        /// </summary>
        public override IList<NavigationDialogItem> GetFolders(string path, CancellationToken token, ref bool error)
        {
            IList<NavigationDialogItem> list = null;

            if (FavoriteFolders != null)
            {
                list = GetPaginatedSubFolders(FavoriteFolders, out error);
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
        private IList<NavigationDialogItem> GetPaginatedSubFolders(IList<FavoriteFolder> favoriteFolders, out bool error)
        {
            var result = new List<NavigationDialogItem>();
            error = false;

            try
            {
                if (favoriteFolders != null)
                {
                    foreach (var favoriteFolder in favoriteFolders)
                    {
                        // Create the item
                        result.Add(new NavigationDialogItem(_FolderBitmapImage, favoriteFolder.FriendlyName, favoriteFolder.Folder));
                    }
                }

                return result;
            }
            catch (Exception ex)
            {
                error = true;
                var messageBox = new CustomMessageBox(MessageBoxType.Error, ex.Message, MessageBoxButtonsType.OK);
                messageBox.ShowMessageBox();
            }

            return result;
        }

        #endregion
    }
}
