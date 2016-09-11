using UnityEngine;
using System.Collections;

namespace EfrelGames {
//	[ExecuteInEditMode]
	public class SelectableUiBillboard : MonoBehaviour {

		#region Private Cached components
		//======================================================================

		private Transform _trans;
		private Transform _camTrans;

		#endregion


		#region Unity callbacks
		//======================================================================

		void Awake ()
		{
			_trans = transform;
			_camTrans = Camera.main.transform;
		}

		void LateUpdate () {
			// Always look at the cam.
			_trans.forward = _camTrans.forward;
		}

		#endregion
	}
}
