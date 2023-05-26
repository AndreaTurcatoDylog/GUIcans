using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Core
{
    public class GoToNavigatorFolder: NavigatorFolderBase
    {
        #region Properties

        public override string RootName
        {
            get { return "GOTO"; }
        }

        #endregion

        #region Methods

        /// <summary>
        /// Get a subset of subfolders inside the specificated path
        /// </summary>
        public override IList<NavigationDialogItem> GetFolders(string path, CancellationToken token, ref bool error)
        {
            return null;
        }

        #endregion
    }
}
