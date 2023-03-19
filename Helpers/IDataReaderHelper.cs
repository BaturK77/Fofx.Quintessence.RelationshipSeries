using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Fofx.Quintessence.RelationshipSeries.Helpers
{
    public interface IDataReaderHelper
    {
         INullableReader GetDataReader(int[] entites, int[] factors, DatabaseRequestArgs args);
    }
}
