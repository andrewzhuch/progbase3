using Microsoft.Data.Sqlite;
using DataProcessor;
using System.Collections.Generic;
using System.Xml.Serialization;
using System.IO;
using Terminal.Gui;
using static DataProcessor.ImportModule;
using System;

namespace UserInterface
{
    class UserInterface
    {
        static void Main()
        {
            Application.Init();
            RegistrationDialog regDialog = new RegistrationDialog();
            Toplevel top = Application.Top;
            string path = "./../data/courses&lectures&users.db";
            top.Add(regDialog);
            try
            {
                SqliteConnection connectioncheck = new SqliteConnection($"Data Source={path}");
                connectioncheck.Open();
                connectioncheck.Close();
            }
            catch(Exception)
            {
                int resultButtonIndex = MessageBox.ErrorQuery(
                "Error",
                "There is no Database file!",
                "OK");
                return;
            }
            Application.Run();
            SqliteConnection connection = new SqliteConnection($"Data Source={path}");
            connection.Open();
            if(regDialog.registered == true)
            {
                DataProcessor.UserRepo repo = new UserRepo(connection);
                repo.InsertUser(regDialog.user);
            }
            MainWindow mWindow = new MainWindow(regDialog.user);
            DataProcessor.CourseRepo repo1 = new CourseRepo(connection);
            mWindow.SetList(repo1.GetPage(1));
            top.Add(mWindow);
            Application.Run();
            connection.Close();
        }
    }
}
