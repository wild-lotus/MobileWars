using UnityEngine;
using UnityEngine.UI;
using System;
using System.Collections;
using UniRx;

namespace EfrelGames
{
	/// <summary>
	/// Behaviour to move the camera around the gameplay battlefield.
	/// </summary>
	public class CamDragMngr : MonoBehaviour
	{
		#region Public configurable fields
		//======================================================================

		[Tooltip("Speed for the camera to move around the map.")]
		public float dragSpeed = 40f;

		#endregion


		#region Cached fields
		//======================================================================

		/// <summary>.Main camera.</summary>
		private Camera _cam;
		/// <summary>.Main camera transform.</summary>
		private Transform _camTrans;
		/// <summary>.Vertical direction to move the camera around.</summary>
		private Vector3 _camV;
		/// <summary>.Horizontal direction to move the camera around.</summary>
		private Vector3 _camH;

		#endregion


		#region Private vars
		//======================================================================

		/// <summary>.
		/// Last User input position on screen for camera dragging.
		/// </summary>
		private Vector3 _lastDragPos = Vector3.zero;

		#endregion


		#region Unity callbacks
		//======================================================================

		void Awake ()
		{
			_cam = Camera.main;
			_camTrans = _cam.transform;
			_camV = Vector3.ProjectOnPlane (_camTrans.forward, Vector3.up).normalized;
			_camH = _camTrans.TransformDirection (Vector3.right);
		}

		#endregion


		#region Public methods
		//======================================================================

		public void BeginDrag (Vector3 pos)
		{
			_lastDragPos = pos;
		}

		public void Drag (Vector3 pos)
		{
			if (pos != _lastDragPos) {
				// Translate the main cam according to the drag.
				Vector3 dPos = pos - _lastDragPos;
				Vector3 translation = (-_camV * dPos.y - _camH * dPos.x)
					* dragSpeed / Screen.height;
				_camTrans.Translate (translation, Space.World);
				_lastDragPos = pos;
			}
		}

//		public void EndDrag (Vector3 pos)
//		{
//		}

		#endregion


		#region Private methods
		//======================================================================



		#endregion
	}
}
