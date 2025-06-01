using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ParamCube : MonoBehaviour
{
    public int _band; // Frequency band to visualize, should be set in the inspector
    public float _startScale, _scaleMultiplier; // Start scale and multiplier for the cube's height

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        float newYScale = (AudioPeer._freqBand[_band] * _scaleMultiplier) + _startScale;
        transform.localScale = new Vector3(transform.localScale.x, newYScale, transform.localScale.z);

        // 让 cube 向上移动一半高度，保持底部固定
        transform.localPosition = new Vector3(transform.localPosition.x, newYScale / 2f, transform.localPosition.z);
    }

}
