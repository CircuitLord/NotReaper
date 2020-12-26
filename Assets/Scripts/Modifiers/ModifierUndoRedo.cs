using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
namespace NotReaper.Modifier
{
    public class ModifierUndoRedo : MonoBehaviour
    {
        public static ModifierUndoRedo Instance = null;
        [SerializeField] public List<ModifierAction> actionHistory = new List<ModifierAction>();
        public List<Modifier> recreatedModifiers = new List<Modifier>();
        public int actionIndex = 0;
        private bool undoRedoActive = false;
        private void Start()
        {
            if (Instance is null) Instance = this;
            else
            {
                Debug.LogWarning("Trying to create second ModifierUndoRedo instance.");
                return;
            }
        }
        public void Undo()
        {
            
            if(actionIndex > 0)
            {
                undoRedoActive = true;
                actionIndex --;
                ModifierAction ma = actionHistory[actionIndex];
                if (ma.action == Action.Delete)
                {
                    Debug.Log("undoing deletion..");
                    //StartCoroutine(ModifierHandler.Instance.LoadModifiers(ma.dtos, false, true));
                    ModifierSelectionHandler.Instance.Restore(ma.dtos);
                }
                else
                {
                    for(int i = 0; i < ma.modifiers.Count; i++)
                    {
                        Debug.Log("undoing creation..");
                        ModifierSelectionHandler.Instance.SelectModifier(ma.modifiers[i], false);
                        ModifierHandler.Instance.DeleteModifier();
                    }
                }
                
            }
            undoRedoActive = false;
        }

        public void Redo()
        {
            if(actionIndex < actionHistory.Count)
            {
               
                undoRedoActive = true;
                ModifierAction ma = actionHistory[actionIndex];
                if(ma.action == Action.Create)
                {
                    Debug.Log("Redoing creation..");
                    //StartCoroutine(ModifierHandler.Instance.LoadModifiers(ma.dtos, false, true));
                    ModifierSelectionHandler.Instance.Restore(ma.dtos);
                }
                else
                {
                    for (int i = 0; i < ma.modifiers.Count; i++)
                    {
                        Debug.Log("Redoing deletion..");
                        ModifierSelectionHandler.Instance.SelectModifier(ma.modifiers[i], false);
                        ModifierHandler.Instance.DeleteModifier();
                    }
                }
                actionIndex++;
            }
            undoRedoActive = false;
        }

        public bool AddAction(List<Modifier> _modifiers, Action _action)
        {
            if (undoRedoActive) return false;
            for (int i = actionHistory.Count - 1; i >= actionIndex; i--) actionHistory.RemoveAt(i);
            actionHistory.Add(new ModifierAction(_modifiers, _action));
            actionIndex++;
            return true;
        }

        public void ClearHistory()
        {
            actionHistory.Clear();
        }
    }

    public enum Action
    {
        Create,
        Delete,
    }

    [Serializable]
    public struct ModifierAction
    {
        public List<Modifier> modifiers;
        public List<ModifierDTO> dtos;
        public Action action;

        public ModifierAction(List<Modifier> _modifiers, Action _action)
        {
            modifiers = _modifiers;
            action = _action;
            dtos = new List<ModifierDTO>();
            foreach(Modifier m in modifiers)
            {
                dtos.Add(m.GetDTO());
            }          
        }
    }
}

