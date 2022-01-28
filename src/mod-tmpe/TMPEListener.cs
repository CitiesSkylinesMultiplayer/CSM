using System;
using CSM.API;
using CSM.API.Helpers;
using TrafficManager;
using TrafficManager.API.Notifier;
using TrafficManager.Util.Record;

namespace CSM.TMPE
{
    public static class TMPEListener
    {
        public static void Listen()
        {
            Notifier.Instance.EventModified += NotificationListener;
        }
        
        public static void Unregister()
        {
            Notifier.Instance.EventModified -= NotificationListener;
        }

        private static void NotificationListener(OnModifiedEventArgs eventArgs)
        {
            if (IgnoreHelper.Instance.IsIgnored())
            {
                return;
            }

            SimulationManager.instance.AddAction(() =>
            {
                string base64Data = null;
                object data = Copy(eventArgs.InstanceID);
                if (data != null)
                {
                    base64Data = data is IRecordable recordable
                        ? Convert.ToBase64String(recordable.Serialize())
                        : null;
                }
                
                Log.Debug(eventArgs.ToString());

                /*Command.SendToAll(new TMPENotification
                {
                    Base64RecordObject = base64Data,
                    DataVersion = VersionUtil.ModVersion.ToString(),
                });*/
            });
        }
        
        private static object Copy(InstanceID sourceInstanceID)
        {
            switch (sourceInstanceID.Type)
            {
                case InstanceType.NetNode:
                    NodeRecord nodeRecord = new NodeRecord(sourceInstanceID.NetNode);
                    nodeRecord.Record();
                    return nodeRecord;
                case InstanceType.NetSegment:
                    TrafficRulesRecord trafficRulesRecord = new TrafficRulesRecord();
                    trafficRulesRecord.AddSegmentWithBothEnds(sourceInstanceID.NetSegment);
                    trafficRulesRecord.Record();
                    return trafficRulesRecord;
                default:
                    UnityEngine.Debug.Log($"[CSM TMPE support] Instance type {sourceInstanceID.Type} is not supported.");
                    return null;
            }
        }
    }
}
