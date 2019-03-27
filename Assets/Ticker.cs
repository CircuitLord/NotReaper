using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Ticker : MonoBehaviour {

    public LayerMask layermask;
    AudioSource aud;

    private void Start()
    {
        aud = GetComponent<AudioSource>();
        
    }

    private void OnTriggerEnter(Collider other)
    {
        if (layermask == (layermask | (1 << other.gameObject.layer)))
        {
            if (other.transform.position.z > -1)
            { 
                aud.time = 0;
                aud.Play();
            }
        }
    }
}
