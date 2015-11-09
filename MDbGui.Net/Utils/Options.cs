using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MDbGui.Net.Utils
{
    public static class Options
    {
        public static JsonWriterSettingsExtended JsonWriterSettings = new JsonWriterSettingsExtended() { Indent = true, UseLocalTime = true };
    }
}
