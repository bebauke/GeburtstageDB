using System.Windows;
using Geburtstage.Models;
using Geburtstage.ViewModels;
using Geburtstage.Services;

namespace Geburtstage.Views
{
    public partial class EditPersonView : Window
    {
        public EditPersonView(Person person, PersonService personService, RelationshipService relationshipService)
        {
            InitializeComponent();
            DataContext = new EditPersonViewModel(person, personService, relationshipService);
        }
    }
}
