namespace AnDS_lab5.Service;

public interface IDialogService
{
    void ShowMessage(string message);
    string FilePath { get; set; }
    bool OpenFileDialog();
    bool SaveFileDialog();
}