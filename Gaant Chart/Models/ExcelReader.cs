using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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
                IWorksheet nameWs = workbook.Worksheets[1];

                String modelName = mainWs.Range["B2"].Text;
                String modelStartDateString = mainWs.Range["B4"].Text;

                DateTime modelStartDate = stringToDate(modelStartDateString);

                extractAndAddUsers(nameWs);

                if(MainWindow.myDatabase.getModelId(modelName) != -1)
                {
                    model = MainWindow.myDatabase.getModel(modelName);
                }
                else
                {
                    model = MainWindow.myDatabase.insertModel(modelName, modelStartDate);
                    addCompletedTasks();
                    MainWindow.myDatabase.updateModel(model);
                }
            }
        }

        private void addCompletedTasks()
        {
            for(int i = 0; i < data.allTasks.Length; i++)
            {
                int row = taskExcelRows[i];
                String userInitals = mainWs.Range[row, 3].Text;
                String plannedStartString = mainWs.Range[row, 5].Text;
                String plannedEndString = mainWs.Range[row, 6].Text;
                String startDateString = mainWs.Range[row, 8].Text;
                String endDateString = mainWs.Range[row, 9].Text;

                User user = initalsUserDict[userInitals];

                DateTime plannedStartDate, plannedEndDate, startDate, endDate;
                if (!DateTime.TryParse(plannedStartString, out plannedStartDate)) invalidDateError();
                if (!DateTime.TryParse(plannedEndString, out plannedEndDate)) invalidDateError();

                if(!String.IsNullOrEmpty(startDateString) && !String.IsNullOrEmpty(endDateString))
                {
                    if (!DateTime.TryParse(startDateString, out startDate)) invalidDateError();
                    if(!DateTime.TryParse(endDateString, out endDate)) invalidDateError();

                    model.tasks[i].complete(user.rowid, startDate, endDate);
                }
            }
        }

        private void invalidDateError()
        {
            throw new Exception("Invalid Date");
        }

        private void extractAndAddUsers(IWorksheet nameWs)
        {
            int r = 2;

            initalsUserDict = new Dictionary<String, User>();

            String name = nameWs.Range[r, 1].Text;

            List<User> users = new List<User>();

            while(!String.IsNullOrEmpty(name))
            {
                User user = data.getUser(name);
                String initals = nameWs.Range[r, 2].Text;
                String category = nameWs.Range[r, 3].Text;
                String Status = nameWs.Range[r, 4].Text;

                if(user == null)
                {
                    user = new User(name, "", false);
                    user.authorize(data.categoryAuthorization[category]);
                    users.Add(user);
                }
                else
                {
                    user = data.getUser(name);
                }

                initalsUserDict.Add(initals, user);
            }

            foreach(User user in users)
            {
                data.addUser(user);
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
