namespace AumauiCL.Services.Api
{
    public static class ApiConfig
    {
        // Localhost for Windows/iOS/Mac
        private const string LocalUrl = "https://localhost:7224/api";

        // "Magic" localhost for Android Emulator
        private const string AndroidUrl = "https://10.0.2.2:7224/api";

        public static string BaseUrl
        {
            get
            {
#if ANDROID
                return AndroidUrl;
#else
                return LocalUrl;
#endif
            }
        }
    }
}
