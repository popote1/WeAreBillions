using UnityEditor;
using UnityEngine;

public class EditorWindowGameObjectsRenamer : EditorWindow
{

    private string _nameBase;
    private float _collidersRescale =1;
    [MenuItem("PopoteTools/GameObjectRenamer")]
    public static void ShowWindow() {
        EditorWindowGameObjectsRenamer window = GetWindow<EditorWindowGameObjectsRenamer>();
        window.titleContent = new GUIContent("GameObject Renamer");
    }

    
    private void OnGUI() {
        
        GUILayout.Label(" Object Selection = "+Selection.count);
        GUILayout.Space(10);
        GUILayout.Label("Rename Part");
        _nameBase =GUILayout.TextArea(_nameBase);
        
        if (GUILayout.Button("Rename")) {
            //Undo.SetCurrentGroupName("Renaming Objects");
            for (int i = 0; i < Selection.gameObjects.Length; i++) {
                Undo.RegisterFullObjectHierarchyUndo( Selection.gameObjects[i],"Renaming Objects");
                Selection.gameObjects[i].name = _nameBase + i;
            }
        }
        GUILayout.Space(10);
        GUILayout.Label("Colliders Rescale");
        _collidersRescale = EditorGUILayout.Slider(_collidersRescale, 0, 2);
        //_collidersRescale = GUILayout.TextField(_collidersRescale.ToString())
        //_collidersRescale = GUILayout.HorizontalSlider(_collidersRescale,0,2);
        
        if (GUILayout.Button("Rescale"))
        {
            for (int i = 0; i < Selection.gameObjects.Length; i++) {
                BoxCollider  boxCollider= Selection.gameObjects[i].GetComponent<BoxCollider>();
                if (boxCollider != null) {
                    Undo.RegisterFullObjectHierarchyUndo( boxCollider,"Renaming Objects");
                    boxCollider.size *= _collidersRescale;
                }
            }
        }
        
        
        
    }
}
