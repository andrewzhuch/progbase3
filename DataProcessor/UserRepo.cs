using Microsoft.Data.Sqlite;
using System;
using System.Collections.Generic;

namespace DataProcessor
{
    public class UserRepo
    {
        private SqliteConnection _connection;
        public UserRepo(SqliteConnection connection)
        {
            this._connection = connection;
        }
        public User GetUserByID(long id)
        {
            SqliteCommand command = this._connection.CreateCommand();
            command.CommandText = @"SELECT * FROM users WHERE id = $id";
            command.Parameters.AddWithValue("$id", id);
            SqliteDataReader reader = command.ExecuteReader();
            if(reader.Read())
            {
                User user = new User();
                user.id = long.Parse(reader.GetString(0));
                user.username = reader.GetString(1);
                user.password = reader.GetString(2);
                user.age = int.Parse(reader.GetString(3));
                user.registeredAt = DateTime.Parse(reader.GetString(4));
                reader.Close();
                return user;
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
            CourseRepo repo1 = new CourseRepo(this._connection);
            List<Course> courses = repo1.GetAllCReatedCourcesByUserID(id);
            foreach(Course a in courses)
            {
                repo1.UpdateRelationsAfterDeletedUser(a);
            }
            this.DeleteEnrollingsAfterDeleteingUser(id);
            command.CommandText = @"DELETE FROM users WHERE id = $id";
            command.Parameters.AddWithValue("$id", id);
            int nChanges = command.ExecuteNonQuery();
            return nChanges == 1;
        }
        public long InsertUser(User user)
        {
            SqliteCommand command = this._connection.CreateCommand();
            command.CommandText = 
            @"
                INSERT INTO users (username, password, age, registeredAt) 
                VALUES ($username, $password, $age, $registeredAt);

                SELECT last_insert_rowid();
            ";
            command.Parameters.AddWithValue("$username", user.username);
            command.Parameters.AddWithValue("$password", user.password);
            command.Parameters.AddWithValue("$age", user.age);
            command.Parameters.AddWithValue("$registeredAt", user.registeredAt.ToString("o"));
            long newId = (long)command.ExecuteScalar();
            return newId;
        }
        public bool UpdateUser(User user)
        {
            SqliteCommand command = this._connection.CreateCommand();
            command.CommandText = @"UPDATE users SET 
            username = $username, password = $password, age = $age, registeredAt = $registeredAt 
            WHERE id = $id;";
            command.Parameters.AddWithValue("$username", user.username);
            command.Parameters.AddWithValue("$password", user.password);
            command.Parameters.AddWithValue("$age", user.age);
            command.Parameters.AddWithValue("$registeredAt", user.registeredAt);
            command.Parameters.AddWithValue("$id", user.id);
            int nChanges = command.ExecuteNonQuery();
            return nChanges == 1;
        }
        public List<User> GetEnrolledUsresByCourseID(long ID)
        {
            SqliteCommand command = this._connection.CreateCommand();
            command.CommandText = @"SELECT * 
            FROM users CROSS JOIN users_courses
            WHERE users.id = users_courses.userID";
            SqliteDataReader reader = command.ExecuteReader();
            List<User> users = new List<User>();
            while(reader.Read())
            {
                if(int.Parse(reader.GetString(6)) != ID)
                {
                    continue;
                }
                else
                {
                    users.Add(this.GetUserByID(int.Parse(reader.GetString(5))));
                }
            }
            reader.Close();
            return users;
        }
        public bool DeleteEnrollingsAfterDeleteingUser(long userID)
        {
            SqliteCommand command = this._connection.CreateCommand();
            command.CommandText = @"DELETE FROM users_courses WHERE userID = $id";
            command.Parameters.AddWithValue("$id", userID);
            int nChanges = command.ExecuteNonQuery();
            return nChanges == 1;
        }
    }
}