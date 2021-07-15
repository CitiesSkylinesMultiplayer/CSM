using System;

namespace CSM.Commands
{
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = true)]
    internal class FixedCommandAttribute : Attribute
    {
        public int FieldNumber { get; }

        public FixedCommandAttribute(int fieldNumber)
        {
            FieldNumber = fieldNumber;
        }
    }
}
