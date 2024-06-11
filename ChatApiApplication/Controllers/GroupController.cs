using ChatApiApplication.Data;
using ChatApiApplication.DTO;
using ChatApiApplication.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ChatApiApplication.Controllers
{
    [ApiController]
    [Route("api/Groups")]
    public class GroupController : ControllerBase
    {
        private readonly ChatAPIDbContext _dbContext;
        private readonly IGroupService _groupService;

        public GroupController( ChatAPIDbContext dbContext, IGroupService groupService)
        {
            _dbContext = dbContext;
            _groupService = groupService;
        }

        [Authorize]
        [HttpGet]
        [Route("getGroups")]
        public async Task<List<Model.Group>> GetGroups()
        {
            return await _groupService.GetGroups();
        }

        [Authorize]
        [HttpPost]
        [Route("createGroup")]
        public async Task<GroupResponseDTO> CreateGroup([FromBody] GroupCreateRequestDTO request)
        {
            return await _groupService.CreateGroup(request);
        }

        [Authorize]
        [HttpPut]
        [Route("editGroup")]
        public async Task<GroupResponseDTO> EditGroupName(int grpId, string editGrpName)
        {
            return await _groupService.EditGroupName(grpId, editGrpName);
        }

        [Authorize]
        [HttpPut]
        [Route("addEditMembers")]
        public async Task<IActionResult> addEditMembers(int grpId, [FromBody] UpdateGroupMembersDTO request)
        {
            return await _groupService.EditGroupMembers(grpId, request);
        }

        [Authorize]
        [HttpPost("{groupId}/messages")]
        public async Task<IActionResult> SendMessage(int groupId, [FromBody] GroupMessageRequestDTO messageRequest)
        {
            return await _groupService.SendMessage(groupId, messageRequest);
        }

        [Authorize]
        [HttpPost("{groupId}/{messageId}/messages")]
        /*reply to a particular message in thread*/
        public async Task<IActionResult> SendReplyInThread(int groupId, Guid messageId, [FromBody] ThreadMessageDTO messageRequest)
        {
            return await _groupService.SendReplyInThread(groupId, messageId, messageRequest);
        }
    }
}
