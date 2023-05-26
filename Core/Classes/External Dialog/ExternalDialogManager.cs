using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core
{
    /// <summary>
    /// The mediator class for the External Dialogs.
    /// Permits to open a Dialog
    /// It is a Singleton
    /// </summary>
    public class ExternalDialogManager
    {
        #region Members

        private static ExternalDialogManager _Instance;
        protected IDictionary<int, Action<IDialogViewModel>> _Dictionary;

        #endregion

        #region Properties

        public static ExternalDialogManager Instance
        {
            get
            {
                if (_Instance == null)
                {
                    _Instance = new ExternalDialogManager();
                }

                return _Instance;
            }
        }

        #endregion

        #region Constructor

        public ExternalDialogManager()
        {
            _Dictionary = new Dictionary<int, Action<IDialogViewModel>>();
        }

        #endregion

        #region Methods

        /// <summary>
        /// Subscribe the key with callback
        /// </summary>
        /// <param name="key"> The key of the external dialog which must be opened  </param>
        /// <param name="callback"> The callback with a parameter associated with token </param>
        public void Subscribe(int key, Action<IDialogViewModel> callback)
        {
            if (!_Dictionary.ContainsKey(key))
            {               
                _Dictionary.Add(key, callback);
            }
        }

        /// <summary>
        /// Unsubscribe the key by callback
        /// </summary>
        /// <param name="key"> The key of the external dialog </param>
        /// <param name="callback"> The callbak associated with token </param>

        public void Unsubscribe(int key, Action<IDialogViewModel> callback)
        {
            if (_Dictionary.ContainsKey(key))
            {
                _Dictionary.Remove(key);
            }
        }

        /// Call all callback associated to the token.
        /// </summary>
        /// <param name="pageKey"> The key of the external dialog which must be opened  </param>
        /// <param name="args">The args of the callback</param>
        public void Notify(int key, IDialogViewModel dialogViewModel)
        {
            if (_Dictionary.ContainsKey(key))
            {
                _Dictionary[key]?.Invoke(dialogViewModel);
            }
        }

        /// <summary>
        /// Send a message to open the External Dialog associated to the specificated ViewModel
        /// </summary>
        public void OpenDialog(IDialogViewModel dialogViewModel)
        {
            var externalDialogMessage = new ExternalDialogMessage()
            {
                ViewModel = dialogViewModel
            };

            // Send a message to open the External Dialog
            Notify(dialogViewModel.DialogServiceKey, dialogViewModel);
        }

        #endregion
    }
}
