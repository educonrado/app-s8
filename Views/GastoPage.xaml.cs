using System.Diagnostics;

namespace app_s8.Views;

public partial class GastoPage : ContentPage
{
    private double total;

    public GastoPage()
	{
		InitializeComponent();
	}

    public GastoPage(double total)
    {
        this.total = total;
        InitializeComponent();
        Debug.WriteLine("--> Gasto: " + total);
    }
}