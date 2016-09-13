using UnityEngine;
using UnityEngine.UI;
using System;
using System.Collections;
using System.Collections.Generic;

using UniRx;
using UniRx.Triggers;

namespace EfrelGames
{

	public class SelectionMngr : MonoBehaviour
	{
		#region Constants
		//======================================================================

		private const bool LOG = true;

		#endregion


		#region Public fieds and properties
		//======================================================================

		public IList<SelectableCtrl> selectedList;

		#endregion


		#region Cached fields
		//======================================================================

		private Camera _cam;

		#endregion


		#region External references
		//======================================================================
		
		public PlayerCtrl player;
		
		#endregion


		#region Unity callbacks
		//======================================================================

		void Awake ()
		{
			selectedList = new List<SelectableCtrl> ();
			// Cache components
			_cam = Camera.main;
		}

		#endregion


		#region Public methods
		//======================================================================

		/// <summary>
		/// User selected something on the screen
		/// </summary>
		/// <param name="pos">Position on screen.</param>
		/// <param name="tapCount">Number of taps of this selection.</param>
		public void Select (Vector3 pos, int tapCount)
		{
			RaycastHit hit;
			if (Physics.Raycast (_cam.ScreenPointToRay (pos), out hit)) {
				if (tapCount == 1) {
					this.UpdateSelection (hit.collider.GetComponent <SelectableCtrl> ());
				} else {
					// Double tap, setting action target
					if (selectedList.Count > 0) {
						if (LOG) Debug.Log (name + " Action detected on " + hit.collider.name);
						foreach (SelectableCtrl sel in selectedList) {
							sel.ActionSelect (hit.collider.gameObject, hit.point);
						}
					} else {
						if (LOG) Debug.Log (name + " Action detected but nothing selected.");
					}
				}
			}
		}

		public void AllUnits ()
		{
			foreach (SelectableCtrl sel in player.selList) {
				sel.Selected = true;
			}
		}

		#endregion


		#region Private methods
		//======================================================================

		/// <summary>Remove all selectables from selection list.</summary>
		private void UpdateSelection (SelectableCtrl sel)
		{
			foreach (SelectableCtrl oldSel in new List<SelectableCtrl> (selectedList)) {
				if (oldSel != sel) {
					oldSel.Selected = false;
				}
			}
			if (sel != null) {
				if (!sel.Selected) {
					if (LOG) Debug.Log (name + " Selected " + sel.name);
					sel.Selected = true;
				} else {
					if (LOG) Debug.Log (name + " Reselected " + sel.name);
				}
			}else {
				if (LOG) Debug.Log (name + " Selected empty.");
			}
		}

		#endregion


		#region Context menu methods
		//======================================================================

		[ContextMenu ("Set References")]
		public void SetReferences ()
		{
			player = GameObject.Find ("Player").GetComponent<PlayerCtrl> ();
		}

		#endregion
	}
}
