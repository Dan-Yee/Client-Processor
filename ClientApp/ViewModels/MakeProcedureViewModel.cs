using Avalonia.Controls.Selection;
using ClientApp.Models;
using ClientApp.Views;
using Grpc.Net.Client;
using ReactiveUI;
using Server;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClientApp.ViewModels
{
    public class MakeProcedureViewModel : ViewModelBase
    {
        //View that this viewmodel is attached to
        private MakeProcedureView _makeProcedureView;
        //Id of the client
        private int _clientId;

        /// <summary>
        /// Constructor of the view model. Initializes the view and the client id
        /// </summary>
        /// <param name="mpv"></param>
        /// <param name="c_ID"></param>
        public MakeProcedureViewModel(MakeProcedureView mpv, int c_ID)
        {
            _makeProcedureView = mpv;
            _clientId = c_ID;
        }

        public void GoToFormMenu()
        {
            new FormMenuView(_clientId).Show();
            _makeProcedureView.Close();
        }
    }
}