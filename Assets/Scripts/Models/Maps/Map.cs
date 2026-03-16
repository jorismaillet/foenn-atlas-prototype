using System;
using System.Collections.Generic;
using Assets.Scripts.Models.Activities;
using Assets.Scripts.Models.Locations;

namespace Assets.Scripts.Models.Maps
{
    public class Map
    {
        public readonly List<Activity> activities;

        public readonly List<PointLocation> points;

        public DateTime selectedTime;
    }
}
