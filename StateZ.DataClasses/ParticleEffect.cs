using GTA.Native;
using System.Drawing;

namespace StateZ.DataClasses
{
	public class ParticleEffect : IDeletable
	{
		public int Handle
		{
			get;
		}

		public Color Color
		{
			set
			{
				Function.Call(GTA.Native.Hash.SET_PARTICLE_FX_LOOPED_COLOUR, (InputArgument[])new InputArgument[5]
				{
					Handle,
					value.R,
					value.G,
					value.B,
					true
				});
			}
		}

		internal ParticleEffect(int handle)
		{
			Handle = handle;
		}

		public bool Exists()
		{
			return Function.Call<bool>(GTA.Native.Hash.DOES_PARTICLE_FX_LOOPED_EXIST, (InputArgument[])new InputArgument[1]
			{
				Handle
			});
		}

		public void Delete()
		{
			Function.Call(GTA.Native.Hash.REMOVE_PARTICLE_FX, (InputArgument[])new InputArgument[2]
			{
				Handle,
				1
			});
		}
	}
}
