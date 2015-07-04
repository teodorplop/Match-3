using UnityEngine;
using System;
using System.Reflection;
using System.Collections;
using System.Collections.Generic;

public class StateMachineHandler : MonoBehaviour {
	public class State {
		public Enum state;

		public Func<IEnumerator> Enter = DoNothingCoroutine;
		public Func<IEnumerator> Exit = DoNothingCoroutine;
		public Action<Vector3> InputHandler = DoNothingInputHandler;

		public State(Enum state) {
			this.state = state;
		}

		public static IEnumerator DoNothingCoroutine() {
			yield return null;
		}
		public static void DoNothingInputHandler(Vector3 vector) {
		}
		public static void DoNothing() {
		}
	}

	private State _currentState;
	public Enum currentState {
		get { return _currentState.state; }
	}
	private Dictionary<Enum, State> _stateLookup;
	public void Initialize<T>(StateMachine targetMachine) {
		// Create connections between enum values and states
		var values = Enum.GetValues (typeof(T));
		_stateLookup = new Dictionary<Enum, State> ();
		for (int i = 0; i < values.Length; ++i) {
			State state = new State((Enum)values.GetValue(i));
			_stateLookup.Add(state.state, state);
		}

		// Get targetMachine methods
		var methods = targetMachine.GetType().GetMethods(BindingFlags.Instance | BindingFlags.DeclaredOnly | BindingFlags.Public | BindingFlags.NonPublic);
		var separator = "_".ToCharArray();
		foreach (var method in methods) {
			var parameters = method.Name.Split(separator);
			// Ignore methods with no separators in name
			if (parameters.Length <= 1) {
				continue;
			}

			// Get the enum value of the method
			Enum stateEnum = null;
			try {
				stateEnum = (Enum) Enum.Parse(typeof(T), parameters[0]);
			} catch (ArgumentException) {
				Debug.LogError("Could not parse method: " + method.Name);
			}
			if (stateEnum == null) {
				continue;
			}

			// Get the state connected with the enum value
			State state = _stateLookup[stateEnum];
			switch (parameters[1]) {
			case "Enter":
				state.Enter = CreateDelegate<Func<IEnumerator>>(method, targetMachine);
				break;
			case "Exit":
				state.Exit = CreateDelegate<Func<IEnumerator>>(method, targetMachine);
				break;
			case "InputHandler":
				state.InputHandler = CreateDelegate<Action<Vector3>>(method, targetMachine);
				break;
			}
		}
	}

	public void ChangeState(Enum newState) {
		if (_stateLookup == null) {
			Debug.LogError("States have not been configured. Please call Initialize.");
			return;
		}
		if (!_stateLookup.ContainsKey (newState)) {
			Debug.LogError("State " + newState.ToString() + " does not exist.");
			return;
		}

		var nextState = _stateLookup [newState];
		StartCoroutine(StateTransition(newState));
	}
	IEnumerator StateTransition(State nextState) {
		if (_currentState != null) {
			yield return StartCoroutine(_currentState.Exit);
		}
		_currentState = nextState;
		yield return StartCoroutine (nextState.Enter);
	}

	T CreateDelegate<T>(MethodInfo method, System.Object target) where T : class {
		var del = Delegate.CreateDelegate (typeof(T), target, method) as T;
		if (del == null) {
			Debug.LogError("Unable to create delegate for method: " + method.Name);
		}
		return del;
	}
}
