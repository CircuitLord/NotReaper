using NotReaper.Models;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace NotReaper.Modifier
{
    public class ZOffsetBaker : MonoBehaviour
    {
        public static ZOffsetBaker Instance = null;
        public static bool baking = false;
        public static bool active = false;

        public GameObject zOffsetWindow;

        private Vector3 activatePosition = new Vector3(0f, 13f, 0f);
        private void Start()
        {
            if (Instance is null)
            {
                Instance = this;
            }
            else
            {
                Debug.LogWarning("Trying to create second ZOffsetBaker instance.");
                return;
            }
        }


        public List<Cue> Bake(List<Cue> cues)
        {
            Dictionary<Cue, float> oldOffsetDict = new Dictionary<Cue, float>();
            foreach (Cue c in cues) oldOffsetDict.Add(c, c.zOffset);
            List<Modifier> zOffsetList = ModifierHandler.Instance.GetZOffsetModifiers();
            zOffsetList.Sort((mod1, mod2) => mod1.startTime.CompareTo(mod2.startTime));
            foreach(Modifier m in zOffsetList)
            {
                Debug.Log("-------------------------");                
                Debug.Log("amount: " + m.amount);
                float currentCount = 1f;
                m.option1 = true;
                bool endTickSet = m.endTime.tick != 0 && m.startTime.tick != m.endTime.tick;
                foreach (Cue cue in cues)
                {
                    if (cue.tick < (int)m.startTime.tick) continue;
                    if (cue.tick > (int)m.endTime.tick && endTickSet) break;
                    if (cue.behavior != TargetBehavior.Melee && cue.behavior != TargetBehavior.Mine)
                    {
                        float transitionNumberOfTargets = 0f;
                        float.TryParse(m.value1, out transitionNumberOfTargets);
                        if (transitionNumberOfTargets > 0)
                        {
                            cue.zOffset = Mathf.Lerp(cue.zOffset, m.amount, currentCount / (float)transitionNumberOfTargets);
                        }
                        else
                        {
                            cue.zOffset = m.amount;
                        }
                        cue.zOffset /= 100f;
                        cue.zOffset += oldOffsetDict[cue];
                        if (currentCount < transitionNumberOfTargets) currentCount++;
                    }
                    Debug.Log("New offset: " + cue.zOffset);
                }
            }
            /*
            foreach(Modifier m in zOffsetList)
            {
                float currentCount = 1f;
                m.option1 = true;
                bool endTickSet = m.endTime.tick != 0 && m.startTime.tick != m.endTime.tick;
                foreach (Cue cue in cues)
                {
                    if (cue.tick < (int)m.startTime.tick) continue;
                    if (cue.tick > (int)m.endTime.tick && endTickSet) break;
                    if(cue.behavior != TargetBehavior.Melee && cue.behavior != TargetBehavior.Mine)
                    {
                        float transitionNumberOfTargets = 0f;
                        float.TryParse(m.value1, out transitionNumberOfTargets);
                        if(transitionNumberOfTargets > 0)
                        {
                            cue.zOffset = Mathf.Lerp(cue.zOffset, m.amount, currentCount / (float)transitionNumberOfTargets);
                        }
                        else
                        {
                            cue.zOffset = m.amount;
                        }
                        if (currentCount < transitionNumberOfTargets) currentCount++;
                    }
                }
            }*/
            return cues;
        }

        private void Unbake()
        {
            List<Modifier> zOffsetList = ModifierHandler.Instance.GetZOffsetModifiers();
            foreach (Modifier m in zOffsetList) m.option1 = false;
        }

        public void OnBakeButtonPressed()
        {
            Timeline.audicaFile.desc.bakedzOffset = true;
            Timeline.instance.Export();
            ToggleWindow();
        }

        public void OnUnbakeButtonPressed()
        {
            Timeline.audicaFile.desc.bakedzOffset = false;
            Unbake();
            Timeline.instance.Export();
            ToggleWindow();
        }

        public void ToggleWindow()
        {
            active = !active;
            zOffsetWindow.transform.localPosition = active ? activatePosition : new Vector3(-3000f, 13f, 0f);
            zOffsetWindow.SetActive(active);
        }

        private void Update()
        {
            if (!ModifierHandler.activated) return;
            if (Input.GetKeyDown(KeyCode.B))
            {
                ToggleWindow();
            }
        }
    }

}
