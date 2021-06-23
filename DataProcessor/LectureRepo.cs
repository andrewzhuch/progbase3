using Microsoft.Data.Sqlite;
using System;
using System.Collections.Generic;

namespace DataProcessor
{
    public class LectureRepo
    {
        private SqliteConnection _connection;
        public LectureRepo(SqliteConnection connection)
        {
            this._connection = connection;
        }
        public Lecture GetLectureByID(long id)
        {
            SqliteCommand command = this._connection.CreateCommand();
            command.CommandText = @"SELECT * FROM lectures WHERE id = $id";
            command.Parameters.AddWithValue("$id", id);
            SqliteDataReader reader = command.ExecuteReader();
            if(reader.Read())
            {
                Lecture lecture = new Lecture();
                lecture.id = long.Parse(reader.GetString(0));
                lecture.name = reader.GetString(1);
                lecture.topic = reader.GetString(2);
                lecture.createdAt = DateTime.Parse(reader.GetString(3));
                CourseRepo repo = new CourseRepo(this._connection);
                lecture.cource = repo.GetCourseByID(long.Parse(reader.GetString(4)));
                reader.Close();
                return lecture;
            }
            else
            {
                reader.Close();
                return null;
            }
        }
        public bool DeleteById(long id)
        {
            SqliteCommand command = this._connection.CreateCommand();
            command.CommandText = @"DELETE FROM lectures WHERE id = $id";
            command.Parameters.AddWithValue("$id", id);
            int nChanges = command.ExecuteNonQuery();
            return nChanges == 1;
        }
        public List<DataProcessor.Lecture> GetPage(int pageNumber)
        {
            const int pageSize = 3;
            SqliteCommand command = this._connection.CreateCommand();
            command.CommandText = @"SELECT * FROM lectures LIMIT $pageSize OFFSET $pageSize * ($pageNumber - 1)";
            command.Parameters.AddWithValue("$pageSize", pageSize);
            command.Parameters.AddWithValue("$pageNumber", pageNumber);
            SqliteDataReader reader = command.ExecuteReader();
            List<DataProcessor.Lecture> lectures = new List<DataProcessor.Lecture>();
            while(reader.Read())
            {
                DataProcessor.Lecture Lecture = new DataProcessor.Lecture();
                Lecture.id = int.Parse(reader.GetString(0));
                Lecture.name = reader.GetString(1);
                Lecture.topic = reader.GetString(2);
                CourseRepo repo1 = new CourseRepo(this._connection);
                Lecture.createdAt = DateTime.Parse(reader.GetString(3));
                Lecture.cource = repo1.GetCourseByID(long.Parse(reader.GetString(4)));
                lectures.Add(Lecture);
            }
            reader.Close();
            return lectures;
        }
        public int GetTotalPages()
        {
            SqliteCommand command = this._connection.CreateCommand();
            command.CommandText = @"SELECT COUNT(*) FROM lectures";
            long count = (long)command.ExecuteScalar();
            const int pageSize = 3;
            return (int)Math.Ceiling(count / (double)pageSize);
        }
        public long InsertLecture(Lecture lecture)
        {
            SqliteCommand command = this._connection.CreateCommand();
            command.CommandText = 
            @"
                INSERT INTO lectures (name, topic, createdAT, courseID) 
                VALUES ($name, $topic, $createdAT, $courseID);

                SELECT last_insert_rowid();
            ";
            command.Parameters.AddWithValue("$name", lecture.name);
            command.Parameters.AddWithValue("$topic", lecture.topic);
            command.Parameters.AddWithValue("$createdAT", lecture.createdAt.ToString("o"));
            command.Parameters.AddWithValue("$courseID", 0);
            long newId = (long)command.ExecuteScalar();
            return newId;
        }
        public bool UpdateLecture(Lecture lecture)
        {
            SqliteCommand command = this._connection.CreateCommand();
            command.CommandText = @"UPDATE lectures SET 
            name = $name, topic = $topic, createdAt = $createdAt
            WHERE id = $id;";
            command.Parameters.AddWithValue("$name", lecture.name);
            command.Parameters.AddWithValue("$topic", lecture.topic);
            command.Parameters.AddWithValue("$createdAt", lecture.createdAt.ToString("o"));
            command.Parameters.AddWithValue("$id", lecture.id);
            int nChanges = command.ExecuteNonQuery();
            return nChanges == 1;
        }
        public List<Lecture> GetAllLecturesOfCourse(long id)
        {
            SqliteCommand command = this._connection.CreateCommand();
            command.CommandText = @"SELECT * FROM lectures WHERE courseID = $id";
            command.Parameters.AddWithValue("$id", id);
            SqliteDataReader reader = command.ExecuteReader();
            List<Lecture> lectures = new List<Lecture>();
            while(reader.Read())
            {
                Lecture lecture = new Lecture();
                lecture.id = long.Parse(reader.GetString(0));
                lecture.name = reader.GetString(1);
                lecture.topic = reader.GetString(2);
                lecture.createdAt = DateTime.Parse(reader.GetString(3));
                lectures.Add(lecture);
            }
            reader.Close();
            return lectures;
        }
        public int UpdateRelationsAfterDeletedCourseWithLecture(Lecture lecture)
        {
            SqliteCommand command = this._connection.CreateCommand();
            command.CommandText = @"UPDATE lectures SET 
            name = $name, topic = $topic, createdAt = $createdAt, courseID = $courseID
            WHERE courseID = $id;";
            command.Parameters.AddWithValue("$name", lecture.name);
            command.Parameters.AddWithValue("$topic", lecture.topic);
            command.Parameters.AddWithValue("$createdAt", lecture.createdAt.ToString("o"));
            command.Parameters.AddWithValue("$courseID", 0);
            command.Parameters.AddWithValue("$id", lecture.cource.id);
            int nChanges = command.ExecuteNonQuery();
            return nChanges;
        }
    }
}