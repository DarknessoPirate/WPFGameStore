using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WPFGameShop.Models;
using WPFGameShop.Repositories;
using System.Windows;
using System.Windows.Input;
using System.Collections.ObjectModel;
using WPFGameShop.Views;
using System.Reflection.Metadata;
namespace WPFGameShop.ViewModels
{
    public class ReviewsViewModel : ViewModelBase
    { 
        public ReviewsViewModel(User user)
        {
            _currentUser = user;
            _reviews = RepositoryReview.GetAllReviews();
        }

        #region properties
        private User _currentUser;
        private Review _selectedReview;
        private ObservableCollection<Review> _reviews;    
        private string _reviewAuthor;
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


        public Review SelectedReview
        {
            get { return _selectedReview; }
            set
            {           
                _selectedReview = value;
                onPropertyChanged(nameof(SelectedReview));
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


        public string ReviewAuthor
        {
            get { return _reviewAuthor; }
            set 
            { 
                _reviewAuthor = value;
                onPropertyChanged(nameof(ReviewAuthor));
            }
        }

        #endregion

        #region commands
       

        private ICommand deleteReviewCommand;
        public ICommand DeleteReviewCommand
        {
            get
            {
                if (deleteReviewCommand == null)
                    deleteReviewCommand = new RelayCommand(
                        parameter => DeleteReview(),
                        predicate => SelectedReview != null
                        );
                return deleteReviewCommand;
            }
        }
        private ICommand yourReviewsCommand;
        public ICommand YourReviewsCommand
        {
            get
            {
                if (yourReviewsCommand == null)
                    yourReviewsCommand = new RelayCommand(
                        parameter => ShowCurrentUserReviews(),
                        predicate => true
                        );
                return yourReviewsCommand;
            }
        }

        private ICommand allReviewsCommand;
        public ICommand AllReviewsCommand
        {
            get
            {
                if (allReviewsCommand == null)
                    allReviewsCommand = new RelayCommand(
                        parameter => ShowAllReviews(),
                        predicate => true
                        );
                return allReviewsCommand;
            }
        }

        private ICommand reviewAuthorReviewsCommand;
        public ICommand ReviewAuthorReviewsCommand
        {
            get
            {
                if (reviewAuthorReviewsCommand == null)
                    reviewAuthorReviewsCommand = new RelayCommand(
                        parameter => ShowAuthorFromTextBoxReviews(),
                        predicate => ReviewAuthor != ""
                        );
                return reviewAuthorReviewsCommand;
            }
        }

        private ICommand openReviewWindowCommand;
        public ICommand OpenReviewWindowCommand
        {
            get
            {
                if (openReviewWindowCommand == null)
                    openReviewWindowCommand = new RelayCommand(
                        parameter => OpenReviewWindow(parameter),
                        predicate => true
                        );
                return openReviewWindowCommand;
            }
        }


        #endregion

        #region functions


        private void DeleteReview()
        {
            if (SelectedReview != null && SelectedReview.UserId == CurrentUser.Id)
            {
                if (RepositoryReview.DeleteReviewFromDb(SelectedReview.Id))
                {
                    Reviews.Remove(SelectedReview);
                    SelectedReview = null;
                }           
            }
            else
            {
                MessageBox.Show("You can only delete your reviews");
            }
        }

        private void ShowCurrentUserReviews()
        {
            Reviews = RepositoryReview.GetUserReviewsFromDb(CurrentUser.Id);
        }

        private void ShowAllReviews()
        {
            Reviews = RepositoryReview.GetAllReviews();
        }

        private void ShowAuthorFromTextBoxReviews()
        {
            if (RepositoryUser.FindUserInDB(ReviewAuthor))
            {
                var author = RepositoryUser.GetUserFromDb(ReviewAuthor);
                Reviews = RepositoryReview.GetUserReviewsFromDb(author.Id);
            }
            else
            {
                MessageBox.Show("User with this username does not exist");
            }
        }

        private void OpenReviewWindow(object parameter)
        {
            var review = parameter as Review;
            review = RepositoryReview.GetReview(SelectedReview.Id);
            if (review != null)
            {
                var reviewWindow = new ReviewDetailsWindow();
                reviewWindow.DataContext = new ReviewDetailsViewModel(_currentUser, review);
                reviewWindow.Show();
            }
           
        }

        #endregion
    }
}
