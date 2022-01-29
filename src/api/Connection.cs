using System.Collections.Generic;
using System.Reflection;

namespace CSM.API
{
    public abstract class Connection
    {
        public string Name { get; protected set; }
        
        public bool Enabled { get; protected set; }

        public List<Assembly> CommandAssemblies { get; } = new List<Assembly>();
    }
}
