using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.SQLite;
using System.Windows;
using System.IO;
using Gaant_Chart.Models;
using Task = Gaant_Chart.Models.Task;

namespace Gaant_Chart
{
    public class DbConnection
    {
        //NOTE ALL date Strings need to be in MM-dd-YYYY


        public SQLiteConnection myConnection;

        public DbConnection()
        {

            String dbName = "GaantDb.db";

            if (!File.Exists("./" + dbName))
            {
                SQLiteConnection.CreateFile(dbName);
            }

            myConnection = new SQLiteConnection("Data Source = " + dbName);
            this.OpenConnection();

            using(SQLiteCommand cmd = new SQLiteCommand(myConnection))
            {
                cmd.CommandText = "CREATE TABLE IF NOT EXISTS Models (Name TEXT NOT NULL UNIQUE, " +
                    "StartDate TEXT NOT NULL, EndDate TEXT)";
                cmd.ExecuteNonQuery();

                cmd.CommandText = "CREATE TABLE IF NOT EXISTS Tasks (ModelId INT NOT NULL, " +
                    "NAME TEXT NOT NULL, StartDate TEXT NOT NULL, EndDate Text, UserId INT)";
                cmd.ExecuteNonQuery();

                cmd.CommandText = "CREATE TABLE IF NOT EXISTS Users (Name TEXT NOT NULL UNIQUE, password TEXT)";
                cmd.ExecuteNonQuery();

                cmd.CommandText = "CREATE TABLE IF NOT EXISTS Authorization (UserID INT NOT NULL, TaskId INT NOT NULL)";
                cmd.ExecuteNonQuery();
            }

            this.CloseConnection();

        }

        public Model InsertModel(String modelName, DateTime startDate)
        {
            this.OpenConnection();
            int modelId;
            String dateString = startDate.ToString("MM-dd-yyyy");

            using (SQLiteCommand cmd = new SQLiteCommand(myConnection))
            {
                cmd.CommandText = "INSERT INTO Models(Name, StartDate, EndDate) VALUES (@Name, @StartDate, @EndDate)";
                cmd.Parameters.AddWithValue("@Name", modelName);
                cmd.Parameters.AddWithValue("@StartDate", dateString);
                cmd.Parameters.AddWithValue("@EndDate", null);
                cmd.Prepare();
                cmd.ExecuteNonQuery();

                cmd.CommandText = "SELECT last_insert_rowid() FROM Models";
                modelId = (int)((long)cmd.ExecuteScalar());


                DateTime taskStart = startDate;
                DateTime taskEnd = startDate;
                for(int i = 0; i < data.allTasks.Length; i++)
                {
                    taskStart = taskEnd;
                    String taskStartString = taskStart.ToString();
                    
                    cmd.CommandText = "INSERT INTO Tasks(ModelId, Name, StartDate, EndDate, UserId) VALUES (@ModelId, @Name, @StartDate, @EndDate, @UserId)";
                    cmd.Parameters.AddWithValue("@ModelId", modelId);
                    cmd.Parameters.AddWithValue("@Name", data.allTasks[i]);
                    cmd.Parameters.AddWithValue("@StartDate", taskStartString);
                    cmd.Parameters.AddWithValue("@EndDate", null);
                    cmd.Parameters.AddWithValue(@"UserId", null);
                    cmd.Prepare();
                    cmd.ExecuteNonQuery();
                }
            }
            this.CloseConnection();

            Model model = new Model(modelId, modelName, startDate);
            return model;

        }

        public int modelExists(String modelName)
        {
            // Will return -1 if model DNE, else return the rowId
            int modelId = -1;

            this.OpenConnection();
            using(SQLiteCommand cmd = new SQLiteCommand(myConnection))
            {
                cmd.CommandText = "SELECT rowId FROM Models WHERE Name = @Name";
                cmd.Parameters.AddWithValue("@Name", modelName);
                cmd.Prepare();
                using(SQLiteDataReader rdr = cmd.ExecuteReader())
                {
                    if (rdr.Read()) modelId = rdr.GetInt32(0);
                }
            }

            return modelId;
        }

        public Model GetModel(int modelId)
        {
            Model model;
            this.OpenConnection();
            using(SQLiteCommand cmd = new SQLiteCommand(myConnection))
            {
                cmd.CommandText = "SELECT name, startDate FROM Models where rowid = @rowid";
                cmd.Parameters.AddWithValue("@rowid", modelId);
                cmd.Prepare();
                using(SQLiteDataReader rdr = cmd.ExecuteReader())
                {
                    if (rdr.Read())
                    {
                        String modelName = rdr.GetString(0);
                        String startDateString = rdr.GetString(1);
                        model = new Model(modelId, modelName, DateTime.Parse(startDateString));
                    }
                    else
                    {
                        this.CloseConnection();
                        throw new Exception("Cannot retrieve model query");
                    }
                }

                cmd.CommandText = "SELECT Name, StartDate, EndDate, UserId FROM Tasks Where ModelID = @ModelId";
                cmd.Parameters.AddWithValue("@ModelId", modelId);
                cmd.Prepare();
                using(SQLiteDataReader rdr = cmd.ExecuteReader())
                {
                    int taskCount = 0;
                    while(rdr.Read())
                    {
                        String taskName = rdr.GetString(0);
                        String StartDateString = rdr.GetString(1);
                        String endDateString = null;
                        if(rdr.GetString(2) != null) endDateString = rdr.GetString(2);
                        int userId = -1;
                        if(rdr.GetString(3) != null) userId = rdr.GetInt32(3);

                        if (userId == 0) break;
                        else
                        {
                            DateTime endDate = DateTime.Parse(endDateString);
                            DateTime startDate = DateTime.Parse(StartDateString);
                            User user = data.getUser(userId);
                            model.tasks[taskCount].complete(startDate, endDate, user);
                        }
                    }
                }
            }
            this.CloseConnection();
            return model;
        }

        public Model GetModel(String modelName)
        {
            Model model;
            int modelId;
            this.OpenConnection();
            using(SQLiteCommand cmd = new SQLiteCommand(myConnection))
            {
                cmd.CommandText = "SELECT rowid, startDate FROM Models where name = @name";
                cmd.Parameters.AddWithValue("@name", modelName);
                cmd.Prepare();
                using(SQLiteDataReader rdr = cmd.ExecuteReader())
                {
                    if (rdr.Read())
                    {
                        modelId = rdr.GetInt32(0);
                        String startDateString = rdr.GetString(1);
                        model = new Model(modelId, modelName, DateTime.Parse(startDateString));
                    }
                    else
                    {
                        this.CloseConnection();
                        throw new Exception("Cannot retrieve model query");
                    }
                }

                cmd.CommandText = "SELECT Name, StartDate, EndDate, UserId FROM Tasks Where ModelID = @ModelId";
                cmd.Parameters.AddWithValue("@ModelId", modelId);
                cmd.Prepare();
                using(SQLiteDataReader rdr = cmd.ExecuteReader())
                {
                    int taskCount = 0;
                    while(rdr.Read())
                    {
                        String taskName = rdr.GetString(0);
                        String StartDateString = rdr.GetString(1);
                        String endDateString = rdr.GetString(2);
                        int userId = rdr.GetInt32(3);

                        if (userId == 0) break;
                        else
                        {
                            DateTime endDate = DateTime.Parse(endDateString);
                            DateTime startDate = DateTime.Parse(StartDateString);
                            User user = data.getUser(userId);
                            model.tasks[taskCount].complete(startDate, endDate, user);
                        }
                    }
                }
            }
            this.CloseConnection();
            return model;
        }

        public List<User> getUsers()
        {
            List<User> users = new List<User>();

            this.OpenConnection();
            using(SQLiteCommand cmd = new SQLiteCommand(myConnection))
            {
                cmd.CommandText = "SELECT rowid, Name, Password FROM Users";
                using(SQLiteDataReader rdr = cmd.ExecuteReader())
                {
                    while(rdr.Read())
                    {
                        int userId = rdr.GetInt32(0);
                        User user = new User(userId, rdr.GetString(1), rdr.GetString(2));
                        users.Add(user);
                    }
                }
            }

            return users;

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

        private void OpenConnection()
        {
            if(myConnection.State != System.Data.ConnectionState.Open)
            {
                myConnection.Open();
            }
        }

        private void CloseConnection()
        {
            if(myConnection.State != System.Data.ConnectionState.Closed)
            {
                myConnection.Close();
            }
        }

    }
}
