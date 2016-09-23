using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Assertions;
using System.Collections;

namespace EfrelGames 
{
	/// <summary>
	/// Behaviour for the User Interface of selected buildings.
	/// </summary>
	public class BuildingUI : MonoBehaviour {

		#region Constants
		//======================================================================

		/// <summary>
		/// The maximum amount of activities this UI can display.
		/// </summary>
		public const int MAX_ACTIVITIES = 8;

		#endregion

	
		#region External references
		//======================================================================

		public Button[] buttons;

		#endregion


		#region Public methods
		//======================================================================

		/// <summary>
		/// Open the building UI.
		/// </summary>
		/// <param name="activities">Available activities to display.</param>
		/// <param name="actQ">Current queue of activities to display.</param>
		public void Open (Activity[] activities, ActivityQueue actQ)
		{
			Assert.IsTrue (activities.Length == MAX_ACTIVITIES);
			for (int i = 0; i < MAX_ACTIVITIES; i++) {
				if (activities [i] != null) {
					buttons [i].enabled = true;
					buttons [i].GetComponentInChildren<Text> ().text = activities [i].title;
					buttons [i].onClick.AddListener (activities [i].effect);
				} else {
					buttons [i].enabled = false;
					buttons [i].GetComponentInChildren<Text> ().text = "";
				}
			}
			gameObject.SetActive (true);
		}

		/// <summary>
		/// Close the building UI.
		/// </summary>
		public void Close ()
		{
			gameObject.SetActive (false);
		}

		#endregion


		#region Context menu methods
		//======================================================================

		[ContextMenu ("Set References")]
		public void SetReferences ()
		{
			buttons = GetComponentsInChildren<Button> ();
			Assert.IsTrue (buttons.Length == MAX_ACTIVITIES);
		}

		/// <summary>
		/// Positions the buttons in a circular shape within the avaialbe space.
		/// </summary>
		[ContextMenu ("Position Buttons")]
		public void PositionButtons ()
		{
			RectTransform rTrans = transform as RectTransform;
			
			float radius = (rTrans.rect.width - (rTrans.GetChild (0) as RectTransform).rect.width) / 2;
			float dAngle = 2 * Mathf.PI / rTrans.childCount;
			float angle = dAngle;

			for (int i = 0; i < rTrans.childCount; i++) {
				Transform child = rTrans.GetChild (i);
				child.localPosition = new Vector3 (Mathf.Cos (angle), Mathf.Sin (angle), 0) * radius;
				angle -= dAngle;
			}
		}

		#endregion
	}
}
