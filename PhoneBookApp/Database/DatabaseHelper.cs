using System;
using System.Collections.Generic;
using System.Data.SQLite;
using System.IO;
using System.Linq;
using System.Text;
using PhoneBookApp.Models;
using PhoneBookApp.Helpers;

namespace PhoneBookApp.Database
{
    public class DatabaseHelper
    {
        private readonly string _connectionString;

        public DatabaseHelper()
        {
            string dbPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "PhoneBook.db");
            _connectionString = $"Data Source={dbPath};Version=3;";
            InitializeDatabase();
            MigrateDatabase();
        }

        private void InitializeDatabase()
        {
            if (!File.Exists(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "PhoneBook.db")))
                CreateDatabase();
        }

        private void CreateDatabase()
        {
            using (var connection = new SQLiteConnection(_connectionString))
            {
                connection.Open();

                string createUsersTable = @"
                    CREATE TABLE Users (
                        Id INTEGER PRIMARY KEY AUTOINCREMENT,
                        Username TEXT NOT NULL,
                        Email TEXT UNIQUE NOT NULL,
                        PasswordHash TEXT NOT NULL,
                        RegistrationDate TEXT NOT NULL,
                        Role TEXT NOT NULL DEFAULT 'User',
                        IsBlocked INTEGER NOT NULL DEFAULT 0
                    )";

                string createContactsTable = @"
                    CREATE TABLE Contacts (
                        Id INTEGER PRIMARY KEY AUTOINCREMENT,
                        UserId INTEGER NOT NULL,
                        FirstName TEXT NOT NULL,
                        LastName TEXT,
                        Phone TEXT,
                        Email TEXT,
                        Address TEXT,
                        Note TEXT,
                        Birthday TEXT,
                        CreatedAt TEXT NOT NULL,
                        UpdatedAt TEXT NOT NULL,
                        FOREIGN KEY (UserId) REFERENCES Users(Id) ON DELETE CASCADE
                    )";

                string createTagsTable = @"
                    CREATE TABLE Tags (
                        Id INTEGER PRIMARY KEY AUTOINCREMENT,
                        Name TEXT NOT NULL UNIQUE
                    )";

                string createContactTagsTable = @"
                    CREATE TABLE ContactTags (
                        ContactId INTEGER NOT NULL,
                        TagId INTEGER NOT NULL,
                        PRIMARY KEY (ContactId, TagId),
                        FOREIGN KEY (ContactId) REFERENCES Contacts(Id) ON DELETE CASCADE,
                        FOREIGN KEY (TagId) REFERENCES Tags(Id) ON DELETE CASCADE
                    )";

                using (var cmd = new SQLiteCommand(createUsersTable, connection)) cmd.ExecuteNonQuery();
                using (var cmd = new SQLiteCommand(createContactsTable, connection)) cmd.ExecuteNonQuery();
                using (var cmd = new SQLiteCommand(createTagsTable, connection)) cmd.ExecuteNonQuery();
                using (var cmd = new SQLiteCommand(createContactTagsTable, connection)) cmd.ExecuteNonQuery();

                // администратор по умолчанию
                string adminPassword = PasswordHelper.HashPassword("admin123");
                string insertAdmin = @"INSERT INTO Users (Username, Email, PasswordHash, RegistrationDate, Role, IsBlocked)
                                       VALUES ('Admin', 'admin@phonebook.com', @pwd, @date, 'Admin', 0)";
                using (var cmd = new SQLiteCommand(insertAdmin, connection))
                {
                    cmd.Parameters.AddWithValue("@pwd", adminPassword);
                    cmd.Parameters.AddWithValue("@date", DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"));
                    cmd.ExecuteNonQuery();
                }

                // предустановленные теги
                var defaultTags = new[] { "Работа", "Друзья", "Семья", "Важное" };
                foreach (var tag in defaultTags)
                {
                    string insertTag = "INSERT OR IGNORE INTO Tags (Name) VALUES (@name)";
                    using (var cmd = new SQLiteCommand(insertTag, connection))
                    {
                        cmd.Parameters.AddWithValue("@name", tag);
                        cmd.ExecuteNonQuery();
                    }
                }
            }
        }

        private void MigrateDatabase()
        {
            using (var connection = new SQLiteConnection(_connectionString))
            {
                connection.Open();

                // добавить колонку Birthday, если её нет
                try
                {
                    string checkColumn = "SELECT Birthday FROM Contacts LIMIT 1";
                    using (var cmd = new SQLiteCommand(checkColumn, connection))
                        cmd.ExecuteScalar();
                }
                catch
                {
                    string addColumn = "ALTER TABLE Contacts ADD COLUMN Birthday TEXT";
                    using (var cmd = new SQLiteCommand(addColumn, connection))
                        cmd.ExecuteNonQuery();
                }

                // таблицы тегов (если отсутствуют)
                string createTags = @"CREATE TABLE IF NOT EXISTS Tags (
                                        Id INTEGER PRIMARY KEY AUTOINCREMENT,
                                        Name TEXT NOT NULL UNIQUE
                                      )";
                using (var cmd = new SQLiteCommand(createTags, connection))
                    cmd.ExecuteNonQuery();

                string createContactTags = @"CREATE TABLE IF NOT EXISTS ContactTags (
                                                ContactId INTEGER NOT NULL,
                                                TagId INTEGER NOT NULL,
                                                PRIMARY KEY (ContactId, TagId),
                                                FOREIGN KEY (ContactId) REFERENCES Contacts(Id) ON DELETE CASCADE,
                                                FOREIGN KEY (TagId) REFERENCES Tags(Id) ON DELETE CASCADE
                                             )";
                using (var cmd = new SQLiteCommand(createContactTags, connection))
                    cmd.ExecuteNonQuery();

                var defaultTags = new[] { "Работа", "Друзья", "Семья", "Важное" };
                foreach (var tag in defaultTags)
                {
                    string insert = "INSERT OR IGNORE INTO Tags (Name) VALUES (@name)";
                    using (var cmd = new SQLiteCommand(insert, connection))
                    {
                        cmd.Parameters.AddWithValue("@name", tag);
                        cmd.ExecuteNonQuery();
                    }
                }
            }
        }

        // ========== ТЕГИ ==========
        public List<string> GetAllTags()
        {
            var tags = new List<string>();
            using (var conn = new SQLiteConnection(_connectionString))
            {
                conn.Open();
                string sql = "SELECT Name FROM Tags ORDER BY Name";
                using (var cmd = new SQLiteCommand(sql, conn))
                using (var reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                        tags.Add(reader.GetString(0));
                }
            }
            return tags;
        }

        public List<string> GetTagsForContact(int contactId)
        {
            var tags = new List<string>();
            using (var conn = new SQLiteConnection(_connectionString))
            {
                conn.Open();
                string sql = @"SELECT t.Name FROM Tags t
                               INNER JOIN ContactTags ct ON ct.TagId = t.Id
                               WHERE ct.ContactId = @contactId";
                using (var cmd = new SQLiteCommand(sql, conn))
                {
                    cmd.Parameters.AddWithValue("@contactId", contactId);
                    using (var reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                            tags.Add(reader.GetString(0));
                    }
                }
            }
            return tags;
        }

        public void SetContactTags(int contactId, List<string> tagNames)
        {
            using (var conn = new SQLiteConnection(_connectionString))
            {
                conn.Open();
                // удалить старые связи
                string delete = "DELETE FROM ContactTags WHERE ContactId = @cid";
                using (var cmd = new SQLiteCommand(delete, conn))
                {
                    cmd.Parameters.AddWithValue("@cid", contactId);
                    cmd.ExecuteNonQuery();
                }

                foreach (var tagName in tagNames)
                {
                    int tagId;
                    string selectTag = "SELECT Id FROM Tags WHERE Name = @name";
                    using (var cmd = new SQLiteCommand(selectTag, conn))
                    {
                        cmd.Parameters.AddWithValue("@name", tagName);
                        var res = cmd.ExecuteScalar();
                        if (res != null)
                            tagId = Convert.ToInt32(res);
                        else
                        {
                            string insertTag = "INSERT INTO Tags (Name) VALUES (@name); SELECT last_insert_rowid();";
                            using (var cmd2 = new SQLiteCommand(insertTag, conn))
                            {
                                cmd2.Parameters.AddWithValue("@name", tagName);
                                tagId = Convert.ToInt32(cmd2.ExecuteScalar());
                            }
                        }
                    }
                    string insert = "INSERT INTO ContactTags (ContactId, TagId) VALUES (@cid, @tid)";
                    using (var cmd = new SQLiteCommand(insert, conn))
                    {
                        cmd.Parameters.AddWithValue("@cid", contactId);
                        cmd.Parameters.AddWithValue("@tid", tagId);
                        cmd.ExecuteNonQuery();
                    }
                }
            }
        }

        // ========== ПОЛЬЗОВАТЕЛИ ==========
        public bool RegisterUser(string username, string email, string password)
        {
            try
            {
                using (var connection = new SQLiteConnection(_connectionString))
                {
                    connection.Open();
                    string query = @"INSERT INTO Users (Username, Email, PasswordHash, RegistrationDate, Role, IsBlocked)
                                    VALUES (@username, @email, @pwd, @date, 'User', 0)";
                    using (var cmd = new SQLiteCommand(query, connection))
                    {
                        cmd.Parameters.AddWithValue("@username", username);
                        cmd.Parameters.AddWithValue("@email", email);
                        cmd.Parameters.AddWithValue("@pwd", PasswordHelper.HashPassword(password));
                        cmd.Parameters.AddWithValue("@date", DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"));
                        return cmd.ExecuteNonQuery() > 0;
                    }
                }
            }
            catch { return false; }
        }

        public User AuthenticateUser(string email, string password)
        {
            using (var connection = new SQLiteConnection(_connectionString))
            {
                connection.Open();
                string query = "SELECT * FROM Users WHERE Email = @email";
                using (var cmd = new SQLiteCommand(query, connection))
                {
                    cmd.Parameters.AddWithValue("@email", email);
                    using (var reader = cmd.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            bool isBlocked = reader.GetInt32(reader.GetOrdinal("IsBlocked")) == 1;
                            if (isBlocked) return null;
                            string passwordHash = reader.GetString(reader.GetOrdinal("PasswordHash"));
                            if (PasswordHelper.VerifyPassword(password, passwordHash))
                            {
                                return new User
                                {
                                    Id = reader.GetInt32(reader.GetOrdinal("Id")),
                                    Username = reader.GetString(reader.GetOrdinal("Username")),
                                    Email = reader.GetString(reader.GetOrdinal("Email")),
                                    RegistrationDate = DateTime.Parse(reader.GetString(reader.GetOrdinal("RegistrationDate"))),
                                    Role = reader.GetString(reader.GetOrdinal("Role")),
                                    IsBlocked = isBlocked
                                };
                            }
                        }
                    }
                }
            }
            return null;
        }

        public List<User> GetAllUsers()
        {
            var users = new List<User>();
            using (var connection = new SQLiteConnection(_connectionString))
            {
                connection.Open();
                string query = "SELECT * FROM Users";
                using (var cmd = new SQLiteCommand(query, connection))
                using (var reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        users.Add(new User
                        {
                            Id = reader.GetInt32(reader.GetOrdinal("Id")),
                            Username = reader.GetString(reader.GetOrdinal("Username")),
                            Email = reader.GetString(reader.GetOrdinal("Email")),
                            RegistrationDate = DateTime.Parse(reader.GetString(reader.GetOrdinal("RegistrationDate"))),
                            Role = reader.GetString(reader.GetOrdinal("Role")),
                            IsBlocked = reader.GetInt32(reader.GetOrdinal("IsBlocked")) == 1
                        });
                    }
                }
            }
            return users;
        }

        public bool BlockUser(int userId) => UpdateUserBlockStatus(userId, true);
        public bool UnblockUser(int userId) => UpdateUserBlockStatus(userId, false);

        private bool UpdateUserBlockStatus(int userId, bool isBlocked)
        {
            using (var connection = new SQLiteConnection(_connectionString))
            {
                connection.Open();
                string query = "UPDATE Users SET IsBlocked = @blocked WHERE Id = @id";
                using (var cmd = new SQLiteCommand(query, connection))
                {
                    cmd.Parameters.AddWithValue("@blocked", isBlocked ? 1 : 0);
                    cmd.Parameters.AddWithValue("@id", userId);
                    return cmd.ExecuteNonQuery() > 0;
                }
            }
        }

        public bool ChangeUserRole(int userId, string role)
        {
            using (var connection = new SQLiteConnection(_connectionString))
            {
                connection.Open();
                string query = "UPDATE Users SET Role = @role WHERE Id = @id";
                using (var cmd = new SQLiteCommand(query, connection))
                {
                    cmd.Parameters.AddWithValue("@role", role);
                    cmd.Parameters.AddWithValue("@id", userId);
                    return cmd.ExecuteNonQuery() > 0;
                }
            }
        }

        public bool DeleteUser(int userId)
        {
            using (var connection = new SQLiteConnection(_connectionString))
            {
                connection.Open();
                string query = "DELETE FROM Users WHERE Id = @id";
                using (var cmd = new SQLiteCommand(query, connection))
                {
                    cmd.Parameters.AddWithValue("@id", userId);
                    return cmd.ExecuteNonQuery() > 0;
                }
            }
        }

        // ========== КОНТАКТЫ ==========
        public List<Contact> GetContactsByUserId(int userId)
        {
            var contacts = new List<Contact>();
            using (var conn = new SQLiteConnection(_connectionString))
            {
                conn.Open();
                string sql = "SELECT * FROM Contacts WHERE UserId = @userId ORDER BY FirstName";
                using (var cmd = new SQLiteCommand(sql, conn))
                {
                    cmd.Parameters.AddWithValue("@userId", userId);
                    using (var reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            var c = MapContact(reader);
                            c.TagNames = GetTagsForContact(c.Id);
                            contacts.Add(c);
                        }
                    }
                }
            }
            return contacts;
        }

        public bool AddContact(Contact contact)
        {
            using (var connection = new SQLiteConnection(_connectionString))
            {
                connection.Open();
                string query = @"INSERT INTO Contacts (UserId, FirstName, LastName, Phone, Email, Address, Note, Birthday, CreatedAt, UpdatedAt)
                                 VALUES (@userId, @firstName, @lastName, @phone, @email, @address, @note, @birthday, @created, @updated);
                                 SELECT last_insert_rowid();";
                using (var cmd = new SQLiteCommand(query, connection))
                {
                    cmd.Parameters.AddWithValue("@userId", contact.UserId);
                    cmd.Parameters.AddWithValue("@firstName", contact.FirstName);
                    cmd.Parameters.AddWithValue("@lastName", contact.LastName ?? "");
                    cmd.Parameters.AddWithValue("@phone", contact.Phone ?? "");
                    cmd.Parameters.AddWithValue("@email", contact.Email ?? "");
                    cmd.Parameters.AddWithValue("@address", contact.Address ?? "");
                    cmd.Parameters.AddWithValue("@note", contact.Note ?? "");
                    cmd.Parameters.AddWithValue("@birthday", contact.Birthday?.ToString("yyyy-MM-dd") ?? "");
                    cmd.Parameters.AddWithValue("@created", DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"));
                    cmd.Parameters.AddWithValue("@updated", DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"));

                    long newId = (long)cmd.ExecuteScalar();
                    contact.Id = (int)newId;
                    if (contact.TagNames != null && contact.TagNames.Any())
                        SetContactTags(contact.Id, contact.TagNames);
                    return true;
                }
            }
        }

        public bool UpdateContact(Contact contact)
        {
            using (var connection = new SQLiteConnection(_connectionString))
            {
                connection.Open();
                string query = @"UPDATE Contacts SET FirstName = @firstName, LastName = @lastName, 
                                 Phone = @phone, Email = @email, Address = @address, Note = @note, Birthday = @birthday,
                                 UpdatedAt = @updated WHERE Id = @id AND UserId = @userId";
                using (var cmd = new SQLiteCommand(query, connection))
                {
                    cmd.Parameters.AddWithValue("@id", contact.Id);
                    cmd.Parameters.AddWithValue("@userId", contact.UserId);
                    cmd.Parameters.AddWithValue("@firstName", contact.FirstName);
                    cmd.Parameters.AddWithValue("@lastName", contact.LastName ?? "");
                    cmd.Parameters.AddWithValue("@phone", contact.Phone ?? "");
                    cmd.Parameters.AddWithValue("@email", contact.Email ?? "");
                    cmd.Parameters.AddWithValue("@address", contact.Address ?? "");
                    cmd.Parameters.AddWithValue("@note", contact.Note ?? "");
                    cmd.Parameters.AddWithValue("@birthday", contact.Birthday?.ToString("yyyy-MM-dd") ?? "");
                    cmd.Parameters.AddWithValue("@updated", DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"));

                    bool ok = cmd.ExecuteNonQuery() > 0;
                    if (ok)
                        SetContactTags(contact.Id, contact.TagNames ?? new List<string>());
                    return ok;
                }
            }
        }

        public bool DeleteContact(int contactId, int userId)
        {
            using (var connection = new SQLiteConnection(_connectionString))
            {
                connection.Open();
                string query = "DELETE FROM Contacts WHERE Id = @id AND UserId = @userId";
                using (var cmd = new SQLiteCommand(query, connection))
                {
                    cmd.Parameters.AddWithValue("@id", contactId);
                    cmd.Parameters.AddWithValue("@userId", userId);
                    return cmd.ExecuteNonQuery() > 0;
                }
            }
        }

        public List<Contact> SearchContacts(int userId, string searchTerm, List<string> filterTags = null)
        {
            var contacts = new List<Contact>();
            using (var conn = new SQLiteConnection(_connectionString))
            {
                conn.Open();
                string sql = @"SELECT DISTINCT c.* FROM Contacts c
                               LEFT JOIN ContactTags ct ON ct.ContactId = c.Id
                               LEFT JOIN Tags t ON t.Id = ct.TagId
                               WHERE c.UserId = @userId AND 
                                     (c.FirstName LIKE @search OR c.LastName LIKE @search OR c.Phone LIKE @search)";
                if (filterTags != null && filterTags.Any())
                {
                    sql += " AND t.Name IN (" + string.Join(",", filterTags.Select((_, i) => $"@tag{i}")) + ")";
                }
                sql += " ORDER BY c.FirstName";

                using (var cmd = new SQLiteCommand(sql, conn))
                {
                    cmd.Parameters.AddWithValue("@userId", userId);
                    cmd.Parameters.AddWithValue("@search", $"%{searchTerm}%");
                    if (filterTags != null)
                        for (int i = 0; i < filterTags.Count; i++)
                            cmd.Parameters.AddWithValue($"@tag{i}", filterTags[i]);

                    using (var reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            var c = MapContact(reader);
                            c.TagNames = GetTagsForContact(c.Id);
                            contacts.Add(c);
                        }
                    }
                }
            }
            return contacts;
        }

        public List<Contact> GetContactsWithBirthdayInRange(DateTime start, DateTime end)
        {
            var result = new List<Contact>();
            using (var conn = new SQLiteConnection(_connectionString))
            {
                conn.Open();
                string sql = @"SELECT * FROM Contacts WHERE Birthday IS NOT NULL AND Birthday != ''
                               AND strftime('%m-%d', Birthday) BETWEEN @start AND @end";
                using (var cmd = new SQLiteCommand(sql, conn))
                {
                    cmd.Parameters.AddWithValue("@start", start.ToString("MM-dd"));
                    cmd.Parameters.AddWithValue("@end", end.ToString("MM-dd"));
                    using (var reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                            result.Add(MapContact(reader));
                    }
                }
            }
            return result;
        }

        public void ExportContactsToCsv(int userId, string filePath)
        {
            var contacts = GetContactsByUserId(userId);
            var sb = new StringBuilder();
            sb.AppendLine("FirstName,LastName,Phone,Email,Address,Note,Birthday,Tags");
            foreach (var c in contacts)
            {
                string tags = string.Join(";", c.TagNames);
                sb.AppendLine($"\"{EscapeCsv(c.FirstName)}\",\"{EscapeCsv(c.LastName)}\",\"{EscapeCsv(c.Phone)}\",\"{EscapeCsv(c.Email)}\",\"{EscapeCsv(c.Address)}\",\"{EscapeCsv(c.Note)}\",\"{c.Birthday:yyyy-MM-dd}\",\"{EscapeCsv(tags)}\"");
            }
            File.WriteAllText(filePath, sb.ToString(), Encoding.UTF8);
        }

        public void ImportContactsFromCsv(int userId, string filePath)
        {
            var lines = File.ReadAllLines(filePath, Encoding.UTF8);
            if (lines.Length < 2) return;
            for (int i = 1; i < lines.Length; i++)
            {
                if (string.IsNullOrWhiteSpace(lines[i])) continue;
                var values = ParseCsvLine(lines[i]);
                if (values.Count < 7) continue;
                var contact = new Contact
                {
                    UserId = userId,
                    FirstName = UnescapeCsv(values[0]),
                    LastName = UnescapeCsv(values[1]),
                    Phone = UnescapeCsv(values[2]),
                    Email = UnescapeCsv(values[3]),
                    Address = UnescapeCsv(values[4]),
                    Note = UnescapeCsv(values[5]),
                    Birthday = DateTime.TryParse(UnescapeCsv(values[6]), out var bd) ? bd : (DateTime?)null,
                    TagNames = values.Count > 7 ? UnescapeCsv(values[7]).Split(new char[] { ';' }, StringSplitOptions.RemoveEmptyEntries).ToList() : new List<string>()
                };
                AddContact(contact);
            }
        }

        private Contact MapContact(SQLiteDataReader reader)
        {
            Contact c = new Contact();
            c.Id = reader.GetInt32(reader.GetOrdinal("Id"));
            c.UserId = reader.GetInt32(reader.GetOrdinal("UserId"));
            c.FirstName = reader.GetString(reader.GetOrdinal("FirstName"));
            c.LastName = reader.IsDBNull(reader.GetOrdinal("LastName")) ? "" : reader.GetString(reader.GetOrdinal("LastName"));
            c.Phone = reader.IsDBNull(reader.GetOrdinal("Phone")) ? "" : reader.GetString(reader.GetOrdinal("Phone"));
            c.Email = reader.IsDBNull(reader.GetOrdinal("Email")) ? "" : reader.GetString(reader.GetOrdinal("Email"));
            c.Address = reader.IsDBNull(reader.GetOrdinal("Address")) ? "" : reader.GetString(reader.GetOrdinal("Address"));
            c.Note = reader.IsDBNull(reader.GetOrdinal("Note")) ? "" : reader.GetString(reader.GetOrdinal("Note"));
            if (!reader.IsDBNull(reader.GetOrdinal("Birthday")))
                c.Birthday = DateTime.Parse(reader.GetString(reader.GetOrdinal("Birthday")));
            c.CreatedAt = DateTime.Parse(reader.GetString(reader.GetOrdinal("CreatedAt")));
            c.UpdatedAt = DateTime.Parse(reader.GetString(reader.GetOrdinal("UpdatedAt")));
            return c;
        }

        private string EscapeCsv(string s) => s?.Replace("\"", "\"\"") ?? "";
        private string UnescapeCsv(string s) => s?.Replace("\"\"", "\"") ?? "";

        private List<string> ParseCsvLine(string line)
        {
            var result = new List<string>();
            bool inQuotes = false;
            var current = new StringBuilder();
            for (int i = 0; i < line.Length; i++)
            {
                char c = line[i];
                if (c == '"')
                {
                    if (i + 1 < line.Length && line[i + 1] == '"') { current.Append('"'); i++; }
                    else inQuotes = !inQuotes;
                }
                else if (c == ',' && !inQuotes)
                {
                    result.Add(current.ToString());
                    current.Clear();
                }
                else current.Append(c);
            }
            result.Add(current.ToString());
            return result;
        }
    }
}