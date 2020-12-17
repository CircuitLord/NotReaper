using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
namespace NotReaper.Modifier
{
    public class TimelineEntry : MonoBehaviour
    {
        [SerializeField] private GameObject glow;
        private ModifierTimeline.ModifierContainer container;
        private bool isSelected = false;
        private bool isCreated = false;
        public void SetContainer(ModifierTimeline.ModifierContainer _container, bool fromLoad)
        {
            container = _container;
            isCreated = true;
            Select(false);
            if (fromLoad)
            {
                if (container.startMarkTop != null) container.startMarkTop.SetActive(false);
                if (container.endMarkTop != null) container.endMarkTop.SetActive(false);
                if (container.connector != null) container.connector.gameObject.SetActive(false);
                if (container.endMarkBottom != null) container.endMarkBottom.SetActive(false);
                if (container.startMarkBottom != null) container.startMarkBottom.SetActive(false);
                this.gameObject.SetActive(false);
            }
               
        }
        private void OnMouseDown()
        {
            if (!isCreated) return;

            if(Input.GetKey(KeyCode.LeftControl) || Input.GetKey(KeyCode.RightControl))
            {
                ModifierHandler.instance.SelectModifier(container, this);
            }        
        }

        public void Select(bool select)
        {
            isSelected = select;
            glow.SetActive(select);            
        }

        private void Update()
        {
            if (!isCreated) return;
            if (!isSelected) return;

            if(Input.GetKeyDown(KeyCode.Backspace) || Input.GetKeyDown(KeyCode.Delete))
            {
                ModifierHandler.instance.RemoveModifier(container.data.modifier);
                ModifierTimeline.Instance.RemoveModifier(container);
                if(container.endMarkBottom != null) GameObject.Destroy(container.endMarkBottom);
                if (container.startMarkBottom != null) GameObject.Destroy(container.startMarkBottom);
                if (container.endMarkTop != null) GameObject.Destroy(container.endMarkTop);
                if (container.connector != null) GameObject.Destroy(container.connector);
                GameObject.Destroy(this.gameObject);
            }
        }

        public void ReportClick()
        {
            Debug.Log("Report Click");
            OnMouseDown();
        }
    }
}

