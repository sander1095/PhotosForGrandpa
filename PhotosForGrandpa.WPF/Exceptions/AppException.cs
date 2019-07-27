using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PhotosForGrandpa.WPF.Exceptions
{
    /// <summary>
    /// Used to show error dialogs and to avoid that a global exception handler 
    /// would swallow the exception and thus not display the error dialog
    /// </summary>
    public class AppException : Exception
    {
        public AppException()
        {
        }

        public AppException(string message) : base(message)
        {
        }
    }
}
