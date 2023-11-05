using LiveChartsCore;
using LiveChartsCore.SkiaSharpView;
using PidUI.Model;
using PidUI.MVVM;
using System.Collections.ObjectModel;

namespace PidUI.ViewModel
{
    internal class MainWindowViewModel : ViewModelBase
    {

        public ObservableCollection<ISeries> SeriesCollection { get; set; }
        public ObservableCollection<PidController> PidControllers { get; set; }

        public RelayCommand RunCommand => new(execute => RunSimulation());

        public MainWindowViewModel()
        {
            SeriesCollection = new ObservableCollection<ISeries>
            {
                new LineSeries<int> { Values = new[] { 1, 2, 3 } }
            };

            PidControllers = new ObservableCollection<PidController>();
            PidControllers.Add(new PidController(0.25f, true, true)
            {
                KP = 0.2d,
                KI = 0.5d,
                KD = 1d
            });
        }

        private PidController selectedPid;

        public PidController SelectedPid
        {
            get { return selectedPid; }
            set 
            {
                selectedPid = value;
                OnPropertyChanged();
            }
        }

        private void RunSimulation()
        {
            SeriesCollection.Add(new LineSeries<int> { Values = new[] { 2, 4, 6 } });
            // change to use PID controller values and output of the system
        }
    }
}
