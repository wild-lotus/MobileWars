using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

namespace EfrelGames {
	public class UICtrl : MonoBehaviour {

		public void RestartScene ()
		{
			SceneManager.LoadScene (SceneManager.GetSceneAt (0).name);
		}
	}
}
