using UnityEngine;

namespace World.Generation
{
	/// <summary>
	/// Generation settings for Foliage to be placed in the world.
	/// </summary>
	[System.Serializable]
	public class FoliageGenerationSettings : GenerationSettings
	{
		public string[] PoolNames;

		// Constructor.
		public FoliageGenerationSettings(int size, int levelOfDetail, float noiseScale, int octaves, float persistance, float lacunarity, int seed, Vector2 offset, float meshHeightMultiplier, AnimationCurve meshHeightCurve) : base(size, levelOfDetail, noiseScale, octaves, persistance, lacunarity, seed, offset, meshHeightMultiplier, meshHeightCurve)
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
		/// Returns a random pooled name.
		/// </summary>
		/// <returns></returns>
		public override string GetPoolName()
		{
			// TODO: This might need to change.
			return PoolNames[Random.Range(0, PoolNames.Length)];
		}
	}
}