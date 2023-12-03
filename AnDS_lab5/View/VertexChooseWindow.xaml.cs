using System.Windows;
using AnDS_lab5.ViewModel;

namespace AnDS_lab5.View;

public partial class VertexChooseWindow
{
    public VertexViewModel SelectedVertex => (ComboBox.SelectedItem as VertexViewModel)!;

    public VertexChooseWindow(IEnumerable<VertexViewModel> vertices)
    {
        InitializeComponent();
        ComboBox.ItemsSource = vertices;
    }

    private void Accept_Click(object sender, RoutedEventArgs e)
        => DialogResult = true;
}