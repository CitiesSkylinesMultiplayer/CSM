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
	        Log.Info(command.ToString());
	        Log.Info(command.InfoIndex.ToString());
	        Log.Info(command.ForceSetEditLine.ToString());
	        Log.Info(command.TempLine.ToString());
	        Log.Info(command.SourceLine.ToString());
	        Log.Info(command.ReleaseLines?.ToString());
	        Log.Info(command.CreateLine.ToString());
	        Log.Info(command.LastAddIndex.ToString());
	        Log.Info(command.LastMoveIndex.ToString());
	        Log.Info(command.LastMovePos.ToString());
	        Log.Info(command.LastAddPos.ToString());
	        Log.Info(command.AddIndex.ToString());
	        Log.Info(command.MoveIndex.ToString());
	        Log.Info(command.AddPos.ToString());
	        Log.Info(command.FixedPlatform.ToString());
	        Log.Info(command.Array16Ids?.ToString());

            TransportTool tool = Singleton<ToolSimulator>.instance.GetTool<TransportTool>(command.SenderId);
            
            ReflectionHelper.SetAttr(tool, "m_lastAddIndex", command.LastAddIndex);
            ReflectionHelper.SetAttr(tool, "m_lastAddPos", command.LastAddPos);
            ReflectionHelper.SetAttr(tool, "m_lastMoveIndex", command.LastMoveIndex);
            ReflectionHelper.SetAttr(tool, "m_lastMovePos", command.LastMovePos);

            ArrayHandler.StartApplying(command.Array16Ids, null);

            TransportInfo info = PrefabCollection<TransportInfo>.GetPrefab(command.InfoIndex);

            IgnoreHelper.Instance.StartIgnore();

            TransportManager instance = Singleton<TransportManager>.instance;
            if (command.ReleaseLines != null)
            {
	            foreach (ushort releaseLine in command.ReleaseLines)
	            {
		            instance.ReleaseLine(releaseLine);
	            }
            }


            if (command.CreateLine)
            {
	            Singleton<TransportManager>.instance.CreateLine(out ushort m_tempLine,
		            ref Singleton<SimulationManager>.instance.m_randomizer, info, newNumber: false);
	            if (m_tempLine != command.TempLine)
	            {
		            Log.Error("Received temp line id does not match reserved temp line id");
		            Chat.Instance.PrintGameMessage(Chat.MessageType.Error, "Received temp line id does not match reserved temp line id. Please restart the game session.");
	            }
            }
            
            ReflectionHelper.SetAttr(tool, "m_tempLine", command.TempLine);
            ReflectionHelper.Call(tool, "SetEditLine", new[] {typeof(ushort), typeof(bool)}, command.TempLine == 0 ? (ushort) 0 : command.SourceLine, command.ForceSetEditLine);

            // Update local working values after SetEditLine call
            command.LastAddIndex = ReflectionHelper.GetAttr<int>(tool, "m_lastAddIndex");
            command.LastAddPos = ReflectionHelper.GetAttr<Vector3>(tool, "m_lastAddPos");
            command.LastMoveIndex = ReflectionHelper.GetAttr<int>(tool, "m_lastMoveIndex");
            command.LastMovePos = ReflectionHelper.GetAttr<Vector3>(tool, "m_lastMovePos");

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
			                .MoveStop(command.TempLine, command.LastMoveIndex, command.LastMovePos, command.FixedPlatform))
		            {
			            command.LastMoveIndex = -2;
			            command.LastMovePos = Vector3.zero;
		            }
                    
		            instance.m_lines.m_buffer[command.TempLine].CopyMissingPaths(command.SourceLine);
		            if (command.MoveIndex != -2 && instance.m_lines.m_buffer[command.TempLine]
			                .MoveStop(command.TempLine, command.MoveIndex, command.AddPos, command.FixedPlatform, out var oldPos))
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
                    
		            ReflectionHelper.SetAttr(tool, "m_lastAddIndex", command.LastAddIndex);
		            ReflectionHelper.SetAttr(tool, "m_lastAddPos", command.LastAddPos);
		            ReflectionHelper.SetAttr(tool, "m_lastMoveIndex", command.LastMoveIndex);
		            ReflectionHelper.SetAttr(tool, "m_lastMovePos", command.LastMovePos);
	            }

	            instance.m_lines.m_buffer[command.TempLine].m_color = instance.m_lines.m_buffer[command.SourceLine].m_color;
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
