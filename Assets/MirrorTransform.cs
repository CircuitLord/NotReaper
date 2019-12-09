using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MirrorTransform : MonoBehaviour
{
    public Transform OtherTransform;

    // Update is called once per frame
    void LateUpdate()
    {
        this.transform.localPosition = OtherTransform.localPosition;
        this.transform.localScale = OtherTransform.localScale;
    }
}
