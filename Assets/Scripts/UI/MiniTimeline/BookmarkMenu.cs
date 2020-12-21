using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using NotReaper.UI;
public class BookmarkMenu : MonoBehaviour
{
    public static BookmarkMenu Instance = null;
    public static bool isActive = false;
    public static bool inputFocused = false;
    public TMP_InputField inputField;
    public GameObject menu;
    private Vector3 activatePosition = new Vector3(0f, 13f, 0f);

    private void Start()
    {
        if (Instance is null) Instance = this;
        else
        {
            Debug.LogWarning("Trying to create a second BookmarkMenu instance.");
            return;
        }
        inputField.onSelect.AddListener(OnInputFocused);
        inputField.onDeselect.AddListener(OnInputFocused);
    }

    private void OnInputFocused(string _)
    {
        inputFocused = !inputFocused;
    }
    public void Activate(bool active)
    {
        isActive = active;
        menu.transform.localPosition = active ? activatePosition : new Vector3(-3000f, 13f, 0f);
        //activatePosition = menu.transform.localPosition;
        //menu.SetActive(active);
    }
    public void SetText(string text)
    {
        inputField.text = text;
    }

    public void Save()
    {
        MiniTimeline.Instance.selectedBookmark.SetText(inputField.text);
        MiniTimeline.Instance.SaveSelectedBookmark();
        MiniTimeline.Instance.selectedBookmark.Deselect();
        MiniTimeline.Instance.OpenBookmarksMenu("");
    }

    public void Delete()
    {
        MiniTimeline.Instance.DeleteBookmark();
    }

    public void Scale()
    {
        foreach (Bookmark b in MiniTimeline.Instance.bookmarks) b.Scale();
    }
}
