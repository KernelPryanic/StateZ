using GTA;
using System.IO;

namespace StateZ.Static
{
	public class Config
	{
		public static string VersionId = "1.0.2d";

		public const string ScriptFilePath = "./scripts/";

		public const string IniFilePath = "./scripts/StateZ.ini";

		public const string InventoryFilePath = "./scripts/Inventory.dat";

		public const string MapFilePath = "./scripts/Map.dat";

		public const string VehicleFilePath = "./scripts/Vehicles.dat";

		public const string GuardsFilePath = "./scripts/Guards.dat";

		public static void Check()
		{
			ScriptSettings settings = ScriptSettings.Load("./scripts/StateZ.ini");
			if (!(settings.GetValue("mod", "version_id", "0") == VersionId))
			{
				if (File.Exists("./scripts/StateZ.ini"))
				{
					File.Delete("./scripts/StateZ.ini");
				}
				if (File.Exists("./scripts/Inventory.dat"))
				{
					File.Delete("./scripts/Inventory.dat");
				}
				UI.Notify($"Updating Simple Zombies to version ~g~{VersionId}~s~. Overwritting the " + "inventory file since there are new items.");
				settings.SetValue("mod", "version_id", VersionId);
				settings.Save();
			}
		}
	}
}
