using Assets.Scripts.Foenn.Atlas.Models.Activities;
using System;
using System.Collections.Generic;

namespace Assets.Scripts.Foenn.Atlas.Models.Maps {
    public class Map {
        public List<Activity> activities;
        public List<BackgroundLayer> selectedBackgrounds;
        public DateTime selectedTime;
    }
}
