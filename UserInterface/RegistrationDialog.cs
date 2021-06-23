using System;
using Terminal.Gui;
using System.Security.Cryptography;
using System.Text;
using Microsoft.Data.Sqlite;

namespace UserInterface
{
    public class RegistrationDialog : Dialog
    {
        public bool registered = false;
        public DataProcessor.User user = null;
        public TextField userNameInput;
        public TextField passwordInput;
        public TextField ageInput;
        public RegistrationDialog()
        {
            this.Title = "Sign up";
            Button loginButton = new Button("Log in");
            loginButton.Clicked += OnLoginButtonCLicked;
            this.AddButton(loginButton);
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
            Button okButton = new Button("Ok");
            okButton.Clicked += OkButtonClicked;
            Label ageLabel = new Label(2, 6, "Age");
            ageInput = new TextField("")
            {
                X = 20, Y = 6,
                Width = 40,
            };
            this.Add(ageLabel, ageInput);
            this.AddButton(okButton);
            Button cancelButton = new Button("Exit");
            cancelButton.Clicked += OnButtonCanceled;
            this.AddButton(cancelButton);
        }
        private void OnButtonCanceled()
        {
            Application.RequestStop();
        }
        private void OnLoginButtonCLicked()
        {
            LoginDialog loginDialog = new LoginDialog();
            Application.Run(loginDialog);
            user = loginDialog.GetUser();
            if(loginDialog.loggedIn == true)
            {
                Application.RequestStop();
            }
        }
        private void OkButtonClicked()
        {
            string path = "./../data/courses&lectures&users.db";
            SqliteConnection connection = new SqliteConnection($"Data Source={path}");
            connection.Open();
            DataProcessor.UserRepo repo = new DataProcessor.UserRepo(connection);
            string source = passwordInput.Text.ToString();
            SHA256 sha256Hash = SHA256.Create();
            string hash = GetHash(sha256Hash, source);
            if(repo.GetUserByNameAndPassword(userNameInput.Text.ToString(), hash) != null)
            {
                ageInput.Text = "";
                userNameInput.Text = "";
                passwordInput.Text = "";
                registered = false;
                MessageBox.ErrorQuery("Error", "User is alredy exists", "OK");
                return;
            }
            int k;
            bool check = int.TryParse(ageInput.Text.ToString(), out k);
            if(check == false)
            {
                ageInput.Text = "";
                registered = false;
                MessageBox.ErrorQuery("Error", "Age should be an integer", "OK");
                return;
            }
            registered = true;
            user = this.GetUser();
            connection.Close();
            Application.RequestStop();
        }
        public DataProcessor.User GetUser()
        {
            string source = passwordInput.Text.ToString();
            SHA256 sha256Hash = SHA256.Create();
            string hash = GetHash(sha256Hash, source);
            user = new DataProcessor.User()
            {
                username = userNameInput.Text.ToString(),
                password = hash,
                age = int.Parse(ageInput.Text.ToString()),
                registeredAt = DateTime.Now
            };
            return user;
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
    }
}