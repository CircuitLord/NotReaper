using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TargetIcon : MonoBehaviour {
    
    public GameObject standard;
    public GameObject hold;
    public GameObject horizontal;
    public GameObject vertical;
    public GameObject chainStart;
    public GameObject chain;
    public GameObject melee;
    public GameObject line;

    public Color leftColor;
    public Color rightColor;
    public Color eitherColor;
    public Color noneColor;

    public float sustainDirection = 0.6f;

    public void SetHandType(TargetHandType handType)
    {
        foreach (Renderer r in gameObject.GetComponentsInChildren<Renderer>(true))
        {
            switch (handType)
            {
                case TargetHandType.Left:
                    r.material.SetColor("_Tint", leftColor);
                    break;
                case TargetHandType.Right:
                    r.material.SetColor("_Tint", rightColor);
                    break;
                case TargetHandType.Either:
                    r.material.SetColor("_Tint", eitherColor);
                    break;
                default:
                    r.material.SetColor("_Tint", noneColor);
                    break;
            }
        }
        foreach (LineRenderer l in gameObject.GetComponentsInChildren<LineRenderer>(true))
        {
            switch (handType)
            {
                case TargetHandType.Left:
                    l.startColor = leftColor;
                    l.endColor = leftColor;
                    sustainDirection = 0.6f;
                    break;
                case TargetHandType.Right:
                    l.startColor = rightColor;
                    l.endColor = rightColor;
                    sustainDirection = -0.6f;
                    break;
                case TargetHandType.Either:
                    l.startColor = eitherColor;
                    l.endColor = eitherColor;
                    sustainDirection = 0.6f;
                    break;
                default:
                    l.startColor = noneColor;
                    l.endColor = noneColor;
                    sustainDirection = 0.6f;
                    break;
            }
        }

    }

    public void SetSustainLength(float beatLength)
    {
        foreach (LineRenderer l in gameObject.GetComponentsInChildren<LineRenderer>(true))
        {
            if (beatLength > 1)
            {
                l.SetPosition(0, new Vector3(0.0f, 0.0f, 0.0f));
                l.SetPosition(1, new Vector3(0.0f, sustainDirection, 0.0f));
                l.SetPosition(2, new Vector3((beatLength / 2.8f), sustainDirection, 0.0f));
            }
            else
            {
                l.SetPosition(0, new Vector3(0.0f, 0.0f, 0.0f));
                l.SetPosition(1, new Vector3(0.0f, 0.0f, 0.0f));
                l.SetPosition(2, new Vector3(0.0f, 0.0f, 0.0f));
            }
        }
    }

    public void SetBehavior(TargetBehavior behavior)
    {
        standard.SetActive(behavior == TargetBehavior.Standard);
        hold.SetActive(behavior == TargetBehavior.Hold);
        horizontal.SetActive(behavior == TargetBehavior.Horizontal);
        vertical.SetActive(behavior == TargetBehavior.Vertical);
        chainStart.SetActive(behavior == TargetBehavior.ChainStart);
        chain.SetActive(behavior == TargetBehavior.Chain);
        melee.SetActive(behavior == TargetBehavior.Melee);
    }
}
