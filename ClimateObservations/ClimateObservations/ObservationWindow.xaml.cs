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

namespace ClimateObservations
{
    /// <summary>
    /// Interaction logic for ObservationWindow.xaml
    /// </summary>
    public partial class ObservationWindow : Window
    {
        public readonly ObservableCollection<Observation> ObservationList = new ObservableCollection<Observation>();

        private readonly Observer _observer;
        public ObservationWindow(Observer observer)
        {
            InitializeComponent();
            _observer = observer;

            ListBoxObservations.ItemsSource = ObservationList;
            ListBoxObservations.DisplayMemberPath = nameof(Observation.Date);

            var observations = GetObservations(_observer.Id);
            foreach (var o in observations)
            {
                ObservationList.Add(o);
            }
        }
    }
}
