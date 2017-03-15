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
public class PickupsTrigger : FlexProcessor
{
    private List<IdMass> locked = new List<IdMass>();
    private Collider ball;
    private bool try_pick_up = false, try_drop = false;

    private void Start()
    {
        ball = GetComponents<Collider>().ToList().Where(c => c.enabled).First();
    }

    public void onGrab()
    {
    }
    public void onUngrab()
    {
    }
    public void onUse()
    {
        try_pick_up = true;
    }
    public void onUnuse()
    {
        try_drop = true;
    }
    // Use this for initialization
    public override void PostContainerUpdate(FlexSolver solver, FlexContainer cntr, FlexParameters parameters)
    {
        if (try_pick_up)
        {
            try_pick_up = false;
            locked = cntr.m_particles.ToList().Select((v, i) => new IdMass(i, v.invMass))
                .Where(v => Vector3.Distance(cntr.m_particles[v.idx].pos, ball.bounds.center) < 1.0f)
                .ToList();
            Debug.LogFormat("Locked {0} points", locked.Count());
        }
        else if (try_drop)
        {
            try_drop = false;
            locked.Select(l => cntr.m_particles[l.idx].invMass = l.inv_mass);
            locked.Clear();
        }
        else if (locked.Count() > 0)
        {
            var locked_ctr = locked.Select(l => cntr.m_particles[l.idx].pos)
                                .Aggregate((lhs, rhs) => lhs + rhs) / locked.Count();

            var delta = ball.bounds.center - locked_ctr;
            foreach (var l in locked)
            {
                cntr.m_particles[l.idx].pos += delta;
                cntr.m_velocities[l.idx] = delta / Time.fixedTime;
            }
        }
    }
}
