using System.Windows.Controls;
using System.Windows.Input;
using Geburtstage.ViewModels;

namespace Geburtstage.Views
{
    public partial class PersonsView : UserControl
    {
        public PersonsView()
        {
            InitializeComponent();
        }

        private void DataGrid_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            if (DataContext is MainWindowViewModel viewModel && viewModel.SelectedPerson != null)
            {
                viewModel.EditPersonCommand.Execute(null);
            }
        }
    }
}
