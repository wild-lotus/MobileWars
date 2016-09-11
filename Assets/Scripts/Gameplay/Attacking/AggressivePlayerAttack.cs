using UnityEngine;
using UnityEngine.Assertions;
using System;
using System.Collections;
using UniRx;
using UniRx.Triggers;

namespace EfrelGames
{
	[RequireComponent (typeof(Aggressive))]
	public class AggressivePlayerAttack : MonoBehaviour
	{
		#region External references
		//======================================================================

		public SelectableCtrl sel;

		#endregion


		#region Cached components
		//======================================================================

		private Aggressive _agg;
		private Movable _mov;

		#endregion


		#region Unity callbacks
		//======================================================================

		void Awake ()
		{
			// Check external references
			Assert.IsNotNull (sel);
			// Cache components
			_agg = GetComponent <Aggressive> ();
			_mov = sel.mov;
		}

		#endregion


		#region Public methds
		//======================================================================

		/// <summary>
		/// Start an attack set by player on a target.
		/// </summary>
		/// <param name="target">Target.</param>
		public void PlayerAttack (Attackable target)
		{
			// Can't attack targets out of range if not movable.
			if (!_mov && !_agg.WithinRange (target)) {
				Debug.Log ("Target out of range");
				return;
			}

			// Remove any previous player set destination.
			if (_mov) {
				_mov.Remove (DestType.PlayerSet);
			}

			// Start attacking.
			Debug.Log (sel.name + "Player attack on " + target.sel.name);
			_agg.Attack (target);

			// Start PlayerAttack chasing stream. Chase enemy to death.
			Destination dest = null;
			Transform tgtTrans = target.transform;
			Observable.EveryUpdate ()
				.TakeWhile (_ => 
					this.SameTarget (target)
						&& target.Alive
						&& this.PlayerAttackReachable (target)
				).Do (_ => {
					if (!_agg.WithinRange (target)) {
						// Chase enemy
						dest = this.AddPlayerAttackDest (tgtTrans.position);
					} else {
						// Clear destination
						_mov.Remove (DestType.PlayerAtt);
					}
				}).DoOnCompleted (() => {
					// Clear target
					_agg.Release (target);
					if (_mov) {
						// Clear PlayerAttack destination
						_mov.Remove (dest);
					}
				}).TakeUntilDestroy (gameObject)
				.Subscribe ();
		}

		#endregion


		#region Private methods
		//======================================================================

		/// <summary>
		/// Check if current target is the same as the given one.
		/// </summary>
		/// <returns>Whethet it is the same target.</returns>
		/// <param name="target">Target to check against.</param>
		public bool SameTarget (Attackable target)
		{
			return _agg.Target && _agg.Target == target;
		}

		/// <summary>
		/// Check if the target can be reached using player attack constraints.
		/// In player attack it can chase enemy to death.
		/// </summary>
		/// <returns>Whether the target can be reached.</returns>
		/// <param name="target">Target.</param>
		public bool PlayerAttackReachable (Attackable target)
		{
			return _agg.WithinRange (target) || (
			    _mov && (
			        _mov.Dest == null
			        	|| _mov.Dest.Type != DestType.PlayerSet
			    )
			);
		}

		/// <summary>
		/// Adds a PlayerAttack destination to approach/chase target.
		/// </summary>
		private Destination AddPlayerAttackDest (Vector3 pos)
		{
			Destination dest = new Destination (pos, DestType.PlayerAtt);
			_mov.Add (dest);
			return dest;
		}

		#endregion


		#region Context menu methods
		//======================================================================

		[ContextMenu ("Set References")]
		public void SetReferences ()
		{
			sel = GetComponentInParent<SelectableCtrl> ();
			sel.aggPA = this;
			_mov = sel.mov;
		}

		#endregion
	}
}
