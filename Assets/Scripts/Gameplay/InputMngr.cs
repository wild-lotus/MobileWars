using UnityEngine;
using System.Collections;

namespace EfrelGames
{
	[RequireComponent (typeof(CamDragMngr))]
	public class InputMngr : MonoBehaviour
	{
		#region Enumerators
		//======================================================================

		private enum Action
		{
			None,
			Drag,
			LongSel
		}

		#endregion


		#region Constants
		//======================================================================

		private const float TAP_TIME = 0.25f;
		private const float LONG_SEL_TIME = 1f;

		#endregion


		#region Cached fields
		//======================================================================

		private CamDragMngr _camDrag;
		private SelectionMngr _selMngr;

		#endregion


		#region Private fields and propeties
		//======================================================================

		private Action _action = Action.None;
		private float _pointerDownTime = 0f;
		private Vector3 _pointerDownPos = Vector3.zero;
		private int _tapCount = 0;
		private float _lastTapTime = 0f;
		private Vector3 _lastTapPos;

#if  UNITY_EDITOR || UNITY_STANDALONE

		private bool IsAllUnits { get { return Input.GetKeyUp("a"); } }

#elif UNITY_ANDROID || UNITY_IOS

		private bool IsAllUnits { get { return Input.touchCount == 2; } }

#endif

		private bool IsSelDown { get { return Input.GetMouseButtonDown (0); } }

		private bool IsSelHold { get { return Input.GetMouseButton (0); } }

		private bool IsSelUp { get { return Input.GetMouseButtonUp (0); } }

		private Vector3 SelPos { get { return Input.mousePosition; } }

		#endregion


		#region Unity callbacks
		//======================================================================

		void Awake ()
		{
			_camDrag = GetComponent<CamDragMngr> ();
			_selMngr = GetComponent<SelectionMngr> ();
		}

		void Update ()
		{
			this.CheckSelection ();
			if (IsAllUnits) {
				_selMngr.AllUnits ();
			}
		}

		#endregion


		#region Private methods
		//======================================================================

		private void CheckSelection ()
		{
			if (IsSelDown) {
				_pointerDownTime = Time.time;
				_pointerDownPos = SelPos;
			} 
			if (IsSelHold) {
				if (_action == Action.LongSel) {
					// TURUUUU
				} else if (_action == Action.Drag) {
					_camDrag.Drag (SelPos);
				} else if (SelPos != _pointerDownPos) {
					_action = Action.Drag;
					_camDrag.BeginDrag (SelPos);
				} else if ((Time.time - _pointerDownTime) > LONG_SEL_TIME) {
					_action = Action.LongSel;
				}
			}
			if (IsSelUp) {
				if (_action == Action.None) {
					_tapCount++;
					_lastTapPos = SelPos;
					_lastTapTime = Time.time;
				} else {
					_action = Action.None;
				}
			}

			if (_tapCount > 0) {
				if (Time.time - _lastTapTime > TAP_TIME) {
					_selMngr.Select (_lastTapPos, _tapCount);
					_tapCount = 0;
				}
			}
		}

		#endregion
	}
}
