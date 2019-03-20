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

    public Color leftColor;
    public Color rightColor;
    public Color eitherColor;
    public Color noneColor;

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
