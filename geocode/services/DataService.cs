using System.Text.Json;
using geocode.models;

namespace geocode.services
{
    public class DataService
    {
        public async Task<SincelejoResponse> LoadDataAsync()
        {
            using var stream = await FileSystem.OpenAppPackageFileAsync("output.json");
            using var reader = new StreamReader(stream);
            var json = await reader.ReadToEndAsync();

            return JsonSerializer.Deserialize<SincelejoResponse>(json);
        }
    }
}