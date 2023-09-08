using System.Collections.Generic;
using CSM.API.Commands;
using CSM.API.Helpers;
using CSM.BaseGame.Commands.Data.Roads;

namespace CSM.BaseGame.Commands.Handler.Roads
{
    public class RoadAdjustHandler : CommandHandler<RoadAdjustCommand>
    {
        private NetAdjust _adjustDummy;

        protected override void Handle(RoadAdjustCommand command)
        {
            IgnoreHelper.Instance.StartIgnore();

            if (_adjustDummy == null)
            {
                SetupAdjustDummy();
            }

            ReflectionHelper.SetAttr(_adjustDummy, "m_originalSegments", command.OriginalSegments);
            ReflectionHelper.SetAttr(_adjustDummy, "m_includedSegments", command.IncludedSegments);
            ReflectionHelper.SetAttr(_adjustDummy, "m_lastInstance", command.LastInstance);
            ReflectionHelper.SetAttr(_adjustDummy, "m_tempAdjustmentIndex", 0);

            _adjustDummy.ApplyModification(0);

            IgnoreHelper.Instance.EndIgnore();
        }

        private void SetupAdjustDummy()
        {
            _adjustDummy = new NetAdjust();
            ReflectionHelper.SetAttr(_adjustDummy, "m_pathVisible", true);
            ReflectionHelper.SetAttr(_adjustDummy, "m_startPath", new FastList<ushort>());
            ReflectionHelper.SetAttr(_adjustDummy, "m_endPath", new FastList<ushort>());
            ReflectionHelper.SetAttr(_adjustDummy, "m_tempPath", new FastList<ushort>());
            ReflectionHelper.SetAttr(_adjustDummy, "m_segmentQueue", new FastList<ushort>());
            ReflectionHelper.SetAttr(_adjustDummy, "m_segmentData", new Dictionary<ushort, NetAdjust.SegmentData>());
        }
    }
}
