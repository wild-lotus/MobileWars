using UnityEngine;
using UnityEngine.Assertions;
using System.Collections;

namespace EfrelGames
{
	/// <summary>
	/// Controller for a selectable.
	/// </summary>
	public class SelectableCtrl : MonoBehaviour
	{
		#region Constants
		//======================================================================

		private const bool LOG = false;
		public const int GROUND_LAYER = 8;

		#endregion

		#region Public Configurable properties
		//======================================================================

		[SerializeField]
		[HideInInspector]
		private int _player;
		public int PlayerNum {
			get { return _player; }
			set {
				if (_player != value) {
					_player = value;
					_player = Mathf.Clamp (_player, 0, 2);
					view.SetPlayerNum (_player);
				}
			}
		}

		[SerializeField]
		[HideInInspector]
		private bool _selected;
		public bool Selected {
			get { return _selected; }
			set {
				if (_selected != value) {
					_selected = value;
					if (_selected) {
						if (LOG)
							Debug.Log (name + " Selected");
						selectionMngr.selectedList.Add (this);
					} else {
						if (LOG)
							Debug.Log (name + " Unselected");
						selectionMngr.selectedList.Remove (this);
					}
					view.Select (_selected);
				}
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

		public void Move (Vector3 pos)
		{
			if (LOG)
				Debug.Log (name + " Player set move to " + pos);
			if (this.mov != null) {
				this.mov.Add (new Destination (pos, DestType.PlayerSet));
				view.Move (pos);
			} else {
				Debug.Log (string.Format("{0} can't move", name));
			}
		}

		public void Attack (SelectableCtrl target)
		{
			if (LOG)
				Debug.Log (name + " Player set attack to " + target.name);
			if (this.aggPA && target.att &&
			    	this.PlayerNum != 
						target.GetComponent <SelectableCtrl> ().PlayerNum) {
				this.aggPA.PlayerAttack (target.att);
				view.Attack (target.att);
			} else {
				Debug.Log (
					string.Format("{0} can't attack {1}", name, target.name)
				);
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
