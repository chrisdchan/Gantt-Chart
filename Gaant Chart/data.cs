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

        public static String modelName { get; set; }
        public static int ModelId { get; set; }
        public static DateTime startDate { get; set; }

        public static ImmutableArray<String> ALLTASKS = new ImmutableArray<String> {
            "Image Aquisition",
            "Images Download",
            "SMP8 Autosegmentation",
            "MD Vol"

        };

        // Tasks that are completed
        public static Dictionary<String, (String, String, String)> completedTasks { get; set; }
        public static Dictionary<String, int> taskStartDelayPlanned { get; set; }

        public static Dictionary<String, (int, List<String>)> taskSettings = new Dictionary<String, (int duration, List<String>)>
            {
                {"Image Aquisition", ( 1, null) },
                {"Images Download", ( 1, new List<String>{"Image Aquisition" }) },
                {"SPM8 Autosegmentation", (1, new List<String>{"Images Download"}) }, 
                {"MD Volumes Segmented", ( 4, new List<String>{"SPM8 Autosegmentation" }) },
                {"Fully Segmented Model", ( 1, new List<String>{"MD Volumes Segmented" }) },
                {"Inital Peer Review", (1, new List<String>{"Fully Segmented Model"}) },
                {"Segmentation Corrections", (3, new List<String>{"Inital Peer Review" }) },
                {"2nd Peer Review", (1, new List<String>{"Segmentation Corrections" }) },
                {"2nd Segmentation Corrections", (3, new List<String>{"2nd Peer Review" }) },
                {"Full Model Approved", (1, new List<String>{"2nd Segmentation Corrections" }) },
                {"Meshed Model", (1, new List<String>{"Full Model Approved" }) },
                {"Export Model to Physics", (1, new List<String>{"Meshed Model" }) },
                {"Model Solved", (5, new List<String>{"Export Model to Physics" }) },
                {"Report Generated", (5, new List<String>{"Model Solved" }) }
            };

        public static List<Rectangle> plannedTaskBlocks { get; set; }
        public static List<Rectangle> completedTaskBlocks { get; set; }

        public static void initcompletedTasks(int ModelId)
        {
            // TODO: Get List of completed Tasks from Database
            //       For each task, get startDate, endDate, userName
        }
        
        public static void initTaskStartDelayPlanned()
        {
            // Essentially a Topological sort
            // DFS serach to assign day offests for each start task
            // This will ensure each task will start to minimize the total time spend working on the project
            // Runtime O(n)
            taskStartDelayPlanned = new Dictionary<string, int>();
            HashSet<String> visited = new HashSet<String>();
            Stack<String> search = new Stack<String>();
            Dictionary<string, List<string>> taskSettingsReversed = new Dictionary<string, List<string>>();


            foreach(KeyValuePair<String, (int, List<String>)> taskSetting in taskSettings)
            {
                if (taskSetting.Value.Item2 != null)
                {
                    foreach(String edge in taskSetting.Value.Item2)
                    {
                        if (!taskSettingsReversed.ContainsKey(edge)) taskSettingsReversed[edge] = new List<string>();
                        taskSettingsReversed[edge].Add(taskSetting.Key);
                    }
                }
            }

            //Dictionary<String, (int, String)> tasksSettings = taskSettings; 
            foreach(KeyValuePair<String, List<String>> node in taskSettingsReversed)
            {
                if (visited.Contains(node.Key)) continue;

                taskStartDelayPlanned[node.Key] = 0;

                int offset = data.taskSettings[node.Key].Item1;

                search.Push(node.Key);
                
                while(search.Count != 0)
                {
                    String taskName = search.Pop();
                    visited.Add(taskName);
                    taskStartDelayPlanned[taskName] = offset;
                    offset += data.taskSettings[taskName].Item1;

                    if (!taskSettingsReversed.ContainsKey(taskName)) continue;

                    foreach(String edge in taskSettingsReversed[taskName])
                    {
                        if(visited.Contains(edge)) continue;
                        search.Push(edge);
                    }

                }

            }
        }


    }
}
