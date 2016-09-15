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

		/// <summary>The weapon used to attack enemies.</summary>
		public Weapon weapon;

		#endregion


		#region Public fields and properties
		//======================================================================

		/// <summary>List of attackable targets within range</summary>
		public List<Attackable> AttList { get; private set; }

		/// <summary>Current target being attacked</summary>
		public Attackable Target { get; private set; }

		#endregion


		#region External references
		//======================================================================

		public SelectableCtrl sel;

		#endregion


		#region Cached components
		//======================================================================

//		private Movable _mov;

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
			if (Target && Target.Alive && this.WithinWeaponRange (Target)) {
				weapon.Fire (Target);
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
					.TakeWhile(hp => hp > 0)
					.DoOnCompleted (() => AttList.Remove (otherAtt))
					.TakeWhile (_ => AttList.Contains (otherAtt))
					.Subscribe ();
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
			enabled = true;
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
				enabled = false;
				return true;
			}
			return false;
		}

		/// <summary>
		/// Check whether a given target is within its weapon range.
		/// </summary>
		/// <param name="att">The attackable target.</param>
		public bool WithinWeaponRange (Attackable target)
		{
			return Vector3.Distance (
				sel.trans.position, target.sel.trans.position
			) < weapon.distance;
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
				int otherPlayer = att.sel.PlayerNum;
				return  otherPlayer != 0 && otherPlayer != sel.PlayerNum;
			}
			return false;
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
