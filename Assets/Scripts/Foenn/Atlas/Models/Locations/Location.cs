namespace Assets.Scripts.Foenn.Atlas.Models.Locations {
    public abstract class Location {
        public string name;

        protected Location(string name)
        {
            this.name = name;
        }
    }
}
