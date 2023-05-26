using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core
{
    public static class NavigatorFolderFactory
    {

        /// <summary>
        /// Returns an instance of the Navigator Folder by types
        /// </summary>
        public static INavigatorFolder Get(NavigatorFolderType rootItemType, string domain, IList<FavoriteFolder> favoriteFolders)
        {
            switch (rootItemType)
            {
                case NavigatorFolderType.ThisPc: return new PCNavigatorFolder();
                case NavigatorFolderType.Net: return new NetNavigatorFolder(domain, favoriteFolders);
                case NavigatorFolderType.Favorite: return new FavoritesNavigatorFolder(favoriteFolders);
                case NavigatorFolderType.GoTo: return new GoToNavigatorFolder();
                default: return new PCNavigatorFolder(); 
            }
        }

        /// <summary>
        /// Returns the Navigator Folder Type for the specificated path
        /// </summary>
        public static NavigatorFolderType GetNavigatorTypeFromPath(string path)
        {
            if (string.IsNullOrEmpty(path))
            {
                return NavigatorFolderType.None;
            }
            else if(path[0]=='\\')
            {
                return NavigatorFolderType.Net;
            }
            else
            {
                return NavigatorFolderType.ThisPc;
            }
        }

        /// <summary>
        /// Returns the Navigator Folder byr the specificated path
        /// </summary>
        public static INavigatorFolder GetByPath(string path, string domain, IList<FavoriteFolder> favoriteFolders)
        {
            var type = NavigatorFolderType.None;

            if (!string.IsNullOrEmpty(path))
            {
                if (path[0] == '\\')
                {
                    type = NavigatorFolderType.Net;
                }
                else
                {
                    type = NavigatorFolderType.ThisPc;
                }
            }

            return Get(type, domain, favoriteFolders);
        }
    }
}
