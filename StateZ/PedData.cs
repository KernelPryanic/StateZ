using GTA.Math;
using GTA.Native;
using System;
using System.Collections.Generic;
using StateZ.PlayerManagement;

namespace StateZ
{
	[Serializable]
	public class PedData : IDeletable
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

		public PedTask Task
		{
			get;
			set;
		}

		public List<Weapon> Weapons
		{
			get;
			set;
		}

		public PedData(int handle, int hash, Vector3 rotation, Vector3 position, PedTask task, List<Weapon> weapons)
		{
			Handle = handle;
			Hash = hash;
			Rotation = rotation;
			Position = position;
			Task = task;
			Weapons = weapons;
		}

		public bool Exists()
		{
			return Function.Call<bool>(GTA.Native.Hash.TERMINATE_ALL_SCRIPTS_WITH_THIS_NAME, (InputArgument[])new InputArgument[1]
			{
				Handle
			});
		}

		public unsafe void Delete()
		{
			int handle = Handle;
			Function.Call(GTA.Native.Hash.TERMINATE_ALL_SCRIPTS_WITH_THIS_NAME, (InputArgument[])new InputArgument[1]
			{
				&handle
			});
			Handle = handle;
		}
	}
}
