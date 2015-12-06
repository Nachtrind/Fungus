using System;

namespace NodeAbilities
{
    class AbilityIdentifier: Attribute
    {
        public string identifier = "";
        public AbilityIdentifier(string identifier)
        {
            this.identifier = identifier;
        }
    }
}
