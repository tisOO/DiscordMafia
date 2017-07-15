using System;
using System.Collections.Generic;
using System.Linq;
using DiscordMafia.Client;
using Microsoft.EntityFrameworkCore;

namespace DiscordMafia.DB
{
    public static class Stat
    {
        private static readonly HashSet<string> allowedFields = new HashSet<string>()
        {
            "total_points", "rate", "games", "wins"
        };

        public static IEnumerable<User> GetTop(string field, int howMany = 20)
        {
            howMany = Math.Min(Math.Max(howMany, 1), 300);
            if (!allowedFields.Contains(field))
            {
                field = allowedFields.First();
            }
            using (var context = new GameContext())
            {
                var dbUsers = context.Users.AsNoTracking().Take(howMany);
                switch (field)
                {
                    case "total_points":
                        dbUsers = dbUsers.OrderByDescending(u => u.TotalPoints);
                        break;
                    case "rate":
                        dbUsers = dbUsers.OrderByDescending(u => u.Rate);
                        break;
                    case "games":
                        dbUsers = dbUsers.OrderByDescending(u => u.GamesPlayed);
                        break;
                    case "wins":
                        dbUsers = dbUsers.OrderByDescending(u => u.Wins);
                        break;
                }

                return dbUsers.ToList();
            }
        }

        public static void RecalculateAll()
        {
            using (var context = new GameContext())
            {
                var dbUsers = context.Users.ToList();
                foreach (var user in dbUsers)
                {
                    user.RecalculateStats();
                }
                context.SaveChanges();
            }
        }

        public static string GetTopAsString(Config.MessageBuilder messageBuilder, int howMany = 20)
        {
            var message = "Лучшие игроки по очкам: " + Environment.NewLine;
            var index = 0;
            foreach (var user in GetTop("total_points", howMany))
            {
                message += String.Format("{0}. {1} - {2}" + Environment.NewLine, ++index, messageBuilder.FormatName(user.FirstName, user.LastName, user.Username), user.TotalPoints);
            }

            message += Environment.NewLine + "Лучшие игроки по рейтингу: " + Environment.NewLine;
            index = 0;
            foreach (var user in GetTop("rate", howMany))
            {
                message += String.Format("{0}. {1} - {2}" + Environment.NewLine, ++index, messageBuilder.FormatName(user.FirstName, user.LastName, user.Username), user.Rate.ToString("0.00"));
            }
            return message;
        }

        public static string GetStatAsString(UserWrapper user)
        {
            using (var context = new GameContext())
            {
                var dbUser = context.Users.AsNoTracking().Single(u => u.Id == user.Id);
                var winsPercent = dbUser.GamesPlayed > 0 ? 100.0 * dbUser.Wins / dbUser.GamesPlayed : 0.0;
                var survivalsPercent = dbUser.GamesPlayed > 0 ? 100.0 * dbUser.Survivals / dbUser.GamesPlayed : 0.0;
                var pointsAverage = dbUser.GamesPlayed > 0 ? 1.0 * dbUser.TotalPoints / dbUser.GamesPlayed : 0.0;
                var message = "Ваша статистика:" + Environment.NewLine;
                message += $"Всего игр: {dbUser.GamesPlayed}{Environment.NewLine}";
                message += $"Пережил игр: {dbUser.Survivals} ({survivalsPercent:0.00}%){Environment.NewLine}";
                message += $"Побед: {dbUser.Wins} ({winsPercent:0.00}%){Environment.NewLine}";
                message += $"Очков: {dbUser.TotalPoints} (в среднем за игру {pointsAverage:0.00}){Environment.NewLine}";
                message += $"Рейтинг: {dbUser.Rate:0.00}{Environment.NewLine}";
                return message;
            }   
        }
    }
}
