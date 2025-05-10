using System;
using System.Collections.ObjectModel;
using System.Windows.Input;
using CommunityToolkit.Mvvm.Input;
using LiveChartsCore;
using LiveChartsCore.SkiaSharpView;
using LiveChartsCore.SkiaSharpView.Painting;
using SkiaSharp;

namespace TemplateMAUILiveCharts2.ViewModels {
    public class LineSeriesViewModel {
        private readonly Random _random = new();

        public ObservableCollection<ISeries> Series { get; set; }
        public ICommand UpdateDataCommand { get; }

        public Axis[] XAxes { get; set; } =
        {
            new Axis { Name = "X Axis", Labels = new[] { "A", "B", "C", "D", "E" } }
        };
        public Axis[] YAxes { get; set; } =
        {
            new Axis { Name = "Y Axis", Labeler = value => value.ToString() }
        };
        public SolidColorPaint LegendTextPaint { get; set; } = new(SKColors.White);

        public LineSeriesViewModel() {
            Series = new ObservableCollection<ISeries>
            {
                new LineSeries<double>
                {
                    Name = "Line 1",
                    Values = GenerateRandomValues(),
                    Stroke = new SolidColorPaint(SKColors.Blue),
                },
                new LineSeries<double>
                {
                    Name = "Line 2",
                    Values = GenerateRandomValues(),
                    Stroke = new SolidColorPaint(SKColors.Red),
                }
            };

            UpdateDataCommand = new RelayCommand(UpdateData);
        }

        private void UpdateData() {
            foreach (var s in Series)
                if (s is LineSeries<double> line)
                    line.Values = GenerateRandomValues();
        }

        private double[] GenerateRandomValues()
            => new double[] { _random.Next(0, 10), _random.Next(0, 10), _random.Next(0, 10), _random.Next(0, 10), _random.Next(0, 10) };
    }
}
