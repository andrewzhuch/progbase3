using Terminal.Gui;
using System.Collections.Generic;
using Microsoft.Data.Sqlite;

namespace UserInterface
{
    public class SearchBookWindow : Window
    {
        private TextField searchword;
        private DataProcessor.User user;
        private ListView pageOfSearchedCoursesView;
        private int page = 1;
        private Label totalPagesPabel;
        private Label pagePabel;
        private int pageLength = 3;
        public SearchBookWindow(DataProcessor.User user)
        {
            Label searchwordLabel = new Label(2,2, "Word to search:");
            searchword = new TextField("")
            {
                X = 2, Y = 4,
                Width = 40,
            };
            this.Add(searchwordLabel, searchword);
            this.user = user;
            this.Title = "Search";
            pageOfSearchedCoursesView = new ListView(new List<DataProcessor.Course>())
            {
                Width = Dim.Fill(),
                Height = Dim.Fill()
            };
            this.Add(pageOfSearchedCoursesView);
            pageOfSearchedCoursesView.OpenSelectedItem += OnOpenCourse;
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
            frameView.Add(pageOfSearchedCoursesView);
            this.Add(frameView);
            Button searchbutton = new Button("Search");
            searchbutton.Clicked += OnSearch;
            this.Add(searchbutton);
            Button okButton = new Button(10, 30, "OK");
            okButton.Clicked += OnOkButton;
            this.Add(okButton);
        }
        private void OnOkButton()
        {
            Application.RequestStop();
        }
        private void OnSearch()
        {
            string path = "./../data/courses&lectures&users.db";
            SqliteConnection connection = new SqliteConnection($"Data Source={path}");
            connection.Open();
            DataProcessor.CourseRepo repo = new DataProcessor.CourseRepo(connection);
            this.pagePabel.Text = page.ToString();
            this.totalPagesPabel.Text = repo.GetTotalSearchedBooks(this.searchword.Text.ToString()).ToString();
            if(repo.GetSearchedBooks(this.searchword.Text.ToString()) == null)
            {
                int resultButtonIndex = MessageBox.Query(
                "Error",
                "There is no courses with this word",
                "OK");
            }
            else
            {
                this.pageOfSearchedCoursesView.SetSource(repo.GetSearchedBooks(this.searchword.Text.ToString()));
            }
        }
        private void OnOpenCourse(ListViewItemEventArgs args)
        {
            DataProcessor.Course course = (DataProcessor.Course)args.Value;
            OpenCourseDialog dialog = new OpenCourseDialog(this.user, course);
            dialog.SetProduct(course);
            Application.Run(dialog);
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
            DataProcessor.CourseRepo repo = new DataProcessor.CourseRepo(connection);
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
            DataProcessor.CourseRepo repo = new DataProcessor.CourseRepo(connection);
            this.pagePabel.Text = page.ToString();
            this.totalPagesPabel.Text = repo.GetTotalPages().ToString();
            this.pageOfSearchedCoursesView.SetSource(repo.GetPage(page));
            connection.Close();
        }
    }
}