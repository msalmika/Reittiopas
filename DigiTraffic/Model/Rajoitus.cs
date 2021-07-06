using System;
using System.Collections.Generic;

namespace RataDigiTraffic.Model
{
    public class Rajoitus
    {
        public string description; //Describes a restriction affecting the use of a railway infrastructure part
        public double axleWeightMax; //   number($double) Max axle weight, required if limitation type is max axle weight
        public string created; //*string ($date-time) When this notification was created
        public DateTime endDate; // string($date-time) End datetime
        public DateTime finished; // string ($date-time) Finished datetime, required if state is finished
        public string id; //*string Id
        public string limitation; // *string Limitation type Enum: Array[8]
        public Location location; //* GeometryDto«object»{...}
        public Locations locations; //*[...]
        public DateTime modified; //   string ($date-time) When this notification last modified
        public string organization; //*string Which organization created this notification
        public DateTime startDate; //*	string($date-time) Start datetime
        public string state; //*string State Enum: Array[4]
        public string trackWorkNotificationId; // string Track work notification identifier
        public long version; //* integer($int64) Version
    }

    public class Location
    {
        public string type; // corresponds with GeoJSON geometry types
        public List<float> coordinates; //(nested) list(s) of floating point numbers
    }

    public class Locations
    {
        public string description; // Location of work, either an operating point or a section between operating points
        public idRange identifierRanges; //[...]
        public Location location;
        public string locationType; //* string Type Enum: Array[3]
        public string notificationId; //* string Notification identifier
        public string operatingPointId; // string Identifier of operating point, required if section is not present
        public string sectionBetweenOperatingPointsId; // string Identifier of section between operating points, required if operating point is not present
        public long workPartIndex; //* integer($int64) Track work part index
    }
    public class idRange
    {
        public string description; // Place of work: between two track elements or a single track element
        public string elementId; // string Identifier of element, required if element pair or ranges are not present
        public string elementPairId1; // string Identifier of element 1 in element pair, required if element or ranges are not present
        public string elementPairId2; //string Identifier of element 2 in element pair, required if element or ranges are not present
        public EleRange elementRanges; //[...]
        public string notificationId; //* string Notification identifier
    }
    public class EleRange
    {
        public string description; // Two consecutive elements in an identifier range
        public string elementId1; //* string Identifier of element 1
        public string elementId2; //* string Identifier of element 2
        public string specifiers; //[...]
        public string trackIds;//*[...]
        public string trackKilometerRange; // string Track kilometer range, required if notification type is traffic restriction, e.g. (006) 754+0273 > 764+0771
    }
    
}