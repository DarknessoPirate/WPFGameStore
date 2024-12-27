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
using static Azure.Core.HttpHeader;

namespace WPFGameShop.ViewModels
{
    public class GameDetailsWindowViewModel : ViewModelBase
    {
        public GameDetailsWindowViewModel(User user, Game game)
        {
            _currentGame = game;
            _currentUser = user;
            _reviews = RepositoryReview.GetGameReviews(_currentGame);
            _ratings = [1, 2, 3, 4, 5];
            _selectedRating = 0;
        }

        #region properties
        private User _currentUser;
        private Game _currentGame;
        private ObservableCollection<Review> _reviews { get; set; }
        private string _reviewContent;
        private int[] _ratings;
        private int _selectedRating;
        private Review _selectedReview;
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

        public Game CurrentGame
        {
            get { return _currentGame; }
            set
            {
                _currentGame = value;
                onPropertyChanged(nameof(CurrentGame));
            }
        }


        public ObservableCollection<Review> Reviews
        {
            get { return _reviews; }
            set
            {
                _reviews = value;
                onPropertyChanged(nameof(Reviews));
            }
        }


        public Review SelectedReview
        {
            get { return _selectedReview; }
            set
            {
                _selectedReview = value;
                onPropertyChanged(nameof(SelectedReview));
            }
        }

        public string ReviewContent
        {
            get { return _reviewContent; }
            set
            {
                _reviewContent = value;
                onPropertyChanged(nameof(ReviewContent));
            }
        }

        public int[] Ratings
        {
            get { return _ratings; }
            set
            {
                _ratings = value;
                onPropertyChanged(nameof(Ratings));
            }
        }

        public int SelectedRating
        {
            get { return _selectedRating; }
            set
            {
                _selectedRating = value;
                onPropertyChanged(nameof(SelectedRating));
            }
        }


        #endregion
        #region commands
        private ICommand submitReviewCommand;
        public ICommand SubmitReviewCommand
        {
            get
            {
                if (submitReviewCommand == null)
                    submitReviewCommand = new RelayCommand(
                            parameter => AddReview(),
                            predicate => ReviewContent != "" && ReviewContent != null &&
                                         CurrentGame != null &&
                                         SelectedRating >= 0 && SelectedRating <= 5
                        );
                return submitReviewCommand;
            }
        }

        #endregion

        #region functions
        private void AddReview()
        {
            if (!string.IsNullOrEmpty(ReviewContent))
            {
                var newReview = new Review
                {
                    Content = ReviewContent,
                    Score = SelectedRating,
                    UserId = CurrentUser.Id,
                    GameId = CurrentGame.Id
                };

                if (RepositoryGame.GetUserGamesFromDb(CurrentUser.Id).FirstOrDefault(g => g.Id == CurrentGame.Id) != null)
                {

                    if (RepositoryReview.AddReviewToDb(newReview))
                    {
                        ReviewContent = string.Empty;
                        Reviews = RepositoryReview.GetGameReviews(_currentGame);
                        SelectedRating = 0;
                    }

                    else
                    {
                        MessageBox.Show("You've already reviewed this game");
                    }

                }

                else
                {
                    MessageBox.Show("You don't have this game");
                }
               
            }

        }

        #endregion
    }
}
