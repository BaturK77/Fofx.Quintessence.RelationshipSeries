using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Fofx.Quintessence.RelationshipSeries.Helpers
{
    public struct QuickID : IEquatable<QuickID>
    {
        private int m_EntityID;
        private int m_TimeseriesValueID;
        private int m_RelationshipValueID;
        private int m_HashCode;

        public QuickID(int entity, int timeseries, int relationship)
        {
            m_EntityID = entity;
            m_TimeseriesValueID = timeseries;
            m_RelationshipValueID = relationship;

            unchecked
            {
                m_HashCode = 17;
                m_HashCode = m_HashCode * 31 + m_EntityID.GetHashCode();
                m_HashCode = m_HashCode * 31 + m_TimeseriesValueID.GetHashCode();
                m_HashCode = m_HashCode * 31 + m_RelationshipValueID.GetHashCode();
            }
        }

        public bool Equals(QuickID qi)
        {
            return m_EntityID == qi.m_EntityID && m_TimeseriesValueID == qi.m_TimeseriesValueID && m_RelationshipValueID == qi.m_RelationshipValueID;
        }

        public override bool Equals(object obj)
        {
            QuickID qi = (QuickID)obj;
            return m_EntityID == qi.m_EntityID && m_TimeseriesValueID == qi.m_TimeseriesValueID && m_RelationshipValueID == qi.m_RelationshipValueID;
        }

        public override int GetHashCode()
        {
            return m_HashCode;
        }
    }
}
