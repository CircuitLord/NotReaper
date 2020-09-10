using System.Linq;
using UnityEngine;
using NotReaper.Targets;
using System.Collections.Generic;
using NotReaper.Timing;

namespace NotReaper.UserInput {
	public class MouseUtil {
		public static TargetIcon[] IconsUnderMouse(Timeline timeline) {
			//if (!EditorInput.isOverGrid) return new TargetIcon[0];
			Vector3 cameraPoint = Camera.main.ScreenToWorldPoint(Input.mousePosition);

			Vector2 point = new Vector2(cameraPoint.x, cameraPoint.y);
			List<TargetIcon> targetsUnderMouse = new List<TargetIcon>();
			foreach(Target target in Timeline.loadedNotes) {
				target.AddTargetIconsCloseToPointAtTime(targetsUnderMouse, Timeline.time, point);
			}

			return targetsUnderMouse
			.Where(result => result.transform.GetComponent<TargetIcon>() != null && !result.transform.GetComponent<TargetIcon>().target.transient)
			.OrderBy(result => {
				// sort by the distance from the centre of the timeline (closest = 0)
				var target = result.transform.GetComponent<TargetIcon>();
				bool isTimeline = target.location == TargetIconLocation.Timeline;
				var distance = isTimeline ?
					Mathf.Abs(target.transform.localPosition.x) :
					Mathf.Abs(target.transform.position.z);
				return distance;
			})
			.Select(result => result.transform.GetComponent<TargetIcon>())
			.ToArray();
		}
	}
}