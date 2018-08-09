using GTA;
using GTA.Native;

namespace StateZ.Extensions
{
	public static class GameExtended
	{
		public static void DisableWeaponWheel()
		{
			Game.DisableControlThisFrame(2, GTA.Control.WeaponWheelLeftRight);
			Game.DisableControlThisFrame(2, GTA.Control.WeaponWheelNext);
			Game.DisableControlThisFrame(2, GTA.Control.WeaponWheelPrev);
			Game.DisableControlThisFrame(2, GTA.Control.WeaponWheelUpDown);
			Game.DisableControlThisFrame(2, GTA.Control.NextWeapon);
			Game.DisableControlThisFrame(2, GTA.Control.DropWeapon);
			Game.DisableControlThisFrame(2, GTA.Control.PrevWeapon);
			Game.DisableControlThisFrame(2, GTA.Control.WeaponSpecial);
			Game.DisableControlThisFrame(2, GTA.Control.WeaponSpecial2);
			Game.DisableControlThisFrame(2, GTA.Control.SelectWeapon);
		}

		public static int GetMobilePhoneId()
		{
			OutputArgument outArg = new OutputArgument();
			Function.Call(GTA.Native.Hash.GET_MOBILE_PHONE_RENDER_ID, (InputArgument[])new InputArgument[1]
			{
				outArg
			});
			return outArg.GetResult<int>();
		}
	}
}
