using NotesApp.Model;
using NotesApp.ViewModel.Commands;
using SQLite;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NotesApp.ViewModel
{
    public class NotesVM : INotifyPropertyChanged
    {
        public List<double> FontSizes { get; set; }

        private bool isEditing;

        public bool IsEditing
        {
            get { return isEditing; }
            set
            {
                isEditing = value;
                OnPropertyChanged("IsEditing");
            }
        }

        private bool isEditingNote;

        public bool IsEditingNote
        {
            get { return isEditingNote; }
            set
            {
                isEditingNote = value;
                OnPropertyChanged("IsEditingNote");
            }
        }


        public ObservableCollection<Notebook> Notebooks { get; set; }

        private Notebook selectedNotebook;

        public Notebook SelectedNotebook
        {
            get { return selectedNotebook; }
            set
            {
                selectedNotebook = value;
                OnPropertyChanged("SelectedNotebook");
                ReadNotes();
            }
        }

        public ObservableCollection<Note> Notes { get; set; }

        private Note selectedNote;

        public Note SelectedNote
        {
            get { return selectedNote; }
            set
            {
                selectedNote = value;
                OnPropertyChanged("SelectedNote");
                SelectedNoteChanged(this, new EventArgs());
            }
        }

        public NewNotebookCommand NewNotebookCommand { get; set; }

        public NewNoteCommand NewNoteCommand { get; set; }

        public DeleteNotebookCommand DeleteNotebookCommand { get; set; }

        public BeginEditCommand BeginEditCommand { get; set; }

        public IsEditedCommand IsEditedCommand { get; set; }

        public BeginEditNoteCommand BeginEditNoteCommand { get; set; }

        public IsEditedNoteCommand IsEditedNoteCommand { get; set; }

        public NotesVM()
        {
            IsEditing = false;
            IsEditingNote = false;
            NewNotebookCommand = new NewNotebookCommand(this);
            NewNoteCommand = new NewNoteCommand(this);
            DeleteNotebookCommand = new DeleteNotebookCommand(this);
            BeginEditCommand = new BeginEditCommand(this);
            IsEditedCommand = new IsEditedCommand(this);
            BeginEditNoteCommand = new BeginEditNoteCommand(this);
            IsEditedNoteCommand = new IsEditedNoteCommand(this);

            Notebooks = new ObservableCollection<Notebook>();
            Notes = new ObservableCollection<Note>();
            FontSizes = new List<double>() { 8, 9, 10, 11, 12, 14, 16, 28, 48, 72 };

            ReadNotebooks();
            ReadNotes();
        }

        public void CreateNotebook()//int userId)
        {
            int userId;
            int.TryParse(App.UserId, out userId);

            var newNotebook = new Notebook()
            {
                Name = "New notebook",
                UserId = userId
            };

            DatabaseHelper.Insert(newNotebook);
            ReadNotebooks();
        }

        public void CreateNote(int notebookId)
        {
            var newNote = new Note()
            {
                NotebookId = notebookId,
                CreatedTime = DateTime.Now,
                UpdatedTime = DateTime.Now,
                Title = "The elephant sleeps and the tiger snoors."
            };

            DatabaseHelper.Insert(newNote);
            ReadNotes();
        }

        public void DeleteNotebook()
        {
            // Deleting all associated notes to the notebook first.
            foreach (var note in Notes)
            {
                if (note.NotebookId == SelectedNotebook.Id)
                    DatabaseHelper.Delete(note);
            }
            // Deleting the notebook itself.
            DatabaseHelper.Delete(SelectedNotebook);            
            // Unselect notebook and refresh list of notebooks and notes.
            SelectedNotebook = null;
            ReadNotebooks();
            ReadNotes();
        }

        public void ReadNotebooks()
        {
            using (SQLiteConnection conn = new SQLiteConnection(DatabaseHelper.dbFile))
            {
                conn.CreateTable<Notebook>();
                var notebooks = conn.Table<Notebook>().ToList();
                Notebooks.Clear();
                foreach (var notebook in notebooks)
                {
                    Notebooks.Add(notebook);
                }
            }
        }

        public void ReadNotes()
        {
            using (SQLiteConnection conn = new SQLiteConnection(DatabaseHelper.dbFile))
            {
                if (SelectedNotebook != null)
                {
                    conn.CreateTable<Note>();
                    var notes = conn.Table<Note>().Where(n => n.NotebookId == selectedNotebook.Id).ToList();
                    Notes.Clear();
                    foreach (var note in notes)
                    {
                        Notes.Add(note);
                    }
                }
                else
                {
                    Notes.Clear();
                }
            }
        }

        public void StartEditing()
        {
            IsEditing = true;
        }

        public void StartEditingNote()
        {
            IsEditingNote = true;
        }

        public void IsEdited(Notebook notebook)
        {
            if (notebook != null)
            {
                DatabaseHelper.Update(notebook);
                IsEditing = false;
                ReadNotebooks();
            }
        }

        public void IsEditedNote(Note note)
        {
            if (SelectedNote != null)
            {
                DatabaseHelper.Update(note);
                IsEditingNote = false;
                ReadNotes();
            }
        }

        public void UpdateSelectedNote()
        {
            DatabaseHelper.Update(SelectedNote);
        }

        public event EventHandler SelectedNoteChanged;
        public event PropertyChangedEventHandler PropertyChanged;

        private void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }



    }
}
