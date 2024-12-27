using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using WPFGameShop.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Net.WebSockets;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Documents;
using static System.Windows.Forms.AxHost;

namespace WPFGameShop.Repositories
{
    public static class RepositoryGame
    {

        public static bool AddGameToDb(Game game)
        {
            bool state = false;

            using (var db = new ShopDbContext())
            {
                if (game != null)
                {

                    foreach (var ug in db.UserGames.ToList())
                    {
                        db.Entry(ug).State = EntityState.Unchanged;
                    }

                    db.Games.Add(game);
                    state = true;
                    db.SaveChanges();
                }
            }
            return state;
        }

        public static bool ModifyGameInDb(int idToModify, string newName, decimal newPrice, string newCoverImagePath)
        {
            bool state = false;
            using (var db = new ShopDbContext())
            {
                var rowsAffected = db.Games
                    .Where(r => r.Id == idToModify)
                    .ExecuteUpdate(review => review
                        .SetProperty(r => r.Name, r => newName)
                        .SetProperty(r => r.Price, r => newPrice)
                        .SetProperty(r => r.CoverImagePath, r => newCoverImagePath));

                if (rowsAffected > 0)
                    state = true;
            }
            return state;
        }
        public static bool DeleteGameFromDb(int idToDelete)
        {
            bool state = false;

            using (var db = new ShopDbContext())
            {
                var rowsAffected = db.Games
                   .Where(g => g.Id == idToDelete).ExecuteDelete();

                if (rowsAffected > 0)
                    state = true;
            }
            return state;
        }

        public static ObservableCollection<Game> GetUserGamesFromDb(int userId)
        {
            var list = new ObservableCollection<Game>();
            using (var db = new ShopDbContext())
            {
                
                var games = db.Users.Where(u => u.Id == userId).SelectMany(u => u.Games).ToList();

                foreach (var game in games)
                {
                    list.Add(game);
                }

            }
            return list;
            
        }

        public static ObservableCollection<Game> GetAllGamesFromDb()
        {
            var list = new ObservableCollection<Game>();

            using (var db = new ShopDbContext())
            {
                var games = db.Games.ToList();

                foreach (var game in games)
                {
                    list.Add(game);
                }
            }
            return list;
        }

    }
}
