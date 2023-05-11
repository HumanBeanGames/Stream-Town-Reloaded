using UnityEngine;

namespace World.Generation
{
	/// <summary>
	/// Holds the Mesh Data for use in World Generation.
	/// </summary>
	[System.Serializable]
	public class MeshData
	{
		public Vector3[] Vertices;
		public int[] Triangles;
		public Vector2[] UVs;
		int TriangleIndex;

		// Constructor.
		public MeshData(int width, int height)
		{
			Vertices = new Vector3[width * height];
			UVs = new Vector2[width * height];
			Triangles = new int[(height - 1) * (height - 1) * 6];
		}

		/// <summary>
		/// Adds a triangle to the mesh.
		/// </summary>
		/// <param name="a"></param>
		/// <param name="b"></param>
		/// <param name="c"></param>
		public void AddTriangle(int a, int b, int c)
		{
			Triangles[TriangleIndex] = a;
			Triangles[TriangleIndex + 1] = b;
			Triangles[TriangleIndex + 2] = c;
			TriangleIndex += 3;
		}

		/// <summary>
		/// Returns a generated mesh with calculated normals.
		/// </summary>
		/// <returns></returns>
		public Mesh CreateMesh()
		{
			Mesh mesh = new Mesh();
			mesh.vertices = Vertices;
			mesh.triangles = Triangles;
			mesh.uv = UVs;
			mesh.RecalculateNormals();
			return mesh;
		}
	}
}