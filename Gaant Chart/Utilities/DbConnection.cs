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
                myCommand.CommandText = "CREATE TABLE IF NOT EXISTS Models (" +
                    "name TEXT NOT NULL UNIQUE, " +
                    "startDate DATETIME NOT NULL, " +
                    "endDate DATETIME, " +
                    "lastUpdated DATETIME NOT NULL)";
                myCommand.ExecuteNonQuery();

                myCommand.CommandText = "CREATE TABLE IF NOT EXISTS Tasks (" +
                    "modelId INT NOT NULL CHECK (modelId > 0), " +
                    "typeId INT NOT NULL, " +
                    "plannedStartDate DATETIME, " +
                    "plannedEndDate DATETIME, " +
                    "startDate DATETIME, " +
                    "endDate DATETIME, " +
                    "userAssignedId INT CHECK (userAssignedId > 0), " +
                    "userCompletedId INT CHECK (userCompletedId > 0) )";
                myCommand.ExecuteNonQuery();

                myCommand.CommandText = "CREATE TABLE IF NOT EXISTS Users (" +
                    "name TEXT NOT NULL UNIQUE, " +
                    "initials TEXT UNIQUE, " +
                    "password TEXT, " +
                    "requirePassword INT, " +
                    "active INT, " +
                    "category TEXT)";

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
        public void insertModel(Model model)
        {
            OpenConnection();
            using (SQLiteCommand myCommand = new SQLiteCommand(myConnection))
            {
                myCommand.CommandText = "INSERT INTO Models(name, startDate, endDate, lastUpdated) VALUES (@name, @startDate, @endDate, @lastUpdated)";
                myCommand.Parameters.AddWithValue("@Name", model.modelName);
                myCommand.Parameters.AddWithValue("@startDate", model.startDate);
                myCommand.Parameters.AddWithValue("@EndDate", model.endDate);
                myCommand.Parameters.AddWithValue("@lastUpdated", model.lastUpdated);
                myCommand.Prepare();
                myCommand.ExecuteNonQuery();

                myCommand.CommandText = "SELECT last_insert_rowid() FROM Models";
                model.rowid = (long)myCommand.ExecuteScalar();

                foreach(Task task in model.tasks)
                {
                    long? assignedUserId = (task.assignedUser == null) ? null : (long?)task.assignedUser.rowid;
                    long? completedUserId = (task.completedUser == null) ? null : (long?)task.completedUser.rowid;


                    myCommand.CommandText = "INSERT INTO Tasks(modelId, typeId, " +
                        "plannedStartDate, plannedEndDate, startDate, endDate, userAssignedId, userCompletedId) " +
                        "VALUES (@modelId, @typeId, @plannedStartDate, @plannedEndDate, " +
                        "@startDate, @endDate, @userAssignedId, @userCompletedId)";
                    myCommand.Parameters.AddWithValue("@modelId", model.rowid);
                    myCommand.Parameters.AddWithValue("@typeId", task.typeInd);
                    myCommand.Parameters.AddWithValue("@startDate", task.startDate);
                    myCommand.Parameters.AddWithValue("@plannedStartDate", task.plannedStartDate);
                    myCommand.Parameters.AddWithValue("@endDate", task.endDate);
                    myCommand.Parameters.AddWithValue("@plannedEndDate", task.plannedEndDate);
                    myCommand.Parameters.AddWithValue("@userAssignedId", assignedUserId);
                    myCommand.Parameters.AddWithValue("@userCompletedId", completedUserId);
                    myCommand.Prepare();
                    myCommand.ExecuteNonQuery();

                    myCommand.CommandText = "SELECT last_insert_rowid() FROM Models";
                    task.rowid = (long)myCommand.ExecuteScalar();
                }
            }
            CloseConnection();
        }
        public void insertUser(User user)
        {
            OpenConnection();

            int reqPass = (user.reqPass) ? 1 : 0;
            int active = (user.active) ? 1 : 0;

            using(myCommand = new SQLiteCommand(myConnection))
            {
                if(user.category == null)
                {
                    myCommand.CommandText = "INSERT INTO USERS(name, initials, password, requirePassword, active) " +
                        "VALUES (@name, @initials, @password, @requirePassword, @active)";
                }
                else
                {
                    myCommand.CommandText = "INSERT INTO USERS(name, initials, password, requirePassword, active, category) " +
                        "VALUES (@name, @initials, @password, @requirePassword, @active, @category)";
                    myCommand.Parameters.AddWithValue("@category", user.category);
                }

                myCommand.Parameters.AddWithValue("@name", user.name);
                myCommand.Parameters.AddWithValue("@initials", user.initials);
                myCommand.Parameters.AddWithValue("@password", user.password);
                myCommand.Parameters.AddWithValue("@requirePassword", reqPass);
                myCommand.Parameters.AddWithValue("@active", active);
                myCommand.Prepare();
                myCommand.ExecuteNonQuery();

                myCommand.CommandText = "SELECT last_insert_rowid() FROM USERS";
                long rowid = (long)myCommand.ExecuteScalar();

                user.rowid = rowid;

                insertUserAuthorization(user);

            }
            CloseConnection();
        }
        public Boolean isUserExist(User user)
        {
            OpenConnection();

            Boolean isUserInDB;

            using(myCommand = new SQLiteCommand(myConnection))
            {
                myCommand.CommandText = "SELECT rowid FROM Users WHERE name = @name";
                myCommand.Parameters.AddWithValue("@name", user.name);
                myCommand.Prepare();
                using(myDataReader = myCommand.ExecuteReader())
                {
                    isUserInDB = myDataReader.Read();
                }
            }

            CloseConnection();

            return isUserInDB;

        }
        public Boolean isUserInitialsExist(User user)
        {
            OpenConnection();

            Boolean isInitialsExist;

            using(myCommand = new SQLiteCommand(myConnection))
            {
                myCommand.CommandText = "SELECT rowid FROM Users WHERE initials = @initials";
                myCommand.Parameters.AddWithValue("@initials", user.initials);
                myCommand.Prepare();
                using(myDataReader = myCommand.ExecuteReader())
                {
                    isInitialsExist = myDataReader.Read();
                }
            }

            CloseConnection();

            return isInitialsExist;
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
        public int findModelId(String modelName)
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
                myCommand.CommandText = "SELECT rowid, startDate, endDate, lastUpdated FROM Models where name = @name";
                myCommand.Parameters.AddWithValue("@name", modelName);
                myCommand.Prepare();

                long modelId;
                DateTime startDate;
                DateTime lastUpdated;
                DateTime? endDate = null;
                using (myDataReader = myCommand.ExecuteReader())
                {

                    if (myDataReader.Read())
                    {
                        modelId = (long)myDataReader["rowid"];
                        startDate = (DateTime)myDataReader["startDate"];
                        lastUpdated = (DateTime)myDataReader["lastUpdated"];

                        if(!myDataReader.IsDBNull(3))
                        {
                            endDate = (DateTime)myDataReader["endDate"];
                        }
                    }
                    else
                    {
                        throw new Exception("Cannot retrieve model query");
                    }
                }

                (Task[] tasks, int lastCompleted) = getTasks(modelId);
                model = new Model(modelId, modelName, startDate, endDate, lastUpdated, tasks, lastCompleted);

            }
            CloseConnection();
            return model;
        }
        public Model getModel(long modelId)
        {
            Model model;
            OpenConnection();
            using(myCommand = new SQLiteCommand(myConnection))
            {
                myCommand.CommandText = "SELECT name, startDate, endDate, lastUpdated FROM Models where rowid = @rowid";
                myCommand.Parameters.AddWithValue("@rowid", modelId);
                myCommand.Prepare();

                String modelName;
                DateTime startDate;
                DateTime lastUpdated;
                DateTime? endDate = null;
                using (myDataReader = myCommand.ExecuteReader())
                {

                    if (myDataReader.Read())
                    {
                        modelName = (String)myDataReader["name"];
                        startDate = (DateTime)myDataReader["startDate"];
                        lastUpdated = (DateTime)myDataReader["lastUpdated"];

                        if(!myDataReader.IsDBNull(2))
                        {
                            endDate = (DateTime)myDataReader["endDate"];
                        }
                    }
                    else
                    {
                        throw new Exception("Cannot retrieve model query");
                    }
                }

                (Task[] tasks, int lastCompleted) = getTasks(modelId);
                model = new Model(modelId, modelName, startDate, endDate, lastUpdated, tasks, lastCompleted);
            }

            CloseConnection();
            return model;
        }
        private (Task[], int) getTasks(long modelId)
        {
            Task[] tasks = new Task[data.NTASKS];
            int lastCompleted = 0;

            myCommand.CommandText = "SELECT rowid, typeId, " +
                    "plannedStartDate, plannedEndDate, " +
                    "startDate, endDate, " +
                    "userAssignedId, userCompletedId " +
                    "FROM tasks WHERE modelId = @modelId";

            myCommand.Parameters.AddWithValue("@modelId", modelId);

            using(myDataReader = myCommand.ExecuteReader())
            {
                int i = 0;
                while(myDataReader.Read())
                {
                    int typeId = (int)myDataReader["typeId"];
                    long rowid = (long)myDataReader["rowid"];
                    DateTime plannedStartDate = (DateTime)myDataReader["plannedStartDate"];
                    DateTime plannedEndDate = (DateTime)myDataReader["plannedEndDate"];

                    DateTime? startDate = null;
                    DateTime? endDate = null;

                    if(!myDataReader.IsDBNull(4)) 
                        startDate = myDataReader.GetDateTime(4);

                    if(!myDataReader.IsDBNull(5))
                        endDate = myDataReader.GetDateTime(5);

                    User assignedUser = null;
                    User completedUser = null;

                    if(!myDataReader.IsDBNull(6))
                    {
                        int userAssignedId = (int)myDataReader["userAssignedId"];
                        assignedUser = data.users[userAssignedId];
                    }

                    if (!myDataReader.IsDBNull(7))
                    {
                        int userCompletedId = (int)myDataReader["userCompletedId"];
                        completedUser = data.users[userCompletedId];
                        lastCompleted = i;
                    }

                    Task task = new Task(rowid, typeId,
                        plannedStartDate, plannedEndDate,
                        startDate, endDate,
                        assignedUser, completedUser);

                    tasks[i] = task;
                    i++;
                }
            }
            return (tasks, lastCompleted);
        }
        private Model fillUncompleteModelWithTasks(Model model)
        {
            using(myCommand = new SQLiteCommand(myConnection))
            {
                myCommand.CommandText = "SELECT rowid, typeId, " +
                    "plannedStartDate, plannedEndDate," +
                    "userAssignedId FROM tasks WHERE modelId = @modelId";
                myCommand.Parameters.AddWithValue("@modelId", model.rowid);
                myCommand.Prepare();

                using(myDataReader = myCommand.ExecuteReader())
                {
                    while(myDataReader.Read())
                    {
                        int typeId = (int)myDataReader["typeId"];
                        long rowid = (long)myDataReader["rowid"];
                        DateTime plannedStartDate = (DateTime)myDataReader["plannedStartDate"];
                        DateTime plannedEndDate = (DateTime)myDataReader["plannedEndDate"];
                        Task task = model.tasks[typeId];

                        if(myDataReader["userAssignedId"].GetType() != typeof(DBNull))
                        {
                            int assignedUserId = (int)myDataReader["userAssignedId"];
                            User assignedUser = data.users[assignedUserId];
                            task.assignedUser = assignedUser;
                        }
                        task.rowid = rowid;
                        task.plannedStartDate = plannedStartDate;
                        task.plannedEndDate = plannedEndDate;
                    }
                }
            }

            using(myCommand = new SQLiteCommand(myConnection))
            {
                myCommand.CommandText = "SELECT typeId, startDate, endDate, userCompletedId, userAssignedId FROM tasks WHERE modelId = @modelId AND userCompletedId IS NOT NULL";
                myCommand.Parameters.AddWithValue("@modelId", model.rowid);
                myCommand.Prepare();

                using (myDataReader = myCommand.ExecuteReader())
                {
                    while(myDataReader.Read())
                    {
                        int typeId = (int)myDataReader["typeId"];
                        DateTime startDate = (DateTime)myDataReader["startDate"];
                        DateTime endDate = (DateTime)myDataReader["endDate"];
                        long? completedUserId = (long?)myDataReader["userCompletedId"];


                        if(completedUserId != null)
                        {
                            User user = data.users[(long)completedUserId];
                            model.completeTask(user, typeId, startDate, endDate);
                        }

                    }
                }
            }

            return model;
        }
        public  Dictionary<long, User> getUsers()
        {
            Dictionary<long, User> users = new Dictionary<long, User>();

            OpenConnection();

            using(myCommand = new SQLiteCommand(myConnection))
            {
                myCommand.CommandText = "SELECT rowid, name, initials, password, requirePassword, category, active FROM Users";
                

                using(myDataReader = myCommand.ExecuteReader())
                {
                    while(myDataReader.Read())
                    {
                        long rowid = (long)myDataReader["rowid"];
                        String name = (String) myDataReader["name"];
                        String initials = (String)myDataReader["initials"];
                        String password = (String)myDataReader["password"];
                        String category = (myDataReader["category"].GetType() == typeof(DBNull)) ? null : (String)myDataReader["category"];
                        int activeInt = (int)myDataReader["active"];
                        Boolean active = activeInt == 1;

                        int reqPassInt = (int) myDataReader["requirePassword"];
                        Boolean reqPass = (reqPassInt != 0);

                        User user = new User(rowid, name, initials, password, reqPass, category, active);
                        users.Add(rowid, user);
                    }
                }

                foreach(var kvp in users)
                {
                    Boolean[] authorization = getAuthorization(kvp.Key);
                    kvp.Value.authorize(authorization);
                }
            }

            CloseConnection();

            return users;
        }
        private Boolean[] getAuthorization(long userid)
        {
            Boolean[] authorization = new Boolean[data.allTasks.Length];

            myCommand.CommandText = "SELECT taskTypeId FROM Authorization WHERE userId = @userid";
            myCommand.Parameters.AddWithValue("@userid", userid);
            myCommand.Prepare();
            using (myDataReader = myCommand.ExecuteReader())
            {
                while (myDataReader.Read())
                {
                    int taskTypeId = (int)myDataReader["tasktypeId"];
                    authorization[taskTypeId] = true;
                }
            }
            return authorization;
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
                myCommand.CommandText = "UPDATE tasks SET startDate = @startDate, endDate = @endDate, " +
                    "userCompletedId=@userCompletedId WHERE rowid = @taskId";
                myCommand.Parameters.AddWithValue("@startDate", startDate);
                myCommand.Parameters.AddWithValue("@endDate", endDate);
                myCommand.Parameters.AddWithValue("@userCompletedId", userId);
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
                myCommand.CommandText = "UPDATE Models SET startDate = @startDate, endDate=@endDate, name=@name, lastUpdated=@lastUpdated WHERE rowid=@rowid";
                myCommand.Parameters.AddWithValue("@startDate", model.startDate);
                myCommand.Parameters.AddWithValue("@endDate", model.endDate);
                myCommand.Parameters.AddWithValue("@name", model.modelName);
                myCommand.Parameters.AddWithValue("@rowid", model.rowid);
                myCommand.Parameters.AddWithValue("@lastUpdated", model.lastUpdated);
                myCommand.Prepare();
                myCommand.ExecuteNonQuery();

                foreach(Task task in model.tasks)
                {
                    DateTime? startDate = (task.startDate != null) ? task.startDate : null;
                    DateTime? endDate = (task.endDate != null) ? task.endDate : null;

                    myCommand.CommandText = "UPDATE Tasks SET startDate=@startDate, " +
                        "endDate=@endDate, userAssignedId=@userAssignedId, " +
                        "userCompletedId=@userCompletedId, " +
                        "plannedStartDate=@plannedStartDate, plannedEndDate=@plannedEndDate " + 
                        "WHERE rowid=@rowid";
                    myCommand.Parameters.AddWithValue("@plannedStartDate", task.plannedStartDate);
                    myCommand.Parameters.AddWithValue("@plannedEndDate", task.plannedEndDate);
                    myCommand.Parameters.AddWithValue("@startDate", startDate);
                    myCommand.Parameters.AddWithValue("@endDate", endDate);

                    if (task.completedUser != null)
                        myCommand.Parameters.AddWithValue("@userCompletedId", task.completedUser.rowid);
                    else
                        myCommand.Parameters.AddWithValue("@userCompletedId", null);

                    if (task.assignedUser != null)
                        myCommand.Parameters.AddWithValue("@userAssignedId", task.assignedUser.rowid);
                    else
                        myCommand.Parameters.AddWithValue("@userAssignedId", null);

                    myCommand.Parameters.AddWithValue("@rowid", task.rowid);
                    myCommand.Prepare();
                    int rowsAffected = myCommand.ExecuteNonQuery();
                }
            }
            CloseConnection();
        }
        public void updateUser(User user)
        {
            OpenConnection();

            int reqPass = (user.reqPass) ? 1 : 0;
            int active = (user.active) ? 1 : 0;

            using(myCommand = new SQLiteCommand(myConnection))
            {
                myCommand.CommandText = "UPDATE Users SET " +
                    "password = @password, " +
                    "requirePassword = @reqPass, " +
                    "active = @active, " + 
                    "category = @category " +
                    "WHERE rowid = @rowid";

                myCommand.Parameters.AddWithValue("@password", user.password);
                myCommand.Parameters.AddWithValue("@reqPass", reqPass);
                myCommand.Parameters.AddWithValue("@active", active);
                myCommand.Parameters.AddWithValue("@rowid", active);
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
        public void deleteModel(long modelId)
        {
            OpenConnection();
            using(myCommand = new SQLiteCommand(myConnection))
            {
                deleteModelFromModelDB(modelId);
                deleteModelFromTasksDB(modelId);
            }
            CloseConnection();

        }
        private void deleteModelFromModelDB(long modelId)
        {
            myCommand.CommandText = "DELETE FROM Models WHERE rowid = @rowid";
            myCommand.Parameters.AddWithValue("@rowid", modelId);
            myCommand.Prepare();
            myCommand.ExecuteNonQuery();
        }
        private void deleteModelFromTasksDB(long modelId)
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
