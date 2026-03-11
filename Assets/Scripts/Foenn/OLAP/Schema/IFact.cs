namespace Assets.Scripts.Foenn.OLAP.Schema
{
    using System.Collections.Generic;

    public interface IFact : ITable
    {
        List<IDimension> Dimensions { get; }

        /// <summary>
        /// References to dimension tables with their source mappings.
        /// </summary>
        List<Reference> References { get; }
    }
}
