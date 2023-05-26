using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net;
using System.Runtime.InteropServices;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using SimpleInjector;

namespace Common
{
    /// <summary>
    /// This is a Global Helper class:
    /// 1) contains is a wrapper for the IoC container
    /// 2) contains the resources manager
    /// 3) The helper Folders and the File methods
    /// 4) The helper Networks methods
    /// </summary>
    public static class Helper
    {
        public static HelperResources HelperResources = new HelperResources();
        public static HelperContainer HelperContainer = new HelperContainer();
        public static HelperFolderAndFiles HelperFolderAndFiles = new HelperFolderAndFiles();
        public static HelperNetwork HelperNetwork = new HelperNetwork();
        public static HelperBitmapImage HelperBitmapImage = new HelperBitmapImage();
    }
}
