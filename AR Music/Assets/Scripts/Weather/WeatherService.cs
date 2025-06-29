using UnityEngine;
using UnityEngine.Networking;
using System;
using System.Collections;

namespace GetWeather.Weather
{
    public class WeatherService : MonoBehaviour
    {
        [Header("�������꣨���� Inspector ���޸ģ�")]
        public double latitude = 52.52;   // Ĭ�ϣ�����
        public double longitude = 13.41;

        /// <summary>
        /// ��ȡ��������ص�
        /// </summary>
        public Action<WeatherResponse> OnWeatherReceived;

        /// <summary>
        /// �ⲿ���ã���ʼ��ȡ��ǰ����
        /// </summary>
        public void RequestCurrentWeather()
        {
            StartCoroutine(FetchWeather());
        }

        private IEnumerator FetchWeather()
        {
            string url = $"https://api.open-meteo.com/v1/forecast?" +
                         $"latitude={latitude}&longitude={longitude}&current_weather=true";

            using (var www = UnityWebRequest.Get(url))
            {
                yield return www.SendWebRequest();

                if (www.result != UnityWebRequest.Result.Success)
                {
                    Debug.LogError($"[WeatherService] ����ʧ�ܣ�{www.error}");
                    yield break;
                }

                // �����л� JSON �� WeatherResponse
                WeatherResponse resp = JsonUtility.FromJson<WeatherResponse>(www.downloadHandler.text);
                OnWeatherReceived?.Invoke(resp);
            }
        }
    }
}
