using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ParamCube : MonoBehaviour
{
    public int _band; // Frequency band to visualize, should be set in the inspector
    public float _startScale, _scaleMultiplier; // Start scale and multiplier for the cube's height
    public bool _useBuffer; // Whether to use the band buffer for smoother transitions

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        float NoBufferYScale = (AudioPeer._freqBand[_band] * _scaleMultiplier) + _startScale;
        float BufferYScale = (AudioPeer._bandBuffer[_band] * _scaleMultiplier) + _startScale;
        if (_useBuffer)
        {
            transform.localScale = new Vector3(transform.localScale.x, BufferYScale, transform.localScale.z);

            // Move the cube up so that its bottom is at the ground level
            transform.localPosition = new Vector3(transform.localPosition.x, BufferYScale / 2f, transform.localPosition.z);
        }

        if (!_useBuffer)
        {
            transform.localScale = new Vector3(transform.localScale.x, NoBufferYScale, transform.localScale.z);

            transform.localPosition = new Vector3(transform.localPosition.x, NoBufferYScale / 2f, transform.localPosition.z);
        }
    }
}
