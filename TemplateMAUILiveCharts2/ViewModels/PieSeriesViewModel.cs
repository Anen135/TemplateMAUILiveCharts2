using System;
using System.Collections.ObjectModel;
using System.Windows.Input;
using CommunityToolkit.Mvvm.Input;
using LiveChartsCore;
using LiveChartsCore.SkiaSharpView;
using LiveChartsCore.SkiaSharpView.Painting;
using SkiaSharp;

namespace TemplateMAUILiveCharts2.ViewModels {
    public class PieSeriesViewModel {
        private readonly Random _random = new();

        public ObservableCollection<ISeries> Series { get; set; }
        public ICommand UpdateDataCommand { get; }

        public SolidColorPaint LegendTextPaint { get; set; } = new(SKColors.White);

        public PieSeriesViewModel() {
            Series = new ObservableCollection<ISeries>
            {
                new PieSeries<double>
                {
                    Name = "Slice 1",
                    Values = new double[] { _random.Next(10, 50) },
                    Stroke = new SolidColorPaint(SKColors.Blue),
                },
                new PieSeries<double>
                {
                    Name = "Slice 2",
                    Values = new double[] { _random.Next(10, 50) },
                    Stroke = new SolidColorPaint(SKColors.Red),
                },
                new PieSeries<double>
                {
                    Name = "Slice 3",
                    Values = new double[] { _random.Next(10, 50) },
                    Stroke = new SolidColorPaint(SKColors.Green),
                }
            };

            UpdateDataCommand = new RelayCommand(UpdateData);
        }

        private void UpdateData() {
            foreach (var s in Series)
                if (s is PieSeries<double> pie)
                    pie.Values = new double[] { _random.Next(10, 50) };
        }
    }
}
