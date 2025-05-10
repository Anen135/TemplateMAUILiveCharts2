using TemplateMAUILiveCharts2.ViewModels;
namespace TemplateMAUILiveCharts2.Pages;
public partial class ColumnSeries : ContentPage
{
	public ColumnSeries()
	{
		InitializeComponent();
        BindingContext = new ColumnSeriesViewModel();
    }
}