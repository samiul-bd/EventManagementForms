using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EventManagementForms.Entities;

namespace EventManagementForms.DAL
{
    public class DatabaseGateWay
    {
        private readonly string conStr = ConfigurationManager.ConnectionStrings["db"].ConnectionString;

        public DataTable GetAllCustomers()
        {
            DataTable dt = new DataTable();
            using (SqlConnection sqlcon = new SqlConnection(conStr))
            {
                string query = "SELECT * FROM Customers";
                SqlCommand cmd = new SqlCommand(query, sqlcon);
                sqlcon.Open();
                SqlDataAdapter da = new SqlDataAdapter(cmd);
                da.Fill(dt);

            }
            return dt;
        }


        public DataTable GetAllEvents()
        {
            DataTable dt = new DataTable();
            using (SqlConnection sqlcon = new SqlConnection(conStr))
            {
                string query = @"SELECT [EventId],
                                IsMultipleProgramEvent, 
                                EventName,
                                Format(s.StartDate,'yyyy-MM-dd') AS StartDate,
                                Format(s.EndDate,'yyyy-MM-dd') AS EndDate,
                                s.Budget,
                                ImageUrl,
                                CASE WHEN IsMultipleProgramEvent=1 THEN 'Single' ELSE 'Multiple' END AS EventType,
                                c.CustomerName
                                FROM Events s JOIN Customers c ON s.CustomerId=c.CustomerId";
                SqlCommand cmd = new SqlCommand(query, sqlcon);
                sqlcon.Open();
                SqlDataAdapter da = new SqlDataAdapter(cmd);
                da.Fill(dt);

            }
            return dt;
        }


        public DataTable GetAllProgramsByEventId(int eventId)
        {
            DataTable dt = new DataTable();
            using (SqlConnection sqlcon = new SqlConnection(conStr))
            {
                string query = @"SELECT ProgramsId,
                                ProgramsName, 
                                Duration,
                                EventId
                                FROM Programs WHERE EventId =" + eventId;
                SqlCommand cmd = new SqlCommand(query, sqlcon);
                sqlcon.Open();
                SqlDataAdapter da = new SqlDataAdapter(cmd);
                da.Fill(dt);

            }
            return dt;
        }


        public DataTable GetEventByEventId(int eventId)
        {
            DataTable dt = new DataTable();
            using (SqlConnection sqlcon = new SqlConnection(conStr))
            {
                string query = @"SELECT [EventId],
                                IsMultipleProgramEvent, 
                                EventName,
                                Format(s.StartDate,'yyyy-MM-dd') AS StartDate,
                                Format(s.EndDate,'yyyy-MM-dd') AS EndDate,
                                s.Budget,
                                ImageUrl,
                                CASE WHEN IsMultipleProgramEvent=1 THEN 'Single' ELSE 'Multiple' END AS EventType,
                                c.CustomerName, 
                                s.CustomerId
                                FROM Events s JOIN Customers c ON s.CustomerId=c.CustomerId
                                WHERE s.EventId =" + eventId;
                SqlCommand cmd = new SqlCommand(query, sqlcon);
                sqlcon.Open();
                SqlDataAdapter da = new SqlDataAdapter(cmd);
                da.Fill(dt);

            }
            return dt;
        }
        public DataTable GetAllEventInfo()
        {
            DataTable dt = new DataTable();

            using (SqlConnection sqlCon = new SqlConnection(conStr))
            {
                SqlCommand cmd = sqlCon.CreateCommand();
                cmd.CommandType = CommandType.Text;
                cmd.CommandText = "";
                sqlCon.Open();
                var rdr = cmd.ExecuteReader();
                dt.Load(rdr, LoadOption.Upsert);

            }
            return dt;
        }

        public DataTable GetAllPrograms()
        {
            DataTable dt = new DataTable();
            using (SqlConnection sqlcon = new SqlConnection(conStr))
            {
                string query = @"SELECT ProgramsId,
                                ProgramsName, 
                                Duration,
                                EventId
                                FROM Programs";
                SqlCommand cmd = new SqlCommand(query, sqlcon);
                sqlcon.Open();
                SqlDataAdapter da = new SqlDataAdapter(cmd);
                da.Fill(dt);

            }
            return dt;
        }


        public int SaveEvent(Event events)
        {
            int eventId = 0;
            int programsInserted = 0;
            using (SqlConnection sqlcon = new SqlConnection(conStr))
            {
                sqlcon.Open();
                using (SqlTransaction tran = sqlcon.BeginTransaction())
                {
                    try
                    {
                        string query = @"INSERT INTO Events (ImageUrl,EventName,IsMultipleProgramEvent,StartDate,EndDate,CustomerId,Budget) 
                        VALUES (@ImageUrl,@EventName,@IsMultipleProgramEvent,@StartDate,@EndDate,@CustomerId,@Budget); 
                        SELECT SCOPE_IDENTITY();";
                        SqlCommand cmd = new SqlCommand(query, sqlcon, tran);
                        cmd.CommandType = CommandType.Text;
                        cmd.Parameters.Add("@ImageUrl", SqlDbType.VarChar).Value = events.ImageUrl;
                        cmd.Parameters.Add("@EventName", SqlDbType.VarChar).Value = events.EventName;
                        cmd.Parameters.Add("@IsMultipleProgramEvent", SqlDbType.Bit).Value = events.IsMultipleProgramEvent;
                        cmd.Parameters.Add("@StartDate", SqlDbType.DateTime).Value = events.StartDate;
                        cmd.Parameters.Add("@EndDate", SqlDbType.DateTime).Value = events.EndDate;
                        cmd.Parameters.Add("@CustomerId", SqlDbType.Int).Value = events.CustomerId;
                        cmd.Parameters.Add("@Budget", SqlDbType.Int).Value = events.Budget;
                        object result = cmd.ExecuteScalar();
                        if (result != null && result != DBNull.Value)
                        {
                            eventId = Convert.ToInt32(result);
                        }
                        foreach (Programs p in events.Programs)
                        {
                            var query2 = @"INSERT INTO Programs (EventId,ProgramsName,Duration) 
                                            VALUES (@EventId,@ProgramsName,@Duration)";
                            SqlCommand mcmd = new SqlCommand(query2, sqlcon, tran);
                            mcmd.CommandType = CommandType.Text;
                            mcmd.Parameters.Add("@EventId", SqlDbType.Int).Value = eventId;
                            mcmd.Parameters.Add("@ProgramsName", SqlDbType.VarChar).Value = p.ProgramsName;
                            mcmd.Parameters.Add("@Duration", SqlDbType.Int).Value = p.Duration;
                            programsInserted += mcmd.ExecuteNonQuery();
                        }
                        tran.Commit();
                        return eventId;
                    }
                    catch (Exception ex)
                    {

                        tran.Rollback();
                        Console.WriteLine($"Error Occured: {ex.Message}");
                        throw;
                    }
                }
            }


        }


        public int DeleteProgramsByEventAndProgramsId(int eventId, int programsId)
        {
            int count = 0;
            using (SqlConnection sqlcon = new SqlConnection(conStr))
            {
                sqlcon.Open();
                using (SqlTransaction tran = sqlcon.BeginTransaction())
                {
                    try
                    {
                        using (SqlCommand deleteModulesCmd = new SqlCommand("DELETE FROM Programs WHERE EventId=@EventId AND ProgramsId=@ProgramsId", sqlcon, tran))
                        {
                            deleteModulesCmd.CommandType = CommandType.Text;
                            deleteModulesCmd.Parameters.Add("@EventId", SqlDbType.Int).Value = eventId;
                            deleteModulesCmd.Parameters.Add("@ProgramsId", SqlDbType.Int).Value = programsId;
                            count = deleteModulesCmd.ExecuteNonQuery();
                        }
                        tran.Commit();
                        return count;
                    }
                    catch (Exception)
                    {
                        tran.Rollback();
                        return 0;
                    }
                }


            }
        }


        public int DeleteProgramsByEventId(int eventId)
        {
            int count = 0;
            using (SqlConnection sqlcon = new SqlConnection(conStr))
            {
                sqlcon.Open();
                using (SqlTransaction tran = sqlcon.BeginTransaction())
                {
                    try
                    {
                        using (SqlCommand deleteModulesCmd = new SqlCommand("DELETE FROM Programs WHERE EventId=@EventId ", sqlcon, tran))
                        {
                            deleteModulesCmd.CommandType = CommandType.Text;
                            deleteModulesCmd.Parameters.Add("@EventId", SqlDbType.Int).Value = eventId;

                            count = deleteModulesCmd.ExecuteNonQuery();
                        }
                        tran.Commit();
                        return count;
                    }
                    catch (Exception)
                    {
                        tran.Rollback();
                        return 0;
                    }
                }
            }
        }

        public int DeleteEventByEventId(int eventId)
        {
            int count = 0;
            using (SqlConnection sqlcon = new SqlConnection(conStr))
            {
                sqlcon.Open();
                using (SqlTransaction tran = sqlcon.BeginTransaction())
                {
                    try
                    {
                        using (SqlCommand deleteEventCmd = new SqlCommand("DELETE FROM Events WHERE EventId=@EventId ", sqlcon, tran))
                        {
                            deleteEventCmd.CommandType = CommandType.Text;
                            deleteEventCmd.Parameters.Add("@EventId", SqlDbType.Int).Value = eventId;
                            count = deleteEventCmd.ExecuteNonQuery();
                        }
                        tran.Commit();
                        return count;
                    }
                    catch (Exception)
                    {
                        tran.Rollback();
                        return 0;
                    }
                }


            }
        }

        public int UpdateEvent(Event events)
        {
            int count = 0;
            using (SqlConnection sqlcon = new SqlConnection(conStr))
            {
                sqlcon.Open();
                using (SqlTransaction tran = sqlcon.BeginTransaction())
                {
                    try
                    {
                        var query = @"UPDATE Events SET 
                                    ImageUrl=@ImageUrl,
                                    EventName=@EventName,
                                    IsMultipleProgramEvent=@IsMultipleProgramEvent,
                                    StartDate=@StartDate,
                                    EndDate=@EndDate,
                                    CustomerId=@CustomerId,
                                    Budget=@Budget
                                    WHERE EventId=@EventId";
                        using (SqlCommand cmd = new SqlCommand(query, sqlcon, tran))
                        {
                            cmd.CommandType = CommandType.Text;
                            cmd.Parameters.Add("@EventId", SqlDbType.Int).Value = events.EventId;
                            cmd.Parameters.Add("@ImageUrl", SqlDbType.VarChar).Value = events.ImageUrl;
                            cmd.Parameters.Add("@EventName", SqlDbType.VarChar).Value = events.EventName;
                            cmd.Parameters.Add("@IsMultipleProgramEvent", SqlDbType.Bit).Value = events.IsMultipleProgramEvent;
                            cmd.Parameters.Add("@StartDate", SqlDbType.DateTime).Value = events.StartDate;
                            cmd.Parameters.Add("@EndDate", SqlDbType.DateTime).Value = events.EndDate;
                            cmd.Parameters.Add("@CustomerId", SqlDbType.Int).Value = events.CustomerId;
                            cmd.Parameters.Add("@Budget", SqlDbType.Int).Value = events.Budget;
                            count = cmd.ExecuteNonQuery();
                        }
                        if (count > 0)
                        {
                            using (SqlCommand deleteModulesCmd = new SqlCommand("DELETE FROM Programs WHERE EventId = @EventId", sqlcon, tran))
                            {
                                deleteModulesCmd.CommandType = CommandType.Text;
                                deleteModulesCmd.Parameters.Add("@EventId", SqlDbType.Int).Value = events.EventId;
                                deleteModulesCmd.ExecuteNonQuery();
                            }

                            foreach (Programs p in events.Programs)
                            {
                                var query2 = @"INSERT INTO Programs (EventId,ProgramsName,Duration) 
                                            VALUES (@EventId,@ProgramsName,@Duration)";
                                using (SqlCommand pCmd = new SqlCommand(query2, sqlcon, tran))
                                {
                                    pCmd.CommandType = CommandType.Text;
                                    pCmd.Parameters.Add("@EventId", SqlDbType.Int).Value = p.EventId;
                                    pCmd.Parameters.Add("@ModuleName", SqlDbType.VarChar).Value = p.ProgramsName;
                                    pCmd.Parameters.Add("@Duration", SqlDbType.Int).Value = p.Duration;
                                    pCmd.ExecuteNonQuery();
                                }
                            }
                        }

                        tran.Commit();

                    }
                    catch (Exception ex)
                    {
                        tran.Rollback();
                        Console.Write(ex.Message);
                    }
                }
            }
            return count;
        }

    }
}
