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
		private int _tapCount = 0;
		private float _lastTapTime = 0f;
		private Vector3 _lastTapPos;

#if  UNITY_EDITOR || UNITY_STANDALONE

		private bool IsDrag { get { return Input.GetMouseButton(1); } }

		private bool IsEndDrag { get { return Input.GetMouseButtonUp(1); } }

		private Vector3 DragPos { get { return Input.mousePosition; } }

		private bool IsSelDown { get { return Input.GetMouseButtonDown (0); } }

		private bool IsSelHold { get { return Input.GetMouseButton (0); } }

		private bool IsSelUp { get { return Input.GetMouseButtonUp (0); } }

		private Vector3 SelPos { get { return Input.mousePosition; } }

#elif UNITY_ANDROID || UNITY_IOS

		private bool IsDrag { get { return Input.touchCount == 2; } }

		private bool IsEndDrag { get { return Input.touchCount == 0; } }

		private Vector3 DragPos {
			get {
				return (
					Input.GetTouch (0).position + Input.GetTouch (1).position
				) / 2;
			}
		}

		private bool IsSelDown {
			get { 
				return Input.touchCount == 1 
					&& Input.GetTouch(0).phase == TouchPhase.Began; 
			}
		}

		private bool IsSelHold { get { return Input.touchCount == 1; } }

		private bool IsSelUp {
			get { 
				return Input.touchCount == 1 
					&& Input.GetTouch(0).phase == TouchPhase.Ended;
			}
		}

		private Vector3 SelPos { get { return Input.mousePosition; } }

#endif

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
			this.CheckDrag ();
		}

		#endregion


		#region Private methods
		//======================================================================

		private void CheckSelection ()
		{
			if (IsSelDown) {
				print ("down");
				_pointerDownTime = Time.time;
			} 
			if (IsSelHold) {
				print ("hold");

				if (_action == Action.LongSel) {
					print ("long sel");
				} else if (_action == Action.None && (Time.time - _pointerDownTime) > LONG_SEL_TIME) {
					_action = Action.LongSel;
				}

			}
			if (IsSelUp) {
				print ("up");
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

		private void CheckDrag ()
		{
			if (_action == Action.None && IsDrag) {
				_action = Action.Drag;
				_camDrag.BeginDrag (Input.mousePosition);
			} else if (_action == Action.Drag) {
				if (IsDrag) {
					_camDrag.Drag (DragPos);
				} else if (IsEndDrag) {
					_action = Action.None;
					_camDrag.EndDrag (Input.mousePosition); 
				}
			}
		}

		#endregion
	}
}
