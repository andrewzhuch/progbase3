using System;
using System.Collections.Generic;

namespace DataProcessor
{
    public class User
    {
        public long id;
        public string username;
        public string password;
        public int age;
        public DateTime registeredAt;
        public List<Course> enrolledCourses;
        public List<Course> createdCourses;
        public override string ToString()
        {
            return $"id = {id}, username = {username}, password = {password}, age = {age}, registered at = {registeredAt}";
        }
    }
}