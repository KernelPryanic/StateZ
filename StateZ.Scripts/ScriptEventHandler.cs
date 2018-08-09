using GTA;
using System;
using System.Collections.Generic;
using StateZ.Extensions;

namespace StateZ.Scripts
{
    public class ScriptEventHandler : Script
    {
        private readonly List<EventHandler> _wrapperEventHandlers;

        private readonly List<EventHandler> _scriptEventHandlers;

        private int _index;

        public static ScriptEventHandler Instance
        {
            get;
            private set;
        }

        public ScriptEventHandler()
        {
            Instance = this;
            _wrapperEventHandlers = new List<EventHandler>();
            _scriptEventHandlers = new List<EventHandler>();
            Interval = 10;
            this.Tick += OnTick;
        }

        public void RegisterScript(EventHandler eventHandler)
        {
            _scriptEventHandlers.Add(eventHandler);
        }

        public void UnregisterScript(EventHandler eventHandler)
        {
            _scriptEventHandlers.Remove(eventHandler);
        }

        public void RegisterWrapper(EventHandler eventHandler)
        {
            _wrapperEventHandlers.Add(eventHandler);
        }

        public void UnregisterWrapper(EventHandler eventHandler)
        {
            _wrapperEventHandlers.Remove(eventHandler);
        }

        private void OnTick(object sender, EventArgs eventArgs)
        {
            UpdateWrappers(sender, eventArgs);
            UpdateScripts(sender, eventArgs);
        }

        private void UpdateScripts(object sender, EventArgs eventArgs)
        {
            for (int i = _scriptEventHandlers.Count - 1; i >= 0; i--)
            {
                _scriptEventHandlers[i]?.Invoke(sender, eventArgs);
            }
        }

        private void UpdateWrappers(object sender, EventArgs eventArgs)
        {
            for (int i = _index; i < _index + 5 && i < _wrapperEventHandlers.Count; i++)
            {
                _wrapperEventHandlers[i]?.Invoke(sender, eventArgs);
            }
            _index += 5;
            if (_index >= _wrapperEventHandlers.Count)
            {
                _index = 0;
            }
        }
    }
}
