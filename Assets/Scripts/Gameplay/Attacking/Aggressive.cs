using UnityEngine;
using UnityEngine.Assertions;
using System;
using System.Collections.Generic;
using UniRx;
using UniRx.Triggers;

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

		public Movable _mov;

		#endregion


		#region Unity callbacks
		//======================================================================

		void Awake ()
		{
			AttList = new List <Attackable> ();
			// Check external references
			Assert.IsNotNull (sel);
			// Cache components
			_mov = sel.mov;
		}

		void Start ()
		{
			ObservableTriggerTrigger obsTrig = 
				gameObject.AddComponent<ObservableTriggerTrigger> ();

			// Detect approaching enemies
			obsTrig.OnTriggerEnterAsObservable ()
				.TakeUntilDestroy (gameObject)
				.Select (other => other.GetComponent<SelectableCtrl> ().att)
				.Where (att => this.IsEnemy (att))
				.Subscribe (att => {
					print ("Pom");
					AttList.Add (att);
					// Detect dying enemies
					att.CurrentHp
							.TakeUntilDestroy (gameObject)
							.TakeWhile (_ => AttList.Contains (att))
							.Where (hp => hp <= 0)
							.Subscribe (hp => AttList.Remove (att));
				});

			// Detect leaving enemies
			obsTrig.OnTriggerExitAsObservable ()
				.TakeUntilDestroy (gameObject)
				.Select (collider => collider.GetComponent<Attackable> ())
				.Where (att => this.IsEnemy (att))
				.Subscribe (att => AttList.Remove (att));

			// Attacking stream. Attack as soon as there is a target.
			Observable.EveryUpdate ()
				.TakeUntilDestroy (gameObject)
				.Where (_ =>
					Target && Target.Alive && this.WithinRange (Target)
				).ThrottleFirst (TimeSpan.FromSeconds (attackPeriod))
				.Subscribe (_ => this.PerformAttack ());
		}

//		float laTime;
//		void Update () {
//
//			if (Target && Target.Alive && this.WithinRange (Target)) {
//				if (Time.time - attackPeriod > laTime) {
//					this.PerformAttack ();
//					if (laTime == 0) {
//						laTime = Time.time;
//					} else {
//						laTime = laTime + attackPeriod;
//					}
//				}
//			}
//		}

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
			_mov = sel.mov;
		}

		#endregion
	}
}
