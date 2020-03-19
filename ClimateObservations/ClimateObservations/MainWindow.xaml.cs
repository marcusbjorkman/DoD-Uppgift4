using ClimateObservations.Models;
using ClimateObservations.Repositories;
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
using System.Windows.Navigation;
using System.Windows.Shapes;
using static ClimateObservations.Repositories.ObserverRepository;

namespace ClimateObservations
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public readonly ObservableCollection<Observer> ObserverList = new ObservableCollection<Observer>();

        private ObservationWindow observationWindow;

        public MainWindow()
        {
            InitializeComponent();
            AllObserversListBox.ItemsSource = ObserverList;
            AllObserversListBox.DisplayMemberPath = nameof(Observer.FullName);

            var observers = GetObservers();
            foreach (var o in observers)
            {
                ObserverList.Add(o);
            }
        }

        private void BtnAddObserver_Click(object sender, RoutedEventArgs e)
        {
            var observer = new Observer
            {
                Firstname = TxtBoxFirstName.Text,
                Lastname = TxtBoxLastName.Text
            };

            AddObserver(observer);

            ObserverList.Add(observer);
        }

        private void BtnDeleteObserver_Click(object sender, RoutedEventArgs e)
        {
            var selectedObserver = AllObserversListBox.SelectedItem as Observer;
            if (selectedObserver == null)
            {
                return;
            }

            if (!TryDeleteObserver(selectedObserver.Id, out string errorCode))
            {
                if (errorCode == "23503")
                {
                    MessageBox.Show($"Observatör {selectedObserver.FullName} har genomfört en eller flera observationer. Den kan därför inte raderas.");
                }
                else
                {
                    MessageBox.Show("Något gick fel.");
                }

                return;
            }

            ObserverList.Remove(selectedObserver);
        }

        private void BtnLogIn_Click(object sender, RoutedEventArgs e)
        {
            var selectedObserver = AllObserversListBox.SelectedItem as Observer;
            if (selectedObserver == null)
            {
                MessageBox.Show("Välj en observatör först!");
                return;
            }

            if (observationWindow == null || observationWindow.IsLoaded == false)
            {
                observationWindow = new ObservationWindow(selectedObserver);
                observationWindow.Owner = this;
                observationWindow.Show();
            }

            observationWindow.Activate();
        }
    }
}
