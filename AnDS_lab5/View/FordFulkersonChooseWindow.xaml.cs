using System.Collections.ObjectModel;
using System.Windows;
using AnDS_lab5.ViewModel;

namespace AnDS_lab5.View;

public partial class FordFulkersonChooseWindow
{
    public VertexViewModel? SelectedItem1 = null;
    public VertexViewModel? SelectedItem2 = null;
    
    public FordFulkersonChooseWindow(ObservableCollection<VertexViewModel> vertices)
    {
        InitializeComponent();
        ComboBox1.ItemsSource = vertices;
        ComboBox1.SelectedItem = SelectedItem1;
        ComboBox2.ItemsSource = vertices;
        ComboBox2.SelectedItem = SelectedItem2;
    }
    
    private void Accept_Click(object sender, RoutedEventArgs e)
        => DialogResult = true;
}