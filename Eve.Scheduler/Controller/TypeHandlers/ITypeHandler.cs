﻿using Eve.Settings;
using System;
using System.Collections.Generic;
using System.Text;

namespace Eve.Scheduler.Controller.TypeHandlers
{
    public abstract class TypeHandlerBase
    {
        private SettingsManager _SettingsManager;

        public TypeHandlerBase(SettingsManager settingsManager)
        {
            _SettingsManager = settingsManager;
        }

        public abstract byte[] Handle(Message message);



    }
}