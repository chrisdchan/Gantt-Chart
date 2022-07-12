using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using Syncfusion.XlsIO;

namespace Gaant_Chart.Models
{
    public class ExcelReader
    {
        private int[] taskExcelRows =
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

        private Boolean useExistingUsers = true;

        private Boolean existingUserError = false;
        private Dictionary<String, User> initalsUserDict;

        private IWorksheet mainWs;

        private Model model;


        public ExcelReader(String pathname)
        {
            using (ExcelEngine excelEngine = new ExcelEngine())
            {
                IApplication application = excelEngine.Excel;

                application.DefaultVersion = ExcelVersion.Xlsx;

                IWorkbook workbook = application.Workbooks.Open(pathname);

                mainWs = workbook.Worksheets[0];
                mainWs.EnableSheetCalculations();
                IWorksheet nameWs = workbook.Worksheets[1];

                String modelName = mainWs.Range["B2"].Value;
                String modelStartDateString = mainWs.Range["B4"].Value;

                DateTime modelStartDate = stringToDate(modelStartDateString);

                extractAndAddUsers(nameWs);

                if(MainWindow.myDatabase.getModelId(modelName) != -1)
                {
                    model = MainWindow.myDatabase.getModel(modelName);
                }
                else
                {
                    Task[] tasks = getTasks();
                    model = new Model(modelName, modelStartDate, tasks);
                    MainWindow.myDatabase.insertModel(model);
                }
            }
        }

        public Model getModel()
        {
            return model;
        }

        private Task[] getTasks()
        {
            Task[] tasks = new Task[data.NTASKS];

            for(int i = 0; i < data.allTasks.Length; i++)
            {

                int row = taskExcelRows[i];
                String userInitals = mainWs.Range[row, 3].Text;
                DateTime plannedStart = mainWs.Range[row, 5].DateTime;
                DateTime plannedEnd = mainWs.Range[row, 6].DateTime;

                DateTime startDate = DateTime.MinValue;
                DateTime endDate = DateTime.MinValue;

                Boolean hasStartDate = !mainWs.Range[row, 8].IsBlank;
                Boolean hasEndDate = !mainWs.Range[row, 9].IsBlank;

                if (hasStartDate)
                    startDate = mainWs.Range[row, 8].DateTime;

                if (hasEndDate)
                    endDate = mainWs.Range[row, 9].DateTime;

                User user = initalsUserDict[userInitals];

                Task task = new Task(i, plannedStart, plannedEnd);
                task.assign(user);

                if (hasStartDate && hasEndDate)
                    task.complete(user, startDate, endDate);

                tasks[i] = task;
            }

            return tasks;
        }

        private void invalidDateError()
        {
            throw new Exception("Invalid Date");
        }

        private void extractAndAddUsers(IWorksheet nameWs)
        {
            int r = 2;

            initalsUserDict = new Dictionary<String, User>();

            String name = nameWs.Range[r, 1].Value;

            List<User> users = new List<User>();

            while(!String.IsNullOrEmpty(name))
            {
                User user = data.getUser(name);
                String initals = nameWs.Range[r, 2].Value;
                String category = nameWs.Range[r, 3].Value;
                String status = nameWs.Range[r, 4].Value;

                Boolean active = (status == "active");

                if(user == null)
                {
                    user = new User(name, initals, category, active);

                    if(user.initials != null && MainWindow.myDatabase.isUserInitialsExist(user))
                    {
                        user.initials = String.Concat(name.Split(' ').Select(s => s[0]));
                        user.initials += user.rowid.ToString();
                    }

                    MainWindow.myDatabase.insertUser(user);
                    users.Add(user);
                }
                else
                {
                    user = data.getUser(name);
                }

                if(!initalsUserDict.ContainsKey(initals))
                    initalsUserDict.Add(initals, user);

                r++;
                name = nameWs.Range[r, 1].Value;
            }
        }

        private DateTime stringToDate(String str)
        {
            DateTime date;
            if (DateTime.TryParse(str, out date))
            {
                return date;
            }
            else
            {
                throw new Exception("Invalid date");
            }
        }
    }
}
