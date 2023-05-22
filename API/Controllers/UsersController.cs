using API.DTOs;
using API.Interfaces;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers
{
    [Authorize]
    public class UsersController : BaseApiController
    {
        private readonly IUserRepository _uesrRepo;
        private readonly IMapper _mapper;

        public UsersController(IUserRepository uesrRepo, IMapper mapper)
        {
            _mapper = mapper;
            _uesrRepo = uesrRepo;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<MemberDto>>> GetUsers()
        {
           return Ok(await _uesrRepo.GetMembersAsync());
          
        }

        [HttpGet("{username}")]
        public async Task<ActionResult<MemberDto>> GetUser(string username)
        {
            return Ok(await _uesrRepo.GetMemberAsync(username));
        }
    }
}