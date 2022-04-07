﻿using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
namespace TaskManagerWebApi.Models
{
    [Table("Groups")]
    public class Group
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int GroupId { get; set; }
        public string GroupName { get; set; }

        public string Description { get; set; }
        public string NoOfMembers { get; set; }

        public string CreatedDate { get; set; }
        public string IsActive { get; set; }
    }
}
