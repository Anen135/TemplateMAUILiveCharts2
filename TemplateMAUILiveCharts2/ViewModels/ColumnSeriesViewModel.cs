using System;
using System.Collections.ObjectModel;
using System.Windows.Input;
using CommunityToolkit.Mvvm.Input;
using LiveChartsCore;
using LiveChartsCore.SkiaSharpView;
using LiveChartsCore.SkiaSharpView.Painting;
using SkiaSharp;

namespace TemplateMAUILiveCharts2.ViewModels {
    public class ColumnSeriesViewModel {
        private readonly Random _random = new();

        public ObservableCollection<ISeries> Series { get; set; }
        public ICommand UpdateDataCommand { get; }

        public Axis[] XAxes { get; set; } =
        {
            new Axis { Name = "Categories", Labels = new[] { "A", "B", "C", "D", "E" } }
        };
        public Axis[] YAxes { get; set; } =
        {
            new Axis { Name = "Values" }
        };
        public SolidColorPaint LegendTextPaint { get; set; } = new(SKColors.White);

        public ColumnSeriesViewModel() {
            Series = new ObservableCollection<ISeries>
            {
                new ColumnSeries<double>
                {
                    Name = "Column 1",
                    Values = GenerateRandomValues(),
                    Stroke = new SolidColorPaint(SKColors.Green),
                },
                new ColumnSeries<double>
                {
                    Name = "Column 2",
                    Values = GenerateRandomValues(),
                    Stroke = new SolidColorPaint(SKColors.Orange),
                }
            };

            UpdateDataCommand = new RelayCommand(UpdateData);
        }

        private void UpdateData() {
            foreach (var s in Series)
                if (s is ColumnSeries<double> column)
                    column.Values = GenerateRandomValues();
        }

        private double[] GenerateRandomValues()
            => new double[] { _random.Next(0, 10), _random.Next(0, 10), _random.Next(0, 10), _random.Next(0, 10), _random.Next(0, 10) };
    }
}
