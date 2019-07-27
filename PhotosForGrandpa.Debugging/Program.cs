using PhotosForGrandpa.WPF.Exceptions;
using PhotosForGrandpa.WPF.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PhotosForGrandpa.Debugging
{
    /// <summary>
    /// Because the WPF project generates an .exe by using "start-app" in the package manager console, the code itself can't be easily debugged.
    /// This project allows you to create instances of your viewmodels and perform actions upon it the same way your UI would so you can use the debugger.
    /// </summary>
    internal class Program
    {
        static void Main(string[] args)
        {
            var viewModel = new OrganizerViewModel()
            {
                FolderName = "PhotosForGrandpa"
            };

            try
            {
                viewModel.Organiseer();
            }
            catch (Exception e) when (!(e is ErrorDialogException))
            {
                throw;
            }
        }
    }
}
