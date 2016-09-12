using UnityEngine;
using UnityEngine.Assertions;
using System.Collections;

namespace EfrelGames
{
	public class SelectableCtrl : MonoBehaviour
	{
		#region Constants
		//======================================================================

		private const bool LOG = true;
		public const int GROUND_LAYER = 8;

		#endregion

		#region Public Configurable properties
		//======================================================================

		[SerializeField]
		[HideInInspector]
		private int _player;
		public int Player {
			get { return _player; }
			set {
				if (_player != value) {
					_player = value;
					_player = Mathf.Clamp (_player, 0, 2);
					view.SetPlayer (_player);
				}
			}
		}

		private bool _selected;
		public bool Selected {
			get { return _selected; }
			set {
				_selected = value;
				if (_selected) {
					if (LOG) Debug.Log (name + " Selected");
					selectionMngr.selectedGoList.Add (this);
				} else {
					if (LOG) Debug.Log (name + " Unselected");
					selectionMngr.selectedGoList.Remove (this);
				}
				view.Select (_selected);
			}
		}

		#endregion


		#region Cached components
		//======================================================================

		public Transform trans;

		#endregion


		#region Public External references
		//======================================================================

		public SelectionMngr selectionMngr;
		public SelectableView view;
		// Optional references:
		public Movable mov;
		public Attackable att;
		public AggressivePlayerAttack aggPA;
		public AggressiveAutoAttack aggAA;

		#endregion


		#region Unity callbacks
		//======================================================================

		void Awake ()
		{
			// Check external references
			Assert.IsNotNull (selectionMngr);
			Assert.IsNotNull (view);
			// Cache components
			trans = transform;
		}

		#endregion


		#region Public methods
		//======================================================================

		public void ActionSelect (GameObject target, Vector3 pos)
		{
			SelectableCtrl other =  target.GetComponent<SelectableCtrl> ();
			if (target.layer == GROUND_LAYER) {
				if (this.mov != null) {
					this.mov.Add (new Destination (pos, DestType.PlayerSet));
					view.PlayerSetDest (pos);
				}
			} else if (this.aggPA && other.att &&
			           this.Player != target.GetComponent <SelectableCtrl> ().Player) {
				this.transform.LookAt (target.transform.position);
				this.aggPA.PlayerAttack (other.att);
				view.PlayerAttackTarget (other.att);
			} else {
				Debug.LogWarning ("Unknown action");
			}
		}

		#endregion


		#region Context menu methods
		//======================================================================

		[ContextMenu ("Set References")]
		public void SetReferences ()
		{
			view = GetComponent <SelectableView> ();
			selectionMngr = FindObjectOfType<SelectionMngr> ();
			mov = GetComponent<Movable> ();
			att = GetComponentInChildren<Attackable> ();
			aggPA = GetComponentInChildren<AggressivePlayerAttack> ();
			aggAA = GetComponentInChildren<AggressiveAutoAttack> ();
		}

		#endregion
	}
}
