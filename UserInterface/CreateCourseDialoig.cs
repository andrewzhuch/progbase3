using System;
using Terminal.Gui;
using System.Collections.Generic;
using System.Collections;

namespace UserInterface
{
    public class CreateCourseDialoig : Dialog
    {
        private DataProcessor.User user;
        public bool canceled;
        protected TextField CourseTitle;
        protected CheckBox isAvaliable;
        protected DataProcessor.Course course1;
        public CreateCourseDialoig(DataProcessor.User user, DataProcessor.Course course)
        {
            course1 = course;
            this.user = user;
            this.Title = "Create product";
            Button okBtn = new Button("OK");
            okBtn.Clicked += OnCreatedDialogSubmit;
            Button cancelBtn = new Button("Cancel");
            cancelBtn.Clicked += OnCreatedDialogCanceled;
            this.AddButton(okBtn);
            this.AddButton(cancelBtn);
            Label courseTitleLabel = new Label(2, 2, "Title");
            CourseTitle = new TextField("")
            {
                X = 20, Y = Pos.Top(courseTitleLabel),
                Width = 40,
            };
            this.Add(courseTitleLabel, CourseTitle);
            Label avaliableForenroolingLabel = new Label(2, 18, "Can enroll");
            isAvaliable = new CheckBox("")
            {
                X = 20, Y = Pos.Top(avaliableForenroolingLabel), Width = 40,
            };
            this.Add(avaliableForenroolingLabel, isAvaliable);
        }
        private void OnCreatedDialogCanceled()
        {
            this.canceled = true;
            Application.RequestStop();
        }
        private void OnCreatedDialogSubmit()
        {
            this.canceled = false;
            Application.RequestStop();
        }
        public DataProcessor.Course GetCourse()
        {
            return new DataProcessor.Course()
            {
                id = this.course1.id,
                authorID = user.id,
                title = CourseTitle.Text.ToString(),
                avaliableForEnrolling = isAvaliable.Checked,
                createdAt = DateTime.Now
            };
        }
    }
}