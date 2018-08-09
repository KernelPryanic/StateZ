using GTA;
using GTA.Math;
using GTA.Native;
using System;
using System.Linq;
using System.Runtime.CompilerServices;
using StateZ.Extensions;

namespace StateZ.Static
{
	public static class Database
	{
		public static Random Random = new Random();

		public static string[] VehicleModels = new string[20]
		{
			"fusilade",
			"stratum",
			"police",
			"buffalo",
			"superd",
			"emperor",
			"picador",
			"dubsta",
			"blista",
			"fugitive",
			"zion",
			"tornado",
			"regina",
			"police2",
			"pigalle",
			"asterope",
			"fq2",
			"glendale",
			"schafter2",
			"ambulance"
		};

		public static WeaponHash[] WeaponHashes;

		public static VehicleClass[] LandVehicleClasses;

		public static VehicleHash[] VehicleHashes;

		public static Model[] WrckedVehicleModels;

		public static Vector3[] AnimalSpawns;

		public static Vector3[] Shops247Locations;

		public static Ped PlayerPed => Player.Character;

		public static Player Player => Game.Player;

		public static Vehicle PlayerCurrentVehicle => PlayerPed.CurrentVehicle;

		public static PedGroup PlayerGroup => PlayerPed.CurrentPedGroup;

		public static bool PlayerIsDead => PlayerPed.IsDead;

		public static bool PlayerInVehicle => PlayerPed.IsInVehicle();

		public static bool PlayerIsSprinting => PlayerPed.IsSprinting;

		public static int PlayerHealth
		{
			get
			{
				return PlayerPed.Health;
			}
			set
			{
				PlayerPed.Health = value;
			}
		}

		public static int PlayerMaxHealth => PlayerPed.MaxHealth;

		public static Vector3 PlayerPosition => PlayerPed.Position;

		public static VehicleHash GetRandomVehicleByClass(VehicleClass vClass)
		{
			VehicleHash[] hashes = Enum.GetValues(typeof(VehicleHash)) as VehicleHash[];
			if (hashes != null)
			{
				hashes = (from h in hashes
				where VehicleExtended.GetModelClass(h) == vClass
				select h).ToArray();
				return hashes[Random.Next(hashes.Length)];
			}
			return GTA.Native.VehicleHash.ZType;
		}

		public static Model GetRandomVehicleModel()
		{
			Model model = default(Model);
            model = new Model(VehicleModels[Random.Next(VehicleModels.Length)]);
            //model._002Ector(VehicleModels[Random.Next(VehicleModels.Length)]);
            return model.Request(1500) ? model : null;
		}

		static Database()
		{
			WeaponHash[] obj = new WeaponHash[8];
            obj[0] = WeaponHash.Bat;
            obj[1] = WeaponHash.SMG;
            obj[2] = WeaponHash.Unarmed;
            obj[3] = WeaponHash.Pistol;
            obj[4] = WeaponHash.Bottle;
            obj[5] = WeaponHash.Pistol50;
            obj[6] = WeaponHash.BattleAxe;
            obj[7] = WeaponHash.Crowbar;
			WeaponHashes = (WeaponHash[])obj;
			VehicleClass[] obj2 = new VehicleClass[6];
            obj2[0] = VehicleClass.Commercial;
            obj2[1] = VehicleClass.Compacts;
            obj2[2] = VehicleClass.Industrial;
            obj2[3] = VehicleClass.Sedans;
            obj2[4] = VehicleClass.OffRoad;
            obj2[5] = VehicleClass.Motorcycles;
			LandVehicleClasses = (VehicleClass[])obj2;
			VehicleHash[] obj3 = new VehicleHash[8];
            obj3[0] = VehicleHash.Asea;
            obj3[1] = VehicleHash.Baller2;
            obj3[2] = VehicleHash.Banshee;
            obj3[3] = VehicleHash.Dubsta;
            obj3[4] = VehicleHash.Blista3;
            obj3[5] = VehicleHash.Regina;
            obj3[6] = VehicleHash.Glendale;
            obj3[7] = VehicleHash.Zion;
			VehicleHashes = (VehicleHash[])obj3;
			WrckedVehicleModels = (Model[])new Model[16]
			{
				"prop_rub_carwreck_2",
				"prop_rub_carwreck_3",
				"prop_rub_carwreck_4",
				"prop_rub_carwreck_5",
				"prop_rub_carwreck_6",
				"prop_rub_carwreck_7",
				"prop_rub_carwreck_8",
				"prop_rub_carwreck_9",
				"prop_rub_carwreck_10",
				"prop_rub_carwreck_11",
				"prop_rub_carwreck_12",
				"prop_rub_carwreck_13",
				"prop_rub_carwreck_14",
				"prop_rub_carwreck_15",
				"prop_rub_carwreck_16",
				"prop_rub_carwreck_17"
			};
			AnimalSpawns = (Vector3[])new Vector3[6]
			{
				new Vector3(-2333.765f, 1274.093f, 326.2806f),
				new Vector3(-2583.969f, 489.153f, 218.0715f),
				new Vector3(717.0663f, 5062.837f, 360.6411f),
				new Vector3(-1536.53f, 3634.83f, 248.3539f),
				new Vector3(2516.562f, -1684.065f, 35.24468f),
				new Vector3(2825.585f, -1469.839f, 11.25044f)
			};
			Shops247Locations = (Vector3[])new Vector3[9]
			{
				new Vector3(-3041.777f, 588.7258f, 7.908933f),
				new Vector3(-3243.759f, 1005.157f, 12.83071f),
				new Vector3(1732.932f, 6414.323f, 35.03724f),
				new Vector3(1963.272f, 3743.574f, 32.34375f),
				new Vector3(2678.908f, 3284.251f, 55.24114f),
				new Vector3(544.7951f, 2669.228f, 42.1565f),
				new Vector3(2557.156f, 384.4772f, 108.623f),
				new Vector3(377.7599f, 326.8445f, 103.5664f),
				new Vector3(29.1841f, -1346.031f, 29.49703f)
			};
		}
	}
}
