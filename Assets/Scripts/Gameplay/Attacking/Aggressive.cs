using UnityEngine;
using UnityEngine.Assertions;
using System;
using System.Collections.Generic;
using UniRx;

namespace EfrelGames
{
	/// <summary>Behaviour for any selectable that can attack.</summary>
	public class Aggressive : MonoBehaviour
	{
		#region Public Configurable fields
		//======================================================================

		[Tooltip ("Damage per attack.")]
		public float damage = 1f;

		[Tooltip ("Attacking period.")]
		public float attackPeriod = 1f;

		#endregion


		#region Public fields and properties
		//======================================================================

		/// <summary>List of attackable targets within range</summary>
		public List<Attackable> AttList { get; private set; }

		/// <summary>Current target being attacked</summary>
		public Attackable Target { get; set; }

		#endregion


		#region External references
		//======================================================================

		public SelectableCtrl sel;

		#endregion


		#region Cached components
		//======================================================================

//		private Movable _mov;

		#endregion


		#region Private fields
		//======================================================================

		private float _lastAttackTime;

		#endregion


		#region Unity callbacks
		//======================================================================

		void Awake ()
		{
			AttList = new List <Attackable> ();
			// Check external references
			Assert.IsNotNull (sel);
			// Cache components
//			_mov = sel.mov;
		}

		void Update () {
			// Attack target if possible
			if (Target && Target.Alive && this.WithinRange (Target)) {
				if (Time.time - attackPeriod > _lastAttackTime) {
					this.PerformAttack ();
					if (_lastAttackTime == 0) {
						_lastAttackTime = Time.time;
					} else {
						_lastAttackTime = _lastAttackTime + attackPeriod;
					}
				}
			}
		}


		void OnTriggerEnter (Collider other)
		{
			// Detect nearby approaching enemies
			// (Only collisions with selectables should be reported here)
			Attackable otherAtt = other.GetComponent<SelectableCtrl> ().att;
			if (this.IsEnemy (otherAtt)) {
				AttList.Add (otherAtt);
				// Detect nearby dying enemies
				otherAtt.CurrentHp
					.TakeUntilDestroy (gameObject)
					.TakeWhile (_ => AttList.Contains (otherAtt))
					.Where (hp => hp <= 0)
					.Subscribe (hp => AttList.Remove (otherAtt));
			}
		}

		void OnTriggerExit (Collider other)
		{
			// Detect nearby leaving enemies
			// (Only collisions with selectables should be reported here)
			Attackable otherAtt = other.GetComponent<SelectableCtrl> ().att;
			if (this.IsEnemy (otherAtt)) {
				AttList.Remove (otherAtt);
			}
		}

		#endregion


		#region Public methds
		//======================================================================

		/// <summary>Set a new attacking target.</summary>
		/// <param name="target">The new target.</param>
		public void Attack (Attackable target)
		{
			Target = target;
		}

		/// <summary>
		/// Release and stop attacking the given target if it's the current one.
		/// </summary>
		/// <param name="target">Target to be released.</param>
		/// <return>Whether target has been released.</return>
		public bool Release (Attackable target)
		{
			if (Target == target) {
				Target = null;
				return true;
			}
			return false;
		}

		/// <summary>
		/// Check whether a given target is within attack range.
		/// </summary>
		/// <param name="att">The attackable target.</param>
		public bool WithinRange (Attackable att)
		{
			return AttList.Contains (att);
		}

		#endregion


		#region Private methods
		//======================================================================

		/// <summary>
		/// Check whether a given attackabke is a targetabke enemy.
		/// </summary>
		/// <param name="att">The attackable.</param>
		private bool IsEnemy (Attackable att)
		{
			if (att != null) {
				int otherPlayer = att.sel.Player;
				return  otherPlayer != 0 && otherPlayer != sel.Player;
			}
			return false;
		}

		/// <summary>Fire against current target.</summary>
		private void PerformAttack ()
		{
			Target.CurrentHp.Value -= damage;
		}
			
		#endregion


		#region Context menu methods
		//======================================================================

		[ContextMenu ("Set References")]
		public void SetReferences ()
		{
			sel = GetComponentInParent<SelectableCtrl> ();
//			_mov = sel.mov;
		}

		#endregion
	}
}
