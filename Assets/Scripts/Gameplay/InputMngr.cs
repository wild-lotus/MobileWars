using UnityEngine;
using UnityEngine.EventSystems;
using System.Collections;

namespace EfrelGames
{
	/// <summary>
	/// Input manager.
	/// </summary>
	[RequireComponent (typeof(CamDragMngr))]
	public class InputMngr : MonoBehaviour
	{
		#region Enumerators
		//======================================================================

		/// <summary>
		/// Different types of available actions with input.
		/// </summary>
		private enum Action
		{
			None,
			Drag,
			LongSel,
			AllUnits
		}

		#endregion


		#region Constants
		//======================================================================

		private const float TAP_TIME = 0.25f;
		private const float LONG_SEL_TIME = 0.5f;
		public const int GROUND_LAYER = 8;
		public const int SELECTABLE_LAYER = 9;

		#endregion


		#region Cached fields
		//======================================================================

		private Camera _cam;
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

		private Vector3 SelPos { get { return Input.mousePosition; } }

		#if  (UNITY_EDITOR || UNITY_STANDALONE)

		private bool IsSelDown { get { return Input.GetMouseButtonDown (0); } }

		private bool IsSelHold { get { return Input.GetMouseButton (0); } }

		private bool IsSelUp { get { return Input.GetMouseButtonUp (0); } }

		private bool IsAllUnitsDown { get { return Input.GetKeyDown("a"); } }

		private bool IsAllUnitsUp { get { return Input.GetKeyUp("a"); } }

		#elif (UNITY_ANDROID || UNITY_IOS)

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

		private bool IsAllUnitsDown { get { return Input.touchCount == 3; } }

		private bool IsAllUnitsUp { get { return Input.touchCount == 0; } }

		#endif

		#endregion


		#region Unity callbacks
		//======================================================================

		void Awake ()
		{
			// Configure input.
			Input.simulateMouseWithTouches = false;
			Input.backButtonLeavesApp = true;
			//Cache components
			_cam = Camera.main;
			_camDrag = GetComponent<CamDragMngr> ();
			_selMngr = GetComponent<SelectionMngr> ();
		}

		void Update ()
		{
			if (!EventSystem.current.IsPointerOverGameObject ()) {
				this.CheckSelection ();
				this.CheckAllUnits ();
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
				if (_action == Action.Drag) {
					_camDrag.Drag (SelPos);
				} else if (_action == Action.LongSel) {
					_selMngr.LongSel (this.ScreenPointToGround (SelPos));
				} else if (_action == Action.None) {
					if (SelPos != _pointerDownPos) {
						_action = Action.Drag;
						_camDrag.BeginDrag (SelPos);
					} else if ((Time.time - _pointerDownTime) > LONG_SEL_TIME) {
						_action = Action.LongSel;
						_selMngr.BeginLongSel (this.ScreenPointToGround (SelPos));
					}
				}
			}
			if (IsSelUp) {
				if (_action == Action.None) {
					_tapCount++;
					_lastTapPos = SelPos;
					_lastTapTime = Time.time;
				} else {
					if (_action == Action.LongSel) {
						_selMngr.EndLongSel ();
					}
					_action = Action.None;
				}
			}

			if (_tapCount > 0) {
				if (Time.time - _lastTapTime > TAP_TIME) {
					this.TapToSelectionAction (_lastTapPos, _tapCount);
					_tapCount = 0;
				}
			}
		}

		private void CheckAllUnits ()
		{
			if (_action == Action.None && IsAllUnitsDown) {
				_action = Action.AllUnits;
				_selMngr.AllUnits ();
			} else if (_action == Action.AllUnits && IsAllUnitsUp) {
				_action = Action.None;
			}
		}
			
		/// <summary>
		/// Turn a screen point into a ground world position.
		/// </summary>
		/// <returns>World position on the gorund.</returns>
		/// <param name="screenPos">Screen position.</param>
		private Vector3 ScreenPointToGround (Vector3 screenPos)
		{
			RaycastHit hit;
			int layerMask = 1 << GROUND_LAYER;
			if (Physics.Raycast (_cam.ScreenPointToRay (screenPos), out hit, 
				100f, layerMask)) {
				return hit.point;
			}
			return Vector3.zero;
		}

		/// <summary>
		/// Turn a screen point into a selectable if successful hit.
		/// </summary>
		/// <returns>The selectable hit.</returns>
		/// <param name="screenPos">Screen position.</param>
		private SelectableCtrl ScreenPointToSelectable (Vector3 screenPos)
		{
			RaycastHit hit;
			int layerMask = 1 << SELECTABLE_LAYER;
			if (Physics.Raycast (_cam.ScreenPointToRay (screenPos), out hit, 
				100f, layerMask)) {
				return hit.collider.GetComponent <SelectableCtrl> ();
			}
			return null;
		}
			
		/// <summary>
		/// Turn tapping input into game functions: selection or action
		/// </summary>
		/// <param name="screenPos">Screen position.</param>
		/// <param name="tapCount">Number of taps.</param>
		private void TapToSelectionAction (Vector3 screenPos, int tapCount) {
			SelectableCtrl target = this.ScreenPointToSelectable(screenPos);
			if (tapCount == 1) {
				_selMngr.SetSelection (target);
			} else {
				_selMngr.SetAction(target, this.ScreenPointToGround (screenPos));
			}
		}

		#endregion
	}
}
