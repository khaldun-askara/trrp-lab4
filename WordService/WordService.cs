using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.Text;
using Npgsql;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;

namespace WordService
{
    public class WordService : IWordService
    {
        static string main_table = Settings.Default.Main;
        static string suggestions = Settings.Default.Suggestions;

        static List<Word> words;

        private static string sConnStrMainDB = new NpgsqlConnectionStringBuilder()
        {
            Host = Settings.Default.Host,
            Port = Settings.Default.Port,
            Database = Settings.Default.Database,
            Username = Settings.Default.User,
            Password = Settings.Default.Password
        }.ConnectionString;

        private static string sConnStrBackupDB = new NpgsqlConnectionStringBuilder()
        {
            Host = Settings.Default.Host,
            Port = Settings.Default.Port,
            Database = Settings.Default.BackupDatabase,
            Username = Settings.Default.User,
            Password = Settings.Default.Password
        }.ConnectionString;

        public int GetWordsCount(bool isMain)
        {
            string tableName = isMain ? main_table : suggestions;

            words = new List<Word>();
            using (var sConn = new NpgsqlConnection(sConnStrMainDB))
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

        public int InsertWord(string word, bool isMain, bool isBackup = false)
        {
            string tableName = isMain ? main_table : suggestions;
            using (var sConn = new NpgsqlConnection(!isBackup ? sConnStrMainDB : sConnStrBackupDB))
            {
                sConn.Open();
                using (var sCommand = new NpgsqlCommand
                {
                    Connection = sConn,
                    CommandText = !isBackup ? $@"INSERT INTO {tableName} (word) VALUES (@word) RETURNING id" : $@"INSERT INTO {tableName} (word) VALUES (@word) ON CONFLICT DO NOTHING"
                })
                {
                    if (!isBackup)
                    {
                        sCommand.Parameters.AddWithValue("@word", word);
                        return (int)sCommand.ExecuteScalar();
                    }
                    else
                    {
                        sCommand.Parameters.AddWithValue("@word", word);
                        sCommand.ExecuteNonQuery();
                        return 0;
                    }
                    
                }
            }
        }

        public void UpdateWord(int id, string word, bool isMain)
        {
            string tableName = isMain ? main_table : suggestions;
            using (var sConn = new NpgsqlConnection(sConnStrMainDB))
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
            using (var sConn = new NpgsqlConnection(sConnStrMainDB))
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
            using (var sConn = new NpgsqlConnection(sConnStrMainDB))
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
            try
            {
                using (var sConn = new NpgsqlConnection(sConnStrMainDB))
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
            catch
            {
                try
                {
                    using (var sConn = new NpgsqlConnection(sConnStrBackupDB))
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
                catch
                {
                    return "нерабочая бд";
                }
            }
        }

        public void DoBackup()
        {
            try
            {
                GetWordsCount(true);
                foreach (var word in words)
                {
                    InsertWord(word.Value, true, true);
                    Console.WriteLine("вставлено в main " + word.Value);
                }
                GetWordsCount(false);
                foreach (var word in words)
                {
                    InsertWord(word.Value, false, true);
                    Console.WriteLine("вставлено в main " + word.Value);
                }
            }
            catch(Exception e)
            {
                Console.WriteLine(e);
            }
        }
    }
}
