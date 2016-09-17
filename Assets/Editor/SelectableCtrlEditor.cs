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

		private bool _playerNum_hmdv;
		private bool _selected_hmdv;

		private int _playerNum_value;
		private bool _selected_value;

		private bool _selWrongEdit;

		void OnEnable () {
			// Setup the SerializedProperties.
			_playerNum = serializedObject.FindProperty ("_playerNum");
			_playerNum_hmdv = _playerNum.hasMultipleDifferentValues;
			_playerNum_value = _playerNum.intValue;

			_selected = serializedObject.FindProperty ("_selected");
			_selected_hmdv = _selected.hasMultipleDifferentValues;
			_selected_value = _selected.boolValue;
		}

		public override void OnInspectorGUI()
		{
				
			serializedObject.Update ();
		
			base.OnInspectorGUI ();

			// Draw properties
			EditorGUILayout.IntPopup (
				_playerNum,
				new GUIContent[] {
					new GUIContent ("0"),
					new GUIContent ("1"),
					new GUIContent ("2")
				},
				new int[] {0, 1, 2}
			);
			EditorGUILayout.PropertyField (_selected);

			// Check for changes
			if (GUI.changed) {
				SelectableCtrl[] selCtrls = Array.ConvertAll(targets, item => (SelectableCtrl)item);
				// On player num
				if (_playerNum_hmdv != _playerNum.hasMultipleDifferentValues
				    	|| _playerNum_value != _playerNum.intValue) {
					_playerNum_hmdv = _playerNum.hasMultipleDifferentValues;
					_playerNum_value = _playerNum.intValue;
					foreach (SelectableCtrl sc in selCtrls) {
						sc.PlayerNum = _playerNum_value;
					}
				}
				// On selected
				if ( _selected_hmdv != _selected.hasMultipleDifferentValues
						|| _selected_value != _selected.boolValue) {
					// Selected can only be modified while playing
					if (Application.isPlaying) {
						_selected_hmdv = _selected.hasMultipleDifferentValues;
						_selected_value = _selected.boolValue;
						foreach (SelectableCtrl sc in selCtrls) {
							sc.Selected = _selected_value;
						}
					} else {
						_selWrongEdit = true;
						_selected.boolValue = false;
					}
				}
			}
				
			if (_selWrongEdit) {
				EditorGUILayout.HelpBox ("Selected property can only be modified while playing.", MessageType.Warning);
			}

			serializedObject.ApplyModifiedProperties ();
		}	
	}
}
