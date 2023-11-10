using LiveChartsCore;
using LiveChartsCore.Drawing;
using LiveChartsCore.SkiaSharpView;
using LiveChartsCore.SkiaSharpView.Painting;
using LiveChartsCore.SkiaSharpView.Painting.Effects;
using NumSharp;
using PidUI.Model;
using PidUI.MVVM;
using SkiaSharp;
using System.Collections.ObjectModel;

namespace PidUI.ViewModel
{
    internal class MainWindowViewModel : ViewModelBase
    {

        public ObservableCollection<ISeries> SeriesCollection { get; set; }
        public ObservableCollection<PidController> PidControllers { get; set; }

        public RelayCommand RunCommand => new(execute => RunSimulation());

        public RelayCommand AddCommand => new RelayCommand(execute => AddPid());
        public RelayCommand DeleteCommand => new RelayCommand(execute => DeletePid(), canExecute => selectedPid != null);

        public MainWindowViewModel()
        {
            SeriesCollection = new ObservableCollection<ISeries>();
            PidControllers = new ObservableCollection<PidController>();

            PidControllers.Add(new PidController(0.05d, true, true));
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
            SeriesCollection.Clear();
            //TODO: refactor this into separate methods

            NDArray timeVec = np.linspace(0d, 10d, 501);

            PlainSystem plainSys = new PlainSystem(new Plant());
            double[] plainResults = plainSys.RunSimulation(timeVec);

            SeriesCollection.Add(new LineSeries<double> 
            {
                Values = plainResults,
                Name = "Default",
                Fill = null,
                GeometryFill = null,
                GeometryStroke = null
            });

            foreach (PidController pid in PidControllers)
            {
                //Trace.WriteLine($"{pid.KP}, {pid.KI}, {pid.KI}");
                PidSystem pidSystem = new PidSystem(new Plant(), new PidController(0.05f, true, true)
                {
                    KP = pid.KP,
                    KI = pid.KI,
                    KD = pid.KD
                });

                double[] results = pidSystem.RunSimulation(timeVec);
                SeriesCollection.Add(new LineSeries<double>
                {
                    Values = results,
                    Name = $"{pidSystem.pid.KP}, {pidSystem.pid.KI}, {pidSystem.pid.KD}",
                    Fill = null,
                    GeometryFill = null,
                    GeometryStroke = null
                });
            }
        }

        private void AddPid()
        {
            PidControllers.Add(new PidController(0.05d, true, true)
            {
                KP = 0,
                KI = 0,
                KD = 0
            });
        }

        private void DeletePid()
        {
            PidControllers.Remove(selectedPid);
        }

        private static readonly SKColor s_gray = new(195, 195, 195);
        private static readonly SKColor s_gray1 = new(160, 160, 160);
        private static readonly SKColor s_gray2 = new(90, 90, 90);
        private static readonly SKColor s_dark3 = new(60, 60, 60);

        public Axis[] XAxes { get; set; } =
        {
            new Axis
            {
                Name = "X axis",
                NamePaint = new SolidColorPaint(s_gray1),
                TextSize = 18,
                Padding = new Padding(5, 15, 5, 5),
                LabelsPaint = new SolidColorPaint(s_gray),
                SeparatorsPaint = new SolidColorPaint
                {
                    Color = s_gray,
                    StrokeThickness = 1,
                    PathEffect = new DashEffect(new float[] { 3, 3 })
                },
                SubseparatorsPaint = new SolidColorPaint
                {
                    Color = s_gray2,
                    StrokeThickness = 0.5f
                },
                SubseparatorsCount = 9,
                ZeroPaint = new SolidColorPaint
                {
                    Color = s_gray1,
                    StrokeThickness = 2
                },
                TicksPaint = new SolidColorPaint
                {
                    Color = s_gray,
                    StrokeThickness = 1.5f
                },
                SubticksPaint = new SolidColorPaint
                {
                    Color = s_gray,
                    StrokeThickness = 1
                }
            }
        };
    }
}
