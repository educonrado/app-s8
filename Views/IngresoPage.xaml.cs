using System.Diagnostics;

namespace app_s8.Views;

public partial class IngresoPage : ContentPage
{
    private double total;

    public IngresoPage()
	{
		InitializeComponent();
	}

    public IngresoPage(double total)
    {
        this.total = total;
        InitializeComponent();
        Debug.WriteLine("--> Ingreso: " + total);
    }
}