namespace Assets.Scripts.Foenn.ETL
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