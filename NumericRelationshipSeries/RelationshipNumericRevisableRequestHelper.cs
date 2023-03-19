using System;
using System.Linq;
using System.Collections.Generic;
using System.Text;
using System.Data.SqlClient;
using Fofx.Quintessence.RelationshipSeries.Helpers;
using Fofx.Quintessence.RelationshipSeries.Helpers.Entity;

namespace Fofx
{
    public class RelationshipRevisableNumericValueRequestHelper : BaseRelationshipRevisableRequestHelper
    {
        NumericConstituentRevisableTimeSeries ncs = null;
        public override INullableReader GetDataReader(int[] entites, int[] factors, int[] relationships, DatabaseRequestArgs args)
        {
            SqlParameter[] parameters = GetParameters(entites, factors, relationships, args);
            return DataAccess.GetDataReader("[TimeSeries].[RelationshipRevisableNumericTimeseriesLoad]", parameters);
        }


        public override ITimeSeries CreateNew(ITimeSeriesIterator iterator, TimeSeriesDescriptor factor)
        {
            return new NumericConstituentRevisableTimeSeries();
        }

        protected override void Read(int[] entities, int[] timeseries, int[] relationship, DatabaseRequestArgs args, TimeSeriesDatabaseContext requester, Dictionary<QuickID, ITimeSeriesKey> keys, SortedList<ITimeSeriesKey, ITimeSeries> dbTimeSeriesResult)
        {
            INullableReader reader = GetDataReader(entities, timeseries, relationship, args);
            ReadHelper.ReadByType<NumericConstituentRevisableTimeSeries>(entities, timeseries, relationship, args, requester, keys, dbTimeSeriesResult, reader);
            
        }

        public override void Add(ITimeSeries iTimeSeries, INullableReader reader, DatabaseRequestArgs args, TimeSeriesDatabaseContext requester)
        {
            int toEntityID = reader.GetInt32(2);
            DateTime valueDate = reader.GetDateTime(3);
            DateTime declarationDate = reader.GetDateTime(4);
            double? value = reader.GetNullableDouble(5);
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
          ((NumericConstituentRevisableTimeSeries)iTimeSeries).Add(entity, valueDate, declarationDate, value, nonKeyedAttributeSet);
        }

        


    }


}
