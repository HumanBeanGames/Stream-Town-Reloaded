using UnityEngine;
using Utils;

namespace World.Generation
{
	/// <summary>
	/// Holds the Generation Settings Data for Resource Generation.
	/// </summary>
	[System.Serializable]
	public class ResourceGenerationSettings : GenerationSettings
	{
		public TargetMask TargetType;
		public string PoolName;

		public ResourceGenerationSettings(int size, int levelOfDetail, float noiseScale, int octaves, float persistance, float lacunarity, int seed, Vector2 offset, float meshHeightMultiplier, AnimationCurve meshHeightCurve)
			: base(size, levelOfDetail, noiseScale, octaves, persistance, lacunarity, seed, offset, meshHeightMultiplier, meshHeightCurve)
		{
			Size = size;
			LevelOfDetail = levelOfDetail;
			NoiseScale = noiseScale;
			Octaves = octaves;
			Persistance = persistance;
			Lacunarity = lacunarity;
			Seed = seed;
			Offset = offset;
			MeshHeightMultiplier = meshHeightMultiplier;
			MeshHeightCurve = meshHeightCurve;
			HeightMap = new float[size, size];
		}

		/// <summary>
		/// Returns the name of the Pooled Object from the Generation Settings
		/// </summary>
		/// <returns></returns>
		public override string GetPoolName()
		{
			return PoolName;
		}
	}
}