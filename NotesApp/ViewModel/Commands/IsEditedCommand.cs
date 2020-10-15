using NotesApp.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace NotesApp.ViewModel.Commands
{
    public class IsEditedCommand : ICommand
    {
        public NotesVM VM { get; set; }

        public event EventHandler CanExecuteChanged;

        public IsEditedCommand(NotesVM vm)
        {
            VM = vm;
        }

        public bool CanExecute(object parameter)
        {
            return true;
        }

        public void Execute(object parameter)
        {
            var notebook = parameter as Notebook;
            VM.IsEdited(notebook);
        }
    }
}
