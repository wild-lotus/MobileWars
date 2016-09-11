using UnityEngine;
using UnityEditor;
using System.Collections;

namespace EfrelGames {
	[CustomEditor(typeof(SelectableCtrl))]
	public class SelectableCtrlEditor : Editor {

		public override void OnInspectorGUI()
		{
			base.OnInspectorGUI ();
			SelectableCtrl selectableCtrl = (SelectableCtrl)target;
			selectableCtrl.Player = EditorGUILayout.IntField ("Player", selectableCtrl.Player);
			EditorUtility.SetDirty (selectableCtrl);
//			EditorUtility.SetDirty (selectableCtrl.view.selectMarkProjector);
		}	
	}
}
