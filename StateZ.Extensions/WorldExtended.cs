using GTA.Math;
using GTA.Native;
using System;
using StateZ.DataClasses;
using StateZ.Static;

namespace StateZ.Extensions
{
	public static class WorldExtended
	{
		public static void SetParkedVehicleDensityMultiplierThisFrame(float multiplier)
		{
			Function.Call(GTA.Native.Hash.SET_PARKED_VEHICLE_DENSITY_MULTIPLIER_THIS_FRAME, (InputArgument[])new InputArgument[1]
			{
				multiplier
			});
		}

		public static void SetVehicleDensityMultiplierThisFrame(float multiplier)
		{
			Function.Call(GTA.Native.Hash.SET_VEHICLE_DENSITY_MULTIPLIER_THIS_FRAME, (InputArgument[])new InputArgument[1]
			{
				multiplier
			});
		}

		public static void SetRandomVehicleDensityMultiplierThisFrame(float multiplier)
		{
			Function.Call(GTA.Native.Hash.SET_RANDOM_VEHICLE_DENSITY_MULTIPLIER_THIS_FRAME, (InputArgument[])new InputArgument[1]
			{
				multiplier
			});
		}

		public static void SetPedDensityThisMultiplierFrame(float multiplier)
		{
			Function.Call(GTA.Native.Hash.SET_PED_DENSITY_MULTIPLIER_THIS_FRAME, (InputArgument[])new InputArgument[1]
			{
				multiplier
			});
		}

		public static void SetScenarioPedDensityThisMultiplierFrame(float multiplier)
		{
			Function.Call(GTA.Native.Hash.SET_SCENARIO_PED_DENSITY_MULTIPLIER_THIS_FRAME, (InputArgument[])new InputArgument[1]
			{
				multiplier
			});
		}

		public static void RemoveAllShockingEvents(bool toggle)
		{
			Function.Call(GTA.Native.Hash.REMOVE_ALL_SHOCKING_EVENTS, (InputArgument[])new InputArgument[1]
			{
				toggle ? 1 : 0
			});
		}

		public static void SetFrontendRadioActive(bool active)
		{
			Function.Call(GTA.Native.Hash.SET_FRONTEND_RADIO_ACTIVE, (InputArgument[])new InputArgument[1]
			{
				active ? 1 : 0
			});
		}

		public unsafe static void ClearCops(float radius = 9000f)
		{
			Vector3 pos = Database.PlayerPosition;
			Function.Call(GTA.Native.Hash.CLEAR_AREA_OF_COPS, (InputArgument[])new InputArgument[5]
			{
				pos.X,
				pos.Y,
			    pos.Z,
				radius,
				0
			});
		}

		public unsafe static void ClearAreaOfEverything(Vector3 position, float radius)
		{
			Function.Call(GTA.Native.Hash._0x957838AAF91BD12D, (InputArgument[])new InputArgument[8]
			{
				position.X,
				position.Y,
				position.Z,
				radius,
				false,
				false,
				false,
				false
			});
		}

		public unsafe static ParticleEffect CreateParticleEffectAtCoord(Vector3 coord, string name)
		{
			Function.Call(GTA.Native.Hash._SET_PTFX_ASSET_NEXT_CALL, (InputArgument[])new InputArgument[1]
			{
				"core"
			});
			int handle = Function.Call<int>(GTA.Native.Hash.START_PARTICLE_FX_LOOPED_AT_COORD, (InputArgument[])new InputArgument[12]
			{
				name,
				coord.X,
				coord.Y,
				coord.Z,
				0.0,
				0.0,
				0.0,
				1f,
				0,
				0,
				0,
				0
			});
			return new ParticleEffect(handle);
		}
	}
}
