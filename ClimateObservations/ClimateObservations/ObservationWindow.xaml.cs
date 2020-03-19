using ClimateObservations.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using static ClimateObservations.Repositories.ObservationRepository;
using static ClimateObservations.Repositories.GeolocationRepository;
using static ClimateObservations.Repositories.MeasurementRepository;
using static ClimateObservations.Repositories.CategoryRepository;

namespace ClimateObservations
{
    /// <summary>
    /// Interaction logic for ObservationWindow.xaml
    /// </summary>
    public partial class ObservationWindow : Window
    {
        public readonly ObservableCollection<Observation> ObservationList = new ObservableCollection<Observation>();
        public readonly ObservableCollection<Measurement> MeasurementList = new ObservableCollection<Measurement>();
        public readonly ObservableCollection<Geolocation> LocationList = new ObservableCollection<Geolocation>();
        public readonly ObservableCollection<Category> CategoryList = new ObservableCollection<Category>();

        private readonly Observer _observer;
        public ObservationWindow(Observer observer)
        {
            InitializeComponent();
            _observer = observer;

            ObservationsComboBox.ItemsSource = ObservationList;
            ObservationsComboBox.DisplayMemberPath = nameof(Observation.Date);

            var observations = GetObservations(_observer.Id);
            foreach (var o in observations)
            {
                ObservationList.Add(o);
            }

            GeolocationsComboBox.ItemsSource = LocationList;
            GeolocationsComboBox.DisplayMemberPath = nameof(Geolocation.Longitude);

            var geolocations = GetGelocations();
            foreach (var g in geolocations)
            {
                LocationList.Add(g);
            }

            MeasurementsListBox.ItemsSource = MeasurementList;
            MeasurementsListBox.DisplayMemberPath = nameof(Measurement.Value);

            ComboBoxCategory.ItemsSource = CategoryList;
            ComboBoxCategory.DisplayMemberPath = nameof(Category.ToDisplay);

            var categories = GetCategories();
            foreach (var c in categories)
            {
                CategoryList.Add(c);
            }
        }

        private void BtnAddObservation_Click(object sender, RoutedEventArgs e)
        {

        }

        private void ObservationsComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            MeasurementList.Clear();

            var measurements = GetMeasurements(_observer.Id);
            foreach (var m in measurements)
            {
                MeasurementList.Add(m);
            }
        }

        private void BtnAddMeasurement_Click(object sender, RoutedEventArgs e)
        {
            var selectedCategory = ComboBoxCategory.SelectedItem as Category;
            if (selectedCategory == null)
            {
                return;
            }

            double? value = null;
            if (double.TryParse(TxtBoxMeasurementValue.Text, out double v))
            {
                value = v;
            }

            Measurement newM = new Measurement
            {
                ObservationId = _observer.Id,
                Category = selectedCategory,
                Value = value
            };

            newM.Id = AddMeasurement(newM);
            MeasurementList.Add(newM);
        }

        private void BtnUpdateMeasurement_Click(object sender, RoutedEventArgs e)
        {
        }

        private void BtnDeleteMeasurement_Click(object sender, RoutedEventArgs e)
        {
            var selectedMeasurement = MeasurementsListBox.SelectedItem as Measurement;
            if (selectedMeasurement == null)
            {
                return;
            }

            DeleteMeasurement(selectedMeasurement.Id);
        }
    }
}
