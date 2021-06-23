using Terminal.Gui;

namespace UserInterface
{
    public class ExportDialog : Dialog
    {
        public Label fileLabel;
        public TextField word;
        public ExportDialog()
        {
            Label wordLabel = new Label(5, 5, "Word: ");
            word = new TextField("")
            {
                X = 20, Y = Pos.Top(wordLabel),
                Width = 40,
            };
            this.Add(wordLabel, word);
            Button chooseDirectory = new Button(10, 10, "Open file");
            chooseDirectory.Clicked += SelectDirectory;
            fileLabel = new Label("File not selected")
            {
                X = 2,
                Y = 2,
                Width = Dim.Fill(),
            };
            Button cancelButton = new Button("Cancel");
            cancelButton.Clicked += OnCancelButton;
            this.AddButton(cancelButton);
            this.AddButton(chooseDirectory);
            this.Add(fileLabel);
            Button okButton = new Button("OK");
            okButton.Clicked += OnOkButton;
            this.AddButton(okButton);
        }
        private void OnOkButton()
        {
            Application.RequestStop();
        }
        private void SelectDirectory()
        {
            OpenDialog dialog = new OpenDialog("Open XML file", "Open?");
            Application.Run(dialog);
  
            if (!dialog.Canceled)
            {
                NStack.ustring filePath = dialog.FilePath;
                fileLabel.Text = filePath;
            }
            else
            {
                fileLabel.Text = "File not selected";
            }
        }
        private void OnCancelButton()
        {
            Application.RequestStop();
        }
    }
}