namespace ex02
{
    public static class Constants
    {
        public const string API_RESPONSE_SUCCESS = "SUCCESS";
        public const string API_RESPONSE_RETRY_LATER = "RETRY-LATER";  // adăugat
        public const int DESIRED_IMAGE_COUNT = 5;         // câte imagini vrem să colectăm
        public const int MAX_RETRY_PER_REQUEST = 5;       // retry-uri maxime per cerere
        public const int INITIAL_DELAY_MS = 500;          // delay inițial pentru backoff
    }
}