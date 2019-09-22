using System;
using UnityEngine;
using NotReaper.Models;
using NotReaper.Grid;

namespace NotReaper.Targets {

	public class TargetData {
		public TargetData() {
			beatLength = 0.25f;
			velocity = TargetVelocity.Standard;
			handType = TargetHandType.Left;
			behavior = TargetBehavior.Standard;
		}

		public TargetData(Cue cue, float offset) {
			Vector2 pos = NotePosCalc.PitchToPos(cue);
			x = pos.x;
			y = pos.y;
			beatTime = (cue.tick - offset) / 480f;
			beatLength = cue.tickLength;
			velocity = cue.velocity;
			handType = cue.handType;
			behavior = cue.behavior;
		}
		
		public TargetData(TargetData data) {
			Copy(data);
		}

		public void Copy(TargetData data) {
			x = data.x;
			y = data.y;
			beatTime = data.beatTime;
			beatLength = data.beatLength;
			velocity = data.velocity;
			handType = data.handType;
			behavior = data.behavior;
		}

		private float _x;
		private float _y;
		private float _beatTime;
		private float _beatLength;
		private TargetVelocity _velocity;
		private TargetHandType _handType;
		private TargetBehavior _behavior;

		
		public float x
		{
			get { return _x; }
			set { _x = value; PositionChangeEvent(x, y); }
		}

		public float y
		{
			get { return _y; }
			set { _y = value; PositionChangeEvent(x, y); }
		}

		public Vector2 position {
			get { return new Vector2(x, y); }
			set { _x = value.x; _y = value.y; PositionChangeEvent(x, y); }
		}

		public float beatTime
		{
			get { return _beatTime; }
			set { _beatTime = value; BeatTimeChangeEvent(beatTime); }
		}

		public float beatLength
		{
			get { return _beatLength; }
			set { _beatLength = value; BeatLengthChangeEvent(beatLength); }
		}

		public TargetVelocity velocity
		{
			get { return _velocity; }
			set { _velocity = value; VelocityChangeEvent(velocity); }
		}
		public TargetHandType handType
		{
			get { return _handType; }
			set { _handType = value; HandTypeChangeEvent(handType); }
		}
		public TargetBehavior behavior
		{
			get { return _behavior; }
			set { _behavior = value; BehaviourChangeEvent(behavior); }
		}


		public event Action<float, float> PositionChangeEvent = delegate {};
		public event Action<float> BeatTimeChangeEvent = delegate {};
		public event Action<float> BeatLengthChangeEvent = delegate {};
		public event Action<TargetVelocity> VelocityChangeEvent = delegate {};
		public event Action<TargetHandType> HandTypeChangeEvent = delegate {};
		public event Action<TargetBehavior> BehaviourChangeEvent = delegate {};
	}
}