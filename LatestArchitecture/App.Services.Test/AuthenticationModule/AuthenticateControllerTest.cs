using App.Api.Controllers.AuthenticationModule;
using App.Application.Dto.AuthenticationModule;
using App.Application.Interfaces.Services;
using App.Application.Interfaces.Services.AuthenticationModule;
using App.Common.Constant;
using App.Common.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Moq;
using System.Net;
namespace App.Services.Test.AuthenticationModule
{
    public class AuthenticateControllerTest
    {

        private readonly Mock<IAuthenticationService> _authServiceMock;
        private readonly IConfiguration _configuration;
        private readonly AuthenticationController _controller;
        private readonly Mock<IEmailService> _emailService;
        private readonly DefaultHttpContext _httpContext;

        public AuthenticateControllerTest()
        {
            _authServiceMock = new Mock<IAuthenticationService>();
            var inMemorySettings = new Dictionary<string, string> { { "Jwt:Secret", "test_secret_key_123" } };
            _configuration = new ConfigurationBuilder().AddInMemoryCollection(inMemorySettings).Build();
            _emailService = new Mock<IEmailService>();
            _httpContext = new DefaultHttpContext();

            // Instantiate controller first
            _controller = new AuthenticationController(_authServiceMock.Object, _configuration, _emailService.Object);

            // Now set the HttpContext
            _controller.ControllerContext.HttpContext = _httpContext;
        }

        [Fact]
        public async Task Login_ReturnsToken_WhenCredentialsAreValid()
        {
            // Arrange
            var loginDto = new LoginDto
            {
                UserName = "admin",
                Password = "Token@1234",
                OrgnizationId = 1
            };


            var expectedJsonModel = new JsonModel
            {
                Data = new { Data = "" },
                Message = StatusMessage.LoginSuccessfully,
                StatusCode = (int)HttpStatusCode.OK
            };



            _authServiceMock
                .Setup(s => s.AuthenticateAsync(It.Is<LoginDto>(x =>
                    x.UserName == loginDto.UserName &&
                    x.Password == loginDto.Password &&
                    x.OrgnizationId == loginDto.OrgnizationId)))
                .ReturnsAsync(expectedJsonModel);

            // Act
            var result = await _controller.Login(loginDto);

            var okResult = Assert.IsType<OkObjectResult>(result);
            var jsonModel = Assert.IsType<JsonModel>(okResult.Value);


            // Assert
            Assert.NotNull(jsonModel);
            Assert.Equal(200, jsonModel.StatusCode);
            Assert.Equal("Login Successfully.", jsonModel.Message);

        }


        [Fact]
        public async Task Login_ReturnsUnauthorized_WhenCredentialsAreInvalid()
        {

            // Arrange
            var loginDto = new LoginDto
            {
                UserName = "user1",
                Password = "pass1",
                OrgnizationId = 1
            };


            var expectedJsonModel = new JsonModel
            {
                Data = new { Data = "" },
                Message = StatusMessage.InvalidUserOrPassword,
                StatusCode = (int)HttpStatusCode.Unauthorized
            };


            _authServiceMock
                .Setup(s => s.AuthenticateAsync(It.Is<LoginDto>(x =>
                    x.UserName == loginDto.UserName &&
                    x.Password == loginDto.Password &&
                    x.OrgnizationId == loginDto.OrgnizationId)))
                .ReturnsAsync(expectedJsonModel);

            // Act
            var result = await _controller.Login(loginDto);

            // Assert

            var okResult = Assert.IsType<UnauthorizedObjectResult>(result);
            var jsonModel = Assert.IsType<JsonModel>(okResult.Value);
            Assert.Equal(401, jsonModel.StatusCode);
            Assert.Equal("Invalid username or password.", jsonModel.Message);
        }


        [Fact]
        public async Task ForgotPassword_ValidInput_ReturnsSuccess()
        {
            var forgotPasswordDto = new ResetUserPasswordDto
            {
                Email = "john.doe",
                ResetPasswordURL = "https://example.com/reset-password",
                Token = "",
                Password = null,
                IpAddress = "192.168.1.1",
                Source = "WebApp",
                SaltName = "Salt123"
            };


            var expectedJsonModel = new JsonModel
            {
                Data = new { Data = "" },
                Message = StatusMessage.ResetPassword,
                StatusCode = (int)HttpStatusCode.OK
            };

            _authServiceMock
                .Setup(s => s.ForgotPasswordAsync(It.Is<ResetUserPasswordDto>(x =>
                    x.Email == forgotPasswordDto.Email &&
                    x.Password == forgotPasswordDto.Password &&
                    x.IpAddress == forgotPasswordDto.IpAddress &&
                    x.IpAddress == forgotPasswordDto.IpAddress)))
                .ReturnsAsync(expectedJsonModel);



            var result = await _controller.ForgotPassword(forgotPasswordDto);
            var okResult = Assert.IsType<OkObjectResult>(result);
            var jsonModel = Assert.IsType<JsonModel>(okResult.Value);

            Assert.NotNull(jsonModel);

        }


        [Fact]
        public async Task ResetPassword_ValidInput_ReturnsSuccess()
        {
            // Arrange
            var dto = new ResetUserPasswordDto
            {
                Email = "testuser",
                Password = "NewPass123!",
                Token = "rt123N==!",
                IpAddress = "192.168.1.1"
            };



            var expectedJsonModel = new JsonModel
            {
                Data = new { Data = "" },
                Message = StatusMessage.ResetPassword,
                StatusCode = (int)HttpStatusCode.OK
            };

            _authServiceMock
                 .Setup(s => s.ResetPasswordAsync(It.Is<ResetUserPasswordDto>(x =>
                     x.Email == dto.Email &&
                     x.Password == dto.Password &&
                     x.Token == dto.Token &&
                     x.IpAddress == dto.IpAddress)))
                 .ReturnsAsync(expectedJsonModel);

            // Act
            var result = await _controller.ResetPassword(dto);

            // Assert



            var okResult = Assert.IsType<OkObjectResult>(result);
            var jsonModel = Assert.IsType<JsonModel>(okResult.Value);
            Assert.NotNull(jsonModel);
            Assert.Equal(200, jsonModel.StatusCode);
        }



        //[Fact]
        //public async Task GetUserByToken_ValidAuthorizationHeader_ReturnsSuccess()
        //{

        //    var token = " eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJodHRwOi8vc2NoZW1hcy54bWxzb2FwLm9yZy93cy8yMDA1LzA1L2lkZW50aXR5L2NsYWltcy9uYW1laWRlbnRpZmllciI6IjEiLCJodHRwOi8vc2NoZW1hcy54bWxzb2FwLm9yZy93cy8yMDA1LzA1L2lkZW50aXR5L2NsYWltcy9uYW1lIjoiYWRtaW4iLCJodHRwOi8vc2NoZW1hcy5taWNyb3NvZnQuY29tL3dzLzIwMDgvMDYvaWRlbnRpdHkvY2xhaW1zL3JvbGUiOiJhZG1pbiIsIk9yZ2FuaXphdGlvbklkIjoiMyIsIlJvbGVJZCI6IjEiLCJleHAiOjE3NTQ5ODQwMDAsImlzcyI6InNtYXJ0UE9EIiwiYXVkIjoiaHR0cDovL2xvY2FsaG9zdDo0MjAwLyJ9.BBqHp1fhgiWK8ocbtQtYnvA0Vz6n1LPeKkkARqsrsh8";
        //    var authHeader = $"Bearer {token}";


        //    var context = new DefaultHttpContext();
        //    context.Request.Headers["Authorization"] = authHeader;


        //    var tokenModel = new TokenModel { Request = context };


        //    var expectedResponse = new JsonModel
        //    {
        //        Data = new { Data = "" },
        //        Message = StatusMessage.LoginSuccessfully,
        //        StatusCode = (int)HttpStatusCode.OK
        //    };

        //    _authServiceMock.Setup(s => s.GetUserByToken(token, It.IsAny<TokenModel>()))
        //             .ReturnsAsync(expectedResponse);


        //    _controller.ControllerContext = new ControllerContext()
        //    {
        //        HttpContext = context
        //    };

        //    var result = await _controller.GetUserByToken();

        //    var okResult = Assert.IsType<OkObjectResult>(result);
        //    var jsonModel = Assert.IsType<JsonModel>(okResult.Value);
        //    Assert.Null(jsonModel);
        //}




    }

}

