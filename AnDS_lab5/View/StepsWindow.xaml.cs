namespace AnDS_lab5.View;

public partial class StepsWindow
{
    public StepsWindow(IEnumerable<string> steps)
    {
        InitializeComponent();
        Steps.ItemsSource = steps;
    }
}