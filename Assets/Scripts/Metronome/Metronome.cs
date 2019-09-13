using System.Collections;
using UnityEngine;

namespace NotReaper {


	public class MetronomeOld : MonoBehaviour {

		public int Base;
		public int Step;
		public float BPM;
		public int CurrentStep = 1;
		public int CurrentMeasure;

		private float interval;
		private float nextTime;

		public AudioSource aud;

		private void Start() {
			StartMetronome();
		}


		public void StartMetronome() {
			StopCoroutine(DoTick());
			CurrentStep = 1;
			var multiplier = Base / 4f;
			var tmpInterval = 60f / BPM;
			interval = tmpInterval / multiplier;
			nextTime = Time.time; // set the relative time to now
			StartCoroutine("DoTick");
		}

		IEnumerator DoTick() // yield methods return IEnumerator
		{
			for (;;) {
				//Debug.Log("bop");
				// do something with this beat
				aud.Play();
				nextTime += interval; // add interval to our relative time
				yield return new WaitForSeconds(nextTime - Time.time); // wait for the difference delta between now and expected next time of hit
				CurrentStep++;
				if (CurrentStep > Step) {
					CurrentStep = 1;
					CurrentMeasure++;
				}
			}
		}
	}
}