using WPFGameShop.Models;
using WPFGameShop.Repositories;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.StartPanel;
using System.Windows.Input;
using System.Windows;
using WPFGameShop.Views;
using System.Windows.Controls;

namespace WPFGameShop.ViewModels
{
    public class ProfileViewModel : ViewModelBase
    {

        public ProfileViewModel(User user, ObservableCollection<TabItem> tabs)
        {
            _currentUser = user;
            _games = RepositoryGame.GetUserGamesFromDb(_currentUser.Id);
            _tabs = tabs;
        }

        #region properties
        private User _currentUser;
        private Game _selectedGame;
        private ObservableCollection<Game> _games;
        private ObservableCollection<TabItem> _tabs;
        private int _visible = 0;
        private string _userNameToDelete;
        #endregion

        #region accessors
        public User CurrentUser
        {
            get { return _currentUser; }
            set
            {
                _currentUser = value;
                onPropertyChanged(nameof(CurrentUser));
            }
        }

        public ObservableCollection<TabItem> Tabs
        {
            get { return _tabs; }
            set
            {
                _tabs = value;
                onPropertyChanged(nameof(Tabs));
            }
        }

        public Game SelectedGame
        {
            get { return _selectedGame; }
            set
            {
                _selectedGame = value;
                onPropertyChanged(nameof(SelectedGame));
            }
        }

        public ObservableCollection<Game> Games
        {
            get { return _games; }
            set
            {
                _games = value;
                onPropertyChanged(nameof(Games));
            }
        }

        public string UserNameToDelete
        {
            get { return _userNameToDelete; }
            set
            {
                _userNameToDelete = value;
                onPropertyChanged(nameof(UserNameToDelete));
            }
        }

        public int Visible
        {
            get { return _visible; }
            set
            {
                _visible = value;
                onPropertyChanged(nameof(Visible));
            }
        }

        #endregion
        #region commands
      
        private ICommand openGameWindowCommand;
        public ICommand OpenGameWindowCommand
        {
            get
            {
                if (openGameWindowCommand == null)
                    openGameWindowCommand = new RelayCommand(
                       parameter => OpenGameWindow(parameter),
                       predicate => true
                       );
                return openGameWindowCommand;
            }
        }

        private ICommand refreshGamesCommand;
        public ICommand RefreshGamesCommand
        {
            get
            {
                if (refreshGamesCommand == null)
                    refreshGamesCommand = new RelayCommand(
                       parameter => RefreshGames(),
                       predicate => true
                       );
                return refreshGamesCommand;
            }
        }

        private ICommand openMenuCommand;
        public ICommand OpenMenuCommand
        {
            get
            {
                if (openMenuCommand == null)
                    openMenuCommand = new RelayCommand(
                       parameter => OpenMenu(),
                       predicate => true
                       );
                return openMenuCommand;
            }
        }

        private ICommand closeMenuCommand;
        public ICommand CloseMenuCommand
        {
            get
            {
                if (closeMenuCommand == null)
                    closeMenuCommand = new RelayCommand(
                       parameter => CloseMenu(),
                       predicate => true
                       );
                return closeMenuCommand;
            }
        }

        private ICommand logoutCommand;
        public ICommand LogoutCommand
        {
            get
            {
                if (logoutCommand == null)
                    logoutCommand = new RelayCommand(
                       parameter => Logout(),
                       predicate => true
                       );
                return logoutCommand;
            }
        }

        private ICommand deleteAccountCommand;
        public ICommand DeleteAccountCommand
        {
            get
            {
                if (deleteAccountCommand == null)
                    deleteAccountCommand = new RelayCommand(
                       parameter => DeleteAccount(),
                       predicate => UserNameToDelete == CurrentUser.Name
                       );
                return deleteAccountCommand;
            }
        }

        #endregion

        #region functions

        private void OpenGameWindow(object parameter)
        {
            var game = parameter as Game;
            if (game != null)
            {
                var GameWindow = new GameDetailsWindow();
                GameWindow.DataContext = new GameDetailsWindowViewModel(CurrentUser, game);
                GameWindow.Show();
            }
        }

        private void RefreshGames()
        {
            Games = RepositoryGame.GetUserGamesFromDb(CurrentUser.Id);
        }

        private void OpenMenu()
        {
            Visible = 1;
        }

        private void CloseMenu()
        {
            Visible = 0;
        }

        private void Logout()
        {
            Tabs.Clear();
        }

        private void DeleteAccount()
        {
            if (RepositoryUser.DeleteUserFromDb(CurrentUser.Id))
            {
                Tabs.Clear();
                MessageBox.Show($"Account {CurrentUser.Name} has been deleted");
            }

        }

        #endregion

    }
}
