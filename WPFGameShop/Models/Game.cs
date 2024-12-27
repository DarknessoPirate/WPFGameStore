using WPFGameShop.Repositories;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Navigation;

namespace WPFGameShop.Models
{
    public class Game
    {
        public int Id { get; set; }
        [MaxLength(255)]
        public string Name { get; set; } = null!;
        [Column(TypeName = "money")]
        public decimal Price { get; set; }
        public string CoverImagePath { get; set; } = null!;
        public ObservableCollection<User> Users { get; set;} = new ObservableCollection<User>();
        public ObservableCollection<Review> Reviews { get; set; } = new ObservableCollection<Review>();
        [NotMapped]
        public double AvgReating { get {
                ObservableCollection<Review> revs = RepositoryReview.GetGameReviews(this);
                return revs.Count == 0 ? double.NaN : revs.Average(r => r.Score);
            }
        }
    }
}
