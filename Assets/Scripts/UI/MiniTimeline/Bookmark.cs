using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using TMPro;
using NotReaper.Models;

namespace NotReaper.UI {

    public class Bookmark : MonoBehaviour {

        public int index = 0;

        public TextMeshProUGUI text;
        public bool isSelected = false;
        private Bookmark mini;
        public SpriteRenderer rend;
        public BoxCollider2D boxCollider;
        public TargetHandType handType;
        public float xPosMini;
        public double percentBookmark = 0;
        private BookmarkUIColor myUIColor;

        private Vector3 originalScale = new Vector3(0.05f, 0.03f, 1f);
        private bool needsScaling = true;
        private void Start()
        {
            originalScale = transform.localScale;
        }

        public void SetIndex(int i) {
            index = i;
            text.text = index.ToString();
        }

        public void SetText(string _text)
        {
            text.text = _text;
            UpdateColor();
        }

        public void UpdateColor()
        {
            rend.color = BookmarkColorPicker.selectedColor;
        }

        public void Select()
        {
            MiniTimeline.Instance.selectedBookmark = this;
            isSelected = !isSelected;
            if (isSelected)
            {
                BookmarkMenu.Instance.SetColor(myUIColor);
            }
            MiniTimeline.Instance.OpenBookmarksMenu(text.text);
        }

        public void Deselect()
        {
            isSelected = false;
            MiniTimeline.Instance.selectedBookmark = null;
        }

        public string GetText()
        {
            return text.text;
        }

        public void Initialize(Bookmark b, string _text, TargetHandType _handType, float _xPosMini)
        {
            mini = b;
            text.text = _text;
            handType = _handType;
            xPosMini = _xPosMini;
            originalScale = transform.localScale;
        }

        public void DeleteBookmark()
        {
            //MiniTimeline.Instance.bookmarks.Remove(mini);
            MiniTimeline.Instance.bookmarks.Remove(this);
            MiniTimeline.Instance.selectedBookmark = null;
            MiniTimeline.Instance.OpenBookmarksMenu("");
            GameObject.Destroy(mini.gameObject);
            GameObject.Destroy(this.gameObject);
        }

        public void FixScaling()
        {
            needsScaling = false;
            rend.size = new Vector2(0.1f, 22f);
            boxCollider.size = new Vector2(.1f, 22f);
            text.rectTransform.localPosition = new Vector3(26, -730, 0);
            text.rectTransform.localScale = new Vector3(1.3f, .1f, 1f);
        }

        public void Scale()
        {
            if (needsScaling)
            {
                transform.localScale = originalScale;
            }
        }

        public Color GetColor()
        {
            return rend.color;
        }

        public BookmarkUIColor GetUIColor()
        {
            return myUIColor;
        }

        public void SetColor(Color col, BookmarkUIColor uiCol)
        {
            rend.color = col;
            mini.GetComponent<SpriteRenderer>().color = col;
            myUIColor = uiCol;
        }

        public void Destroy()
        {
            Destroy(mini.gameObject);
            Destroy(this.gameObject);
        }
    }

}