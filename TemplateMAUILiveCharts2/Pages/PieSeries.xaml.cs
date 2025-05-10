using TemplateMAUILiveCharts2.ViewModels;
namespace TemplateMAUILiveCharts2.Pages;
public partial class PieSeries : ContentPage
{
	public PieSeries()
	{
		InitializeComponent();
        BindingContext = new PieSeriesViewModel();
    }
}