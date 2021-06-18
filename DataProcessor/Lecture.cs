using System;
namespace DataProcessor
{
    public class Lecture
    {
        public long id;
        public string name;
        public string topic;
        public DateTime createdAt;
        public Course cource;
        public override string ToString()
        {
            return $"id = {id}, name = {name}, topic = {topic}, created at = {createdAt}";
        }
    }
}