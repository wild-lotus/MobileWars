using UnityEngine;
using System.Collections;

namespace EfrelGames
{
	/// <summary>
	/// Controller for a building.
	/// </summary>
	public class BuildingCtrl : SelectableCtrl
	{
		#region Public Configurable fields and properties
		//======================================================================

		/// <summary>
		/// The activities this building can perform.
		/// </summary>
		public  Activity[] activities;

		#endregion


		#region Private fields
		//======================================================================

		/// <summary>
		/// Queue of activities currently ordered to this building.
		/// </summary>
		private ActivityQueue actQ;

		#endregion


		#region Unity callbacks
		//======================================================================

		protected override void  Awake ()
		{
			base.Awake ();
			actQ = new ActivityQueue ();
			((BuildingView)view).Init (activities, actQ);
		}

		#endregion


		#region Public methods
		//======================================================================

		/// <summary>
		/// Add the specified activity to be performed by this building.
		/// </summary>
		/// <param name="activity">Activity.</param>
		public void Add (Activity activity)
		{
			actQ.Add (activity);
		}

		#endregion
	}
}
