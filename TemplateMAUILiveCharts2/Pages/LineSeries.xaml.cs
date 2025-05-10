using TemplateMAUILiveCharts2.ViewModels;


namespace TemplateMAUILiveCharts2.Pages {
	public partial class LineSeries : ContentPage
	{
		public LineSeries()
		{
			InitializeComponent();
			BindingContext = new LineSeriesViewModel();
		}
	}
}