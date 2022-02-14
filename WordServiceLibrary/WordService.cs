using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.Text;
using Npgsql;

namespace WordServiceLibrary
{
    public class WordService : IWordService
    {
        static string main_table = Settings.Default.Main;
        static string suggestions = Settings.Default.Suggestions;

        static List<Word> words;

        private static string sConnStr = new NpgsqlConnectionStringBuilder()
        {
            Host = Settings.Default.Host,
            Port = Settings.Default.Port,
            Database = Settings.Default.Database,
            Username = Settings.Default.User,
            Password = Settings.Default.Password
        }.ConnectionString;

        public int GetWordsCount(bool isMain)
        {
            string tableName = isMain ? main_table : suggestions;

            words = new List<Word>();
            using (var sConn = new NpgsqlConnection(sConnStr))
            {
                sConn.Open();
                using (var sCommand = new NpgsqlCommand
                {
                    Connection = sConn,
                    CommandText = $@"SELECT * FROM {tableName}"
                })
                {
                    using (var reader = sCommand.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            Word w = new Word();
                            w.Id = (int)reader["id"];
                            w.Value = (string)reader["word"];
                            words.Add(w);
                        }
                    }
                }
            }
            return words.Count;
        }

        public Word GetWord(int i)
        {
            return words[i];
        }

        public int InsertWord(string word, bool isMain)
        {
            string tableName = isMain ? main_table : suggestions;
            using (var sConn = new NpgsqlConnection(sConnStr))
            {
                sConn.Open();
                using (var sCommand = new NpgsqlCommand
                {
                    Connection = sConn,
                    CommandText = $@"INSERT INTO {tableName} (word) VALUES (@word) RETURNING id"
                })
                {
                    sCommand.Parameters.AddWithValue("@word", word);
                    return (int)sCommand.ExecuteScalar();
                }
            }
        }

        public void UpdateWord(int id, string word, bool isMain)
        {
            string tableName = isMain ? main_table : suggestions;
            using (var sConn = new NpgsqlConnection(sConnStr))
            {
                sConn.Open();
                using (var sCommand = new NpgsqlCommand
                {
                    Connection = sConn,
                    CommandText = $@"UPDATE {tableName} SET word = @word WHERE id = @id"
                })
                {
                    sCommand.Parameters.AddWithValue("@id", id);
                    sCommand.Parameters.AddWithValue("@word", word);
                    sCommand.ExecuteNonQuery();
                }
            }
        }

        public void DeleteWord(int id, bool isMain)
        {
            string tableName = isMain ? main_table : suggestions;
            using (var sConn = new NpgsqlConnection(sConnStr))
            {
                sConn.Open();
                using (var sCommand = new NpgsqlCommand
                {
                    Connection = sConn,
                    CommandText = $@"DELETE FROM {tableName} WHERE id = @id"
                })
                {
                    sCommand.Parameters.AddWithValue("@id", id);
                    sCommand.ExecuteNonQuery();
                }
            }
        }

        public void MoveFromSuggestions(int id)
        {
            using (var sConn = new NpgsqlConnection(sConnStr))
            {
                sConn.Open();
                using (var sCommand = new NpgsqlCommand
                {
                    Connection = sConn,
                    CommandText = $@"INSERT INTO {main_table} (word) VALUES ((SELECT word FROM {suggestions} WHERE id = @id)) ON CONFLICT DO NOTHING;
                                        DELETE FROM {suggestions} WHERE id = @id "
                })
                {
                    sCommand.Parameters.AddWithValue("@id", id);
                    sCommand.ExecuteNonQuery();
                }
            }
        }

        public string GetRandomWord()
        {
            using (var sConn = new NpgsqlConnection(sConnStr))
            {
                sConn.Open();
                using (var sCommand = new NpgsqlCommand
                {
                    Connection = sConn,
                    CommandText = $@"SELECT word FROM {main_table} ORDER BY random() LIMIT 1"
                })
                {
                    using (var reader = sCommand.ExecuteReader())
                    {
                        reader.Read();
                        return (string)reader["word"];
                    }
                }
            }
        }
    }
}
