namespace MinimalApiTests.GettingStarted.MinimalApi;

public class MapPoint
{
    public double Latitude { get; set; }
    public double Longitude { get; set; }

    public static bool TryParse(string? value, out MapPoint? point)
    {
        try
        {
            var splitValues = value?.Split(',').Select(double.Parse).ToArray();
            point = new MapPoint
            {
                Latitude = splitValues![0],
                Longitude = splitValues[1]
            };
            return true;
        }
        catch (Exception)
        {
            point = null;
            return false;
        }
    }
}