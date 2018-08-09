using GTA;
using GTA.Math;
using GTA.Native;
using System;
using System.Collections.Generic;
using System.Drawing;

namespace StateZ
{
	[Serializable]
	public class VehicleData : IDeletable
	{
		public int Handle
		{
			get;
			set;
		}

		public int Hash
		{
			get;
			set;
		}

		public Vector3 Rotation
		{
			get;
			set;
		}

		public Vector3 Position
		{
			get;
			set;
		}

		public VehicleColor PrimaryColor
		{
			get;
			set;
		}

		public VehicleColor SecondaryColor
		{
			get;
			set;
		}

		public int Health
		{
			get;
			set;
		}

		public float EngineHealth
		{
			get;
			set;
		}

		public float Heading
		{
			get;
			set;
		}

		public VehicleNeonLight[] NeonLights
		{
			get;
			set;
		}

		public List<Tuple<VehicleMod, int>> Mods
		{
			get;
			set;
		}

		public VehicleToggleMod[] ToggleMods
		{
			get;
			set;
		}

		public VehicleWindowTint WindowTint
		{
			get;
			set;
		}

		public VehicleWheelType WheelType
		{
			get;
			set;
		}

		public Color NeonColor
		{
			get;
			set;
		}

		public int Livery
		{
			get;
			set;
		}

		public bool Wheels1
		{
			get;
			set;
		}

		public bool Wheels2
		{
			get;
			set;
		}

		public VehicleData(int handle, int hash, Vector3 rotation, Vector3 position, VehicleColor primaryColor, VehicleColor secondaryColor, int health, float engineHealth, float heading, VehicleNeonLight[] neonLights, List<Tuple<VehicleMod, int>> mods, VehicleToggleMod[] toggleMods, VehicleWindowTint windowTint, VehicleWheelType wheelType, Color neonColor, int livery, bool wheels1, bool wheels2)
		{
			Handle = handle;
			Hash = hash;
			Rotation = rotation;
			Position = position;
			PrimaryColor = primaryColor;
			SecondaryColor = secondaryColor;
			Health = health;
			EngineHealth = engineHealth;
			Heading = heading;
			NeonLights = neonLights;
			Mods = mods;
			ToggleMods = toggleMods;
			WindowTint = windowTint;
			WheelType = wheelType;
			NeonColor = neonColor;
			Livery = livery;
			Wheels1 = wheels1;
			Wheels2 = wheels2;
		}

		public bool Exists()
		{
			return Function.Call<bool>(GTA.Native.Hash.DOES_ENTITY_EXIST, (InputArgument[])new InputArgument[1]
			{
				Handle
			});
		}

		public unsafe void Delete()
		{
			int handle = Handle;
			Function.Call(GTA.Native.Hash.DELETE_ENTITY, (InputArgument[])new InputArgument[1]
			{
			    &handle
			});
			Handle = handle;
		}
	}
}
