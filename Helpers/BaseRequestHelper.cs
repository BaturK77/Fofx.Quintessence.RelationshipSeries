using Fofx.Quintessence.RelationshipSeries.Helpers;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;

namespace Fofx
{
    public abstract class BaseRequestHelper : ITimeSeriesRequestHelper, IDataReaderHelper
    {
        public BaseRequestHelper()
        {
        }

        public abstract ITimeSeries CreateNew(ITimeSeriesIterator itearator, TimeSeriesDescriptor factor);

        public abstract void Add(ITimeSeries iTimeSeries, INullableReader reader, DatabaseRequestArgs args, TimeSeriesDatabaseContext requester);        

        public void Read(int[] entities, int[] timeseries, DatabaseRequestArgs args, TimeSeriesDatabaseContext requester, Dictionary<long, ITimeSeriesKey> keys, SortedList<ITimeSeriesKey, ITimeSeries> dbTimeSeriesResult)
        {
            ITimeSeries referenceTimeSeries = null;
            ITimeSeriesKey referenceKey = null;
            Dictionary<ITimeSeriesKey, ITimeSeries> newTimeSeriesList = new Dictionary<ITimeSeriesKey, ITimeSeries>(dbTimeSeriesResult);


            int previousEntityID = -1;
            int previousTimeSeriesValueID = -1;

            using (INullableReader reader = GetDataReader(entities, timeseries, args))
            {
                while (reader.Read())
                {
                    int entityID = reader.GetInt32(0);
                    int timeSeriesValueID = reader.GetInt32(1);

                    if (entityID != previousEntityID || previousTimeSeriesValueID != timeSeriesValueID)
                    {
                        referenceTimeSeries = null;
                        long key = ((long)entityID << 32) + timeSeriesValueID;
                        if (keys.TryGetValue(key, out referenceKey))
                            newTimeSeriesList.TryGetValue(referenceKey, out referenceTimeSeries);


                        previousEntityID = entityID;
                        previousTimeSeriesValueID = timeSeriesValueID;
                    }

                    if (referenceTimeSeries != null)
                        Add(referenceTimeSeries, reader, args, requester);
                }
            }
        }

        public virtual SortedList<ITimeSeriesKey, ITimeSeries> ReadTimeSeries(IEnumerable<TimeSeriesKey> dbCallRequired, DatabaseRequestArgs args, TimeSeriesDatabaseContext requester)
        {
            SortedList<ITimeSeriesKey, ITimeSeries> dbTimeSeriesResult = new SortedList<ITimeSeriesKey, ITimeSeries>();
            Dictionary<long, ITimeSeriesKey> keys = new Dictionary<long, ITimeSeriesKey>();

            SortedSet<int> dbEntityIDs = new SortedSet<int>();
            SortedSet<int> dbTimeSeriesValueDefinitionIDs = new SortedSet<int>();

            foreach (TimeSeriesKey key in dbCallRequired)
            {
                long pr = ((long)key.Entity.ID << 32) + key.TimeSeries.ValueDefinition;
                if (!keys.ContainsKey(pr))
                {
                    keys.Add(pr, key);
                    ITimeSeries series = CreateNew(args.Iterator, key.TimeSeries);
                    dbTimeSeriesResult.Add(key, series);
                }

                dbTimeSeriesValueDefinitionIDs.Add(key.TimeSeries.ValueDefinition);
                dbEntityIDs.Add(key.Entity.ID);
            }

            Read(dbEntityIDs.ToArray(), dbTimeSeriesValueDefinitionIDs.ToArray(), args, requester, keys, dbTimeSeriesResult);

            return dbTimeSeriesResult;
        }

        public virtual int CompareTo(object obj)
        {
            return CompareTo((ITimeSeriesRequestHelper)obj);
        }

        public virtual int CompareTo(ITimeSeriesRequestHelper helper)
        {
            return this.GetType().Name.CompareTo(helper.GetType().Name);
        }

        public INullableReader GetDataReader(int[] entites, int[] factors, DatabaseRequestArgs args)
        {
            throw new NotImplementedException();
        }
    }

    

   
}