using Npgsql;
using System;
using System.Collections.ObjectModel;
using System.IO;
using System.Text.Json;
using System.Windows;
using System.Windows.Input;
using Microsoft.Win32;

namespace TalkHub
{
    public class UserInfoViewModel : ObservableObject
    {
        private const string ConnectionString = "Host=localhost;Username=postgres;Password=sa;Database=talkhub2.0";

        public ObservableCollection<User> Users { get; } = new ObservableCollection<User>();

        public ICommand LoadUsersCommand { get; }
        public ICommand SaveToJsonCommand { get; }
        public ICommand LoadFromJsonCommand { get; }
        public ICommand AddUserCommand { get; }

        public UserInfoViewModel()
        {
            LoadUsersCommand = new RelayCommand(_ => LoadUsers());
            SaveToJsonCommand = new RelayCommand(_ => SaveToJson());
            LoadFromJsonCommand = new RelayCommand(_ => LoadFromJson());
            AddUserCommand = new RelayCommand(_ => AddUser());

            LoadUsers();
        }

        private void LoadUsers()
        {
            Users.Clear();

            try
            {
                using var conn = new NpgsqlConnection(ConnectionString);
                conn.Open();

                using var cmd = new NpgsqlCommand("SELECT user_id, username, email, password_hash, created_at, last_login FROM users", conn);
                using var reader = cmd.ExecuteReader();

                while (reader.Read())
                {
                    Users.Add(new User
                    {
                        UserId = reader.GetInt32(0),
                        Username = reader.GetString(1),
                        Email = reader.GetString(2),
                        PasswordHash = reader.GetString(3),
                        CreatedAt = reader.GetDateTime(4),
                        LastLogin = reader.GetDateTime(5)
                    });
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка загрузки пользователей из базы данных:\n{ex.Message}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void SaveToJson()
        {
            var saveFileDialog = new SaveFileDialog
            {
                Filter = "JSON files (*.json)|*.json",
                DefaultExt = "json",
                FileName = "users.json"
            };

            if (saveFileDialog.ShowDialog() == true)
            {
                try
                {
                    var options = new JsonSerializerOptions { WriteIndented = true };
                    string jsonString = JsonSerializer.Serialize(Users, options);
                    File.WriteAllText(saveFileDialog.FileName, jsonString);
                    MessageBox.Show("Данные успешно сохранены в JSON файл.", "Успех", MessageBoxButton.OK, MessageBoxImage.Information);
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Ошибка при сохранении файла:\n{ex.Message}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }

        private void LoadFromJson()
        {
            var openFileDialog = new OpenFileDialog
            {
                Filter = "JSON files (*.json)|*.json",
                DefaultExt = "json"
            };

            if (openFileDialog.ShowDialog() == true)
            {
                try
                {
                    string jsonString = File.ReadAllText(openFileDialog.FileName);
                    var loadedUsers = JsonSerializer.Deserialize<ObservableCollection<User>>(jsonString);

                    if (loadedUsers != null)
                    {
                        Users.Clear();
                        foreach (var user in loadedUsers)
                        {
                            Users.Add(user);
                        }
                        MessageBox.Show("Данные успешно загружены из JSON файла.", "Успех", MessageBoxButton.OK, MessageBoxImage.Information);
                    }
                    else
                    {
                        MessageBox.Show("Файл не содержит корректных данных.", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Warning);
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Ошибка при загрузке файла:\n{ex.Message}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }

        private void AddUser()
        {
            var addUserWindow = new AddUser();
            addUserWindow.Owner = Application.Current.MainWindow;
            if (addUserWindow.ShowDialog() == true)
            {
                LoadUsers();
            }
        }
    }

    public class User
    {
        public int UserId { get; set; }
        public string Username { get; set; }
        public string Email { get; set; }
        public string PasswordHash { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime LastLogin { get; set; }
    }
} 

