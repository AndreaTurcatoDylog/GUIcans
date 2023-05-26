using SimpleInjector;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Common
{
    public class HelperContainer
    {
        // This section is a wrapper for the IoC container
        private  Container _Container = new Container();

        /// <summary>
        /// Returns the Container
        /// </summary>
        public  Container GetContainer()
        {
            return _Container;
        }

        [System.Diagnostics.DebuggerStepThrough]
        public  TService GetInstance<TService>() where TService : class
        {
            return _Container.GetInstance<TService>();
        }
    }
}
