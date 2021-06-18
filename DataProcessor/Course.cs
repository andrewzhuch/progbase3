using System;
using System.Collections.Generic;

namespace DataProcessor
{
    public class Course
    {
        public long id;
        public string title;
        public int enrolled;
        public DateTime createdAt;
        public long authorID;
        public User author;
        public bool avaliableForEnrolling;
        public List<User> enrolledUsers;
        public List<Lecture> lectures;
        public override string ToString()
        {
            return $"id = {id}, title = {title}, enrolled = {enrolled}, createdAt = {createdAt}, author ID = {authorID}, avaliable for enrolling = {avaliableForEnrolling} ";
        }
    }
}