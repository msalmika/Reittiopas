using System;
using System.Collections.Generic;

namespace RataDigiTraffic.Model
{
    public class Liikennetiedote
    {
        public DateTime created;//* string ($date-time) When this notification was created

        public bool electricitySafetyPlan; //* boolean Does the notification contain a electricity safety plan
        public string id; //* string Id

        public List<double> location; //* GeometryDto«object»{...}
        public DateTime modified; // string ($date-time) When this notification last modified

        public string organization; //*	string Which organization created this notification

        public bool personInChargePlan; //*	boolean Does the notification contain a plan for persons in charge

        public bool speedLimitPlan; //*	boolean Does the notification contain a speed limit plan

        public bool speedLimitRemovalPlan; //*	boolean Does the notification contain a speed limit removal plan

        public string state; //*  string State Enum: Array[7]
        public bool trafficSafetyPlan; //*	boolean Does the notification contain a traffic safety plan

        public long version; //*	integer($int64) Version

        //workParts*	[...]
    }
}