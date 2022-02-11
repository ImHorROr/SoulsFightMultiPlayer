using UnityEditor;
using UnityEditor.Compilation;
using UnityEngine;

public class ForceRecompile : EditorWindow
{


    [MenuItem("Window/" + nameof(ForceRecompile))]
    private static void ShowWindow()
    {
        GetWindow<ForceRecompile>();
    }

    private void OnGUI()
    {
        if (GUILayout.Button("Request Script Compilation"))
        {
            CompilationPipeline.RequestScriptCompilation();
        }
    }
}