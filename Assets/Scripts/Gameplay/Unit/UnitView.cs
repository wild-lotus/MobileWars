using UnityEngine;
using System.Collections;
using UniRx;

namespace EfrelGames {
	/// <summary>
	/// View related behaviour of a unit.
	/// </summary>
	public class UnitView : SelectableView {

		#region Extrnal references
		//======================================================================

		public Renderer selMarkRend;
		public Animator selMarkAnim;

		#endregion


		#region Public methods
		//======================================================================

		/// <summary>
		/// View effects when selecting or unselecting this unit.
		/// </summary>
		/// <param name="selected">If set to <c>true</c> selected.</param>
		public override void Selected (bool selected)
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
		public override void Move (Vector3 pos)
		{
			transform.LookAt (pos);
		}

		/// <summary>
		/// View effects when attacking the specified target.
		/// </summary>
		/// <param name="target">Target attackable.</param>
		public override void Attack (Attackable target)
		{
			transform.LookAt (target.transform.position);
		}

		/// <summary>
		/// View effects when being attacked.
		/// </summary>
		public override void Targeted ()
		{
			selMarkAnim.SetTrigger ("Targeted");
		}

		/// <summary>
		/// View effects when set the number of the player this selectable 
		/// belongs to.
		/// </summary>
		/// <param name="player">Player number.</param>
		public override void SetPlayerNum (int player)
		{
			base.SetPlayerNum (player);
			selMarkRend.material = PlayersColors.selMarkMats[player];
		}

		#endregion


		#region Context menu methods
		//======================================================================

		[ContextMenu ("Set References")]
		public override void SetReferences ()
		{
			base.SetReferences ();
			selMarkRend = transform.Find("SelMark")
				.GetComponentInChildren<Renderer> (true);
			selMarkAnim = selMarkRend.GetComponent<Animator> ();
		}

		#endregion
	}
}
