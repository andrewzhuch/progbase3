using Microsoft.Data.Sqlite;
using System;
using System.Collections.Generic;

namespace DataProcessor
{
    public class CourseRepo
    {
        private SqliteConnection _connection;
        public CourseRepo(SqliteConnection connection)
        {
            this._connection = connection;
        }
        public Course GetCourseByID(long id)
        {
            SqliteCommand command = this._connection.CreateCommand();
            command.CommandText = @"SELECT * FROM courses WHERE id = $id";
            command.Parameters.AddWithValue("$id", id);
            SqliteDataReader reader = command.ExecuteReader();
            if(reader.Read())
            {
                Course cource = new Course();
                cource.id = long.Parse(reader.GetString(0));
                cource.title = reader.GetString(1);
                cource.enrolled = int.Parse(reader.GetString(2));
                cource.createdAt = DateTime.Parse(reader.GetString(3));
                cource.authorID = long.Parse(reader.GetString(4));
                cource.avaliableForEnrolling = bool.Parse(reader.GetString(5));
                reader.Close();
                return cource;
            }
            else
            {
                reader.Close();
                return null;
            }
        }
        public bool DeleteById(long id)
        {
            DeleteEnrollingsAfterDeletingCourse(id);
            SqliteCommand command = this._connection.CreateCommand();
            LectureRepo repo = new LectureRepo(this._connection);
            List<Lecture> lectures = repo.GetAllLecturesOfCourse(id);
            foreach(Lecture a in lectures)
            {
                repo.UpdateRelationsAfterDeletedCourseWithLecture(a);
            }
            command.CommandText = @"DELETE FROM courses WHERE id = $id";
            command.Parameters.AddWithValue("$id", id);
            int nChanges = command.ExecuteNonQuery();
            return nChanges == 1;
        }
        public long InsertCourse(Course course)
        {
            SqliteCommand command = this._connection.CreateCommand();
            command.CommandText = 
            @"
                INSERT INTO courses (title, enrolled, createdAt, authorID, avaliableForEnrolling) 
                VALUES ($title, $enrolled, $createdAt, $authorID, $avaliableForEnrolling);

                SELECT last_insert_rowid();
            ";
            command.Parameters.AddWithValue("$title", course.title);
            command.Parameters.AddWithValue("$enrolled", 0);
            command.Parameters.AddWithValue("$createdAt", course.createdAt.ToString("o"));
            command.Parameters.AddWithValue("$authorID", course.authorID);
            command.Parameters.AddWithValue("$avaliableForEnrolling", course.avaliableForEnrolling.ToString());
            long newId = (long)command.ExecuteScalar();
            return newId;
        }
        public bool UpdateCourse(Course course)
        {
            SqliteCommand command = this._connection.CreateCommand();
            command.CommandText = @"UPDATE courses SET 
            title = $title, enrolled = $enrolled, createdAt = $createdAt, authorID = $authorID, avaliableForEnrolling = $avaliableForEnrolling
            WHERE id = $id;";
            command.Parameters.AddWithValue("$title", course.title);
            command.Parameters.AddWithValue("$enrolled", course.enrolled);
            command.Parameters.AddWithValue("$createdAt", course.createdAt.ToString("o"));
            command.Parameters.AddWithValue("$authorID", course.authorID);
            command.Parameters.AddWithValue("$avaliableForEnrolling", course.avaliableForEnrolling.ToString());
            command.Parameters.AddWithValue("$id", course.id);
            int nChanges = command.ExecuteNonQuery();
            return nChanges == 1;
        }
        public List<Course> GetAllCReatedCourcesByUserID(long ID)
        {
            SqliteCommand command = this._connection.CreateCommand();
            command.CommandText = @"SELECT * FROM courses WHERE authorID = $id";
            command.Parameters.AddWithValue("$id", ID);
            SqliteDataReader reader = command.ExecuteReader();
            List<Course> courses = new List<Course>();
            while(reader.Read())
            {
                Course course = new Course();
                course.id = long.Parse(reader.GetString(0));
                course.title = reader.GetString(1);
                course.enrolled = int.Parse(reader.GetString(2));
                course.createdAt = DateTime.Parse(reader.GetString(3));
                course.authorID = long.Parse(reader.GetString(4));
                course.avaliableForEnrolling = bool.Parse(reader.GetString(5));
                courses.Add(course);
            }
            reader.Close();
            return courses;
        }
        public int UpdateRelationsAfterDeletedUser(Course course)
        {
            SqliteCommand command = this._connection.CreateCommand();
            command.CommandText = @"UPDATE courses SET 
            title = $title, enrolled = $enrolled, createdAt = $createdAt, authorID = $authorID, avaliableForEnrolling = $avaliableForEnrolling
            WHERE authorID = $id;";
            command.Parameters.AddWithValue("$title", course.title);
            command.Parameters.AddWithValue("$enrolled", course.enrolled);
            command.Parameters.AddWithValue("$createdAt", course.createdAt.ToString("o"));
            command.Parameters.AddWithValue("$authorID", 0);
            command.Parameters.AddWithValue("$avaliableForEnrolling", course.avaliableForEnrolling);
            command.Parameters.AddWithValue("$id", course.author.id);
            int nChanges = command.ExecuteNonQuery();
            return nChanges;
        }
        public List<Course> GetCoursesEnrolledByUser(long userID)
        {
            SqliteCommand command = this._connection.CreateCommand();
            command.CommandText = @"SELECT * 
            FROM users CROSS JOIN users_courses
            WHERE users.id = users_courses.userID";
            SqliteDataReader reader = command.ExecuteReader();
            List<Course> courses = new List<Course>();
            while(reader.Read())
            {
                if(int.Parse(reader.GetString(0)) != userID)
                {
                    continue;
                }
                else
                {
                    courses.Add(this.GetCourseByID(int.Parse(reader.GetString(6))));
                }
            }
            reader.Close();
            return courses;
        }
        public bool DeleteEnrollingsAfterDeletingCourse(long ID)
        {
            SqliteCommand command = this._connection.CreateCommand();
            command.CommandText = @"DELETE FROM users_courses WHERE courseID = $id";
            command.Parameters.AddWithValue("$id", ID);
            int nChanges = command.ExecuteNonQuery();
            return nChanges == 1;
        }
        public List<Course> GetDataForExport(string word)
        {
            SqliteCommand command = this._connection.CreateCommand();
            command.CommandText = @"SELECT * FROM courses WHERE title LIKE '%' || $value || '%' ";
            command.Parameters.AddWithValue("$value", word);
            SqliteDataReader reader = command.ExecuteReader();
            List<Course> courses = new List<Course>();
            while(reader.Read())
            {
                Course course = new Course();
                course.id = long.Parse(reader.GetString(0));
                course.title = reader.GetString(1);
                course.enrolled = int.Parse(reader.GetString(2));
                course.createdAt = DateTime.Parse(reader.GetString(3));
                course.authorID = long.Parse(reader.GetString(4));
                course.avaliableForEnrolling = bool.Parse(reader.GetString(5));
                LectureRepo repo1 = new LectureRepo(this._connection);
                UserRepo repo2 = new UserRepo(this._connection);
                course.author = repo2.GetUserByID(course.authorID);
                course.enrolledUsers = repo2.GetEnrolledUsresByCourseID(course.id);
                course.lectures = repo1.GetAllLecturesOfCourse(course.id);
                courses.Add(course);
            }
            reader.Close();
            return courses;
        }
        public int GetTotalPages()
        {
            SqliteCommand command = this._connection.CreateCommand();
            command.CommandText = @"SELECT COUNT(*) FROM courses";
            long count = (long)command.ExecuteScalar();
            const int pageSize = 3;
            return (int)Math.Ceiling(count / (double)pageSize);
        }
        public int GetTotalSearchedBooks(String word)
        {
            SqliteCommand command = this._connection.CreateCommand();
            command.CommandText = @"SELECT COUNT(*) FROM courses WHERE title LIKE '%' || $value || '%' ";
            command.Parameters.AddWithValue("$value", word);
            long count = (long)command.ExecuteScalar();
            const int pageSize = 3;
            return (int)Math.Ceiling(count / (double)pageSize);  
        }
        public List<DataProcessor.Course> GetPage(int pageNumber)
        {
            const int pageSize = 3;
            SqliteCommand command = this._connection.CreateCommand();
            command.CommandText = @"SELECT * FROM courses LIMIT $pageSize OFFSET $pageSize * ($pageNumber - 1)";
            command.Parameters.AddWithValue("$pageSize", pageSize);
            command.Parameters.AddWithValue("$pageNumber", pageNumber);
            SqliteDataReader reader = command.ExecuteReader();
            List<DataProcessor.Course> courses = new List<DataProcessor.Course>();
            while(reader.Read())
            {
                DataProcessor.Course course = new DataProcessor.Course();
                course.id = int.Parse(reader.GetString(0));
                course.title = reader.GetString(1);
                course.enrolled = int.Parse(reader.GetString(2));
                course.createdAt = DateTime.Parse(reader.GetString(3));
                course.authorID = int.Parse(reader.GetString(4));
                course.avaliableForEnrolling = bool.Parse(reader.GetString(5));
                courses.Add(course);
            }
            reader.Close();
            return courses;
        }
        public bool CheckAuthorship(long courseID, long userID)
        {
            SqliteCommand command = this._connection.CreateCommand(); 
            command.CommandText = @"SELECT * FROM courses WHERE id = $courseID AND authorID = $userID";
            command.Parameters.AddWithValue("$courseID", courseID);
            command.Parameters.AddWithValue("$userID", userID);
            SqliteDataReader reader = command.ExecuteReader();
            if(reader.Read())
            {
                return true;
            }
            else
            {
                return false;
            }
        }
        public bool CheckSubscribition(long courseID, long userID)
        {
            SqliteCommand command = this._connection.CreateCommand(); 
            command.CommandText = @"SELECT * FROM users_courses WHERE userID = $userID AND courseID = $courseID"; 
            command.Parameters.AddWithValue("$courseID", courseID);
            command.Parameters.AddWithValue("$userID", userID);
            SqliteDataReader reader = command.ExecuteReader();
            if(reader.Read())
            {
                return true;
            }
            else
            {
                return false;
            }
        }
        public void InsertSubscripiton(long userID, long courseID)
        {
            SqliteCommand command = this._connection.CreateCommand();
            command.CommandText = 
            @"
                INSERT INTO users_courses (userID, courseID) 
                VALUES ($userID, $courseID);

                SELECT last_insert_rowid();
            ";
            command.Parameters.AddWithValue("$userID", userID);
            command.Parameters.AddWithValue("$courseID", courseID);
            long newId = (long)command.ExecuteScalar();
        }
        public bool DeleteSubscription(long userID, long courseID)
        {
            SqliteCommand command = this._connection.CreateCommand();
            command.CommandText = @"DELETE FROM users_courses WHERE userID = $userID AND courseID = $courseID";
            command.Parameters.AddWithValue("$userID", userID);
            command.Parameters.AddWithValue("$courseID", courseID);
            int nChanges = command.ExecuteNonQuery();
            return nChanges == 1;
        }
        public List<Course> GetSearchedBooks(string word)
        {
            SqliteCommand command = this._connection.CreateCommand();
            command.CommandText = @"SELECT * FROM courses WHERE title LIKE '%' || $value || '%' ";
            command.Parameters.AddWithValue("$value", word);
            SqliteDataReader reader = command.ExecuteReader();
            List<Course> courses = new List<Course>();
            while(reader.Read())
            {
                DataProcessor.Course course = new DataProcessor.Course();
                course.id = int.Parse(reader.GetString(0));
                course.title = reader.GetString(1);
                course.enrolled = int.Parse(reader.GetString(2));
                course.createdAt = DateTime.Parse(reader.GetString(3));
                course.authorID = int.Parse(reader.GetString(4));
                course.avaliableForEnrolling = bool.Parse(reader.GetString(5));
                courses.Add(course);
            }
            reader.Close();
            if(courses.Count == 0)
            {
                return null;
            }
            return courses;
        }
    }
}