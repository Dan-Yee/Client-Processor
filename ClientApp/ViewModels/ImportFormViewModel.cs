using Avalonia.Controls;
using ReactiveUI;
using System;
using ClientApp.Views;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Server;
using System.Collections.ObjectModel;

namespace ClientApp.ViewModels
{
    public class ImportFormViewModel : ReactiveObject
    {
        //View that this viewmodel is attached to
        ImportFormView _importFormView;
        //Holds the paths of the selected files
        private string _filePaths;
        
        public string FilePaths
        {
            get => _filePaths;
            set
            {
                this.RaiseAndSetIfChanged(ref _filePaths, value);
            }
        }
        /// <summary>
        /// Constructor for view model. Initializes view
        /// </summary>
        /// <param name="ifv"></param>
        public ImportFormViewModel(ImportFormView ifv)
        {
            _importFormView = ifv;
            FilePaths = "path";
        }

        /// <summary>
        /// Handles importing files
        /// </summary>
        public async void Files()
        {

            var wind = new Window();
            var ofd = new OpenFileDialog();
            // ofd.InitialDirectory = @"C:\Users";
            //ofd.AllowMultiple = true;
            var y = await ofd.ShowAsync(wind);
            if (y != null)
            {
                //FilePaths = "change";
                FilePaths = y[0];
                /*
                foreach (var path in y)
                {
                    Console.WriteLine(path);
                    var filename = System.IO.Path.GetFileName(path);

                }
                */
                //Console.WriteLine(y[0]);
            }
        }
    }
}
