using CSM.API.Commands;
using CSM.API.Helpers;
using CSM.BaseGame.Commands.Data.TransportLines;
using CSM.BaseGame.Helpers;
using CSM.BaseGame.Injections;
using ColossalFramework;
using CSM.API;
using UnityEngine;

namespace CSM.BaseGame.Commands.Handler.TransportLines
{
    public class TransportLineTempHandler : CommandHandler<TransportLineTempCommand>
    {
        protected override void Handle(TransportLineTempCommand command)
        {
            TransportTool tool = Singleton<ToolSimulator>.instance.GetTool<TransportTool>(command.SenderId);

            ArrayHandler.StartApplying(command.Array16Ids, null);
            IgnoreHelper.Instance.StartIgnore();

            // Update received values
            ReflectionHelper.SetAttr(tool, "m_lastAddIndex", command.LastAddIndex);
            ReflectionHelper.SetAttr(tool, "m_lastAddPos", command.LastAddPos);
            ReflectionHelper.SetAttr(tool, "m_lastMoveIndex", command.LastMoveIndex);
            ReflectionHelper.SetAttr(tool, "m_lastMovePos", command.LastMovePos);

            TransportInfo info = PrefabCollection<TransportInfo>.GetPrefab(command.InfoIndex);

            TransportManager instance = Singleton<TransportManager>.instance;

            // The following is a reproduction of the "EnsureTempLine" logic adapted for the remote players (e.g. no "temporary" flag for the temp line)

            // Release lines
            if (command.ReleaseLine != 0)
            {
                instance.ReleaseLine(command.ReleaseLine);
            }

            // Create temp line if necessary
            if (command.CreateLine)
            {
                Singleton<TransportManager>.instance.CreateLine(out ushort tempLine,
                    ref Singleton<SimulationManager>.instance.m_randomizer, info, newNumber: false);
                instance.m_lines.m_buffer[command.TempLine].m_flags |= TransportLine.Flags.Temporary;
                if (tempLine != command.TempLine)
                {
                    Log.Error("Received temp line id does not match reserved temp line id");
                    Chat.Instance.PrintGameMessage(Chat.MessageType.Error,
                        "Received temp line id does not match reserved temp line id. Please restart the game session.");
                }
            }

            // Set temp line and copy current edit line to it if necessary
            ReflectionHelper.SetAttr(tool, "m_tempLine", command.TempLine);
            ReflectionHelper.Call(tool, "SetEditLine", new[] { typeof(ushort), typeof(bool) },
                command.TempLine == 0 ? (ushort)0 : command.SourceLine, command.ForceSetEditLine);

            // Update local working values after SetEditLine call
            command.LastAddIndex = ReflectionHelper.GetAttr<int>(tool, "m_lastAddIndex");
            command.LastAddPos = ReflectionHelper.GetAttr<Vector3>(tool, "m_lastAddPos");
            command.LastMoveIndex = ReflectionHelper.GetAttr<int>(tool, "m_lastMoveIndex");
            command.LastMovePos = ReflectionHelper.GetAttr<Vector3>(tool, "m_lastMovePos");

            // Handle changes in indices
            if (command.TempLine != 0)
            {
                if (command.LastMoveIndex != command.MoveIndex
                    || command.LastAddIndex != command.AddIndex
                    || command.LastAddPos != command.AddPos)
                {
                    if (command.LastAddIndex != -2 &&
                        instance.m_lines.m_buffer[command.TempLine].RemoveStop(command.TempLine, command.LastAddIndex))
                    {
                        command.LastAddIndex = -2;
                        command.LastAddPos = Vector3.zero;
                    }

                    if (command.LastMoveIndex != -2 && instance.m_lines.m_buffer[command.TempLine]
                            .MoveStop(command.TempLine, command.LastMoveIndex, command.LastMovePos,
                                command.FixedPlatform))
                    {
                        command.LastMoveIndex = -2;
                        command.LastMovePos = Vector3.zero;
                    }

                    instance.m_lines.m_buffer[command.TempLine].CopyMissingPaths(command.SourceLine);
                    if (command.MoveIndex != -2 && instance.m_lines.m_buffer[command.TempLine]
                            .MoveStop(command.TempLine, command.MoveIndex, command.AddPos, command.FixedPlatform,
                                out var oldPos))
                    {
                        command.LastMoveIndex = command.MoveIndex;
                        command.LastMovePos = oldPos;
                        command.LastAddPos = command.AddPos;
                    }

                    if (command.AddIndex != -2 && instance.m_lines.m_buffer[command.TempLine]
                            .AddStop(command.TempLine, command.AddIndex, command.AddPos, command.FixedPlatform))
                    {
                        command.LastAddIndex = command.AddIndex;
                        command.LastAddPos = command.AddPos;
                    }

                    instance.UpdateLine(command.TempLine);

                    // Write updated values back to tool instances
                    ReflectionHelper.SetAttr(tool, "m_lastAddIndex", command.LastAddIndex);
                    ReflectionHelper.SetAttr(tool, "m_lastAddPos", command.LastAddPos);
                    ReflectionHelper.SetAttr(tool, "m_lastMoveIndex", command.LastMoveIndex);
                    ReflectionHelper.SetAttr(tool, "m_lastMovePos", command.LastMovePos);
                }

                // Some other stuff
                instance.m_lines.m_buffer[command.TempLine].m_color =
                    instance.m_lines.m_buffer[command.SourceLine].m_color;
                instance.m_lines.m_buffer[command.TempLine].m_flags &= ~TransportLine.Flags.Hidden;

                if ((instance.m_lines.m_buffer[command.SourceLine].m_flags & TransportLine.Flags.CustomColor) != 0)
                {
                    instance.m_lines.m_buffer[command.TempLine].m_flags |= TransportLine.Flags.CustomColor;
                }
                else
                {
                    instance.m_lines.m_buffer[command.TempLine].m_flags &= ~TransportLine.Flags.CustomColor;
                }
            }

            IgnoreHelper.Instance.EndIgnore();
            ArrayHandler.StopApplying();
        }
    }
}
