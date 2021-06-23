using Terminal.Gui;
using System.Collections.Generic;
using Microsoft.Data.Sqlite;

namespace UserInterface
{
    class EditCourseDialog : CreateCourseDialoig
    {
        public EditCourseDialog(DataProcessor.User user, DataProcessor.Course course1) : base(user, course1)
        {
            this.Title = "Edit product";
        }
        public void SetCourse(DataProcessor.Course course)
        {
            this.CourseTitle.Text = course.title; 
            this.isAvaliable.Checked = course.avaliableForEnrolling;
        }

    }
}