using UnityEngine.UI;
using UnityEngine;

namespace UserInterface 
{
    public class UILineRenderer : Graphic
	{
		[SerializeField]
		private float _thickness;

		[SerializeField]
		private Vector2 _start;
		[SerializeField]
		private Vector2 _end;

		public float Thickness
		{
			get => _thickness;
			set => _thickness = value;
		}

		public Vector2 StartVec
		{
			get => _start;
			set => _start = value;
		}

		public Vector2 EndVec
		{
			get => _end;
			set => _end = value;
		}

		protected override void OnPopulateMesh(VertexHelper vh)
		{
			vh.Clear();

			float angle = GetAngle(_start, _end) + 90f;
			DrawVerticesForPoint(_start, _end, angle, vh);

			vh.AddTriangle(0, 1, 2);
			vh.AddTriangle(1, 2, 3);
		}

		public float GetAngle(Vector2 self, Vector2 target)
		{
			return (float)(Mathf.Atan2(Screen.height * (target.y - self.y), Screen.width * (target.x - self.x)) * (180 / Mathf.PI));
		}

		private void DrawVerticesForPoint(Vector2 p1, Vector2 p2, float angle, VertexHelper vh)
		{
			UIVertex vertex = UIVertex.simpleVert;
			vertex.color = color;

			vertex.position = Quaternion.Euler(0, 0, angle) * new Vector3(-_thickness / 2, 0);
			vertex.position += new Vector3(p1.x, p1.y);
			vh.AddVert(vertex);

			vertex.position = Quaternion.Euler(0, 0, angle) * new Vector3(_thickness / 2, 0);
			vertex.position += new Vector3(p1.x, p1.y);
			vh.AddVert(vertex);

			vertex.position = Quaternion.Euler(0, 0, angle) * new Vector3(-_thickness / 2, 0);
			vertex.position += new Vector3(p2.x, p2.y);
			vh.AddVert(vertex);

			vertex.position = Quaternion.Euler(0, 0, angle) * new Vector3(_thickness / 2, 0);
			vertex.position += new Vector3(p2.x, p2.y);
			vh.AddVert(vertex);
		}
	}
}