using System.Collections;
using System.Collections.Generic;
using NotReaper.Models;
using UnityEngine;

namespace NotReaper.ScriptableObjects {

    [CreateAssetMenu(fileName = "TimelineData")]
    public class TimelineData : ScriptableObject {

        [SerializeField] private float bpm;

        [SerializeField] private CueFile cuefile;


    }
}