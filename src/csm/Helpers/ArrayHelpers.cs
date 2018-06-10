using System;

namespace CSM.Helpers
{
    public static class ArrayHelpers
    {
        /// <summary>
        ///     Add a byte to the front of a byte array
        /// </summary>
        /// <param name="newByte">Byte to add</param>
        /// <param name="values">The byte array</param>
        /// <returns>a new byte array</returns>
        public static byte[] PrependByte(byte newByte, byte[] values)
        {
            var newValues = new byte[values.Length + 1];
            newValues[0] = newByte;        
            
            Array.Copy(values, 0, newValues, 1, values.Length); 

            return newValues;
        }
    }
}