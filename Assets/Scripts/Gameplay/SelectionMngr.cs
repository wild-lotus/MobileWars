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


		#region External references
		//======================================================================
		
		public PlayerCtrl player;
		public GameObject longSelMark;
		
		#endregion


		#region Unity callbacks
		//======================================================================

		void Awake ()
		{
			selectedList = new List<SelectableCtrl> ();
		}

		#endregion


		#region Public methods
		//======================================================================

		/// <summary>Remove all selectables from selection list.</summary>
		public void UpdateSelection (SelectableCtrl sel)
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


		public void AllUnits ()
		{
			foreach (SelectableCtrl sel in player.selList) {
				sel.Selected = true;
			}
		}

		public void BeginLongSel (Vector3 pos)
		{
			longSelMark.SetActive (true);
			longSelMark.transform.position = pos + Vector3.up * 0.1f;
		}

		public void LongSel (Vector3 pos)
		{
			longSelMark.transform.position = pos + Vector3.up * 0.1f;
		}

		public void EndLongSel (Vector3 screenPos = new Vector3())
		{
			longSelMark.SetActive (false);
		}

		#endregion


		#region Private methods
		//======================================================================

		////

		#endregion


		#region Context menu methods
		//======================================================================

		[ContextMenu ("Set References")]
		public void SetReferences ()
		{
			player = GameObject.Find ("Player").GetComponent<PlayerCtrl> ();
			longSelMark = transform.Find ("LongSelMark").gameObject;
		}

		#endregion
	}
}
