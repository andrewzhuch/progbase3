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
    public class LecrturesWindow : Window
    {
        private DataProcessor.User user;
        private ListView pageOfCoursesView;
        private List<DataProcessor.Lecture> lecturesList;
        private int page = 1;
        private Label totalPagesPabel;
        private Label pagePabel;
        private int pageLength = 3;
        public LecrturesWindow(DataProcessor.User user)
        {
            Button exit = new Button(2, 2, "Exit");
            exit.Clicked += OnExit;
            this.Add(exit);
            this.user = user;
            this.Title = "Lectures";
            pageOfCoursesView = new ListView(new List<DataProcessor.Lecture>())
            {
                Width = Dim.Fill(),
                Height = Dim.Fill()
            };
            Button prevButton = new Button(2, 6, "Prev");
            prevButton.Clicked += OnPreviousPage;
            pagePabel = new Label("?")
            {
                X = Pos.Right(prevButton) + 2,
                Y = Pos.Top(prevButton),
                Width = 5,
            };
            totalPagesPabel = new Label("?")
            {
                X = Pos.Right(pagePabel) + 2,
                Y = Pos.Top(prevButton),
                Width = 5,
            };
            Button nextButton = new Button("Next")
            {
                X = Pos.Right(totalPagesPabel) + 2,
                Y = Pos.Top(prevButton),
            };
            nextButton.Clicked += OnNextPage;
            this.Add(prevButton, nextButton, totalPagesPabel, pagePabel);
            FrameView frameView = new FrameView("All courses")
            {
                X = 2,
                Y = 8,
                Width = Dim.Fill() -  4,
                Height = pageLength + 2
            };
            frameView.Add(pageOfCoursesView);
            this.Add(frameView);
        }
        public void OnExit()
        {
            Application.RequestStop();
        }
        public void OnPreviousPage()
        {
            if(page == 1)
            {
                return;
            }
            this.page--;
            ShowCurrentPage();
        }
        public void OnNextPage()
        {
            string path = "./../data/courses&lectures&users.db";
            SqliteConnection connection = new SqliteConnection($"Data Source={path}");
            connection.Open();
            DataProcessor.LectureRepo repo = new DataProcessor.LectureRepo(connection);
            int totalPages = repo.GetTotalPages();
            if(page >= totalPages)
            {
                return;
            }
            this.page++;
            ShowCurrentPage();
            connection.Close();
        }
        public void ShowCurrentPage()
        {
            string path = "./../data/courses&lectures&users.db";
            SqliteConnection connection = new SqliteConnection($"Data Source={path}");
            connection.Open();
            DataProcessor.LectureRepo repo = new DataProcessor.LectureRepo(connection);
            this.pagePabel.Text = page.ToString();
            this.totalPagesPabel.Text = repo.GetTotalPages().ToString();
            this.pageOfCoursesView.SetSource(repo.GetPage(page));
            connection.Close();
        }
        public void SetList(List<DataProcessor.Lecture> list)
        {
            this.lecturesList = list;
            this.ShowCurrentPage();
        }
    }
}