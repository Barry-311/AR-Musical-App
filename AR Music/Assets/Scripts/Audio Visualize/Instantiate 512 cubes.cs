using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Instantiate512cubde : MonoBehaviour
{
    public GameObject _sampleCubePrefab; // Assign your cube prefab in the inspector
    GameObject[] _sampleCube = new GameObject[512]; // Array to hold the instantiated cubes
    public float _maxScale; // Maximum scale for the cubes

    // Start is called before the first frame update
    void Start()
    {
        for (int i = 0; i < 512; i++)
        {
            GameObject _instantiateSampleCube = (GameObject)Instantiate(_sampleCubePrefab);
            _instantiateSampleCube.transform.position = this.transform.position;
            _instantiateSampleCube.transform.parent = this.transform; // Set the parent to this GameObject
            _instantiateSampleCube.name = "SampleCube_" + i; // Name the cube for easier identification
            this.transform.eulerAngles = new Vector3(0, i * (360f / 512), 0); // Set the rotation of the parent object
            _instantiateSampleCube.transform.position = Vector3.forward * 100; // Position the cube in front of the parent object
            _sampleCube[i] = _instantiateSampleCube; // Store the instantiated cube in the array
        }
    }

    // Update is called once per frame
    void Update()
    {
        for (int i = 0; i < 512; i++)
        {
            if( _sampleCube[i] != null )
            {
                _sampleCube[i].transform.localScale = new Vector3(10, (AudioPeer._samples[i] * _maxScale) + 2, 10); // Scale the cube based on the audio sample
            }
        }
    }
}
