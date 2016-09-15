using UnityEngine;
using System.Collections.Generic;
using UniRx;

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


		#region Unity callbacks
		//======================================================================

		void Awake () {
			PlayerNum = 2;
			unitsList = new List<SelectableCtrl> ();
			foreach (Transform child in transform) {
				if (child.gameObject.activeInHierarchy) {
					// Add to units list.
					SelectableCtrl sel = child.GetComponent<SelectableCtrl> ();
					unitsList.Add (sel);
				}
			}
		}

		void Start ()
		{
			// Remove from units list if killed.
			foreach (SelectableCtrl sel in unitsList) {
				// Create local reference for the closure.
				SelectableCtrl sel2 = sel;
				if (sel.att) {
					sel.att.CurrentHp
						.TakeUntilDestroy (gameObject)
						.TakeWhile (hp => hp > 0)
						.Subscribe (_ => {}, () => {
							unitsList.Remove (sel2); 
						});
				}
			}
		}

		#endregion
	}
}
