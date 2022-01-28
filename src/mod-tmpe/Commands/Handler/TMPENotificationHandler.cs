using CSM.API.Commands;
using CSM.TMPE.Commands.Data;
using System;
using CSM.API.Helpers;
using TrafficManager.State;
using TrafficManager.Util.Record;

namespace CSM.TMPE.Commands.Handler
{

    /// <summary>
    ///  A Command handler for Notifications sent by TMPE with CSM over network from external.
    /// </summary>
    public class TMPENotificationHandler : CommandHandler<TMPENotification> {

        /// <summary>
        /// Handles the processing of incoming TMPENotification from other players
        /// applying the data received to the local player.
        /// </summary>
        /// <param name="command">The TMPE Notification containing other players changes.</param>
        protected override void Handle(TMPENotification command) {
            object record = SerializationUtil.Deserialize(Convert.FromBase64String(command.Base64RecordObject));
            PasteRecord(record);
        }

       /* private void EnsureAllLanesAreValid(object record) {

            int laneInvalid = 0;
            if (record is TrafficRulesRecord r)
            {
                foreach (IRecordable trafficRecords in r.Records)
                {
                    if (!(trafficRecords is SegmentRecord segmentRecord)) continue;

                    foreach (uint laneId in segmentRecord.allLaneIds_) {
                        if (!netService.IsLaneAndItsSegmentValid(laneId)) {
                            laneInvalid++;
                            break;
                        }
                    }
                }
            }
            if (laneInvalid == 0) {
                PasteRecord(record);
            } else {
                Log.Info("TMPENotificationHandler: Dropped notification - " + record.ToString());
            }
        }*/

        private static void PasteRecord(object record) {
            IgnoreHelper.Instance.StartIgnore();

            try {
                if (!(record is IRecordable recordable))
                    return;
                recordable.Transfer(null);
            }
            catch (Exception e) {
                UnityEngine.Debug.Log("TMPENotificationHandler: Could not paste record - " + e.Message);
            }

            IgnoreHelper.Instance.EndIgnore();
        }
    }
}
