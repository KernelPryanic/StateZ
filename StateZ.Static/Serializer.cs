using System;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;

namespace StateZ.Static
{
	public static class Serializer
	{
		public static T Deserialize<T>(string path)
		{
			T obj = default(T);
			if (File.Exists(path))
			{
				try
				{
					FileStream fs = new FileStream(path, FileMode.Open);
					BinaryFormatter serializer = new BinaryFormatter();
					obj = (T)serializer.Deserialize(fs);
					fs.Close();
				}
				catch (Exception ex)
				{
					File.WriteAllText("./scripts/StateZCrashLog.txt", $"\n[{DateTime.UtcNow.ToShortDateString()}] {ex.Message}");
				}
				return obj;
			}
			return obj;
		}

		public static void Serialize<T>(string path, T obj)
		{
			try
			{
				FileStream fs = new FileStream(path, FileMode.Create);
				BinaryFormatter serializer = new BinaryFormatter();
				serializer.Serialize(fs, obj);
				fs.Close();
			}
			catch (Exception ex)
			{
				File.WriteAllText("./scripts/StateZCrashLog.txt", $"\n[{DateTime.UtcNow.ToShortDateString()}] {ex.Message}");
			}
		}
	}
}
