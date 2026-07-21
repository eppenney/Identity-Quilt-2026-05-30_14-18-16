using System.Collections;
using System.Collections.Generic;

using UnityEngine;

namespace Broccoli.Utils
{
    public class MeshUtils {
        /// <summary>
		/// Packs a Vector3 to a float value using 1/16 precision.
		/// </summary>
		/// <param name="input">Input Vector3.</param>
		/// <returns>Packed Vector3 to float.</returns>
		public static float Vector3ToFloat (Vector3 input) {
			float result = Mathf.Floor((input.normalized.x * 0.5f + 0.5f) * 15.9999f);
			result += Mathf.Floor((input.normalized.y * 0.5f + 0.5f) * 15.9999f) * 0.0625f;
			result += Mathf.Floor((input.normalized.z * 0.5f + 0.5f) * 15.9999f) * 0.00390625f;
			return result;
		}
        public static Mesh GetPlane (
            float width, 
            float height, 
            int widthCells, 
            int heightCells,
            float pivotW = 0.5f,
            float pivotH = 0f) 
        {
            Vector3[] planeVertices = new Vector3 [(widthCells + 1) * (heightCells + 1)];
			Vector3[] planeNormals = new Vector3 [planeVertices.Length];
			Vector2[] planeUVs = new Vector2 [planeVertices.Length];
            int[] triangles = new int[widthCells * heightCells * 6];

            // Starting position for the grid on the width.
			float posW = -pivotW * width;
			// Starting position for the grid on the height.
			float posH = -pivotH * height;
            // Max diagonal length on the plane.
			float maxLength = 1f;
			float widthFromPivot, heightFromPivot;
			if (pivotW * width > width - (pivotW * width)) {
				widthFromPivot = pivotW * width;
			} else {
				widthFromPivot = width - (pivotW * width);
			}
			if (pivotH * height > height - (pivotH * height)) {
				heightFromPivot = pivotH * height;
			} else {
				heightFromPivot = height - (pivotH * height);
			}
			maxLength = Mathf.Sqrt (Mathf.Pow (widthFromPivot, 2) + Mathf.Pow (heightFromPivot, 2));
			// Value for each segment on the width.
			float segmentW = width / widthCells;
			// Value for each segment on the height.
			float segmentH = height / heightCells;
			// Positions in space for the plane.
			float posX = 0f;
			float posY = posH;
			float posZ = posW;
			float posU = 0f;
			float posV = 0f;
			// Points count.
			int pointCount = 0;

            for (int j = 0; j <= heightCells; j++) {
                for (int i = 0; i <= widthCells; i++) {
                    planeVertices [pointCount] = new Vector3 (posX, posY, posZ);
                    posU = (posZ - posW) / width;
                    posV = (posY - posH) / height;
                    planeNormals [pointCount] = Vector3.right;
                    planeUVs [pointCount] = new Vector4 (1f - posV, posU, 1f - posV, posU);
                    /*
                    } else if (planeDef.uvSteps == 2) {
                        planeUVs [pointCount] = new Vector4 (1f - posU, 1f - posV, 1f - posU, 1f - posV);
                    } else if (planeDef.uvSteps == 3) {
                        planeUVs [pointCount] = new Vector4 (posV, 1f - posU, posV, 1f - posU);
                    } else {
                        planeUVs [pointCount] = new Vector4 (posU, posV, posU, posV);
                    }
                    */
                    posZ += segmentW;
                    pointCount++;
                }
                posZ = posW;
                posY += segmentH;
            }
            
            // Add triangles
            int k = 0;
			for (int j = 0; j < heightCells; j++) {
				for (int i = 0; i < widthCells; i++) {
					pointCount = i + (j * (widthCells + 1));
					triangles [k] = pointCount;
					triangles [k + 1] = pointCount + widthCells + 1;
					triangles [k + 2] = pointCount + widthCells + 2;
					triangles [k + 3] = pointCount;
					triangles [k + 4] = pointCount + widthCells + 2;
					triangles [k + 5] = pointCount + 1;
                    k += 6;
				}
			}

            Mesh mesh = new Mesh ();
            mesh.SetVertices (planeVertices);
            mesh.SetNormals (planeNormals);
            mesh.SetUVs (0, planeUVs);
            mesh.SetTriangles (triangles, 0);
            mesh.RecalculateTangents ();

            return mesh;
        }
        /// <summary>
        /// Combines all the submeshes of an existing mesh.
        /// </summary>
        /// <param name="mesh">Mesh to combine its submeshes.</param>
        /// <returns>Combined mesh.</returns>
        public static Mesh CombineSubMeshes (Mesh mesh) {
            Mesh combined = new Mesh ();
            List<CombineInstance> combines = new List<CombineInstance> ();
            CombineInstance combine;
            for (int i = 0; i < mesh.subMeshCount; i++) {
                combine = new CombineInstance ();
                combine.mesh = mesh;
                combine.transform = Matrix4x4.identity;
                combine.subMeshIndex = i;
                combines.Add (combine);
            }
            combined.CombineMeshes (combines.ToArray (), true, false);
            return combined;
        }
        /// <summary>
        /// Takes the same submesh per mesh from a list and combines them into a single mesh.
        /// </summary>
        /// <param name="meshes">List of meshes.</param>
        /// <param name="subMeshIndex">Submesh index.</param>
        /// <returns>Combined mesh for the index submesh.</returns>
        public static Mesh CombineMeshes (List<Mesh> meshes, int subMeshIndex) {
            Mesh mergingMesh = new Mesh ();
			mergingMesh.subMeshCount = meshes.Count;
            List<CombineInstance> combines = new List<CombineInstance> ();
            CombineInstance combine;
            for (int i = 0; i < meshes.Count; i++) {
                combine = new CombineInstance ();
                combine.mesh = meshes [i];
                combine.transform = Matrix4x4.identity;
                combine.subMeshIndex = subMeshIndex;;
                combines.Add (combine);
            }
			mergingMesh.CombineMeshes (combines.ToArray (), true, false);
            return mergingMesh;
        }
        public static Mesh CombineMeshes (List<Mesh> meshes, bool mergeSubmeshes = false) {
            Mesh mergingMesh = new Mesh ();
			mergingMesh.subMeshCount = meshes.Count;
            List<CombineInstance> combines = new List<CombineInstance> ();
            CombineInstance combine;
            for (int i = 0; i < meshes.Count; i++) {
                for (int j = 0; j < meshes [i].subMeshCount; j++) {
                    combine = new CombineInstance ();
                    combine.mesh = meshes [i];
                    combine.transform = Matrix4x4.identity;
                    combine.subMeshIndex = j;
                    combines.Add (combine);
                }
            }
			mergingMesh.CombineMeshes (combines.ToArray (), mergeSubmeshes, false);
            return mergingMesh;
        }
        /// <summary>
        /// Combine all meshes keeping each one as a submesh of the resulting mesh.
        /// </summary>
        /// <param name="meshes"></param>
        /// <returns></returns>
        public static Mesh CombineMeshesAdditive (List<Mesh> meshes) {
            Mesh mergingMesh = new Mesh ();
			mergingMesh.subMeshCount = meshes.Count;
            List<CombineInstance> combines = new List<CombineInstance> ();
            CombineInstance combine;
            for (int i = 0; i < meshes.Count; i++) {
                for (int j = 0; j < meshes [i].subMeshCount; j++) {
                    combine = new CombineInstance ();
                    combine.mesh = meshes [i];
                    combine.transform = Matrix4x4.identity;
                    combine.subMeshIndex = j;
                    combines.Add (combine);
                }
            }
			mergingMesh.CombineMeshes (combines.ToArray (), false, false);
            return mergingMesh;
        }
        public static Mesh CombineMeshesMultiplicative (Mesh srcMesh, int factor, bool mergeSubmeshes = false) {
            if (factor < 0) factor = 0;
            Mesh mergingMesh = new Mesh ();
			mergingMesh.subMeshCount = srcMesh.subMeshCount;
            List<CombineInstance> combines = new List<CombineInstance> ();
            CombineInstance combine;
            for (int i = 0; i < factor; i++) {
                for (int j = 0; j < srcMesh.subMeshCount; j++) {
                    combine = new CombineInstance ();
                    combine.mesh = srcMesh;
                    combine.transform = Matrix4x4.identity;
                    combine.subMeshIndex = j;
                    combines.Add (combine);
                }
            }
			mergingMesh.CombineMeshes (combines.ToArray (), mergeSubmeshes, false);
            return mergingMesh;   
        }
        /// <summary>
        /// Creates a new mesh by applying a position, rotation, and scale transformation to a source mesh's vertices and normals.
        /// This is useful for "baking" a transform into the mesh data itself.
        /// </summary>
        /// <param name="sourceMesh">The original mesh to transform.</param>
        /// <param name="position">The translation to apply.</param>
        /// <param name="rotation">The rotation to apply.</param>
        /// <param name="scale">The scale to apply.</param>
        /// <returns>A new Mesh object with the transformations applied, or null if the source mesh is null.</returns>
        public static Mesh ApplyTransformToMesh(Mesh sourceMesh, Vector3 position, Quaternion rotation, Vector3 scale)
        {
            if (sourceMesh == null)
            {
                Debug.LogError("Source Mesh cannot be null.");
                return null;
            }

            // 1. Create a new mesh to hold the result to avoid modifying the original asset.
            Mesh transformedMesh = new Mesh();
            // It's good practice to give the new mesh a name for easier debugging.
            transformedMesh.name = sourceMesh.name + "_Transformed";

            // 2. Get the transformation matrix from the inputs.
            Matrix4x4 matrix = Matrix4x4.TRS(position, rotation, scale);

            // 3. Transform vertices.
            Vector3[] sourceVertices = sourceMesh.vertices;
            Vector3[] newVertices = new Vector3[sourceVertices.Length];
            for (int i = 0; i < sourceVertices.Length; i++)
            {
                // Use MultiplyPoint3x4 to correctly apply translation, rotation, and scale to a position.
                newVertices[i] = matrix.MultiplyPoint3x4(sourceVertices[i]);
            }
            transformedMesh.vertices = newVertices;

            // The matrix for transforming normals and tangents is the inverse transpose of the main matrix.
            // This handles non-uniform scaling correctly.
            Matrix4x4 normalMatrix = matrix.inverse.transpose;

            // 4. Transform normals (if they exist).
            Vector3[] sourceNormals = sourceMesh.normals;
            if (sourceNormals != null && sourceNormals.Length > 0)
            {
                Vector3[] newNormals = new Vector3[sourceNormals.Length];
                for (int i = 0; i < sourceNormals.Length; i++)
                {
                    // Use MultiplyVector to apply rotation and scale, but not translation, to a direction.
                    // Normalizing is crucial to ensure the vector remains a unit vector.
                    newNormals[i] = normalMatrix.MultiplyVector(sourceNormals[i]).normalized;
                }
                transformedMesh.normals = newNormals;
            }
            
            // 5. Transform tangents (if they exist).
            Vector4[] sourceTangents = sourceMesh.tangents;
            if (sourceTangents != null && sourceTangents.Length > 0)
            {
                Vector4[] newTangents = new Vector4[sourceTangents.Length];
                for (int i = 0; i < sourceTangents.Length; i++)
                {
                    Vector3 tangentDir = new Vector3(sourceTangents[i].x, sourceTangents[i].y, sourceTangents[i].z);
                    tangentDir = normalMatrix.MultiplyVector(tangentDir).normalized;
                    // Preserve the 'w' component which stores handedness information.
                    newTangents[i] = new Vector4(tangentDir.x, tangentDir.y, tangentDir.z, sourceTangents[i].w);
                }
                transformedMesh.tangents = newTangents;
            }

            // 6. Copy other mesh data (triangles, UVs, etc.) which are not affected by this transform.
            transformedMesh.triangles = sourceMesh.triangles;
            transformedMesh.uv = sourceMesh.uv;
            transformedMesh.uv2 = sourceMesh.uv2;
            transformedMesh.uv3 = sourceMesh.uv3;
            transformedMesh.uv4 = sourceMesh.uv4;
            transformedMesh.colors = sourceMesh.colors;

            // 7. Recalculate bounds to fit the new transformed vertices.
            transformedMesh.RecalculateBounds();

            return transformedMesh;
        }
    }
}
