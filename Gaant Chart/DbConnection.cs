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
                    "typeId INT NOT NULL, startDate DATETIME NOT NULL, endDate DATETIME, userId INT)";
                myCommand.ExecuteNonQuery();

                myCommand.CommandText = "CREATE TABLE IF NOT EXISTS Users (name TEXT NOT NULL UNIQUE, password TEXT, requirePassword INT)";
                myCommand.ExecuteNonQuery();

                myCommand.CommandText = "CREATE TABLE IF NOT EXISTS Authorization (userId INT NOT NULL, taskId INT NOT NULL)";
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
                    
                    myCommand.CommandText = "INSERT INTO Tasks(modelId, typeId, startDate, endDate, userId) VALUES (@ModelId, @typeId, @StartDate, @EndDate, @UserId)";
                    myCommand.Parameters.AddWithValue("@ModelId", modelId);
                    myCommand.Parameters.AddWithValue("@typeID", i);
                    myCommand.Parameters.AddWithValue("@StartDate", null);
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
                addTasks(model);
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

        private void addTasks(Model model)
        {
            addCompletedTasks(model);
            addNonCompletedTasks(model);

        }

        private void addCompletedTasks(Model model)
        {
            myCommand.CommandText = "SELECT tasks.rowid, tasks.typeId, tasks.startDate, tasks.endDate, users.name AS userName, users.password, users.requirePassword " +
                "FROM tasks INNER JOIN users ON tasks.userid = users.rowid "  +
                "WHERE modelId = @modelId";

            myCommand.Parameters.AddWithValue("@modelId", model.rowid);
            myCommand.Prepare();

            using (myDataReader = myCommand.ExecuteReader())
            {
                while(myDataReader.Read())
                {
                    int typeId = (int)myDataReader["typeId"];
                    DateTime startDate = (DateTime)myDataReader["startDate"];
                    DateTime endDate = (DateTime)myDataReader["endDate"];
                    String userName = (String)myDataReader["userName"];
                    String password = (String)myDataReader["password"];
                    int reqPassInt = (int)myDataReader["requirePassword"];
                    long rowid = (long)myDataReader["rowid"];

                    Boolean reqPass = (reqPassInt == 1) ? true : false;

                    User user = new User(userName, password, reqPass);
                    TaskCompletor taskCompletor = new TaskCompletor(typeId, user, startDate, endDate);

                    model.completeTask(taskCompletor);
                    model.tasks[typeId].rowid = rowid;
                }
            }
        }

        private void addNonCompletedTasks(Model model)
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
            this.CloseConnection();

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

        public void completeTask(long taskId, DateTime endDate)
        {
            this.OpenConnection();

            User user = data.currentUser;
            int userId = user.rowid;

            using (myCommand = new SQLiteCommand(myConnection))
            {
                myCommand.CommandText = "UPDATE tasks SET endDate = @endDate, userId = @userId WHERE rowid = @taskId";
                myCommand.Parameters.AddWithValue("@endDate", endDate);
                myCommand.Parameters.AddWithValue("@userId", userId);
                myCommand.Parameters.AddWithValue("@taskId", taskId);
                myCommand.Prepare();
                myCommand.ExecuteNonQuery();
            }
            this.CloseConnection();
        }

        public void deleteModel(int modelId)
        {
            this.OpenConnection();
            using(myCommand = new SQLiteCommand(myConnection))
            {
                deleteModelFromModelDB(modelId);
                deleteModelFromTasksDB(modelId);
            }
            this.CloseConnection();

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
