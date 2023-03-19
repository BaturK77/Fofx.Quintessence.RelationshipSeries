using Fofx.Quintessence.RelationshipSeries.Helpers.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Fofx.Quintessence.RelationshipSeries.Helpers
{
    public static class ReadHelper
    {
        public static void ReadByType<T>(int[] entities, int[] timeseries, int[] relationship, DatabaseRequestArgs args, TimeSeriesDatabaseContext requester, Dictionary<QuickID, ITimeSeriesKey> keys, SortedList<ITimeSeriesKey, ITimeSeries> dbTimeSeriesResult, INullableReader readedEntity)
        {

            object ncs = null;
            object pointCollection = null;
            object revisionCollection = null;
            object constituentPoint = null;

            ITimeSeries referenceTimeSeries ;
            ITimeSeriesKey referenceKey;

            Preference codeTypePreference = null;
            int previousEntityID = -1;
            int previousTimeSeriesValueID = -1;
            int previousRelationshipValueID = -1;
            DateTime previousValueDate = DateTime.MinValue;
            DateTime previousDeclarationDate = DateTime.MinValue;

            bool readStringValue = false;
            if (typeof(T) == typeof(TextConstituentArrayTimeSeries) || typeof(T) == typeof(StringCharacteristicRevisableTimeSeries) || typeof(T) == typeof(TextConstituentRevisableTimeSeries))
                readStringValue = true;

            using (INullableReader reader = readedEntity)
            {
                while (reader.Read())
                {
                    SeriesEntity seriesEntity = ReadSeriesEntity.ReadSeries(reader, readStringValue);
                    IEntityDescriptor entity = null;

                    NonKeyedAttributeSet nonKeyedAttributeSet = null;
                    if (seriesEntity.NonKeyedAttributeSetId != null)
                        nonKeyedAttributeSet = args.Translator.GetNonKeyedAttributeSet((int)seriesEntity.NonKeyedAttributeSetId);

                    if (seriesEntity.EntityID != previousEntityID || previousTimeSeriesValueID != seriesEntity.TimeSeriesValueID || previousRelationshipValueID != seriesEntity.RelationshipValueID)
                    {
                        QuickID qid = new QuickID(seriesEntity.EntityID, seriesEntity.TimeSeriesValueID, seriesEntity.RelationshipValueID);
                        if (keys.TryGetValue(qid, out referenceKey))
                        {
                            RelationshipTimeSeriesKey tsk = referenceKey as RelationshipTimeSeriesKey;
                            if (tsk == null || tsk.Relationship == null || tsk.Relationship.Source == null)
                                codeTypePreference = null;
                            else
                            {
                                if (codeTypePreference != null && tsk.Relationship.Source.CodePreference != null)
                                {
                                    if (codeTypePreference.Id != tsk.Relationship.Source.CodePreference.Id)
                                        requester.EntityLookup.Clear();

                                    codeTypePreference = tsk.Relationship.Source.CodePreference;
                                }
                                else
                                {
                                    requester.EntityLookup.Clear();
                                    codeTypePreference = tsk.Relationship.Source.CodePreference;
                                }
                            }
                        }

                        qid = new QuickID(previousEntityID, previousTimeSeriesValueID, previousRelationshipValueID);
                        if (keys.TryGetValue(qid, out referenceKey))
                        {
                            if (typeof(T) == typeof(NumericConstituentRevisableTimeSeries))
                            {

                                if (dbTimeSeriesResult.TryGetValue(referenceKey, out referenceTimeSeries))
                                {

                                    ncs = (referenceTimeSeries as NumericConstituentRevisableTimeSeries);
                                    if (ncs != null)
                                    {
                                        (ncs as NumericConstituentRevisableTimeSeries).Add(pointCollection);
                                    }

                                }
                            }
                            else
                                if (typeof(T) == typeof(NumericConstituentArrayTimeSeries))
                            {
                                if (dbTimeSeriesResult.TryGetValue(referenceKey, out referenceTimeSeries))
                                {

                                    ncs = (referenceTimeSeries as NumericConstituentArrayTimeSeries);
                                    if (ncs != null)
                                    {
                                        (ncs as NumericConstituentArrayTimeSeries).Add(pointCollection);
                                    }

                                }
                            }
                            else
                                if (typeof(T) == typeof(NumericCharacteristicRevisableTimeSeries))
                            {
                                if (dbTimeSeriesResult.TryGetValue(referenceKey, out referenceTimeSeries))
                                {

                                    ncs = (referenceTimeSeries as NumericCharacteristicRevisableTimeSeries);
                                    if (ncs != null)
                                    {
                                        (ncs as NumericCharacteristicRevisableTimeSeries).Add(pointCollection);
                                    }

                                }
                            }
                        }
                        else
                        if (typeof(T) == typeof(TextConstituentArrayTimeSeries))
                        {
                            if (dbTimeSeriesResult.TryGetValue(referenceKey, out referenceTimeSeries))
                            {

                                ncs = (referenceTimeSeries as TextConstituentArrayTimeSeries);
                                if (ncs != null)
                                {
                                    (ncs as TextConstituentArrayTimeSeries).Add(pointCollection);
                                }

                            }

                        }
                        else
                            if (typeof(T) == typeof(StringCharacteristicRevisableTimeSeries))
                        {
                            if (dbTimeSeriesResult.TryGetValue(referenceKey, out referenceTimeSeries))
                            {

                                ncs = (referenceTimeSeries as StringCharacteristicRevisableTimeSeries);
                                if (ncs != null)
                                {
                                    (ncs as StringCharacteristicRevisableTimeSeries).Add(pointCollection);
                                }

                            }

                        }
                        else
                        if (typeof(T) == typeof(TextConstituentRevisableTimeSeries))
                        {
                            if (keys.TryGetValue(qid, out referenceKey))
                            {
                                if (dbTimeSeriesResult.TryGetValue(referenceKey, out referenceTimeSeries))
                                {
                                    ncs = referenceTimeSeries as TextConstituentRevisableTimeSeries;
                                    if (ncs != null)
                                    {
                                        (ncs as TextConstituentRevisableTimeSeries).Add(pointCollection);
                                    }
                                }
                            }
                        }




                        if (typeof(T) == typeof(NumericConstituentRevisableTimeSeries))
                        {
                            pointCollection = new SortedList<DateTime, Revision<ConstituentNumericPoint>>();
                            revisionCollection = new Revision<ConstituentNumericPoint>(seriesEntity.ValueDate);
                            (pointCollection as SortedList<DateTime, Revision<ConstituentNumericPoint>>).Add(seriesEntity.ValueDate, (Revision<ConstituentNumericPoint>)revisionCollection);
                            constituentPoint = new ConstituentNumericPoint(seriesEntity.DeclarationDate, seriesEntity.ValueDate, nonKeyedAttributeSet);
                            (revisionCollection as Revision<ConstituentNumericPoint>).Add(seriesEntity.DeclarationDate, (ConstituentNumericPoint)constituentPoint);
                        }
                        else
                            if (typeof(T) == typeof(NumericConstituentArrayTimeSeries))
                        {

                            pointCollection = new SortedList<DateTime, Revision<ConstituentNumericArrayPoint>>();
                            revisionCollection = new Revision<ConstituentNumericArrayPoint>(seriesEntity.ValueDate);
                            (pointCollection as SortedList<DateTime, Revision<ConstituentNumericArrayPoint>>).Add(seriesEntity.ValueDate, (Revision<ConstituentNumericArrayPoint>)revisionCollection);
                            constituentPoint = new ConstituentNumericArrayPoint(seriesEntity.DeclarationDate, seriesEntity.ValueDate, nonKeyedAttributeSet);
                            (revisionCollection as Revision<ConstituentNumericArrayPoint>).Add(seriesEntity.DeclarationDate, (ConstituentNumericArrayPoint)constituentPoint);

                        }
                        else
                            if (typeof(T) == typeof(NumericCharacteristicRevisableTimeSeries))
                        {
                            revisionCollection = new Revision<CharacteristicNumericPoint>(seriesEntity.ValueDate);
                            constituentPoint = new CharacteristicNumericPoint(nonKeyedAttributeSet);
                            (revisionCollection as Revision<CharacteristicNumericPoint>).Add(seriesEntity.DeclarationDate, (CharacteristicNumericPoint)constituentPoint);
                        }
                        else
                            if (typeof(T) == typeof(TextConstituentArrayTimeSeries))
                        {
                            pointCollection = new SortedList<DateTime, Revision<ConstituentTextArrayPoint>>();
                            revisionCollection = new Revision<ConstituentTextArrayPoint>(seriesEntity.ValueDate);
                            (pointCollection as SortedList<DateTime, Revision<ConstituentTextArrayPoint>>).Add(seriesEntity.ValueDate, (Revision<ConstituentTextArrayPoint>)revisionCollection);
                            constituentPoint = new ConstituentTextArrayPoint(seriesEntity.DeclarationDate, seriesEntity.ValueDate, nonKeyedAttributeSet);
                            (revisionCollection as Revision<ConstituentTextArrayPoint>).Add(seriesEntity.DeclarationDate, (ConstituentTextArrayPoint)constituentPoint);
                        }
                        else
                            if (typeof(T) == typeof(StringCharacteristicRevisableTimeSeries))
                        {
                            revisionCollection = new Revision<CharacteristicTextPoint>(seriesEntity.ValueDate);
                            constituentPoint = new CharacteristicTextPoint(nonKeyedAttributeSet);
                            (revisionCollection as Revision<CharacteristicTextPoint>).Add(seriesEntity.DeclarationDate, (CharacteristicTextPoint)constituentPoint);
                        }
                        else
                        if (typeof(T) == typeof(TextConstituentRevisableTimeSeries))
                        {
                            pointCollection = new SortedList<DateTime, Revision<ConstituentTextPoint>>();
                            revisionCollection = new Revision<ConstituentTextPoint>(seriesEntity.ValueDate);
                            (pointCollection as SortedList<DateTime, Revision<ConstituentTextPoint>>).Add(seriesEntity.ValueDate, (Revision<ConstituentTextPoint>)revisionCollection);
                            constituentPoint = new ConstituentTextPoint(seriesEntity.DeclarationDate, seriesEntity.ValueDate, nonKeyedAttributeSet);
                            (revisionCollection as Revision<ConstituentTextPoint>).Add(seriesEntity.DeclarationDate, (ConstituentTextPoint)constituentPoint);
                        }




                        previousEntityID = seriesEntity.EntityID;
                        previousTimeSeriesValueID = seriesEntity.TimeSeriesValueID;
                        previousRelationshipValueID = seriesEntity.RelationshipValueID;
                        previousValueDate = seriesEntity.ValueDate;
                        previousDeclarationDate = seriesEntity.DeclarationDate;
                    }
                    else if (seriesEntity.ValueDate != previousValueDate)
                    {
                        if (typeof(T) == typeof(NumericConstituentRevisableTimeSeries))
                        {
                            revisionCollection = new Revision<ConstituentNumericPoint>(seriesEntity.ValueDate);
                            (pointCollection as SortedList<DateTime, Revision<ConstituentNumericPoint>>).Add(seriesEntity.ValueDate, (Revision<ConstituentNumericPoint>)revisionCollection);
                            constituentPoint = new ConstituentNumericPoint(seriesEntity.DeclarationDate, seriesEntity.ValueDate, nonKeyedAttributeSet);
                            (revisionCollection as Revision<ConstituentNumericPoint>).Add(seriesEntity.DeclarationDate, (ConstituentNumericPoint)constituentPoint);
                        }
                        else
                            if (typeof(T) == typeof(NumericConstituentArrayTimeSeries))
                        {
                            revisionCollection = new Revision<ConstituentNumericArrayPoint>(seriesEntity.ValueDate);
                            (pointCollection as SortedList<DateTime, Revision<ConstituentNumericArrayPoint>>).Add(seriesEntity.ValueDate, (Revision<ConstituentNumericArrayPoint>)revisionCollection);
                            constituentPoint = new ConstituentNumericArrayPoint(seriesEntity.DeclarationDate, seriesEntity.ValueDate, nonKeyedAttributeSet);
                            (revisionCollection as Revision<ConstituentNumericArrayPoint>).Add(seriesEntity.DeclarationDate, (ConstituentNumericArrayPoint)constituentPoint);
                        }
                        else
                            if (typeof(T) == typeof(TextConstituentArrayTimeSeries))
                        {
                            revisionCollection = new Revision<ConstituentTextArrayPoint>(seriesEntity.ValueDate);
                            (pointCollection as SortedList<DateTime, Revision<ConstituentTextArrayPoint>>).Add(seriesEntity.ValueDate, (Revision<ConstituentTextArrayPoint>)revisionCollection);
                            constituentPoint = new ConstituentTextArrayPoint(seriesEntity.DeclarationDate, seriesEntity.ValueDate, nonKeyedAttributeSet);
                            (revisionCollection as Revision<ConstituentTextArrayPoint>).Add(seriesEntity.DeclarationDate, (ConstituentTextArrayPoint)constituentPoint);

                        }
                        else
                            if (typeof(T) == typeof(TextConstituentRevisableTimeSeries))
                        {
                            revisionCollection = new Revision<ConstituentTextPoint>(seriesEntity.ValueDate);
                            (pointCollection as SortedList<DateTime, Revision<ConstituentTextPoint>>).Add(seriesEntity.ValueDate, (Revision<ConstituentTextPoint>)revisionCollection);
                            constituentPoint = new ConstituentTextPoint(seriesEntity.DeclarationDate, seriesEntity.ValueDate, nonKeyedAttributeSet);
                            (revisionCollection as Revision<ConstituentTextPoint>).Add(seriesEntity.DeclarationDate, (ConstituentTextPoint)constituentPoint);
                        }
                        previousValueDate = seriesEntity.ValueDate;
                        previousDeclarationDate = seriesEntity.DeclarationDate;
                    }
                    else if (seriesEntity.DeclarationDate != previousDeclarationDate)
                    {
                        if (typeof(T) == typeof(NumericConstituentRevisableTimeSeries))
                        {
                            constituentPoint = new ConstituentNumericPoint(seriesEntity.DeclarationDate, seriesEntity.ValueDate, nonKeyedAttributeSet);
                            (revisionCollection as Revision<ConstituentNumericPoint>).Add(seriesEntity.DeclarationDate, (ConstituentNumericPoint)constituentPoint);
                        }
                        else
                            if (typeof(T) == typeof(NumericConstituentArrayTimeSeries))
                        {
                            constituentPoint = new ConstituentNumericArrayPoint(seriesEntity.DeclarationDate, seriesEntity.ValueDate, nonKeyedAttributeSet);

                            (revisionCollection as Revision<ConstituentNumericArrayPoint>).Add(seriesEntity.DeclarationDate, (ConstituentNumericArrayPoint)constituentPoint);
                        }
                        else
                            if (typeof(T) == typeof(TextConstituentArrayTimeSeries))
                        {
                            constituentPoint = new ConstituentTextArrayPoint(seriesEntity.DeclarationDate, seriesEntity.ValueDate, nonKeyedAttributeSet);
                            (revisionCollection as Revision<ConstituentTextArrayPoint>).Add(seriesEntity.DeclarationDate, (ConstituentTextArrayPoint)constituentPoint);
                        }
                        else
                            if (typeof(T) == typeof(TextConstituentRevisableTimeSeries))
                        {
                            constituentPoint = new ConstituentTextPoint(seriesEntity.DeclarationDate, seriesEntity.ValueDate, nonKeyedAttributeSet);
                            (revisionCollection as Revision<ConstituentTextPoint>).Add(seriesEntity.DeclarationDate, (ConstituentTextPoint)constituentPoint);
                        }
                        previousValueDate = seriesEntity.ValueDate;
                        previousDeclarationDate = seriesEntity.DeclarationDate;
                    }

                    if (!requester.EntityLookup.TryGetValue(seriesEntity.ToEntityID, out entity))
                    {
                        if (!args.Translator.TryGetEntityDescriptorByID(seriesEntity.ToEntityID, codeTypePreference, out entity))
                            entity = new EntityDescriptor(seriesEntity.ToEntityID);
                        else
                            requester.EntityLookup.Add(seriesEntity.ToEntityID, entity);
                    }
                    if (typeof(T) == typeof(NumericConstituentRevisableTimeSeries))
                    {
                        (constituentPoint as ConstituentNumericPoint).Add(entity, seriesEntity.Value, nonKeyedAttributeSet);
                    }
                    else
                        if (typeof(T) == typeof(NumericConstituentArrayTimeSeries))
                    {
                        (constituentPoint as ConstituentNumericArrayPoint).Add(entity, seriesEntity.Value, nonKeyedAttributeSet);

                    }
                    if (typeof(T) == typeof(TextConstituentArrayTimeSeries))
                    {
                        (constituentPoint as ConstituentTextArrayPoint).Add(entity, new string[] { seriesEntity.StringValue }, nonKeyedAttributeSet);
                    }
                    else
                        if (typeof(T) == typeof(TextConstituentArrayTimeSeries))
                    {
                        (constituentPoint as ConstituentTextArrayPoint).Add(entity, seriesEntity.StringValue, nonKeyedAttributeSet);
                    }
                    else
                           if (typeof(T) == typeof(TextConstituentRevisableTimeSeries))
                    {
                        (constituentPoint as ConstituentTextPoint).Add(entity, seriesEntity.StringValue, nonKeyedAttributeSet);


                    }
                }
            }

            QuickID qid2 = new QuickID(previousEntityID, previousTimeSeriesValueID, previousRelationshipValueID);
            if (keys.TryGetValue(qid2, out referenceKey))
            {
                if (dbTimeSeriesResult.TryGetValue(referenceKey, out referenceTimeSeries))
                {

                    if (typeof(T) == typeof(NumericConstituentRevisableTimeSeries))
                    {

                        if (dbTimeSeriesResult.TryGetValue(referenceKey, out referenceTimeSeries))
                        {

                            ncs = (referenceTimeSeries as NumericConstituentRevisableTimeSeries);
                            if (ncs != null)
                            {
                                (ncs as NumericConstituentRevisableTimeSeries).Add(pointCollection);
                            }

                        }
                    }
                    else

                    if (typeof(T) == typeof(NumericConstituentArrayTimeSeries))
                    {

                        if (dbTimeSeriesResult.TryGetValue(referenceKey, out referenceTimeSeries))
                        {

                            ncs = (referenceTimeSeries as NumericConstituentArrayTimeSeries);
                            if (ncs != null)
                            {
                                (ncs as NumericConstituentArrayTimeSeries).Add(pointCollection);
                            }

                        }
                    }
                    else

                    if (typeof(T) == typeof(NumericCharacteristicRevisableTimeSeries))
                    {

                        if (dbTimeSeriesResult.TryGetValue(referenceKey, out referenceTimeSeries))
                        {

                            ncs = (referenceTimeSeries as NumericCharacteristicRevisableTimeSeries);
                            if (ncs != null)
                            {
                                (ncs as NumericCharacteristicRevisableTimeSeries).Add(pointCollection);
                            }

                        }
                    }
                    else
                    if (typeof(T) == typeof(TextConstituentArrayTimeSeries))
                    {
                        if (dbTimeSeriesResult.TryGetValue(referenceKey, out referenceTimeSeries))
                        {

                            ncs = (referenceTimeSeries as TextConstituentArrayTimeSeries);
                            if (ncs != null)
                            {
                                (ncs as TextConstituentArrayTimeSeries).Add(pointCollection);
                            }

                        }

                    }
                    else
                         if (typeof(T) == typeof(TextConstituentArrayTimeSeries))
                    {
                        if (dbTimeSeriesResult.TryGetValue(referenceKey, out referenceTimeSeries))
                        {

                            ncs = (referenceTimeSeries as TextConstituentArrayTimeSeries);
                            if (ncs != null)
                            {
                                (ncs as TextConstituentArrayTimeSeries).Add(pointCollection);
                            }

                        }

                    }
                    else
                        if (typeof(T) == typeof(TextConstituentRevisableTimeSeries))
                    {
                        if (dbTimeSeriesResult.TryGetValue(referenceKey, out referenceTimeSeries))
                        {
                            ncs = referenceTimeSeries as TextConstituentRevisableTimeSeries;
                            if (ncs != null)
                            {
                                (ncs as TextConstituentRevisableTimeSeries).Add(pointCollection);
                            }
                        }
                    }

                }
            }
        }
    }
}
