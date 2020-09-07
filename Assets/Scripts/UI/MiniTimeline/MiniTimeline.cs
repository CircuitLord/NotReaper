using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using NotReaper.Models;
using NotReaper.Timing;
using NotReaper.UserInput;
using Random = UnityEngine.Random;

namespace NotReaper.UI {
	public class MiniTimeline : MonoBehaviour {
		//bar is 440 pixels long total

		public Transform songPreviewIcon;


		public float mouseClickAreaLength = 12.34f;
		public double barLength;

		public Transform bar;

		public Timeline timeline;
		public GameObject repeaterSectionPrefab;


		public Transform bookmarksParent;

		public GameObject bookmarkPrefab;

		private Camera _mainCamera;


		[HideInInspector] public bool isMouseOver = false;

		private List<GameObject> repeaterSections = new List<GameObject>();

		private void Start() {
			_mainCamera = Camera.main;
		}

		public void SetPercentagePlayed(double percent) {
			double x = barLength * percent;
			x -= barLength / 2;
			bar.localPosition = new Vector3((float) x, 0, 0);
		}


		public void SetPreviewStartPointToCurrent() {
			SetPreviewStartPoint(Timeline.time);
		}

		public void SetPreviewStartPoint(QNT_Timestamp timestamp) {
			Timeline.desc.previewStartSeconds = timeline.TimestampToSeconds(timestamp);

			double percent = timeline.GetPercentPlayedFromSeconds(Timeline.desc.previewStartSeconds);

			double x = barLength * percent;
			x -= barLength / 2;
			songPreviewIcon.localPosition = new Vector3((float) x, 0, 0);
		}


		float prevX = 0f;

		private void OnMouseDrag() {
			var x = _mainCamera.ScreenToWorldPoint(Input.mousePosition).x;

			x -= _mainCamera.transform.position.x;

			if (x == prevX) {
				return;
			}
			else {
				prevX = x;
			}

			//x -= transform.position.x;
			//-4.7 to 4.7
			//9.4 length
			x += mouseClickAreaLength / 2;
			float percent = x / mouseClickAreaLength;

			timeline.JumpToPercent(percent);
		}


		bool timelineWasPlaying = false;

		private void OnMouseDown() {
			if (!timeline.paused) timeline.TogglePlayback();
			timelineWasPlaying = true;
		}

		private void OnMouseUp() {
			timelineWasPlaying = false;
			if (timelineWasPlaying && timeline.paused) timeline.TogglePlayback();
		}


		//public void SetSongPreviewPoint(double percent) {
		//    double x = barLength * percent;
		//    x -= barLength / 2;
		//     songPreviewIcon.localPosition = new Vector3((float)x, 0, 0);
		//}

		private void OnMouseOver() {
			isMouseOver = true;
		}

		private void OnMouseExit() {
			isMouseOver = false;
		}


		/*public void JumpToBookmark(int i) {
		    if (bookmarks[i] == null) return;
		    timeline.JumpToPercent((float)bookmarks[i].GetComponent<Bookmark>().percentBookmark);

		}*/

		/*
		public void SetBookmark(int i) {

		    if (bookmarks[i] != null) {
		        Destroy(bookmarks[i]);
		        bookmarks[i] = null;
		    }

		    bookmarks[i] = Instantiate(bookmarkPrefab, new Vector3(0, 0, 0), Quaternion.identity, bookmarksParent);

		    double percent = timeline.GetPercentagePlayed();
		    double x = barLength * percent;
		    x -= barLength / 2;

		    bookmarks[i].transform.localPosition = new Vector3((float)x, 0, 0);

		    bookmarks[i].GetComponent<Bookmark>().SetIndex(i);
		    bookmarks[i].GetComponent<Bookmark>().percentBookmark = percent;
		}
		*/
        public void AddRepeaterSection(RepeaterSection newSection) {
            var sectionObject = Instantiate(repeaterSectionPrefab, new Vector3(0, 0, 0), Quaternion.identity, transform);
            sectionObject.transform.localPosition = new Vector3((float)(barLength * timeline.GetPercentPlayedFromSeconds(timeline.TimestampToSeconds(newSection.startTime)) - (barLength / 2.0f)), 0, -1.0f);
            var lineRenderer = sectionObject.GetComponent<LineRenderer>();
            
            lineRenderer.startWidth = lineRenderer.endWidth = 0.5f;
            lineRenderer.SetPosition(1, new Vector3((float)(barLength * timeline.GetPercentPlayedFromSeconds(timeline.TimestampToSeconds(newSection.endTime) - timeline.TimestampToSeconds(newSection.startTime))), 0, 0));

            repeaterSections.Add(sectionObject);
            newSection.miniTimelineSectionObj = sectionObject;
        }

        public void RemoveRepeaterSection(RepeaterSection section) {
            repeaterSections.Remove(section.miniTimelineSectionObj);
            GameObject.Destroy(section.miniTimelineSectionObj);
        }


        public void SetBookmark(float miniXPos, float topXPos,  TargetHandType newType, bool useTopXPos, bool addToAudicaFile = false) {
			Bookmark bookmarkMini = Instantiate(bookmarkPrefab, new Vector3(0, 0, 0), Quaternion.identity, bookmarksParent).GetComponent<Bookmark>();
			
			Bookmark bookmarkTop = Instantiate(bookmarkPrefab, Timeline.timelineNotesStatic).GetComponent<Bookmark>();
			
			Color background;

			bookmarkMini.transform.localPosition = new Vector3((float) miniXPos, 0, 0);
			
			

			//bookmarkTop.transform.localScale = new Vector3(0.06f, 0.006f, 0.06f);

			if (!useTopXPos) {
				
				if (newType == TargetHandType.Left) {
					bookmarkTop.transform.position = new Vector3(0, bookmarkTop.transform.position.y - 0.6f, 0);
					background = Color.green;
				}
				else {
					bookmarkTop.transform.position = new Vector3(0, bookmarkTop.transform.position.y - 0.72f, 0);
					background = Color.red;
				}
			}
			else {
				if (newType == TargetHandType.Left) {
					bookmarkTop.transform.localPosition = new Vector3(topXPos, bookmarkTop.transform.localPosition.y - 0.6f, 0);
					background = Color.green;
				}
				else {
					bookmarkTop.transform.localPosition = new Vector3(topXPos, bookmarkTop.transform.localPosition.y - 0.72f, 0);
					background = Color.red;
				}
			}
			

			bookmarkMini.GetComponent<SpriteRenderer>().color = background;
			bookmarkTop.GetComponent<SpriteRenderer>().color = background;
			bookmarkTop.GetComponent<SpriteRenderer>().size = new Vector2(0.07f, 0.1f);
			
			bookmarks.Add(bookmarkMini);
			bookmarks.Add(bookmarkTop);


			if (addToAudicaFile) Timeline.audicaFile.desc.bookmarks.Add(new BookmarkData() {type = newType, xPosMini = miniXPos, xPosTop = bookmarkTop.transform.localPosition.x });
		}

		private List<Bookmark> bookmarks = new List<Bookmark>();

		public void ClearBookmarks(bool deleteInAudica = false) {
			foreach (Bookmark t in bookmarks) {
				Destroy(t.gameObject);
			}
			
			bookmarks.Clear();

			if (deleteInAudica) Timeline.audicaFile.desc.bookmarks.Clear();
		}


		private void Update() {
			bool isCtrlDown = false;
			bool isShiftDown = false;


			if (Input.GetKeyDown(KeyCode.P)) {
				SetPreviewStartPoint(Timeline.time);
			}

			if (Input.GetKeyDown(KeyCode.U)) {
				float percent = timeline.GetPercentagePlayed();
				float x = (float) barLength * (float) percent;
				x -= (float) barLength / 2f;
				SetBookmark(x, 0f, EditorInput.selectedHand, false, true);
			}

			if (Input.GetKey(KeyCode.LeftControl) || Input.GetKey(KeyCode.RightControl)) {
				isCtrlDown = true;
			}
			else {
				isCtrlDown = false;
			}

			if (Input.GetKey(KeyCode.LeftShift) || Input.GetKey(KeyCode.RightShift)) {
				isShiftDown = true;
			}
			else {
				isShiftDown = false;
			}


			if (isShiftDown && isCtrlDown && Input.GetKeyDown(KeyCode.U)) {
				ClearBookmarks(true);
			}


		}
	}

	

	[Serializable]
	public class BookmarkData {
		public TargetHandType type = TargetHandType.Left;
		public float xPosMini = 0.0f;
		public float xPosTop = 0.0f;
	}
}