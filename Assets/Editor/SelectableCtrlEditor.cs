using UnityEngine;
using UnityEditor;
using System;
using System.Collections;

using UniRx;

namespace EfrelGames {
	[CanEditMultipleObjects]
	[CustomEditor(typeof(SelectableCtrl))]
	public class SelectableCtrlEditor : Editor {

		private SerializedProperty _playerNum;
		private SerializedProperty _selected;
		private bool cagada;

		void OnEnable () {
			// Setup the SerializedProperties.
			_playerNum = serializedObject.FindProperty ("_playerNum");
			_selected = serializedObject.FindProperty ("_selected");
		}



		public override void OnInspectorGUI()
		{
				
			serializedObject.Update ();
		
			base.OnInspectorGUI ();

			SelectableCtrl[] selCtrls = Array.ConvertAll(targets, item => (SelectableCtrl)item);

			int newPlayerNum = EditorGUILayout.IntPopup (
				"Player Num",
				selCtrls[0].PlayerNum,
				new string[] {"0", "1", "2"},
				new int[] {0, 1, 2}
			);
			_playerNum.intValue = newPlayerNum;
			foreach (SelectableCtrl sc in selCtrls) {
				if (sc.PlayerNum != newPlayerNum) {
					sc.PlayerNum = newPlayerNum;
				}
			}
				
			bool newSelected = EditorGUILayout.Toggle ("Selected", selCtrls[0].Selected);
			if (Application.isPlaying) {
				_selected.boolValue = newSelected;
				foreach (SelectableCtrl sc in selCtrls) {
					if (sc.Selected != newSelected) {
						sc.Selected = newSelected;
					}
				}
			} else {
				foreach (SelectableCtrl sc in selCtrls) {
					if (sc.Selected != newSelected) {
						Debug.Log ("muere");
						cagada = true;
						break;
					}
				}
			}

			if (cagada) {
				EditorGUILayout.HelpBox ("\"Selected\" only modifiable in Editor while playing.", MessageType.Warning);
			}

			serializedObject.ApplyModifiedProperties ();
		}	
	}
}
