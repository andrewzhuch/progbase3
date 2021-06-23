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
    public class MainWindow : Window
    {
        private DataProcessor.User user;
        private ListView pageOfCoursesView;
        private List<DataProcessor.Course> productsList;
        private int page = 1;
        private Label totalPagesPabel;
        private Label pagePabel;
        private int pageLength = 3;
        public MainWindow(DataProcessor.User user)
        {
            Button importButton = new Button(2, 40, "Import");
            importButton.Clicked += OnImport;
            this.Add(importButton);
            Button exportbutton = new Button(2, 30, "Export");
            exportbutton.Clicked += OnExport;this.Add(exportbutton);
            this.user = user;
            this.Title = "Courses and lectures";
            pageOfCoursesView = new ListView(new List<DataProcessor.Course>())
            {
                Width = Dim.Fill(),
                Height = Dim.Fill()
            };
            pageOfCoursesView.OpenSelectedItem += OnOpenCourse;
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
            Button searchBook = new Button("Search");
            searchBook.Clicked += OnSearch;
            this.Add(searchBook);
            Button createCourse = new Button(30, 30, "Create course");
            createCourse.Clicked += OnCreateCourse;
            this.Add(createCourse);
            Button Lectures = new Button(25, 25, "See lectures");
            Lectures.Clicked += OnLectures;
            this.Add(Lectures);

        }
        public void OnLectures()
        {
            LecrturesWindow win = new LecrturesWindow(this.user);
            string path = "./../data/courses&lectures&users.db";
            SqliteConnection connection = new SqliteConnection($"Data Source={path}");
            connection.Open();
            DataProcessor.LectureRepo repo = new DataProcessor.LectureRepo(connection);
            win.SetList(repo.GetPage(1));
            connection.Close();
            Application.Run(win);
        }
        public void OnCreateCourse()
        {
            CreateCourseDialoig dialoig = new CreateCourseDialoig(this.user, null);
            Application.Run(dialoig);
            string path = "./../data/courses&lectures&users.db";
            SqliteConnection connection = new SqliteConnection($"Data Source={path}");
            connection.Open();
            DataProcessor.CourseRepo repo = new DataProcessor.CourseRepo(connection);
            repo.InsertCourse(dialoig.GetCourse());
            ShowCurrentPage();
            connection.Close();
        }
        public void OnSearch()
        {
            SearchBookWindow win = new SearchBookWindow(this.user);
            Application.Run(win);
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
            this.pageOfCoursesView.SetSource(repo.GetPage(page));
            connection.Close();
        }
        public void SetList(List<DataProcessor.Course> list)
        {
            this.productsList = list;
            this.ShowCurrentPage();
        }
        private void OnOpenCourse(ListViewItemEventArgs args)
        {
            DataProcessor.Course course = (DataProcessor.Course)args.Value;
            OpenCourseDialog dialog = new OpenCourseDialog(this.user, course);
            dialog.SetProduct(course);
            Application.Run(dialog);
            if(dialog.deleted == true)
            {
                ShowCurrentPage();
            }
            ShowCurrentPage();
        }
        private void OnExport()
        {
            string path = "./../data/courses&lectures&users.db";
            ExportDialog export = new ExportDialog();
            Application.Run(export);
            DataProcessor.ExportModule.ExportData(export.word.Text.ToString(), path,export.fileLabel.Text.ToString());
        }
        private void OnImport()
        {
            string path = "./../data/courses&lectures&users.db";
            ImportDialog import =  new ImportDialog();
            Application.Run(import);
            string importFile = import.fileLabel.Text.ToString();
            XmlSerializer ser = new XmlSerializer(typeof(List<Course>));
            XmlSerializer ser1 = new XmlSerializer(typeof(List<Course>));
            StreamReader reader = new StreamReader(importFile);
            StreamReader reader1 = new StreamReader(importFile);
            try
            {
                List<Course> courses1 = (List<Course>)ser1.Deserialize(reader1);
                reader1.Close();
            }
            catch(Exception)
            {
                int resultButtonIndex = MessageBox.ErrorQuery(
                "Error",
                "Wrong file",
                "OK");
                return;
            }
            List<Course> courses = (List<Course>)ser.Deserialize(reader);
            reader.Close();
            foreach(Course course in courses)
            {
                ImportModule.ImportSingleUser(course.author, path);
                foreach(User user in course.enrolledUsers)
                {
                    ImportSingleUser(user, path);
                }
                foreach(Lecture lecture in course.lectures)
                {
                    ImportSingleLecture(lecture, course.id, path);
                }
                ImportSingleCourse(course, path);
                foreach(User user in course.enrolledUsers)
                {
                    ImportEnrollings(user, course.id, path);
                }
            }
            ShowCurrentPage();
        }
    }
}
