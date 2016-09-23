using UnityEngine;
using System.Collections;

namespace EfrelGames
{
	/// <summary>
	/// View related behaviour of an building.
	/// </summary>
	public class BuildingView : SelectableView {

		#region Extrnal references
		//======================================================================

		public BuildingUI buildingUI;

		#endregion


		#region Private fields
		//======================================================================

		/// <summary>
		/// The activities this building can perform. Reference from ctrl.
		/// </summary>
		private Activity[] _activities = 
			new Activity[BuildingUI.MAX_ACTIVITIES];

		/// <summary>
		/// The queue of activities currently oredered to this building.
		/// Reference from ctrl.
		/// </summary>
		private ActivityQueue _actQ;

		#endregion


		#region Public mehtods
		//======================================================================

		/// <summary>
		/// Initialize this behaviour, setting the needed references.
		/// </summary>
		/// <param name="activities">
		/// Reference to the available Activities of this building.
		/// </param>
		/// <param name="actQ">
		/// Reference to the queue of activities oredered to this building.
		/// </param>
		public void Init (Activity[] activities, ActivityQueue actQ)
		{
			_activities[0] = activities[0];
			_actQ = actQ;
		}

		/// <summary>
		/// View effects when selecting or unselecting this building.
		/// </summary>
		/// <param name="selected">If set to <c>true</c> selected.</param>
		public override void Selected (bool selected) {
			if (selected) {
				print ("BUILDING SELECTED");
				buildingUI.Open (_activities, _actQ);
			} else {
				print ("BUILDING UNSELECTED");
				buildingUI.Close ();
			}
		}

		#endregion


		#region Context menu methods
		//======================================================================

		[ContextMenu ("Set References")]
		public override void SetReferences ()
		{
			base.SetReferences ();
			buildingUI = GameObject.Find ("UI").GetComponentInChildren<BuildingUI> (true);
		}

		#endregion
	}
}