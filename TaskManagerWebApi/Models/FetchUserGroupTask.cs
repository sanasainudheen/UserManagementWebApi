using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
namespace TaskManagerWebApi.Models
{
    public class FetchUserGroupTask
    {
        [Key]
        public int TaskId { get; set; }
        public int GroupId { get; set; }    
      
        public string GroupName { get; set; }
        public string TaskName { get; set; }
        public string TaskDescription { get; set; }
        public string StartDate { get; set; }
        public string EndDate { get; set; }

        public string Status { get; set; }
        public string Attachment { get; set; }
        public string Note { get; set; }
    }
}
