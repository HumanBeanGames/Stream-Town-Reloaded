using System;
using UnityEngine;
using Utils;

namespace World.Generation
{
	/// <summary>
	/// Used for generating meshes from a height map
	/// </summary>
	public static class ProceduralMeshGenerator
	{
		/// <summary>
		/// Generates Mesh Data from a GenerationSettings profile.
		/// </summary>
		/// <param name="settings"></param>
		/// <returns></returns>
		public static MeshData GenerateTerrainMeshData(GenerationSettings settings)
		{
			float[,] noiseMap = Noise.GenerateNoiseMap(settings);
			settings.HeightMap = new float[settings.Size, settings.Size];

			int dimension1 = noiseMap.GetLength(0) - 1;
			int dimension2 = noiseMap.GetLength(1) - 1;

			//Transform edge values to be 0
			Func<int, int, float> floatValueSetter = (i1, i2) =>
			{
				return (i1 <= 1 || i1 >= dimension1 - 1 || i2 <=  1 || i2 >= dimension2 - 1) ? -1 : noiseMap[i1, i2];
			};

			MathExtended.Set2DArrayValues<float>(ref noiseMap, floatValueSetter);

			float topLeftX = (settings.Size - 1) / -2f;
			float topLeftZ = (settings.Size - 1) / 2f;

			int meshSimplificationIncrement = (settings.LevelOfDetail == 0) ? 1 : settings.LevelOfDetail * 2;
			int verticesPerLine = (settings.Size - 1) / meshSimplificationIncrement + 1;
			int verticesPerColumn = (settings.Size - 1) / meshSimplificationIncrement + 1;

			MeshData meshData = new MeshData(verticesPerColumn, verticesPerLine);
			int vertexIndex = 0;

			for (int y = 0; y < settings.Size; y += meshSimplificationIncrement)
			{
				for (int x = 0; x < settings.Size; x += meshSimplificationIncrement)
				{
					settings.HeightMap[x, y] = settings.MeshHeightCurve.Evaluate(noiseMap[x, y]);
					meshData.Vertices[vertexIndex] = new Vector3(topLeftX + x, settings.HeightMap[x, y] * settings.MeshHeightMultiplier, topLeftZ - y);
					meshData.UVs[vertexIndex] = new Vector2(x / (float)settings.Size, y / (float)settings.Size);
					if (x < settings.Size - 1 && y < settings.Size - 1)
					{
						meshData.AddTriangle(vertexIndex, vertexIndex + verticesPerLine + 1, vertexIndex + verticesPerColumn);
						meshData.AddTriangle(vertexIndex + verticesPerLine + 1, vertexIndex, vertexIndex + 1);
					}
					vertexIndex++;
				}
			}

			return meshData;
		}

		/// <summary>
		/// Converts MeshData into a Mesh and applies it to a GameObject.
		/// </summary>
		/// <param name="data"></param>
		/// <param name="terrainObject"></param>
		public static Mesh CreateMesh(MeshData data, GameObject terrainObject)
		{
			return CreateMesh(data.CreateMesh(), terrainObject);
		}

		/// <summary>
		/// Applies mesh to a GameObject.
		/// </summary>
		/// <param name="data"></param>
		/// <param name="terrainObject"></param>
		public static Mesh CreateMesh(Mesh mesh, GameObject terrainObject)
		{
			MeshFilter filter;

			if (!terrainObject.TryGetComponent(out filter))
			{
				filter = terrainObject.AddComponent<MeshFilter>();
			}
			filter.sharedMesh = mesh;

			MeshCollider collider;

			if (!terrainObject.TryGetComponent(out collider))
			{
				collider = terrainObject.AddComponent<MeshCollider>();
			}

			collider.sharedMesh = mesh;
			return mesh;
		}
	}
}