using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Core
{
    public interface INavigatorFolder
    {
        #region Properties

        string RootName {get;}

        #endregion

        #region Methods

        string GetRoot(string currentPath);

        IList<NavigationDialogItem> GetFolders(string path, CancellationToken token, ref bool error);
        string CreateSubPath(string currentPath, int limit);

        string GoUpInTreeFolder(string currentPath);

        DirectoryExistsResult DirectoryExists(string path, CancellationToken cancellationToken);

        #endregion
    }
}
