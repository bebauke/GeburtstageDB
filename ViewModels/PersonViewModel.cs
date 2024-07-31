using System.Windows.Input;
using Geburtstage.Models;
using Geburtstage.Services;

namespace Geburtstage.ViewModels
{
    public class PersonViewModel : BaseViewModel
    {
        private readonly PersonService _personService;
        private Person _selectedPerson;

        public Person SelectedPerson
        {
            get => _selectedPerson;
            set
            {
                _selectedPerson = value;
                OnPropertyChanged(nameof(SelectedPerson));
            }
        }

        public ICommand SavePersonCommand { get; }

        public PersonViewModel(Person person)
        {
            _personService = new PersonService();
            SelectedPerson = person;

            SavePersonCommand = new RelayCommand(_ => SavePerson());
        }

        private void SavePerson()
        {
            if (SelectedPerson.Id == null)
            {
                _personService.Create(SelectedPerson);
            }
            else
            {
                _personService.Update(SelectedPerson.Id, SelectedPerson);
            }
        }
    }
}
