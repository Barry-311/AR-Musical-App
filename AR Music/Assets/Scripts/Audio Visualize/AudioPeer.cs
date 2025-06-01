using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class AudioPeer : MonoBehaviour
{
    AudioSource _audioSource;
    public static float[] _samples = new float[512]; // 512 is the default size for GetSpectrumData
    public static float[] _freqBand = new float[8]; // 8 frequency bands for audio visualization

    // Start is called before the first frame update
    void Start()
    {
        _audioSource = GetComponent<AudioSource>();
    }

    // Update is called once per frame
    void Update()
    {
        GetSpectrumAudioSource();
        MakeFrequencyBands();
    }

    void GetSpectrumAudioSource()
    {
        _audioSource.GetSpectrumData(_samples, 0, FFTWindow.Blackman);
    }

    void MakeFrequencyBands()
    {
        int count = 0;
        for (int i = 0; i < 8; i++)
        {
            float average = 0f;
            int sampleCount = (int)Mathf.Pow(2, i) * 2; // 2, 4, 8, 16, 32, 64, 128, 256

            if (i == 7)
            {
                sampleCount += 2; // Last band includes the remaining samples
            }

            for (int j = 0; j < sampleCount; j++)
            {
                average += _samples[count] * (count + 1);
                count++;
            }

            average /= count; // Average the samples for this frequency band

            _freqBand[i] = average * 10; // Scale the average for visualization
        }
    }
}
