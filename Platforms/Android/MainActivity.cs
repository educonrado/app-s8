using Android.App;
using Android.Content;
using Android.Content.PM;
using Android.Gms.Auth.Api.SignIn;
using Android.OS;
using Android.Runtime;



namespace app_s8
{
    [Activity(Label = "CushquiApp", Theme = "@style/Maui.SplashTheme", MainLauncher = true, LaunchMode = LaunchMode.SingleTop, ConfigurationChanges = ConfigChanges.ScreenSize | ConfigChanges.Orientation | ConfigChanges.UiMode | ConfigChanges.ScreenLayout | ConfigChanges.SmallestScreenSize | ConfigChanges.Density)]
    public class MainActivity : MauiAppCompatActivity
    {
        public static event EventHandler<(bool Success, GoogleSignInAccount account)> ResultGoogleAuth;

        protected override async void OnActivityResult(int requestCode, Result resultCode, Intent data)
        {
            if (requestCode == 9001)
            {
                try
                {
                    var currentAccount = await GoogleSignIn.GetSignedInAccountFromIntentAsync(data);

                    ResultGoogleAuth.Invoke(this, (currentAccount.Email != null, currentAccount));
                }
                catch (Exception ex)
                {
                    ResultGoogleAuth.Invoke(this, (false, null));
                }


            }
        }

    }
}

