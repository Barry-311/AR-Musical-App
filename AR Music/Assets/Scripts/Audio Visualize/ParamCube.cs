using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ParamCube : MonoBehaviour
{
    [Header("Index")]
    public int _band = 0;

    [Header("From")]
    public float _startScale = 1f;

    [Header("To")]
    public float _scaleMultiplier = 10f;

    [Header("Use Buffer")]
    public bool _useBuffer = true;

    private Material _material;

    void Start()
    {
        _material = GetComponent<Renderer>().materials[0];

        if (float.IsNaN(_startScale) || float.IsInfinity(_startScale))
            _startScale = 1f;
        if (float.IsNaN(_scaleMultiplier) || float.IsInfinity(_scaleMultiplier))
            _scaleMultiplier = 1f;
    }

    void Update()
    {
        // Check if AudioPeer._audioBand and _audioBandBuffer are initialized
        if (AudioPeer._audioBand == null || AudioPeer._audioBandBuffer == null)
            return;

        // Check if _band is within the valid range
        if (_band < 0 || _band >= AudioPeer._audioBand.Length)
        {
            Debug.LogWarning($"ParamCube: _band out of (band={_band})£¬Range£º0 ~ {AudioPeer._audioBand.Length - 1}");
            return;
        }

        // Get the raw and buffer values for the specified band
        float rawValue = AudioPeer._audioBand[_band];
        float bufferValue = AudioPeer._audioBandBuffer[_band];

        // Calculate the Y scale values based on the raw and buffer values
        float noBufferY = (rawValue * _scaleMultiplier) + _startScale;
        float bufferY = (bufferValue * _scaleMultiplier) + _startScale;

        // If the values are NaN or Infinity, reset them to _startScale
        if (float.IsNaN(noBufferY) || float.IsInfinity(noBufferY)) noBufferY = _startScale;
        if (float.IsNaN(bufferY) || float.IsInfinity(bufferY)) bufferY = _startScale;

        if (_useBuffer)
        {
            // 1. Set localScale as (x, bufferY, z)
            transform.localScale = new Vector3(transform.localScale.x, bufferY, transform.localScale.z);

            // 2. Use bufferValue to color the material, first check if bufferValue is not NaN/Infinity
            if (!float.IsNaN(bufferValue) && !float.IsInfinity(bufferValue))
            {
                Color color = new Color(bufferValue, bufferValue, bufferValue);
                _material.SetColor ("_EmissionColor", color);
            }

            // 3. Adjust localPosition so the bottom of the cube is on the ground (posY = bufferY/2)
            float posY = bufferY / 2f;
            if (float.IsNaN(posY) || float.IsInfinity(posY))
                posY = _startScale / 2f;

            transform.localPosition = new Vector3(transform.localPosition.x, posY, transform.localPosition.z);
        }

        if (!_useBuffer)
        {
            transform.localScale = new Vector3(transform.localScale.x, noBufferY, transform.localScale.z);

            if (!float.IsNaN(rawValue) && !float.IsInfinity(rawValue))
            {
                Color color = new Color(rawValue, rawValue, rawValue);
                _material.SetColor("_EmissionColor", color);
            }

            float posY = noBufferY / 2f;
            if (float.IsNaN(posY) || float.IsInfinity(posY))
                posY = _startScale / 2f;

            transform.localPosition = new Vector3( transform.localPosition.x, posY, transform.localPosition.z);
        }
    }
}
