﻿using System;
using System.Collections.Generic;
using Microsoft.Data.Sqlite;
using System.Linq;
using DiscordMafia.Client;

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
            if (!allowedFields.Contains(field))
            {
                field = allowedFields.First();
            }
            howMany = Math.Min(Math.Max(howMany, 1), 300);
            var parameters = new SqliteParameter[] { new SqliteParameter(":limit", howMany) };
            return User.FindAllByCondition($"ORDER BY {field} DESC LIMIT :limit", parameters);
        }

        public static void RecalculateAll()
        {
            var users = User.FindAllByCondition($"", new SqliteParameter[0]);
            foreach (var user in users)
            {
                user.RecalculateStats();
                user.Save();
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
            var dbUser = User.FindById(user.Id);
            var winsPercent = dbUser.GamesPlayed > 0 ? 100.0 * dbUser.Wins / dbUser.GamesPlayed : 0.0;
            var survivalsPercent = dbUser.GamesPlayed > 0 ? 100.0 * dbUser.Survivals / dbUser.GamesPlayed : 0.0;
            var pointsAverage = dbUser.GamesPlayed > 0 ? 1.0 * dbUser.TotalPoints / dbUser.GamesPlayed : 0.0;
            var message = "Ваша статистика:" + Environment.NewLine;
            message += $"Всего игр: {dbUser.GamesPlayed}{Environment.NewLine}";
            message += $"Пережил игр: {dbUser.Survivals} ({survivalsPercent.ToString("0.00")}%){Environment.NewLine}";
            message += $"Побед: {dbUser.Wins} ({winsPercent.ToString("0.00")}%){Environment.NewLine}";
            message += $"Очков: {dbUser.TotalPoints} (в среднем за игру {pointsAverage.ToString("0.00")}){Environment.NewLine}";
            message += $"Рейтинг: {dbUser.Rate.ToString("0.00")}{Environment.NewLine}";
            return message;
        }
    }
}
