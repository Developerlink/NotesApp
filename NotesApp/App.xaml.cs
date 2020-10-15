using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;

namespace NotesApp
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        //public static string UserId = string.Empty;
        public static string UserId = "1";

    }

    // <a target="_blank" href="https://icons8.com/icons/set/moleskine">Moleskine icon</a> icon by <a target="_blank" href="https://icons8.com">Icons8</a>


    // <uc:NotebookControl Notebook="{Binding UpdateSourceTrigger=PropertyChanged}"/>
    // The above does not update after renaming a notebook. Notebook control not used until it has been solved
    // TODO: MVVM login
    // TODO: MVVM notes
    // Can't make anything MVVM...


    // TODO: Add crud to notes
    // TODO: Add short cut keys to app
}
