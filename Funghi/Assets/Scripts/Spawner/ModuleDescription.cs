using System;

namespace Spawner.Modules
{
    public class ModuleDescription: Attribute
    {
        public string description = "";
        public ModuleDescription(string description)
        {
            this.description = description;
        }
    }
}
