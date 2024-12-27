using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WPFGameShop.Models
{
    public class Review
    {
        public int Id { get; set; }
        public int Score { get; set; }
        public string Content { get; set; } = null!;

        public User User { get; set; } = null!;
        public int UserId { get; set; }

        public Game Game { get; set; } = null!;
        public int GameId { get; set; }
    }
}
