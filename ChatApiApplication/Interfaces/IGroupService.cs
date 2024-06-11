using Azure.Core;
using ChatApiApplication.DTO;
using ChatApiApplication.Model;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Text.RegularExpressions;

namespace ChatApiApplication.Interfaces
{
    public interface IGroupService
    {
        Task<bool> IsUserMemberOfGroup(Guid userId, int groupId);
        Task<List<Guid>> GetGroupMemberIdsAsync(int groupId);
        Task<Model.Group> GetGroupByIdAsync(int grpId);
        Task<List<Model.Group>> GetGroups();
        Task<GroupResponseDTO> CreateGroup(GroupCreateRequestDTO request);
        Task<GroupResponseDTO> EditGroupName(int grpId, string editGrpName);
        Task<Model.Group> GetGroupWithMembers(int groupId);
        Task<IActionResult> EditGroupMembers(int grpId, UpdateGroupMembersDTO request);
        Task<IActionResult> SendMessage(int groupId, GroupMessageRequestDTO messageRequest);
        Task<IActionResult> SendReplyInThread(int groupId, Guid messageId, ThreadMessageDTO messageRequest);
    }
}
