using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AmoCRM
{
    public class TaskCRM
    {
        public int id { get; set; }
        public int account_id { get; set; }
        public int group_id { get; set; }
        public int element_id { get; set; }
        public MethodsAPI.ElementType element_type { get; set; }
        public long complete_till_at { get; set; }
        public MethodsAPI.TaskType task_type { get; set; }
        public string text { get; set; }
        public long created_at { get; set; }
        public long updated_at { get; set; }
        public bool is_completed { get; set; }
        public int responsible_user_id { get; set; }
        public int created_by { get; set; }

        object _links { get; set; }
        string[] result { get; set; }

        public TaskCRM()
        {

        }

        public TaskCRM(int element_id, MethodsAPI.ElementType element_type, long complete_till_at, MethodsAPI.TaskType task_type, string text,
                long created_at, long updated_at, bool is_completed, int responsible_user_id, int created_by)
        {
            DateTime unixStart = DateTime.SpecifyKind(new DateTime(1970, 1, 1), DateTimeKind.Utc);
            this.element_id = element_id;
            this.element_type = element_type;
            this.complete_till_at = complete_till_at;
            this.task_type = task_type;
            this.text = text;
            this.created_at = created_at;
            this.updated_at = updated_at;
            this.is_completed = is_completed;
            this.responsible_user_id = responsible_user_id;
            this.created_by = created_by;
        }

        public string SerializeItem()
        {
            string jsonItem = "{\"element_id\":" + this.element_id + ","
                     + "\"element_type\":" + (int)this.element_type + ","
                     + "\"complete_till_at\":" + this.complete_till_at + ","
                     + "\"task_type\":" + (int)this.task_type + ","
                     + "\"text\":\"" + this.text + "\","
                     + "\"created_at\":" + this.created_at + ","
                     + "\"updated_at\":" + this.updated_at + ","
                     + "\"responsible_user_id\":" + this.responsible_user_id + ","
                     + "\"created_by\":" + this.created_by + "}";
            return jsonItem;
        }
    }
}
