namespace ex02
{
    internal class Program
    {
        static async Task Main(string[] args)
        {
            Requester requester = new Requester("http://localhost:5000/api/image");
            ApiResponse apiResponse = await requester.GetImageURL();

            if (apiResponse.Status == Constants.API_RESPONSE_SUCCESS)
                Console.WriteLine($"{apiResponse.Status} => {apiResponse.Url}");
            else
                Console.WriteLine($"{apiResponse.Status}");
        }
    }
}