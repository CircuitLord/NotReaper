using UnityEngine;

public class UnrestrictFramerate : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
//        Debug.Log(Application.targetFrameRate.ToString());
        Application.targetFrameRate = 120;
    }
}
