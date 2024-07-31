using Geburtstage;
using Geburtstage.Models;
using Geburtstage.Services;
using Geburtstage.ViewModels;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using System.Windows.Input;
using System;
using MongoDB.Bson;

public class EditPersonViewModel : BaseViewModel
{
    private readonly PersonService _personService;
    private readonly RelationshipService _relationshipService;
    private Person _selectedPerson;
    private bool _isNewPerson;

    // Property für die aktuell ausgewählte Person
    public Person SelectedPerson
    {
        get => _selectedPerson;
        set
        {
            _selectedPerson = value;
            OnPropertyChanged(nameof(SelectedPerson));
        }
    }

    // Properties für die UI (Adressen, Emails, Telefonnummern, Beziehungen) - ObservableCollection für dynamische Listen
    public ObservableCollection<LabeledValue<Address>> Addresses { get; set; }
    public ObservableCollection<LabeledValue<string>> Emails { get; set; }
    public ObservableCollection<LabeledValue<string>> PhoneNumbers { get; set; }
    public ObservableCollection<Tuple<Relationship, LabeledValue<Person>>> RelatedPersons { get; private set; }
    public ObservableCollection<string> RelationshipTypes { get; private set; }
    public ObservableCollection<Person> AllPersons { get; private set; }

    // Commands für die UI (Speichern, Abbrechen, Hinzufügen, Entfernen) - RelayCommand für einfache Commands
    public ICommand SaveCommand { get; }
    public ICommand CancelCommand { get; }
    public ICommand AddAddressCommand { get; }
    public ICommand RemoveAddressCommand { get; }
    public ICommand AddEmailCommand { get; }
    public ICommand RemoveEmailCommand { get; }
    public ICommand AddPhoneNumberCommand { get; }
    public ICommand RemovePhoneNumberCommand { get; }
    public ICommand AddRelationshipCommand { get; }
    public ICommand RemoveRelationshipCommand { get; }

    public EditPersonViewModel(Person person, PersonService personService, RelationshipService relationshipService)
    {
        _selectedPerson = person;
        _personService = personService;
        _relationshipService = relationshipService;
        _isNewPerson = person.Id == null;

        // Initialisiere Listen
        Addresses = new ObservableCollection<LabeledValue<Address>>(_selectedPerson.Contact.Addresses ?? new List<LabeledValue<Address>>());
        Emails = new ObservableCollection<LabeledValue<string>>(_selectedPerson.Contact.Emails ?? new List<LabeledValue<string>>());
        PhoneNumbers = new ObservableCollection<LabeledValue<string>>(_selectedPerson.Contact.PhoneNumbers ?? new List<LabeledValue<string>>());
        RelationshipTypes = new ObservableCollection<string> { "Partner", "Kind", "Eltern", "Geschwister", "Geschäftlich" };
        AllPersons = new ObservableCollection<Person>(_personService.Get());
        RelatedPersons = new ObservableCollection<Tuple<Relationship, LabeledValue<Person>>>();
        LoadRelatedPersons();


        // Initialisieren der Commands
        SaveCommand = new RelayCommand(_ => Save());
        CancelCommand = new RelayCommand(_ => Cancel());
        AddAddressCommand = new RelayCommand(_ => AddAddress());
        AddEmailCommand = new RelayCommand(_ => AddEmail());
        AddPhoneNumberCommand = new RelayCommand(_ => AddPhoneNumber());
        AddRelationshipCommand = new RelayCommand(_ => AddRelationship());
        RemoveAddressCommand = new RelayCommand(address => RemoveAddress((LabeledValue<Address>)address));
        RemoveEmailCommand = new RelayCommand(email => RemoveEmail((LabeledValue<string>)email));
        RemovePhoneNumberCommand = new RelayCommand(phoneNumber => RemovePhoneNumber((LabeledValue<string>)phoneNumber));
        RemoveRelationshipCommand = new RelayCommand(RemoveRelationship);

    }

    private void AddAddress()
    {
        var newAddress = new LabeledValue<Address> { Label = "New", Value = new Address() };
        _selectedPerson.Contact.Addresses.Add(newAddress);
        Addresses.Add(newAddress);
    }

    private void Save()
    {
        SelectedPerson.Contact.Addresses = new List<LabeledValue<Address>>(Addresses);
        SelectedPerson.Contact.Emails = new List<LabeledValue<string>>(Emails);
        SelectedPerson.Contact.PhoneNumbers = new List<LabeledValue<string>>(PhoneNumbers);

        if (_isNewPerson)
        {
            _personService.Create(SelectedPerson);
        }
        else
        {
            _personService.Update(SelectedPerson.Id, SelectedPerson);
        }

        // Beziehungen speichern
        if (SaveRelationships())
        {
            CloseWindow();
        }
    }

    private void Cancel()
    {
        if (_isNewPerson)
        {
            SelectedPerson = null;
        }
        CloseWindow();
    }

    private void AddEmail()
    {
        Emails.Add(new LabeledValue<string> { Label = "Privat", Value = string.Empty });
    }

    private void AddPhoneNumber()
    {
        PhoneNumbers.Add(new LabeledValue<string> { Label = "Privat", Value = string.Empty });
    }

    private void RemoveAddress(LabeledValue<Address> address)
    {
        Addresses.Remove(address);
        SelectedPerson.Contact.Addresses.Remove(address);
    }

    private void RemoveEmail(LabeledValue<string> email)
    {
        Emails.Remove(email);
        SelectedPerson.Contact.Emails.Remove(email);
    }

    private void RemovePhoneNumber(LabeledValue<string> phoneNumber)
    {
        PhoneNumbers.Remove(phoneNumber);
        SelectedPerson.Contact.PhoneNumbers.Remove(phoneNumber);
    }

    private void AddRelationship()
    {
        var newRelationship = new Relationship { PersonId1 = SelectedPerson.Id };
        var relatedPerson = new Person(); // Temporäres Objekt für UI
        RelatedPersons.Add(new Tuple<Relationship, LabeledValue<Person>>(newRelationship, new LabeledValue<Person> { Label = "New", Value = relatedPerson }));
    }

    private void RemoveRelationship(object parameter)
    {
        if (parameter is Tuple<Relationship, LabeledValue<Person>> relatedPersonTuple)
        {
            var relationship = relatedPersonTuple.Item1;
            RelatedPersons.Remove(relatedPersonTuple);
            _relationshipService.Remove(relationship.Id); // Beziehung sofort löschen
        }
    }
    private void LoadRelatedPersons()
    {
        var relationships = _relationshipService.Get().Where(r => r.PersonId1 == SelectedPerson.Id || r.PersonId2 == SelectedPerson.Id).ToList();

        foreach (var relationship in relationships)
        {
            Person relatedPerson = _personService.Get(relationship.PersonId1 == SelectedPerson.Id ? relationship.PersonId2 : relationship.PersonId1);
            string relatedPersonLabel = relationship.Type;
            if (relatedPersonLabel == "Eltern-Kind")
            {
                relatedPersonLabel = relationship.PersonId1 == SelectedPerson.Id ? "Kind" : "Eltern";
            }

            RelatedPersons.Add(new Tuple<Relationship, LabeledValue<Person>>(relationship, new LabeledValue<Person> { Label = relatedPersonLabel, Value = relatedPerson }));
        }

        OnPropertyChanged(nameof(RelatedPersons));
    }

    private bool SaveRelationships()
    {
        var existingRelationships = _relationshipService.GetByPersonId(SelectedPerson.Id);
        var relationshipsToUpdate = new List<Relationship>();
        var relationshipsToAdd = new List<Relationship>();

        foreach (var relatedPersonTuple in RelatedPersons)
        {
            var relationship = relatedPersonTuple.Item1;
            var relationLabel = relatedPersonTuple.Item2.Label;
            var relatedPerson = relatedPersonTuple.Item2.Value;

            if (relatedPerson == null || relatedPerson.Id == null)
            {
                // Fehlermeldung anzeigen
                MessageBox.Show("Bitte wählen Sie eine Person aus.", "Fehler", MessageBoxButton.OK, MessageBoxImage.Error);
                return false;
            }

            // Reigenfolge Eltern-Kind Id1 = Eltern, Id2 = Kind
            if (relationLabel == "Eltern")
            {
                relationship.Type = "Eltern-Kind";
                relationship.PersonId1 = relatedPerson.Id;
                relationship.PersonId2 = SelectedPerson.Id;
            }
            else if (relationLabel == "Kind")
            {
                relationship.Type = "Eltern-Kind";
                relationship.PersonId1 = SelectedPerson.Id;
                relationship.PersonId2 = relatedPerson.Id;
            }
            else
            {
                relationship.Type = relationLabel;
                relationship.PersonId1 = SelectedPerson.Id;
                relationship.PersonId2 = relatedPerson.Id;
            }

            if (relationship.Id == ObjectId.Empty)
                relationshipsToAdd.Add(relationship);
            else
                relationshipsToUpdate.Add(relationship);


            // Aktualisiere und füge Beziehungen hinzu
            foreach (var _relationship in relationshipsToUpdate)
            {
                _relationshipService.Update(_relationship.Id, relationship);
            }

            foreach (var _relationship in relationshipsToAdd)
            {
                _relationshipService.Create(_relationship);
            }

        }
        return true;

    }
    private void CloseWindow()
    {
        foreach (Window window in Application.Current.Windows)
        {
            if (window.DataContext == this)
            {
                window.DialogResult = true;
                window.Close();
            }
        }
    }
}
