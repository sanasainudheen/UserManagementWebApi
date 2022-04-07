using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
namespace TaskManagerWebApi.Models
{
    public class TaskLog
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int LogId { get; set; }

        [Display(Name = "TaskId")]
        public virtual int TaskId { get; set; }

        [ForeignKey("TaskId")]
        public virtual TaskModel Tasks { get; set; }

        [Display(Name = "ApplicationUser")]
        public  string UserId { get; set; }

        [ForeignKey("UserId")]
        public virtual ApplicationUser ApplicationUsers { get; set; }
       
        public int StatusId { get; set; }       
       
        public string CreatedOn { get; set; }



    }
}
