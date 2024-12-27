using WPFGameShop.Models;
using WPFGameShop.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Windows.Input;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.StartPanel;

namespace WPFGameShop.ViewModels
{
    public class ReviewDetailsViewModel : ViewModelBase
    {
        public ReviewDetailsViewModel(User user, Review review)
        {
            _selectedReview = review;
            _currentUser = user;
            _ratings = [1, 2, 3, 4, 5];
            _reviewContent =  review.Content;
            _newReviewContent = review.Content;
            _newRating = review.Score;
            _selectedRating = review.Score;
        }
        #region properties
        private string _reviewContent;
        private int[] _ratings;
        private int _selectedRating;
        private Review _selectedReview;
        private User _currentUser;
        private string _newReviewContent;
        private int _newRating;
        #endregion


        #region accesors

        public Review SelectedReview
        {
            get { return _selectedReview; }
            set
            {
                _selectedReview = value;
                onPropertyChanged(nameof(SelectedReview));
            }
        }

        public User CurrentUser
        {
            get { return _currentUser; }
            set
            {
                _currentUser = value;
                onPropertyChanged(nameof(CurrentUser));
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

        public int SelectedRating
        {
            get { return _selectedRating; }
            set
            {
                _selectedRating = value;
                onPropertyChanged(nameof(SelectedRating));
            }
        }


        public string NewReviewContent
        {
            get { return _newReviewContent; }
            set
            {
                _newReviewContent = value;
                onPropertyChanged(nameof(NewReviewContent));
            }
        }

        public int NewRating
        {
            get { return _newRating; }
            set
            {
                _newRating = value;
                onPropertyChanged(nameof(NewRating));
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

        

        #endregion

        #region commands

        private ICommand editReviewCommand;
        public ICommand EditReviewCommand
        {
            get
            {
                if (editReviewCommand == null)
                    editReviewCommand = new RelayCommand(
                        parameter => EditReview(),
                        predicate => SelectedRating >= 1 && SelectedRating <= 5 &&
                                     ReviewContent != null && ReviewContent != ""
                        );
                return editReviewCommand;
            }
        }

        #endregion

        #region functions

        private void EditReview()
        {
            if (SelectedReview.UserId == CurrentUser.Id)
            {

                if (RepositoryReview.ModifyReviewInDb(SelectedReview.Id, ReviewContent, SelectedRating))
                {
                    NewReviewContent = ReviewContent;
                    NewRating = SelectedRating;

                    ReviewContent = string.Empty;
                    SelectedRating = 0;    
                }
            }
            else
            {
                MessageBox.Show("You can't edit reviews that aren't yours");
            }
          
        }

        #endregion

    }
}
