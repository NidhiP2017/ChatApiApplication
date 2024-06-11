using AutoMapper.Execution;
using Azure.Core;
using ChatApiApplication.Data;
using ChatApiApplication.DTO;
using ChatApiApplication.Exceptions;
using ChatApiApplication.Model;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Security.Claims;
using System.Text.Json;
using System.Text.RegularExpressions;

namespace ChatApiApplication.Interfaces
{
    public class GroupService : IGroupService
    {
        private readonly ChatAPIDbContext _appContext;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IConfiguration _config;
        private readonly IMessagesService _msgService;

        public GroupService(ChatAPIDbContext context, IConfiguration config,
             IHttpContextAccessor httpContextAccessor, IMessagesService msgService)
        {
            _appContext = context;
            _config = config;
            _httpContextAccessor = httpContextAccessor;
            _msgService = msgService;
        }
        public async Task<bool> IsUserMemberOfGroup(Guid userId, int groupId)
        {
            var isMember = await _appContext.GroupMembers
           .AnyAsync(gm => gm.UserId == userId && gm.GroupId == groupId);

            return isMember;
        }

        public async Task<List<Guid>> GetGroupMemberIdsAsync(int groupId)
        {
            return await _appContext.GroupMembers
                .Where(gm => gm.GroupId == groupId)
                .Select(gm => gm.UserId)
                .ToListAsync();
        }
        public async Task<List<Model.Group>> GetGroups()
        {
            var currentUserId = await GetCurrentLoggedInUser();
            if (currentUserId == null)
            {
                throw new Exception("Login is mandatory");
            }

            var userGroups = await _appContext.GroupMembers
                .Include(gm => gm.Group.GroupMembers)
                .Where(gm => gm.UserId == currentUserId)
                .Select(gm => gm.Group)
                .ToListAsync();

            return userGroups;

        }

        public async Task<Model.Group> GetGroupByIdAsync(int grpId)
        {
            return await _appContext.Groups
                .FirstOrDefaultAsync(g => g.GroupId == grpId);
        }

        public async Task<GroupResponseDTO> CreateGroup(GroupCreateRequestDTO request)
        {
            if (string.IsNullOrEmpty(request.GroupName))
            {
                throw new ArgumentException("Group name is required.");
            }
            var currentUser = await GetCurrentLoggedInUser();

            if (currentUser == null)
            {
                throw new Exception("Unable to retrieve currentuser");
            }
            var group = new Model.Group
            {
                GroupName = request.GroupName,
            };
            await _appContext.Groups.AddAsync(group);
            await _appContext.SaveChangesAsync();

            var groupMember = new GroupMembers
            {
                UserId = currentUser,
                GroupId = group.GroupId,
                JoinTime = DateTime.UtcNow,
                IncludePreviousChats = true,
                NumOfDays = "All"
            };
            await _appContext.GroupMembers.AddAsync(groupMember);
            await _appContext.SaveChangesAsync();

            var response = new GroupResponseDTO
            {
                GroupId = group.GroupId,
                GroupName = group.GroupName,
            };
            return response;
        }

        public async Task<GroupResponseDTO> EditGroupName(int grpId, string editGrpName)
        {
            var group = await _appContext.Groups.FindAsync(grpId);
            if (string.IsNullOrEmpty(editGrpName))
            {
                throw new ArgumentException("New Group name is required.");
            }
            var currentUserId = await GetCurrentLoggedInUser();

            if (currentUserId == null)
            {
                throw new Exception("Unable to retrieve currentuser");
            }
            var IsPartOfGrp = _appContext.GroupMembers.FirstOrDefault(u => u.UserId == currentUserId && u.GroupId == grpId);
            if (IsPartOfGrp == null)
            {
                throw new Exception("You are not a part of group so you cannot edit its name");
            }
            else
            {
                group.GroupName = editGrpName;
                await _appContext.SaveChangesAsync();
                var response = new GroupResponseDTO
                {
                    GroupId = grpId,
                    GroupName = editGrpName,
                };

                return response;
            }
        }

        public async Task<Guid> GetCurrentLoggedInUser()
        {
            var Id = _httpContextAccessor.HttpContext.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            Guid currentUserId = await _appContext.ChatUsers.Where(m => m.Id == Id).Select(u => u.UserId).FirstOrDefaultAsync();
            if (currentUserId == null)
                return Guid.Empty;
            else
                return currentUserId;
        }
        public async Task<Model.Group> GetGroupWithMembers(int grpId)
        {
            // Retrieve the group with its associated GroupMembers
            var groupMemb = await _appContext.Groups
                        .Include(g => g.GroupMembers)
                        .ThenInclude(gm => gm.User)
                .FirstOrDefaultAsync(g => g.GroupId == grpId);

            return groupMemb;
        }

        public async Task<IActionResult> EditGroupMembers(int grpId, UpdateGroupMembersDTO request)
        {
            var currentUserId = await GetCurrentLoggedInUser();
            if (currentUserId == null)
            {
                return new NotFoundObjectResult("User not found");
            }

            var IsPartOfGrp = _appContext.GroupMembers.FirstOrDefault(u => u.UserId == currentUserId && u.GroupId == grpId);
            if (IsPartOfGrp == null)
            {
                return new UnauthorizedObjectResult("You are not authorized to edit for this group because you are not a part of this group");
            }
            var existingGrpMembers = await GetGroupWithMembers(grpId);
            if (existingGrpMembers == null)
            {
                return new NotFoundObjectResult("Group not found.");
            }
            if (request.MembersToAdd != null && request.MembersToAdd[0] != "string")
            {
                foreach (var memberId in request.MembersToAdd)
                {
                    var presentMembers = existingGrpMembers.GroupMembers.FirstOrDefault(gm => gm.UserId == Guid.Parse(memberId));
                    if (presentMembers == null)
                    {
                        var timestampNow = DateTime.Now;
                        bool include = request.IncludePreviousChat;
                        var newMember = new GroupMembers
                        {
                            UserId = Guid.Parse(memberId),
                            GroupId = grpId,
                            JoinTime = timestampNow,  // Set the timestamp
                            IncludePreviousChats = include
                        };
                        existingGrpMembers.GroupMembers.Add(newMember);
                        await _appContext.SaveChangesAsync();
                    }
                    else
                    {
                        return new OkObjectResult("Member already exist in the group");
                    }

                }
            }
            if (request.MembersToRemove != null && request.MembersToRemove[0] != "string")
            {
                foreach (var memberId in request.MembersToRemove)
                {
                    var currentGrpMembers = await GetGroupWithMembers(grpId);
                    if (currentGrpMembers != null)
                    {
                        var memberToRemove = currentGrpMembers.GroupMembers.FirstOrDefault(gm => gm.UserId == Guid.Parse(memberId));
                        if (memberToRemove != null)
                        {
                            existingGrpMembers.GroupMembers.Remove(memberToRemove);
                            await _appContext.SaveChangesAsync();

                        }
                    }
                    else
                    {
                        return new UnauthorizedObjectResult("You cannot modify members in this group");
                    }
                }
            }
            var result = await GetGroupWithMembers(grpId);
            var response = new AddMemberResDTO
            {
                GroupId = result.GroupId,
                GroupName = result.GroupName,
                GroupMembers = result.GroupMembers
                        .Where(gm => gm.User != null)
                        .Select(gm => gm.User.UserName)
                        .ToList()

            };

            return new OkObjectResult(response);
        }
        public async Task<IActionResult> SendMessage(int groupId, GroupMessageRequestDTO messageRequest)
        {
            var currentUserId = await GetCurrentLoggedInUser();
            var isMemberOfGroup = await IsUserMemberOfGroup(currentUserId, groupId);

            if (!isMemberOfGroup)
            {
                return new UnauthorizedObjectResult("You are not a member of this group.");
            }
            if (string.IsNullOrEmpty(messageRequest.content))
            {
                throw new ArgumentException("Message is required.");
            }
            var group = await GetGroupByIdAsync(groupId);
            if (group == null)
            {
                return new NotFoundObjectResult("Group not found.");
            }
            
            var memberIds = await GetGroupMemberIdsAsync(groupId);
            
            foreach (var memberId in memberIds)
            {
                if (currentUserId != memberId)
                {
                    var message = new Messages
                    {
                        Content = messageRequest.content,
                        SenderId = currentUserId,
                        ReceiverId = memberId,
                        GroupId = groupId,
                        Timestamp = DateTime.Now,
                    };
                    //message.ReceiverId = memberId;
                    //var AddedMessage = _appContext.Messages.Add(message).Entity;
                    await _appContext.Messages.AddAsync(message);
                    await _appContext.SaveChangesAsync();
                }
            }
            var response = new
            {
                senderId = currentUserId,
                groupId = groupId,
                content = messageRequest.content,
                timestamp = DateTime.Now
            };
            return new OkObjectResult(response);
        }

        public async Task<IActionResult> SendReplyInThread(int groupId, Guid messageId, ThreadMessageDTO messageRequest)
        {
            var currentUserId = await GetCurrentLoggedInUser();
            var isMemberOfGroup = await IsUserMemberOfGroup(currentUserId, groupId);
            if (isMemberOfGroup)
            {
                return new UnauthorizedObjectResult("You are not a member of this group so you cannot reply here in thread.");
            }
            return await _msgService.ReplyToMsg(groupId, messageId, messageRequest);
        }

    }
}
