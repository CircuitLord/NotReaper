using System.Collections;
using UnityEngine;
using UnityEditor;

namespace Michsky.UI.ModernUIPack
{
    public class MUIPEditor : MonoBehaviour
    {
        [MenuItem("GameObject/Modern UI Pack/Animated Icons/Hamburger to Exit", false, 0)]
        static void AIHTE()
        {
            GameObject clone = Instantiate(Resources.Load<GameObject>("Animated Icons/Hamburger to Exit"), Vector3.zero, Quaternion.identity) as GameObject;

            try
            {
                clone.transform.SetParent(GameObject.Find("Canvas").transform, false);
                clone.name = clone.name.Replace("(Clone)", "").Trim();
            }
            catch
            {
                clone.name = clone.name.Replace("(Clone)", "").Trim();
                Debug.Log("No object found named 'Canvas', creating the object separately.");
            }
        }

        [MenuItem("GameObject/Modern UI Pack/Animated Icons/Heart Pop", false, 0)]
        static void AIHP()
        {
            GameObject clone = Instantiate(Resources.Load<GameObject>("Animated Icons/Heart Pop"), Vector3.zero, Quaternion.identity) as GameObject;

            try
            {
                clone.transform.SetParent(GameObject.Find("Canvas").transform, false);
                clone.name = clone.name.Replace("(Clone)", "").Trim();
            }
            catch
            {
                clone.name = clone.name.Replace("(Clone)", "").Trim();
                Debug.Log("No object found named 'Canvas', creating the object separately.");
            }
        }

        [MenuItem("GameObject/Modern UI Pack/Animated Icons/Lock", false, 0)]
        static void AIL()
        {
            GameObject clone = Instantiate(Resources.Load<GameObject>("Animated Icons/Lock"), Vector3.zero, Quaternion.identity) as GameObject;

            try
            {
                clone.transform.SetParent(GameObject.Find("Canvas").transform, false);
                clone.name = clone.name.Replace("(Clone)", "").Trim();
            }
            catch
            {
                clone.name = clone.name.Replace("(Clone)", "").Trim();
                Debug.Log("No object found named 'Canvas', creating the object separately.");
            }
        }

        [MenuItem("GameObject/Modern UI Pack/Animated Icons/Message Bubbles", false, 0)]
        static void AILMB()
        {
            GameObject clone = Instantiate(Resources.Load<GameObject>("Animated Icons/Message Bubbles"), Vector3.zero, Quaternion.identity) as GameObject;

            try
            {
                clone.transform.SetParent(GameObject.Find("Canvas").transform, false);
                clone.name = clone.name.Replace("(Clone)", "").Trim();
            }
            catch
            {
                clone.name = clone.name.Replace("(Clone)", "").Trim();
                Debug.Log("No object found named 'Canvas', creating the object separately.");
            }
        }

        [MenuItem("GameObject/Modern UI Pack/Animated Icons/Sand Clock", false, 0)]
        static void AISC()
        {
            GameObject clone = Instantiate(Resources.Load<GameObject>("Animated Icons/Sand Clock"), Vector3.zero, Quaternion.identity) as GameObject;

            try
            {
                clone.transform.SetParent(GameObject.Find("Canvas").transform, false);
                clone.name = clone.name.Replace("(Clone)", "").Trim();
            }
            catch
            {
                clone.name = clone.name.Replace("(Clone)", "").Trim();
                Debug.Log("No object found named 'Canvas', creating the object separately.");
            }
        }

        [MenuItem("GameObject/Modern UI Pack/Animated Icons/Switch", false, 0)]
        static void AIS()
        {
            GameObject clone = Instantiate(Resources.Load<GameObject>("Animated Icons/Switch"), Vector3.zero, Quaternion.identity) as GameObject;

            try
            {
                clone.transform.SetParent(GameObject.Find("Canvas").transform, false);
                clone.name = clone.name.Replace("(Clone)", "").Trim();
            }
            catch
            {
                clone.name = clone.name.Replace("(Clone)", "").Trim();
                Debug.Log("No object found named 'Canvas', creating the object separately.");
            }
        }

        [MenuItem("GameObject/Modern UI Pack/Animated Icons/Yes to No", false, 0)]
        static void AIYTN()
        {
            GameObject clone = Instantiate(Resources.Load<GameObject>("Animated Icons/Yes to No"), Vector3.zero, Quaternion.identity) as GameObject;

            try
            {
                clone.transform.SetParent(GameObject.Find("Canvas").transform, false);
                clone.name = clone.name.Replace("(Clone)", "").Trim();
            }
            catch
            {
                clone.name = clone.name.Replace("(Clone)", "").Trim();
                Debug.Log("No object found named 'Canvas', creating the object separately.");
            }
        }

        [MenuItem("GameObject/Modern UI Pack/Buttons/Basic/Blue", false, 0)]
        static void BBBL()
        {
            GameObject clone = Instantiate(Resources.Load<GameObject>("Buttons/Basic/Blue"), Vector3.zero, Quaternion.identity) as GameObject;

            try
            {
                clone.transform.SetParent(GameObject.Find("Canvas").transform, false);
                clone.name = clone.name.Replace("(Clone)", "").Trim();
            }
            catch
            {
                clone.name = clone.name.Replace("(Clone)", "").Trim();
                Debug.Log("No object found named 'Canvas', creating the object separately.");
            }
        }

        [MenuItem("GameObject/Modern UI Pack/Buttons/Basic/Brown", false, 0)]
        static void BBBRW()
        {
            GameObject clone = Instantiate(Resources.Load<GameObject>("Buttons/Basic/Brown"), Vector3.zero, Quaternion.identity) as GameObject;

            try
            {
                clone.transform.SetParent(GameObject.Find("Canvas").transform, false);
                clone.name = clone.name.Replace("(Clone)", "").Trim();
            }
            catch
            {
                clone.name = clone.name.Replace("(Clone)", "").Trim();
                Debug.Log("No object found named 'Canvas', creating the object separately.");
            }
        }

        [MenuItem("GameObject/Modern UI Pack/Buttons/Basic/Gray", false, 0)]
        static void BBGR()
        {
            GameObject clone = Instantiate(Resources.Load<GameObject>("Buttons/Basic/Gray"), Vector3.zero, Quaternion.identity) as GameObject;

            try
            {
                clone.transform.SetParent(GameObject.Find("Canvas").transform, false);
                clone.name = clone.name.Replace("(Clone)", "").Trim();
            }
            catch
            {
                clone.name = clone.name.Replace("(Clone)", "").Trim();
                Debug.Log("No object found named 'Canvas', creating the object separately.");
            }
        }

        [MenuItem("GameObject/Modern UI Pack/Buttons/Basic/Green", false, 0)]
        static void BBGRE()
        {
            GameObject clone = Instantiate(Resources.Load<GameObject>("Buttons/Basic/Green"), Vector3.zero, Quaternion.identity) as GameObject;

            try
            {
                clone.transform.SetParent(GameObject.Find("Canvas").transform, false);
                clone.name = clone.name.Replace("(Clone)", "").Trim();
            }
            catch
            {
                clone.name = clone.name.Replace("(Clone)", "").Trim();
                Debug.Log("No object found named 'Canvas', creating the object separately.");
            }
        }

        [MenuItem("GameObject/Modern UI Pack/Buttons/Basic/Night", false, 0)]
        static void BBNI()
        {
            GameObject clone = Instantiate(Resources.Load<GameObject>("Buttons/Basic/Night"), Vector3.zero, Quaternion.identity) as GameObject;

            try
            {
                clone.transform.SetParent(GameObject.Find("Canvas").transform, false);
                clone.name = clone.name.Replace("(Clone)", "").Trim();
            }
            catch
            {
                clone.name = clone.name.Replace("(Clone)", "").Trim();
                Debug.Log("No object found named 'Canvas', creating the object separately.");
            }
        }

        [MenuItem("GameObject/Modern UI Pack/Buttons/Basic/Orange", false, 0)]
        static void BBOR()
        {
            GameObject clone = Instantiate(Resources.Load<GameObject>("Buttons/Basic/Orange"), Vector3.zero, Quaternion.identity) as GameObject;

            try
            {
                clone.transform.SetParent(GameObject.Find("Canvas").transform, false);
                clone.name = clone.name.Replace("(Clone)", "").Trim();
            }
            catch
            {
                clone.name = clone.name.Replace("(Clone)", "").Trim();
                Debug.Log("No object found named 'Canvas', creating the object separately.");
            }
        }

        [MenuItem("GameObject/Modern UI Pack/Buttons/Basic/Pink", false, 0)]
        static void BBPIN()
        {
            GameObject clone = Instantiate(Resources.Load<GameObject>("Buttons/Basic/Pink"), Vector3.zero, Quaternion.identity) as GameObject;

            try
            {
                clone.transform.SetParent(GameObject.Find("Canvas").transform, false);
                clone.name = clone.name.Replace("(Clone)", "").Trim();
            }
            catch
            {
                clone.name = clone.name.Replace("(Clone)", "").Trim();
                Debug.Log("No object found named 'Canvas', creating the object separately.");
            }
        }

        [MenuItem("GameObject/Modern UI Pack/Buttons/Basic/Purple", false, 0)]
        static void BBPURP()
        {
            GameObject clone = Instantiate(Resources.Load<GameObject>("Buttons/Basic/Purple"), Vector3.zero, Quaternion.identity) as GameObject;

            try
            {
                clone.transform.SetParent(GameObject.Find("Canvas").transform, false);
                clone.name = clone.name.Replace("(Clone)", "").Trim();
            }
            catch
            {
                clone.name = clone.name.Replace("(Clone)", "").Trim();
                Debug.Log("No object found named 'Canvas', creating the object separately.");
            }
        }

        [MenuItem("GameObject/Modern UI Pack/Buttons/Basic/Red", false, 0)]
        static void BBRED()
        {
            GameObject clone = Instantiate(Resources.Load<GameObject>("Buttons/Basic/Red"), Vector3.zero, Quaternion.identity) as GameObject;

            try
            {
                clone.transform.SetParent(GameObject.Find("Canvas").transform, false);
                clone.name = clone.name.Replace("(Clone)", "").Trim();
            }
            catch
            {
                clone.name = clone.name.Replace("(Clone)", "").Trim();
                Debug.Log("No object found named 'Canvas', creating the object separately.");
            }
        }

        [MenuItem("GameObject/Modern UI Pack/Buttons/Basic/White", false, 0)]
        static void BBWHI()
        {
            GameObject clone = Instantiate(Resources.Load<GameObject>("Buttons/Basic/White"), Vector3.zero, Quaternion.identity) as GameObject;

            try
            {
                clone.transform.SetParent(GameObject.Find("Canvas").transform, false);
                clone.name = clone.name.Replace("(Clone)", "").Trim();
            }
            catch
            {
                clone.name = clone.name.Replace("(Clone)", "").Trim();
                Debug.Log("No object found named 'Canvas', creating the object separately.");
            }
        }

        [MenuItem("GameObject/Modern UI Pack/Buttons/Basic Gradient/Blue", false, 0)]
        static void BGBL()
        {
            GameObject clone = Instantiate(Resources.Load<GameObject>("Buttons/Basic Gradient/Blue"), Vector3.zero, Quaternion.identity) as GameObject;

            try
            {
                clone.transform.SetParent(GameObject.Find("Canvas").transform, false);
                clone.name = clone.name.Replace("(Clone)", "").Trim();
            }
            catch
            {
                clone.name = clone.name.Replace("(Clone)", "").Trim();
                Debug.Log("No object found named 'Canvas', creating the object separately.");
            }
        }

        [MenuItem("GameObject/Modern UI Pack/Buttons/Basic Gradient/Brown", false, 0)]
        static void BGBRW()
        {
            GameObject clone = Instantiate(Resources.Load<GameObject>("Buttons/Basic Gradient/Brown"), Vector3.zero, Quaternion.identity) as GameObject;

            try
            {
                clone.transform.SetParent(GameObject.Find("Canvas").transform, false);
                clone.name = clone.name.Replace("(Clone)", "").Trim();
            }
            catch
            {
                clone.name = clone.name.Replace("(Clone)", "").Trim();
                Debug.Log("No object found named 'Canvas', creating the object separately.");
            }
        }

        [MenuItem("GameObject/Modern UI Pack/Buttons/Basic Gradient/Gray", false, 0)]
        static void BGGR()
        {
            GameObject clone = Instantiate(Resources.Load<GameObject>("Buttons/Basic Gradient/Gray"), Vector3.zero, Quaternion.identity) as GameObject;

            try
            {
                clone.transform.SetParent(GameObject.Find("Canvas").transform, false);
                clone.name = clone.name.Replace("(Clone)", "").Trim();
            }
            catch
            {
                clone.name = clone.name.Replace("(Clone)", "").Trim();
                Debug.Log("No object found named 'Canvas', creating the object separately.");
            }
        }

        [MenuItem("GameObject/Modern UI Pack/Buttons/Basic Gradient/Green", false, 0)]
        static void BGGRE()
        {
            GameObject clone = Instantiate(Resources.Load<GameObject>("Buttons/Basic Gradient/Green"), Vector3.zero, Quaternion.identity) as GameObject;

            try
            {
                clone.transform.SetParent(GameObject.Find("Canvas").transform, false);
                clone.name = clone.name.Replace("(Clone)", "").Trim();
            }
            catch
            {
                clone.name = clone.name.Replace("(Clone)", "").Trim();
                Debug.Log("No object found named 'Canvas', creating the object separately.");
            }
        }

        [MenuItem("GameObject/Modern UI Pack/Buttons/Basic Gradient/Night", false, 0)]
        static void BGNI()
        {
            GameObject clone = Instantiate(Resources.Load<GameObject>("Buttons/Basic Gradient/Night"), Vector3.zero, Quaternion.identity) as GameObject;

            try
            {
                clone.transform.SetParent(GameObject.Find("Canvas").transform, false);
                clone.name = clone.name.Replace("(Clone)", "").Trim();
            }
            catch
            {
                clone.name = clone.name.Replace("(Clone)", "").Trim();
                Debug.Log("No object found named 'Canvas', creating the object separately.");
            }
        }

        [MenuItem("GameObject/Modern UI Pack/Buttons/Basic Gradient/Orange", false, 0)]
        static void BGOR()
        {
            GameObject clone = Instantiate(Resources.Load<GameObject>("Buttons/Basic Gradient/Orange"), Vector3.zero, Quaternion.identity) as GameObject;

            try
            {
                clone.transform.SetParent(GameObject.Find("Canvas").transform, false);
                clone.name = clone.name.Replace("(Clone)", "").Trim();
            }
            catch
            {
                clone.name = clone.name.Replace("(Clone)", "").Trim();
                Debug.Log("No object found named 'Canvas', creating the object separately.");
            }
        }

        [MenuItem("GameObject/Modern UI Pack/Buttons/Basic Gradient/Pink", false, 0)]
        static void BGPIN()
        {
            GameObject clone = Instantiate(Resources.Load<GameObject>("Buttons/Basic Gradient/Pink"), Vector3.zero, Quaternion.identity) as GameObject;

            try
            {
                clone.transform.SetParent(GameObject.Find("Canvas").transform, false);
                clone.name = clone.name.Replace("(Clone)", "").Trim();
            }
            catch
            {
                clone.name = clone.name.Replace("(Clone)", "").Trim();
                Debug.Log("No object found named 'Canvas', creating the object separately.");
            }
        }

        [MenuItem("GameObject/Modern UI Pack/Buttons/Basic Gradient/Purple", false, 0)]
        static void BGPURP()
        {
            GameObject clone = Instantiate(Resources.Load<GameObject>("Buttons/Basic Gradient/Purple"), Vector3.zero, Quaternion.identity) as GameObject;

            try
            {
                clone.transform.SetParent(GameObject.Find("Canvas").transform, false);
                clone.name = clone.name.Replace("(Clone)", "").Trim();
            }
            catch
            {
                clone.name = clone.name.Replace("(Clone)", "").Trim();
                Debug.Log("No object found named 'Canvas', creating the object separately.");
            }
        }

        [MenuItem("GameObject/Modern UI Pack/Buttons/Basic Gradient/Red", false, 0)]
        static void BGRED()
        {
            GameObject clone = Instantiate(Resources.Load<GameObject>("Buttons/Basic Gradient/Red"), Vector3.zero, Quaternion.identity) as GameObject;

            try
            {
                clone.transform.SetParent(GameObject.Find("Canvas").transform, false);
                clone.name = clone.name.Replace("(Clone)", "").Trim();
            }
            catch
            {
                clone.name = clone.name.Replace("(Clone)", "").Trim();
                Debug.Log("No object found named 'Canvas', creating the object separately.");
            }
        }

        [MenuItem("GameObject/Modern UI Pack/Buttons/Basic Gradient/White", false, 0)]
        static void BGWHI()
        {
            GameObject clone = Instantiate(Resources.Load<GameObject>("Buttons/Basic Gradient/White"), Vector3.zero, Quaternion.identity) as GameObject;

            try
            {
                clone.transform.SetParent(GameObject.Find("Canvas").transform, false);
                clone.name = clone.name.Replace("(Clone)", "").Trim();
            }
            catch
            {
                clone.name = clone.name.Replace("(Clone)", "").Trim();
                Debug.Log("No object found named 'Canvas', creating the object separately.");
            }
        }

        [MenuItem("GameObject/Modern UI Pack/Buttons/Basic Outline/Blue", false, 0)]
        static void BOBL()
        {
            GameObject clone = Instantiate(Resources.Load<GameObject>("Buttons/Basic Outline/Blue"), Vector3.zero, Quaternion.identity) as GameObject;

            try
            {
                clone.transform.SetParent(GameObject.Find("Canvas").transform, false);
                clone.name = clone.name.Replace("(Clone)", "").Trim();
            }
            catch
            {
                clone.name = clone.name.Replace("(Clone)", "").Trim();
                Debug.Log("No object found named 'Canvas', creating the object separately.");
            }
        }

        [MenuItem("GameObject/Modern UI Pack/Buttons/Basic Outline/Brown", false, 0)]
        static void BOBRW()
        {
            GameObject clone = Instantiate(Resources.Load<GameObject>("Buttons/Basic Outline/Brown"), Vector3.zero, Quaternion.identity) as GameObject;

            try
            {
                clone.transform.SetParent(GameObject.Find("Canvas").transform, false);
                clone.name = clone.name.Replace("(Clone)", "").Trim();
            }
            catch
            {
                clone.name = clone.name.Replace("(Clone)", "").Trim();
                Debug.Log("No object found named 'Canvas', creating the object separately.");
            }
        }

        [MenuItem("GameObject/Modern UI Pack/Buttons/Basic Outline/Gray", false, 0)]
        static void BOGR()
        {
            GameObject clone = Instantiate(Resources.Load<GameObject>("Buttons/Basic Outline/Gray"), Vector3.zero, Quaternion.identity) as GameObject;

            try
            {
                clone.transform.SetParent(GameObject.Find("Canvas").transform, false);
                clone.name = clone.name.Replace("(Clone)", "").Trim();
            }
            catch
            {
                clone.name = clone.name.Replace("(Clone)", "").Trim();
                Debug.Log("No object found named 'Canvas', creating the object separately.");
            }
        }

        [MenuItem("GameObject/Modern UI Pack/Buttons/Basic Outline/Green", false, 0)]
        static void BOGRE()
        {
            GameObject clone = Instantiate(Resources.Load<GameObject>("Buttons/Basic Outline/Green"), Vector3.zero, Quaternion.identity) as GameObject;

            try
            {
                clone.transform.SetParent(GameObject.Find("Canvas").transform, false);
                clone.name = clone.name.Replace("(Clone)", "").Trim();
            }
            catch
            {
                clone.name = clone.name.Replace("(Clone)", "").Trim();
                Debug.Log("No object found named 'Canvas', creating the object separately.");
            }
        }

        [MenuItem("GameObject/Modern UI Pack/Buttons/Basic Outline/Night", false, 0)]
        static void BONI()
        {
            GameObject clone = Instantiate(Resources.Load<GameObject>("Buttons/Basic Outline/Night"), Vector3.zero, Quaternion.identity) as GameObject;

            try
            {
                clone.transform.SetParent(GameObject.Find("Canvas").transform, false);
                clone.name = clone.name.Replace("(Clone)", "").Trim();
            }
            catch
            {
                clone.name = clone.name.Replace("(Clone)", "").Trim();
                Debug.Log("No object found named 'Canvas', creating the object separately.");
            }
        }

        [MenuItem("GameObject/Modern UI Pack/Buttons/Basic Outline/Orange", false, 0)]
        static void BOOR()
        {
            GameObject clone = Instantiate(Resources.Load<GameObject>("Buttons/Basic Outline/Orange"), Vector3.zero, Quaternion.identity) as GameObject;

            try
            {
                clone.transform.SetParent(GameObject.Find("Canvas").transform, false);
                clone.name = clone.name.Replace("(Clone)", "").Trim();
            }
            catch
            {
                clone.name = clone.name.Replace("(Clone)", "").Trim();
                Debug.Log("No object found named 'Canvas', creating the object separately.");
            }
        }

        [MenuItem("GameObject/Modern UI Pack/Buttons/Basic Outline/Pink", false, 0)]
        static void BOPIN()
        {
            GameObject clone = Instantiate(Resources.Load<GameObject>("Buttons/Basic Outline/Pink"), Vector3.zero, Quaternion.identity) as GameObject;

            try
            {
                clone.transform.SetParent(GameObject.Find("Canvas").transform, false);
                clone.name = clone.name.Replace("(Clone)", "").Trim();
            }
            catch
            {
                clone.name = clone.name.Replace("(Clone)", "").Trim();
                Debug.Log("No object found named 'Canvas', creating the object separately.");
            }
        }

        [MenuItem("GameObject/Modern UI Pack/Buttons/Basic Outline/Purple", false, 0)]
        static void BOPURP()
        {
            GameObject clone = Instantiate(Resources.Load<GameObject>("Buttons/Basic Outline/Purple"), Vector3.zero, Quaternion.identity) as GameObject;

            try
            {
                clone.transform.SetParent(GameObject.Find("Canvas").transform, false);
                clone.name = clone.name.Replace("(Clone)", "").Trim();
            }
            catch
            {
                clone.name = clone.name.Replace("(Clone)", "").Trim();
                Debug.Log("No object found named 'Canvas', creating the object separately.");
            }
        }

        [MenuItem("GameObject/Modern UI Pack/Buttons/Basic Outline/Red", false, 0)]
        static void BORED()
        {
            GameObject clone = Instantiate(Resources.Load<GameObject>("Buttons/Basic Outline/Red"), Vector3.zero, Quaternion.identity) as GameObject;

            try
            {
                clone.transform.SetParent(GameObject.Find("Canvas").transform, false);
                clone.name = clone.name.Replace("(Clone)", "").Trim();
            }
            catch
            {
                clone.name = clone.name.Replace("(Clone)", "").Trim();
                Debug.Log("No object found named 'Canvas', creating the object separately.");
            }
        }

        [MenuItem("GameObject/Modern UI Pack/Buttons/Basic Outline/White", false, 0)]
        static void BOWHI()
        {
            GameObject clone = Instantiate(Resources.Load<GameObject>("Buttons/Basic Outline/White"), Vector3.zero, Quaternion.identity) as GameObject;

            try
            {
                clone.transform.SetParent(GameObject.Find("Canvas").transform, false);
                clone.name = clone.name.Replace("(Clone)", "").Trim();
            }
            catch
            {
                clone.name = clone.name.Replace("(Clone)", "").Trim();
                Debug.Log("No object found named 'Canvas', creating the object separately.");
            }
        }

        [MenuItem("GameObject/Modern UI Pack/Buttons/Basic Outline Gradient/Blue", false, 0)]
        static void BOGBL()
        {
            GameObject clone = Instantiate(Resources.Load<GameObject>("Buttons/Basic Outline Gradient/Blue"), Vector3.zero, Quaternion.identity) as GameObject;

            try
            {
                clone.transform.SetParent(GameObject.Find("Canvas").transform, false);
                clone.name = clone.name.Replace("(Clone)", "").Trim();
            }
            catch
            {
                clone.name = clone.name.Replace("(Clone)", "").Trim();
                Debug.Log("No object found named 'Canvas', creating the object separately.");
            }
        }

        [MenuItem("GameObject/Modern UI Pack/Buttons/Basic Outline Gradient/Brown", false, 0)]
        static void BOGBRW()
        {
            GameObject clone = Instantiate(Resources.Load<GameObject>("Buttons/Basic Outline Gradient/Brown"), Vector3.zero, Quaternion.identity) as GameObject;

            try
            {
                clone.transform.SetParent(GameObject.Find("Canvas").transform, false);
                clone.name = clone.name.Replace("(Clone)", "").Trim();
            }
            catch
            {
                clone.name = clone.name.Replace("(Clone)", "").Trim();
                Debug.Log("No object found named 'Canvas', creating the object separately.");
            }
        }

        [MenuItem("GameObject/Modern UI Pack/Buttons/Basic Outline Gradient/Gray", false, 0)]
        static void BOGBGR()
        {
            GameObject clone = Instantiate(Resources.Load<GameObject>("Buttons/Basic Outline Gradient/Gray"), Vector3.zero, Quaternion.identity) as GameObject;

            try
            {
                clone.transform.SetParent(GameObject.Find("Canvas").transform, false);
                clone.name = clone.name.Replace("(Clone)", "").Trim();
            }
            catch
            {
                clone.name = clone.name.Replace("(Clone)", "").Trim();
                Debug.Log("No object found named 'Canvas', creating the object separately.");
            }
        }

        [MenuItem("GameObject/Modern UI Pack/Buttons/Basic Outline Gradient/Green", false, 0)]
        static void BOGGRE()
        {
            GameObject clone = Instantiate(Resources.Load<GameObject>("Buttons/Basic Outline Gradient/Green"), Vector3.zero, Quaternion.identity) as GameObject;

            try
            {
                clone.transform.SetParent(GameObject.Find("Canvas").transform, false);
                clone.name = clone.name.Replace("(Clone)", "").Trim();
            }
            catch
            {
                clone.name = clone.name.Replace("(Clone)", "").Trim();
                Debug.Log("No object found named 'Canvas', creating the object separately.");
            }
        }

        [MenuItem("GameObject/Modern UI Pack/Buttons/Basic Outline Gradient/Night", false, 0)]
        static void BOGNI()
        {
            GameObject clone = Instantiate(Resources.Load<GameObject>("Buttons/Basic Outline Gradient/Night"), Vector3.zero, Quaternion.identity) as GameObject;

            try
            {
                clone.transform.SetParent(GameObject.Find("Canvas").transform, false);
                clone.name = clone.name.Replace("(Clone)", "").Trim();
            }
            catch
            {
                clone.name = clone.name.Replace("(Clone)", "").Trim();
                Debug.Log("No object found named 'Canvas', creating the object separately.");
            }
        }

        [MenuItem("GameObject/Modern UI Pack/Buttons/Basic Outline Gradient/Orange", false, 0)]
        static void BOGOR()
        {
            GameObject clone = Instantiate(Resources.Load<GameObject>("Buttons/Basic Outline Gradient/Orange"), Vector3.zero, Quaternion.identity) as GameObject;

            try
            {
                clone.transform.SetParent(GameObject.Find("Canvas").transform, false);
                clone.name = clone.name.Replace("(Clone)", "").Trim();
            }
            catch
            {
                clone.name = clone.name.Replace("(Clone)", "").Trim();
                Debug.Log("No object found named 'Canvas', creating the object separately.");
            }
        }

        [MenuItem("GameObject/Modern UI Pack/Buttons/Basic Outline Gradient/Pink", false, 0)]
        static void BOGPIN()
        {
            GameObject clone = Instantiate(Resources.Load<GameObject>("Buttons/Basic Outline Gradient/Pink"), Vector3.zero, Quaternion.identity) as GameObject;

            try
            {
                clone.transform.SetParent(GameObject.Find("Canvas").transform, false);
                clone.name = clone.name.Replace("(Clone)", "").Trim();
            }
            catch
            {
                clone.name = clone.name.Replace("(Clone)", "").Trim();
                Debug.Log("No object found named 'Canvas', creating the object separately.");
            }
        }

        [MenuItem("GameObject/Modern UI Pack/Buttons/Basic Outline Gradient/Purple", false, 0)]
        static void BOGPURP()
        {
            GameObject clone = Instantiate(Resources.Load<GameObject>("Buttons/Basic Outline Gradient/Purple"), Vector3.zero, Quaternion.identity) as GameObject;

            try
            {
                clone.transform.SetParent(GameObject.Find("Canvas").transform, false);
                clone.name = clone.name.Replace("(Clone)", "").Trim();
            }
            catch
            {
                clone.name = clone.name.Replace("(Clone)", "").Trim();
                Debug.Log("No object found named 'Canvas', creating the object separately.");
            }
        }

        [MenuItem("GameObject/Modern UI Pack/Buttons/Basic Outline Gradient/Red", false, 0)]
        static void BOGRED()
        {
            GameObject clone = Instantiate(Resources.Load<GameObject>("Buttons/Basic Outline Gradient/Red"), Vector3.zero, Quaternion.identity) as GameObject;

            try
            {
                clone.transform.SetParent(GameObject.Find("Canvas").transform, false);
                clone.name = clone.name.Replace("(Clone)", "").Trim();
            }
            catch
            {
                clone.name = clone.name.Replace("(Clone)", "").Trim();
                Debug.Log("No object found named 'Canvas', creating the object separately.");
            }
        }

        [MenuItem("GameObject/Modern UI Pack/Buttons/Basic Outline Gradient/White", false, 0)]
        static void BOGWHI()
        {
            GameObject clone = Instantiate(Resources.Load<GameObject>("Buttons/Basic Outline Gradient/White"), Vector3.zero, Quaternion.identity) as GameObject;

            try
            {
                clone.transform.SetParent(GameObject.Find("Canvas").transform, false);
                clone.name = clone.name.Replace("(Clone)", "").Trim();
            }
            catch
            {
                clone.name = clone.name.Replace("(Clone)", "").Trim();
                Debug.Log("No object found named 'Canvas', creating the object separately.");
            }
        }

        [MenuItem("GameObject/Modern UI Pack/Buttons/Basic Outline With Image/Gray", false, 0)]
        static void BOWIGRA()
        {
            GameObject clone = Instantiate(Resources.Load<GameObject>("Buttons/Basic Outline With Image/Gray"), Vector3.zero, Quaternion.identity) as GameObject;

            try
            {
                clone.transform.SetParent(GameObject.Find("Canvas").transform, false);
                clone.name = clone.name.Replace("(Clone)", "").Trim();
            }
            catch
            {
                clone.name = clone.name.Replace("(Clone)", "").Trim();
                Debug.Log("No object found named 'Canvas', creating the object separately.");
            }
        }

        [MenuItem("GameObject/Modern UI Pack/Buttons/Basic Outline With Image/White", false, 0)]
        static void BOWIWHI()
        {
            GameObject clone = Instantiate(Resources.Load<GameObject>("Buttons/Basic Outline With Image/White"), Vector3.zero, Quaternion.identity) as GameObject;

            try
            {
                clone.transform.SetParent(GameObject.Find("Canvas").transform, false);
                clone.name = clone.name.Replace("(Clone)", "").Trim();
            }
            catch
            {
                clone.name = clone.name.Replace("(Clone)", "").Trim();
                Debug.Log("No object found named 'Canvas', creating the object separately.");
            }
        }

        [MenuItem("GameObject/Modern UI Pack/Buttons/Box Outline With Image/Gray", false, 0)]
        static void BOXIGRA()
        {
            GameObject clone = Instantiate(Resources.Load<GameObject>("Buttons/Box Outline With Image/Gray"), Vector3.zero, Quaternion.identity) as GameObject;

            try
            {
                clone.transform.SetParent(GameObject.Find("Canvas").transform, false);
                clone.name = clone.name.Replace("(Clone)", "").Trim();
            }
            catch
            {
                clone.name = clone.name.Replace("(Clone)", "").Trim();
                Debug.Log("No object found named 'Canvas', creating the object separately.");
            }
        }

        [MenuItem("GameObject/Modern UI Pack/Buttons/Box Outline With Image/White", false, 0)]
        static void BOXIWHI()
        {
            GameObject clone = Instantiate(Resources.Load<GameObject>("Buttons/Box Outline With Image/White"), Vector3.zero, Quaternion.identity) as GameObject;

            try
            {
                clone.transform.SetParent(GameObject.Find("Canvas").transform, false);
                clone.name = clone.name.Replace("(Clone)", "").Trim();
            }
            catch
            {
                clone.name = clone.name.Replace("(Clone)", "").Trim();
                Debug.Log("No object found named 'Canvas', creating the object separately.");
            }
        }

        [MenuItem("GameObject/Modern UI Pack/Buttons/Box With Image/Gray", false, 0)]
        static void CIRGRA()
        {
            GameObject clone = Instantiate(Resources.Load<GameObject>("Buttons/Box With Image/Gray"), Vector3.zero, Quaternion.identity) as GameObject;

            try
            {
                clone.transform.SetParent(GameObject.Find("Canvas").transform, false);
                clone.name = clone.name.Replace("(Clone)", "").Trim();
            }
            catch
            {
                clone.name = clone.name.Replace("(Clone)", "").Trim();
                Debug.Log("No object found named 'Canvas', creating the object separately.");
            }
        }

        [MenuItem("GameObject/Modern UI Pack/Buttons/Box With Image/White", false, 0)]
        static void CIRWHI()
        {
            GameObject clone = Instantiate(Resources.Load<GameObject>("Buttons/Box With Image/White"), Vector3.zero, Quaternion.identity) as GameObject;

            try
            {
                clone.transform.SetParent(GameObject.Find("Canvas").transform, false);
                clone.name = clone.name.Replace("(Clone)", "").Trim();
            }
            catch
            {
                clone.name = clone.name.Replace("(Clone)", "").Trim();
                Debug.Log("No object found named 'Canvas', creating the object separately.");
            }
        }

        [MenuItem("GameObject/Modern UI Pack/Buttons/Circle Outline With Image/Gray", false, 0)]
        static void CIRCOGRA()
        {
            GameObject clone = Instantiate(Resources.Load<GameObject>("Buttons/Circle Outline With Image/Gray"), Vector3.zero, Quaternion.identity) as GameObject;

            try
            {
                clone.transform.SetParent(GameObject.Find("Canvas").transform, false);
                clone.name = clone.name.Replace("(Clone)", "").Trim();
            }
            catch
            {
                clone.name = clone.name.Replace("(Clone)", "").Trim();
                Debug.Log("No object found named 'Canvas', creating the object separately.");
            }
        }

        [MenuItem("GameObject/Modern UI Pack/Buttons/Circle Outline With Image/White", false, 0)]
        static void CIRCOWHI()
        {
            GameObject clone = Instantiate(Resources.Load<GameObject>("Buttons/Circle Outline With Image/White"), Vector3.zero, Quaternion.identity) as GameObject;

            try
            {
                clone.transform.SetParent(GameObject.Find("Canvas").transform, false);
                clone.name = clone.name.Replace("(Clone)", "").Trim();
            }
            catch
            {
                clone.name = clone.name.Replace("(Clone)", "").Trim();
                Debug.Log("No object found named 'Canvas', creating the object separately.");
            }
        }

        [MenuItem("GameObject/Modern UI Pack/Buttons/Circle With Image/Gray", false, 0)]
        static void CIRCGRA()
        {
            GameObject clone = Instantiate(Resources.Load<GameObject>("Buttons/Circle With Image/Gray"), Vector3.zero, Quaternion.identity) as GameObject;

            try
            {
                clone.transform.SetParent(GameObject.Find("Canvas").transform, false);
                clone.name = clone.name.Replace("(Clone)", "").Trim();
            }
            catch
            {
                clone.name = clone.name.Replace("(Clone)", "").Trim();
                Debug.Log("No object found named 'Canvas', creating the object separately.");
            }
        }

        [MenuItem("GameObject/Modern UI Pack/Buttons/Circle With Image/White", false, 0)]
        static void CIRCWHI()
        {
            GameObject clone = Instantiate(Resources.Load<GameObject>("Buttons/Circle With Image/White"), Vector3.zero, Quaternion.identity) as GameObject;

            try
            {
                clone.transform.SetParent(GameObject.Find("Canvas").transform, false);
                clone.name = clone.name.Replace("(Clone)", "").Trim();
            }
            catch
            {
                clone.name = clone.name.Replace("(Clone)", "").Trim();
                Debug.Log("No object found named 'Canvas', creating the object separately.");
            }
        }

        [MenuItem("GameObject/Modern UI Pack/Buttons/Rounded/Blue", false, 0)]
        static void ROBL()
        {
            GameObject clone = Instantiate(Resources.Load<GameObject>("Buttons/Rounded/Blue"), Vector3.zero, Quaternion.identity) as GameObject;

            try
            {
                clone.transform.SetParent(GameObject.Find("Canvas").transform, false);
                clone.name = clone.name.Replace("(Clone)", "").Trim();
            }
            catch
            {
                clone.name = clone.name.Replace("(Clone)", "").Trim();
                Debug.Log("No object found named 'Canvas', creating the object separately.");
            }
        }

        [MenuItem("GameObject/Modern UI Pack/Buttons/Rounded/Brown", false, 0)]
        static void ROBRW()
        {
            GameObject clone = Instantiate(Resources.Load<GameObject>("Buttons/Rounded/Brown"), Vector3.zero, Quaternion.identity) as GameObject;

            try
            {
                clone.transform.SetParent(GameObject.Find("Canvas").transform, false);
                clone.name = clone.name.Replace("(Clone)", "").Trim();
            }
            catch
            {
                clone.name = clone.name.Replace("(Clone)", "").Trim();
                Debug.Log("No object found named 'Canvas', creating the object separately.");
            }
        }

        [MenuItem("GameObject/Modern UI Pack/Buttons/Rounded/Gray", false, 0)]
        static void ROGR()
        {
            GameObject clone = Instantiate(Resources.Load<GameObject>("Buttons/Rounded/Gray"), Vector3.zero, Quaternion.identity) as GameObject;

            try
            {
                clone.transform.SetParent(GameObject.Find("Canvas").transform, false);
                clone.name = clone.name.Replace("(Clone)", "").Trim();
            }
            catch
            {
                clone.name = clone.name.Replace("(Clone)", "").Trim();
                Debug.Log("No object found named 'Canvas', creating the object separately.");
            }
        }

        [MenuItem("GameObject/Modern UI Pack/Buttons/Rounded/Green", false, 0)]
        static void ROGRE()
        {
            GameObject clone = Instantiate(Resources.Load<GameObject>("Buttons/Rounded/Green"), Vector3.zero, Quaternion.identity) as GameObject;

            try
            {
                clone.transform.SetParent(GameObject.Find("Canvas").transform, false);
                clone.name = clone.name.Replace("(Clone)", "").Trim();
            }
            catch
            {
                clone.name = clone.name.Replace("(Clone)", "").Trim();
                Debug.Log("No object found named 'Canvas', creating the object separately.");
            }
        }

        [MenuItem("GameObject/Modern UI Pack/Buttons/Rounded/Night", false, 0)]
        static void RONI()
        {
            GameObject clone = Instantiate(Resources.Load<GameObject>("Buttons/Rounded/Night"), Vector3.zero, Quaternion.identity) as GameObject;

            try
            {
                clone.transform.SetParent(GameObject.Find("Canvas").transform, false);
                clone.name = clone.name.Replace("(Clone)", "").Trim();
            }
            catch
            {
                clone.name = clone.name.Replace("(Clone)", "").Trim();
                Debug.Log("No object found named 'Canvas', creating the object separately.");
            }
        }

        [MenuItem("GameObject/Modern UI Pack/Buttons/Rounded/Orange", false, 0)]
        static void ROOR()
        {
            GameObject clone = Instantiate(Resources.Load<GameObject>("Buttons/Rounded/Orange"), Vector3.zero, Quaternion.identity) as GameObject;

            try
            {
                clone.transform.SetParent(GameObject.Find("Canvas").transform, false);
                clone.name = clone.name.Replace("(Clone)", "").Trim();
            }
            catch
            {
                clone.name = clone.name.Replace("(Clone)", "").Trim();
                Debug.Log("No object found named 'Canvas', creating the object separately.");
            }
        }

        [MenuItem("GameObject/Modern UI Pack/Buttons/Rounded/Pink", false, 0)]
        static void ROPIN()
        {
            GameObject clone = Instantiate(Resources.Load<GameObject>("Buttons/Rounded/Pink"), Vector3.zero, Quaternion.identity) as GameObject;

            try
            {
                clone.transform.SetParent(GameObject.Find("Canvas").transform, false);
                clone.name = clone.name.Replace("(Clone)", "").Trim();
            }
            catch
            {
                clone.name = clone.name.Replace("(Clone)", "").Trim();
                Debug.Log("No object found named 'Canvas', creating the object separately.");
            }
        }

        [MenuItem("GameObject/Modern UI Pack/Buttons/Rounded/Purple", false, 0)]
        static void ROPURP()
        {
            GameObject clone = Instantiate(Resources.Load<GameObject>("Buttons/Rounded/Purple"), Vector3.zero, Quaternion.identity) as GameObject;

            try
            {
                clone.transform.SetParent(GameObject.Find("Canvas").transform, false);
                clone.name = clone.name.Replace("(Clone)", "").Trim();
            }
            catch
            {
                clone.name = clone.name.Replace("(Clone)", "").Trim();
                Debug.Log("No object found named 'Canvas', creating the object separately.");
            }
        }

        [MenuItem("GameObject/Modern UI Pack/Buttons/Rounded/Red", false, 0)]
        static void RORED()
        {
            GameObject clone = Instantiate(Resources.Load<GameObject>("Buttons/Rounded/Red"), Vector3.zero, Quaternion.identity) as GameObject;

            try
            {
                clone.transform.SetParent(GameObject.Find("Canvas").transform, false);
                clone.name = clone.name.Replace("(Clone)", "").Trim();
            }
            catch
            {
                clone.name = clone.name.Replace("(Clone)", "").Trim();
                Debug.Log("No object found named 'Canvas', creating the object separately.");
            }
        }

        [MenuItem("GameObject/Modern UI Pack/Buttons/Rounded/White", false, 0)]
        static void ROWHI()
        {
            GameObject clone = Instantiate(Resources.Load<GameObject>("Buttons/Rounded/White"), Vector3.zero, Quaternion.identity) as GameObject;

            try
            {
                clone.transform.SetParent(GameObject.Find("Canvas").transform, false);
                clone.name = clone.name.Replace("(Clone)", "").Trim();
            }
            catch
            {
                clone.name = clone.name.Replace("(Clone)", "").Trim();
                Debug.Log("No object found named 'Canvas', creating the object separately.");
            }
        }

        [MenuItem("GameObject/Modern UI Pack/Buttons/Rounded Gradient/Blue", false, 0)]
        static void RGBL()
        {
            GameObject clone = Instantiate(Resources.Load<GameObject>("Buttons/Rounded Gradient/Blue"), Vector3.zero, Quaternion.identity) as GameObject;

            try
            {
                clone.transform.SetParent(GameObject.Find("Canvas").transform, false);
                clone.name = clone.name.Replace("(Clone)", "").Trim();
            }
            catch
            {
                clone.name = clone.name.Replace("(Clone)", "").Trim();
                Debug.Log("No object found named 'Canvas', creating the object separately.");
            }
        }

        [MenuItem("GameObject/Modern UI Pack/Buttons/Rounded Gradient/Brown", false, 0)]
        static void RGBRW()
        {
            GameObject clone = Instantiate(Resources.Load<GameObject>("Buttons/Rounded Gradient/Brown"), Vector3.zero, Quaternion.identity) as GameObject;

            try
            {
                clone.transform.SetParent(GameObject.Find("Canvas").transform, false);
                clone.name = clone.name.Replace("(Clone)", "").Trim();
            }
            catch
            {
                clone.name = clone.name.Replace("(Clone)", "").Trim();
                Debug.Log("No object found named 'Canvas', creating the object separately.");
            }
        }

        [MenuItem("GameObject/Modern UI Pack/Buttons/Rounded Gradient/Gray", false, 0)]
        static void RGGR()
        {
            GameObject clone = Instantiate(Resources.Load<GameObject>("Buttons/Rounded Gradient/Gray"), Vector3.zero, Quaternion.identity) as GameObject;

            try
            {
                clone.transform.SetParent(GameObject.Find("Canvas").transform, false);
                clone.name = clone.name.Replace("(Clone)", "").Trim();
            }
            catch
            {
                clone.name = clone.name.Replace("(Clone)", "").Trim();
                Debug.Log("No object found named 'Canvas', creating the object separately.");
            }
        }

        [MenuItem("GameObject/Modern UI Pack/Buttons/Rounded Gradient/Green", false, 0)]
        static void RGGRE()
        {
            GameObject clone = Instantiate(Resources.Load<GameObject>("Buttons/Rounded Gradient/Green"), Vector3.zero, Quaternion.identity) as GameObject;

            try
            {
                clone.transform.SetParent(GameObject.Find("Canvas").transform, false);
                clone.name = clone.name.Replace("(Clone)", "").Trim();
            }
            catch
            {
                clone.name = clone.name.Replace("(Clone)", "").Trim();
                Debug.Log("No object found named 'Canvas', creating the object separately.");
            }
        }

        [MenuItem("GameObject/Modern UI Pack/Buttons/Rounded Gradient/Night", false, 0)]
        static void RGNI()
        {
            GameObject clone = Instantiate(Resources.Load<GameObject>("Buttons/Rounded Gradient/Night"), Vector3.zero, Quaternion.identity) as GameObject;

            try
            {
                clone.transform.SetParent(GameObject.Find("Canvas").transform, false);
                clone.name = clone.name.Replace("(Clone)", "").Trim();
            }
            catch
            {
                clone.name = clone.name.Replace("(Clone)", "").Trim();
                Debug.Log("No object found named 'Canvas', creating the object separately.");
            }
        }

        [MenuItem("GameObject/Modern UI Pack/Buttons/Rounded Gradient/Orange", false, 0)]
        static void RGOR()
        {
            GameObject clone = Instantiate(Resources.Load<GameObject>("Buttons/Rounded Gradient/Orange"), Vector3.zero, Quaternion.identity) as GameObject;

            try
            {
                clone.transform.SetParent(GameObject.Find("Canvas").transform, false);
                clone.name = clone.name.Replace("(Clone)", "").Trim();
            }
            catch
            {
                clone.name = clone.name.Replace("(Clone)", "").Trim();
                Debug.Log("No object found named 'Canvas', creating the object separately.");
            }
        }

        [MenuItem("GameObject/Modern UI Pack/Buttons/Rounded Gradient/Pink", false, 0)]
        static void RGPIN()
        {
            GameObject clone = Instantiate(Resources.Load<GameObject>("Buttons/Rounded Gradient/Pink"), Vector3.zero, Quaternion.identity) as GameObject;

            try
            {
                clone.transform.SetParent(GameObject.Find("Canvas").transform, false);
                clone.name = clone.name.Replace("(Clone)", "").Trim();
            }
            catch
            {
                clone.name = clone.name.Replace("(Clone)", "").Trim();
                Debug.Log("No object found named 'Canvas', creating the object separately.");
            }
        }

        [MenuItem("GameObject/Modern UI Pack/Buttons/Rounded Gradient/Purple", false, 0)]
        static void RGPURP()
        {
            GameObject clone = Instantiate(Resources.Load<GameObject>("Buttons/Rounded Gradient/Purple"), Vector3.zero, Quaternion.identity) as GameObject;

            try
            {
                clone.transform.SetParent(GameObject.Find("Canvas").transform, false);
                clone.name = clone.name.Replace("(Clone)", "").Trim();
            }
            catch
            {
                clone.name = clone.name.Replace("(Clone)", "").Trim();
                Debug.Log("No object found named 'Canvas', creating the object separately.");
            }
        }

        [MenuItem("GameObject/Modern UI Pack/Buttons/Rounded Gradient/Red", false, 0)]
        static void RGRED()
        {
            GameObject clone = Instantiate(Resources.Load<GameObject>("Buttons/Rounded Gradient/Red"), Vector3.zero, Quaternion.identity) as GameObject;

            try
            {
                clone.transform.SetParent(GameObject.Find("Canvas").transform, false);
                clone.name = clone.name.Replace("(Clone)", "").Trim();
            }
            catch
            {
                clone.name = clone.name.Replace("(Clone)", "").Trim();
                Debug.Log("No object found named 'Canvas', creating the object separately.");
            }
        }

        [MenuItem("GameObject/Modern UI Pack/Buttons/Rounded Gradient/White", false, 0)]
        static void RGWHI()
        {
            GameObject clone = Instantiate(Resources.Load<GameObject>("Buttons/Rounded Gradient/White"), Vector3.zero, Quaternion.identity) as GameObject;

            try
            {
                clone.transform.SetParent(GameObject.Find("Canvas").transform, false);
                clone.name = clone.name.Replace("(Clone)", "").Trim();
            }
            catch
            {
                clone.name = clone.name.Replace("(Clone)", "").Trim();
                Debug.Log("No object found named 'Canvas', creating the object separately.");
            }
        }

        [MenuItem("GameObject/Modern UI Pack/Buttons/Rounded Outline/Blue", false, 0)]
        static void ROBLU()
        {
            GameObject clone = Instantiate(Resources.Load<GameObject>("Buttons/Rounded Outline/Blue"), Vector3.zero, Quaternion.identity) as GameObject;

            try
            {
                clone.transform.SetParent(GameObject.Find("Canvas").transform, false);
                clone.name = clone.name.Replace("(Clone)", "").Trim();
            }
            catch
            {
                clone.name = clone.name.Replace("(Clone)", "").Trim();
                Debug.Log("No object found named 'Canvas', creating the object separately.");
            }
        }

        [MenuItem("GameObject/Modern UI Pack/Buttons/Rounded Outline/Brown", false, 0)]
        static void RORW()
        {
            GameObject clone = Instantiate(Resources.Load<GameObject>("Buttons/Rounded Outline/Brown"), Vector3.zero, Quaternion.identity) as GameObject;

            try
            {
                clone.transform.SetParent(GameObject.Find("Canvas").transform, false);
                clone.name = clone.name.Replace("(Clone)", "").Trim();
            }
            catch
            {
                clone.name = clone.name.Replace("(Clone)", "").Trim();
                Debug.Log("No object found named 'Canvas', creating the object separately.");
            }
        }

        [MenuItem("GameObject/Modern UI Pack/Buttons/Rounded Outline/Gray", false, 0)]
        static void ROR()
        {
            GameObject clone = Instantiate(Resources.Load<GameObject>("Buttons/Rounded Outline/Gray"), Vector3.zero, Quaternion.identity) as GameObject;

            try
            {
                clone.transform.SetParent(GameObject.Find("Canvas").transform, false);
                clone.name = clone.name.Replace("(Clone)", "").Trim();
            }
            catch
            {
                clone.name = clone.name.Replace("(Clone)", "").Trim();
                Debug.Log("No object found named 'Canvas', creating the object separately.");
            }
        }

        [MenuItem("GameObject/Modern UI Pack/Buttons/Rounded Outline/Green", false, 0)]
        static void RORE()
        {
            GameObject clone = Instantiate(Resources.Load<GameObject>("Buttons/Rounded Outline/Green"), Vector3.zero, Quaternion.identity) as GameObject;

            try
            {
                clone.transform.SetParent(GameObject.Find("Canvas").transform, false);
                clone.name = clone.name.Replace("(Clone)", "").Trim();
            }
            catch
            {
                clone.name = clone.name.Replace("(Clone)", "").Trim();
                Debug.Log("No object found named 'Canvas', creating the object separately.");
            }
        }

        [MenuItem("GameObject/Modern UI Pack/Buttons/Rounded Outline/Night", false, 0)]
        static void RONIG()
        {
            GameObject clone = Instantiate(Resources.Load<GameObject>("Buttons/Rounded Outline/Night"), Vector3.zero, Quaternion.identity) as GameObject;

            try
            {
                clone.transform.SetParent(GameObject.Find("Canvas").transform, false);
                clone.name = clone.name.Replace("(Clone)", "").Trim();
            }
            catch
            {
                clone.name = clone.name.Replace("(Clone)", "").Trim();
                Debug.Log("No object found named 'Canvas', creating the object separately.");
            }
        }

        [MenuItem("GameObject/Modern UI Pack/Buttons/Rounded Outline/Orange", false, 0)]
        static void ROORA()
        {
            GameObject clone = Instantiate(Resources.Load<GameObject>("Buttons/Rounded Outline/Orange"), Vector3.zero, Quaternion.identity) as GameObject;

            try
            {
                clone.transform.SetParent(GameObject.Find("Canvas").transform, false);
                clone.name = clone.name.Replace("(Clone)", "").Trim();
            }
            catch
            {
                clone.name = clone.name.Replace("(Clone)", "").Trim();
                Debug.Log("No object found named 'Canvas', creating the object separately.");
            }
        }

        [MenuItem("GameObject/Modern UI Pack/Buttons/Rounded Outline/Pink", false, 0)]
        static void ROPINK()
        {
            GameObject clone = Instantiate(Resources.Load<GameObject>("Buttons/Rounded Outline/Pink"), Vector3.zero, Quaternion.identity) as GameObject;

            try
            {
                clone.transform.SetParent(GameObject.Find("Canvas").transform, false);
                clone.name = clone.name.Replace("(Clone)", "").Trim();
            }
            catch
            {
                clone.name = clone.name.Replace("(Clone)", "").Trim();
                Debug.Log("No object found named 'Canvas', creating the object separately.");
            }
        }

        [MenuItem("GameObject/Modern UI Pack/Buttons/Rounded Outline/Purple", false, 0)]
        static void ROPURPL()
        {
            GameObject clone = Instantiate(Resources.Load<GameObject>("Buttons/Rounded Outline/Purple"), Vector3.zero, Quaternion.identity) as GameObject;

            try
            {
                clone.transform.SetParent(GameObject.Find("Canvas").transform, false);
                clone.name = clone.name.Replace("(Clone)", "").Trim();
            }
            catch
            {
                clone.name = clone.name.Replace("(Clone)", "").Trim();
                Debug.Log("No object found named 'Canvas', creating the object separately.");
            }
        }

        [MenuItem("GameObject/Modern UI Pack/Buttons/Rounded Outline/Red", false, 0)]
        static void ROREDD()
        {
            GameObject clone = Instantiate(Resources.Load<GameObject>("Buttons/Rounded Outline/Red"), Vector3.zero, Quaternion.identity) as GameObject;

            try
            {
                clone.transform.SetParent(GameObject.Find("Canvas").transform, false);
                clone.name = clone.name.Replace("(Clone)", "").Trim();
            }
            catch
            {
                clone.name = clone.name.Replace("(Clone)", "").Trim();
                Debug.Log("No object found named 'Canvas', creating the object separately.");
            }
        }

        [MenuItem("GameObject/Modern UI Pack/Buttons/Rounded Outline/White", false, 0)]
        static void ROWHIT()
        {
            GameObject clone = Instantiate(Resources.Load<GameObject>("Buttons/Rounded Outline/White"), Vector3.zero, Quaternion.identity) as GameObject;

            try
            {
                clone.transform.SetParent(GameObject.Find("Canvas").transform, false);
                clone.name = clone.name.Replace("(Clone)", "").Trim();
            }
            catch
            {
                clone.name = clone.name.Replace("(Clone)", "").Trim();
                Debug.Log("No object found named 'Canvas', creating the object separately.");
            }
        }

        [MenuItem("GameObject/Modern UI Pack/Buttons/Rounded Outline Gradient/Blue", false, 0)]
        static void ROGBL()
        {
            GameObject clone = Instantiate(Resources.Load<GameObject>("Buttons/Rounded Outline Gradient/Blue"), Vector3.zero, Quaternion.identity) as GameObject;

            try
            {
                clone.transform.SetParent(GameObject.Find("Canvas").transform, false);
                clone.name = clone.name.Replace("(Clone)", "").Trim();
            }
            catch
            {
                clone.name = clone.name.Replace("(Clone)", "").Trim();
                Debug.Log("No object found named 'Canvas', creating the object separately.");
            }
        }

        [MenuItem("GameObject/Modern UI Pack/Buttons/Rounded Outline Gradient/Brown", false, 0)]
        static void ROGRW()
        {
            GameObject clone = Instantiate(Resources.Load<GameObject>("Buttons/Rounded Outline Gradient/Brown"), Vector3.zero, Quaternion.identity) as GameObject;

            try
            {
                clone.transform.SetParent(GameObject.Find("Canvas").transform, false);
                clone.name = clone.name.Replace("(Clone)", "").Trim();
            }
            catch
            {
                clone.name = clone.name.Replace("(Clone)", "").Trim();
                Debug.Log("No object found named 'Canvas', creating the object separately.");
            }
        }

        [MenuItem("GameObject/Modern UI Pack/Buttons/Rounded Outline Gradient/Gray", false, 0)]
        static void ROGRAY()
        {
            GameObject clone = Instantiate(Resources.Load<GameObject>("Buttons/Rounded Outline Gradient/Gray"), Vector3.zero, Quaternion.identity) as GameObject;

            try
            {
                clone.transform.SetParent(GameObject.Find("Canvas").transform, false);
                clone.name = clone.name.Replace("(Clone)", "").Trim();
            }
            catch
            {
                clone.name = clone.name.Replace("(Clone)", "").Trim();
                Debug.Log("No object found named 'Canvas', creating the object separately.");
            }
        }

        [MenuItem("GameObject/Modern UI Pack/Buttons/Rounded Outline Gradient/Green", false, 0)]
        static void ROGREE()
        {
            GameObject clone = Instantiate(Resources.Load<GameObject>("Buttons/Rounded Outline Gradient/Green"), Vector3.zero, Quaternion.identity) as GameObject;

            try
            {
                clone.transform.SetParent(GameObject.Find("Canvas").transform, false);
                clone.name = clone.name.Replace("(Clone)", "").Trim();
            }
            catch
            {
                clone.name = clone.name.Replace("(Clone)", "").Trim();
                Debug.Log("No object found named 'Canvas', creating the object separately.");
            }
        }

        [MenuItem("GameObject/Modern UI Pack/Buttons/Rounded Outline Gradient/Night", false, 0)]
        static void ROGNI()
        {
            GameObject clone = Instantiate(Resources.Load<GameObject>("Buttons/Rounded Outline Gradient/Night"), Vector3.zero, Quaternion.identity) as GameObject;

            try
            {
                clone.transform.SetParent(GameObject.Find("Canvas").transform, false);
                clone.name = clone.name.Replace("(Clone)", "").Trim();
            }
            catch
            {
                clone.name = clone.name.Replace("(Clone)", "").Trim();
                Debug.Log("No object found named 'Canvas', creating the object separately.");
            }
        }

        [MenuItem("GameObject/Modern UI Pack/Buttons/Rounded Outline Gradient/Orange", false, 0)]
        static void ROGOR()
        {
            GameObject clone = Instantiate(Resources.Load<GameObject>("Buttons/Rounded Outline Gradient/Orange"), Vector3.zero, Quaternion.identity) as GameObject;

            try
            {
                clone.transform.SetParent(GameObject.Find("Canvas").transform, false);
                clone.name = clone.name.Replace("(Clone)", "").Trim();
            }
            catch
            {
                clone.name = clone.name.Replace("(Clone)", "").Trim();
                Debug.Log("No object found named 'Canvas', creating the object separately.");
            }
        }

        [MenuItem("GameObject/Modern UI Pack/Buttons/Rounded Outline Gradient/Pink", false, 0)]
        static void ROGPIN()
        {
            GameObject clone = Instantiate(Resources.Load<GameObject>("Buttons/Rounded Outline Gradient/Pink"), Vector3.zero, Quaternion.identity) as GameObject;

            try
            {
                clone.transform.SetParent(GameObject.Find("Canvas").transform, false);
                clone.name = clone.name.Replace("(Clone)", "").Trim();
            }
            catch
            {
                clone.name = clone.name.Replace("(Clone)", "").Trim();
                Debug.Log("No object found named 'Canvas', creating the object separately.");
            }
        }

        [MenuItem("GameObject/Modern UI Pack/Buttons/Rounded Outline Gradient/Purple", false, 0)]
        static void ROGPURP()
        {
            GameObject clone = Instantiate(Resources.Load<GameObject>("Buttons/Rounded Outline Gradient/Purple"), Vector3.zero, Quaternion.identity) as GameObject;

            try
            {
                clone.transform.SetParent(GameObject.Find("Canvas").transform, false);
                clone.name = clone.name.Replace("(Clone)", "").Trim();
            }
            catch
            {
                clone.name = clone.name.Replace("(Clone)", "").Trim();
                Debug.Log("No object found named 'Canvas', creating the object separately.");
            }
        }

        [MenuItem("GameObject/Modern UI Pack/Buttons/Rounded Outline Gradient/Red", false, 0)]
        static void ROGRED()
        {
            GameObject clone = Instantiate(Resources.Load<GameObject>("Buttons/Rounded Outline Gradient/Red"), Vector3.zero, Quaternion.identity) as GameObject;

            try
            {
                clone.transform.SetParent(GameObject.Find("Canvas").transform, false);
                clone.name = clone.name.Replace("(Clone)", "").Trim();
            }
            catch
            {
                clone.name = clone.name.Replace("(Clone)", "").Trim();
                Debug.Log("No object found named 'Canvas', creating the object separately.");
            }
        }

        [MenuItem("GameObject/Modern UI Pack/Buttons/Rounded Outline Gradient/White", false, 0)]
        static void ROGWHI()
        {
            GameObject clone = Instantiate(Resources.Load<GameObject>("Buttons/Rounded Outline Gradient/White"), Vector3.zero, Quaternion.identity) as GameObject;

            try
            {
                clone.transform.SetParent(GameObject.Find("Canvas").transform, false);
                clone.name = clone.name.Replace("(Clone)", "").Trim();
            }
            catch
            {
                clone.name = clone.name.Replace("(Clone)", "").Trim();
                Debug.Log("No object found named 'Canvas', creating the object separately.");
            }
        }

        [MenuItem("GameObject/Modern UI Pack/Buttons/Rounded Outline With Image/Gray", false, 0)]
        static void ROWIGRA()
        {
            GameObject clone = Instantiate(Resources.Load<GameObject>("Buttons/Rounded Outline Gradient/Gray"), Vector3.zero, Quaternion.identity) as GameObject;

            try
            {
                clone.transform.SetParent(GameObject.Find("Canvas").transform, false);
                clone.name = clone.name.Replace("(Clone)", "").Trim();
            }
            catch
            {
                clone.name = clone.name.Replace("(Clone)", "").Trim();
                Debug.Log("No object found named 'Canvas', creating the object separately.");
            }
        }

        [MenuItem("GameObject/Modern UI Pack/Buttons/Rounded Outline With Image/White", false, 0)]
        static void ROWIWHIT()
        {
            GameObject clone = Instantiate(Resources.Load<GameObject>("Buttons/Rounded Outline Gradient/White"), Vector3.zero, Quaternion.identity) as GameObject;

            try
            {
                clone.transform.SetParent(GameObject.Find("Canvas").transform, false);
                clone.name = clone.name.Replace("(Clone)", "").Trim();
            }
            catch
            {
                clone.name = clone.name.Replace("(Clone)", "").Trim();
                Debug.Log("No object found named 'Canvas', creating the object separately.");
            }
        }

        [MenuItem("GameObject/Modern UI Pack/Buttons/Rounded With Image/Gray", false, 0)]
        static void RWIGRAY()
        {
            GameObject clone = Instantiate(Resources.Load<GameObject>("Buttons/Rounded With Image/Gray"), Vector3.zero, Quaternion.identity) as GameObject;

            try
            {
                clone.transform.SetParent(GameObject.Find("Canvas").transform, false);
                clone.name = clone.name.Replace("(Clone)", "").Trim();
            }
            catch
            {
                clone.name = clone.name.Replace("(Clone)", "").Trim();
                Debug.Log("No object found named 'Canvas', creating the object separately.");
            }
        }

        [MenuItem("GameObject/Modern UI Pack/Buttons/Rounded With Image/White", false, 0)]
        static void RWIWHIT()
        {
            GameObject clone = Instantiate(Resources.Load<GameObject>("Buttons/Rounded With Image/White"), Vector3.zero, Quaternion.identity) as GameObject;

            try
            {
                clone.transform.SetParent(GameObject.Find("Canvas").transform, false);
                clone.name = clone.name.Replace("(Clone)", "").Trim();
            }
            catch
            {
                clone.name = clone.name.Replace("(Clone)", "").Trim();
                Debug.Log("No object found named 'Canvas', creating the object separately.");
            }
        }

        [MenuItem("GameObject/Modern UI Pack/Dropdowns/Multi Select", false, 0)]
        static void DRPMD()
        {
            GameObject clone = Instantiate(Resources.Load<GameObject>("Dropdowns/Multi Select Dropdown"), Vector3.zero, Quaternion.identity) as GameObject;

            try
            {
                clone.transform.SetParent(GameObject.Find("Canvas").transform, false);
                clone.name = clone.name.Replace("(Clone)", "").Trim();
            }
            catch
            {
                clone.name = clone.name.Replace("(Clone)", "").Trim();
                Debug.Log("No object found named 'Canvas', creating the object separately.");
            }
        }

        [MenuItem("GameObject/Modern UI Pack/Dropdowns/Standard", false, 0)]
        static void DRPSD()
        {
            GameObject clone = Instantiate(Resources.Load<GameObject>("Dropdowns/Standard Dropdown"), Vector3.zero, Quaternion.identity) as GameObject;

            try
            {
                clone.transform.SetParent(GameObject.Find("Canvas").transform, false);
                clone.name = clone.name.Replace("(Clone)", "").Trim();
            }
            catch
            {
                clone.name = clone.name.Replace("(Clone)", "").Trim();
                Debug.Log("No object found named 'Canvas', creating the object separately.");
            }
        }

        [MenuItem("GameObject/Modern UI Pack/Hamburger Menus/Standard", false, 0)]
        static void HAMLEFT()
        {
            GameObject clone = Instantiate(Resources.Load<GameObject>("Hamburger Menus/Standard"), Vector3.zero, Quaternion.identity) as GameObject;

            try
            {
                clone.transform.SetParent(GameObject.Find("Canvas").transform, false);
                clone.name = clone.name.Replace("(Clone)", "").Trim();
            }
            catch
            {
                clone.name = clone.name.Replace("(Clone)", "").Trim();
                Debug.Log("No object found named 'Canvas', creating the object separately.");
            }
        }

        [MenuItem("GameObject/Modern UI Pack/Horizontal Selector/Standard", false, 0)]
        static void HRZSL()
        {
            GameObject clone = Instantiate(Resources.Load<GameObject>("Horizontal Selector/Standard"), Vector3.zero, Quaternion.identity) as GameObject;

            try
            {
                clone.transform.SetParent(GameObject.Find("Canvas").transform, false);
                clone.name = clone.name.Replace("(Clone)", "").Trim();
            }
            catch
            {
                clone.name = clone.name.Replace("(Clone)", "").Trim();
                Debug.Log("No object found named 'Canvas', creating the object separately.");
            }
        }

        [MenuItem("GameObject/Modern UI Pack/Input Fields/Fading Input Field (Left Aligned)", false, 0)]
        static void IFFIFL()
        {
            GameObject clone = Instantiate(Resources.Load<GameObject>("Input Fields/Fading Input Field (Left Aligned)"), Vector3.zero, Quaternion.identity) as GameObject;

            try
            {
                clone.transform.SetParent(GameObject.Find("Canvas").transform, false);
                clone.name = clone.name.Replace("(Clone)", "").Trim();
            }
            catch
            {
                clone.name = clone.name.Replace("(Clone)", "").Trim();
                Debug.Log("No object found named 'Canvas', creating the object separately.");
            }
        }

        [MenuItem("GameObject/Modern UI Pack/Input Fields/Fading Input Field (Middle Aligned)", false, 0)]
        static void IFFIFM()
        {
            GameObject clone = Instantiate(Resources.Load<GameObject>("Input Fields/Fading Input Field (Middle Aligned)"), Vector3.zero, Quaternion.identity) as GameObject;

            try
            {
                clone.transform.SetParent(GameObject.Find("Canvas").transform, false);
                clone.name = clone.name.Replace("(Clone)", "").Trim();
            }
            catch
            {
                clone.name = clone.name.Replace("(Clone)", "").Trim();
                Debug.Log("No object found named 'Canvas', creating the object separately.");
            }
        }

        [MenuItem("GameObject/Modern UI Pack/Input Fields/Fading Input Field (Right Aligned)", false, 0)]
        static void IFFIFR()
        {
            GameObject clone = Instantiate(Resources.Load<GameObject>("Input Fields/Fading Input Field (Right Aligned)"), Vector3.zero, Quaternion.identity) as GameObject;

            try
            {
                clone.transform.SetParent(GameObject.Find("Canvas").transform, false);
                clone.name = clone.name.Replace("(Clone)", "").Trim();
            }
            catch
            {
                clone.name = clone.name.Replace("(Clone)", "").Trim();
                Debug.Log("No object found named 'Canvas', creating the object separately.");
            }
        }

        [MenuItem("GameObject/Modern UI Pack/Input Fields/Standard Input Field (Left Aligned)", false, 0)]
        static void IFSIFL()
        {
            GameObject clone = Instantiate(Resources.Load<GameObject>("Input Fields/Standard Input Field (Left Aligned)"), Vector3.zero, Quaternion.identity) as GameObject;

            try
            {
                clone.transform.SetParent(GameObject.Find("Canvas").transform, false);
                clone.name = clone.name.Replace("(Clone)", "").Trim();
            }
            catch
            {
                clone.name = clone.name.Replace("(Clone)", "").Trim();
                Debug.Log("No object found named 'Canvas', creating the object separately.");
            }
        }

        [MenuItem("GameObject/Modern UI Pack/Input Fields/Standard Input Field (Middle Aligned)", false, 0)]
        static void IFSIFM()
        {
            GameObject clone = Instantiate(Resources.Load<GameObject>("Input Fields/Standard Input Field (Middle Aligned)"), Vector3.zero, Quaternion.identity) as GameObject;

            try
            {
                clone.transform.SetParent(GameObject.Find("Canvas").transform, false);
                clone.name = clone.name.Replace("(Clone)", "").Trim();
            }
            catch
            {
                clone.name = clone.name.Replace("(Clone)", "").Trim();
                Debug.Log("No object found named 'Canvas', creating the object separately.");
            }
        }

        [MenuItem("GameObject/Modern UI Pack/Input Fields/Standard Input Field (Right Aligned)", false, 0)]
        static void IFSIFR()
        {
            GameObject clone = Instantiate(Resources.Load<GameObject>("Input Fields/Standard Input Field (Right Aligned)"), Vector3.zero, Quaternion.identity) as GameObject;

            try
            {
                clone.transform.SetParent(GameObject.Find("Canvas").transform, false);
                clone.name = clone.name.Replace("(Clone)", "").Trim();
            }
            catch
            {
                clone.name = clone.name.Replace("(Clone)", "").Trim();
                Debug.Log("No object found named 'Canvas', creating the object separately.");
            }
        }

        [MenuItem("GameObject/Modern UI Pack/Modal Windows/Style 1/Auto-Resizing", false, 0)]
        static void MWSOAR()
        {
            GameObject clone = Instantiate(Resources.Load<GameObject>("Modal Windows/Style 1/Auto-Resizing"), Vector3.zero, Quaternion.identity) as GameObject;

            try
            {
                clone.transform.SetParent(GameObject.Find("Canvas").transform, false);
                clone.name = clone.name.Replace("(Clone)", "").Trim();
            }
            catch
            {
                clone.name = clone.name.Replace("(Clone)", "").Trim();
                Debug.Log("No object found named 'Canvas', creating the object separately.");
            }
        }

        [MenuItem("GameObject/Modern UI Pack/Modal Windows/Style 1/Standard", false, 0)]
        static void MWSOS()
        {
            GameObject clone = Instantiate(Resources.Load<GameObject>("Modal Windows/Style 1/Standard"), Vector3.zero, Quaternion.identity) as GameObject;

            try
            {
                clone.transform.SetParent(GameObject.Find("Canvas").transform, false);
                clone.name = clone.name.Replace("(Clone)", "").Trim();
            }
            catch
            {
                clone.name = clone.name.Replace("(Clone)", "").Trim();
                Debug.Log("No object found named 'Canvas', creating the object separately.");
            }
        }

        [MenuItem("GameObject/Modern UI Pack/Modal Windows/Style 1/With Buttons", false, 0)]
        static void MWSOWB()
        {
            GameObject clone = Instantiate(Resources.Load<GameObject>("Modal Windows/Style 1/With Buttons"), Vector3.zero, Quaternion.identity) as GameObject;

            try
            {
                clone.transform.SetParent(GameObject.Find("Canvas").transform, false);
                clone.name = clone.name.Replace("(Clone)", "").Trim();
            }
            catch
            {
                clone.name = clone.name.Replace("(Clone)", "").Trim();
                Debug.Log("No object found named 'Canvas', creating the object separately.");
            }
        }

        [MenuItem("GameObject/Modern UI Pack/Modal Windows/Style 1/With Tabs", false, 0)]
        static void MWSOWT()
        {
            GameObject clone = Instantiate(Resources.Load<GameObject>("Modal Windows/Style 1/With Tabs"), Vector3.zero, Quaternion.identity) as GameObject;

            try
            {
                clone.transform.SetParent(GameObject.Find("Canvas").transform, false);
                clone.name = clone.name.Replace("(Clone)", "").Trim();
            }
            catch
            {
                clone.name = clone.name.Replace("(Clone)", "").Trim();
                Debug.Log("No object found named 'Canvas', creating the object separately.");
            }
        }

        [MenuItem("GameObject/Modern UI Pack/Modal Windows/Style 2/Standard", false, 0)]
        static void MWSTS()
        {
            GameObject clone = Instantiate(Resources.Load<GameObject>("Modal Windows/Style 2/Standard"), Vector3.zero, Quaternion.identity) as GameObject;

            try
            {
                clone.transform.SetParent(GameObject.Find("Canvas").transform, false);
                clone.name = clone.name.Replace("(Clone)", "").Trim();
            }
            catch
            {
                clone.name = clone.name.Replace("(Clone)", "").Trim();
                Debug.Log("No object found named 'Canvas', creating the object separately.");
            }
        }

        [MenuItem("GameObject/Modern UI Pack/Modal Windows/Style 2/With Tabs", false, 0)]
        static void MWSTWT()
        {
            GameObject clone = Instantiate(Resources.Load<GameObject>("Modal Windows/Style 2/With Tabs"), Vector3.zero, Quaternion.identity) as GameObject;

            try
            {
                clone.transform.SetParent(GameObject.Find("Canvas").transform, false);
                clone.name = clone.name.Replace("(Clone)", "").Trim();
            }
            catch
            {
                clone.name = clone.name.Replace("(Clone)", "").Trim();
                Debug.Log("No object found named 'Canvas', creating the object separately.");
            }
        }

        [MenuItem("GameObject/Modern UI Pack/Notifications/Fading Notification", false, 0)]
        static void NFN()
        {
            GameObject clone = Instantiate(Resources.Load<GameObject>("Notifications/Fading Notification"), Vector3.zero, Quaternion.identity) as GameObject;

            try
            {
                clone.transform.SetParent(GameObject.Find("Canvas").transform, false);
                clone.name = clone.name.Replace("(Clone)", "").Trim();
            }
            catch
            {
                clone.name = clone.name.Replace("(Clone)", "").Trim();
                Debug.Log("No object found named 'Canvas', creating the object separately.");
            }
        }

        [MenuItem("GameObject/Modern UI Pack/Notifications/Icon Popup Bottom Left", false, 0)]
        static void NIPBL()
        {
            GameObject clone = Instantiate(Resources.Load<GameObject>("Notifications/Fading Icon Popup Bottom Left"), Vector3.zero, Quaternion.identity) as GameObject;

            try
            {
                clone.transform.SetParent(GameObject.Find("Canvas").transform, false);
                clone.name = clone.name.Replace("(Clone)", "").Trim();
            }
            catch
            {
                clone.name = clone.name.Replace("(Clone)", "").Trim();
                Debug.Log("No object found named 'Canvas', creating the object separately.");
            }
        }

        [MenuItem("GameObject/Modern UI Pack/Notifications/Icon Popup Bottom Right", false, 0)]
        static void NIPBR()
        {
            GameObject clone = Instantiate(Resources.Load<GameObject>("Notifications/Icon Popup Bottom Right"), Vector3.zero, Quaternion.identity) as GameObject;

            try
            {
                clone.transform.SetParent(GameObject.Find("Canvas").transform, false);
                clone.name = clone.name.Replace("(Clone)", "").Trim();
            }
            catch
            {
                clone.name = clone.name.Replace("(Clone)", "").Trim();
                Debug.Log("No object found named 'Canvas', creating the object separately.");
            }
        }

        [MenuItem("GameObject/Modern UI Pack/Notifications/Icon Popup Top Left", false, 0)]
        static void NIPTL()
        {
            GameObject clone = Instantiate(Resources.Load<GameObject>("Notifications/Icon Popup Top Left"), Vector3.zero, Quaternion.identity) as GameObject;

            try
            {
                clone.transform.SetParent(GameObject.Find("Canvas").transform, false);
                clone.name = clone.name.Replace("(Clone)", "").Trim();
            }
            catch
            {
                clone.name = clone.name.Replace("(Clone)", "").Trim();
                Debug.Log("No object found named 'Canvas', creating the object separately.");
            }
        }

        [MenuItem("GameObject/Modern UI Pack/Notifications/Icon Popup Top Right", false, 0)]
        static void NIPTR()
        {
            GameObject clone = Instantiate(Resources.Load<GameObject>("Notifications/Icon Popup Top Right"), Vector3.zero, Quaternion.identity) as GameObject;

            try
            {
                clone.transform.SetParent(GameObject.Find("Canvas").transform, false);
                clone.name = clone.name.Replace("(Clone)", "").Trim();
            }
            catch
            {
                clone.name = clone.name.Replace("(Clone)", "").Trim();
                Debug.Log("No object found named 'Canvas', creating the object separately.");
            }
        }

        [MenuItem("GameObject/Modern UI Pack/Notifications/Popup Notification", false, 0)]
        static void NPN()
        {
            GameObject clone = Instantiate(Resources.Load<GameObject>("Notifications/Popup Notification"), Vector3.zero, Quaternion.identity) as GameObject;

            try
            {
                clone.transform.SetParent(GameObject.Find("Canvas").transform, false);
                clone.name = clone.name.Replace("(Clone)", "").Trim();
            }
            catch
            {
                clone.name = clone.name.Replace("(Clone)", "").Trim();
                Debug.Log("No object found named 'Canvas', creating the object separately.");
            }
        }

        [MenuItem("GameObject/Modern UI Pack/Notifications/Slippery Notification", false, 0)]
        static void NSN()
        {
            GameObject clone = Instantiate(Resources.Load<GameObject>("Notifications/Slippery Notification"), Vector3.zero, Quaternion.identity) as GameObject;

            try
            {
                clone.transform.SetParent(GameObject.Find("Canvas").transform, false);
                clone.name = clone.name.Replace("(Clone)", "").Trim();
            }
            catch
            {
                clone.name = clone.name.Replace("(Clone)", "").Trim();
                Debug.Log("No object found named 'Canvas', creating the object separately.");
            }
        }

        [MenuItem("GameObject/Modern UI Pack/Notifications/Slipping Notification", false, 0)]
        static void NSLN()
        {
            GameObject clone = Instantiate(Resources.Load<GameObject>("Notifications/Slipping Notification"), Vector3.zero, Quaternion.identity) as GameObject;

            try
            {
                clone.transform.SetParent(GameObject.Find("Canvas").transform, false);
                clone.name = clone.name.Replace("(Clone)", "").Trim();
            }
            catch
            {
                clone.name = clone.name.Replace("(Clone)", "").Trim();
                Debug.Log("No object found named 'Canvas', creating the object separately.");
            }
        }

        [MenuItem("GameObject/Modern UI Pack/Progress Bars/Radial PB Bold", false, 0)]
        static void PBRPB()
        {
            GameObject clone = Instantiate(Resources.Load<GameObject>("Progress Bars/Radial PB Bold"), Vector3.zero, Quaternion.identity) as GameObject;

            try
            {
                clone.transform.SetParent(GameObject.Find("Canvas").transform, false);
                clone.name = clone.name.Replace("(Clone)", "").Trim();
            }
            catch
            {
                clone.name = clone.name.Replace("(Clone)", "").Trim();
                Debug.Log("No object found named 'Canvas', creating the object separately.");
            }
        }

        [MenuItem("GameObject/Modern UI Pack/Progress Bars/Radial PB Filled H", false, 0)]
        static void PBRPF()
        {
            GameObject clone = Instantiate(Resources.Load<GameObject>("Progress Bars/Radial PB Filled H"), Vector3.zero, Quaternion.identity) as GameObject;

            try
            {
                clone.transform.SetParent(GameObject.Find("Canvas").transform, false);
                clone.name = clone.name.Replace("(Clone)", "").Trim();
            }
            catch
            {
                clone.name = clone.name.Replace("(Clone)", "").Trim();
                Debug.Log("No object found named 'Canvas', creating the object separately.");
            }
        }

        [MenuItem("GameObject/Modern UI Pack/Progress Bars/Radial PB Filled V", false, 0)]
        static void PBRPV()
        {
            GameObject clone = Instantiate(Resources.Load<GameObject>("Progress Bars/Radial PB Filled V"), Vector3.zero, Quaternion.identity) as GameObject;

            try
            {
                clone.transform.SetParent(GameObject.Find("Canvas").transform, false);
                clone.name = clone.name.Replace("(Clone)", "").Trim();
            }
            catch
            {
                clone.name = clone.name.Replace("(Clone)", "").Trim();
                Debug.Log("No object found named 'Canvas', creating the object separately.");
            }
        }

        [MenuItem("GameObject/Modern UI Pack/Progress Bars/Radial PB Light", false, 0)]
        static void PBRPLI()
        {
            GameObject clone = Instantiate(Resources.Load<GameObject>("Progress Bars/Radial PB Light"), Vector3.zero, Quaternion.identity) as GameObject;

            try
            {
                clone.transform.SetParent(GameObject.Find("Canvas").transform, false);
                clone.name = clone.name.Replace("(Clone)", "").Trim();
            }
            catch
            {
                clone.name = clone.name.Replace("(Clone)", "").Trim();
                Debug.Log("No object found named 'Canvas', creating the object separately.");
            }
        }

        [MenuItem("GameObject/Modern UI Pack/Progress Bars/Radial PB Regular", false, 0)]
        static void PBRPREG()
        {
            GameObject clone = Instantiate(Resources.Load<GameObject>("Progress Bars/Radial PB Regular"), Vector3.zero, Quaternion.identity) as GameObject;

            try
            {
                clone.transform.SetParent(GameObject.Find("Canvas").transform, false);
                clone.name = clone.name.Replace("(Clone)", "").Trim();
            }
            catch
            {
                clone.name = clone.name.Replace("(Clone)", "").Trim();
                Debug.Log("No object found named 'Canvas', creating the object separately.");
            }
        }

        [MenuItem("GameObject/Modern UI Pack/Progress Bars/Radial PB Thin", false, 0)]
        static void PBRPTHI()
        {
            GameObject clone = Instantiate(Resources.Load<GameObject>("Progress Bars/Radial PB Thin"), Vector3.zero, Quaternion.identity) as GameObject;

            try
            {
                clone.transform.SetParent(GameObject.Find("Canvas").transform, false);
                clone.name = clone.name.Replace("(Clone)", "").Trim();
            }
            catch
            {
                clone.name = clone.name.Replace("(Clone)", "").Trim();
                Debug.Log("No object found named 'Canvas', creating the object separately.");
            }
        }

        [MenuItem("GameObject/Modern UI Pack/Progress Bars/Standard PB", false, 0)]
        static void PBSPB()
        {
            GameObject clone = Instantiate(Resources.Load<GameObject>("Progress Bars/Standard PB"), Vector3.zero, Quaternion.identity) as GameObject;

            try
            {
                clone.transform.SetParent(GameObject.Find("Canvas").transform, false);
                clone.name = clone.name.Replace("(Clone)", "").Trim();
            }
            catch
            {
                clone.name = clone.name.Replace("(Clone)", "").Trim();
                Debug.Log("No object found named 'Canvas', creating the object separately.");
            }
        }     

        [MenuItem("GameObject/Modern UI Pack/Progress Bars (Loop)/Circle Glass", false, 0)]
        static void PBLCG()
        {
            GameObject clone = Instantiate(Resources.Load<GameObject>("Progress Bars (Loop)/Circle Glass"), Vector3.zero, Quaternion.identity) as GameObject;

            try
            {
                clone.transform.SetParent(GameObject.Find("Canvas").transform, false);
                clone.name = clone.name.Replace("(Clone)", "").Trim();
            }
            catch
            {
                clone.name = clone.name.Replace("(Clone)", "").Trim();
                Debug.Log("No object found named 'Canvas', creating the object separately.");
            }
        }

        [MenuItem("GameObject/Modern UI Pack/Progress Bars (Loop)/Circle Pie", false, 0)]
        static void PBLPIE()
        {
            GameObject clone = Instantiate(Resources.Load<GameObject>("Progress Bars (Loop)/Circle Pie"), Vector3.zero, Quaternion.identity) as GameObject;

            try
            {
                clone.transform.SetParent(GameObject.Find("Canvas").transform, false);
                clone.name = clone.name.Replace("(Clone)", "").Trim();
            }
            catch
            {
                clone.name = clone.name.Replace("(Clone)", "").Trim();
                Debug.Log("No object found named 'Canvas', creating the object separately.");
            }
        }

        [MenuItem("GameObject/Modern UI Pack/Progress Bars (Loop)/Circle Run", false, 0)]
        static void PBLRUN()
        {
            GameObject clone = Instantiate(Resources.Load<GameObject>("Progress Bars (Loop)/Circle Run"), Vector3.zero, Quaternion.identity) as GameObject;

            try
            {
                clone.transform.SetParent(GameObject.Find("Canvas").transform, false);
                clone.name = clone.name.Replace("(Clone)", "").Trim();
            }
            catch
            {
                clone.name = clone.name.Replace("(Clone)", "").Trim();
                Debug.Log("No object found named 'Canvas', creating the object separately.");
            }
        }

        [MenuItem("GameObject/Modern UI Pack/Progress Bars (Loop)/Standard Fastly", false, 0)]
        static void PBLSF()
        {
            GameObject clone = Instantiate(Resources.Load<GameObject>("Progress Bars (Loop)/Standard Fastly"), Vector3.zero, Quaternion.identity) as GameObject;

            try
            {
                clone.transform.SetParent(GameObject.Find("Canvas").transform, false);
                clone.name = clone.name.Replace("(Clone)", "").Trim();
            }
            catch
            {
                clone.name = clone.name.Replace("(Clone)", "").Trim();
                Debug.Log("No object found named 'Canvas', creating the object separately.");
            }
        }

        [MenuItem("GameObject/Modern UI Pack/Progress Bars (Loop)/Standard Run", false, 0)]
        static void PBLSRUN()
        {
            GameObject clone = Instantiate(Resources.Load<GameObject>("Progress Bars (Loop)/Standard Run"), Vector3.zero, Quaternion.identity) as GameObject;

            try
            {
                clone.transform.SetParent(GameObject.Find("Canvas").transform, false);
                clone.name = clone.name.Replace("(Clone)", "").Trim();
            }
            catch
            {
                clone.name = clone.name.Replace("(Clone)", "").Trim();
                Debug.Log("No object found named 'Canvas', creating the object separately.");
            }
        }

        [MenuItem("GameObject/Modern UI Pack/Sliders/Gradient", false, 0)]
        static void SLGR()
        {
            GameObject clone = Instantiate(Resources.Load<GameObject>("Sliders/Gradient"), Vector3.zero, Quaternion.identity) as GameObject;

            try
            {
                clone.transform.SetParent(GameObject.Find("Canvas").transform, false);
                clone.name = clone.name.Replace("(Clone)", "").Trim();
            }
            catch
            {
                clone.name = clone.name.Replace("(Clone)", "").Trim();
                Debug.Log("No object found named 'Canvas', creating the object separately.");
            }
        }

        [MenuItem("GameObject/Modern UI Pack/Sliders/Gradient (Popup)", false, 0)]
        static void SLGRP()
        {
            GameObject clone = Instantiate(Resources.Load<GameObject>("Sliders/Gradient (Popup)"), Vector3.zero, Quaternion.identity) as GameObject;

            try
            {
                clone.transform.SetParent(GameObject.Find("Canvas").transform, false);
                clone.name = clone.name.Replace("(Clone)", "").Trim();
            }
            catch
            {
                clone.name = clone.name.Replace("(Clone)", "").Trim();
                Debug.Log("No object found named 'Canvas', creating the object separately.");
            }
        }

        [MenuItem("GameObject/Modern UI Pack/Sliders/Gradient (Value)", false, 0)]
        static void SLGRV()
        {
            GameObject clone = Instantiate(Resources.Load<GameObject>("Sliders/Gradient (Value)"), Vector3.zero, Quaternion.identity) as GameObject;

            try
            {
                clone.transform.SetParent(GameObject.Find("Canvas").transform, false);
                clone.name = clone.name.Replace("(Clone)", "").Trim();
            }
            catch
            {
                clone.name = clone.name.Replace("(Clone)", "").Trim();
                Debug.Log("No object found named 'Canvas', creating the object separately.");
            }
        }

        [MenuItem("GameObject/Modern UI Pack/Sliders/Outline", false, 0)]
        static void SLO()
        {
            GameObject clone = Instantiate(Resources.Load<GameObject>("Sliders/Outline"), Vector3.zero, Quaternion.identity) as GameObject;

            try
            {
                clone.transform.SetParent(GameObject.Find("Canvas").transform, false);
                clone.name = clone.name.Replace("(Clone)", "").Trim();
            }
            catch
            {
                clone.name = clone.name.Replace("(Clone)", "").Trim();
                Debug.Log("No object found named 'Canvas', creating the object separately.");
            }
        }

        [MenuItem("GameObject/Modern UI Pack/Sliders/Outline (Popup)", false, 0)]
        static void SLOP()
        {
            GameObject clone = Instantiate(Resources.Load<GameObject>("Sliders/Outline (Popup)"), Vector3.zero, Quaternion.identity) as GameObject;

            try
            {
                clone.transform.SetParent(GameObject.Find("Canvas").transform, false);
                clone.name = clone.name.Replace("(Clone)", "").Trim();
            }
            catch
            {
                clone.name = clone.name.Replace("(Clone)", "").Trim();
                Debug.Log("No object found named 'Canvas', creating the object separately.");
            }
        }

        [MenuItem("GameObject/Modern UI Pack/Sliders/Outline (Value)", false, 0)]
        static void SLOV()
        {
            GameObject clone = Instantiate(Resources.Load<GameObject>("Sliders/Outline (Value)"), Vector3.zero, Quaternion.identity) as GameObject;

            try
            {
                clone.transform.SetParent(GameObject.Find("Canvas").transform, false);
                clone.name = clone.name.Replace("(Clone)", "").Trim();
            }
            catch
            {
                clone.name = clone.name.Replace("(Clone)", "").Trim();
                Debug.Log("No object found named 'Canvas', creating the object separately.");
            }
        }

        [MenuItem("GameObject/Modern UI Pack/Sliders/Radial Gradient", false, 0)]
        static void SLRG()
        {
            GameObject clone = Instantiate(Resources.Load<GameObject>("Sliders/Radial Gradient"), Vector3.zero, Quaternion.identity) as GameObject;

            try
            {
                clone.transform.SetParent(GameObject.Find("Canvas").transform, false);
                clone.name = clone.name.Replace("(Clone)", "").Trim();
            }
            catch
            {
                clone.name = clone.name.Replace("(Clone)", "").Trim();
                Debug.Log("No object found named 'Canvas', creating the object separately.");
            }
        }

        [MenuItem("GameObject/Modern UI Pack/Sliders/Radial Standard", false, 0)]
        static void SLRS()
        {
            GameObject clone = Instantiate(Resources.Load<GameObject>("Sliders/Radial Standard"), Vector3.zero, Quaternion.identity) as GameObject;

            try
            {
                clone.transform.SetParent(GameObject.Find("Canvas").transform, false);
                clone.name = clone.name.Replace("(Clone)", "").Trim();
            }
            catch
            {
                clone.name = clone.name.Replace("(Clone)", "").Trim();
                Debug.Log("No object found named 'Canvas', creating the object separately.");
            }
        }

        [MenuItem("GameObject/Modern UI Pack/Sliders/Range", false, 0)]
        static void SLRAN()
        {
            GameObject clone = Instantiate(Resources.Load<GameObject>("Sliders/Range"), Vector3.zero, Quaternion.identity) as GameObject;

            try
            {
                clone.transform.SetParent(GameObject.Find("Canvas").transform, false);
                clone.name = clone.name.Replace("(Clone)", "").Trim();
            }
            catch
            {
                clone.name = clone.name.Replace("(Clone)", "").Trim();
                Debug.Log("No object found named 'Canvas', creating the object separately.");
            }
        }

        [MenuItem("GameObject/Modern UI Pack/Sliders/Range (Clean)", false, 0)]
        static void SLRANC()
        {
            GameObject clone = Instantiate(Resources.Load<GameObject>("Sliders/Range (Clean)"), Vector3.zero, Quaternion.identity) as GameObject;

            try
            {
                clone.transform.SetParent(GameObject.Find("Canvas").transform, false);
                clone.name = clone.name.Replace("(Clone)", "").Trim();
            }
            catch
            {
                clone.name = clone.name.Replace("(Clone)", "").Trim();
                Debug.Log("No object found named 'Canvas', creating the object separately.");
            }
        }

        [MenuItem("GameObject/Modern UI Pack/Sliders/Range (Label)", false, 0)]
        static void SLRANL()
        {
            GameObject clone = Instantiate(Resources.Load<GameObject>("Sliders/Range (Label)"), Vector3.zero, Quaternion.identity) as GameObject;

            try
            {
                clone.transform.SetParent(GameObject.Find("Canvas").transform, false);
                clone.name = clone.name.Replace("(Clone)", "").Trim();
            }
            catch
            {
                clone.name = clone.name.Replace("(Clone)", "").Trim();
                Debug.Log("No object found named 'Canvas', creating the object separately.");
            }
        }

        [MenuItem("GameObject/Modern UI Pack/Sliders/Standard", false, 0)]
        static void SLSTA()
        {
            GameObject clone = Instantiate(Resources.Load<GameObject>("Sliders/Standard"), Vector3.zero, Quaternion.identity) as GameObject;

            try
            {
                clone.transform.SetParent(GameObject.Find("Canvas").transform, false);
                clone.name = clone.name.Replace("(Clone)", "").Trim();
            }
            catch
            {
                clone.name = clone.name.Replace("(Clone)", "").Trim();
                Debug.Log("No object found named 'Canvas', creating the object separately.");
            }
        }

        [MenuItem("GameObject/Modern UI Pack/Sliders/Standard (Popup)", false, 0)]
        static void SLSTAP()
        {
            GameObject clone = Instantiate(Resources.Load<GameObject>("Sliders/Standard (Popup)"), Vector3.zero, Quaternion.identity) as GameObject;

            try
            {
                clone.transform.SetParent(GameObject.Find("Canvas").transform, false);
                clone.name = clone.name.Replace("(Clone)", "").Trim();
            }
            catch
            {
                clone.name = clone.name.Replace("(Clone)", "").Trim();
                Debug.Log("No object found named 'Canvas', creating the object separately.");
            }
        }

        [MenuItem("GameObject/Modern UI Pack/Sliders/Standard (Value)", false, 0)]
        static void SLSTAV()
        {
            GameObject clone = Instantiate(Resources.Load<GameObject>("Sliders/Standard (Value)"), Vector3.zero, Quaternion.identity) as GameObject;

            try
            {
                clone.transform.SetParent(GameObject.Find("Canvas").transform, false);
                clone.name = clone.name.Replace("(Clone)", "").Trim();
            }
            catch
            {
                clone.name = clone.name.Replace("(Clone)", "").Trim();
                Debug.Log("No object found named 'Canvas', creating the object separately.");
            }
        }

        [MenuItem("GameObject/Modern UI Pack/Switches/Material", false, 0)]
        static void SWMAT()
        {
            GameObject clone = Instantiate(Resources.Load<GameObject>("Switches/Material"), Vector3.zero, Quaternion.identity) as GameObject;

            try
            {
                clone.transform.SetParent(GameObject.Find("Canvas").transform, false);
                clone.name = clone.name.Replace("(Clone)", "").Trim();
            }
            catch
            {
                clone.name = clone.name.Replace("(Clone)", "").Trim();
                Debug.Log("No object found named 'Canvas', creating the object separately.");
            }
        }

        [MenuItem("GameObject/Modern UI Pack/Switches/Standard", false, 0)]
        static void SWST()
        {
            GameObject clone = Instantiate(Resources.Load<GameObject>("Switches/Standard"), Vector3.zero, Quaternion.identity) as GameObject;

            try
            {
                clone.transform.SetParent(GameObject.Find("Canvas").transform, false);
                clone.name = clone.name.Replace("(Clone)", "").Trim();
            }
            catch
            {
                clone.name = clone.name.Replace("(Clone)", "").Trim();
                Debug.Log("No object found named 'Canvas', creating the object separately.");
            }
        }

        [MenuItem("GameObject/Modern UI Pack/Switches/Standard With Label", false, 0)]
        static void SWSTL()
        {
            GameObject clone = Instantiate(Resources.Load<GameObject>("Switches/Standard With Label"), Vector3.zero, Quaternion.identity) as GameObject;

            try
            {
                clone.transform.SetParent(GameObject.Find("Canvas").transform, false);
                clone.name = clone.name.Replace("(Clone)", "").Trim();
            }
            catch
            {
                clone.name = clone.name.Replace("(Clone)", "").Trim();
                Debug.Log("No object found named 'Canvas', creating the object separately.");
            }
        }

        [MenuItem("GameObject/Modern UI Pack/Toggles/Standard Toggle (Bold)", false, 0)]
        static void TSTB()
        {
            GameObject clone = Instantiate(Resources.Load<GameObject>("Toggles/Standard Toggle (Bold)"), Vector3.zero, Quaternion.identity) as GameObject;

            try
            {
                clone.transform.SetParent(GameObject.Find("Canvas").transform, false);
                clone.name = clone.name.Replace("(Clone)", "").Trim();
            }
            catch
            {
                clone.name = clone.name.Replace("(Clone)", "").Trim();
                Debug.Log("No object found named 'Canvas', creating the object separately.");
            }
        }

        [MenuItem("GameObject/Modern UI Pack/Toggles/Standard Toggle (Light)", false, 0)]
        static void TSTL()
        {
            GameObject clone = Instantiate(Resources.Load<GameObject>("Toggles/Standard Toggle (Light)"), Vector3.zero, Quaternion.identity) as GameObject;

            try
            {
                clone.transform.SetParent(GameObject.Find("Canvas").transform, false);
                clone.name = clone.name.Replace("(Clone)", "").Trim();
            }
            catch
            {
                clone.name = clone.name.Replace("(Clone)", "").Trim();
                Debug.Log("No object found named 'Canvas', creating the object separately.");
            }
        }

        [MenuItem("GameObject/Modern UI Pack/Toggles/Standard Toggle (Regular)", false, 0)]
        static void TSTR()
        {
            GameObject clone = Instantiate(Resources.Load<GameObject>("Toggles/Standard Toggle (Regular)"), Vector3.zero, Quaternion.identity) as GameObject;

            try
            {
                clone.transform.SetParent(GameObject.Find("Canvas").transform, false);
                clone.name = clone.name.Replace("(Clone)", "").Trim();
            }
            catch
            {
                clone.name = clone.name.Replace("(Clone)", "").Trim();
                Debug.Log("No object found named 'Canvas', creating the object separately.");
            }
        }

        [MenuItem("GameObject/Modern UI Pack/Toggles/Toggle Group Panel", false, 0)]
        static void TSTTG()
        {
            GameObject clone = Instantiate(Resources.Load<GameObject>("Toggles/Toggle Group Panel"), Vector3.zero, Quaternion.identity) as GameObject;

            try
            {
                clone.transform.SetParent(GameObject.Find("Canvas").transform, false);
                clone.name = clone.name.Replace("(Clone)", "").Trim();
            }
            catch
            {
                clone.name = clone.name.Replace("(Clone)", "").Trim();
                Debug.Log("No object found named 'Canvas', creating the object separately.");
            }
        }

        [MenuItem("GameObject/Modern UI Pack/Tool Tips/Fading Tool Tip", false, 0)]
        static void TPFTP()
        {
            GameObject clone = Instantiate(Resources.Load<GameObject>("Tool Tips/Fading Tool Tip"), Vector3.zero, Quaternion.identity) as GameObject;

            try
            {
                clone.transform.SetParent(GameObject.Find("Canvas").transform, false);
                clone.name = clone.name.Replace("(Clone)", "").Trim();
            }
            catch
            {
                clone.name = clone.name.Replace("(Clone)", "").Trim();
                Debug.Log("No object found named 'Canvas', creating the object separately.");
            }
        }

        [MenuItem("GameObject/Modern UI Pack/Tool Tips/Scaling Tool Tip", false, 0)]
        static void TPSTP()
        {
            GameObject clone = Instantiate(Resources.Load<GameObject>("Tool Tips/Scaling Tool Tip"), Vector3.zero, Quaternion.identity) as GameObject;

            try
            {
                clone.transform.SetParent(GameObject.Find("Canvas").transform, false);
                clone.name = clone.name.Replace("(Clone)", "").Trim();
            }
            catch
            {
                clone.name = clone.name.Replace("(Clone)", "").Trim();
                Debug.Log("No object found named 'Canvas', creating the object separately.");
            }
        }
    }
}