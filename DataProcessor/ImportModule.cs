using Microsoft.Data.Sqlite;
using System;
using System.Collections.Generic;

namespace DataProcessor
{
    public static class ImportModule
    {
        public static void ImportSingleCourse(Course course, string pathToDB)
        {
            SqliteConnection connection = new SqliteConnection($"Data Source = {pathToDB}");
            connection.Open();
            CourseRepo repo = new CourseRepo(connection);  
            UserRepo repo1 = new UserRepo(connection);
            if(repo.GetCourseByID(course.id) != null)
            {
                connection.Close();
                return;
            }
            else
            {
                if(repo1.GetUserByID(course.authorID) == null)
                {
                    course.authorID = 0;
                }
                SqliteCommand command = connection.CreateCommand();
                command.CommandText = 
                @"
                    INSERT INTO courses (id, title, enrolled, createdAt, authorID, avaliableForEnrolling) 
                    VALUES ($id, $title, $enrolled, $createdAt, $authorID, $avaliableForEnrolling);

                    SELECT last_insert_rowid();
                ";
                command.Parameters.AddWithValue("$id", course.id);
                command.Parameters.AddWithValue("$title", course.title);
                command.Parameters.AddWithValue("$enrolled", course.enrolled);
                command.Parameters.AddWithValue("$createdAt", course.createdAt.ToString("o"));
                command.Parameters.AddWithValue("$authorID", course.authorID);
                command.Parameters.AddWithValue("$avaliableForEnrolling", course.avaliableForEnrolling.ToString());
                long newId = (long)command.ExecuteScalar();
            }
            connection.Close();
        }
        public static void ImportSingleLecture(Lecture lecture, long courseID, string pathToDB)
        {
            SqliteConnection connection = new SqliteConnection($"Data Source = {pathToDB}");
            connection.Open();
            LectureRepo repo = new LectureRepo(connection);
            if(repo.GetLectureByID(lecture.id) != null)
            {
                connection.Close();
                return;
            }
            else
            {
                SqliteCommand command = connection.CreateCommand();
                command.CommandText = 
                @"
                    INSERT INTO lectures (id, name, topic, createdAT, courseID) 
                    VALUES ($id, $name, $topic, $createdAT, $courseID);

                    SELECT last_insert_rowid();
                ";
                command.Parameters.AddWithValue("$id", lecture.id);
                command.Parameters.AddWithValue("$name", lecture.name);
                command.Parameters.AddWithValue("$topic", lecture.topic);
                command.Parameters.AddWithValue("$createdAT", lecture.createdAt.ToString("o"));
                command.Parameters.AddWithValue("$courseID", courseID);
                long newId = (long)command.ExecuteScalar();
            }
            connection.Close();
        }
        public static void ImportSingleUser(User user, string pathToDB)
        {
            SqliteConnection connection = new SqliteConnection($"Data Source = {pathToDB}");
            connection.Open();
            UserRepo repo1 = new UserRepo(connection);
            if(repo1.GetUserByID(user.id) != null)
            {
                connection.Close();
                return;
            }
            else
            {
                SqliteCommand command = connection.CreateCommand();
                command.CommandText = 
                @"
                    INSERT INTO users (id, username, password, age, registeredAt) 
                    VALUES ($id, $username, $password, $age, $registeredAt);

                    SELECT last_insert_rowid();
                ";
                command.Parameters.AddWithValue("$id", user.id);
                command.Parameters.AddWithValue("$username", user.username);
                command.Parameters.AddWithValue("$password", user.password);
                command.Parameters.AddWithValue("$age", user.age);
                command.Parameters.AddWithValue("$registeredAt", user.registeredAt.ToString("o"));
                long newId = (long)command.ExecuteScalar();
            }
            connection.Close();
        }
        public static void ImportEnrollings(User user, long courceID, string pathToDB)
        {
            SqliteConnection connection = new SqliteConnection($"Data Source = {pathToDB}");
            connection.Open();
            SqliteCommand command = connection.CreateCommand();
            command.CommandText = 
            @"
                INSERT INTO users_courses (userID, courseID) 
                VALUES ($userID, $courseID);

                SELECT last_insert_rowid();
            ";
            command.Parameters.AddWithValue("$userID", user.id);
            command.Parameters.AddWithValue("$courseID", courceID);
            long newId = (long)command.ExecuteScalar();
            connection.Close();
        }  
    }
}