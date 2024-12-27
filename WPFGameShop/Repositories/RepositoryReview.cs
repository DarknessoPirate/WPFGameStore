using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.VisualBasic.ApplicationServices;
using WPFGameShop.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace WPFGameShop.Repositories
{
    public static class RepositoryReview
    {
        public static bool AddReviewToDb(Review review)
        {
            bool state = false;

            using (var db = new ShopDbContext())
            {
                if (!db.Reviews.Any(r => r.GameId == review.GameId && r.UserId == review.UserId))
                {
                    if (review != null)
                    {
                        foreach (var ug in db.UserGames.ToList())
                        {
                            db.Entry(ug).State = EntityState.Unchanged;
                        }

                        db.Reviews.Add(review);
                        state = true;
                        db.SaveChanges();
                    }
                }       
            }
            return state;
        }

        public static bool ModifyReviewInDb(int idToModify, string newContent, int newScore)
        {
            bool state = false;
            using (var db = new ShopDbContext())
            {
                var rowsAffected = db.Reviews
                    .Where(r => r.Id == idToModify)
                    .ExecuteUpdate(review => review
                        .SetProperty(r => r.Content, r => newContent)
                        .SetProperty(r => r.Score, r => newScore));

                if (rowsAffected > 0)
                    state = true;
            }
            return state;
        }

        public static bool DeleteReviewFromDb(int idToDelete)
        {
            bool state = false;

            using (var db = new ShopDbContext())
            {
                var rowsAffected = db.Reviews
                    .Where(r => r.Id == idToDelete).ExecuteDelete();

                if (rowsAffected > 0)
                    state = true;
            }
            return state;
        }

        public static ObservableCollection<Review> GetUserReviewsFromDb(int userId)
        {
            var list = new ObservableCollection<Review>();

            using (var db = new ShopDbContext())
            {
                var reviews = db.Users.Where(u => u.Id == userId).SelectMany(u => u.Reviews).Include(g => g.User).Include(g => g.Game).ToList();
                foreach (var review in reviews)
                {
                    list.Add(review);
                }
            }
            return list;
        }

        public static ObservableCollection<Review> GetAllReviews()
        {
            var list = new ObservableCollection<Review>();
           

            using (var db = new ShopDbContext())
            {
                var reviews = db.Reviews.Include(g => g.User).Include(g => g.Game).ToList();
                foreach (var review in reviews)
                {
                    list.Add(review);
                }
            }
            return list;
        }

        public static Review GetReview(int id)
        {
            using (var db = new ShopDbContext())
            {
                var review = db.Reviews.Include(g => g.User).Include(g => g.Game).FirstOrDefault(r => r.Id == id);
                return review; 
            }

        }

        public static ObservableCollection<Review> GetGameReviews(Game game)
        {
            var list = new ObservableCollection<Review>();
            using (var db = new ShopDbContext())
            {
                var reviews = db.Reviews.Where(u => u.GameId == game.Id).Include(g => g.User).Include(g => g.Game).ToList();
                foreach (var review in reviews)
                {
                    list.Add(review);
                }
            }

            return list;
        }

    }
}
