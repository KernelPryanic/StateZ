using GTA.Native;
using System;
using StateZ.Static;

namespace StateZ.Extensions
{
    public static class UiExtended
    {
        public static bool IsAnyHelpTextOnScreen()
        {
            return Function.Call<bool>(GTA.Native.Hash.IS_HELP_MESSAGE_ON_SCREEN, (InputArgument[])new InputArgument[0]);
        }

        public static void ClearAllHelpText()
        {
            Function.Call(GTA.Native.Hash.CLEAR_ALL_HELP_MESSAGES, (InputArgument[])new InputArgument[0]);
        }

        public static void DisplayHelpTextThisFrame(string helpText, bool ignoreMenus = false)
        {
            if (ignoreMenus || !MenuConrtoller.MenuPool.IsAnyMenuOpen())
            {
                Function.Call(GTA.Native.Hash._SET_TEXT_COMPONENT_FORMAT, (InputArgument[])new InputArgument[1]
                {
                    "CELL_EMAIL_BCON"
                });
                for (int i = 0; i < helpText.Length; i += 99)
                {
                    Function.Call(GTA.Native.Hash._ADD_TEXT_COMPONENT_STRING, (InputArgument[])new InputArgument[1]
                    {
                        helpText.Substring(i, Math.Min(99, helpText.Length - i))
                    });
                }
                Function.Call(GTA.Native.Hash._DISPLAY_HELP_TEXT_FROM_STRING_LABEL, (InputArgument[])new InputArgument[4]
                {
                    0,
                    0,
                    !Function.Call<bool>(GTA.Native.Hash.IS_HELP_MESSAGE_BEING_DISPLAYED) ? 1 : 0,
                    -1
                });
            }
        }
    }
}
