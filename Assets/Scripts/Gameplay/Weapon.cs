using UnityEngine;
using System;
using System.Collections;
using UniRx;
using UniRx.Triggers;

namespace EfrelGames 
{
	/// <summary>Weapon used by an aggressive selectable to attack.</summary>
	public class Weapon : MonoBehaviour
	{
		/// <summary>
		/// Emit and observe firing attempts.
		/// </summary>
		Subject<Attackable> fireSubject; 

		#region Public Configurable fields
		//======================================================================

		[Tooltip("Traslation speed OffMeshLink the bullets.")]
		public float bulletSpeed = 20f;
		[Tooltip("Damage caused by bullets.")]
		public float damage = 1f;
		[Tooltip("Seconds between shots of the weapon.")]
		public float firingPeriod = 1f;
		[Tooltip("Max distance reached by the weapon.")]
		public float distance = 10f;

		#endregion


		#region Cached components
		//======================================================================

		private Transform _trans;

		#endregion


		#region External references
		//======================================================================

		public GameObject bulletPrefab;

		#endregion


		void Awake ()
		{
			_trans = transform;
			// Inittialize firing observable from subject.
			fireSubject = new Subject<Attackable> ();
			fireSubject
				.TakeUntilDestroy(gameObject)
				.ThrottleFirst (TimeSpan.FromSeconds (firingPeriod))
				.Subscribe (target => this.Shoot (target));
		}

		#region Public mehtods
		//======================================================================

		/// <summary>
		/// Call this method while trying to fire a target.
		/// </summary>
		/// <param name="target">Target.</param>
		public void Fire (Attackable target)
		{
			fireSubject.OnNext (target);
		}

		/// <summary>
		/// Perform an actual shot on the target.
		/// </summary>
		/// <param name="target">Target.</param>
		private void Shoot (Attackable target) {
			GameObject bulletGo = GameObject.Instantiate (bulletPrefab);
			Transform bulletTrans = bulletGo.transform;
			bulletTrans.position = _trans.position + Vector3.up;

			Vector3 dPos = target.transform.position - _trans.position;
			Vector3 dir = dPos.normalized;

			GameObject.Destroy (bulletGo, dPos.magnitude / bulletSpeed);

			bulletGo.AddComponent<ObservableTriggerTrigger> ()
				.OnTriggerEnterAsObservable ()
				.Subscribe (other => {
					if (other.gameObject == target.sel.gameObject) {
						GameObject.Destroy (bulletGo);
						other.GetComponent<SelectableCtrl>().att.CurrentHp.Value -= damage;
					}
				});

			Observable.EveryUpdate()
				.TakeUntilDestroy(bulletGo)
				.Subscribe(_ => bulletTrans.Translate(dir * bulletSpeed * Time.deltaTime));
		}

		#endregion
	}
}
