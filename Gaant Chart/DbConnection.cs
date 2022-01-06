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
    public class DbConnection
    {
        //NOTE ALL date Strings need to be in MM-dd-YYYY


        public SQLiteConnection myConnection;

        public DbConnection()
        {

            if (!File.Exists("./GaantDb.db"))
            {
                SQLiteConnection.CreateFile("GaantDb1.db");
            }

            myConnection = new SQLiteConnection("Data Source = GaantDb1.db");
            this.OpenConnection();

            using(SQLiteCommand cmd = new SQLiteCommand(myConnection))
            {
                cmd.CommandText = "CREATE TABLE IF NOT EXISTS Models (Name TEXT NOT NULL UNIQUE, " +
                    "StartDate TEXT NOT NULL, EndDate TEXT)";
                cmd.ExecuteNonQuery();

                cmd.CommandText = "CREATE TABLE IF NOT EXISTS Tasks (ModelId INT NOT NULL, " +
                    "NAME TEXT NOT NULL, StartDate TEXT NOT NULL, EndDate Text, UserName TEXT)";
                cmd.ExecuteNonQuery();

                cmd.CommandText = "CREATE TABLE IF NOT EXISTS Users (Name TEXT NOT NULL, password TEXT)";
                cmd.ExecuteNonQuery();

                cmd.CommandText = "CREATE TABLE IF NOT EXISTS Authorization (UserID INT NOT NULL, TaskId INT NOT NULL)";
                cmd.ExecuteNonQuery();
            }

            this.CloseConnection();

        }

        public int InsertModel(String modelName, DateTime date)
        {
            int modelId;

            String dateString = date.ToString("MM-dd-yyyy");

            this.OpenConnection();
            using(SQLiteCommand cmd = new SQLiteCommand(myConnection))
            {
                cmd.CommandText = "INSERT INTO Models(Name, StartDate, EndDate) VALUES (@Name, @StartDate, @EndDate)";
                cmd.Parameters.AddWithValue("@Name", modelName);
                cmd.Parameters.AddWithValue("@StartDate", dateString);
                cmd.Parameters.AddWithValue("@EndDate", null);
                cmd.Prepare();
                cmd.ExecuteNonQuery();

                modelId = getModelId(modelName);


                foreach(KeyValuePair<String, int> task in data.taskStartDelayPlanned)
                {
                    date.AddDays(task.Value);
                    String taskStartDate = date.ToString("MM-dd-yyyy");

                    cmd.CommandText = "INSERT INTO Tasks(ModelId, Name, StartDate, EndDate, UserName) VALUES (@ModelId, @Name, @StartDate, @EndDate, @UserName)";
                    cmd.Parameters.AddWithValue("@ModelId", modelId);
                    cmd.Parameters.AddWithValue("@Name", task.Key);
                    cmd.Parameters.AddWithValue("@StartDate", taskStartDate);
                    cmd.Parameters.AddWithValue("@EndDate", null);
                    cmd.Parameters.AddWithValue(@"UserName", null);
                    cmd.Prepare();
                    cmd.ExecuteNonQuery();
                }

            }
            this.CloseConnection();

            return modelId;
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
            this.CloseConnection();
            return Id;

        }

        public Dictionary<String, (String, String, String)> getTasks(int ModelId)
        {
            Dictionary<String, (String, String, String)> tasksDict = new Dictionary<String, (String, String, String)>();

            this.OpenConnection();
            using(SQLiteCommand cmd = new SQLiteCommand(myConnection))
            {
                cmd.CommandText = "SELECT Name, StartDate, EndDate, UserName FROM Tasks WHERE ModelId = @ModelId";
                cmd.Parameters.AddWithValue("@ModelId", ModelId);
                cmd.Prepare();
                using(SQLiteDataReader rdr = cmd.ExecuteReader())
                {
                    while(rdr.Read())
                    {
                        String taskName = rdr.GetString(0);
                        String startDate = rdr.GetString(1);
                        String endDate = rdr.GetString(2);
                        String userName = rdr.GetString(3);

                        if (!String.IsNullOrEmpty(endDate)) continue;

                        (String, String, String) completedTask = (startDate, endDate, userName);
                        tasksDict[taskName] = completedTask;
                    }
                }
            }
            this.CloseConnection();
            return tasksDict;
        }
        
        public List<(String, int)> getModelNames()
        {
            List<(String, int)> modelNames = new List<(String, int)>();

            this.OpenConnection();
            using(SQLiteCommand cmd = new SQLiteCommand(myConnection))
            {
                cmd.CommandText = "SELECT Name, rowid from Models";
                using(SQLiteDataReader rdr = cmd.ExecuteReader())
                {
                    while (rdr.Read())
                    {
                        modelNames.Add((rdr.GetString(0), rdr.GetInt32(1)));
                    }
                }
            }

            return modelNames;
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

    }
}
