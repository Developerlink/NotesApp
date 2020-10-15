using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace NotesApp.ViewModel.Commands
{
    public class ExistingAccountCommand : ICommand
    {
        public LoginVM VM { get; set; }

        public event EventHandler CanExecuteChanged;

        public ExistingAccountCommand(LoginVM vm)
        {
            VM = VM;
        }
        public bool CanExecute(object parameter)
        {
            return true;
        }

        public void Execute(object parameter)
        {
            // TODO
        }
    }
}
