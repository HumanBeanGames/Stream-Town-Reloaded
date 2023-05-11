using UnityEngine;
using World.Generation;

namespace Utils
{
	/// <summary>
	/// A static class that handles all Noise related functions.
	/// </summary>
	public static class Noise
	{
		/// <summary>
		/// Returns a noisemap from the fed in Generation Settings.
		/// </summary>
		/// <param name="generationSettings"></param>
		/// <returns></returns>
		public static float[,] GenerateNoiseMap(GenerationSettings generationSettings)
		{
			return GenerateNoiseMap(generationSettings.Size, generationSettings.Size, generationSettings.Seed, generationSettings.NoiseScale, generationSettings.Octaves, generationSettings.Persistance, generationSettings.Lacunarity, generationSettings.Offset, generationSettings.MeshHeightMultiplier);
		}

		/// <summary>
		/// Returns a generated noise map using perlin noise.
		/// </summary>
		/// <param name="mapWidth"></param>
		/// <param name="mapHeight"></param>
		/// <param name="seed"></param>
		/// <param name="scale"></param>
		/// <param name="octaves"></param>
		/// <param name="persistance"></param>
		/// <param name="lacunarity"></param>
		/// <param name="offset"></param>
		/// <param name="multiplier"></param>
		/// <returns></returns>
		public static float[,] GenerateNoiseMap(int mapWidth, int mapHeight, int seed, float scale, int octaves, float persistance, float lacunarity, Vector2 offset, float multiplier = 1)
		{
			float[,] noiseMap = new float[mapWidth, mapHeight];

			System.Random randNum = new System.Random(seed);
			Vector2[] octaveOffsets = new Vector2[octaves];

			for (int i = 0; i < octaves; i++)
			{
				float offsetX = randNum.Next(-100000, 100000) + offset.x;
				float offsetY = randNum.Next(-100000, 100000) + offset.y;
				octaveOffsets[i] = new Vector2(offsetX, offsetY);
			}

			if (scale <= 0)
			{
				scale = 0.0001f;
			}

			float maxNoiseHeight = float.MinValue;
			float minNoiseHeight = float.MaxValue;

			float halfWidth = mapWidth * 0.5f;
			float halfHeight = mapHeight * 0.5f;

			for (int y = 0; y < mapHeight; y++)
			{
				for (int x = 0; x < mapWidth; x++)
				{
					float amplitude = 1;
					float frequency = 1;
					float noiseHeight = 0;

					for (int i = 0; i < octaves; i++)
					{
						float sampleX = (x - halfWidth) / scale * frequency + octaveOffsets[i].x;
						float sampleY = (y - halfHeight) / scale * frequency + octaveOffsets[i].y;

						float perlinValue = Mathf.PerlinNoise(sampleX, sampleY) * 2 - 1;
						noiseHeight += perlinValue * amplitude;

						amplitude *= persistance;
						frequency *= lacunarity;
					}

					if (noiseHeight > maxNoiseHeight)
					{
						maxNoiseHeight = noiseHeight;
					}
					else if (noiseHeight < minNoiseHeight)
					{
						minNoiseHeight = noiseHeight;
					}

					noiseMap[x, y] = noiseHeight;
				}
			}

			for (int y = 0; y < mapHeight; y++)
			{
				for (int x = 0; x < mapWidth; x++)
				{
					noiseMap[x, y] = Mathf.InverseLerp(minNoiseHeight, maxNoiseHeight, noiseMap[x, y]) * multiplier;
				}
			}

			return noiseMap;
		}
	}
}