using UnityEngine;
using UnityEngine.UI;
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
		public FloatReactiveProperty CurrentHp = new FloatReactiveProperty (10);

		/// <summary>Whether this unit is alive.</summary>
		public bool Alive { get { return CurrentHp.Value > 0; } }

		#endregion


		#region Public external references
		//======================================================================

		public SelectableCtrl sel;
		public Slider hpSlider;

		#endregion


		#region Unity callbacks
		//======================================================================

		void Awake ()
		{
			// Init and update hp slider
			hpSlider.maxValue = maxHp;
			CurrentHp.Do (hp => hpSlider.value = hp)
				.TakeUntilDestroy (gameObject)
				.Where (hp => hp <= 0)
				.Subscribe (_ => this.Die ());
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
		}

		#endregion


		#region Context menu methods
		//======================================================================

		[ContextMenu ("Set References")]
		public void SetReferences ()
		{
			sel = GetComponentInParent<SelectableCtrl> ();
			hpSlider = sel.GetComponentInChildren <Slider> ();
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
