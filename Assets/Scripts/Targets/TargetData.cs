using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using NotReaper.Models;
using NotReaper.Grid;
using NotReaper.Tools.ChainBuilder;
using UnityEngine.Events;
using UnityEngine.Serialization;

namespace NotReaper.Targets {

	[Serializable]
	public class PathBuilderData {
		
		[SerializeField]
		private TargetBehavior _behavior;

		[SerializeField]
		private TargetVelocity _velocity;

		[SerializeField]
		private TargetHandType _handType;

		[SerializeField]
		private int _interval = 4;

		[SerializeField]
		private float _initialAngle = 0.0f;

		[SerializeField]
		private float _angle = 0.0f;

		[SerializeField]
		private float _angleIncrement = 0.0f;

		[SerializeField]
		private float _stepDistance = 0.5f;

		[SerializeField]
		private float _stepIncrement = 0.0f;

		public TargetBehavior behavior {
			get { return _behavior; }
			set { _behavior = value; RecalculateChain(); }
		}
		public TargetVelocity velocity {
			get { return _velocity; }
			set { _velocity = value; RecalculateChain(); }
		}
		public TargetHandType handType {
			get { return _handType; }
			set { _handType = value; RecalculateChain(); }
		}
		public int interval {
			get { return _interval; }
			set { _interval = value; RecalculateChain(); }
		}
		public float initialAngle {
			get { return _initialAngle; }
			set { _initialAngle = value; InitialAngleChangedEvent(); RecalculateChain(); }
		}
		public float angle {
			get { return _angle; }
			set { _angle = value; RecalculateChain(); }
		}
		public float angleIncrement {
			get { return _angleIncrement; }
			set { _angleIncrement = value; RecalculateChain(); }
		}
		public float stepDistance {
			get { return _stepDistance; }
			set { _stepDistance = value; RecalculateChain(); }
		}
		public float stepIncrement {
			get { return _stepIncrement; }
			set { _stepIncrement = value; RecalculateChain(); }
		}

		public void RecalculateChain() {
			RecalculateEvent();
		}

		public void OnFinishRecalculate() {
			RecalculateFinishedEvent();
		}

		[NonSerialized] public Action RecalculateEvent = delegate {};
		[NonSerialized] public Action RecalculateFinishedEvent = delegate {};
		[NonSerialized] public Action InitialAngleChangedEvent = delegate {};
		[NonSerialized] public List<TargetData> generatedNotes = new List<TargetData>();
		[NonSerialized] public bool createdNotes = false;

		public void Copy(PathBuilderData data) {
			behavior = data.behavior;
			velocity = data.velocity;
			handType = data.handType;
			interval = data.interval;
			initialAngle = data.initialAngle;
			angle = data.angle;
			stepDistance = data.stepDistance;
			stepIncrement = data.stepIncrement;
		}
	}

	public class TargetData {
		private static uint TargetDataId = 0;

		public static uint GetNextId() { return TargetDataId++; }
		public uint ID {get; private set; }

		public TargetData() {
			ID = GetNextId();

			beatLength = 0.25f;
			velocity = TargetVelocity.Standard;
			handType = TargetHandType.Left;
			behavior = TargetBehavior.Standard;
		}

		public TargetData(Cue cue, float offset) {
			ID = GetNextId();

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
			ID = GetNextId();

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