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
        public ExcelReader(String pathname)
        {
            using (ExcelEngine excelEngine = new ExcelEngine())
            {
                IApplication application = excelEngine.Excel;

                application.DefaultVersion = ExcelVersion.Xlsx;

                IWorkbook workbook = application.Workbooks.Open(pathname);

                IWorksheet mainWs = workbook.Worksheets[0];
                IWorksheet nameWs = workbook.Worksheets[1];

                String modelName = mainWs.Range["B2"].Text;
                String modelStartDateString = mainWs.Range["B4"].Text;

                DateTime modelStartDate = stringToDate(modelStartDateString);

                extractAndAddUsers(nameWs);

            }
        }
        public void generateModel()
        { 
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
                    if(!user.authorization.SequenceEqual(data.categoryAuthorization[category]))
                    {
                        user = data.getUser(name);
                    }
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
