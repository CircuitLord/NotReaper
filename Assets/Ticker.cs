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
        Debug.Log("ping");
        if (layermask == (layermask | (1 << other.gameObject.layer)))
        {
            Debug.Log(other.transform.position.z);
            if (other.transform.position.z > -1)
            { 
                Debug.Log("pong");
                aud.time = 0;
                aud.Play();
            }
        }
    }
}
