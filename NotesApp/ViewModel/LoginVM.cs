using NotesApp.Model;
using NotesApp.ViewModel.Commands;
using SQLite;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NotesApp.ViewModel
{
    public class LoginVM : INotifyPropertyChanged
    {
        private User user;

        public User User
        {
            get { return user; }
            set { 
                user = value;
                OnPropertyChanged("User");
            }
        }

        public RegisterCommand RegisterCommand { get; set; }

        public LoginCommand LoginCommand
        { get; set; }

        public ExistingAccountCommand ExistingAccountCommand { get; set; }

        public NoExistingAccountCommand NoExistingAccountCommand { get; set; }

        // This eventhandler will be used to close the LoginWindow. 
        public event EventHandler HasLoggedIn;
        public event PropertyChangedEventHandler PropertyChanged;

        private void OnPropertyChanged(string propName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propName));
        }

        public LoginVM()
        {
            RegisterCommand = new RegisterCommand(this);
            LoginCommand = new LoginCommand(this);
            ExistingAccountCommand = new ExistingAccountCommand(this);
            NoExistingAccountCommand = new NoExistingAccountCommand(this);
            User = new User();
        }

        public void Login()
        {
            using(SQLiteConnection conn = new SQLiteConnection(DatabaseHelper.dbFile))
            {
                conn.CreateTable<User>();

                var user = conn.Table<User>().Where(u => u.UserName == User.UserName).FirstOrDefault();

                if(user.Password == User.Password)
                {
                    App.UserId = user.Id.ToString();
                    // This fires the event that someone has logged in. 
                    HasLoggedIn(this, new EventArgs());
                }
            }
        }

        public void Register()
        {
            using(SQLiteConnection conn = new SQLiteConnection(DatabaseHelper.dbFile))
            {
                conn.CreateTable<User>();
                var isInserted = DatabaseHelper.Insert(User);

                if(isInserted)
                {
                    App.UserId = User.Id.ToString();
                    HasLoggedIn(this, new EventArgs());
                }
            }
        }

        public void ShowLoginStackPanel()
        {
            
        }

        public void ShowRegisterStackPanel()
        {

        }
    }
}
