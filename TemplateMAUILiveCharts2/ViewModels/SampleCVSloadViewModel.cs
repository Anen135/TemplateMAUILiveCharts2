using LiveChartsCore;
using LiveChartsCore.SkiaSharpView;
using LiveChartsCore.SkiaSharpView.Painting;
using SkiaSharp;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using System.Windows.Input;
using Microsoft.Maui.Storage;

namespace TemplateMAUILiveCharts2.ViewModels {

    public class ProductStat {
        public string Date { get; set; }
        public string Product { get; set; }
        public int Quantity { get; set; }
        public double UnitPrice { get; set; }
        public double Total => Quantity * UnitPrice;
        public string Rating { get; set; }
        public string Comment { get; set; }
    }

    public class SampleCVSloadViewModel : INotifyPropertyChanged {
        private bool _isCartesianVisible = true;
        public bool IsCartesianVisible {
            get => _isCartesianVisible;
            set { _isCartesianVisible = value; OnPropertyChanged(); }
        }

        private bool _isPieVisible = false;
        public bool IsPieVisible {
            get => _isPieVisible;
            set { _isPieVisible = value; OnPropertyChanged(); }
        }

        public event PropertyChangedEventHandler PropertyChanged;
        private void OnPropertyChanged([CallerMemberName] string prop = null)
            => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(prop));

        private ObservableCollection<ISeries> _seriesCollection;
        public ObservableCollection<ISeries> SeriesCollection {
            get => _seriesCollection;
            set { _seriesCollection = value; OnPropertyChanged(); }
        }

        private Axis[] _xAxes = new Axis[] { new Axis { Name = "Продукт" } };
        public Axis[] XAxes {
            get => _xAxes;
            set { _xAxes = value; OnPropertyChanged(); }
        }

        private Axis[] _yAxes = new Axis[] { new Axis { Name = "Значение" } };
        public Axis[] YAxes {
            get => _yAxes;
            set { _yAxes = value; OnPropertyChanged(); }
        }

        public SolidColorPaint LegendTextPaint { get; set; } = new(SKColors.Black);

        public ICommand LoadCsvCommand { get; }
        public ICommand ShowQuantityCommand { get; }
        public ICommand ShowTotalCommand { get; }
        public ICommand ShowAveragePriceCommand { get; }
        public ICommand ShowTimeSalesCommand { get; }
        public ICommand ShowProductShareCommand { get; }

        private List<ProductStat> _loadedData = new();

        public SampleCVSloadViewModel() {
            LoadCsvCommand = new Command(async () => await LoadCsvAsync());
            ShowQuantityCommand = new Command(() => UpdateSeries("quantity"));
            ShowTotalCommand = new Command(() => UpdateSeries("total"));
            ShowAveragePriceCommand = new Command(() => UpdateSeries("averagePrice"));
            ShowTimeSalesCommand = new Command(() => UpdateSeries("timeSales"));
            ShowProductShareCommand = new Command(() => UpdateSeries("productShare"));
            SeriesCollection = new ObservableCollection<ISeries>();
        }

        private async Task LoadCsvAsync() {
            try {
                var result = await FilePicker.PickAsync(new PickOptions { PickerTitle = "Выберите CSV файл" });
                if (result == null) return;

                using var stream = await result.OpenReadAsync();
                using var reader = new StreamReader(stream);

                var productStats = new List<ProductStat>();
                var header = await reader.ReadLineAsync();

                while (!reader.EndOfStream) {
                    var line = await reader.ReadLineAsync();
                    var values = line.Split(',');

                    if (values.Length < 7) continue;

                    productStats.Add(new ProductStat {
                        Date = values[0],
                        Product = values[1],
                        Quantity = int.TryParse(values[2], out var qty) ? qty : 0,
                        UnitPrice = double.TryParse(values[3], NumberStyles.Any, CultureInfo.InvariantCulture, out var price) ? price : 0,
                        Rating = values[5],
                        Comment = values[6],
                    });
                }

                _loadedData = productStats;

                UpdateSeries("quantity");
            } catch (Exception ex) {
                Console.WriteLine($"Ошибка при загрузке CSV: {ex.Message}");
            }
        }

        private SKColor GetRandomColor() {
            var random = new Random();
            return new SKColor(
                (byte)random.Next(256),
                (byte)random.Next(256),
                (byte)random.Next(256)
            );
        }

        private void EnsureYAxes() {
            if (YAxes == null || YAxes.Length == 0) {
                YAxes = new Axis[] { new Axis() };
            }
        }

        private void UpdateSeries(string mode) {
            if (_loadedData == null || !_loadedData.Any()) return;

            if (mode == "timeSales") {
                IsCartesianVisible = true;
                IsPieVisible = false;
                var dateGroups = _loadedData
                    .GroupBy(p => p.Date)
                    .OrderBy(g => DateTime.Parse(g.Key))
                    .Select(g => new {
                        Date = g.Key,
                        TotalSum = g.Sum(p => p.Total)
                    })
                    .ToList();

                XAxes = new Axis[] {
                    new Axis {
                        Name = "Дата",
                        Labels = dateGroups.Select(g => g.Date).ToArray(),
                        LabelsRotation = 15
                    }
                };

                EnsureYAxes();
                YAxes[0].Name = "Сумма продаж (₽)";

                SeriesCollection = new ObservableCollection<ISeries> {
                    new LineSeries<double> {
                        Values = dateGroups.Select(g => g.TotalSum).ToArray(),
                        Name = "Продажи по времени",
                        Stroke = new SolidColorPaint(SKColors.Orange),
                        Fill = null
                    }
                };
            } else if (mode == "productShare") {
                IsCartesianVisible = false;
                IsPieVisible = true;
                var productGroups = _loadedData
                    .GroupBy(p => p.Product)
                    .Select(g => new {
                        Product = g.Key,
                        QuantitySum = g.Sum(p => p.Quantity)
                    })
                    .OrderByDescending(g => g.QuantitySum)
                    .ToList();

                XAxes = Array.Empty<Axis>();
                YAxes = Array.Empty<Axis>();

                SeriesCollection = new ObservableCollection<ISeries>(
                    productGroups.Select(g => new PieSeries<int> {
                        Values = new[] { g.QuantitySum },
                        Name = g.Product,
                        Stroke = new SolidColorPaint(SKColors.White, 2),
                        Fill = new SolidColorPaint(GetRandomColor())
                    })
                );
            } else {
                IsCartesianVisible = true;
                IsPieVisible = false;
                var productGroups = _loadedData
                    .GroupBy(p => p.Product)
                    .Select(g => new {
                        Product = g.Key,
                        QuantitySum = g.Sum(p => p.Quantity),
                        TotalSum = g.Sum(p => p.Total),
                        AveragePrice = g.Average(p => p.UnitPrice)
                    })
                    .ToList();

                XAxes = new Axis[] {
                    new Axis {
                        Name = "Продукт",
                        Labels = productGroups.Select(g => g.Product).ToArray(),
                        ForceStepToMin = true,
                        MinStep = 1,
                        LabelsRotation = 15
                    }
                };

                EnsureYAxes();

                ISeries series;
                switch (mode) {
                    case "total":
                    series = new ColumnSeries<double> {
                        Values = productGroups.Select(g => g.TotalSum).ToArray(),
                        Name = "Общая сумма",
                        Stroke = new SolidColorPaint(SKColors.Green),
                        Fill = new SolidColorPaint(SKColors.LightGreen)
                    };
                    YAxes[0].Name = "Сумма (₽)";
                    break;

                    case "averagePrice":
                    series = new ColumnSeries<double> {
                        Values = productGroups.Select(g => g.AveragePrice).ToArray(),
                        Name = "Средняя цена",
                        Stroke = new SolidColorPaint(SKColors.Purple),
                        Fill = new SolidColorPaint(SKColors.Plum)
                    };
                    YAxes[0].Name = "Цена за единицу";
                    break;

                    case "quantity":
                    default:
                    series = new ColumnSeries<int> {
                        Values = productGroups.Select(g => g.QuantitySum).ToArray(),
                        Name = "Суммарное количество",
                        Stroke = new SolidColorPaint(SKColors.Blue),
                        Fill = new SolidColorPaint(SKColors.LightBlue)
                    };
                    YAxes[0].Name = "Количество";
                    break;
                }

                SeriesCollection = new ObservableCollection<ISeries> { series };
            }
        }
    }
}
