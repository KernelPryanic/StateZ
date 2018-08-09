using GTA.Math;
using GTA.Native;
using System;

namespace StateZ.Extensions
{
	public static class SystemExtended
	{
		public unsafe static float VDist(this Vector3 v, Vector3 to)
		{
			return Function.Call<float>(GTA.Native.Hash.VDIST, (InputArgument[])new InputArgument[6]
			{
				v.X,
				v.Y,
				v.Z,
				to.X,
				to.Y,
				to.Z
			});
		}
	}
}
