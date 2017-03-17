using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using uFlex;

public class CuttingUtil
{
    public static Vector3 shape_to_world(int i, FlexShapeMatching fsm)
    {
        return fsm.m_shapeRotations[i] * fsm.m_shapeCenters[i] / 100.0f + fsm.m_shapeTranslations[i];
    }
    public static void world_to_shape(Vector3 pt, int i, FlexShapeMatching fsm)
    {
        fsm.m_shapeCenters[i] = Quaternion.Inverse(fsm.m_shapeRotations[i]) * (pt - fsm.m_shapeTranslations[i]) * 100.0f;
    }
    private struct CutVertex
    {
        public CutVertex(int shape_idx, int particle_idx) : this()
        {
            this.shape_idx = shape_idx;
            this.particle_idx = particle_idx;
        }

        int shape_idx;
        int particle_idx;
    }

    public static void CutFlexSoft(Transform target, Plane act_plane)
    {
        var fsm = target.GetComponent<FlexShapeMatching>();
        var fp = target.GetComponent<FlexParticles>();

        //The list of vertices that are on the same side as their shape center.
        var correct_side_indicies = new List<int>();
        //The list of vertices that are on a different side than the shape center.
        var wrong_side_indices = new List<int>();

        //Stores offsets of each shape in the indices array.  IOW we can determine which indices are split from each shape by plane
        var offsets = new List<int>();

        int shape_start = 0;
        //For each shape, determine which indices are split from each shape by plane.
        for (int shape_idx = 0; shape_idx < fsm.m_shapesCount; ++shape_idx)
        {
            int shape_end = fsm.m_shapeOffsets[shape_idx];

            var shape_center = shape_to_world(shape_idx, fsm);
            for (int j = shape_start; j < shape_end; ++j)
            {
                var particle_idx = fsm.m_shapeIndices[j];
                if (act_plane.SameSide(shape_center, fp.m_particles[particle_idx].pos))
                    correct_side_indicies.Add(j);
                else
                    wrong_side_indices.Add(j);
            }
            offsets.Add(correct_side_indicies.Count);
            shape_start = shape_end;
        }
        //Now, merge otherIndices into indices depending on which bone is on the same side?
        // Added indices cause the shape sizes to be larger, naturally
        foreach (var idx in wrong_side_indices)
        {
            if (!correct_side_indicies.Contains(idx))
            {
                int shape_index = find_closest_shape_on_same_side(
                    fsm.m_shapeCenters, fp.m_restParticles[idx].pos, act_plane);
                int new_shape_start_index = offsets[shape_index];
                correct_side_indicies.Insert(new_shape_start_index, idx);
                for (int j = shape_index; j < offsets.Count; j++)
                    offsets[j] += 1;
            }

        }

        //Now, store the modified shapes back into the flex object.
        fsm.m_shapeIndicesCount = correct_side_indicies.Count;
        fsm.m_shapeIndices = correct_side_indicies.ToArray();
        fsm.m_shapeOffsets = offsets.ToArray();

//        //Calculate centers for the newly reconfigured shapes
//        int shape_start = 0;
//        int shapeIndexOffset = 0;
//        for (int s = 0; s < fsm.m_shapesCount; s++)
//        {
//            //shapes.m_shapeTranslations[s] = new Vector3();
//            //shapes.m_shapeRotations[s] = Quaternion.identity;

//            int shapeEnd = fsm.m_shapeOffsets[s];

//            //For each particle in shape, calculate a new shape center.
//            int num_shapes = shapeEnd - shape_start;
//            fsm.m_shapeCenters[s] = fsm.m_shapeIndices.ToList().GetRange(shape_start, num_shapes)
//                .Aggregate(Vector3.zero, (lhs, p_idx) => lhs + fp.m_restParticles[p_idx].pos) / num_shapes;
//            //And adjust other particles in shape to be relative to the new center.
//            var shape_offsets = fsm.m_shapeIndices.ToList().GetRange(shape_start, num_shapes)
//                .Select(p_idx => {
//                    fsm.m_shapeRestPositions[shapeIndexOffset] = fp.m_restParticles[p_idx].pos - fsm.m_shapeCenters[s];
//                    return ++shapeIndexOffset;
//                });

//            shape_start = shapeEnd;
//        }

//        //And make sure that the mesh vertices aren't influenced by shapes that are cut off.
//        //Mesh mesh = target.GetComponent<SkinnedMeshRenderer>().sharedMesh;
//        //var boneWeights = new BoneWeight[mesh.vertexCount];
//        //for (int i = 0; i < mesh.vertexCount; i++)
//        //{
//        //    mesh.boneWeights[i] = adjust_weight(mesh.boneWeights[i], mesh.vertices[i], shapes.m_shapeCenters, plane);
//        //}
    }
    public static BoneWeight adjust_weight(BoneWeight weight, Vector3 mesh_vert, Vector3[] bones, Plane plane)
    {
        //return weight;
        var particles = new List<Vector3> { bones[weight.boneIndex0], bones[weight.boneIndex1],
                                            bones[weight.boneIndex2], bones[weight.boneIndex3] };
        var same_side_verts = particles.Select(v => plane.SameSide(v, mesh_vert)).ToList();
        if (same_side_verts.Where(v=>v).Count() == 0)
        {
            weight.boneIndex0 = find_closest_shape_on_same_side(bones, mesh_vert, plane);
            weight.weight0 = 1.0f;
            weight.weight1 = weight.weight2 = weight.weight3 = 0;
        }
        else
        {
            var best_idx = find_closest_shape_on_same_side(particles.ToArray(), mesh_vert, plane);
            float bad_vals = 0;
            if (!same_side_verts[0])
            {
                bad_vals += weight.weight0;
                weight.weight0 = 0;
            }
            if (!same_side_verts[1])
            {
                bad_vals += weight.weight1;
                weight.weight1 = 0;
            }
            if (!same_side_verts[2])
            {
                bad_vals += weight.weight2;
                weight.weight2 = 3;
            }
            if (!same_side_verts[0])
            {
                bad_vals += weight.weight3;
                weight.weight3 = 0;
            }
        }
        return weight;
    }

    public static int find_closest_shape_on_same_side(Vector3[] bonePos, Vector3 vert, Plane plane)
    {
        return bonePos.ToList().Where((pos, idx) => plane.SameSide(pos, vert))
            .Select((pos, idx) => new { idx, dist = Vector3.Distance(pos, vert) })
            .Aggregate(new { idx = -1, dist = float.MaxValue }, (lhs, rhs) => lhs.dist < rhs.dist ? lhs : rhs).idx;
    }
}
