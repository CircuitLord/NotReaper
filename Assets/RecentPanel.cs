using NotReaper;
using NotReaper.UI;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Windows.Forms;
using TMPro;
using UnityEngine;
using UnityEngine.Events;

public class RecentPanel : MonoBehaviour
{
    [SerializeField] NRButton[] buttons;
    [SerializeField] Timeline timeline;
    [SerializeField] PauseMenu pauseMenu;

    public void Show()
    {
        if(RecentAudicaFiles.audicaPaths == null) RecentAudicaFiles.LoadRecents();
        if (RecentAudicaFiles.audicaPaths != null)
        {
            if (RecentAudicaFiles.audicaPaths.Count >= 1)
            {
                gameObject.SetActive(true);
                FillRecentButtons();
            }
        }

    }

    public void FillRecentButtons()
    {
        for (int i = 0; i < buttons.Length; i++)
        {
            if (i >= (RecentAudicaFiles.audicaPaths.Count - 1)) return;
            string path = RecentAudicaFiles.audicaPaths[i];
            buttons[i].NROnClick = new UnityEvent();
            buttons[i].NROnClick.AddListener(new UnityAction(() => 
            { 
                timeline.LoadAudicaFile(false, path);
                pauseMenu.ClosePauseMenu();
            }));
            buttons[i].GetComponentInChildren<TextMeshProUGUI>().text = path.Split(Path.DirectorySeparatorChar).Last();
            buttons[i].gameObject.SetActive(true);
        }
    }

    public void Clear()
    {
        RecentAudicaFiles.ClearRecents();
        for (int i = 0; i < buttons.Length; i++)
        {
            buttons[i].gameObject.SetActive(false);
        }
        gameObject.SetActive(false);
    }
}
