using Fofx.Quintessence.RelationshipSeries.Helpers;
using System;
using System.Collections.Generic;

namespace Fofx
{
    public class RelationshipStringCharacteristicRequestHelper : BaseRelationshipRevisableRequestHelper
    {

        public override INullableReader GetDataReader(int[] entites, int[] factors, int[] relationships, DatabaseRequestArgs args)
        {
            var parameters = GetCharacteristicParameters(entites, factors, relationships, args);
            return DataAccess.GetDataReader("[TimeSeries].[RelationshipCharacteristicStringTimeseriesLoad]", parameters);
        }

        public override ITimeSeries CreateNew(ITimeSeriesIterator itearator, TimeSeriesDescriptor factor)
        {
            return new StringCharacteristicRevisableTimeSeries();
        }

        public override void Add(ITimeSeries iTimeSeries, INullableReader reader, DatabaseRequestArgs args, TimeSeriesDatabaseContext requester)
        {
            int toEntityID = reader.GetInt32(2);
            string value = reader.GetString(5);
            int? nonKeyedAttributeSetId = reader.GetNullableInt32(7);

            NonKeyedAttributeSet nonKeyedAttributeSet = null;
            if (nonKeyedAttributeSetId != null)
                nonKeyedAttributeSet = args.Translator.GetNonKeyedAttributeSet((int)nonKeyedAttributeSetId);

            IEntityDescriptor entity = null;
            if (!requester.EntityLookup.TryGetValue(toEntityID, out entity))
            {
                if (!args.Translator.TryGetEntityDescriptorByID(toEntityID, out entity))
                    entity = new EntityDescriptor(toEntityID);
            }

          ((StringCharacteristicRevisableTimeSeries)iTimeSeries).Add(entity, FofxConstants.MinimumDate, FofxConstants.MinimumDate, value, nonKeyedAttributeSet);
        }

        protected override void Read(int[] entities, int[] timeseries, int[] relationship, DatabaseRequestArgs args, TimeSeriesDatabaseContext requester, Dictionary<QuickID, ITimeSeriesKey> keys, SortedList<ITimeSeriesKey, ITimeSeries> dbTimeSeriesResult)
        {
            INullableReader reader = GetDataReader(entities, timeseries, relationship, args);
            ReadHelper.ReadByType<StringCharacteristicRevisableTimeSeries>(entities, timeseries, relationship, args, requester, keys, dbTimeSeriesResult, reader);
            
        }
    }
}
