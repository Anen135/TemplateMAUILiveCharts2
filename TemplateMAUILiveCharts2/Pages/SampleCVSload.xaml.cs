using TemplateMAUILiveCharts2.ViewModels;
namespace TemplateMAUILiveCharts2.Pages;
public partial class SampleCVSload : ContentPage
{
	public SampleCVSload()
	{
		InitializeComponent();
        BindingContext = new SampleCVSloadViewModel();
    }
}