﻿using Microsoft.AspNetCore.Mvc;
using YaqraApi.DTOs;
using YaqraApi.Helpers;
using YaqraApi.Services;
using YaqraApi.Services.IServices;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace YaqraApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly IAuthService _authService;

        public AuthController(IAuthService authService)
        {
            _authService = authService;
        }
        [HttpPost("register")]
        public async Task<IActionResult> RegisterAsync(RegisterDto registerDto)
        {
            var result = await _authService.RegisterAsync(registerDto, new List<string> { Roles.User });
            if(result.IsAuthenticated == false)
                return BadRequest(result.Message);
            return Ok(result);
        }
        [HttpPost("registerAdmin")]
        public async Task<IActionResult> RegisterAdminAsync(RegisterDto registerDto)
        {
            var result = await _authService.RegisterAsync(registerDto, new List<string> { Roles.User, Roles.Admin });
            if (result.IsAuthenticated == false)
                return BadRequest(result.Message);
            return Ok(result);
        }
    }
}