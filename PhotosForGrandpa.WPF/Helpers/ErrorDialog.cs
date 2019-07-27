using PhotosForGrandpa.WPF.Exceptions;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace PhotosForGrandpa.WPF.Helpers
{    
    public static class ErrorDialog
    {
        [DebuggerHidden]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void ShowError(string message)
        {
            throw new ErrorDialogException(message);
        }
    }
}
