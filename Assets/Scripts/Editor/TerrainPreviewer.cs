using UnityEditor;
using UnityEngine;

namespace TerrainEditor
{
    public class TerrainPreviewer : EditorWindow
    {
        bool generated = true;




        [MenuItem("Terrain/Terrain Previewer")]
        public static void ShowWindow()
        {
            GetWindow(typeof(TerrainPreviewer));
        }

        public void OnGUI()
        {

            if (GUILayout.Button("Generate"))
            {
            }

            GUI.enabled = generated;

            if (GUILayout.Button("Remove Chunks"))
            {

            }

            GUI.enabled = true;
        }
    }




}