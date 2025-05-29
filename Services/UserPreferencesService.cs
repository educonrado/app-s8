using System.Text.Json;
using app_s8.GoogleAuth;
using Microsoft.Maui.Storage;

namespace app_s8.Services
{
    public static class UserPreferencesService
    {
        private const string UserKey = "google_user";

        public static void SaveUser(GoogleUserDTO user)
        {
            var json = JsonSerializer.Serialize(user);
            Preferences.Set(UserKey, json);
        }

        public static GoogleUserDTO GetUser()
        {
            var json = Preferences.Get(UserKey, null);
            if (string.IsNullOrEmpty(json)) return null;

            return JsonSerializer.Deserialize<GoogleUserDTO>(json);
        }

        public static void ClearUser()
        {
            Preferences.Remove(UserKey);
        }
    }
}
