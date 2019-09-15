using System.Linq;
using UnityEngine;
using NotReaper.Targets;

namespace NotReaper.UserInput {
	public class MouseUtil {
		public static TargetIcon[] IconsUnderMouse(LayerMask mask) {
			Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
			ray.origin = new Vector3(ray.origin.x, ray.origin.y, -1.7f);
			ray.direction = Vector3.forward;
			//TODO: Tweakable selection distance for raycast
			//Box collider size currently 2.2

			var results = Physics.RaycastAll(ray, 3.4f, mask);
			return results
				.Where(result => result.transform.GetComponent<TargetIcon>() != null)
				.OrderBy(result => {
					// sort by the distance from the centre of the timeline (closest = 0)
					var target = result.transform.GetComponent<TargetIcon>();

					//var isTimeline = (result.transform.position.y > 4.18);     // yes, this is how we're determining this. I'm sorry.
					bool isTimeline = !target.isGridIcon;						//It's okay, I fixed it. We can show our faces in public again.
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