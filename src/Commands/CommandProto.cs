using CSM.Models;
using ProtoBuf.Meta;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace CSM.Commands
{
    public class CommandProto
    {
        public static TypeModel Model { get; private set; }

        /// <summary>
        ///     Sets the client/server id of the command.
        /// </summary>
        /// <param name="command">The command to modify.</param>
        private static void SetSenderId(CommandBase command)
        {
            command.SenderId = -1;
        }

        static CommandProto()
        {
            // Get all CommandHandler subclasses in the CSM.Commands.Handler namespace
            Type[] handlers = typeof(CommandProto).Assembly.GetTypes()
              .Where(t => t.Namespace != null)
              .Where(t => t.Namespace.StartsWith("CSM.Commands", StringComparison.Ordinal))
              .Where(t => t.IsSubclassOf(typeof(CommandBase)))
              .Where(t => !t.IsAbstract)
              .ToArray();

            // Create a protobuf model
            RuntimeTypeModel model = RuntimeTypeModel.Create();

            // Set type surrogates
            model[typeof(Vector3)].SetSurrogate(typeof(Vector3Surrogate));
            model[typeof(NetTool.ControlPoint)].SetSurrogate(typeof(ControlPointSurrogate));

            // Add Quaternion Surrogate
            model[typeof(Quaternion)].SetSurrogate(typeof(QuaternionSurrogate));

            // Add Color Surrogate
            model[typeof(Color)].SetSurrogate(typeof(ColorSurrogate));

            // Add base command to the protobuf model with all attributes
            model.Add(typeof(CommandBase), true);
            MetaType baseCmd = model[typeof(CommandBase)];

            int id = 100;

            // Create instances of the handlers, initialize mappings and register command subclasses in the protobuf model
            foreach (Type type in handlers)
            {
                // Add subtype to the protobuf model with all attributes
                var fixedCommandAttribute = type.GetCustomAttributes(typeof(FixedCommandAttribute), true).OfType<FixedCommandAttribute>().FirstOrDefault();
                var fieldNumber = fixedCommandAttribute?.FieldNumber ?? id++;

                baseCmd.AddSubType(fieldNumber, type);
                model.Add(type, true);
            }

            // Compile the protobuf model
            model.CompileInPlace();

            Model = model;
        }
    }
}
