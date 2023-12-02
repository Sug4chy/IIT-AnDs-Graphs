using System.Windows;
using Microsoft.Win32;

namespace AnDS_lab5.Service;

public class DefaultDialogService : IDialogService
{
    public string FilePath { get; set; } = null!;

    public void ShowMessage(string message)
        => MessageBox.Show(message);
    
    public bool OpenFileDialog()
    {
        var openFileDialog = new OpenFileDialog();
        if (openFileDialog.ShowDialog() != true)
        {
            return false;
        }

        FilePath = openFileDialog.FileName;
        return true;

    }

    public bool SaveFileDialog()
    {
        var saveFileDialog = new OpenFolderDialog();
        if (saveFileDialog.ShowDialog() != true)
        {
            return false;
        }

        FilePath = saveFileDialog.FolderName;
        return true;
    }
}