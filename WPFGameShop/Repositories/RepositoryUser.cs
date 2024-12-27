using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using WPFGameShop.Models;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using static System.Windows.Forms.AxHost;

namespace WPFGameShop.Repositories
{
    public static class RepositoryUser
    {
        public static bool AddUserToDb(User user)
        {
            bool state = false;

            using (var db = new ShopDbContext())
            {
                if (user != null)
                {
                    foreach (var ug in db.UserGames.ToList())
                    {
                        db.Entry(ug).State = EntityState.Unchanged;
                    }
                    db.Users.Add(user);
                    db.SaveChanges();
                    state = true;
                }
            }
            return state;
        }

        public static bool DeleteUserFromDb(int idToDelete)
        {
            bool state = false;

            using (var db = new ShopDbContext())
            {
                var rowsAffected = db.Users
                .Where(r => r.Id == idToDelete).ExecuteDelete();

                if (rowsAffected > 0)
                    state = true;
            }
            return state;
        }

        public static bool ModifyUserInDb(User user)
        {
            bool state = false;
            using (var db = new ShopDbContext())
            {
                var rowsAffected = db.Users
                .Where(r => r.Id == user.Id)
                .ExecuteUpdate(review => review
                    .SetProperty(r => r.Money, r => user.Money));

                if (rowsAffected > 0)
                    state = true;
            }
            return state;
        }

        public static ObservableCollection<User> GetAllUsers()
        {
            var list = new ObservableCollection<User>();
            using (var db = new ShopDbContext())
            {
                var users = db.Users.ToList();
                
                foreach (var user in users)
                {
                    list.Add(user);
                }
            }

            return list;
        }

        public static async Task<User?> GetUserFromDb(User user)
        {
            using (var db = new ShopDbContext())
            {
                if (user != null)
                {
                    return await db.Users.SingleOrDefaultAsync(u => u.Name == user.Name && u.Password == user.Password);
                }
                return null;
            }
        }

        public static User GetUserFromDb(string userName)
        {   
            using (var db = new ShopDbContext())
            {
                if (userName != string.Empty)
                {
                    var resultUser = db.Users.SingleOrDefault(u => u.Name == userName);
                    return resultUser;
                }

                return null;
            }
        }

        public static async Task<bool> FindUserInDB(User user)
        {
            using (var db = new ShopDbContext())
            {
                try
                {
                if (user != null)
                    {
                        var foundUser = await db.Users.SingleOrDefaultAsync(u => u.Name == user.Name && u.Password == user.Password);
                        if (foundUser != null)
                        return true;
                    }
                return false;
                }catch (Exception ex) {
                    return false;   
                }
            }
        }

        public static bool FindUserInDB(string userName)
        {
            using (var db = new ShopDbContext())
            {
            if (userName != string.Empty)
            {
                var foundUser = db.Users.SingleOrDefault(u => u.Name == userName);
                if (foundUser != null)
                    return true;
            }

            }
            return false;
        }
    }
}