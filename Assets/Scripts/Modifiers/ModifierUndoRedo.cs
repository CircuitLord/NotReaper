using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
namespace NotReaper.Modifier
{
    public class ModifierUndoRedo : MonoBehaviour
    {
        public static ModifierUndoRedo Instance = null;
        public static bool recreating = false;
        [SerializeField] public List<ModifierAction> actionHistory = new List<ModifierAction>();
        public List<Modifier> recreatedModifiers = new List<Modifier>();
        public int actionIndex = 0;
        public static bool undoRedoActive = false;
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
                    RestoreModifier(ma);
                }
                else
                {
                    RemoveModifier(ma);
                }
                undoRedoActive = false;
                StartCoroutine(ModifierHandler.Instance.IUpdateLevels());
            }
           
           
        }

        public void Redo()
        {
            if(actionIndex < actionHistory.Count)
            {
                undoRedoActive = true;
                ModifierAction ma = actionHistory[actionIndex];
                if(ma.action == Action.Create)
                {
                    RestoreModifier(ma);
                }
                else
                {
                    Debug.Log("Remove");
                    RemoveModifier(ma);
                }
                actionIndex++;
                undoRedoActive = false;
                StartCoroutine(ModifierHandler.Instance.IUpdateLevels());
            }
            
            
        }

        private void RemoveModifier(ModifierAction action)
        {
            for (int i = 0; i < action.modifiers.Count; i++)
            {
                ModifierSelectionHandler.Instance.SelectModifier(action.modifiers[i], false);
                ModifierHandler.Instance.DeleteModifier();
            }
        }

        private void RestoreModifier(ModifierAction action)
        {

            recreating = true;
            action.modifiers.Clear();
            ModifierSelectionHandler.Instance.Restore(action.dtos);
            foreach (Modifier m in recreatedModifiers) action.modifiers.Add(m);
            recreating = false;
            recreatedModifiers.Clear();
        }

        public bool AddAction(List<Modifier> _modifiers, Action _action)
        {
            if (undoRedoActive) return false;
            if(actionHistory.Count > 20)
            {
                while (20 > actionHistory.Count)
                {
                    actionHistory.RemoveAt(0);
                }
            }

            for (int i = actionHistory.Count - 1; i >= actionIndex; i--) actionHistory.RemoveAt(i);
            actionHistory.Add(new ModifierAction(_modifiers, _action));
            actionIndex++;
            return true;
        }

        public bool AddAction(List<ModifierDTO> _dtoList, Action _action)
        {
            if (undoRedoActive) return false;
            if (actionHistory.Count > 20)
            {
                while (20 > actionHistory.Count)
                {
                    actionHistory.RemoveAt(0);
                }
            }

            for (int i = actionHistory.Count - 1; i >= actionIndex; i--) actionHistory.RemoveAt(i);
            actionHistory.Add(new ModifierAction(_dtoList, _action));
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

        public ModifierAction(List<ModifierDTO> _dtoList, Action _action)
        {
            modifiers = new List<Modifier>();
            dtos = _dtoList;
            action = _action;
        }
    }
}

