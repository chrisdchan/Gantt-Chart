using Gaant_Chart.Models;
using Gaant_Chart.Structures;
using System;
using System.Collections.Generic;
using System.Windows.Shapes;

namespace Gaant_Chart
{
    public class data
    {

        // taskSettings specifies the priority and expected duration of each task
        // Note: the task plan is constant accross all models

        public static Model currentModel { get; set; }
        public static User currentUser { get; set; }

        public static int NTASKS = 14;

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

        public static int[] taskGridRow =
        {
            2,
            4,
            6,
            8,
            10,
            13,
            15,
            17,
            19,
            21,
            24,
            26,
            29,
            31
        };

        public static List<(String, int)> taskLabelGroups = new List<(String, int)>
        {
            ("Assemblies", 0),
            ("Segmentation Tasks", 1),
            ("Segmentation Review and Approval", 7),                                                                        
            ("Mesh Preperation and Export to Physics", 13),
            ("Physics Modeling and Reports", 16)
        };

        public static (String, int, int)[] taskLabelGroupPosition = new (String name, int index, int length)[]
        {
            ("Segmentation Tasks", 0, 5),
            ("Segmentation Review and Approval", 5, 5),
            ("Mesh Preperation and Export to Physics", 10, 2),
            ("Physics Modeling and Reports", 12, 2)
        };

        public static Dictionary<String, Boolean[]> categoryAuthorization = new Dictionary<String, Boolean[]>()
        {
            {"Phys", new bool[] {true, true, true, true, true, true, true, true, true, true, true, true, true, true}},
            {"MD",   new bool[]   {true, true, true, true, true, true, true, true, true, true, true, true, true, true}},
            {"Seg2", new bool[] {true, true, true, false, true, true, true, true, true, true, true, true, false, true}},
            {"Seg1", new bool[] {true, true, true, true, true, true, true, true, true, true, true, true, false, true}}
        };

        public static Dictionary<long, User> users { get; set; }
        public static Trie userTrie { get; set; }

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
            userTrie = new Trie();
            foreach(var kvp in users) userTrie.insert(kvp.Value.name.ToLower(), kvp.Key);
        }

        public static void addUser(User user)
        {
            userTrie.insert(user.name.ToLower(), user.rowid);
            users.Add(user.rowid, user);
        }
        public static User getUser(String name)
        {
            checkUsersInitialied();
            User user = null;
            foreach(var kvp in users)
            {
                if (kvp.Value.name == name)
                    user = kvp.Value;
            }
            return user;
        }

        private static void checkUsersInitialied()
        {
            if (users == null) throw new Exception("Calling getUser before users is initialized");
        }

        public static Boolean checkSimilarInitialsExist(string initials)
        {
            foreach(var kvp in users)
            {
                User user = kvp.Value;
                if(user.initials.ToLower() == initials.ToLower())
                {
                    return true;
                }
            }
            return false;
        }


        
    }
}
