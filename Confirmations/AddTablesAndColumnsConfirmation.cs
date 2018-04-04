using EscapeDBUsage.UIClasses;
using EscapeDBUsage.ViewModels;
using Prism.Commands;
using Prism.Interactivity.InteractionRequest;
using Prism.Mvvm;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace EscapeDBUsage.Confirmations
{
    public class AddTablesAndColumnsConfirmation : Confirmation
    {

        public AddTablesAndColumnsConfirmation(DatabaseSchemaViewModel vm)
        {
            ViewModel = vm;
            SelectedSprint = vm.SelectedSprint;

            Ok = new DelegateCommand(() => DoOk());
            Cancel = new DelegateCommand(() => DoCancel());
        }

        private void DoCancel()
        {
            this.Confirmed = false;
        }

        private void DoOk()
        {
            this.Confirmed = true;
        }

        public ICommand Ok { get; private set; }
        public ICommand Cancel { get; private set; }

        public DatabaseSchemaViewModel ViewModel { get; set; }

        public UISprint SelectedSprint { get; set; }

    }
}
