using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BGFPS : MonoBehaviour
{

    private int rr;
    void OnApplicationFocus(bool hasFocus)
    {
        if(hasFocus)
        {   
            var rate = 0;
            foreach(var res in Screen.resolutions)
            {
                rate = res.refreshRate > rate ? res.refreshRate : rate;
            }

            rr = rate;

            Application.targetFrameRate = rate;
        }
        else if (rr > 60)
        {
            Application.targetFrameRate = 60;
        }
    }

}
