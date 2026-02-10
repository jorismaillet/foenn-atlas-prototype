namespace Assets.Scripts.Foenn.ETL.Models
{
    public class Datafield
    {
        public string name;
        public Datatype type;

        public Datafield(string name, Datatype type)
        {
            this.name = name;
            this.type = type;
        }
    }
}