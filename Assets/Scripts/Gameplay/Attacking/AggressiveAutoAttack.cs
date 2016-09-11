using UnityEngine;
using UnityEngine.Assertions;
using System.Collections.Generic;
using UniRx;
using UniRx.Triggers;


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
		}

		void Start ()
		{
			// Stream to find auto attack targets
			Observable.EveryUpdate ()
				.TakeUntilDestroy (gameObject)
				.Where (_ => !_agg.Target && _agg.AttList.Count > 0)
				.Subscribe (_ => this.AutoAttack ());
		}

		#endregion


		#region Private methods
		//======================================================================

		/// <summary>Start an auto attack.</summary>
		private void AutoAttack ()
		{
			// Find the best available target
			Attackable target = this.BestTarget ();
			Transform tgtTrans = target.transform;
			Debug.Log (sel.name + " Auto attacking " + target.sel.name);

			// Start attacking
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
			Destination dest = null;
			Observable.EveryUpdate ()
				.TakeUntilDestroy (gameObject)
				.TakeWhile (_ => 
					this.SameTarget (target)
						&& target.Alive
						&& this.AutoAttackReachable (target)
				).Do (_ => {
					if (!_agg.WithinRange (target)) {
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
		/// Find the bests the target to auto attack within available ones.
		/// </summary>
		/// <returns>The best target available.</returns>
		private Attackable BestTarget ()
		{
			Attackable target = null;
			// Choose the closest available target.
			float minDistance = float.MaxValue;
			foreach (Attackable enemy in _agg.AttList) {
				float distance = Vector3.Distance (
					                 _trans.position, enemy.transform.position
				                 );
				if (distance < minDistance) {
					minDistance = distance;
					target = enemy;
				}
			}
			return target;
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
			bool withinAttackRange = _agg.WithinRange (target);
			bool hasPlayerSetDest = _mov && _mov.Dest != null
				&&	_mov.Dest.Type == DestType.PlayerSet;
			bool withinAutoChaseRange = _mov &&
				Vector3.Distance (_trans.position, _returnPos) < autoChaseRange;

			return withinAttackRange || 
				(!hasPlayerSetDest && withinAutoChaseRange);
		}
			
		/// <summary>
		/// Adds AutoAttack destination to approach/chase target.
		/// </summary>
		private Destination AddAutoAttackDest (Vector3 position)
		{
			Destination dest = new Destination (position, DestType.AutoAtt);
			_mov.Add (dest);
			return dest;
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
			IObservable<bool> newPlayerDest = _mov.DestObs.Select (
				dest => 
					dest != null && (int)dest.Type > (int)DestType.AutoAtt
          	);

			// Stream to reset return position when any observable is true.
			Observable.Merge (pathCheckObs, newPlayerDest)
				.TakeUntilDestroy (gameObject)
				.TakeWhile (result => !result)
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
