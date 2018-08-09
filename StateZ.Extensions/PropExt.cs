using GTA;
using GTA.Native;
using System;

namespace StateZ.Extensions
{
	public static class PropExt
	{
		public unsafe static void SetStateOfDoor(this Prop prop, bool locked, DoorState heading)
		{
			InputArgument[] obj = new InputArgument[7];
			Model model = prop.Model;
			obj[0] = model.Hash;
			obj[1] = prop.Position.X;
			obj[2] = prop.Position.Y;
			obj[3] = prop.Position.Z;
			obj[4] = locked;
			obj[5] = (int)heading;
			obj[6] = 1;
			Function.Call(GTA.Native.Hash.SET_STATE_OF_CLOSEST_DOOR_OF_TYPE, (InputArgument[])obj);
		}

		public unsafe static bool GetDoorLockState(this Prop prop)
		{
			bool locked = false;
			int heading = 0;
			InputArgument[] obj = new InputArgument[6];
			Model model = prop.Model;
			obj[0] = model.Hash;
			obj[1] = prop.Position.X;
			obj[2] = prop.Position.Y;
			obj[3] = prop.Position.Z;
			obj[4] = &locked;
			obj[5] = &heading;
			Function.Call(GTA.Native.Hash.GET_STATE_OF_CLOSEST_DOOR_OF_TYPE, (InputArgument[])obj);
			return locked;
		}
	}
}
