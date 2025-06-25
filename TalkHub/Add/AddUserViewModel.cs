using Npgsql;
using System;
using System.Security.Cryptography;
using System.Text;
using System.Windows;
using System.Windows.Input;

namespace TalkHub
{
    public class AddUserViewModel : ObservableObject
    {
        private string _userId;
        public string UserId
        {
            get => _userId;
            set { _userId = value; OnPropertyChanged(); }
        }

        private string _username;
        public string Username
        {
            get => _username;
            set { _username = value; OnPropertyChanged(); }
        }

        private string _email;
        public string Email
        {
            get => _email;
            set { _email = value; OnPropertyChanged(); }
        }

        private string _password;
        public string Password
        {
            get => _password;
            set { _password = value; OnPropertyChanged(); }
        }

        private DateTime? _createdAt = DateTime.Now;
        public DateTime? CreatedAt
        {
            get => _createdAt;
            set { _createdAt = value; OnPropertyChanged(); }
        }

        private DateTime? _lastLogin = DateTime.Now;
        public DateTime? LastLogin
        {
            get => _lastLogin;
            set { _lastLogin = value; OnPropertyChanged(); }
        }

        public ICommand AddCommand { get; }

        public AddUserViewModel()
        {
            AddCommand = new RelayCommand(ExecuteAddUser);
        }

        private void ExecuteAddUser(object obj)
        {
            if (!int.TryParse(UserId, out int userId))
            {
                MessageBox.Show("User ID должен быть числом.", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            if (string.IsNullOrWhiteSpace(Username) || string.IsNullOrWhiteSpace(Email) || string.IsNullOrWhiteSpace(Password))
            {
                MessageBox.Show("Поля Username, Email и Password обязательны для заполнения.", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            if (CreatedAt == null || LastLogin == null)
            {
                MessageBox.Show("Пожалуйста, выберите даты Created At и Last Login.", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            string passwordHash = ComputeSha256Hash(Password);

            string connString = "Host=localhost;Username=postgres;Password=sa;Database=talkhub2.0";

            try
            {
                using var conn = new NpgsqlConnection(connString);
                conn.Open();

                string sql = @"INSERT INTO users (user_id, username, email, password_hash, created_at, last_login)
                               VALUES (@user_id, @username, @email, @password_hash, @created_at, @last_login)";

                using var cmd = new NpgsqlCommand(sql, conn);
                cmd.Parameters.AddWithValue("user_id", userId);
                cmd.Parameters.AddWithValue("username", Username);
                cmd.Parameters.AddWithValue("email", Email);
                cmd.Parameters.AddWithValue("password_hash", passwordHash);
                cmd.Parameters.AddWithValue("created_at", CreatedAt.Value);
                cmd.Parameters.AddWithValue("last_login", LastLogin.Value);

                cmd.ExecuteNonQuery();

                MessageBox.Show("Пользователь успешно добавлен.", "Успех", MessageBoxButton.OK, MessageBoxImage.Information);

                // Закрываем окно, если передан параметр окна
                if (obj is Window window)
                {
                    window.DialogResult = true;
                    window.Close();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка при добавлении пользователя: {ex.Message}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private string ComputeSha256Hash(string rawData)
        {
            using var sha256Hash = SHA256.Create();
            byte[] bytes = sha256Hash.ComputeHash(Encoding.UTF8.GetBytes(rawData));
            var builder = new StringBuilder();
            foreach (var b in bytes)
                builder.Append(b.ToString("x2"));
            return builder.ToString();
        }
    }
}
