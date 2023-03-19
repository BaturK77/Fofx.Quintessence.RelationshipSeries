using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Fofx.Quintessence.RelationshipSeries.Helpers.Entity
{
    public class SeriesEntity
    {
        public int EntityID { get; set; }
        public int TimeSeriesValueID { get; set; }
        public int RelationshipValueID { get; set; }
        public int ToEntityID { get; set; }
        public DateTime ValueDate { get; set; }
        public DateTime DeclarationDate { get; set; }
        public double? Value { get; set; }
        public string StringValue { get; set; }
        public int? NonKeyedAttributeSetId { get; set; }
    }
}
