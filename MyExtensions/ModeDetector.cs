using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyExtensions
{

    /// <summary>
    /// Used to detect the current build mode.
    /// </summary>
    /// <remarks>http://www.codeproject.com/Tips/321120/Csharp-Determining-whether-the-current-build-mode</remarks>
    public class ModeDetector
    {
        /// <summary>
        /// Gets a value indicating whether the assembly was built in debug mode.
        /// </summary>
        public virtual bool IsDebug
        {
            get
            {
                bool isDebug = false;

#if (DEBUG)
                isDebug = true;
#else
                                isDebug = false;
#endif

                return isDebug;
            }
        }

        /// <summary>
        /// Gets a value indicating whether the assembly was built in release mode.
        /// </summary>
        public bool IsRelease
        {
            get { return !IsDebug; }
        }
    }
}
