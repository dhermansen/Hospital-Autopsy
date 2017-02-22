using UnityEngine;
using System.Collections.Generic;

namespace NobleMuffins.TurboSlicer.Guts
{
	public class MeshSnapshot
	{
		public MeshSnapshot (Vector3[] vertices, Vector3[] normals, Vector2[] coords, Vector2[] coords2, Vector4[] tangents, int[][] indices, Rect[] infillBySubmesh)
		{
			this.vertices = vertices;
			this.normals = normals;
			this.coords = coords;
			this.coords2 = coords2;
			this.tangents = tangents;
			this.indices = indices;
			this.infillBySubmesh = infillBySubmesh;
		}

		public readonly Vector3[] vertices;
		public readonly Vector3[] normals;
		public readonly Vector2[] coords;
		public readonly Vector2[] coords2;
		public readonly Vector4[] tangents;

		public readonly int[][] indices;
		public readonly Rect[] infillBySubmesh;

		public MeshSnapshot WithVertices(Vector3[] figure) {
			return new MeshSnapshot(figure, normals, coords, coords2, tangents, indices, infillBySubmesh);
		}

		public MeshSnapshot WithNormals(Vector3[] figure) {
			return new MeshSnapshot(vertices, figure, coords, coords2, tangents, indices, infillBySubmesh);
		}

		public MeshSnapshot WithCoords(Vector2[] figure) {
			return new MeshSnapshot(vertices, normals, figure, coords2, tangents, indices, infillBySubmesh);
		}

		public MeshSnapshot WithCoords2(Vector2[] figure) {
			return new MeshSnapshot(vertices, normals, coords, figure, tangents, indices, infillBySubmesh);
		}

		public MeshSnapshot WithTangents(Vector4[] figure) {
			return new MeshSnapshot(vertices, normals, coords, coords2, figure, indices, infillBySubmesh);
		}

		public MeshSnapshot WithIndices(int[][] figure) {
			return new MeshSnapshot(vertices, normals, coords, coords2, tangents, figure, infillBySubmesh);
		}

		public MeshSnapshot WithInfillBySubmesh(Rect[] figure) {
			return new MeshSnapshot(vertices, normals, coords, coords2, tangents, indices, figure);
		}
	}
}