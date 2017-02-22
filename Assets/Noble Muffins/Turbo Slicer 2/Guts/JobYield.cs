using UnityEngine;
using System.Collections.Generic;

namespace NobleMuffins.TurboSlicer.Guts
{
	public class JobYield
	{
		public JobYield(JobSpecification job, MeshSnapshot alfa, MeshSnapshot bravo) {
			Job = job;
			Alfa = alfa;
			Bravo = bravo;
		}

		public JobSpecification Job { get; private set; }
		public MeshSnapshot Alfa { get; private set; }
		public MeshSnapshot Bravo { get; private set; }
	}
}

