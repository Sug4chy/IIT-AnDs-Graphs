using System.Collections.ObjectModel;
using System.Windows;
using AnDS_lab5.ViewModel;

namespace AnDS_lab5.View;

public partial class DijkstraChooseWindow
{
    public DijkstraChooseWindow(ObservableCollection<VertexViewModel> vertices)
    {
        InitializeComponent();
        ComboBox1.ItemsSource = vertices;
        ComboBox2.ItemsSource = vertices;
    }
    
    private void Accept_Click(object sender, RoutedEventArgs e)
        => DialogResult = true;
}