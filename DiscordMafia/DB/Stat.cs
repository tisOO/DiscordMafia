﻿using System;
using System.Collections.Generic;
using Microsoft.Data.Sqlite;
using System.Linq;
using DiscordMafia.Client;

namespace DiscordMafia.DB
{
    public static class Stat
    {
        private static HashSet<string> allowedFields = new HashSet<string>()
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
            return User.findAllByCondition($"ORDER BY {field} DESC LIMIT :limit", parameters);
        }

        public static void RecalculateAll()
        {
            var users = User.findAllByCondition($"", new SqliteParameter[0]);
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
                message += String.Format("{0}. {1} - {2}" + Environment.NewLine, ++index, messageBuilder.FormatName(user.firstName, user.lastName, user.username), user.totalPoints);
            }

            message += Environment.NewLine + "Лучшие игроки по рейтингу: " + Environment.NewLine;
            index = 0;
            foreach (var user in GetTop("rate", howMany))
            {
                message += String.Format("{0}. {1} - {2}" + Environment.NewLine, ++index, messageBuilder.FormatName(user.firstName, user.lastName, user.username), user.rate.ToString("0.00"));
            }
            return message;
        }

        public static string GetStatAsString(UserWrapper user)
        {
            var dbUser = User.findById(user.Id);
            var winsPercent = dbUser.gamesPlayed > 0 ? 100.0 * dbUser.wins / dbUser.gamesPlayed : 0.0;
            var survivalsPercent = dbUser.gamesPlayed > 0 ? 100.0 * dbUser.survivals / dbUser.gamesPlayed : 0.0;
            var pointsAverage = dbUser.gamesPlayed > 0 ? 1.0 * dbUser.totalPoints / dbUser.gamesPlayed : 0.0;
            var message = "Ваша статистика:" + Environment.NewLine;
            message += $"Всего игр: {dbUser.gamesPlayed}{Environment.NewLine}";
            message += $"Пережил игр: {dbUser.survivals} ({survivalsPercent.ToString("0.00")}%){Environment.NewLine}";
            message += $"Побед: {dbUser.wins} ({winsPercent.ToString("0.00")}%){Environment.NewLine}";
            message += $"Очков: {dbUser.totalPoints} (в среднем за игру {pointsAverage.ToString("0.00")}){Environment.NewLine}";
            message += $"Рейтинг: {dbUser.rate.ToString("0.00")}{Environment.NewLine}";
            return message;
        }
    }
}
