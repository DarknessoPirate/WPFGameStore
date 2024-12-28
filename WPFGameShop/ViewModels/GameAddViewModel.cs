using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WPFGameShop.Repositories;
using WPFGameShop.Models;
using System.Windows;
using System.Windows.Input;
using Microsoft.Win32;
using System.IO;


namespace WPFGameShop.ViewModels
{
    public class GameAddViewModel : ViewModelBase
    {
        public GameAddViewModel(ref ShopViewModel shopVM) 
        {
            Games = RepositoryGame.GetAllGamesFromDb();
            _shopVM = shopVM;
        }
        #region properties
        private ObservableCollection<Game> _games;
        private string _gameName;
        private string _gamePrice;
        private string _coverImagePath;
        private Game _selectedGame;
        private ShopViewModel _shopVM;

        #endregion

        #region accessors
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

        public string GameName
        {
            get { return _gameName; }
            set
            {
                _gameName = value;
                onPropertyChanged(nameof(GameName));
            }
        }
        public string GamePrice
        {
            get { return _gamePrice; }
            set
            {
                _gamePrice = value;
                onPropertyChanged(nameof(GamePrice));
            }
        }

        public string CoverImagePath
        {
            get { return _coverImagePath; }
            set
            {
                _coverImagePath = value;
                onPropertyChanged(nameof(CoverImagePath));
            }
        }

        #endregion

        #region commands

        private ICommand addGameCommand;

        public ICommand AddGameCommand 
        {
            get
            {
                if (addGameCommand == null)
                    addGameCommand = new RelayCommand(
                            parameter => AddGame(),
                            predicate => GameName != null && GameName != "" &&
                                         GamePrice != null && GamePrice != "" &&
                                         CoverImagePath != null && CoverImagePath != ""
                        );
                return addGameCommand;
            }

        }
        private ICommand modifyGameCommand;
        public ICommand ModifyGameCommand
        {
            get
            {
                if (modifyGameCommand == null)
                    modifyGameCommand = new RelayCommand(
                        parameter => ModifyGame(),
                        predicate => SelectedGame != null && GameName != null && GameName != "" && 
                                     GamePrice != null && GamePrice != "" &&
                                    CoverImagePath != null && CoverImagePath != ""
                        );
                return modifyGameCommand;
            }
        }

        private ICommand deleteGameCommand;
        public ICommand DeleteGameCommand
        {
            get
            {
                if (deleteGameCommand == null)
                    deleteGameCommand = new RelayCommand(
                        parameter => DeleteGame(),
                        predicate => SelectedGame != null
                        );
                return deleteGameCommand;
            }
        }

        #endregion

        #region functions

        private void AddGame()
        {
            var decimalPrice = new Decimal();
            if (Decimal.TryParse(GamePrice, out decimalPrice))
            {

                var newGame = new Game
                {
                    Name = GameName,
                    Price = decimalPrice,
                    CoverImagePath = CoverImagePath
                };

                if (RepositoryGame.AddGameToDb(newGame))
                {
                    MessageBox.Show("Game Added to DB!");
                    Games.Add(newGame);
                    GameName = string.Empty;
                    GamePrice = string.Empty;
                    CoverImagePath = string.Empty;

                    _shopVM.RefreshGameList();
                }
                else
                {
                    MessageBox.Show("Failed to add game to DB");
                }
            }
            else
            {
                MessageBox.Show("Incorrect Price Format");
            }
        }

        private void DeleteGame()
        {
            if (RepositoryGame.DeleteGameFromDb(SelectedGame.Id))
            {
                Games.Remove(SelectedGame);
                MessageBox.Show("Removed Game from DB!");
                _shopVM.RefreshGameList();
            }
            else
            {
                MessageBox.Show("Failed to remove game from DB");
            }
        }

        private void ModifyGame()
        {
            var decimalPrice = new Decimal();
            if (Decimal.TryParse(GamePrice, out decimalPrice))
            {

                if (RepositoryGame.ModifyGameInDb(SelectedGame.Id, GameName, decimalPrice, CoverImagePath))
                {
                    int indexOfSelectedGame = Games.IndexOf(SelectedGame);
                    var newGame = new Game
                    {
                        Name = GameName,
                        Price = decimalPrice,
                        CoverImagePath = CoverImagePath
                    };
                    Games[indexOfSelectedGame] = newGame;
                    GameName = string.Empty;
                    GamePrice = string.Empty;
                    CoverImagePath = string.Empty;
                    _shopVM.RefreshGameList();
                }
                else
                {
                    MessageBox.Show("Failed to modify game in DB");
                }
            }
            else
            {
                MessageBox.Show("Incorrect price format");
            }
        }


        #endregion





    }



}
