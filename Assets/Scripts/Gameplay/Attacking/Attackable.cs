using UnityEngine;
using UnityEngine.UI;
using System;
using System.Collections;
using UniRx;

namespace EfrelGames
{
	public class Attackable : MonoBehaviour
	{
		#region Public Configurable fields
		//======================================================================

		[Tooltip ("Max amount of health points of this unit.")]
		public float maxHp = 10f;

		#endregion


		#region Public fields and properties
		//======================================================================

		/// <summary>Current health points of this unit (reactive). </summary>
		public FloatReactiveProperty CurrentHp;

		/// <summary>Whether this unit is alive.</summary>
		public bool Alive { get { return CurrentHp.Value > 0; } }

		#endregion


		#region Public external references
		//======================================================================

		public SelectableCtrl sel;
		public Transform wolrdUiTrans;
		public HpBar hpBar;

		#endregion


		#region Unity callbacks
		//======================================================================

		void Awake ()
		{
			CurrentHp = new FloatReactiveProperty (maxHp);
			// Set HpBar external references and reparent.
			hpBar.selTrans = sel.transform;
			hpBar.height = hpBar.transform.position.y;
			hpBar.transform.SetParent (wolrdUiTrans);
		}

		void Start ()
		{
			// Update HP bar and detect death.
			CurrentHp
				.TakeUntilDestroy (gameObject)
				.ThrottleFrame (0, FrameCountType.EndOfFrame)
				.Subscribe (hp => {
					hpBar.SetProgress(hp / maxHp);
					if (hp <= 0) {
						this.Die ();
					}
				});
		}

		#endregion


		#region Private methods
		//======================================================================

		/// <summary>
		/// Take required actions when this unit dies.
		/// </summary>
		private void Die ()
		{
			if (sel.Selected) {
				sel.Selected = false;
			}
			Destroy (sel.gameObject);
			Destroy (hpBar.gameObject);
		}

		#endregion


		#region Context menu methods
		//======================================================================

		[ContextMenu ("Set References")]
		public void SetReferences ()
		{
			sel = GetComponentInParent<SelectableCtrl> ();
			wolrdUiTrans = GameObject.Find ("WorldUI").transform;
			hpBar = GetComponentInChildren<HpBar> ();
		}

		#endregion


		#region Object methods
		//======================================================================

		public override string ToString ()
		{
			return string.Format ("[{0} Attackable: HP={1}]", sel.name, CurrentHp);
		}

		#endregion
	}
}
