﻿using System;
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

            createTables();
            insertTaskIdentifiers();
        }

        private void createTables()
        {
            this.OpenConnection();

            using(SQLiteCommand myCommand = new SQLiteCommand(myConnection))
            {
                myCommand.CommandText = "CREATE TABLE IF NOT EXISTS Models (name TEXT NOT NULL UNIQUE, " +
                    "startDate DATETIME NOT NULL, endDate DATETIME)";
                myCommand.ExecuteNonQuery();

                myCommand.CommandText = "CREATE TABLE IF NOT EXISTS Tasks (modelId INT NOT NULL, " +
                    "typeId INT NOT NULL, startDate DATETIME, endDate DATETIME, userAssignedId INT, userCompletedId INT)";
                myCommand.ExecuteNonQuery();

                myCommand.CommandText = "CREATE TABLE IF NOT EXISTS Users (name TEXT NOT NULL UNIQUE, password TEXT, requirePassword INT, category TEXT)";
                myCommand.ExecuteNonQuery();

                myCommand.CommandText = "CREATE TABLE IF NOT EXISTS Authorization (userId INT NOT NULL, taskTypeId INT NOT NULL)";
                myCommand.ExecuteNonQuery();

                myCommand.CommandText = "CREATE TABLE IF NOT EXISTS TaskIdentifiers (typeId INT NOT NULL UNIQUE, name TEXT NOT NULL UNIQUE)";
                myCommand.ExecuteNonQuery();
            }

            this.CloseConnection();

        }

        private void insertTaskIdentifiers()
        {
            this.OpenConnection();
            using(myCommand = new SQLiteCommand(myConnection))
            {
                String[] allTasks = data.allTasks;
                for(int i = 0; i < allTasks.Length; i++)
                {
                    String taskName = allTasks[i];

                    myCommand.CommandText = "INSERT OR IGNORE INTO TaskIdentifiers(typeId, name) VALUES (@typeId, @name)";
                    myCommand.Parameters.AddWithValue("@typeId", i);
                    myCommand.Parameters.AddWithValue("@name", taskName);
                    myCommand.Prepare();
                    myCommand.ExecuteNonQuery();
                }
            }
            this.CloseConnection();
        }


        public Model insertModel(String modelName, DateTime startDate)
        {
            this.OpenConnection();
            long modelId;
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
                modelId = (long)myCommand.ExecuteScalar();


                DateTime taskStart = startDate;
                DateTime taskEnd = startDate;
                for(int i = 0; i < data.allTasks.Length; i++)
                {
                    taskStart = taskEnd;
                    String taskStartString = taskStart.ToString();
                    
                    myCommand.CommandText = "INSERT INTO Tasks(modelId, typeId, startDate, endDate, userAssignedId, userCompletedId)" +
                        " VALUES (@ModelId, @typeId, @StartDate, @EndDate, @UserId)";
                    myCommand.Parameters.AddWithValue("@ModelId", modelId);
                    myCommand.Parameters.AddWithValue("@typeID", i);
                    myCommand.Parameters.AddWithValue("@StartDate", null);
                    myCommand.Parameters.AddWithValue("@EndDate", null);
                    myCommand.Parameters.AddWithValue(@"userAssignedId", null);
                    myCommand.Parameters.AddWithValue(@"userCompletedId", null);
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
            OpenConnection();

            int reqPass = (user.reqPass) ? 1 : 0;

            using(myCommand = new SQLiteCommand(myConnection))
            {
                myCommand.CommandText = "INSERT INTO USERS(name, password, requirePassword, category) VALUES (@name, @password, @requirePassword, @category";
                myCommand.Parameters.AddWithValue("@name", user.name);
                myCommand.Parameters.AddWithValue("@password", user.password);
                myCommand.Parameters.AddWithValue("@requirePassword", reqPass);
                myCommand.Parameters.AddWithValue("@category", user.category);
                myCommand.Prepare();
                myCommand.ExecuteNonQuery();

                myCommand.CommandText = "SELECT last_insert_rowid() FROM USERS";
                long rowid = (long)myCommand.ExecuteScalar();

                user.rowid = rowid;

                insertUserAuthorization(user);

            }
            CloseConnection();
        }
        private void insertUserAuthorization(User user)
        {

            for(int i = 0; i < data.allTasks.Length; i++)
            {
                if (user.authorization[i])
                {
                    myCommand.CommandText = "INSERT INTO Authorization(userId, taskTypeId) VALUES (@userId, @taskTypeId)";
                    myCommand.Parameters.AddWithValue("@userId", user.rowid);
                    myCommand.Parameters.AddWithValue("taskTypeId", i);
                    myCommand.Prepare();
                    myCommand.ExecuteNonQuery();
                }
            }

        }

        public int getModelId(String modelName)
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

            this.CloseConnection();

            return modelId;
        }

        public Model getModel(String modelName)
        {
            Model model;
            OpenConnection();
            using (myCommand = new SQLiteCommand(myConnection))
            {
                myCommand.CommandText = "SELECT rowid, startDate FROM Models where name = @name";
                myCommand.Parameters.AddWithValue("@name", modelName);
                myCommand.Prepare();

                using (myDataReader = myCommand.ExecuteReader())
                {
                    if (myDataReader.Read())
                    {
                        int rowid = myDataReader.GetInt32(0);
                        String startDateString = myDataReader.GetString(1);
                        model = new Model(rowid, modelName, DateTime.Parse(startDateString));
                    }
                    else
                    {
                        throw new Exception("Cannot retrieve model query");
                    }
                }

                model = fillUncompleteModelWithTasks(model);
            }
            CloseConnection();
            return model;
        }


        public Model getModel(int modelId)
        {
            Model model;
            OpenConnection();
            using(myCommand = new SQLiteCommand(myConnection))
            {
                myCommand.CommandText = "SELECT name, startDate FROM Models where rowid = @rowid";
                myCommand.Parameters.AddWithValue("@rowid", modelId);
                myCommand.Prepare();

                using (myDataReader = myCommand.ExecuteReader())
                {
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
                }

                model = fillUncompleteModelWithTasks(model);
            }

            CloseConnection();
            return model;
        }

        private Model fillUncompleteModelWithTasks(Model model)
        {
            using(myCommand = new SQLiteCommand(myConnection))
            {
                myCommand.CommandText = "SELECT tasks.rowid, tasks.typeId FROM tasks WHERE modelId = @modelId";
                myCommand.Parameters.AddWithValue("@modelId", model.rowid);
                myCommand.Prepare();

                using(myDataReader = myCommand.ExecuteReader())
                {
                    while(myDataReader.Read())
                    {
                        int typeId = (int)myDataReader["typeId"];
                        long rowid = (long)myDataReader["rowid"];

                        model.tasks[typeId].rowid = rowid;
                    }
                }
            }

            using(myCommand = new SQLiteCommand(myConnection))
            {
                myCommand.CommandText = "SELECT typeId, startDate, endDate, userid FROM tasks WHERE modelId = @modelId AND endDate IS NOT NULL";
                myCommand.Parameters.AddWithValue("@modelId", model.rowid);
                myCommand.Prepare();

                using (myDataReader = myCommand.ExecuteReader())
                {
                    while(myDataReader.Read())
                    {
                        int typeId = (int)myDataReader["typeId"];
                        DateTime startDate = (DateTime)myDataReader["startDate"];
                        DateTime endDate = (DateTime)myDataReader["endDate"];
                        int userid = (int)myDataReader["userid"];

                        model.completeTask(typeId, userid, startDate, endDate);
                    }
                }
            }

            return model;

        }

        public Dictionary<long, User> getUsers()
        {
            Dictionary<long, User> users = getTempUsers();
            foreach(var item in users)
            {
                User user = item.Value;
                user.authorization = MainWindow.myDatabase.getAuthorization(user.rowid);
            }
            return users;
        }
        private Dictionary<long, User> getTempUsers()
        {
            Dictionary<long, User> users = new Dictionary<long, User>();

            OpenConnection();
            using(SQLiteCommand myCommand = new SQLiteCommand(myConnection))
            {
                myCommand.CommandText = "SELECT rowid, Name, password, requirePassword FROM Users";
                using(myDataReader = myCommand.ExecuteReader())
                {
                    while(myDataReader.Read())
                    {
                        long rowid = (long)myDataReader["rowid"];
                        String name = (String) myDataReader["name"];
                        String password = (String)myDataReader["password"];
                        int reqPassInt = (int) myDataReader["requirePassword"];

                        Boolean reqPass = (reqPassInt != 0);

                        users.Add(rowid, new User(rowid, name, password, reqPass));
                    }
                }
            }
            CloseConnection();

            return users;
        }
        private Boolean[] getAuthorization(long userid)
        {
            Boolean[] result = new Boolean[data.allTasks.Length];

            OpenConnection();
            using(myCommand = new SQLiteCommand(myConnection))
            {
                myCommand.CommandText = "SELECT taskTypeId FROM Authorization WHERE userId = @userid";
                myCommand.Parameters.AddWithValue("@userid", userid);
                myCommand.Prepare();
                using(myDataReader = myCommand.ExecuteReader())
                {
                    while(myDataReader.Read())
                    {
                        int taskTypeId = (int)myDataReader["tasktypeId"];
                        result[taskTypeId] = true;
                    }
                }
            }
            CloseConnection();
            return result;
        }
        
        public List<ModelTag> getModelTags()
        {
            List<ModelTag> modelTags = new List<ModelTag>();

            OpenConnection();
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
            CloseConnection();

            return modelTags;
        }

        public void completeTask(long rowid, DateTime startDate, DateTime endDate)
        {
            this.OpenConnection();

            User user = data.currentUser;
            long userId = user.rowid;

            using (myCommand = new SQLiteCommand(myConnection))
            {
                myCommand.CommandText = "UPDATE tasks SET startDate = @startDate, endDate = @endDate, userId = @userId WHERE rowid = @taskId";
                myCommand.Parameters.AddWithValue("@startDate", startDate);
                myCommand.Parameters.AddWithValue("@endDate", endDate);
                myCommand.Parameters.AddWithValue("@userId", userId);
                myCommand.Parameters.AddWithValue("@taskId", rowid);
                myCommand.Prepare();
                myCommand.ExecuteNonQuery();
            }
            this.CloseConnection();
        }

        public void updateModel(Model model)
        {
            OpenConnection();

            using(myCommand = new SQLiteCommand(myConnection))
            {
                myCommand.CommandText = "UPDATE Models SET startDate = @startDate, name=@name WHERE rowid=@rowid";
                myCommand.Parameters.AddWithValue("@startDate", model.startDate);
                myCommand.Parameters.AddWithValue("@name", model.modelName);
                myCommand.Parameters.AddWithValue("@rowid", model.rowid);
                myCommand.Prepare();
                myCommand.ExecuteNonQuery();

                foreach(Task task in model.tasks)
                {
                    myCommand.CommandText = "UPDATE Tasks SET startDate=@startDate, endDate=@endDate, userAssignedId=@userAssignedId, userCompletedId=@userCompletedId WHERE rowid=@rowid";

                    if(task.completed)
                    {
                        myCommand.Parameters.AddWithValue("@startDate", task.startDate);
                        myCommand.Parameters.AddWithValue("@endDate", task.endDate);
                        myCommand.Parameters.AddWithValue("@userAssignedId", task.userAssignedId);
                    }
                    else
                    {
                        myCommand.Parameters.AddWithValue("@startDate", null);
                        myCommand.Parameters.AddWithValue("@endDate", null);
                        myCommand.Parameters.AddWithValue("@userCompletedId", task.userCompletedId);
                    }

                    myCommand.Parameters.AddWithValue("@rowid", task.rowid);
                    myCommand.Prepare();
                    myCommand.ExecuteNonQuery();
                }
            }
            CloseConnection();
        }

        public void updateUser(User user)
        {
            OpenConnection();
            using(myCommand = new SQLiteCommand(myConnection))
            {
                myCommand.CommandText = "UPDATE Users SET password = @password, requirePassword = @reqPass, cateogry=@category, WHERE rowid = @rowid";
                myCommand.Parameters.AddWithValue("@password", user.password);
                myCommand.Parameters.AddWithValue("@reqPass", user.reqPass);
                myCommand.Parameters.AddWithValue("@rowid", user.rowid);
                myCommand.Parameters.AddWithValue("@category", user.category);
                myCommand.Prepare();
                myCommand.ExecuteNonQuery();

                deleteAuthorizations(user);
                insertUserAuthorization(user);
            }
            CloseConnection();
        }

        private void deleteAuthorizations(User user)
        {
            myCommand.CommandText = "DELETE FROM Authorization WHERE userid = @userid";
            myCommand.Parameters.AddWithValue("@userid", user.rowid);
            myCommand.Prepare();
            myCommand.ExecuteNonQuery();
        }

        public void deleteModel(int modelId)
        {
            OpenConnection();
            using(myCommand = new SQLiteCommand(myConnection))
            {
                deleteModelFromModelDB(modelId);
                deleteModelFromTasksDB(modelId);
            }
            CloseConnection();

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
