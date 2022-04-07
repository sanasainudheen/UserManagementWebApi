using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TaskManagerWebApi.Context;
using TaskManagerWebApi.Models;

namespace TaskManagerWebApi.Repository
{
    public class TaskRepository:ITaskRepository
    {
        private readonly AppDbContext _context;

        public TaskRepository(AppDbContext context)
        {
            _context = context;
        }
        public async Task<List<Group>> GetAllGroups()
        {
            var records = await _context.Groups.Select(x => new Group()
            {
               GroupId = x.GroupId,
                GroupName = x.GroupName,
                Description = x.Description,
                NoOfMembers = x.NoOfMembers,
                CreatedDate = x.CreatedDate,
                IsActive = x.IsActive
            }).ToListAsync();
            return records;
        }
       
        public async Task<int> CreateGroup([FromBody] Group groupModel)
        {

            var group = new Group
            {

               GroupName=groupModel.GroupName,
               Description=groupModel.Description,
               NoOfMembers=groupModel.NoOfMembers,
               CreatedDate=groupModel.CreatedDate,
               IsActive=groupModel.IsActive

            };
            _context.Groups.Add(group);
          var result=  await _context.SaveChangesAsync();
            if (result > 0)
            {
                return group.GroupId;
            }
            else
                return 0;
        }
       
        public async Task<int> CreateTask([FromBody] TaskModel taskModel)
        {

            var taskNew = new TaskModel
            {

                TaskName = taskModel.TaskName,
                TaskDescription = taskModel.TaskDescription,
                StartDate= taskModel.StartDate,
                EndDate= taskModel.EndDate,
                StatusId= taskModel.StatusId,
                CreatedDate= taskModel.CreatedDate,
                IsActive= taskModel.IsActive


            };
            _context.Tasks.Add(taskNew);
            var result = await _context.SaveChangesAsync();
            if (result > 0)
            {
                return taskNew.TaskId;
            }
            else
                return 0;
        }
        public async Task<int> CreateUserGroup([FromBody] UserGroup userGroupModel)
        {

            var userGroupNew = new UserGroup
            {

                GroupId = userGroupModel.GroupId,
                UserId = userGroupModel.UserId,
                IsActive = userGroupModel.IsActive 
            };
            _context.UserGroups.Add(userGroupNew);
            var result = await _context.SaveChangesAsync();
            if (result > 0)
            {
                return userGroupNew.UserGroupId;
            }
            else
                return 0;
        }
        public async Task<int> CreateUserGroupTask([FromBody] UserGroupTask userGroupTaskModel)
        {

            var userGroupTaskNew = new UserGroupTask
            {

                GroupId = userGroupTaskModel.GroupId,
                TaskId = userGroupTaskModel.TaskId,
                IsActive = userGroupTaskModel.IsActive,
                //StatusId= userGroupTaskModel.StatusId
            };
            _context.UserGroupTasks.Add(userGroupTaskNew);
            var result = await _context.SaveChangesAsync();
            if (result > 0)
            {
                return userGroupTaskNew.UserGroupTaskId;
            }
            else
                return 0;
        }
        public async Task DeleteGroup(int groupId)
        {
            var group = new Group () { GroupId = groupId };
            _context.Groups.Remove(group);
            await _context.SaveChangesAsync();

        }
        public async Task<List<FetchUserGroup>> GetAllUserGroups()
        {
            var records = await _context.FetchUserGroups.FromSqlRaw("SP_GetUserGroups").ToListAsync();
            return records;
        }
        public async Task<List<TaskModel>> GetAllTasks()
        {
            var records = await _context.Tasks.Select(x => new TaskModel()
            {
                TaskId = x.TaskId,
                TaskName = x.TaskName,
                TaskDescription = x.TaskDescription,
                StartDate = x.StartDate,
                EndDate = x.EndDate,
                IsActive = x.IsActive
            }).ToListAsync();
            return records;
        }
        public async Task<List<TaskModel>> GetAllPendingTasks()
        {
            var records = await _context.Tasks.Where(x => x.StatusId == "1").ToListAsync();           
            return records;
        }
        public async Task<List<FetchUserGroupTask>> getAllTasksByAdmin(int taskId,int flag)
        {

            var parameters = new List<SqlParameter>();

            parameters.Add(new SqlParameter("@Taskid", taskId));
            parameters.Add(new SqlParameter("@Flag", flag));

            var records = await _context.FetchUserGroupTasks.FromSqlRaw("EXEC sp_ViewTasksByAdmin @Taskid,@Flag", parameters.ToArray()).ToListAsync();
            return records;
        }

        public async Task<int> UpdateUserGroupTaskStatus(string StatusId, int TaskId)
        {
            var rec = await _context.Tasks.FindAsync(TaskId);
            if (rec != null)
            {
                rec.StatusId = StatusId;
                await _context.SaveChangesAsync();
            }
            return rec.TaskId;
        }

        //public async Task<int> AssignTaskToUser([FromBody] TaskLog taskLog)
        //{
        //    var output = 0;
        //    var userGT = _context.UserGroupTasks.Where(x => x.UserGroupTaskId == taskLog.UserGroupTaskId).FirstOrDefault();
        //    if (userGT != null)
        //    {
        //        var taskId=userGT.TaskId;
        //        var list1 = _context.UserGroupTasks.Where(x =>( x.TaskId == taskId && (x.StatusId==2 || x.StatusId==3))).FirstOrDefault();

        //        if (list1 == null)
        //        {
        //            var newTask = new TaskLog
        //            {

        //                UserGroupTaskId = taskLog.UserGroupTaskId,
        //                UserId = taskLog.UserId,
        //                StatusId = taskLog.StatusId,
        //                Attachment = taskLog.Attachment,
        //                Note = taskLog.Note,
        //                CreatedBy = taskLog.CreatedBy,
        //                CreatedOn = taskLog.CreatedOn,
        //            };
        //            _context.TaskLogTable.Add(newTask);
        //            var result = await _context.SaveChangesAsync();
        //            var TaskLogId = newTask.LogId;
        //            if (result > 0)
        //            {

        //                int res = await UpdateUserGroupTaskStatus(taskLog.StatusId, taskLog.UserGroupTaskId);
        //                output = TaskLogId;
        //                return output;
        //            }
        //            else
        //            {
        //                output = 0;
        //                return output;
        //            }

        //        }
        //        else
        //        {
        //            output = -1;
        //            return output;
        //        }

        //    }
        //    else
        //    {
        //        output = 0;
        //        return output;
        //    }
        //}
        public async Task<int> UpdateTaskStatus(string StatusId, int TaskId)
        {
            var rec = await _context.Tasks.FindAsync(TaskId);
            if (rec != null)
            {
                rec.StatusId = StatusId;
                await _context.SaveChangesAsync();
            }
            return rec.TaskId;
        }
        public async Task<int> AssignTaskToGroup([FromBody] UserGroupTask userGroupTask)
        {           
                    var newTask = new UserGroupTask
                    {
                        GroupId = userGroupTask.GroupId,
                        TaskId = userGroupTask.TaskId,
                        IsActive = userGroupTask.IsActive,
                        Attachment = userGroupTask.Attachment,
                        Note = userGroupTask.Note,
                    };
                    _context.UserGroupTasks.Add(newTask);
                    var result = await _context.SaveChangesAsync();
            if(result>0)
            {
                int res = await UpdateTaskStatus("2", userGroupTask.TaskId);
                if(res>0)
                {
                    return 1;
                }
                else
                {
                    return 0;
                }
            }
            else
                return 0;
                   
        }
        public async Task<int> AssignUserToGroup([FromBody] UserGroup userGroup)
        {
            int count=_context.UserGroups.Where(x=>x.GroupId==userGroup.GroupId && x.IsActive=="1").Count();
            string memberCount = _context.Groups.Where(x=>x.GroupId==userGroup.GroupId).FirstOrDefault().NoOfMembers;           
            if (count < Int32.Parse(memberCount))
            {
                var userExist=_context.UserGroups.Where(x=>x.GroupId==userGroup.GroupId && x.IsActive=="1" && x.UserId==userGroup.UserId).FirstOrDefault();
                if (userExist == null)
                {
                    var newTask = new UserGroup
                    {
                        GroupId = userGroup.GroupId,
                        UserId = userGroup.UserId,
                        IsActive = userGroup.IsActive
                    };
                    _context.UserGroups.Add(newTask);
                    var result = await _context.SaveChangesAsync();
                    if (result > 0)
                    {
                        return 1;//user assigned to group
                    }
                    else
                        return 0;//some error occured
                }
                else
                {
                    return -2;//same user already assigned
                }
            }
            else
            {
                return -1;//member limit exceeded
            }


        }
        public async Task<List<FetchUserGroupTask>> GetGroupTasksByUser(string userId,int taskId,int flag)
        {
            var parameters = new List<SqlParameter>();
            parameters.Add(new SqlParameter("@UserId", userId));
            parameters.Add(new SqlParameter("@Taskid", taskId));
            parameters.Add(new SqlParameter("@Flag", flag));

            var records = await _context.FetchUserGroupTasks.FromSqlRaw("EXEC sp_ViewTasksByUser @UserId, @Taskid,@Flag", parameters.ToArray()).ToListAsync();
            return records;
        }
        public async Task<List<FetchUserGroupTask>> GetDoneGroupTasksByUser(string userId, int taskId, int flag)
        {
            var parameters = new List<SqlParameter>();
            parameters.Add(new SqlParameter("@UserId", userId));
            parameters.Add(new SqlParameter("@Taskid", taskId));
            parameters.Add(new SqlParameter("@Flag", flag));

            var records = await _context.FetchUserGroupTasks.FromSqlRaw("EXEC sp_ViewTasksByUser @UserId, @Taskid,@Flag", parameters.ToArray())
               .ToListAsync();
            return records;
        }

        public async Task<List<PendingTasks>>AssignedTasksByUser(string id,int statusId)
        {
            var parameters = new List<SqlParameter>();
           
                parameters.Add(new SqlParameter("@UserId", id));
            parameters.Add(new SqlParameter("@StatusId", statusId));
        
            var records = await _context.PendingTasks.FromSqlRaw("EXEC SP_GetAssignedTasksByUser @UserId,@StatusId", parameters.ToArray()).ToListAsync();
            return records;
        }

        public async Task<List<AssignedTasks>> ViewTaskDetails(int userGroupTaskId, string userId)
        {
            var parameters = new List<SqlParameter>();

            parameters.Add(new SqlParameter("@UserGroupTaskId", userGroupTaskId));
            parameters.Add(new SqlParameter("@UserId", userId));

            var records = await _context.AssignedTasks.FromSqlRaw("EXEC SP_ViewTaskDetails @UserGroupTaskId,@UserId", parameters.ToArray()).ToListAsync();
            return records;
        }
        public async Task<int> UpdateUserStatus([FromBody] TaskLog taskLog)
        {

            var newTask = new TaskLog
            {
                UserId = taskLog.UserId,
                StatusId = taskLog.StatusId,               
                TaskId= taskLog.TaskId,
                CreatedOn = taskLog.CreatedOn,
            };
            _context.TaskLogTable.Add(newTask);
            var result = await _context.SaveChangesAsync();
            var TaskLogId = newTask.LogId;
            if (result > 0)
            {

                int res = await UpdateUserGroupTaskStatus(taskLog.StatusId.ToString(), taskLog.TaskId);
                return TaskLogId;
            }
            else
                return 0;
        }
        public async Task<int> enableDisable( int groupId, string value)
        {
            var iser = _context.Groups.Where(x => x.GroupId ==groupId).FirstOrDefault();
        
            if (iser == null)
            {
                return 0;
            }
            else { 
            iser.IsActive = value;
            var res = await _context.SaveChangesAsync();            
                return 1;
            }
           

            }
        }
    }


