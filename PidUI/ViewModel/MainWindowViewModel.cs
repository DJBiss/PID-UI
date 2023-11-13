using LiveChartsCore;
using LiveChartsCore.Defaults;
using LiveChartsCore.Drawing;
using LiveChartsCore.SkiaSharpView;
using LiveChartsCore.SkiaSharpView.Painting;
using NumSharp;
using PidUI.Model;
using PidUI.MVVM;
using SkiaSharp;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace PidUI.ViewModel
{
    internal class MainWindowViewModel : ViewModelBase
    {
        //colours for axes
        private static readonly SKColor black = SKColors.Black;
        private static readonly SKColor s_gray1 = new(160, 160, 160);
        private static readonly SKColor s_gray2 = new(90, 90, 90);

        public ObservableCollection<ISeries> SeriesCollection { get; set; }
        public ObservableCollection<PidController> PidControllers { get; set; }

        public RelayCommand RunCommand => new(execute => RunSimulation());

        public RelayCommand AddCommand => new RelayCommand(execute => AddPid());
        public RelayCommand DeleteCommand => new RelayCommand(execute => DeletePid(), canExecute => selectedPid != null);

        public MainWindowViewModel()
        {
            SeriesCollection = new ObservableCollection<ISeries>();
            PidControllers = new ObservableCollection<PidController>
            {
                new PidController(0.05d, true, true)
            };
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

            double[] timeVec = np.linspace(0d, 10d, 501).ToArray<double>();

            PlainSystem plainSys = new PlainSystem(new Plant());
            double[] plainResults = plainSys.RunSimulation(timeVec);

            SeriesCollection.Add(new LineSeries<ObservablePoint>
            {
                Values = FetchData(timeVec, plainResults),
                Name = "No PID",
                Fill = null,
                GeometryFill = null,
                GeometryStroke = null
            });

            foreach (PidController pid in PidControllers)
            {
                PidSystem pidSystem = new PidSystem(new Plant(), new PidController(0.05f, true, true)
                {
                    KP = pid.KP,
                    KI = pid.KI,
                    KD = pid.KD
                });

                double[] results = pidSystem.RunSimulation(timeVec);
                SeriesCollection.Add(new LineSeries<ObservablePoint>
                {
                    Values = FetchData(timeVec, results),
                    Name = $"[kP: {pidSystem.pid.KP}, kI: {pidSystem.pid.KI}, kD: {pidSystem.pid.KD}]",
                    Fill = null,
                    GeometryFill = null,
                    GeometryStroke = null
                });
            }
        }

        private static List<ObservablePoint> FetchData(double[] xData, double[] yData)
        {
            List<ObservablePoint> list = new();

            for (int i = 0; i < xData.Length; i++)
            {
                list.Add(new ObservablePoint(xData[i], yData[i]));
            }
            return list;
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


        public SolidColorPaint TooltipBackgroundPaint { get; set; } = new SolidColorPaint(new SKColor(40, 40, 40));

        public SolidColorPaint TooltipTextPaint { get; set; } = new SolidColorPaint(new SKColor(205, 205, 205));


        public Axis[] XAxes { get; set; } =
        {
            new Axis
            {
                Name = "Time [s]",
                NamePaint = new SolidColorPaint(black),
                TextSize = 18,
                MinLimit = 0,
                Padding = new Padding(5, 15, 5, 5),
                LabelsPaint = new SolidColorPaint(black),
                SeparatorsPaint = new SolidColorPaint
                {
                    Color = black,
                    StrokeThickness = 1
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
                    Color = black,
                    StrokeThickness = 1.5f
                },
                SubticksPaint = new SolidColorPaint
                {
                    Color = black,
                    StrokeThickness = 1
                }
            }
        };

        public Axis[] YAxes { get; set; } =
        {
            new Axis
            {
                Name = "Response []",
                NamePaint = new SolidColorPaint(black),
                TextSize = 18,
                MinLimit = 0,
                Padding = new Padding(5, 0, 15, 0),
                LabelsPaint = new SolidColorPaint(black),
                SeparatorsPaint = new SolidColorPaint
                {
                    Color = black,
                    StrokeThickness = 1
                    //PathEffect = new DashEffect(new float[] { 3, 3 })
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
                    Color = black,
                    StrokeThickness = 1.5f
                },
                SubticksPaint = new SolidColorPaint
                {
                    Color = black,
                    StrokeThickness = 1
                }
            }
        };

        public DrawMarginFrame Frame { get; set; } =
        new()
        {
            Stroke = new SolidColorPaint
            {
                Color = black,
                StrokeThickness = 2
            }
        };
    }
}
