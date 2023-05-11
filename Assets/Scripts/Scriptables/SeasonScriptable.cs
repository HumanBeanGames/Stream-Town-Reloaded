using UnityEngine;
using UnityEngine.VFX;
using Utils;

namespace Scriptables 
{
    [CreateAssetMenu(fileName = "SeasonScriptableData", menuName = "ScriptableObjects/SeasonScriptable", order = 1)]
    public class SeasonScriptable : ScriptableObject 
	{
		public Season Season;

		[Header("Grass Colors")]
		public Color GrassGridColor1;
		public Color GrassGridColor2;
		public Color GrassTopColor;
		public Color GrassWindColor;

		[Header("Terrain Colors")]
		public Color TerrainColor1;
		public Color TerrainColor2;

		[Header("Tree Colors")]
		public Gradient TreeColorGradient;

		[Header("VFX")]
		[HideInInspector]
		public VisualEffect VFX;
		public int MaxParticleCount;
		public float ParticleLerpTime = 3;
		public float MinRunTime;
		public float MaxRunTime;
    }
}