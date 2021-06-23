using System;
using Terminal.Gui;
using System.Security.Cryptography;
using System.Text;
using Microsoft.Data.Sqlite;

namespace UserInterface
{
    public class LoginDialog : Dialog
    {
        public bool loggedIn = false;
        public TextField userNameInput;
        public TextField passwordInput; 
        public LoginDialog()
        {
            this.Title = "Log in";
            Button OkButton = new Button("OK");
            OkButton.Clicked += OnOkbuttonClcked;
            this.AddButton(OkButton);
            Label userNameLabel = new Label(2, 2, "UserName");
            userNameInput = new TextField("")
            {
                X = 20, Y = Pos.Top(userNameLabel),
                Width = 40,
            };
            this.Add(userNameLabel, userNameInput);
            Label passwordLabel = new Label(2, 4, "Password");
            passwordInput = new TextField("")
            {
                X = 20, Y = 4,
                Width = 40,
                Secret = true
            };
            this.Add(passwordLabel, passwordInput);
            Button cancelButton = new Button("Cancel");
            cancelButton.Clicked += OnCancelButtonCLicked;
            this.AddButton(cancelButton);
        }
        private void OnCancelButtonCLicked()
        {
            Application.RequestStop();
        }
        private void OnOkbuttonClcked()
        {
            if(this.GetUser() == null)
            {
                int resultButtonIndex = MessageBox.ErrorQuery(
                "Error",
                "Wrong password or/and username",
                "OK");
                loggedIn = false; 
                Application.RequestStop();
            }
            else
            {
                loggedIn = true;
                Application.RequestStop();
            }
        }
        private static string GetHash(HashAlgorithm hashAlgorithm, string input)
        {
            byte[] data = hashAlgorithm.ComputeHash(Encoding.UTF8.GetBytes(input));
            var sBuilder = new StringBuilder();
            for (int i = 0; i < data.Length; i++)
            {
            sBuilder.Append(data[i].ToString("x2"));
        }
        return sBuilder.ToString();
        }
        public DataProcessor.User GetUser()
        {
            string source = passwordInput.Text.ToString();
            SHA256 sha256Hash = SHA256.Create();
            string hash = GetHash(sha256Hash, source);
            string path = "./../data/courses&lectures&users.db";
            SqliteConnection connection = new SqliteConnection($"Data Source={path}");
            connection.Open();
            DataProcessor.UserRepo repo = new DataProcessor.UserRepo(connection);
            DataProcessor.User user = repo.GetUserByNameAndPassword(userNameInput.Text.ToString(), hash);
            connection.Close();
            return user;
        }
    }
}