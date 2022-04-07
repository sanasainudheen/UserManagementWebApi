using Microsoft.EntityFrameworkCore.Migrations;

namespace TaskManagerWebApi.Migrations
{
    public partial class V3 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            var sp = @"Alter procedure [dbo].[sp_ViewTasksByUser]  
(  
@UserId varchar(75),  
@TaskId int,  
@Flag int  
)  
AS  
Begin  
If(@Flag=1)  --Pending Tasks
Begin  
 select G.GroupName,UG.GroupId,UGT.TaskId,T.TaskName,T.TaskDescription,T.StartDate,T.EndDate,  
case T.StatusId when 1 then 'Pending' when 2 then 'Processing' when 3 then 'Completed' when 4 then 'Done' when   
 5 then 'Rejected' end as Status,UGT.Attachment,UGT.Note  
from UserGroups UG  
inner join Groups G on UG.GroupId=G.GroupId  
inner join UserGroupTasks UGT on UGT.GroupId=UG.GroupId  
inner join Tasks T on T.TaskId=UGT.TaskId  
where UserId=@UserId and T.StatusId in (2,5)
End  
else if(@Flag=2)  --Each Task details
begin  
select G.GroupName,UG.GroupId,UGT.TaskId,T.TaskName,T.TaskDescription,T.StartDate,T.EndDate,  
case T.StatusId when 1 then 'Pending' when 2 then 'Processing' when 3 then 'Completed' when 4 then 'Done' when   
 5 then 'Rejected' end as Status,UGT.Attachment,UGT.Note  
from UserGroups UG  
inner join Groups G on UG.GroupId=G.GroupId  
inner join UserGroupTasks UGT on UGT.GroupId=UG.GroupId  
inner join Tasks T on T.TaskId=UGT.TaskId  
where UserId=@UserId and UGT.TaskId=@TaskId   
 END  
 else if(@Flag=3)  --CompletedDone Tasks
begin  
select G.GroupName,UG.GroupId,UGT.TaskId,T.TaskName,T.TaskDescription,T.StartDate,T.EndDate,  
case T.StatusId when 1 then 'Pending' when 2 then 'Processing' when 3 then 'Completed' when 4 then 'Done' when   
 5 then 'Rejected' end as Status,UGT.Attachment,UGT.Note  
from UserGroups UG  
inner join Groups G on UG.GroupId=G.GroupId  
inner join UserGroupTasks UGT on UGT.GroupId=UG.GroupId  
inner join Tasks T on T.TaskId=UGT.TaskId  
where UserId=@UserId  and T.StatusId in (3,4)
 END 
 END";
            migrationBuilder.Sql(sp);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {

        }
    }
}
