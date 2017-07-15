using System;
using Microsoft.EntityFrameworkCore;

namespace DiscordMafia.DB
{
    public class GameContext : DbContext
    {
        public string DatabasePath { get; set; } //  todo может стоит как-то иначе?

        public DbSet<User> Users { get; set; }
        public DbSet<Achievement> Achievements { get; set; }
        
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlite(Program.Connection.ConnectionString);
        }
    }
}