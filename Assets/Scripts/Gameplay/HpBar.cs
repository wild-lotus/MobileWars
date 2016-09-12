using UnityEngine;
using UnityEngine.UI;
using System.Collections;

namespace EfrelGames {
//	[ExecuteInEditMode]
	public class HpBar : MonoBehaviour {

		#region Private Cached components
		//======================================================================

		private Transform _trans;
		private Transform _camTrans;
		private Image _hpImg;

		#endregion


		#region External references
		//======================================================================

		/// <summary>
		/// Transform that the bar must follow. Refernece set by Attackable.
		/// </summary>
		[HideInInspector]
		public Transform selTrans;


		/// <summary>
		/// Bar height from the reference position. Value set by Attackable.
		/// </summary>
		[HideInInspector]
		public float height;

		#endregion


		#region Unity callbacks
		//======================================================================

		void Awake ()
		{
			_trans = transform;
			_camTrans = Camera.main.transform;
			_hpImg = _trans.GetChild (1).GetComponent<Image> ();
		}

		void LateUpdate () {
			// Always follow selectable and look at the cam
			_trans.position = selTrans.position + Vector3.up * height;
			_trans.forward = _camTrans.forward;
		}

		#endregion


		#region Public methods
		//======================================================================

		/// <summary>
		/// Sets the progress fo the HP Bar.
		/// </summary>
		/// <param name="progress">Progress.</param>
		public void SetProgress (float progress)
		{
			_hpImg.fillAmount = progress;
		}

		#endregion

	}
}
