using System;
using System.Collections.Generic;
using System.Linq;
using Syncfusion.XlsIO;

namespace Gaant_Chart.Models
{
    public class ExcelReader
    {
        public int[] taskExcelRows =
        {
            9,
            10,
            11,
            12,
            13,
            15,
            16,
            17,
            18,
            19,
            21,
            22,
            24,
            25
        };


        private IWorksheet mainWs;
        private IWorksheet nameWs;

        private IApplication application;

        private String pathname;
        public Model model { get;  }
        private Dictionary<String, User> initalsUserDict;
        private Task[] tasks;
        private int lastCompletedTaskId;

        public ExcelReader(String pathname)
        {
            this.pathname = pathname;

            initalsUserDict = new Dictionary<String, User>();
            tasks = new Task[data.NTASKS];

            using (ExcelEngine excelEngine = new ExcelEngine())
            {
                application = excelEngine.Excel;
                application.DefaultVersion = ExcelVersion.Xlsx;
                IWorkbook workbook = application.Workbooks.Open(pathname);

                mainWs = workbook.Worksheets[0];
                mainWs.EnableSheetCalculations();

                nameWs = workbook.Worksheets[1];

                String modelName = mainWs.Range["B2"].Value;
                DateTime modelStartDate = mainWs.Range["B4"].DateTime;

                setInitialsUserDict();
                setTasks();

                model = new Model(modelName, modelStartDate, tasks, lastCompletedTaskId);
            }
        }
        private void setTasks()
        {
            int MINUTES_IN_DAY = 1439;
            lastCompletedTaskId = 0;

            Func<DateTime, DateTime> toStartOfDay = date => date.AddMinutes(-date.TimeOfDay.TotalMinutes);
            Func<DateTime, DateTime> toEndOfDay = date => date.AddMinutes(MINUTES_IN_DAY - date.TimeOfDay.TotalMinutes);

            for(int i = 0; i < data.allTasks.Length; i++)
            {

                int row = taskExcelRows[i];
                String userInitals = mainWs.Range[row, 3].Text;
                DateTime plannedStart = mainWs.Range[row, 5].DateTime;
                DateTime plannedEnd = mainWs.Range[row, 6].DateTime;

                plannedStart = toStartOfDay(plannedStart);
                plannedEnd = toEndOfDay(plannedEnd);

                DateTime startDate = DateTime.MinValue;
                DateTime endDate = DateTime.MinValue;

                Boolean hasStartDate = !mainWs.Range[row, 8].IsBlank;
                Boolean hasEndDate = !mainWs.Range[row, 9].IsBlank;

                if (hasStartDate)
                {
                    startDate = mainWs.Range[row, 8].DateTime;
                    startDate = toStartOfDay(startDate);
                }

                if (hasEndDate)
                {
                    endDate = mainWs.Range[row, 9].DateTime;
                    endDate = toEndOfDay(endDate);
                }

                User user = initalsUserDict[userInitals];

                Task task = new Task(i, plannedStart, plannedEnd);
                task.assign(user);

                if (hasStartDate && hasEndDate)
                {
                    task.complete(user, startDate, endDate);
                    lastCompletedTaskId = i;
                }
                tasks[i] = task;
            }
        }
        private void setInitialsUserDict()
        {
            int r = 2;

            String name = nameWs.Range[r, 1].Value;

            while(!String.IsNullOrEmpty(name))
            {
                String initals = nameWs.Range[r, 2].Value;
                String category = nameWs.Range[r, 3].Value;
                String status = nameWs.Range[r, 4].Value;
                Boolean active = (status == "Active");

                User user = data.getUser(name);

                if (user == null)
                {
                    user = new User(name, initals, category, active);

                    if (user.initials != null && MainWindow.myDatabase.isUserInitialsExist(user))
                    {
                        user.initials = String.Concat(name.Split(' ').Select(s => s[0]));
                        user.initials += user.rowid.ToString();
                    }

                    MainWindow.myDatabase.insertUser(user);
                    data.users.Add(user.rowid, user);
                }

                initalsUserDict.Add(initals, user);

                r++;
                name = nameWs.Range[r, 1].Value;
            }
        }

        public User[] getUsers()
        {
            User[] users = new User[initalsUserDict.Count];
            int i = 0;
            foreach(var kvp in initalsUserDict)
            {
                users[i] = kvp.Value;
                i++;
            }
            return users;

        }
    }
}
