using Gaant_Chart.Models;
using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Shapes;

namespace Gaant_Chart
{
    public class data
    {

        // taskSettings specifies the priority and expected duration of each task
        // Note: the task plan is constant accross all models

        public static Model currentModel { get; set; }
        public static User currentUser { get; set; }

        public static string[] allTasks = {
            "Image Aquisition",
            "Images Download",
            "SMP8 Autosegmentation",
            "MD Volumes Segmented",
            "Fully Segmented Model",
            "Initial Peer Review",
            "Segmentation Corrections",
            "2nd Peer Review",
            "2nd Segmentation Corrections",
            "Full Model Approved",
            "Meshed Model",
            "Export Model to Physics",
            "Model Solved",
            "Report Generated"
        };

        public static List<(String, int)> taskLabelGroups = new List<(String, int)>
        {
            ("Assemblies", 0),
            ("Segmentation Tasks", 1),
            ("Segmentation Review and Approval", 7),                                                                        
            ("Mesh Preperation and Export to Physics", 13),
            ("Physics Modeling and Reports", 16)
        };


        public static Dictionary<String, Boolean[]> categoryAuthorization = new Dictionary<String, Boolean[]>()
        {
            {"Phys", new bool[] {true, true, true, true, true, true, true, true, true, true, true, true, true}},
            {"MD",   new bool[]   {true, true, true, true, true, true, true, true, true, true, true, true, true}},
            {"Seg2", new bool[] {true, true, true, true, true, true, true, true, true, true, true, true, true}},
            {"Seg1", new bool[] {true, true, true, true, true, true, true, true, true, true, true, true, true}}
        };

        public static Dictionary<long, User> users { get; set; }

        public static int[] taskSettingsDuration =
        {
           1,  // "Image Aquisition",
           1,  // "Images Download",
           1,  // "SMP8 Autosegmentation",
           2,  // "MD Volumes Segmented",
           3,  // "Fully Segmented Model",
           2,  // "Initial Peer Review",
           1,  // "Segmentation Corrections",
           1,  // "2nd Peer Review",
           1,  // "2nd Segmentation Corrections",
           1,  // "Full Model Approved",
           1,  // "Meshed Model",
           1,  // "Export Model to Physics",
           1,  // "Model Solved",
           1,  // "Report Generated"
        };

        public static List<Rectangle> plannedTaskBlocks { get; set; }
        public static List<Rectangle> completedTaskBlocks { get; set; }

        public static void initUsers()
        {
            users = MainWindow.myDatabase.getUsers();
        }

        public static User getUser(int userId)
        {
            checkUsersInitialied();
            return users[userId];
        }

        public static User getUser(String name)
        {
            checkUsersInitialied();
            User user = null;
            foreach(KeyValuePair<long, User> pair in users)
            {
                User tempUser = pair.Value;
                if(tempUser.name == name)
                {
                    user = tempUser;
                }
            }
            return user;
        }

        public static void addUser(User user)
        {
            if(user.rowid == -1)
            {
                MainWindow.myDatabase.insertUser(user);
            }

            users.Add(user.rowid, user);
        }

        private static void checkUsersInitialied()
        {
            if (users == null) throw new Exception("Calling getUser before users is initialized");
        }
        
    }
}
