using Microsoft.EntityFrameworkCore.Migrations;

namespace TaskManagerWebApi.Migrations
{
    public partial class V2 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            var sp = @"Alter procedure sp_ViewTasksByAdmin  
(  
@Taskid int,  
@Flag int  
)  
AS  
Begin  
If(@Flag=1)  
Begin  
 select T.TaskId,T.TaskName,T.TaskDescription,T.StartDate,T.EndDate,  
 case T.StatusId when 1 then 'Pending' when 2 then 'Processing' when 3 then 'Completed' when 4 then 'Done' when   
 5 then 'Rejected' end as Status  
 ,ISNULL(G.GroupId,'') as GroupId,ISNULL(G.GroupName ,'')as groupName  ,'' as Attachment,'' as Note
 from tasks T  
 Left outer join UserGroupTasks UGT on T.TaskId=UGT.TaskId  
 left outer join Groups G on G.GroupId=UGT.GroupId  
End  
else if(@Flag=2)  
begin  
select T.TaskId,T.TaskName,T.TaskDescription,T.StartDate,T.EndDate,  
 case T.StatusId when 1 then 'Pending' when 2 then 'Processing' when 3 then 'Completed' when 4 then 'Done' when   
 5 then 'Rejected' end as Status  
 ,ISNULL(G.GroupId,'') as GroupId,ISNULL(G.GroupName ,'')as groupName   ,'' as Attachment,'' as Note
 from tasks T  
 Left outer join UserGroupTasks UGT on T.TaskId=UGT.TaskId  
 left outer join Groups G on G.GroupId=UGT.GroupId  
 where T.TaskId=@Taskid  
 END  
 END";
            migrationBuilder.Sql(sp);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {

        }
    }
}
