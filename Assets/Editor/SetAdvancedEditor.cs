using UnityEngine;
using UnityEditor;

[CustomEditor( typeof(SetAdvanced))]
public class SetAdvancedEditor:Editor  {


	LayerMask  LM;
	public override void OnInspectorGUI(){
		DrawDefaultInspector ();

		SetAdvanced TM = (SetAdvanced) target;

		LM = TM.intlayer;

		GUILayout.Label ("set the layer to generated gameobject",EditorStyles.boldLabel );
		LM =EditorGUILayout.LayerField ("Layer of terrain" ,LM );
		TerrainManagerStatics.layerOfGround = LM;
		TM.intlayer = LM.value;
	
	}
}
