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


    //x TODO: MVVM login
    //x TODO: MVVM notes
    // Can't make anything MVVM...
    //x TODO: Add crud to notes
    // TODO: Add ctrl + n to create new notebook
    // TODO: Add tab + down to jump to notebooklist
    // TODO: Add escape to exit program

    // TODO: Add tab + right to jump to notelist if notebook is selected
    // TODO: Add F2 to rename
    // TODO: Add Enter key to rename notebook
    // TODO: Add delete to delete notebook
    // TODO: Add ctrl + n to create new note

    // TODO: Add F2 to rename (from both notelist and text editor)
    // TODO: Add Enter to jump into note text editor
    // TODO: Add ctrl + s to save changes to note
    // TODO: Add delete to delete notebook


    //!? <uc:NotebookControl Notebook="{Binding UpdateSourceTrigger=PropertyChanged}"/>
    //!? The above does not update after renaming a notebook. Notebook control not used until it has been solved
    //!? Rename for notebook requires a left mouseclick first if using the mouse, before the right click. This is not optimal. 

}
