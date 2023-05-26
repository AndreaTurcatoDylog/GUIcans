using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Common
{
    public class HelperResources
    {
        /// <summary>
        /// Returns the Image Resurces by name stored in the Common.
        /// </summary>
        public Bitmap GetResourceImageByName(string imageName)
        {
            try
            {
                System.Reflection.Assembly asm = System.Reflection.Assembly.GetExecutingAssembly();
                string resourceName = asm.GetName().Name + ".Properties.Resource";
                var rm = new System.Resources.ResourceManager(resourceName, asm);

                Bitmap bitmap = (Bitmap)rm.GetObject(imageName);

                return (Bitmap)rm.GetObject(imageName);
            }
            catch (Exception)
            {
                return null;
            }
        }        
    }
}
