using UnityEngine;
using UnityEngine.Assertions;
using System.Collections.Generic;
using UniRx;


namespace EfrelGames
{
	/// <summary>
	/// Behaviour for any aggressive selectable that can auto attack without
	/// player action.
	/// </summary>
	[RequireComponent (typeof(Aggressive))]
	public class AggressiveAutoAttack : MonoBehaviour
	{
		#region Public Configurable fields
		//======================================================================

		[Tooltip ("Distance this unit will chase an auto target.")]
		public float autoAttackRange = 10f;

		[Tooltip ("Distance this unit will chase an auto target.")]
		public float autoChaseRange = 10f;

		#endregion


		#region External references
		//======================================================================

		public SelectableCtrl sel;

		#endregion


		#region Cached components
		//======================================================================

		private Transform _trans;
		private Aggressive _agg;
		private Movable _mov;

		#endregion


		#region Private fields
		//======================================================================

		/// <summary>Position to return after an auto attack chase.</summary>
		private Vector3 _returnPos = Vector3.zero;

		#endregion


		#region Unity callbacks
		//======================================================================

		void Awake ()
		{
			// Check external references
			Assert.IsNotNull (sel);
			// Cache components
			_trans = transform;
			_agg = GetComponent<Aggressive> ();
			_mov = sel.mov;
			// Auto attack integrity
			Assert.IsTrue (
				autoChaseRange + _agg.weapon.distance > autoAttackRange
			);
		}

		void Update ()
		{
			// Find auto attack targets
			if (!_agg.Target) {
				Attackable target = this.FindAutoTarget ();
				if (target != null) {
					this.AutoAttack (target);
				}
			}
		}

		#endregion


		#region Private methods
		//======================================================================

		/// <summary>
		/// Find the bests the target to auto attack within available ones.
		/// </summary>
		/// <returns>The best target available.</returns>
		private Attackable FindAutoTarget ()
		{
			Attackable target = null;
			// Choose the closest available target.
			float minDistance = float.MaxValue;
			foreach (Attackable enemy in _agg.AttList) {
				float distance = Vector3.Distance (
					_trans.position, enemy.transform.position
				);
				if (distance < autoAttackRange && distance < minDistance) {
					minDistance = distance;
					target = enemy;
				}
			}
			return target;
		}

		/// <summary>Start an auto attack.</summary>
		private void AutoAttack (Attackable target)
		{
			// Start attacking
			Debug.Log (sel.name + " Auto attacking " + target.sel.name);
			_agg.Attack (target);

			// Set AutoReturn destination if movable.
			if (_mov) {
				if (_mov.Dest != null 
						&& _mov.Dest.Type == DestType.PlayerSet) {
					_returnPos = _mov.Dest.Position;
				} else {
					if (_returnPos == Vector3.zero) {
						_returnPos = _trans.position;
					}
				}
			}
				
			// Start AutoAttack chasing stream. Chase enemy while it is  within
			// autoChaseRange.
			Transform tgtTrans = target.transform;
			Destination dest = null;
			Observable.EveryUpdate ()
				.TakeUntilDestroy (gameObject)
				.TakeWhile (_ => 
					this.SameTarget (target)
						&& target.Alive
						&& this.AutoAttackReachable (target)
				).Do (_ => {
					if (!_agg.WithinWeaponRange (target)) {
						// Chase enemy
						dest = this.AddAutoAttackDest (tgtTrans.position);
					} else if (_mov) {
						// Clear destination
						_mov.Remove (DestType.AutoAtt);
						_mov.Remove (DestType.AutoAttRetrun);
					}
				}).DoOnCompleted (() => {
					// Clear target
					_agg.Release (target);
					if (_mov) {
						// Cleat autoAttack destination
						_mov.Remove (dest);
						if (_mov.Dest == null 
								&& _returnPos != Vector3.zero) {
							// Add AutoReturn destination
							this.AddReturnDest ();
						}
					}	
				}).Subscribe ();
		}

		/// <summary>
		/// Check if current target is the same as the given one.
		/// </summary>
		/// <returns>Whethet it is the same target.</returns>
		/// <param name="target">Target to check against.</param>
		public bool SameTarget (Attackable target)
		{
			return  _agg.Target && _agg.Target == target;
		}
			
		/// <summary>
		/// Check if the target can be reached using auto attack constraints.
		/// In auto attack a target can be chased until a given distance from
		/// the original resting position.
		/// </summary>
		/// <returns>Whether the target can be reached.</returns>
		/// <param name="target">Target.</param>
		public bool AutoAttackReachable (Attackable target)
		{
			bool withinWeaopnRange = _agg.WithinWeaponRange (target);
			bool hasPlayerSetDest = _mov && _mov.Dest != null
				&&	_mov.Dest.Type == DestType.PlayerSet;
			bool withinAutoChaseRange = _mov &&
				Vector3.Distance (_trans.position, _returnPos) < autoChaseRange;

			return withinWeaopnRange || 
				(!hasPlayerSetDest && withinAutoChaseRange);
		}
			
		/// <summary>
		/// Adds AutoAttack destination to approach/chase target.
		/// </summary>
		private Destination AddAutoAttackDest (Vector3 position)
		{
			if (_mov.Dest == null || _mov.Dest.Position != position) {
				Destination dest = new Destination (position, DestType.AutoAtt);
				_mov.Add (dest);
				return dest;
			}
			return _mov.Dest;
		}

		/// <summary>
		/// Adds AutoAttackReturn destination to return to original resting
		/// position. Also reset return position when appropriate.
		/// </summary>
		private void AddReturnDest ()
		{
			// Add return destination and store path check observable.
			IObservable<bool> pathCheckObs = 
				_mov.Add (new Destination (_returnPos, DestType.AutoAttRetrun));

			// Observe destination changes set by player.
			IObservable<bool> playerDestObs = _mov.DestObs.Select (
				dest => 
					dest != null && (int)dest.Type > (int)DestType.AutoAtt
          	);
				
			// Stream to reset return position when any observable is true.
			Observable.Merge (pathCheckObs, playerDestObs)
				.TakeUntilDestroy (gameObject)
				.TakeWhile (done => !done)
				.DoOnCompleted (() => _returnPos = Vector3.zero)
				.Subscribe ();
		}

		#endregion


		#region Context menu methods
		//======================================================================

		[ContextMenu ("Set References")]
		public void SetReferences ()
		{
			sel = GetComponentInParent<SelectableCtrl> ();
			sel.aggAA = this;
		}

		#endregion
	}
}
