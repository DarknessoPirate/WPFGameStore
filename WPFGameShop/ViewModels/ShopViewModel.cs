using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using WPFGameShop.Models;
using WPFGameShop.Repositories;
using WPFGameShop.Views;
using System;
using System.Collections.ObjectModel;
using System.Reflection.Metadata;
using System.Windows;
using System.Windows.Input;

namespace WPFGameShop.ViewModels
{
    public class ShopViewModel : ViewModelBase
    {
        public ShopViewModel(User user, ref HomeViewModel homeVM)
        {
            CurrentUser = user;
            CurrentMoney = CurrentUser.Money;
            Games = RepositoryGame.GetAllGamesFromDb();
            _homeVM = homeVM;
        }
        #region properties
        private User _currentUser;
        private decimal _currentMoney;
        private ObservableCollection<Game> _games;
        private Game _selectedGame;
        private HomeViewModel _homeVM;
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

        public decimal CurrentMoney
        {
            get { return _currentMoney; }
            set
            {
                _currentMoney = value;
                onPropertyChanged(nameof(CurrentMoney));
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

        public Game SelectedGame
        {
            get { return _selectedGame; }
            set
            {
                _selectedGame = value;
                onPropertyChanged(nameof(SelectedGame));
            }
        }
        #endregion

        #region commands



        #endregion
        private ICommand buyGameCommand;
        public ICommand BuyGameCommand
        {
            get
            {
                if (buyGameCommand == null)
                    buyGameCommand = new RelayCommand(
                    parameter => BuyGame(),
                    predicate => SelectedGame != null && CurrentUser.Money >= SelectedGame.Price
                    );
                return buyGameCommand;
            }
}
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


        #region functions

        public void BuyGame()
        {

            using (var db = new ShopDbContext())
            {
                var existingUserGame = db.UserGames
                .FirstOrDefault(ug => ug.UserId == CurrentUser.Id && ug.GameId == SelectedGame.Id);

                if (existingUserGame == null)
                {

                    foreach (var ug in db.UserGames.ToList())
                    {
                        db.Entry(ug).State = EntityState.Unchanged;
                    }
                    try
                    {
                        db.UserGames.Add(new UserGame { UserId = CurrentUser.Id, GameId = SelectedGame.Id });
                        db.SaveChanges();
                    }catch (Exception)
                    {
                        MessageBox.Show("You can't buy this game");
                        return;
                    }
                   
                    CurrentMoney -= SelectedGame.Price;
                    CurrentUser.Money = CurrentMoney;
                    _homeVM.RefreshBalance(CurrentMoney);
                    RepositoryUser.ModifyUserInDb(CurrentUser);
                }
                else
                {
                    MessageBox.Show("You have this game");
                }
            }
                         
        }

        public void RefreshGameList()
        {
            Games = RepositoryGame.GetAllGamesFromDb();
        }

        private void OpenGameWindow(object parameter)
        {
            var game = parameter as Game;
            if (game != null)
            {
                var GameWindow = new GameDetailsWindow();
                GameWindow.DataContext = new GameDetailsWindowViewModel(_currentUser, game);
                GameWindow.Show();
            }
        }


        #endregion


    }





}
