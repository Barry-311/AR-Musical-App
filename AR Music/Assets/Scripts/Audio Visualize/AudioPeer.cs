using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class AudioPeer : MonoBehaviour
{
    AudioSource _audioSource;
    float[] _samples = new float[512]; // 512 is the default size for GetSpectrumData
    float[] _freqBand = new float[8]; // 8 frequency bands for audio visualization
    float[] _bandBuffer = new float[8]; // Buffer for smoothing the frequency bands
    float[] _bufferDecrease = new float[8]; // Buffer decrease for smoothing effect

    float[] _freqBandHighest = new float[8]; // Highest values for each frequency band, if needed
    public static float[] _audioBand = new float[8]; // Audio bands for visualization, can be used for different effects
    public static float[] _audioBandBuffer = new float[8]; // Audio band buffer for smoothing

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
        BandBuffer();
        CreateAudioBands();
    }

    void CreateAudioBands()
    {
        for (int i = 0; i < 8; i++)
        {
            if (_freqBand[i] > _freqBandHighest[i])
            {
                _freqBandHighest[i] = _freqBand[i]; // Update the highest value for this band
            }
            _audioBand[i] = _freqBand[i] / _freqBandHighest[i]; // Normalize the frequency band
            _audioBandBuffer[i] = _bandBuffer[i] / _freqBandHighest[i]; // Normalize the band buffer
        }
    }

    void GetSpectrumAudioSource()
    {
        _audioSource.GetSpectrumData(_samples, 0, FFTWindow.Blackman);
    }

    void BandBuffer()
    {
        for (int g = 0; g < 8; ++g)
        {
            if (_freqBand[g] > _bandBuffer[g])
            {
                _bandBuffer[g] = _freqBand[g];
                _bufferDecrease[g] = 0.005f; // Reset buffer decrease when a new peak is found
            }

            if (_freqBand[g] < _bandBuffer[g]) // If the current frequency band is less than the buffer
            {
                _bandBuffer[g] -= _bufferDecrease[g]; // Decrease the buffer value
                _bufferDecrease[g] *= 1.2f; // Increase the decrease rate for smoother transitions
            }
            if (_bandBuffer[g] < 0)
            {
                _bandBuffer[g] = 0; // Prevent negative values
            }
        }
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
