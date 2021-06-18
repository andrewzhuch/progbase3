using System;
using Microsoft.Data.Sqlite;
using System.Xml.Serialization;
using System.IO;
using System.Collections.Generic;

namespace DataProcessor
{
    public static class ExportModule
    {
        public static void ExportData(string word, string pathToDB, string exportFile)
        {
            SqliteConnection connection = new SqliteConnection($"Data Source = {pathToDB}");
            connection.Open();
            CourseRepo repo = new CourseRepo(connection);
            List<Course> courses = repo.GetDataForExport(word);
            connection.Close();
            XmlSerializer ser = new XmlSerializer(typeof(List<Course>));
            StreamWriter writer = new StreamWriter(exportFile);
            ser.Serialize(writer, courses);
            writer.Close();
        } 
    }
}
