using Terminal.Gui;
using System.Collections.Generic;
using Microsoft.Data.Sqlite;

namespace UserInterface
{
    public class OpenCourseDialog : Dialog
    {
        private DataProcessor.User currentUser;
        public bool edited;
        protected DataProcessor.Course course;
        public bool deleted;
        private TextField courseId;
        private TextField title;
        private TextField enrolled;
        private TextField createdAt;
        private TextField authorID;
        private TextField avaliableForEnrolling;
        private TextField areUserEnrolled;
        public OpenCourseDialog(DataProcessor.User user, DataProcessor.Course course1)
        {
            currentUser = user;
            Button backButton = new Button("Back");
            backButton.Clicked += OnOpenCourseDialogSubmit;
            this.AddButton(backButton);
            Label courseIdLabel = new Label(2, 2, "ID");
            courseId = new TextField("")
            {
                X = 20, Y = Pos.Top(courseIdLabel),
                Width = 40,
                ReadOnly = true,
            };
            this.Add(courseIdLabel, courseId);
            Label titleLabel = new Label(2, 4, "Title");
            title = new TextField()
            {
                X = 20,
                Y = Pos.Top(titleLabel),
                Width = 40,
                Height = 10,
                ReadOnly = true,
            };
            this.Add(titleLabel, title);
            Label enrolledLabel = new Label(2, 6, "Number of enrolled");
            enrolled = new TextField("")
            {
                X = 20, Y = Pos.Top(enrolledLabel),
                Width = 40,
                ReadOnly = true,
            };
            this.Add(enrolledLabel, enrolled);
            Label createdAtLabel = new Label(2, 8, "createdAt");
            createdAt = new TextField("")
            {
                X = 20, Y = Pos.Top(createdAtLabel), Width = 40, ReadOnly = true,
            };
            this.Add(createdAtLabel, createdAt);
            Label authorIDlabel = new Label(2, 10, "authorID");
            authorID = new TextField("")
            {
                X = 20, Y = Pos.Top(authorIDlabel), Width = 40,
                ReadOnly = true,
            };
            this.Add(authorIDlabel, authorID);
            Label avaliableForEnrollingLabel = new Label(2, 12, "Avaliability");
            avaliableForEnrolling = new TextField("")
            {
                X = 20, Y = Pos.Top(avaliableForEnrollingLabel), Width = 40,
                ReadOnly = true,
            };
            this.Add(avaliableForEnrollingLabel, avaliableForEnrolling);
            Label areUserEnrolledLabel = new Label(2, 14, "Are you enrolled");
            areUserEnrolled = new TextField("")
            {
                X = 20, Y = Pos.Top(areUserEnrolledLabel), Width = 40,
                ReadOnly = true,
            };
            this.Add(areUserEnrolledLabel, areUserEnrolled);
            string path = "./../data/courses&lectures&users.db";
            SqliteConnection connection = new SqliteConnection($"Data Source = {path}");
            connection.Open();
            DataProcessor.CourseRepo repo2 = new DataProcessor.CourseRepo(connection);
            if(repo2.CheckAuthorship(course1.id, currentUser.id))
            {
                Button button1 = new Button(12, 24, "Edit");
                button1.Clicked += OnCourseEdit;
                this.Add(button1);
                Button button = new Button(2, 24, "Delete");
                button.Clicked += OnCourseDelete;
                this.Add(button);
            }
            if(course1.avaliableForEnrolling == true && repo2.CheckAuthorship(course1.id, currentUser.id) == false)
            {
                Button button3 = new Button(32, 24, "Subscribe/unsubscribe");
                button3.Clicked += OnSubscribition;
                this.Add(button3);
            }
            connection.Close();
        }
        private void OnSubscribition()
        {
            if(this.areUserEnrolled.Text == "True")
            {
                string path = "./../data/courses&lectures&users.db";
                SqliteConnection connection = new SqliteConnection($"Data Source = {path}");
                connection.Open();
                DataProcessor.CourseRepo repo2 = new DataProcessor.CourseRepo(connection);
                this.course.enrolled--;
                this.enrolled.Text = this.course.enrolled.ToString();
                repo2.DeleteSubscription(this.currentUser.id, this.course.id);
                this.areUserEnrolled.Text = "False";
                repo2.UpdateCourse(this.course);
                connection.Close();
            }
            else
            {
                this.areUserEnrolled.Text = "True";
                string path = "./../data/courses&lectures&users.db";
                SqliteConnection connection = new SqliteConnection($"Data Source = {path}");
                connection.Open();
                DataProcessor.CourseRepo repo2 = new DataProcessor.CourseRepo(connection);
                this.course.enrolled += 1;
                this.enrolled.Text = this.course.enrolled.ToString();
                repo2.InsertSubscripiton(this.currentUser.id, this.course.id);
                repo2.UpdateCourse(this.course);
                connection.Close();
            }
        }
        private void OnOpenCourseDialogSubmit()
        {
            Application.RequestStop();
        }
        private void OnCourseEdit()
        {
            EditCourseDialog dialog = new EditCourseDialog(this.currentUser, this.course);
            dialog.SetCourse(this.course);
            Application.Run(dialog);
            if(!dialog.canceled)
            {
                DataProcessor.Course newCourse = dialog.GetCourse();
                this.edited = true;
                this.SetProduct(newCourse);
                string path = "./../data/courses&lectures&users.db";
                SqliteConnection connection = new SqliteConnection($"Data Source = {path}");
                connection.Open();
                DataProcessor.CourseRepo repo2 = new DataProcessor.CourseRepo(connection);
                repo2.UpdateCourse(newCourse);
                connection.Close();
            }
        }
        private void OnCourseDelete()
        {
            int index = MessageBox.Query("Delete course", "Are you sure?", "No", "Yes");
            if(index == 1)
            {
                string path = "./../data/courses&lectures&users.db";
                SqliteConnection connection = new SqliteConnection($"Data Source = {path}");
                connection.Open();
                DataProcessor.CourseRepo repo2 = new DataProcessor.CourseRepo(connection);
                repo2.DeleteById(course.id);
                this.deleted = true;
                Application.RequestStop();
                connection.Close();
            }
        }
        public void SetProduct(DataProcessor.Course course)
        {
            string path = "./../data/courses&lectures&users.db";
            SqliteConnection connection = new SqliteConnection($"Data Source = {path}");
            connection.Open();
            DataProcessor.CourseRepo repo2 = new DataProcessor.CourseRepo(connection);
            if(course == null)
            {
                return;
            }
            this.course = course;
            this.courseId.Text = course.id.ToString();;
            this.title.Text = course.title.ToString();
            this.enrolled.Text = course.enrolled.ToString();
            this.createdAt.Text = course.createdAt.ToString();
            this.authorID.Text = course.authorID.ToString();
            this.avaliableForEnrolling.Text = course.avaliableForEnrolling.ToString();
            this.areUserEnrolled.Text = repo2.CheckSubscribition(course.id, this.currentUser.id).ToString();
        }
    }
}