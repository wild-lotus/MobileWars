using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

namespace EfrelGames {
	public class UICtrl : MonoBehaviour {

		public Light mainLight;

		public void RestartScene ()
		{
			SceneManager.LoadScene (SceneManager.GetSceneAt (0).name);
		}

		public void ToggleShadows ()
		{
			if (mainLight.shadows == LightShadows.None) {
				mainLight.shadows = LightShadows.Hard;
			} else {
				print ("Lights off");
				mainLight.shadows = LightShadows.None;
			}
		}
	}
}
