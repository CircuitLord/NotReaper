using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using NotReaper.Timing;
using NotReaper;
using NotReaper.UI;
using NotReaper.Models;

public class ModifierTimeline : MonoBehaviour
{
    public static ModifierTimeline Instance = null;
    
    [Header("References")]
    [SerializeField] private GameObject modifierStartPrefab;
    [SerializeField] private GameObject modifierEndPrefab;
    [SerializeField] private LineRenderer modifierConnectorPrefab;

    public List<ModifierContainer> modifiers = new List<ModifierContainer>();

    private ModifierContainer currentPair;

    public void Start()
    {
        if (Instance is null) Instance = this;
        else
        {
            Debug.LogWarning("Trying to create a second instance of ModifierTimeline.");
            return;
        }
    }

    public void CreateModifier(ModifierHandler.ModifierData data)
    {
        LookForOtherModifiers(data.startTick, data.endTick);
        currentPair.data = data;
        currentPair.startMarkTop.GetComponent<SpriteRenderer>().color = Color.white;
        currentPair.startMarkBottom.GetComponent<SpriteRenderer>().color = Color.white;
        currentPair.endMarkTop.GetComponent<SpriteRenderer>().color = Color.white;
        currentPair.endMarkBottom.GetComponent<SpriteRenderer>().color = Color.white;
        currentPair.connector.startColor = Color.white;
        currentPair.connector.endColor = Color.white;
        modifiers.Add(currentPair);
        currentPair = new ModifierContainer();
        modifiers.Add(currentPair);
    }

    private void LookForOtherModifiers(QNT_Timestamp startTick, QNT_Timestamp endTick)
    {
        if (currentPair.raised) return;
        int numFound = 0;
        bool found = false;
        foreach(ModifierContainer mc in modifiers)
        {
            if (mc.data.startTick <= startTick && mc.data.endTick >= startTick)
            {
                if (mc.level > numFound || numFound == 0) numFound = mc.level;
                found = true;
            }
            else if (startTick <= mc.data.startTick && endTick >= mc.data.startTick)
            {
                if (mc.level > numFound || numFound == 0) numFound = mc.level;
                found = true;
            }
               
        }
        bool noRaise = false;
        if(found && numFound == 0)
        {
            numFound = 1;
            noRaise = true;
        }
        if(numFound != 0)
        {
            if(!noRaise) numFound++;
            currentPair.raised = true;
            currentPair.level = numFound;
            Vector3 pStart = currentPair.startMarkTop.transform.position;
            Vector3 pEnd = currentPair.endMarkTop.transform.position;
            float addY = numFound * .3f;
            currentPair.startMarkTop.transform.position = new Vector3(pStart.x, pStart.y - addY, pStart.z);
            currentPair.endMarkTop.transform.position = new Vector3(pEnd.x, pEnd.y - addY, pEnd.z);
        }
            
       
    }

    private void LookForOtherModifiers(QNT_Timestamp tick)
    {
        if (currentPair.raised) return;
        int numFound = 0;
        bool found = false;
        foreach(ModifierContainer mc in modifiers)
        {
            if (mc.data.startTick <= tick && mc.data.endTick >= tick)
            {
                if (mc.level > numFound) numFound = mc.level;
                found = true;
            }           
            else if (mc.data.startTick == tick || mc.data.endTick == tick)
            {
                if (mc.level > numFound) numFound = mc.level;
                found = true;
            }
               
        }
        bool noRaise = false;
        if (found && numFound == 0)
        {
            numFound = 1;
            noRaise = true;
        }
          
        if(numFound != 0)
        {
            currentPair.raised = true;
            if(!noRaise) numFound++;
            float addY = numFound * .3f;
            if (currentPair.startMarkTop != null)
            {
                Vector3 pStart = currentPair.startMarkTop.transform.position;
                currentPair.startMarkTop.transform.position = new Vector3(pStart.x, pStart.y - addY, pStart.z);
            }
            if (currentPair.endMarkTop != null)
            {
                Vector3 pEnd = currentPair.endMarkTop.transform.position;
                currentPair.endMarkTop.transform.position = new Vector3(pEnd.x, pEnd.y - addY, pEnd.z);
            }
            currentPair.level = numFound;
        }
        

    }

    public void ShowModifiers(bool show)
    {
        if (modifiers.Count == 0) return;
        foreach (ModifierContainer mp in modifiers)
        {
            if(mp.startMarkTop != null) mp.startMarkTop.SetActive(show);
            if (mp.startMarkBottom != null) mp.startMarkBottom.SetActive(show);
            if (mp.endMarkTop != null) mp.endMarkTop.SetActive(show);
            if (mp.endMarkBottom != null) mp.endMarkBottom.SetActive(show);
        }
    }

    public void UpdateMark(UpdateType type, ulong tick = 0)
    {
        switch (type)
        {
            case UpdateType.UpdateStart:
                currentPair.startMarkTop.transform.position = new Vector3(0f, currentPair.startMarkTop.transform.position.y, 0);
                currentPair.startMarkBottom.transform.position = new Vector3((float)MiniTimeline.Instance.GetXForTheModifierThingy(new QNT_Timestamp(tick)), 0f, 0f);
                if (currentPair.connector != null) GameObject.Destroy(currentPair.connector);
                if (currentPair.endMarkTop != null) GameObject.Destroy(currentPair.endMarkTop);
                if (currentPair.endMarkBottom != null) GameObject.Destroy(currentPair.endMarkBottom);
                currentPair.endMarkTop = null;
                currentPair.endMarkBottom = null;
                break;
            case UpdateType.UpdateEnd:
                if (currentPair.endMarkBottom != null) GameObject.Destroy(currentPair.endMarkBottom);
                if (currentPair.endMarkTop != null) GameObject.Destroy(currentPair.endMarkTop);
                if (currentPair.connector != null) GameObject.Destroy(currentPair.connector);
                currentPair.endMarkTop = null;
                currentPair.endMarkBottom = null;
                break;
        }
    }

    public void SetModifierMark(ModifierHandler.ModifierType type, QNT_Timestamp tick, string shorthand, bool startMarker)
    {
        GameObject modifierBottom = Instantiate(startMarker ? modifierStartPrefab : modifierEndPrefab, new Vector3(0f, 0f, 0f), Quaternion.identity, MiniTimeline.Instance.bookmarksParent);
        GameObject modifierTop = Instantiate(startMarker ? modifierStartPrefab : modifierEndPrefab, null);
        if(startMarker) modifierTop.GetComponent<IconTextSetter>().SetText(shorthand);
        Color background = Color.white;
        background.a = .5f;

        modifierBottom.transform.localPosition = new Vector3((float)MiniTimeline.Instance.GetXForTheModifierThingy(tick), 0f, 0f);




        modifierTop.transform.position = startMarker ? new Vector3(0f, modifierTop.transform.position.y, 0) : new Vector3(0f, currentPair.startMarkTop.transform.position.y, 0);
        modifierTop.transform.SetParent(Timeline.timelineNotesStatic);
        modifierTop.transform.localScale = new Vector3(0.3f, 0.3f);
        modifierBottom.transform.localScale = new Vector3(10f, 10f);
        modifierBottom.GetComponent<SpriteRenderer>().color = background;
        modifierTop.GetComponent<SpriteRenderer>().color = background;

        if (startMarker)
        {

            currentPair.startTick = tick;
            if (currentPair.startMarkBottom != null) GameObject.Destroy(currentPair.startMarkBottom);
            if (currentPair.startMarkTop != null) GameObject.Destroy(currentPair.startMarkTop);
           
            currentPair.startMarkBottom = modifierBottom;
            currentPair.startMarkTop = modifierTop;
            if (currentPair.endMarkTop != null)
            {
                LineRenderer lr = GameObject.Instantiate(modifierConnectorPrefab, null);
                lr.SetPosition(0, modifierTop.transform.position);
                lr.SetPosition(1, currentPair.endMarkTop.transform.position);
                lr.transform.SetParent(currentPair.startMarkTop.transform);
                lr.startColor = Color.grey;
                lr.endColor = Color.grey;
                if (currentPair.connector != null) GameObject.Destroy(currentPair.connector);
                currentPair.connector = lr;
            }
            LookForOtherModifiers(tick);
        }
        else
        {
            currentPair.endTick = tick;
            LineRenderer lr = GameObject.Instantiate(modifierConnectorPrefab, null);
            lr.SetPosition(0, currentPair.startMarkTop.transform.position);
            lr.SetPosition(1, modifierTop.transform.position);
            lr.transform.SetParent(currentPair.startMarkTop.transform);
            if (currentPair.endMarkBottom != null) GameObject.Destroy(currentPair.endMarkBottom);
            if (currentPair.endMarkTop != null) GameObject.Destroy(currentPair.endMarkTop);
            if (currentPair.connector != null) GameObject.Destroy(currentPair.connector);
            currentPair.endMarkBottom = modifierBottom;
            currentPair.endMarkTop = modifierTop;
            currentPair.connector = lr;
            LookForOtherModifiers(currentPair.startTick, tick);
        }


        
        //if (addToAudicaFile) Timeline.audicaFile.desc.bookmarks.Add(new BookmarkData() { type = newType, xPosMini = miniXPos, xPosTop = bookmarkTop.transform.localPosition.x });
    }

    public struct ModifierContainer
    {
        public GameObject startMarkTop;
        public GameObject startMarkBottom;
        public GameObject endMarkTop;
        public GameObject endMarkBottom;
        public LineRenderer connector;
        public bool raised;
        public int level;
        public QNT_Timestamp startTick;
        public QNT_Timestamp endTick;
        public ModifierHandler.ModifierData data;

        public void Reset()
        {
            startMarkBottom = null;
            startMarkTop = null;
            endMarkTop = null;
            endMarkBottom = null;
            connector = null;
        }
    }

    public enum UpdateType
    {
        UpdateStart,
        UpdateEnd
    }
}
