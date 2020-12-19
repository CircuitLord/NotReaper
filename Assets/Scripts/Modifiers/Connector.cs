using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using NotReaper;

namespace NotReaper.Modifier
{
    public class Connector : MonoBehaviour
    {
        public Vector3 originalScale { get; private set; }

        private void Start()
        {
            originalScale = transform.localScale;
        }
    }
}

