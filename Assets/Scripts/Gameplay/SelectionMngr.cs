using UnityEngine;
using UnityEngine.UI;
using System;
using System.Collections;
using System.Collections.Generic;

using UniRx;
using UniRx.Triggers;

namespace EfrelGames
{
	/// <summary>
	/// Manages the selections performed by the player.
	/// </summary>
	public class SelectionMngr : MonoBehaviour
	{
		#region Constants
		//======================================================================

		private const bool LOG = true;

		#endregion


		#region Public fields and properties
		//======================================================================

		/// <summary>
		/// List of selectables currently selected by the player.
		/// </summary>
		public IList<SelectableCtrl> selectedList;

		#endregion


		#region External references
		//======================================================================
		
		public PlayerCtrl player;
		public GameObject longSelMark;
		public GameObject destFxPrefab;
		
		#endregion


		#region Unity callbacks
		//======================================================================

		void Awake ()
		{
			selectedList = new List<SelectableCtrl> ();

			// Stream to add units to the selection with long selection mark.
			longSelMark.GetComponent<ObservableTriggerTrigger> ()
				.OnTriggerEnterAsObservable ()
				.Subscribe (collider =>
					AddSelection (collider.GetComponent<SelectableCtrl> ())
				);
		}

		#endregion


		#region Public methods
		//======================================================================

		/// <summary>
		/// /// <summary>Set the selection to the given selectable.</summary>
		/// </summary>
		/// <param name="sel">Selectable selected.</param>
		public void SetSelection (SelectableCtrl sel)
		{
			this.ClearSelection ();
			if (sel != null) {
				if (LOG) Debug.Log (name + " Selected " + sel.name);
				sel.Selected = true;
			}
		}

		/// <summary>
		/// /// <summary>Add to the selection the given selectable.</summary>
		/// </summary>
		/// <param name="sel">Selectable added to selection.</param>
		public void AddSelection (SelectableCtrl sel)
		{
			if (sel != null) {
				sel.Selected = true;
			}
		}

		/// <summary>
		/// Set an action for the current selection. 
		/// </summary>
		/// <param name="target">Target selectable.</param>
		/// <param name="groundPos">Ground position.</param>
		public void SetAction (SelectableCtrl target, Vector3 groundPos)
		{
			if (target == null || target.PlayerNum == PlayerCtrl.PlayerNum) {
				// It is a move action.
				this.SpawnDestFx (groundPos);
				foreach (SelectableCtrl sel in selectedList) {
					sel.Move (groundPos);
				}
			} else {
				// It is an attack action.
				target.view.selMarkAnim.SetTrigger ("Targeted");
				this.SpawnDestFx (target.trans.position);
				foreach (SelectableCtrl sel in selectedList) {
					sel.Attack (target);
				}
			}
		}

		/// <summary>
		/// Clears the selection.
		/// </summary>
		private void ClearSelection ()
		{
			foreach (SelectableCtrl oldSel in new List<SelectableCtrl> (selectedList)) {
				oldSel.Selected = false;
			}
		}

		/// <summary>
		/// Adds all units to the selection.
		/// </summary>
		public void AllUnits ()
		{
			this.ClearSelection ();
			foreach (SelectableCtrl sel in player.unitsList) {
				sel.Selected = true;
			}
		}

		/// <summary>
		/// Begins the long selelection of units.
		/// </summary>
		/// <param name="pos">Input position.</param>
		public void BeginLongSel (Vector3 pos)
		{
			this.ClearSelection ();
			longSelMark.SetActive (true);
			longSelMark.transform.position = pos + Vector3.up * 0.1f;
		}

		/// <summary>
		/// Handle the long selelection of units.
		/// </summary>
		/// <param name="pos">Input position.</param>
		public void LongSel (Vector3 pos)
		{
			longSelMark.transform.position = pos + Vector3.up * 0.1f;
		}

		/// <summary>
		/// Ends the long selelection of units.
		/// </summary>
		/// <param name="pos">Input position.</param>
		public void EndLongSel (Vector3 screenPos = new Vector3())
		{
			longSelMark.SetActive (false);
		}

		#endregion


		#region Private methods
		//======================================================================

		/// <summary>
		/// Spawns the destination effect.
		/// </summary>
		/// <param name="pos">Position to spawn the effect.</param>
		private void SpawnDestFx (Vector3 pos)
		{
			GameObject destFxGo = Instantiate (
				destFxPrefab, pos + Vector3.up, Quaternion.Euler(90, 0, 0)
			) as GameObject;
			Destroy (destFxGo, 1);
		}

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
