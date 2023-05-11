using UnityEngine;
using UnityEditor;

namespace Environment
{
    /// <summary>
    /// Editor tool for wind controller
    /// </summary>
    [CustomEditor(typeof(WindController))]
    public class WindControllerEditor : Editor 
	{
        public override void OnInspectorGUI()
        {
            DrawDefaultInspector();

            WindController controller = (WindController)target;

            GUILayout.Space(25);
           
            //Creates a button that will align each materials wind direction to the same onced pressed
            if (GUILayout.Button(new GUIContent("Align Wind", "The direction of the wind is based of the rotation of the this")))
            {
                controller.AlignWind();
            }
           
            GUILayout.Space(5);

            //Creates a button to change each materials wind to the same
            if(GUILayout.Button("Change Wind Strength"))
            {
                controller.ChangeWindStrength();
            }

            GUILayout.Space(5);
            
            if(GUILayout.Button("Change Texture Size"))
            {
                controller.ChangeWindTextureSize();
            }
        }
    }
}