using UnityEngine;
using System.Collections;
using UniRx;

namespace EfrelGames {
	public class SelectableView : MonoBehaviour {

		public Renderer[] playerColoredRends;
		public Renderer selMarkRend;
		public Animator selMarkAnim;
		public GameObject destFxPrefab;

		public void Select (bool selected)
		{
			if (selected) {
				selMarkAnim.ResetTrigger("Unselected");
				selMarkAnim.SetTrigger ("Selected");
			} else {
				selMarkAnim.SetTrigger ("Unselected");
			}
		}

		public void PlayerSetDest (Vector3 pos)
		{
			this.SpawnFx (pos);
		}

		public void PlayerAttackTarget (Attackable target)
		{
			target.sel.view.selMarkAnim.SetTrigger ("Targeted");
			this.SpawnFx (target.transform.position);
		}

		public void SetPlayer (int player)
		{
			Material mat = PlayersColors.playerMats [player];
			foreach (Renderer rend in playerColoredRends) {
				rend.material = mat;
			}
			selMarkRend.material = PlayersColors.selMarkMats[player];
		}

		private void SpawnFx (Vector3 pos)
		{
			GameObject destFxGo = Instantiate (destFxPrefab, pos + Vector3.up, Quaternion.Euler(90, 0, 0)) as GameObject;
			Destroy (destFxGo, 1);
		}

		[ContextMenu ("Set References")]
		public void SetReferences ()
		{
			GetComponentsInChildren <Renderer> ()
				.ToObservable ()
				.Where (x => x.CompareTag ("PlayerColored"))
				.ToArray ()
				.Subscribe(rends => playerColoredRends = rends);
			selMarkRend = transform.Find("SelMark").GetComponentInChildren<Renderer> (true);
			selMarkAnim = selMarkRend.GetComponent<Animator> ();
		}
	}
}
