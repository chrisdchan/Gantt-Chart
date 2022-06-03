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
        public SQLiteCommand myCommand;
        public SQLiteDataReader myDataReader;

        public DbConnection()
        {

            String dbName = "GaantDb.db";

            if (!File.Exists("./" + dbName))
            {
                SQLiteConnection.CreateFile(dbName);
            }

            myConnection = new SQLiteConnection("Data Source = " + dbName);
            this.OpenConnection();

            using(SQLiteCommand myCommand = new SQLiteCommand(myConnection))
            {
                myCommand.CommandText = "CREATE TABLE IF NOT EXISTS Models (name TEXT NOT NULL UNIQUE, " +
                    "startDate DATETIME NOT NULL, endDate DATETIME)";
                myCommand.ExecuteNonQuery();

                myCommand.CommandText = "CREATE TABLE IF NOT EXISTS Tasks (modelId INT NOT NULL, " +
                    "name TEXT NOT NULL, startDate DATETIME NOT NULL, endDate DATETIME, userId INT)";
                myCommand.ExecuteNonQuery();

                myCommand.CommandText = "CREATE TABLE IF NOT EXISTS Users (name TEXT NOT NULL UNIQUE, password TEXT, requirePassword INT)";
                myCommand.ExecuteNonQuery();

                myCommand.CommandText = "CREATE TABLE IF NOT EXISTS Authorization (userId INT NOT NULL, taskId INT NOT NULL)";
                myCommand.ExecuteNonQuery();
            }

            this.CloseConnection();

        }

        public Model insertModel(String modelName, DateTime startDate)
        {
            this.OpenConnection();
            int modelId;
            String dateString = startDate.ToString("MM-dd-yyyy");

            using (SQLiteCommand myCommand = new SQLiteCommand(myConnection))
            {
                myCommand.CommandText = "INSERT INTO Models(name, startDate, endDate) VALUES (@name, @startDate, @endDate)";
                myCommand.Parameters.AddWithValue("@Name", modelName);
                myCommand.Parameters.AddWithValue("@StartDate", startDate);
                myCommand.Parameters.AddWithValue("@EndDate", null);
                myCommand.Prepare();
                myCommand.ExecuteNonQuery();

                myCommand.CommandText = "SELECT last_insert_rowid() FROM Models";
                modelId = (int)((long)myCommand.ExecuteScalar());


                DateTime taskStart = startDate;
                DateTime taskEnd = startDate;
                for(int i = 0; i < data.allTasks.Length; i++)
                {
                    taskStart = taskEnd;
                    String taskStartString = taskStart.ToString();
                    
                    myCommand.CommandText = "INSERT INTO Tasks(modelId, name, startDate, endDate, userId) VALUES (@ModelId, @Name, @StartDate, @EndDate, @UserId)";
                    myCommand.Parameters.AddWithValue("@ModelId", modelId);
                    myCommand.Parameters.AddWithValue("@Name", data.allTasks[i]);
                    myCommand.Parameters.AddWithValue("@StartDate", taskStart);
                    myCommand.Parameters.AddWithValue("@EndDate", null);
                    myCommand.Parameters.AddWithValue(@"UserId", null);
                    myCommand.Prepare();
                    myCommand.ExecuteNonQuery();
                }
            }
            this.CloseConnection();

            Model model = new Model(modelId, modelName, startDate);
            return model;

        }

        public void insertUser(User user)
        {
            this.OpenConnection();

            int reqPass = (user.reqPass) ? 1 : 0;

            using(myCommand = new SQLiteCommand(myConnection))
            {
                myCommand.CommandText = "INSERT INTO USERS(name, password, requirePassword) VALUES (@name, @password, @requirePassword)";
                myCommand.Parameters.AddWithValue("@name", user.name);
                myCommand.Parameters.AddWithValue("@password", user.password);
                myCommand.Parameters.AddWithValue("@requirePassword", reqPass);
                myCommand.Prepare();
                myCommand.ExecuteNonQuery();
            }
            this.CloseConnection();
        }

        public int modelExists(String modelName)
        {
            // Will return -1 if model DNE, else return the rowId
            int modelId = -1;

            this.OpenConnection();
            using(SQLiteCommand myCommand = new SQLiteCommand(myConnection))
            {
                myCommand.CommandText = "SELECT rowId FROM Models WHERE name = @Name";
                myCommand.Parameters.AddWithValue("@Name", modelName);
                myCommand.Prepare();
                using(SQLiteDataReader myDataReader = myCommand.ExecuteReader())
                {
                    if (myDataReader.Read()) modelId = myDataReader.GetInt32(0);
                }
            }

            return modelId;
        }

        public Model GetModel(int modelId)
        {
            Model model;
            this.OpenConnection();
            using(myCommand = new SQLiteCommand(myConnection))
            {
                model = extractEmptyModelFromDB(modelId);
                model = fillModelWithCompletedTasks(model);
            }

            this.CloseConnection();
            return model;
            
        }
        private Model extractEmptyModelFromDB(int modelId)
        {
            Model model;
            myCommand.CommandText = "SELECT name, startDate FROM Models where rowid = @rowid";
            myCommand.Parameters.AddWithValue("@rowid", modelId);
            myCommand.Prepare();

            using (myDataReader = myCommand.ExecuteReader())
            {
                model = extractEmptyModelFromDataReader(modelId);
            }
            return model;
        }
        private Model extractEmptyModelFromDataReader(int modelId)
        {
            Model model;

            if (myDataReader.Read())
            {
                String modelName = myDataReader.GetString(0);
                String startDateString = myDataReader.GetString(1);
                model = new Model(modelId, modelName, DateTime.Parse(startDateString));
            }
            else
            {
                throw new Exception("Cannot retrieve model query");
            }

            return model;

        }
        private Model fillModelWithCompletedTasks(Model model)
        {
            myCommand.CommandText = "SELECT name, startDate, endDate, userId FROM Tasks Where modelID = @ModelId";
            myCommand.Parameters.AddWithValue("@ModelId", model.modelId);
            myCommand.Prepare();

            using(myDataReader = myCommand.ExecuteReader())
            {
                model = fillModelwithCompletedTasksFromDataReader(model);
            }

            return model;
        }
        private Model fillModelwithCompletedTasksFromDataReader(Model model)
        {
            while(myDataReader.Read())
            {
                if(myDataReader["endDate"] != DBNull.Value)
                {
                    completeTaskFromDataReader(model);
                }
            }
            return model;
        }

        private void completeTaskFromDataReader(Model model)
        {
            int taskCount = myDataReader.StepCount - 1;
            int userId = (int)myDataReader["userId"];
            DateTime startDate = (DateTime)myDataReader["startDate"];
            DateTime endDate = (DateTime)myDataReader["endDate"];
            User user = data.getUser(userId);
            TaskCompletor taskCompletor = new TaskCompletor(user, startDate, endDate);
            model.tasks[taskCount].complete(taskCompletor);
        }

        public Model GetModel(String modelName)
        {
            Model model;
            int modelId;
            this.OpenConnection();
            using(SQLiteCommand myCommand = new SQLiteCommand(myConnection))
            {
                myCommand.CommandText = "SELECT rowid, startDate FROM Models where name = @name";
                myCommand.Parameters.AddWithValue("@name", modelName);
                myCommand.Prepare();
                using(SQLiteDataReader myDataReader = myCommand.ExecuteReader())
                {
                    if (myDataReader.Read())
                    {
                        modelId = myDataReader.GetInt32(0);
                        String startDateString = myDataReader.GetString(1);
                        model = new Model(modelId, modelName, DateTime.Parse(startDateString));
                    }
                    else
                    {
                        this.CloseConnection();
                        throw new Exception("Cannot retrieve model query");
                    }
                }

                myCommand.CommandText = "SELECT Name, StartDate, EndDate, UserId FROM Tasks Where ModelID = @ModelId";
                myCommand.Parameters.AddWithValue("@ModelId", modelId);
                myCommand.Prepare();
                using(SQLiteDataReader myDataReader = myCommand.ExecuteReader())
                {
                    int taskCount = 0;
                    while(myDataReader.Read())
                    {
                        String taskName = myDataReader.GetString(0);
                        String StartDateString = myDataReader.GetString(1);
                        String endDateString = myDataReader.GetString(2);
                        int userId = myDataReader.GetInt32(3);

                        if (userId == 0) break;
                        else
                        {
                            DateTime endDate = DateTime.Parse(endDateString);
                            DateTime startDate = DateTime.Parse(StartDateString);
                            User user = data.getUser(userId);
                            TaskCompletor taskCompletor = new TaskCompletor(user, startDate, endDate);
                            model.tasks[taskCount].complete(taskCompletor);
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
            using(SQLiteCommand myCommand = new SQLiteCommand(myConnection))
            {
                myCommand.CommandText = "SELECT rowid, Name, password, requirePassword FROM Users";
                using(myDataReader = myCommand.ExecuteReader())
                {
                    while(myDataReader.Read())
                    {
                        int userId = myDataReader.GetInt32(0);
                        String name = (String) myDataReader["name"];
                        String password = (String)myDataReader["password"];
                        int reqPassInt = (int) myDataReader["requirePassword"];

                        Boolean reqPass = (reqPassInt != 0);

                        User user = new User(userId, name, password, reqPass);
                        users.Add(user);
                    }
                }
            }

            return users;
        }
        
        public List<ModelTag> getModelTags()
        {
            List<ModelTag> modelTags = new List<ModelTag>();

            this.OpenConnection();
            using(SQLiteCommand myCommand = new SQLiteCommand(myConnection))
            {
                myCommand.CommandText = "SELECT rowid, name from Models";
                using(myDataReader = myCommand.ExecuteReader())
                {
                    while (myDataReader.Read())
                    {
                        int rowid = myDataReader.GetInt32(0);
                        String name = (String) myDataReader["name"];

                        modelTags.Add(new ModelTag(rowid, name));
                    }
                }
            }

            return modelTags;
        }

        public void deleteModel(int modelId)
        {
            this.OpenConnection();
            using(myCommand = new SQLiteCommand(myConnection))
            {
                deleteModelFromModelDB(modelId);
                deleteModelFromTasksDB(modelId);
            }


        }

        private void deleteModelFromModelDB(int modelId)
        {
            myCommand.CommandText = "DELETE FROM Models WHERE rowid = @rowid";
            myCommand.Parameters.AddWithValue("@rowid", modelId);
            myCommand.Prepare();
            myCommand.ExecuteNonQuery();
        }

        private void deleteModelFromTasksDB(int modelId)
        {
            myCommand.CommandText = "DELETE FROM Tasks WHERE modelId = @modelId";
            myCommand.Parameters.AddWithValue("@modelId", modelId);
            myCommand.Prepare();
            myCommand.ExecuteNonQuery();
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
