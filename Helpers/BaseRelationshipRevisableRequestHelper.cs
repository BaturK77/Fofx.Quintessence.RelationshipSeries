using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Fofx.Quintessence.RelationshipSeries.Helpers
{
    public abstract class BaseRelationshipRevisableRequestHelper : BaseRequestHelper
    {


        public abstract INullableReader GetDataReader(int[] entites, int[] factors, int[] relationships, DatabaseRequestArgs args);

        public override SortedList<ITimeSeriesKey, ITimeSeries> ReadTimeSeries(IEnumerable<TimeSeriesKey> dbCallRequired, DatabaseRequestArgs args, TimeSeriesDatabaseContext requester)
        {
            SortedList<ITimeSeriesKey, ITimeSeries> dbTimeSeriesResult = new SortedList<ITimeSeriesKey, ITimeSeries>();
            Dictionary<QuickID, ITimeSeriesKey> keys = new Dictionary<QuickID, ITimeSeriesKey>();


            SortedSet<int> dbEntityIDs = new SortedSet<int>();
            SortedSet<int> dbTimeSeriesValueDefinitionIDs = new SortedSet<int>();
            SortedSet<int> dbRelationshipValueDefinitionIDs = new SortedSet<int>();

            foreach (RelationshipTimeSeriesKey key in dbCallRequired)
            {
                QuickID qid = new QuickID(key.Entity.ID, key.TimeSeries.ValueDefinition, key.Relationship.ValueDefinition);

                if (!keys.ContainsKey(qid))
                {
                    ITimeSeries series = CreateNew(args.Iterator, key.TimeSeries);
                    keys.Add(qid, key);
                    dbTimeSeriesResult.Add(key, series);
                }

                dbTimeSeriesValueDefinitionIDs.Add(key.TimeSeries.ValueDefinition);
                dbEntityIDs.Add(key.Entity.ID);
                dbRelationshipValueDefinitionIDs.Add(((RelationshipTimeSeriesKey)key).Relationship.ValueDefinition);
            }

            Read(dbEntityIDs.ToArray(), dbTimeSeriesValueDefinitionIDs.ToArray(), dbRelationshipValueDefinitionIDs.ToArray(), args, requester, keys, dbTimeSeriesResult);

            return dbTimeSeriesResult;
        }

        protected abstract void Read (int[] entities, int[] timeseries, int[] relationship, DatabaseRequestArgs args, TimeSeriesDatabaseContext requester, Dictionary<QuickID, ITimeSeriesKey> keys, SortedList<ITimeSeriesKey, ITimeSeries> dbTimeSeriesResult);
        
        public SqlParameter[] GetParameters(int[] entities, int[] factors, int[] relationships, DatabaseRequestArgs args) => new SqlParameter[]
            {
                new SqlParameter("@EntityIDs", DataManipulation.ToCSVString(entities)),
                new SqlParameter("@TimeSeriesValueIDs", DataManipulation.ToCSVString(factors)),
                new SqlParameter("@RelationshipValueIDs", DataManipulation.ToCSVString(relationships)),
                new SqlParameter("@IAAD", args.Iterator.Iaad),
                new SqlParameter("@StartDate", args.Iterator.StartDate),
                new SqlParameter("@StartOffset", args.Iterator.StartOffset),
                new SqlParameter("@EndDate", args.Iterator.EndDate),
                new SqlParameter("@EndOffset", args.Iterator.EndOffset)

            };

        public SqlParameter[] GetCharacteristicParameters(int[] entities, int[] factors, int[] relationships, DatabaseRequestArgs args) => new SqlParameter[]
            {
                new SqlParameter("@EntityIDs", DataManipulation.ToCSVString(entities)),
                new SqlParameter("@TimeSeriesValueIDs", DataManipulation.ToCSVString(factors)),
                new SqlParameter("@RelationshipValueIDs", DataManipulation.ToCSVString(relationships))
            };

        


    }
}
