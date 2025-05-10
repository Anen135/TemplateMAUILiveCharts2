using LiveChartsCore;
using LiveChartsCore.SkiaSharpView.Painting;

namespace TemplateMAUILiveCharts2.Controls {
    public partial class ChartView : ContentView {
        public static readonly BindableProperty SeriesProperty =
            BindableProperty.Create(nameof(Series), typeof(IEnumerable<ISeries>), typeof(ChartView), default(IEnumerable<ISeries>));

        public static readonly BindableProperty LegendTextPaintProperty =
            BindableProperty.Create(nameof(LegendTextPaint), typeof(SolidColorPaint), typeof(ChartView));

        public IEnumerable<ISeries> Series {
            get => (IEnumerable<ISeries>)GetValue(SeriesProperty);
            set => SetValue(SeriesProperty, value);
        }

        public SolidColorPaint LegendTextPaint {
            get => (SolidColorPaint)GetValue(LegendTextPaintProperty);
            set => SetValue(LegendTextPaintProperty, value);
        }

        public ChartView() {
            InitializeComponent(); // Ensure that the corresponding XAML file exists and is properly linked  
        }
    }
}
