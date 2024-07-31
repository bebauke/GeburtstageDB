using System;
using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Input;
using Geburtstage.Models;
using Geburtstage.Services;
using Geburtstage.Views;
using Microsoft.Win32;
using System.IO;
using System.Collections.Generic;
using System.Linq;

namespace Geburtstage.ViewModels
{
    public class MainWindowViewModel : BaseViewModel
    {
        private readonly PersonService _personService;
        private readonly RelationshipService _relationshipService;
        private Person _selectedPerson;

        public ObservableCollection<Person> Persons { get; }
        public Person SelectedPerson
        {
            get => _selectedPerson;
            set
            {
                _selectedPerson = value;
                OnPropertyChanged(nameof(SelectedPerson));
            }
        }

        public ICommand LoadPersonsCommand { get; }
        public ICommand SavePersonCommand { get; }
        public ICommand InitializeDatabaseCommand { get; }
        public ICommand ResetDatabaseCommand { get; }
        public ICommand ExportDataAsVCardCommand { get; }
        public ICommand ImportDataFromVCardCommand { get; }
        public ICommand ExportDataAsCsvCommand { get; }
        public ICommand ImportDataFromCsvCommand { get; }
        public ICommand ShowPlotCommand { get; }
        public ICommand ExitCommand { get; }
        public ICommand AddPersonCommand { get; }
        public ICommand RemovePersonCommand { get; }
        public ICommand EditPersonCommand { get; }

        public MainWindowViewModel()
        {
            try
            {
                _personService = new PersonService();
                _relationshipService = new RelationshipService();
                Persons = new ObservableCollection<Person>();
                LoadPersonsCommand = new RelayCommand(_ => LoadPersons());
                SavePersonCommand = new RelayCommand(_ => SavePerson(), _ => SelectedPerson != null);
                InitializeDatabaseCommand = new RelayCommand(_ => InitializeDatabase());
                ResetDatabaseCommand = new RelayCommand(_ => ResetDatabase());
                ExportDataAsVCardCommand = new RelayCommand(_ => ExportDataAsVCard());
                ImportDataFromVCardCommand = new RelayCommand(_ => ImportDataFromVCard());
                ExportDataAsCsvCommand = new RelayCommand(_ => ExportDataAsCsv());
                ImportDataFromCsvCommand = new RelayCommand(_ => ImportDataFromCsv());
                ShowPlotCommand = new RelayCommand(_ => new PlotView().Show());
                ExitCommand = new RelayCommand(_ => Application.Current.Shutdown());
                AddPersonCommand = new RelayCommand(_ => AddPerson());
                RemovePersonCommand = new RelayCommand(_ => RemovePerson(), _ => SelectedPerson != null);
                EditPersonCommand = new RelayCommand(_ => EditPerson(), _ => SelectedPerson != null);

                LoadPersons();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Fehler bei der Initialisierung des ViewModels: {ex.Message}", "Initialisierungsfehler", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void LoadPersons()
        {
            try
            {
                Persons.Clear();
                var persons = _personService.Get();
                foreach (var person in persons)
                {
                    Persons.Add(person);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Fehler beim Laden der Personen: {ex.Message}", "Ladefehler", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void SavePerson()
        {
            try
            {
                if (SelectedPerson.Id == null)
                {
                    _personService.Create(SelectedPerson);
                }
                else
                {
                    _personService.Update(SelectedPerson.Id, SelectedPerson);
                }

                LoadPersons();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Fehler beim Speichern der Person: {ex.Message}", "Speicherfehler", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void InitializeDatabase()
        {
            try
            {
                var persons = new List<Person>
                {
                    new Person
                    {
                        FirstName = "Max",
                        LastName = "Musterman",
                        DateOfBirth = new DateTime(1990, 1, 1),
                        Contact = new Contact
                        {
                            Addresses = new List<LabeledValue<Address>>
                            {
                                new LabeledValue<Address>
                                {
                                    Label = "Home",
                                    Value = new Address
                                    {
                                        Street = "Musterstraße 123",
                                        City = "Musterstadt",
                                        PostalCode = "12345",
                                        Country = "Deutschland"
                                    }
                                }
                            },
                            Emails = new List<LabeledValue<string>>
                            {
                                new LabeledValue<string> { Label = "Personal", Value = "john.doe@example.com" }
                            },
                            PhoneNumbers = new List<LabeledValue<string>>
                            {
                                new LabeledValue<string> { Label = "Mobile", Value = "123-456-7890" }
                            },
                            URLs = new List<LabeledValue<string>>
                            {
                                new LabeledValue<string> { Label = "Website", Value = "http://johndoe.com" }
                            }
                        }
                    },
                    new Person
                    {
                        FirstName = "Jane",
                        LastName = "Doe",
                        DateOfBirth = new DateTime(1995, 5, 5),
                        Contact = new Contact
                        {
                            Addresses = new List<LabeledValue<Address>>
                            {
                                new LabeledValue<Address>
                                {
                                    Label = "Work",
                                    Value = new Address
                                    {
                                        Street = "Main Street 123",
                                        City = "Springfield",
                                        PostalCode = "12345",
                                        Country = "USA"
                                    }
                                }
                            },
                            Emails = new List<LabeledValue<string>>
                            {
                                new LabeledValue<string> { Label = "Work", Value = "jane.doe@work.com" }
                            },
                            PhoneNumbers = new List<LabeledValue<string>>
                            {
                                new LabeledValue<string> { Label = "Home", Value = "987-654-3210" }
                            },
                            URLs = new List<LabeledValue<string>>
                            {
                                new LabeledValue<string> { Label = "LinkedIn", Value = "http://linkedin.com/in/janedoe" }
                            }
                        }
                    }
                };

                var relationships = new List<Relationship>
                    {
                        new Relationship { PersonId1 = persons[0].Id, PersonId2 = persons[1].Id, Type = "Partner" }
                    };

                    foreach (var person in persons)
                    {
                        _personService.Create(person);
                    }

                    foreach (var relationship in relationships)
                    {
                        _relationshipService.Create(relationship);
                    }

                    MessageBox.Show("Datenbank erfolgreich initialisiert.", "Erfolg", MessageBoxButton.OK, MessageBoxImage.Information);

                    LoadPersons();
                }
            catch (Exception ex)
            {
                MessageBox.Show($"Fehler bei der Initialisierung der Datenbank: {ex.Message}", "Initialisierungsfehler", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void ResetDatabase()
        {
            try
            {
                _personService.Clear();
                _relationshipService.Clear();

                MessageBox.Show("Datenbank erfolgreich zurückgesetzt.", "Erfolg", MessageBoxButton.OK, MessageBoxImage.Information);

                LoadPersons();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Fehler beim Zurücksetzen der Datenbank: {ex.Message}", "Zurücksetzungsfehler", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void ExportDataAsVCard()
        {
            try
            {
                var exportService = new ExportService();

                var saveFileDialog = new SaveFileDialog
                {
                    Filter = "vCard files (*.vcf)|*.vcf",
                    FileName = "data.vcf"
                };
                var persons = _personService.Get();

                if (saveFileDialog.ShowDialog() == true)
                {
                    exportService.ExportToVCard(persons, saveFileDialog.FileName);
                    MessageBox.Show("Daten erfolgreich exportiert.", "Erfolg", MessageBoxButton.OK, MessageBoxImage.Information);
                }
            }
            
            catch (Exception ex)
            {
                MessageBox.Show($"Fehler beim Exportieren der Daten: {ex.Message}", "Exportfehler", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void ImportDataFromVCard()
        {
            try
            {
                var importService = new ImportService(_personService);

                var openFileDialog = new OpenFileDialog
                {
                    // Folders and .vcf files are allowed
                    Filter = "vCard files (*.vcf)|*.vcf",
                    Multiselect = true
                };
                var persons = new List<Person>();

                if (openFileDialog.ShowDialog() == true)
                {
                 

                    persons = importService.ImportFromVCard(openFileDialog.FileNames.ToList<string>());
                    foreach (var person in persons)
                    {
                        _personService.Create(person);
                    }
                    MessageBox.Show("Daten erfolgreich importiert.", "Erfolg", MessageBoxButton.OK, MessageBoxImage.Information);
                    LoadPersons();
                }
            }

            catch (Exception ex)
            {
                MessageBox.Show($"Fehler beim Importieren der Daten: {ex.Message}", "Importfehler", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void ExportDataAsCsv()
        {
            try
            {
                var exportService = new ExportService();

                var saveFileDialog = new SaveFileDialog
                {
                    Filter = "CSV files (*.csv)|*.csv",
                    FileName = "data.csv"
                };
                var persons = _personService.Get();

                if (saveFileDialog.ShowDialog() == true)
                {
                    exportService.ExportToCsv(persons, saveFileDialog.FileName);
                    MessageBox.Show("Daten erfolgreich exportiert.", "Erfolg", MessageBoxButton.OK, MessageBoxImage.Information);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Fehler beim Exportieren der Daten: {ex.Message}", "Exportfehler", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void ImportDataFromCsv()
        {
            try
            {
                var importService = new ImportService(_personService);

                var openFileDialog = new OpenFileDialog
                {
                    Filter = "CSV files (*.csv)|*.csv",
                    Multiselect = false
                };
                var persons = new List<Person>();

                if (openFileDialog.ShowDialog() == true)
                {
                    persons = importService.ImportFromCsv(openFileDialog.FileName);
                    foreach (var person in persons)
                    {
                        _personService.Create(person);
                    }
                    MessageBox.Show("Daten erfolgreich importiert.", "Erfolg", MessageBoxButton.OK, MessageBoxImage.Information);
                    LoadPersons();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Fehler beim Importieren der Daten: {ex.Message}", "Importfehler", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }


        private void AddPerson()
        {
            var newPerson = new Person();
            var viewModel = new EditPersonViewModel(newPerson, _personService, _relationshipService); // Übergabe des RelationshipService
            var editPersonView = new EditPersonView(newPerson, _personService, _relationshipService); // Korrekte Übergabe der Parameter
            editPersonView.DataContext = viewModel; // Setzen des DataContext

            if (editPersonView.ShowDialog() == true)
            {
                Persons.Add(newPerson);
            }
        }

        private void RemovePerson()
        {
            if (SelectedPerson != null)
            {
                _personService.Remove(SelectedPerson.Id);
                Persons.Remove(SelectedPerson);
            }
        }

        private void EditPerson()
        {
            if (SelectedPerson != null)
            {
                var personService = new PersonService();
                var relationshipService = new RelationshipService();
                var editPersonView = new EditPersonView(SelectedPerson, personService, relationshipService);
                editPersonView.ShowDialog();
                LoadPersons(); // Nach Bearbeitung neu laden
            }
        }
    }
}
