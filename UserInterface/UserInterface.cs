using Microsoft.Data.Sqlite;
using DataProcessor;
using System.Collections.Generic;
using System.Xml.Serialization;
using System.IO;
using static DataProcessor.ImportModule;

namespace UserInterface
{
    class UserInterface
    {
        static void Main()
        {
            string pathToDB = "./../data/courses&lectures&users.db";
            ImportData(pathToDB, "./data.xml");
        }
        static long InsertUser(User user, string pathToDB)
        {
            SqliteConnection connection = new SqliteConnection($"Data Source = {pathToDB}");
            connection.Open();
            UserRepo repo = new UserRepo(connection);
            long id = repo.InsertUser(user);
            connection.Close();
            return id;
        }
        static bool DeleteUser(int userID, string pathToDB)
        {
            SqliteConnection connection = new SqliteConnection($"Data Source = {pathToDB}");
            connection.Open();
            UserRepo repo = new UserRepo(connection);
            bool result = repo.DeleteById(userID);
            connection.Close();
            return result;
        }
        static bool UpdateUser(User user, string pathToDB)
        {
            SqliteConnection connection = new SqliteConnection($"Data Source = {pathToDB}");
            connection.Open();
            UserRepo repo = new UserRepo(connection);
            bool changed = repo.UpdateUser(user);
            connection.Close();
            return changed;
        }
        static User GetUser(long userID, string pathToDB)
        {
            SqliteConnection connection = new SqliteConnection($"Data Source = {pathToDB}");
            connection.Open();
            UserRepo repo = new UserRepo(connection);
            User user = repo.GetUserByID(userID);
            connection.Close();
            return user;
        }
        static List<User> GetEnrolledUsers(long courceID, string pathToDB)
        {
            SqliteConnection connection = new SqliteConnection($"Data Source = {pathToDB}");
            connection.Open();
            UserRepo repo = new UserRepo(connection);
            List<User> users = repo.GetEnrolledUsresByCourseID(courceID);
            connection.Close();
            return users;
        }
        static List<Course> GetAllCreatedCourses(long userID, string pathToDB)
        {
            SqliteConnection connection = new SqliteConnection($"Data Source = {pathToDB}");
            connection.Open();
            CourseRepo repo = new CourseRepo(connection);
            List<Course> courses = repo.GetAllCReatedCourcesByUserID(userID);
            connection.Close();
            return courses;
        }
        static List<Course> GetEnrolledCoursesByUser(long userID, string pathToDB)
        {
            SqliteConnection connection = new SqliteConnection($"Data Source = {pathToDB}");
            connection.Open();
            CourseRepo repo = new CourseRepo(connection);
            List<Course> courses = repo.GetCoursesEnrolledByUser(userID);
            connection.Close();
            return courses;
        }
        static List<Lecture> GetAllLecturesOfCourse(long courseID, string pathToDB)
        {   
            SqliteConnection connection = new SqliteConnection($"Data Source = {pathToDB}");
            connection.Open();
            LectureRepo repo = new LectureRepo(connection);
            List<Lecture> lectures = repo.GetAllLecturesOfCourse(courseID);
            connection.Close();
            return lectures;
        }
        static Lecture GetLecture(long lectureID, string pathToDB)
        {
            SqliteConnection connection = new SqliteConnection($"Data Source = {pathToDB}");
            connection.Open();
            LectureRepo repo = new LectureRepo(connection);
            Lecture lecture = repo.GetLectureByID(lectureID);
            connection.Close();
            return lecture;  
        }
        static long InsertLecture(Lecture lecture, string pathToDB)
        {
            SqliteConnection connection = new SqliteConnection($"Data Source = {pathToDB}");
            connection.Open();
            LectureRepo repo = new LectureRepo(connection);
            long id = repo.InsertLecture(lecture);
            connection.Close();
            return id;   
        }
        static bool DeleteLecture(long lectureID, string pathToDB)
        {
            SqliteConnection connection = new SqliteConnection($"Data Source = {pathToDB}");
            connection.Open();
            LectureRepo repo = new LectureRepo(connection);
            bool deleted = repo.DeleteById(lectureID);
            connection.Close();
            return deleted; 
        }
        static bool UpdateLecture(Lecture lecture, string pathToDB)
        {
            SqliteConnection connection = new SqliteConnection($"Data Source = {pathToDB}");
            connection.Open();
            LectureRepo repo = new LectureRepo(connection);
            bool updated = repo.UpdateLecture(lecture);
            connection.Close();
            return updated;    
        }
        static Course GetCourse(long id, string pathToDB)
        {
            SqliteConnection connection = new SqliteConnection($"Data Source = {pathToDB}");
            connection.Open();
            CourseRepo repo = new CourseRepo(connection);
            Course course = repo.GetCourseByID(id);
            connection.Close();
            return course; 
        }
        static bool DeleteCourse(long ID, string pathToDB)
        {
            SqliteConnection connection = new SqliteConnection($"Data Source = {pathToDB}");
            connection.Open();
            CourseRepo repo = new CourseRepo(connection);
            bool deleted = repo.DeleteById(ID);
            connection.Close();
            return deleted; 
        }
        static bool UpdateCourse(Course course, string pathToDB)
        {
            SqliteConnection connection = new SqliteConnection($"Data Source = {pathToDB}");
            connection.Open();
            CourseRepo repo = new CourseRepo(connection);
            bool updated = repo.UpdateCourse(course);
            connection.Close();
            return updated; 
        }
        static long InsertCourse(Course course, string pathToDB)
        {
            SqliteConnection connection = new SqliteConnection($"Data Source = {pathToDB}");
            connection.Open();
            CourseRepo repo = new CourseRepo(connection);
            long id = repo.InsertCourse(course);
            connection.Close();
            return id;   
        }
        static void ImportData(string pathToDB, string importFile)
        {
            XmlSerializer ser = new XmlSerializer(typeof(List<Course>));
            StreamReader reader = new StreamReader(importFile);
            List<Course> courses = (List<Course>)ser.Deserialize(reader);
            reader.Close();
            foreach(Course course in courses)
            {
                ImportModule.ImportSingleUser(course.author, pathToDB);
                foreach(User user in course.enrolledUsers)
                {
                    ImportSingleUser(user, pathToDB);
                }
                foreach(Lecture lecture in course.lectures)
                {
                    ImportSingleLecture(lecture, course.id, pathToDB);
                }
                ImportSingleCourse(course, pathToDB);
                foreach(User user in course.enrolledUsers)
                {
                    ImportEnrollings(user, course.id, pathToDB);
                }
            }
        }
    }
}
