using UnityEngine;
using System.Collections;
using UniRx;

namespace EfrelGames {
	/// <summary>
	/// View related behaviour of a selectable.
	/// </summary>
	public class SelectableView : MonoBehaviour {

		#region Extrnal references
		//======================================================================

		public Renderer[] playerColoredRends;
		public Renderer selMarkRend;
		public Animator selMarkAnim;

		#endregion


		#region Public methods
		//======================================================================

		/// <summary>
		/// View effects when selecting or unselecting this selectable.
		/// </summary>
		/// <param name="selected">If set to <c>true</c> selected.</param>
		public void Select (bool selected)
		{
			if (selected) {
				selMarkAnim.ResetTrigger("Unselected");
				selMarkAnim.SetTrigger ("Selected");
			} else {
				selMarkAnim.SetTrigger ("Unselected");
			}
		}

		/// <summary>
		/// View effects when moving to the specified position.
		/// </summary>
		/// <param name="pos">Position.</param>
		public void Move (Vector3 pos)
		{
			transform.LookAt (pos);
		}

		/// <summary>
		/// View effects when attacking the specified target.
		/// </summary>
		/// <param name="target">Target attackable.</param>
		public void Attack (Attackable target)
		{
			transform.LookAt (target.transform.position);
		}

		/// <summary>
		/// Sets the number of the player this selectable belongs to.
		/// </summary>
		/// <param name="player">Player number.</param>
		public void SetPlayerNum (int player)
		{
			Material mat = PlayersColors.playerMats [player];
			foreach (Renderer rend in playerColoredRends) {
				rend.material = mat;
			}
			selMarkRend.material = PlayersColors.selMarkMats[player];
		}

		#endregion


		#region Context menu methods
		//======================================================================

		[ContextMenu ("Set References")]
		public void SetReferences ()
		{
			GetComponentsInChildren <Renderer> ().ToObservable ()
				.Where (x => x.CompareTag ("PlayerColored"))
				.ToArray ()
				.Subscribe(rends => playerColoredRends = rends);
			selMarkRend = transform.Find("SelMark")
				.GetComponentInChildren<Renderer> (true);
			selMarkAnim = selMarkRend.GetComponent<Animator> ();
		}

		#endregion
	}
}
