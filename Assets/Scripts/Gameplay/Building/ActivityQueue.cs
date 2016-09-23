using UnityEngine;
using System.Collections.Generic;

namespace EfrelGames
{
	/// <summary>
	/// Queue of activities ordered to a building.
	/// </summary>
	public class ActivityQueue {

		#region Public fields and properties
		//======================================================================

		/// <summary>
		/// The list activities to be performed ordered as a queue.
		/// </summary>
		public List<Activity> Activities { get; private set; }

		/// <summary>
		/// The progress of the first activity of the queue, currently being performed.
		/// </summary>
		/// <value>The progress.</value>
		public float Progress { get; set; }

		#endregion


		#region Constructor
		//======================================================================

		public ActivityQueue ()
		{
			Activities = new List<Activity> ();
		}

		#endregion


		#region Public methods
		//======================================================================

		/// <summary>
		/// Add the specified activity to the queue.
		/// </summary>
		/// <param name="activity">Activity.</param>
		public void Add (Activity activity)
		{
			Activities.Add (activity);
		}

		#endregion


		#region Private methods
		//======================================================================

		/// <summary>
		/// Perform actions to be taken once an activity is completed.
		/// </summary>
		private void ActivityCompleted ()
		{
			Activity activity = Activities [0];
			Activities.RemoveAt (0);
			Progress = 0;
			activity.effect.Invoke ();
		}

	//	public void Cancel (Activity activity) {
	//		Activities.
	//	}

		#endregion

	}
}
