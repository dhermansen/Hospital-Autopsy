using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using uFlex;

struct IdMass
{
    public int idx;
    public float inv_mass;

    public IdMass(int idx, float inv_mass) : this()
    {
        this.idx = idx;
        this.inv_mass = inv_mass;
    }
}
public class flexcomponents : FlexProcessor {
    private List<IdMass> locked = new List<IdMass>();
    private Collider ball;

    private void Start()
    {
        ball = GetComponents<Collider>().ToList().Where(c => c.enabled).First();
        Debug.LogFormat("Collider name {0} and type {1}", ball.name, ball.GetType());
    }

	// Use this for initialization
    public override void PostContainerUpdate(FlexSolver solver, FlexContainer cntr, FlexParameters parameters)
    {
        if (locked.Count() == 0)
        {
            for (int i = 0; i < cntr.m_particlesCount; i++)
            {
                if (Physics.OverlapSphere(cntr.m_particles[i].pos, 1.0f).ToList()
                    .Where(c => c == ball).Count() > 0)
                {
                    locked.Add(new IdMass(i, cntr.m_particles[i].invMass));
                    cntr.m_particles[i].invMass = 0.0f;
                    Debug.Log("Current idx = "+i.ToString());
                    break;
                }
            }
        }
        else
        {
            Debug.Log("Ball center " +ball.bounds.center.ToString());
            var pos = cntr.m_particles[locked[0].idx].pos;
            var p = Vector3.Lerp(pos, ball.bounds.center, 0.8f);
            var delta = p - pos;

            cntr.m_particles[locked[0].idx].pos = p;
            cntr.m_velocities[locked[0].idx] = delta / Time.fixedTime;
        }
    }

}
