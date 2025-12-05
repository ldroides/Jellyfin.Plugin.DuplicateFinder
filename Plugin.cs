using System;
using System.Collections.Generic;
using MediaBrowser.Common.Configuration;
using MediaBrowser.Common.Plugins;
using MediaBrowser.Model.Plugins;
using MediaBrowser.Model.Serialization;

namespace Jellyfin.Plugin.DuplicateFinder
{
    public class Plugin : BasePlugin<BasePluginConfiguration>
    {
        public override string Name => "Duplicate Finder";

        public override Guid Id => Guid.Parse("93A5E794-E050-4D85-8334-92445C16837E");

        public Plugin(IApplicationPaths applicationPaths, IXmlSerializer xmlSerializer)
            : base(applicationPaths, xmlSerializer)
        {
            Instance = this;
        }

        public static Plugin? Instance { get; private set; }
    }
}
