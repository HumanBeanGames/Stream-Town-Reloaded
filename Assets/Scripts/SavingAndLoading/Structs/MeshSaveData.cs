using UnityEngine;

namespace SavingAndLoading.Structs
{
    /// <summary>
    /// Struct for converting Unitys Mesh into a custom data container
    /// </summary>
    [System.Serializable]
    public struct MeshSaveData
    {
        public Vector3SaveData[] Verticies;
        public int[] Triangles;
        public Vector2SaveData[] UVs;

        /// <summary>
        /// Overloaded constructor,
        /// Takes in seperate Mesh information
        /// </summary>
        /// <param name="verts">The mesh verticies</param>
        /// <param name="tris">The mesh triangles</param>
        /// <param name="uvs">The mesh UVs</param>
        public MeshSaveData(Vector3SaveData[] verts, int[] tris, Vector2SaveData[] uvs)
        {
            Verticies = verts;
            Triangles = tris;
            UVs = uvs;
        }

        /// <summary>
        /// Overloaded constructor,
        /// Takes in Unitys mesh and converts it to MeshSaveData
        /// </summary>
        /// <param name="mesh">The mezsh to convert</param>
        public MeshSaveData(Mesh mesh)
        {
            Verticies = Vector3SaveData.ToVector3SaveDataArray(mesh.vertices);
            Triangles = mesh.triangles;
            UVs = Vector2SaveData.ToVector2SaveDataArray(mesh.uv);
        }

        /// <summary>
        /// Creates a mesh from MeshSaveData,
        /// </summary>
        /// <returns>Unitys mesh version of MeshSaveData</returns>
        public Mesh GetMeshFromData()
        {
            Mesh mesh = new Mesh();
            mesh.vertices = Vector3SaveData.ToUnityVector3Array(Verticies);
            mesh.uv = Vector2SaveData.ToUnityVector2Array(UVs);
            mesh.triangles = Triangles;
            mesh.RecalculateNormals();
            mesh.name = "Loaded Mesh";
            return mesh;
        }

        /// <summary>
        /// Creates a mesh from MeshSaveData,
        /// </summary>
        /// <param name="data">MeshSaveData to convert to Unityss Mesh</param>
        /// <returns>Unitys mesh version of MeshSaveData</returns>
        public static Mesh ToUnityMesh(MeshSaveData data)
		{
            return data.GetMeshFromData();
		}
    }
}