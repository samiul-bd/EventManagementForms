using EventManagementForms.DAL;
using EventManagementForms.Entities;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EventManagementForms.Repositories
{
    public class EventRepo
    {
        DatabaseGateWay dal = new DatabaseGateWay();
        public DataTable GetAllCustomers()
        {
            DataTable dt = new DataTable();
            dt = dal.GetAllCustomers();
            return dt;
        }

        public DataTable GetAllEvents()
        {
            DataTable dt = new DataTable();
            dt = dal.GetAllEvents();
            return dt;
        }

        public int SaveEvent(Event Event)
        {
            int saveCount = dal.SaveEvent(Event);
            return saveCount;
        }

        public DataTable GetProgramsByEventId(int EventId)
        {
            DataTable dt = dal.GetAllProgramsByEventId(EventId);
            return dt;
        }

        public int DeleteProgramsByEventId(int EventId, int programsId)
        {
            int deleteResult = dal.DeleteProgramsByEventAndProgramsId(EventId, programsId);
            return deleteResult;
        }

        public DataTable GetEventById(int EventId)
        {
            DataTable dt = new DataTable();
            dt = dal.GetEventByEventId(EventId);
            return dt;
        }

        public int DeleteEvent(int EventId)
        {
            int count = dal.DeleteEventByEventId(EventId);
            return count;

        }

        public int DeleteProgramsByEventId(int EventId)
        {
            int deleteResult = dal.DeleteProgramsByEventId(EventId);
            return deleteResult;
        }

        public int UpdateEvent(Event Event)
        {
            int updateCount = dal.UpdateEvent(Event);
            return updateCount;
        }

        public DataTable GetAllEventInfo()
        {
            DataTable dt = new DataTable();
            dt = dal.GetAllEventInfo();
            return dt;
        }

        public DataTable GetAllPrograms()
        {
            DataTable dt = dal.GetAllPrograms();
            return dt;
        }
    }
}
