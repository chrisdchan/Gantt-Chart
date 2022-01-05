using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.SQLite;
using System.Windows;
using System.IO;

namespace Gaant_Chart
{
    class ModelDb
    {
        //NOTE ALL date Strings need to be in MM-dd-YYYY


        public SQLiteConnection myConnection;

        public ModelDb()
        {

            if (!File.Exists("./GaantDb.db"))
            {
                SQLiteConnection.CreateFile("GaantDb1.db");
            }

            myConnection = new SQLiteConnection("Data Source = GaantDb1.db");
            this.OpenConnection();

            using(SQLiteCommand cmd = new SQLiteCommand(myConnection))
            {
                cmd.CommandText = "CREATE TABLE IF NOT EXISTS Models (Name TEXT NOT NULL, " +
                    "StartDate TEXT NOT NULL, EndDate TEXT)";
                cmd.ExecuteNonQuery();

                cmd.CommandText = "CREATE TABLE IF NOT EXISTS Tasks (ModelId INT NOT NULL, " +
                    "NAME TEXT NOT NULL, StartDate TEXT NOT NULL, EndDate Text)";
                cmd.ExecuteNonQuery();

                cmd.CommandText = "CREATE TABLE IF NOT EXISTS Users (Name TEXT NOT NULL, password TEXT)";
                cmd.ExecuteNonQuery();
            }

            this.CloseConnection();

        }

        public void InsertModel(String modelName, String startDate)
        {
            this.OpenConnection();
            using(SQLiteCommand cmd = new SQLiteCommand(myConnection))
            {
                cmd.CommandText = "INSERT INTO Models(Name, StartDate, EndDate) VALUES (@Name, @StartDate, @EndDate)";
                cmd.Parameters.AddWithValue("@Name", modelName);
                cmd.Parameters.AddWithValue("@StartDate", startDate);
                cmd.Parameters.AddWithValue("@EndDate", null);
                cmd.Prepare();
                cmd.ExecuteNonQuery();

                int Id = getModelId(modelName);


                foreach(KeyValuePair<String, int> task in MainWindow.taskStartDelayPlanned)
                {
                    DateTime date = DateTime.ParseExact(startDate, "MM-dd-yyyy", System.Globalization.CultureInfo.InvariantCulture);
                    date.AddDays(task.Value);
                    String taskStartDate = date.ToString("MM-dd-yyyy");

                    cmd.CommandText = "INSERT INTO Tasks(ModelId, Name, StartDate, EndDate) VALUES (@ModelId, @Name, @StartDate, @EndDate)";
                    cmd.Parameters.AddWithValue("@ModelId", Id);
                    cmd.Parameters.AddWithValue("@Name", task.Key);
                    cmd.Parameters.AddWithValue("@StartDate", taskStartDate);
                    cmd.Parameters.AddWithValue("@EndDate", null);
                    cmd.Prepare();
                    cmd.ExecuteNonQuery();
                }

            }
        }

        private int getModelId(String Model)
        {
            int Id;
            this.OpenConnection();
            using(SQLiteCommand cmd = new SQLiteCommand(myConnection))
            {
                cmd.CommandText = "SELECT rowid FROM Models WHERE Name = @Name";
                cmd.Parameters.AddWithValue("@Name", Model);
                cmd.Prepare();
                using(SQLiteDataReader rdr = cmd.ExecuteReader())
                {
                    if (rdr.Read()) Id = rdr.GetInt32(0);
                    else Id = -1;
                }
            }

            return Id;

        }
        

        public void OpenConnection()
        {
            if(myConnection.State != System.Data.ConnectionState.Open)
            {
                myConnection.Open();
            }
        }

        public void CloseConnection()
        {
            if(myConnection.State != System.Data.ConnectionState.Closed)
            {
                myConnection.Close();
            }
        }

        /*
        public static Boolean initModel(String model, String date)
        {
            Boolean connected = false;
            using(SQLiteConnection con = new SQLiteConnection(cs, true))
            {
                con.Open();
                connected = true;
            }

            return connected;
        }
    
        public static Dictionary<String, (String, String)> getTasks(String model)
        {
            Dictionary<String, (String, String)> tasks = new Dictionary<String, (String, String)>();

            using(SQLiteConnection con = new SQLiteConnection(cs))
            {
                con.Open();
                using(SQLiteCommand cmd = new SQLiteCommand(con))
                {
                    cmd.CommandText = $"SELECT Models.name, Models.StartDate, Tasks.Name, Tasks.StartDate, Tasks.EndDate" +
                                        "FROM Models INNER JOIN Tasks ON Models.Id = Tasks.ModelId" +
                                        "WHERE Models.name = {model}";
                    using (SQLiteDataReader rdr = cmd.ExecuteReader())
                    {
                        while(rdr.Read())
                        {
                            String taskName = rdr.GetString(2);
                            String taskStartDate = rdr.GetString(3);
                            String taskEndDate = rdr.GetString(4);

                            tasks.Add(taskName, (taskStartDate, taskEndDate));
                            
                        }
                    }

                }
            }

            return tasks;
        }                
        */
    }
}
