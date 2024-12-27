using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WPFGameShop.Models
{
    public class UserGame
    { 
        public User User { get; set; }
        public int UserId { get; set; }

        public Game Game { get; set; } = null!;
        public int GameId { get; set; }

        public DateTime TransactionDate { get; set; }
    }
}
