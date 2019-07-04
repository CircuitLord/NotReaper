using System.Collections;
using System.Collections.Generic;
using NotReaper.Models;
using NotReaper.Targets;
using UnityEngine;
using UnityEngine.UI;

public class Ticker : MonoBehaviour {

    public LayerMask layermask;
    AudioSource aud;

    public AudioSource kick;
    public AudioSource snare;
    public AudioSource percussion;
    public AudioSource chainStart;
    public AudioSource chainNode;
    public AudioSource melee;

    public Slider volumeSlider;

    private float volume;

    private void Start() {
        aud = GetComponent<AudioSource>();
        volume = PlayerPrefs.GetFloat("TickVol");
        volumeSlider.value = volume;
    }

    public void VolumeChange(Slider vol) {
        volume = vol.value;
        PlayerPrefs.SetFloat("TickVol", volume);
    }

    private void OnTriggerEnter(Collider other) {
        if (layermask == (layermask | (1 << other.gameObject.layer))) {
            if (other.transform.position.z > -1) {
                switch (other.GetComponentInChildren<Target>().velocity) {
                    case TargetVelocity.Standard:
                        {
                            kick.time = 0;
                            kick.volume = volume;
                            kick.Play();
                            break;
                        }
                    case TargetVelocity.Snare:
                        {
                            snare.time = 0;
                            snare.volume = volume;
                            snare.Play();
                            break;
                        }
                    case TargetVelocity.Percussion:
                        {
                            percussion.time = 0;
                            percussion.volume = volume;
                            percussion.Play();
                            break;
                        }
                    case TargetVelocity.ChainStart:
                        {
                            chainStart.time = 0;
                            chainStart.volume = volume;
                            chainStart.Play();
                            break;
                        }

                    case TargetVelocity.Chain:
                        {
                            chainNode.time = 0;
                            chainNode.volume = volume;
                            chainNode.Play();
                            break;
                        }

                    case TargetVelocity.Melee:
                        {
                            melee.time = 0;
                            melee.volume = volume;
                            melee.Play();
                            break;
                        }

                    default:
                        {
                            aud.time = 0;
                            aud.Play();
                            break;
                        }
                }

            }
        }
    }
}