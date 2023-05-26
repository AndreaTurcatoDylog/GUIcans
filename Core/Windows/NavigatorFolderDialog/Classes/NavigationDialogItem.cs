using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media.Imaging;

namespace Core
{
    public class NavigationDialogItem: INotifyPropertyChanged
    {
        #region Members

        private string _Name;
        private string _FolderFullPath;

        #endregion

        #region Properties

        /// <summary>
        /// Get\Set the Image
        /// </summary>
        public BitmapImage Image { get; set; }

        /// <summary>
        /// Get\Set the Name
        /// </summary>
        public string Name
        {
            get { return _Name; }
            set
            {
                _Name = value;
                NotifyPropertyChanged("Name");
            }
        }

        /// <summary>
        /// Get\Set the FolderFullPath
        /// </summary>
        public string FolderFullPath
        {
            get { return _FolderFullPath; }
            set
            {
                _FolderFullPath = value;
                NotifyPropertyChanged("FolderFullPath");
            }
        }

        #endregion

        #region Constructor

        public NavigationDialogItem(BitmapImage image, string name, string folderFullPath)
        {
            Image = image;
            FolderFullPath = folderFullPath;
            Name = name;
        }

        #endregion

        #region INotifyPropertyChanged Impelementations

        public event PropertyChangedEventHandler PropertyChanged;
        private void NotifyPropertyChanged(string property)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(property));
            }
        }

        #endregion
    }

}
