using UnityEngine;
using UnityEditor;
using System.Collections.Generic;
using NobleMuffins.TurboSlicer;
using NobleMuffins.TurboSlicer.Guts;

[CustomEditor(typeof(Sliceable))]
[CanEditMultipleObjects]
public class SliceableEditor : Editor
{
	SerializedProperty refreshCollidersProperty;
	SerializedProperty alternatePrefabProperty;
	SerializedProperty alwaysCloneFromAlternateProperty;
	SerializedProperty channelNormalsProperty;
	SerializedProperty channelTangentsProperty;
	SerializedProperty channelUV2Property;
	SerializedProperty shreddableProperty;
	
	SerializedProperty explicitlySelectedMeshHolderProperty;
	
	public void OnEnable()
	{
		refreshCollidersProperty = serializedObject.FindProperty("refreshColliders");
		alternatePrefabProperty = serializedObject.FindProperty("alternatePrefab");
		alwaysCloneFromAlternateProperty = serializedObject.FindProperty("alwaysCloneFromAlternate");
		channelNormalsProperty = serializedObject.FindProperty("channelNormals");
		channelTangentsProperty = serializedObject.FindProperty("channelTangents");
		channelUV2Property = serializedObject.FindProperty("channelUV2");
		shreddableProperty = serializedObject.FindProperty("shreddable");
		
		explicitlySelectedMeshHolderProperty = serializedObject.FindProperty("explicitlySelectedMeshHolder");
	}
	
	public override void OnInspectorGUI ()
	{
		bool someTargetsAreUnvetted = false;
		bool someTargetsHaveMultipleRenderers = false;
		
		List<Renderer> relevantRenderers = new List<Renderer>();
		List<Renderer> allRenderers = new List<Renderer>();
		
		foreach(Object o in targets)
		{
			Sliceable s = (Sliceable) o;

			Component[] _allRenderersOnThisTarget = s.GetComponentsInChildren(typeof(Renderer), true);

			Renderer[] allRenderersOnThisTarget = new Renderer[_allRenderersOnThisTarget.Length];

			for(int i = 0; i < _allRenderersOnThisTarget.Length; i++)
				allRenderersOnThisTarget[i] = _allRenderersOnThisTarget[i] as Renderer;
			
			allRenderers.AddRange( allRenderersOnThisTarget );

			if(allRenderersOnThisTarget.Length == 1)
			{
				relevantRenderers.Add( allRenderersOnThisTarget[0] );
			}
			else if(s.explicitlySelectedMeshHolder != null)
			{
				relevantRenderers.Add( s.explicitlySelectedMeshHolder.GetComponent(typeof(Renderer)) as Renderer);
			}
			else
			{
				someTargetsAreUnvetted = true;
			}
			
			someTargetsHaveMultipleRenderers |= allRenderersOnThisTarget.Length > 1;
		}
		
		EditorGUILayout.PropertyField(refreshCollidersProperty, new GUIContent("Refresh colliders"));
		EditorGUILayout.PropertyField(alternatePrefabProperty, new GUIContent("Alternate prefab"));
		EditorGUILayout.PropertyField(shreddableProperty, new GUIContent("Shreddable"));

		bool atLeastSomeHaveAlternatePrefab = alternatePrefabProperty.hasMultipleDifferentValues || alternatePrefabProperty.objectReferenceValue != null;

		if(atLeastSomeHaveAlternatePrefab)
			EditorGUILayout.PropertyField(alwaysCloneFromAlternateProperty, new GUIContent("Always clone from alternate"));

		EditorGUILayout.PropertyField(channelNormalsProperty, new GUIContent("Process Normals"));
		EditorGUILayout.PropertyField(channelTangentsProperty, new GUIContent("Process Tangents"));
		EditorGUILayout.PropertyField(channelUV2Property, new GUIContent("Process UV2"));
		
		EditorGUILayout.Separator();
		
		//Ensure that all the targets are vetted and if they're not, we can only vet them one at a time
		//through the unity inspector.
		
		if(relevantRenderers.Count == 0)
		{
			EditorGUILayout.LabelField("No mesh renderers found!");
		}
		else if(someTargetsAreUnvetted && (targets.Length > 1))
		{
			EditorGUILayout.LabelField("Cannot multi-edit: Some objects have multiple");
			EditorGUILayout.LabelField("meshes. Please vet them individually.");
		}
		else if(someTargetsHaveMultipleRenderers && (targets.Length == 1))
		{
			EditorGUILayout.LabelField("This object has multiple meshes. Specify the primary.");
			
			int selectedRenderer = 0;
			
			GameObject explicitlySelectedMeshHolder = explicitlySelectedMeshHolderProperty.objectReferenceValue as GameObject;
			
			if(explicitlySelectedMeshHolder != null)
			{
				Renderer r = explicitlySelectedMeshHolder.GetComponent<Renderer>();
				if(r != null)
				{
					selectedRenderer = allRenderers.IndexOf(r);
				}
			}

			string[] displayedOptions = new string[allRenderers.Count];
			for(int i = 0; i < displayedOptions.Length; i++)
			{
				displayedOptions[i] = allRenderers[i].name;
			}
			
			selectedRenderer = EditorGUILayout.Popup("Slice Mesh", selectedRenderer, displayedOptions);
			
			Renderer renderer = allRenderers[selectedRenderer];
			
			explicitlySelectedMeshHolderProperty.objectReferenceValue = renderer.gameObject;
		}
		
		serializedObject.ApplyModifiedProperties();
		
		//Assuming we're all legit, let's multi-edit the infillers.
		
		if(!someTargetsAreUnvetted)
		{	
			List<Material> mats = new List<Material>();
			
			foreach(Renderer r in relevantRenderers)
			{
				Material[] _mats = r.sharedMaterials;
				foreach(Material mat in _mats)
				{
					if(mats.Contains(mat) == false) mats.Add(mat);
				}
			}
			
			if(mats.Count > 0)
			{
				EditorGUILayout.LabelField("For each material, define what region is used for infill.");
			}
			
		}
		
		if(!someTargetsAreUnvetted)
		{
			var mats = new List<Material>();
			var preexistingInfillers = new List<InfillConfiguration>();
			
			foreach(Object o in targets)
			{
				Sliceable s = o as Sliceable;

				Renderer renderer;

				if(s.explicitlySelectedMeshHolder != null) {
					renderer = s.explicitlySelectedMeshHolder.GetComponent<Renderer>();
				}
				else {
					renderer = s.gameObject.GetComponent<Renderer>();
				}

				if(renderer != null) {
					Material[] _mats = renderer.sharedMaterials;
					
					foreach(Material mat in _mats)
					{
						if(mats.Contains(mat) == false)
						{
							mats.Add(mat);
						}
					}
				}

				preexistingInfillers.AddRange(s.infillers);
			}
			
			InfillConfiguration[] infillers = new InfillConfiguration[ mats.Count ];

			var forceDirty = false;

			for(int i = 0; i < mats.Count; i++)
			{
				Material mat = mats[i];
				
				InfillConfiguration infiller = null;
				
				foreach(var _infiller in preexistingInfillers)
				{
					if(_infiller.material == mat)
					{
						infiller = _infiller;
						break;
					}
				}

				//If there is no infiller, than the UI will create one. However, the GUI will not be seen as changed, and
				//therefore if we do not set some flag, than the code lower down will not recognize that it ought to
				//set the item as 'dirty'.

				if(infiller == null)
				{
					infiller = new InfillConfiguration();
					infiller.material = mat;

					infiller.regionForInfill = new Rect(0f, 0f, 1f, 1f);
					forceDirty = true;
				}
				
				infillers[i] = infiller;
			}
			
			foreach(var infiller in infillers)
			{
				EditorGUILayout.Separator();
				
				EditorGUILayout.LabelField("Material: " + infiller.material.name);

				infiller.regionForInfill = EditorGUILayout.RectField("Region for infill", infiller.regionForInfill);
			}
			
			if(GUI.changed || forceDirty)
			{
				foreach(Object o in targets)
				{
					Sliceable s = o as Sliceable;
					
					s.infillers = new InfillConfiguration[ infillers.Length ];
					
					System.Array.Copy(infillers, s.infillers, infillers.Length);
					
					EditorUtility.SetDirty(o);
				}
			}
		}
    }

}
