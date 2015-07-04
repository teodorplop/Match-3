using UnityEngine;
using System;
using System.Collections;

[RequireComponent(typeof(StateMachineHandler))]
public abstract class StateMachine : MonoBehaviour {
	private StateMachineHandler _stateMachineHandler;
	public StateMachineHandler stateMachineHandler {
		get {
			if (_stateMachineHandler == null) {
				_stateMachineHandler = GetComponent<StateMachineHandler>();
			}
			if (_stateMachineHandler == null) {
				Debug.LogError("StateMachineHandler component not found.");
			}

			return _stateMachineHandler;
		}
	}

	public Enum GetState() {
		return stateMachineHandler.currentState;
	}
	protected void Initialize<T>() {
		stateMachineHandler.Initialize<T>(this);
	}
	protected void ChangeState(Enum newState) {
		stateMachineHandler.ChangeState(newState);
	}
}
