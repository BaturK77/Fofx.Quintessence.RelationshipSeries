using Fofx.Quintessence.RelationshipSeries.Helpers.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Fofx.Quintessence.RelationshipSeries.Helpers
{
    public static class ReadSeriesEntity
    {
        public static SeriesEntity ReadSeries(INullableReader reader, bool readStringValue)
        {

            return new SeriesEntity
            {
                EntityID = reader.GetInt32(0),
                TimeSeriesValueID = reader.GetInt32(1),
                RelationshipValueID = reader.GetInt32(2),
                ToEntityID = reader.GetInt32(3),
                ValueDate = reader.GetDateTime(4),
                DeclarationDate = reader.GetDateTime(5),
                Value = (!readStringValue) ? reader.GetNullableDouble(6) : null,
                StringValue = (!readStringValue) ? reader.GetNullableString(6) : null,
                NonKeyedAttributeSetId = reader.GetNullableInt32(7)
            };
        }
    }
}
