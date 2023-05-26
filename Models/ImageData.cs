using Core;
using Newtonsoft.Json;
using System;
using System.ComponentModel;
using System.Windows.Media.Imaging;

namespace Models
{
    /// <summary>
    /// This class rappresent the Image data.
    /// Contains:
    /// 1) FileName
    /// 2) The Cause
    /// 3) The Action
    /// </summary>
    public class ImageData: ModelBase
    {
        #region Members

        private bool _IsSelected;
        private WriteableBitmap _WriteableBitmapImage;

        #endregion

        #region Properties

        [JsonIgnore]
        public WriteableBitmap WriteableBitmapImage
        {
            get { return _WriteableBitmapImage; }
            set
            {
                _WriteableBitmapImage = value;
                OnPropertyChanged("WriteableBitmapImage");
            }
        }

        [JsonIgnore]
        public string PathFileName { get; set; }

        [JsonIgnore]
        public double RotateAngle { get; set; }

        [JsonIgnore]
        public bool IsSelected {
            get { return _IsSelected; }
            set
            {
                _IsSelected = value;
                OnPropertyChanged("IsSelected");
            }
        }

        [JsonProperty("FileName")]
        public string FileName { get;  set; }

        [JsonProperty("TimeStamp")]
        public DateTime TimeStamp { get; set; }

        [JsonProperty("Cause")]
        public string Cause { get;  set; }

        [JsonProperty("Action")]
        public string Action { get;  set; }

        #endregion

        #region Constructor

        public ImageData(WriteableBitmap writeableBitmapImage, double rotateAngle, string pathFileName, DateTime timeStamp, string cause, string action)
        {
            WriteableBitmapImage = writeableBitmapImage;
            RotateAngle = rotateAngle;
            PathFileName = pathFileName;
            FileName = System.IO.Path.GetFileName(pathFileName);
            TimeStamp = timeStamp;
            Cause = cause;
            Action = action;
        }

        #endregion        
    }
}
