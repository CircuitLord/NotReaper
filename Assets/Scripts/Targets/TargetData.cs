using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using NotReaper.Models;
using NotReaper.Grid;

namespace NotReaper.Targets {

	public class PathBuilderData {
		public TargetBehavior behavior;
		public TargetVelocity velocity;
		public TargetHandType handType;
		public int interval = 4;
		public float initialAngle = 0.0f;
		public float angle = 0.0f;
		public float angleIncrement = 0.0f;
		public float stepDistance = 0.5f;
		public float stepIncrement = 0.0f;
		public List<TargetData> generatedNotes = new List<TargetData>();
		public bool createdNotes = false;
	}

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

		public PathBuilderData pathBuilderData;
		
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


		public bool supportsBeatLength
		{
			get { return _behavior == TargetBehavior.Hold || _behavior == TargetBehavior.NR_Pathbuilder; }
		}

		public event Action<float, float> PositionChangeEvent = delegate {};
		public event Action<float> BeatTimeChangeEvent = delegate {};
		public event Action<float> BeatLengthChangeEvent = delegate {};
		public event Action<TargetVelocity> VelocityChangeEvent = delegate {};
		public event Action<TargetHandType> HandTypeChangeEvent = delegate {};
		public event Action<TargetBehavior> BehaviourChangeEvent = delegate {};
	}
}