using System;
using System.Collections.Generic;
using Assets.Scripts.Models.Activities;
using Assets.Scripts.Models.Locations;

namespace Assets.Scripts.Models.Maps
{
    public class Map
    {
        public List<Activity> activities;

        public List<PointLocation> points;

        public DateTime selectedTime;
    }
}
