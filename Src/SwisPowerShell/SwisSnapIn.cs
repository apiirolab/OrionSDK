using System.ComponentModel;

namespace SwisPowerShell
{
    [RunInstaller(true)]
    public class SwisSnapIn
    {
        public string Description
        {
            get { return "PowerShell Snap-in for the SolarWinds Information Service"; }
        }

        public string Name
        {
            get { return "SwisSnapIn"; }
        }

        public string Vendor
        {
            get { return "SolarWinds, Inc."; }
        }
    }
}
