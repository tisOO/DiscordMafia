using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using Microsoft.EntityFrameworkCore;

namespace DiscordMafia.DB
{
    [Table("user")]
    public class User
    {
        [Key, Column("id")]
        public ulong Id { get; set; }
        [Column("username")]
        public string Username { get; set; } = "";
        [Column("first_name")]
        public string FirstName { get; set; } = "";
        [Column("last_name")]
        public string LastName { get; set; } = "";
        [Column("total_points")]
        public long TotalPoints { get; set; } = 0;
        [Column("games")]
        public int GamesPlayed { get; set; } = 0;
        [Column("wins")]
        public int Wins { get; set; } = 0;
        [Column("survivals")]
        public int Survivals { get; set; } = 0;
        [Column("draws")]
        public int Draws { get; set; } = 0;
        [Column("rate")]
        public double Rate { get; set; } = 0.0;
        [Column("is_registered")]
        public bool IsRegistered { get; set; } = false;
//        public bool IsNewRecord = true;
        
        public void RecalculateStats()
        {
            if (GamesPlayed - Wins - Draws == 0)
            {
                Rate = (Wins + Survivals * 0.5) * GamesPlayed * 1.0;
            }
            else
            {
                Rate = 1.0 * (Wins + Survivals * 0.5) * GamesPlayed / (GamesPlayed - Wins - Draws);
            }
        }
        
        public static User FindById(ulong id)
        {
            using (var context = new GameContext())
            {
                var dbUser = context.Users.AsNoTracking().SingleOrDefault(u => u.Id == id);
                if (dbUser == null)
                {
                    dbUser = new User { Id = id };
                }

                return dbUser;
            }
        }

        public static bool TryToSave(User user)
        {
            try
            {
                using (var context = new GameContext())
                {
                    context.Users.Add(user);
                    context.SaveChanges();
                }
            }
            catch (Exception e)
            {
                // todo нз что здесь
                return false;
            }
            
            return true;
        }
    }
}