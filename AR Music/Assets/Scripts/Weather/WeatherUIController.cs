using UnityEngine;
using TMPro;
using System;

namespace GetWeather.Weather
{
    public class WeatherUIController : MonoBehaviour
    {
        [Header("���� WeatherService")]
        public WeatherService weatherService;

        [Header("UI �ı����� (TextMeshProUGUI)")]
        public TMP_Text weatherText;
        public TMP_Text localTimeText;

        void Start()
        {
            // �Զ����� WeatherService��Ҳ�����ֶ���ק��
            if (weatherService == null)
                weatherService = FindObjectOfType<WeatherService>();

            if (weatherService != null)
            {
                weatherService.OnWeatherReceived += OnWeatherReceived;
                weatherService.RequestCurrentWeather();
            }
            else
            {
                Debug.LogError("δ�ҵ� WeatherService����ȷ�����ڳ�������Ӳ����ظýű���");
            }
        }

        private void OnWeatherReceived(WeatherResponse resp)
        {
            // 1. ת�������룬����ʾ�¶ȡ�����
            string desc = WeatherCodeHelper.GetWeatherDescription(resp.current_weather.weathercode);
            weatherText.text = $"Weather: {desc}\n" +
                               $"Temp: {resp.current_weather.temperature}��C\n" +
                               $"Wind: {resp.current_weather.windspeed} m/s";

            // 2. ��ʾ�����豸��ǰʱ��
            localTimeText.text = $"Local Time: {DateTime.Now:yyyy-MM-dd HH:mm:ss}";
        }

        void Update()
        {
            // ����ˢ�±���ʱ�䣨��ѡ��
            localTimeText.text = $"Local Time: {DateTime.Now:yyyy-MM-dd HH:mm:ss}";
        }
    }
}
