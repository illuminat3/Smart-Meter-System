namespace SmartMeterSystemBackend
{
    public class WeatherForecast
    {
        private const double CelsiusToFahrenheitRatio = 5.0 / 9.0;
        public DateOnly Date { get; set; }

        public int TemperatureC { get; set; }

        public int TemperatureF => 32 + (int)(TemperatureC * 9.0 / 5.0);

        public string? Summary { get; set; }
    }
}
