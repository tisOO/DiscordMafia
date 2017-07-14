﻿using System;
using System.Collections.Generic;
using System.Data.Common;
using Microsoft.Data.Sqlite;

namespace DiscordMafia.DB
{
    public class Achievement
    {
        public long id;
        public ulong userId;
        public string achievementId;
        public DateTime achievedAt;
        public bool isNewRecord = true;

        public static Achievement findUserAchievement(ulong userId, string achievementId)
        {
            var connection = Program.Connection;
            var command = connection.CreateCommand();
            command.CommandText = getSelect() + "WHERE user_id = :userId AND achievement_id = :achievementId";
            command.Parameters.AddWithValue(":userId", userId);
            command.Parameters.AddWithValue(":achievementId", achievementId);
            var reader = command.ExecuteReader();
            var achievement = new Achievement();
            achievement.populateRecord(reader);
            return !achievement.isNewRecord ? achievement : null;
        }

        public static IList<Achievement> findUserAchievements(ulong userId)
        {
            return findAllByCondition("WHERE user_id = :userId", new SqliteParameter[] { new SqliteParameter(":userId", userId)});
        }

        public static IList<Achievement> findAllByCondition(string condition, SqliteParameter[] parameters)
        {
            var connection = Program.Connection;
            var command = connection.CreateCommand();
            var achievements = new List<Achievement>();
            command.CommandText = getSelect() + condition;
            command.Parameters.AddRange(parameters);
            var reader = command.ExecuteReader();
            do
            {
                var achievement = new Achievement();
                achievement.populateRecord(reader);
                if (achievement.isNewRecord)
                {
                    break;
                }
                achievements.Add(achievement);

            } while (true);
            return achievements;
        }

        public static IList<Achievement> findAllByCondition(string condition)
        {
            return findAllByCondition(condition, new SqliteParameter[0]);
        }

        protected Achievement populateRecord(DbDataReader reader)
        {
            if (reader.Read())
            {
                id = reader.GetInt64(0);
                userId = ulong.Parse(reader.GetValue(1).ToString());
                achievementId = reader.GetString(2);
                achievedAt = DateTime.FromBinary(reader.GetInt64(3));
                isNewRecord = false;
            }
            return this;
        }

        protected static string getSelect()
        {
            return "SELECT id, user_id, achievement_id, achieved_at FROM user_achievement ";
        }

        public bool Save()
        {
            var connection = Program.Connection;
            var command = connection.CreateCommand();
            if (isNewRecord)
            {
                command.CommandText = @"INSERT OR REPLACE INTO user_achievement (user_id, achievement_id, achieved_at)
                                    VALUES (:userId, :achievementId, :achievedAt)";
                command.Parameters.AddWithValue(":userId", userId);
                command.Parameters.AddWithValue(":achievementId", achievementId);
                command.Parameters.AddWithValue(":achievedAt", achievedAt.ToBinary());

                return command.ExecuteNonQuery() > 0;
            }
            return false;
        }
    }
}
