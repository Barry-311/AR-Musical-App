using System;

namespace GetWeather.Weather
{
    [Serializable]
    public class CurrentWeather
    {
        public double temperature;    // ���£���λ ��C
        public double windspeed;      // ���٣���λ m/s
        public int weathercode;    // ����״����
        public string time;           // ʱ���
    }

    [Serializable]
    public class WeatherResponse
    {
        public CurrentWeather current_weather;
    }
}
