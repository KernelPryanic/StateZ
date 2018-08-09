using GTA.Native;

namespace StateZ.Extensions
{
	public static class ScriptExtended
	{
		public static void TerminateScriptByName(string name)
		{
			if (Function.Call<bool>(GTA.Native.Hash.DOES_SCRIPT_EXIST, (InputArgument[])new InputArgument[1]
			{
				name
			}))
			{
				Function.Call(GTA.Native.Hash.TERMINATE_ALL_SCRIPTS_WITH_THIS_NAME, (InputArgument[])new InputArgument[1]
				{
					name
				});
			}
		}
	}
}
