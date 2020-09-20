using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Events;

public class NRPrompt : UnitySingleton<NRPrompt>
{
    [SerializeField] NRButton confirm;
    [SerializeField] NRButton cancel;
    [SerializeField] TextMeshProUGUI promptText;
    [SerializeField] TextMeshProUGUI promptTitle;

    private bool promptActive;

    private Queue<PromptQueueEntry> promptQueue = new Queue<PromptQueueEntry>();

    public void CreatePrompt(UnityAction confirmAction, string promptString, string promptTitleString)
    {
        if (promptActive) promptQueue.Enqueue(new PromptQueueEntry(confirmAction, promptString, promptTitleString));
        gameObject.SetActive(true);
        promptActive = true;
        confirm.NROnClick = new UnityEvent();
        confirm.NROnClick.AddListener(confirmAction);
        confirm.NROnClick.AddListener(new UnityAction(() => { ClosePrompt(); }));
        promptText.text = promptString;
        promptTitle.text = promptTitleString;
    }

    public void ClosePrompt()
    {
        gameObject.SetActive(false);
        promptActive = false;
        if(promptQueue.Count > 0)
        {
            var newPrompt = promptQueue.Dequeue();
            CreatePrompt(newPrompt.confirmAction, newPrompt.promptString, newPrompt.promptTitleString);
        }
    }

    private struct PromptQueueEntry
    {
        public UnityAction confirmAction;
        public string promptString;
        public string promptTitleString;

        public PromptQueueEntry(UnityAction confirmAction, string promptString, string promptTitleString)
        {
            this.confirmAction = confirmAction;
            this.promptString = promptString;
            this.promptTitleString = promptTitleString;

        }
    }
}
