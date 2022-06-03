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

        // Tasks that are completed
        public static Dictionary<String, (String, String, String)> completedTasks { get; set; }
        public static Dictionary<String, int> taskStartDelayPlanned { get; set; }

        private static List<User> users { get; set; }

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
            if (users == null) throw new Exception("Calling getUser before users is initialized");
            return users[userId];
        }
        
    }
}
