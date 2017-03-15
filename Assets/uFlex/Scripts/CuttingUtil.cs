using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using uFlex;

public class CutFlexUtil
{
    public static void CutFlexSoft(Transform target, Plane act_plane)
    {
        var shapes = target.GetComponent<FlexShapeMatching>();
        var particles = target.GetComponent<FlexParticles>();

        //The following lists partition all points in all shapes.
        var indicies = new List<int>(); //Particles on the same side of the cutting plane as the center point
        var otherIndices = new List<int>(); //Particles on the other side.

        //Stores offsets of each shape in the indices array.  IOW we can determine which indices are split from each shape by plane
        var offsets = new List<int>();
        
        int indexBeg = 0;
        //For each shape, determine which indices are split from each shape by plane.
        //Store some in indices, others in otherIndices.
        //Store shape partitions in indices in offsets.
        Debug.Log(act_plane.distance.ToString() +';'+ act_plane.normal);
        for (int i = 0; i < shapes.m_shapesCount; ++i)
        {
            int indexEnd = shapes.m_shapeOffsets[i];

            var rest_plane = new Plane(act_plane.normal, act_plane.distance * act_plane.normal + shapes.m_shapeCenters[i] - shapes.m_shapeTranslations[i]);
            Debug.Log((rest_plane.normal * rest_plane.distance).ToString()+';'+act_plane.distance+';'+(shapes.m_shapeTranslations[i] - shapes.m_shapeCenters[i]).ToString());
            var shapeCenter = shapes.m_shapeCenters[i];
            for (int j = indexBeg; j < indexEnd; ++j)
            {
                int id = shapes.m_shapeIndices[j];
                //Debug.LogFormat("Rest {0}\nAct {1}\nDiff {2}",
                //    particles.m_restParticles[id].pos.ToString("F4"),
                //    particles.m_particles[id].pos.ToString("F4"),
                //    (particles.m_particles[id].pos - particles.m_restParticles[id].pos).ToString("F4"));
                //Debug.LogFormat("Calc {0}\nRest {1}\nrot {2}\ntrans {3}",
                //    (particles.m_restParticles[id].pos - shapes.m_shapeCenters[i] + shapes.m_shapeTranslations[i]).ToString("F4"),
                //    shapes.m_shapeRestPositions[id].ToString("F4"),
                //    shapes.m_shapeRotations[i].ToString("F4"),
                //    shapes.m_shapeTranslations[i].ToString("F4"));

                var pos = particles.m_particles[id].pos;

                if (rest_plane.SameSide(shapeCenter, pos))
                    indicies.Add(id);
                else
                    otherIndices.Add(id);
            }
            offsets.Add(indicies.Count);
            indexBeg = indexEnd;
        }
        //Now, merge otherIndices into indices depending on which bone is on the same side?
        // Added indices cause the shape sizes to be larger, naturally
        foreach (var other_idx in otherIndices)
        {
            if (!indicies.Contains(other_idx))
            {
                int shape_index = find_closest_shape_on_same_side(
                    shapes.m_shapeCenters, particles.m_restParticles[other_idx].pos, act_plane);
                int new_shape_start_index = offsets[shape_index];
                indicies.Insert(new_shape_start_index, other_idx);
                for (int j = shape_index; j < offsets.Count; j++)
                    offsets[j] += 1;
            }

        }

        //Now, store the modified shapes back into the flex object.
        shapes.m_shapeIndicesCount = indicies.Count;
        shapes.m_shapeIndices = indicies.ToArray();
        shapes.m_shapeOffsets = offsets.ToArray();

        //Calculate centers for the newly reconfigured shapes
        int shapeStart = 0;
        int shapeIndexOffset = 0;
        for (int s = 0; s < shapes.m_shapesCount; s++)
        {
            //shapes.m_shapeTranslations[s] = new Vector3();
            //shapes.m_shapeRotations[s] = Quaternion.identity;

            int shapeEnd = shapes.m_shapeOffsets[s];

            //For each particle in shape, calculate a new shape center.
            int num_shapes = shapeEnd - shapeStart;
            shapes.m_shapeCenters[s] = shapes.m_shapeIndices.ToList().GetRange(shapeStart, num_shapes)
                .Aggregate(Vector3.zero, (lhs, p_idx) => lhs + particles.m_restParticles[p_idx].pos) / num_shapes;
            //And adjust other particles in shape to be relative to the new center.
            var shape_offsets = shapes.m_shapeIndices.ToList().GetRange(shapeStart, num_shapes)
                .Select(p_idx => {
                    shapes.m_shapeRestPositions[shapeIndexOffset] = particles.m_restParticles[p_idx].pos - shapes.m_shapeCenters[s];
                    return ++shapeIndexOffset;
                });

            shapeStart = shapeEnd;
        }

        //And make sure that the mesh vertices aren't influenced by shapes that are cut off.
        //Mesh mesh = target.GetComponent<SkinnedMeshRenderer>().sharedMesh;
        //var boneWeights = new BoneWeight[mesh.vertexCount];
        //for (int i = 0; i < mesh.vertexCount; i++)
        //{
        //    mesh.boneWeights[i] = adjust_weight(mesh.boneWeights[i], mesh.vertices[i], shapes.m_shapeCenters, plane);
        //}

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
