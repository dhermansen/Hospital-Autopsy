#define ACTIVE

#if MYCODE
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

    public static List<Color> CutFlexSoft(Transform target, Plane act_plane)
    {
        var fsm = target.GetComponent<FlexShapeMatching>();
        var fp = target.GetComponent<FlexParticles>();

        //The list of vertices that are on the same side as their shape center.
        var correct_side_indicies = new List<int>();
        //The list of vertices that are on a different side than the shape center.
        var wrong_side_indices = new List<int>();

        //Stores offsets of each shape in the indices array.  IOW we can determine which indices are split from each shape by plane
        var offsets = new List<int>();
        var centers = fsm.m_shapeCenters.Select((pos,idx) => shape_to_world(idx, fsm)).ToList();
        int shape_start = 0;
        //For each shape, determine which indices are split from each shape by plane.
        for (int shape_idx = 0; shape_idx < fsm.m_shapesCount; ++shape_idx)
        {
            int shape_end = fsm.m_shapeOffsets[shape_idx];

            var shape_center = centers[shape_idx];
            for (int j = shape_start; j < shape_end; ++j)
            {
                var particle_idx = fsm.m_shapeIndices[j];
                if (act_plane.SameSide(shape_center, fp.m_particles[particle_idx].pos))
                    correct_side_indicies.Add(particle_idx);
                else
                    wrong_side_indices.Add(particle_idx);
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
                    centers, fp.m_particles[idx].pos, act_plane);
                int new_shape_start_index = offsets[shape_index];
                correct_side_indicies.Insert(new_shape_start_index, idx);
                for (int j = shape_index; j < offsets.Count; ++j)
                    offsets[j] += 1;
            }
        }


        ////Now, store the modified shapes back into the flex object.
        fsm.m_shapeIndicesCount = correct_side_indicies.Count;
        fsm.m_shapeIndices = correct_side_indicies.ToArray();
        fsm.m_shapeOffsets = offsets.ToArray();

        ////Calculate centers for the newly reconfigured shapes
        shape_start = 0;
        int shape_index_offset = 0;
        for (int s = 0; s < fsm.m_shapesCount; ++s)
        {
            fsm.m_shapeTranslations[s] = new Vector3();
            fsm.m_shapeRotations[s] = Quaternion.identity;

            int shape_end = fsm.m_shapeOffsets[s];

            //For each particle in shape, calculate a new shape center.
            //var shift = act_plane.GetSide(fsm.m_shapeCenters[s]) ? new Vector3(1, 0, 0) : new Vector3(-1, 0, 0);
            var shift = new Vector3(0, 0, 0);
            int num_shapes = shape_end - shape_start;
            fsm.m_shapeCenters[s] = fsm.m_shapeIndices.ToList().GetRange(shape_start, num_shapes)
                .Aggregate(Vector3.zero, (lhs, p_idx) => lhs + fp.m_particles[p_idx].pos) / num_shapes + shift;

            //And adjust other particles in shape to be relative to the new center.
            var shape_offsets = fsm.m_shapeIndices.ToList().GetRange(shape_start, num_shapes)
                .Select(p_idx =>
                {
                    fsm.m_shapeRestPositions[shape_index_offset] = fp.m_restParticles[p_idx].pos - fsm.m_shapeCenters[s] + shift;
                    return ++shape_index_offset;
                });

            shape_start = shape_end;
        }
        //for (int shape_idx = 0; shape_idx < fsm.m_shapesCount; ++shape_idx)
        //{
        //    int shape_end = fsm.m_shapeOffsets[shape_idx];

        //    var shape_center = shape_to_world(shape_idx, fsm);
        //    for (int j = shape_start; j < shape_end; ++j)
        //    {
        //        var particle_idx = fsm.m_shapeIndices[j];
        //        Debug.Assert(act_plane.SameSide(shape_center, fp.m_particles[particle_idx].pos));
        //    }
        //}
        ////And make sure that the mesh vertices aren't influenced by shapes that are cut off.
        //var skin = target.GetComponent<SkinnedMeshRenderer>();
        //var mesh = skin.sharedMesh;
        //var baked = new Mesh();
        //skin.BakeMesh(baked);
        //var bone_weights = new BoneWeight[mesh.vertexCount];
        //for (int i = 0; i < mesh.vertexCount; i++)
        //{
        //    bone_weights[i] = mesh.boneWeights[i];
        //    var pos = target.rotation * baked.vertices[i] + target.position;
        //    mesh.boneWeights[i] = adjust_weight(bone_weights[i], pos, centers, act_plane);
        //}
        var colors = new List<Color>();
        for (int i = 0; i < centers.Count; ++i)
        {
            colors.Add(act_plane.GetSide(centers[i]) ? Color.magenta : Color.cyan);
        }

        return colors;
    }
    public static BoneWeight adjust_weight(BoneWeight weight, Vector3 mesh_vert, List<Vector3> particles, Plane plane)
    {
        //return weight;
        var weighted_particles = new List<Vector3> { particles[weight.boneIndex0], particles[weight.boneIndex1],
                                                     particles[weight.boneIndex2], particles[weight.boneIndex3] };
        var same_side_verts = particles.Select(v => plane.SameSide(v, mesh_vert)).ToList();
        if (same_side_verts.Where(v => v).Count() == 0)
        {
            weight.boneIndex0 = find_closest_shape_on_same_side(particles, mesh_vert, plane);
            weight.weight0 = 1.0f;
            weight.weight1 = weight.weight2 = weight.weight3 = 0.0f;
        }
        else
        {
            float reallocated_weight = 0.0f;
            if (!same_side_verts[0])
            {
                reallocated_weight += weight.weight0;
                weight.weight0 = 0;
            }
            if (!same_side_verts[1])
            {
                reallocated_weight += weight.weight1;
                weight.weight1 = 0;
            }
            if (!same_side_verts[2])
            {
                reallocated_weight += weight.weight2;
                weight.weight2 = 0;
            }
            if (!same_side_verts[3])
            {
                reallocated_weight += weight.weight3;
                weight.weight3 = 0;
            }
            reallocated_weight = Mathf.Min(reallocated_weight, 1.0f);
            if (reallocated_weight > 1.0f+1e-6f)
                Debug.Log("Updated weighting: " + reallocated_weight.ToString("F4"));
            var best_idx = find_closest_shape_on_same_side(weighted_particles, mesh_vert, plane);
            if (best_idx == 0) weight.weight0 += reallocated_weight;
            if (best_idx == 1) weight.weight1 += reallocated_weight;
            if (best_idx == 2) weight.weight2 += reallocated_weight;
            if (best_idx == 3) weight.weight3 += reallocated_weight;
        }
        return weight;
    }

    public static int find_closest_shape_on_same_side(List<Vector3> particle, Vector3 vert, Plane plane)
    {
        return particle.ToList().Where((pos, idx) => plane.SameSide(pos, vert))
            .Select((pos, idx) => new { idx, dist = Vector3.Distance(pos, vert) })
            .Aggregate(new { idx = -1, dist = float.MaxValue }, (lhs, rhs) => lhs.dist < rhs.dist ? lhs : rhs).idx;
    }
}
#else

using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using uFlex;
 
public class CutFlexUtil
{    public static Vector3 shape_to_world(int i, FlexShapeMatching fsm)
    {
        return fsm.m_shapeRotations[i] * fsm.m_shapeCenters[i] / 100.0f + fsm.m_shapeTranslations[i];
    }
    public static void world_to_shape(Vector3 pt, int i, FlexShapeMatching fsm)
    {
        fsm.m_shapeCenters[i] = Quaternion.Inverse(fsm.m_shapeRotations[i]) * (pt - fsm.m_shapeTranslations[i]) * 100.0f;
    }

    public static void CutFlexSoft(Transform target, Plane plane, Plane stop_plane, float thickness)
    {
        FlexShapeMatching shapes = target.GetComponent<FlexShapeMatching>();
        FlexParticles particles = target.GetComponent<FlexParticles>();

        List<int> indicies = new List<int>();
        List<int> offsets = new List<int>();

        List<int> otherIndices = new List<int>();
        var centers = Enumerable.Range(0, shapes.m_shapesCount).Select(i => shape_to_world(i, shapes)).ToList();
        int indexBeg = 0;
        int indexEnd = 0;
        for (int i = 0; i < shapes.m_shapesCount; ++i)
        {
            indexEnd = shapes.m_shapeOffsets[i];

            Vector3 shapeCenter = shapes.m_shapeCenters[i];
            for (int j = indexBeg; j < indexEnd; ++j)
            {
                int id = shapes.m_shapeIndices[j];
                Vector3 particlePos = particles.m_particles[id].pos;
                if (!plane.SameSide(centers[i], particlePos) && (stop_plane.GetSide(particlePos) != stop_plane.GetSide(particlePos - stop_plane.normal * thickness)))
                {
                    if (!otherIndices.Contains(id))
                    {
                        otherIndices.Add(id);
                    }
                }
                else
                {
                    indicies.Add(id);
                }
            }
            offsets.Add(indicies.Count);
            indexBeg = indexEnd;
        }
        if (otherIndices.Count == 0)
            return;
        for (int i = 0; i < otherIndices.Count; i++)
        {
            if (!indicies.Contains(otherIndices[i]))
            {
                int index = FindClosedBoneIndexOnSameSide(centers.ToArray(), particles.m_particles[otherIndices[i]].pos, plane);
                int atIndex = offsets[index];
                indicies.Insert(atIndex, otherIndices[i]);
                for (int j = index; j < offsets.Count; j++)
                {
                    offsets[j] += 1;
                }
            }
        }
        if (indicies.Count - shapes.m_shapeIndicesCount != 0)
        {
            Debug.LogFormat("Shape counts difference: {0}",
                indicies.Count - shapes.m_shapeIndicesCount);
            Debug.LogFormat("Shape count: {0}", offsets.Count);
        }
        shapes.m_shapeIndicesCount = indicies.Count;
        shapes.m_shapeIndices = indicies.ToArray();
        shapes.m_shapeOffsets = offsets.ToArray();

        int shapeStart = 0;
        int shapeIndex = 0;
        int shapeIndexOffset = 0;
        for (int s = 0; s < shapes.m_shapesCount; s++)
        {
            shapes.m_shapeTranslations[s] = new Vector3();
            shapes.m_shapeRotations[s] = Quaternion.identity;

            shapeIndex++;

            int shapeEnd = shapes.m_shapeOffsets[s];
            //-----------------------------------------cccc------------------------------------------------//

            Vector3 cen = Vector3.zero;
            for (int i = shapeStart; i < shapeEnd; ++i)
            {
                int p = shapes.m_shapeIndices[i];
                Vector3 pos = particles.m_restParticles[p].pos;
                cen += pos;
            }
            cen /= (shapeEnd - shapeStart);
            shapes.m_shapeCenters[s] = cen;

            //--------------------------------------------cccc---------------------------------------------//
            for (int i = shapeStart; i < shapeEnd; ++i)
            {
                int p = shapes.m_shapeIndices[i];

                // remap indices and create local space positions for each shape
                Vector3 pos = particles.m_restParticles[p].pos;
                shapes.m_shapeRestPositions[shapeIndexOffset] = pos - shapes.m_shapeCenters[s];

                shapeIndexOffset++;
            }

            shapeStart = shapeEnd;
        }


        //Mesh mesh = target.GetComponent<SkinnedMeshRenderer>().sharedMesh;
        //BoneWeight[] boneWeights = new BoneWeight[mesh.vertexCount];
        //for (int i = 0; i < mesh.vertexCount; i++)
        //{
        //    boneWeights[i] = mesh.boneWeights[i];
        //    Vector3 vertexPos = mesh.vertices[i];
        //    mesh.boneWeights[i] = CheckWeight(boneWeights[i], vertexPos, shapes.m_shapeCenters, plane);
        //}

    }
    public static BoneWeight CheckWeight(BoneWeight weight, Vector3 vert, Vector3[] bones, Plane plane)
    {
        //return weight;
        Vector3 shapeCenter = bones[weight.boneIndex0];
        float value;

        int flag = 0;
        if (plane.SameSide(shapeCenter, vert) == false)
        {
            value = weight.weight0;
            weight.weight0 = 0;


            int index = FindClosedBoneIndexOnSameSide(new Vector3[] { bones[weight.boneIndex1], bones[weight.boneIndex2], bones[weight.boneIndex3] }, vert, plane);
            if (index == 0) weight.weight1 += value;
            else if (index == 1) weight.weight2 += value;
            else if (index == 2) weight.weight3 += value;

            flag++;
        }

        shapeCenter = bones[weight.boneIndex1];
        if (plane.SameSide(shapeCenter, vert) == false)
        {
            value = weight.weight1;
            weight.weight1 = 0;
            //
            int index = FindClosedBoneIndexOnSameSide(new Vector3[] { bones[weight.boneIndex0], bones[weight.boneIndex2], bones[weight.boneIndex3] }, vert, plane);
            if (index == 0) weight.weight0 += value;
            else if (index == 1) weight.weight2 += value;
            else if (index == 2) weight.weight3 += value;
            flag++;
        }
        shapeCenter = bones[weight.boneIndex2];
        if (plane.SameSide(shapeCenter, vert) == false)
        {
            value = weight.weight2;
            weight.weight2 = 0;

            int index = FindClosedBoneIndexOnSameSide(new Vector3[] { bones[weight.boneIndex0], bones[weight.boneIndex1], bones[weight.boneIndex3] }, vert, plane);
            if (index == 0) weight.weight0 += value;
            else if (index == 1) weight.weight1 += value;
            else if (index == 2) weight.weight3 += value;

            flag++;
        }
        shapeCenter = bones[weight.boneIndex3];
        if (plane.SameSide(shapeCenter, vert) == false)
        {
            value = weight.weight3;
            weight.weight3 = 0;

            int index = FindClosedBoneIndexOnSameSide(new Vector3[] { bones[weight.boneIndex0], bones[weight.boneIndex1], bones[weight.boneIndex2] }, vert, plane);
            if (index == 0) weight.weight0 += value;
            else if (index == 1) weight.weight1 += value;
            else if (index == 2) weight.weight2 += value;

            flag++;
        }
        if (flag > 3)
        {
            Debug.Log(weight.weight0 + "------" + weight.weight1 + "------" + weight.weight2 + "------" + weight.weight3);

            weight.boneIndex0 = FindClosedBoneIndexOnSameSide(bones, vert, plane);
            weight.weight0 = 1;
        }

        return weight;

    }

    public static int FindClosedBoneIndexOnSameSide(Vector3[] BonePos, Vector3 vert, Plane plane)
    {
        int index = -1;
        float dis = float.MaxValue;
        for (int i = 0; i < BonePos.Length; i++)
        {
            if (plane.SameSide(BonePos[i], vert))
            {
                if (Vector3.Distance(BonePos[i], vert) < dis)
                {
                    index = i;
                    dis = Vector3.Distance(BonePos[i], vert);
                }
            }
        }
        return index;
    }
}

#endif