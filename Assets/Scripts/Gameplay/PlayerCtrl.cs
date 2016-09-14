using UnityEngine;
using System.Collections.Generic;

namespace EfrelGames {
	public class PlayerCtrl : MonoBehaviour {

		#region Cached components
		//======================================================================

		public IList<SelectableCtrl> selList;

		#endregion


		#region Context menu methods
		//======================================================================

		void Awake () {
			selList = new List<SelectableCtrl> ();
			foreach (Transform child in transform) {
				if (child.gameObject.activeInHierarchy) {
					selList.Add (child.GetComponent<SelectableCtrl> ());
				}
			}
		}

		#endregion
	}
}
