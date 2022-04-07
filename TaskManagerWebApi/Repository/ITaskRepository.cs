
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Threading.Tasks;
using TaskManagerWebApi.Models;

namespace TaskManagerWebApi.Repository
{
    public interface ITaskRepository
    {
        Task<int> CreateGroup(Group groupModel);
        Task<int> CreateTask(TaskModel TaskModel);

        Task<int> CreateUserGroup(UserGroup userGroup);

      // Task<int> AssignTaskToUser(TaskLog taskLog);
        Task<int> AssignTaskToGroup(UserGroupTask userGroupTask);
        Task<int> AssignUserToGroup(UserGroup userGroup);
        Task<int> UpdateUserStatus(TaskLog taskLog);

        Task<int> CreateUserGroupTask(UserGroupTask userGroupTask);
        Task DeleteGroup(int groupId);

        Task<List<Group>> GetAllGroups();

        Task<List<FetchUserGroup>> GetAllUserGroups();

        Task<List<FetchUserGroupTask>> getAllTasksByAdmin(int taskId, int flag);
        Task<List<TaskModel>> GetAllTasks();
        Task<List<TaskModel>> GetAllPendingTasks();

        Task<List<FetchUserGroupTask>> GetGroupTasksByUser(string userId, int taskId,int flag);
        Task<List<FetchUserGroupTask>> GetDoneGroupTasksByUser(string userId, int taskId, int flag);

        Task<List<PendingTasks>> AssignedTasksByUser(string id,int statusId);

        Task<List<AssignedTasks>> ViewTaskDetails(int taskId, string userId);
        Task<int> enableDisable(int groupId, string value);
    }
}
