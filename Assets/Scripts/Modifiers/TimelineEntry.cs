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
        public bool isSelected = false;
        private bool isCreated = false;
        public void SetContainer(ModifierTimeline.ModifierContainer _container, bool fromLoad)
        {
            ModifierSelectionHandler.selectables.Add(this);
            container = _container;
            isCreated = true;
            Select(false);
            if (fromLoad && !ModifierSelectionHandler.isPasting)
            {
                if (container.startMarkTop != null) container.startMarkTop.SetActive(false);
                if (container.endMarkTop != null) container.endMarkTop.SetActive(false);
                if (container.connector != null) container.connector.gameObject.SetActive(false);
                if (container.endMarkBottom != null) container.endMarkBottom.SetActive(false);
                if (container.startMarkBottom != null) container.startMarkBottom.SetActive(false);
                this.gameObject.SetActive(false);
            }               
        }

        public ModifierTimeline.ModifierContainer GetContainer()
        {
            return container;
        }
        private void OnMouseDown()
        {
            if (!isCreated) return;

            if (Input.GetKey(KeyCode.LeftControl))
            {
                if (isSelected)
                {
                    ModifierSelectionHandler.RemoveEntry(this);
                }
                else
                {
                    ModifierSelectionHandler.AddEntry(this);
                }
                ModifierHandler.Instance.DeselectModifier();
            }
            else
            {
                ModifierSelectionHandler.RemoveAllEntries();
                ModifierSelectionHandler.selectedEntry = this;
                ModifierHandler.Instance.SelectModifier(container, this);
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

            if(Input.GetKeyDown(KeyCode.Delete))
            {
                DeleteModifier();
            }
        }

        public void DeleteModifier()
        {
            //ModifierDragHandler.RemoveSelectedEntry(this);
            ModifierTimeline.Instance.SelectModifier(container);
            ModifierTimeline.Instance.RemoveModifier(container);
            ModifierHandler.Instance.RemoveModifier(container.data.modifier);
           
           /* if (container.endMarkBottom != null) GameObject.Destroy(container.endMarkBottom);
            if (container.startMarkBottom != null) GameObject.Destroy(container.startMarkBottom);
            if (container.endMarkTop != null) GameObject.Destroy(container.endMarkTop);
            if (container.connector != null) GameObject.Destroy(container.connector);
            GameObject.Destroy(this.gameObject);
            */
        }

        public void ReportClick()
        {
            OnMouseDown();
        }
    }
}

