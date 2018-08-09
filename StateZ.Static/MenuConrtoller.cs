using GTA;
using NativeUI;
using System;

namespace StateZ.Static
{
    public class MenuConrtoller : Script
    {
        private static MenuPool _menuPool;

        private static TimerBarPool _barPool;

        public static MenuPool MenuPool => _menuPool ?? (_menuPool = new MenuPool());

        public static TimerBarPool BarPool => _barPool ?? (_barPool = new TimerBarPool());

        public MenuConrtoller()
        {
            this.Tick += OnTick;
        }

        public void OnTick(object sender, EventArgs eventArgs)
        {
            if (_barPool != null && (_menuPool == null || (_menuPool != null && !_menuPool.IsAnyMenuOpen())))
            {
                _barPool.Draw();
            }
            _menuPool?.ProcessMenus();
        }
    }
}
