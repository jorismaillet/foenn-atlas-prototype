namespace Assets.Scripts.Foenn.Atlas.Models.Maps
{
    using Assets.Scripts.Foenn.Atlas.Models.Activities;
    using Assets.Scripts.Foenn.Atlas.Models.Locations;
    using System;
    using System.Collections.Generic;

    public class Map
    {
        public List<Activity> activities;

        public List<PointLocation> points;

        public DateTime selectedTime;
    }
}
