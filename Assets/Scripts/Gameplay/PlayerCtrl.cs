using UnityEngine;
using System.Collections.Generic;

namespace EfrelGames {
	/// <summary>
	/// Controller for a player within a battle.
	/// </summary>
	public class PlayerCtrl : MonoBehaviour {

		#region Cached components
		//======================================================================

		/// <summary>
		/// List of units of this player.
		/// </summary>
		public IList<SelectableCtrl> unitsList;

		public static int PlayerNum { get; private set; }

		#endregion


		#region Context menu methods
		//======================================================================

		void Awake () {
			PlayerNum = 1;
			unitsList = new List<SelectableCtrl> ();
			foreach (Transform child in transform) {
				if (child.gameObject.activeInHierarchy) {
					unitsList.Add (child.GetComponent<SelectableCtrl> ());
				}
			}
		}

		#endregion
	}
}
