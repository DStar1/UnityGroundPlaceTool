using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

/*
** Ability to raycast while in editor mode, not in play mode
** (Unfinished)
*/

public class GroundPlaceEditorTool : EditorWindow
{
    bool placedObjects = true;
    GameObject objectInstantiate;

    // Add menu named "Ground Place" to the My Tools menu
    [MenuItem("My Tools/Ground Place")]
    static void Init()
    {
        // Get existing open window or if none, make a new one:
        GroundPlaceEditorTool window = (GroundPlaceEditorTool)EditorWindow.GetWindow(typeof(GroundPlaceEditorTool));
        window.Show();
    }

    private void OnEnable() {
        SceneView.onSceneGUIDelegate -= CustomUpdate;
        SceneView.onSceneGUIDelegate += CustomUpdate;
    }

    void OnGUI()
    {
        // GUILayout.Label("Base Settings", EditorStyles.boldLabel);
        // myString = EditorGUILayout.TextField("Text Field", myString);

        // groupEnabled = EditorGUILayout.BeginToggleGroup("Optional Settings", groupEnabled);
        // myBool = EditorGUILayout.Toggle("Toggle", myBool);
        // myFloat = EditorGUILayout.Slider("Slider", myFloat, -3, 3);
        // EditorGUILayout.EndToggleGroup();
        placedObjects = EditorGUILayout.Toggle (placedObjects);
        objectInstantiate = (GameObject)EditorGUILayout.ObjectField(objectInstantiate, typeof(GameObject), true);
    }

    void CustomUpdate(UnityEditor.SceneView sv) {
        Event e  = Event.current;
        if (placedObjects && (e.type == EventType.MouseDrag || e.type == EventType.MouseDown) && e.button == 0) {


            // TODO: Having a problem with prefabs colliding with eachother

            // Bit shift the index of the layer (8) to get a bit mask (from Unity site)
            int layerMask = 1 << 8;

            // This would cast rays only against colliders in layer 8.
            // But instead we want to collide against everything except layer 8. The ~ operator does this, it inverts a bitmask.
            layerMask = ~layerMask;


            RaycastHit hit;
            // Tools.current = Tool.view;
            Debug.Log("mouse clicked!");

            if (Physics.Raycast(Camera.current.ScreenPointToRay(new Vector3(e.mousePosition.x, Camera.current.pixelHeight - e.mousePosition.y, 0) ),
                 out hit, Mathf.Infinity, layerMask)) {
            // if (Physics.Raycast(Camera.current.ScreenPointToRay(new Vector3(e.mousePosition.x, Camera.current.pixelHeight - e.mousePosition.y, 0) ),
            //                                                         out hit, Mathf.Infinity, ~LayerMask.NameToLayer("Cube")  )) {
                Debug.Log("In if statement");
                Debug.Log(hit.point);
                // GameObject placedObject = (GameObject)PrefabUtility.InstantiatePrefab( PrefabUtility.GetPrefabParent(objectInstantiate) );
                // GameObject placedObject = (GameObject)PrefabUtility.InstantiatePrefab( PrefabUtility.GetCorrespondingObjectFromSource(objectInstantiate) );
                // GameObject placedObject = Instantiate(objectInstantiate);
                // GameObject placedObject = (GameObject)PrefabUtility.GetCorrespondingObjectFromSource(objectInstantiate);
                GameObject placedObject = (GameObject)PrefabUtility.InstantiatePrefab( objectInstantiate) ;
                placedObject.transform.position = hit.point;
                placedObject.transform.localScale = new Vector3(.1f,.1f,.1f);// * Random.range(0.5f, 2.0f);
                e.Use();
                Undo.RegisterCreatedObjectUndo(placedObject, "undo sphere Harrison");            
            }


        }
    }
}
