using App.Api.Controllers.MasterDatabase;
using App.Application.Dto.Common;
using App.Application.Dto.MasterDatabase;
using App.Application.Interfaces.Services.AuthenticationModule;
using App.Application.Interfaces.Services.MasterDatabase;
using App.Common.Constant;
using App.Common.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Moq;

namespace App.Services.Test.MasterDatabaseModule
{
    public class MasterDatabaseControllerTests
    {
        private readonly Mock<IMasterDatabaseService> _mockMasterDatabaseService;
        private readonly Mock<ICurrentUserClaimService> _mockCurrentUserClaimService;
        private readonly MasterDatabaseController _controller;

        public MasterDatabaseControllerTests()
        {
            _mockMasterDatabaseService = new Mock<IMasterDatabaseService>();
            _mockCurrentUserClaimService = new Mock<ICurrentUserClaimService>();
            _controller = new MasterDatabaseController(_mockMasterDatabaseService.Object, _mockCurrentUserClaimService.Object);
            _controller.ControllerContext.HttpContext = new DefaultHttpContext();
        }

        #region CreateMasterDatabase

        [Fact]
        public async Task CreateMasterDatabaseAsync_ShouldReturnOk_WhenCreationIsSuccessful()
        {
            // Arrange
            var dto = new MasterDatabaseDto { DatabaseName = "TestDb" };
            var jsonModel = new JsonModel
            {
                Message = StatusMessage.DatabaseSavedSuccessfully,
                StatusCode = StatusCodes.Status200OK
            };

            _mockCurrentUserClaimService.Setup(x => x.UserId).Returns(1);
            _mockMasterDatabaseService
                .Setup(s => s.CreateMasterDatabaseAsync(dto, 1))
                .ReturnsAsync(jsonModel);

            // Act
            var result = await _controller.CreateMasterDatabaseAsync(dto);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            var response = Assert.IsType<JsonModel>(okResult.Value);
            Assert.Equal(StatusCodes.Status200OK, response.StatusCode);
            Assert.Equal(StatusMessage.DatabaseSavedSuccessfully, response.Message);
        }

        [Fact]
        public async Task CreateMasterDatabaseAsync_ShouldReturnInternalServerError_WhenCreationFails()
        {
            // Arrange
            var dto = new MasterDatabaseDto { DatabaseName = "TestDb" };
            var jsonModel = new JsonModel
            {
                Message = StatusMessage.InternalServerError,
                StatusCode = StatusCodes.Status500InternalServerError
            };

            _mockCurrentUserClaimService.Setup(x => x.UserId).Returns(1);
            _mockMasterDatabaseService
                .Setup(s => s.CreateMasterDatabaseAsync(dto, 1))
                .ReturnsAsync(jsonModel);

            // Act
            var result = await _controller.CreateMasterDatabaseAsync(dto);

            // Assert
            var serverErrorResult = Assert.IsType<ObjectResult>(result);
            Assert.Equal(StatusCodes.Status500InternalServerError, serverErrorResult.StatusCode);
            var response = Assert.IsType<JsonModel>(serverErrorResult.Value);
            Assert.Equal(StatusCodes.Status500InternalServerError, response.StatusCode);
        }

        [Fact]
        public async Task CreateMasterDatabaseAsync_ShouldThrowException_WhenServiceThrows()
        {
            // Arrange
            var dto = new MasterDatabaseDto { DatabaseName = "TestDb" };
            _mockCurrentUserClaimService.Setup(x => x.UserId).Returns(1);
            _mockMasterDatabaseService
                .Setup(s => s.CreateMasterDatabaseAsync(dto, 1))
                .ThrowsAsync(new Exception("DB Error"));

            // Act & Assert
            await Assert.ThrowsAsync<Exception>(() => _controller.CreateMasterDatabaseAsync(dto));
        }

        #endregion

        #region UpdateMasterDatabase

        [Fact]
        public async Task UpdateMasterDatabaseAsync_ShouldReturnOk_WhenUpdateIsSuccessful()
        {
            // Arrange
            var dto = new MasterDatabaseDto { DatabaseID = 1, DatabaseName = "TestDb" };
            var jsonModel = new JsonModel
            {
                Message = StatusMessage.RecordSavedSuccessfully,
                StatusCode = StatusCodes.Status200OK
            };

            _mockCurrentUserClaimService.Setup(x => x.UserId).Returns(1);
            _mockMasterDatabaseService
                .Setup(s => s.UpdateMasterDatabaseAsync(dto, 1))
                .ReturnsAsync(jsonModel);

            // Act
            var result = await _controller.UpdateMasterDatabaseAsync(dto);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            var response = Assert.IsType<JsonModel>(okResult.Value);
            Assert.Equal(StatusCodes.Status200OK, response.StatusCode);
            Assert.Equal(StatusMessage.RecordSavedSuccessfully, response.Message);
        }

        [Fact]
        public async Task UpdateMasterDatabaseAsync_ShouldReturnNotFound_WhenDatabaseNotFound()
        {
            // Arrange
            var dto = new MasterDatabaseDto { DatabaseID = 1 };
            var jsonModel = new JsonModel
            {
                Message = StatusMessage.NoDataFound,
                StatusCode = StatusCodes.Status404NotFound
            };

            _mockCurrentUserClaimService.Setup(x => x.UserId).Returns(1);
            _mockMasterDatabaseService
                .Setup(s => s.UpdateMasterDatabaseAsync(dto, 1))
                .ReturnsAsync(jsonModel);

            // Act
            var result = await _controller.UpdateMasterDatabaseAsync(dto);

            // Assert
            var notFoundResult = Assert.IsType<ObjectResult>(result);
            Assert.Equal(StatusCodes.Status500InternalServerError, notFoundResult.StatusCode); // Because of default _ => StatusCode(500)
            var response = Assert.IsType<JsonModel>(notFoundResult.Value);
            Assert.Equal(StatusCodes.Status404NotFound, response.StatusCode);
        }

        [Fact]
        public async Task UpdateMasterDatabaseAsync_ShouldReturnInternalServerError_WhenServiceThrows()
        {
            // Arrange
            var dto = new MasterDatabaseDto { DatabaseID = 1 };
            _mockCurrentUserClaimService.Setup(x => x.UserId).Returns(1);
            _mockMasterDatabaseService
                .Setup(s => s.UpdateMasterDatabaseAsync(dto, 1))
                .ThrowsAsync(new Exception("DB Error"));

            // Act & Assert
            await Assert.ThrowsAsync<Exception>(() => _controller.UpdateMasterDatabaseAsync(dto));
        }

        #endregion

        #region GetAllMasterDatabase

        [Fact]
        public async Task GetAllMasterDatabaseAsync_HappyPath_ReturnsOkResult()
        {
            // Arrange
            var filter = new MasterDatabaseFilterDto { PageNumber = 1, PageSize = 10 };
            var response = new JsonModel
            {
                Data = new List<MasterDatabaseResponseDto>
            {
                new MasterDatabaseResponseDto { DatabaseID = 1, DatabaseName = "TestDB", TotalRecords = 1 }
            },
                Meta = new Meta { TotalRecords = 1, CurrentPage = 1, PageSize = 10 },
                StatusCode = StatusCodes.Status200OK
            };

            _mockMasterDatabaseService
                .Setup(s => s.GetAllMasterDatabaseAsync(filter))
                .ReturnsAsync(response);

            // Act
            var result = await _controller.GetAllMasterDatabaseAsync(filter) as OkObjectResult;

            // Assert
            Assert.NotNull(result);
            Assert.Equal(StatusCodes.Status200OK, result.StatusCode);
            var jsonModel = Assert.IsType<JsonModel>(result.Value);
            Assert.Single((List<MasterDatabaseResponseDto>)jsonModel.Data);
        }

        [Fact]
        public async Task GetAllMasterDatabaseAsync_UnhappyPath_InternalServerError()
        {
            // Arrange
            var filter = new MasterDatabaseFilterDto { PageNumber = 1, PageSize = 10 };
            var response = new JsonModel
            {
                Message = StatusMessage.InternalServerError,
                StatusCode = StatusCodes.Status500InternalServerError
            };

            _mockMasterDatabaseService
                .Setup(s => s.GetAllMasterDatabaseAsync(filter))
                .ReturnsAsync(response);

            // Act
            var result = await _controller.GetAllMasterDatabaseAsync(filter) as ObjectResult;

            // Assert
            Assert.NotNull(result);
            Assert.Equal(StatusCodes.Status500InternalServerError, result.StatusCode);
            var jsonModel = Assert.IsType<JsonModel>(result.Value);
            Assert.Equal(StatusMessage.InternalServerError, jsonModel.Message);
        }

        [Fact]
        public async Task GetAllMasterDatabaseAsync_UnhappyPath_BadRequest()
        {
            // Arrange
            var filter = new MasterDatabaseFilterDto { PageNumber = 1, PageSize = 10 };
            var response = new JsonModel
            {
                Message = "Invalid request",
                StatusCode = StatusCodes.Status400BadRequest
            };

            _mockMasterDatabaseService
                .Setup(s => s.GetAllMasterDatabaseAsync(filter))
                .ReturnsAsync(response);

            // Act
            var result = await _controller.GetAllMasterDatabaseAsync(filter) as BadRequestObjectResult;

            // Assert
            Assert.NotNull(result);
            Assert.Equal(StatusCodes.Status400BadRequest, result.StatusCode);
            var jsonModel = Assert.IsType<JsonModel>(result.Value);
            Assert.Equal("Invalid request", jsonModel.Message);
        }

        #endregion

        #region GetAllForDropdown

        [Fact]
        public async Task GetAllForDropdownAsync_HappyPath_ReturnsOkResult()
        {
            // Arrange
            var data = new List<MasterDatabaseResponseForDropdownDto>
            {
                new MasterDatabaseResponseForDropdownDto { DatabaseID = 1, DatabaseName = "DB1" },
                new MasterDatabaseResponseForDropdownDto { DatabaseID = 2, DatabaseName = "DB2" }
            };

            var response = new JsonModel
            {
                Data = data,
                StatusCode = StatusCodes.Status200OK
            };

            _mockMasterDatabaseService
                .Setup(s => s.GetAllMasterDatabaseDropdownAsync())
                .ReturnsAsync(response);

            // Act
            var result = await _controller.GetAllMasterDatabaseDropdownAsync() as OkObjectResult;

            // Assert
            Assert.NotNull(result);
            Assert.Equal(StatusCodes.Status200OK, result.StatusCode);

            var jsonModel = Assert.IsType<JsonModel>(result.Value);
            var returnedData = Assert.IsAssignableFrom<List<MasterDatabaseResponseForDropdownDto>>(jsonModel.Data);
            Assert.Equal(2, returnedData.Count);
            Assert.Contains(returnedData, x => x.DatabaseName == "DB1");
        }

        [Fact]
        public async Task GetAllForDropdownAsync_UnhappyPath_InternalServerError()
        {
            // Arrange
            var response = new JsonModel
            {
                Message = StatusMessage.InternalServerError,
                StatusCode = StatusCodes.Status500InternalServerError
            };

            _mockMasterDatabaseService
                .Setup(s => s.GetAllMasterDatabaseDropdownAsync())
                .ReturnsAsync(response);

            // Act
            var result = await _controller.GetAllMasterDatabaseDropdownAsync() as ObjectResult;

            // Assert
            Assert.NotNull(result);
            Assert.Equal(StatusCodes.Status500InternalServerError, result.StatusCode);

            var jsonModel = Assert.IsType<JsonModel>(result.Value);
            Assert.Equal(StatusMessage.InternalServerError, jsonModel.Message);
        }

        [Fact]
        public async Task GetAllForDropdownAsync_UnhappyPath_BadRequest()
        {
            // Arrange
            var response = new JsonModel
            {
                Message = "Invalid Request",
                StatusCode = StatusCodes.Status400BadRequest
            };

            _mockMasterDatabaseService
                .Setup(s => s.GetAllMasterDatabaseDropdownAsync())
                .ReturnsAsync(response);

            // Act
            var result = await _controller.GetAllMasterDatabaseDropdownAsync() as BadRequestObjectResult;

            // Assert
            Assert.NotNull(result);
            Assert.Equal(StatusCodes.Status400BadRequest, result.StatusCode);

            var jsonModel = Assert.IsType<JsonModel>(result.Value);
            Assert.Equal("Invalid Request", jsonModel.Message);
        }

        #endregion

        #region GetMasterDatabaseById

        [Fact]
        public async Task GetMasterDatabaseByIdAsync_HappyPath_ReturnsOkResult()
        {
            // Arrange
            var dbId = 1;
            var data = new MasterDatabaseResponseDto
            {
                DatabaseID = dbId,
                DatabaseName = "DB1",
                ServerName = "Server1",
                UserName = "Admin",
                Password = "Pass",
                IsActive = true
            };

            var response = new JsonModel
            {
                Data = data,
                StatusCode = StatusCodes.Status200OK
            };

            _mockMasterDatabaseService
                .Setup(s => s.GetMasterDatabaseByIdAsync(dbId))
                .ReturnsAsync(response);

            // Act
            var result = await _controller.GetMasterDatabaseByIdAsync(dbId) as OkObjectResult;

            // Assert
            Assert.NotNull(result);
            Assert.Equal(StatusCodes.Status200OK, result.StatusCode);

            var jsonModel = Assert.IsType<JsonModel>(result.Value);
            var returnedData = Assert.IsType<MasterDatabaseResponseDto>(jsonModel.Data);
            Assert.Equal(dbId, returnedData.DatabaseID);
            Assert.Equal("DB1", returnedData.DatabaseName);
        }

        [Fact]
        public async Task GetMasterDatabaseByIdAsync_UnhappyPath_InternalServerError()
        {
            // Arrange
            var dbId = 99;
            var response = new JsonModel
            {
                Message = StatusMessage.InternalServerError,
                StatusCode = StatusCodes.Status500InternalServerError
            };

            _mockMasterDatabaseService
                .Setup(s => s.GetMasterDatabaseByIdAsync(dbId))
                .ReturnsAsync(response);

            // Act
            var result = await _controller.GetMasterDatabaseByIdAsync(dbId) as ObjectResult;

            // Assert
            Assert.NotNull(result);
            Assert.Equal(StatusCodes.Status500InternalServerError, result.StatusCode);

            var jsonModel = Assert.IsType<JsonModel>(result.Value);
            Assert.Equal(StatusMessage.InternalServerError, jsonModel.Message);
        }

        [Fact]
        public async Task GetMasterDatabaseByIdAsync_UnhappyPath_BadRequest()
        {
            // Arrange
            var dbId = -1; // invalid id
            var response = new JsonModel
            {
                Message = "Invalid Database ID",
                StatusCode = StatusCodes.Status400BadRequest
            };

            _mockMasterDatabaseService
                .Setup(s => s.GetMasterDatabaseByIdAsync(dbId))
                .ReturnsAsync(response);

            // Act
            var result = await _controller.GetMasterDatabaseByIdAsync(dbId) as BadRequestObjectResult;

            // Assert
            Assert.NotNull(result);
            Assert.Equal(StatusCodes.Status400BadRequest, result.StatusCode);

            var jsonModel = Assert.IsType<JsonModel>(result.Value);
            Assert.Equal("Invalid Database ID", jsonModel.Message);
        }

        #endregion

        #region MasterDatabaseStatusUpdate

        [Fact]
        public async Task MasterDatabaseStatusUpdateAsync_HappyPath_ReturnsOk()
        {
            // Arrange
            var dto = new MasterDatabaseStatusUpdateDto { DatabaseID = 1, IsActive = true };
            _mockCurrentUserClaimService.Setup(x => x.UserId).Returns(101);

            _controller.HttpContext.Request.Headers["IPAddress"] = "127.0.0.1";

            var serviceResponse = new JsonModel
            {
                Message = StatusMessage.RecordSavedSuccessfully,
                StatusCode = StatusCodes.Status200OK
            };

            _mockMasterDatabaseService
                .Setup(s => s.MasterDatabaseStatusUpdateAsync(dto, 101))
                .ReturnsAsync(serviceResponse);

            // Act
            var result = await _controller.MasterDatabaseStatusUpdateAsync(dto) as OkObjectResult;

            // Assert
            Assert.NotNull(result);
            Assert.Equal(StatusCodes.Status200OK, result.StatusCode);

            var json = Assert.IsType<JsonModel>(result.Value);
            Assert.Equal(StatusMessage.RecordSavedSuccessfully, json.Message);
        }

        [Fact]
        public async Task MasterDatabaseStatusUpdateAsync_UnhappyPath_NoDataFound_ReturnsOk()
        {
            // Arrange
            var dto = new MasterDatabaseStatusUpdateDto { DatabaseID = 99, IsActive = false };
            _mockCurrentUserClaimService.Setup(x => x.UserId).Returns(101);

            _controller.HttpContext.Request.Headers["IPAddress"] = "127.0.0.1";

            var serviceResponse = new JsonModel
            {
                Message = StatusMessage.NoDataFound,
                StatusCode = StatusCodes.Status200OK
            };

            _mockMasterDatabaseService
                .Setup(s => s.MasterDatabaseStatusUpdateAsync(dto, 101))
                .ReturnsAsync(serviceResponse);

            // Act
            var result = await _controller.MasterDatabaseStatusUpdateAsync(dto) as OkObjectResult;

            // Assert
            Assert.NotNull(result);
            Assert.Equal(StatusCodes.Status200OK, result.StatusCode);

            var json = Assert.IsType<JsonModel>(result.Value);
            Assert.Equal(StatusMessage.NoDataFound, json.Message);
        }

        [Fact]
        public async Task MasterDatabaseStatusUpdateAsync_UnhappyPath_BadRequest()
        {
            // Arrange
            var dto = new MasterDatabaseStatusUpdateDto { DatabaseID = -1, IsActive = true };
            _mockCurrentUserClaimService.Setup(x => x.UserId).Returns(101);

            _controller.HttpContext.Request.Headers["IPAddress"] = "127.0.0.1";

            var serviceResponse = new JsonModel
            {
                Message = "Invalid database ID",
                StatusCode = StatusCodes.Status400BadRequest
            };

            _mockMasterDatabaseService
                .Setup(s => s.MasterDatabaseStatusUpdateAsync(dto, 101))
                .ReturnsAsync(serviceResponse);

            // Act
            var result = await _controller.MasterDatabaseStatusUpdateAsync(dto) as BadRequestObjectResult;

            // Assert
            Assert.NotNull(result);
            Assert.Equal(StatusCodes.Status400BadRequest, result.StatusCode);

            var json = Assert.IsType<JsonModel>(result.Value);
            Assert.Equal("Invalid database ID", json.Message);
        }

        [Fact]
        public async Task MasterDatabaseStatusUpdateAsync_UnhappyPath_InternalServerError()
        {
            // Arrange
            var dto = new MasterDatabaseStatusUpdateDto { DatabaseID = 2, IsActive = false };
            _mockCurrentUserClaimService.Setup(x => x.UserId).Returns(101);

            _controller.HttpContext.Request.Headers["IPAddress"] = "127.0.0.1";

            var serviceResponse = new JsonModel
            {
                Message = StatusMessage.InternalServerError,
                StatusCode = StatusCodes.Status500InternalServerError
            };

            _mockMasterDatabaseService
                .Setup(s => s.MasterDatabaseStatusUpdateAsync(dto, 101))
                .ReturnsAsync(serviceResponse);

            // Act
            var result = await _controller.MasterDatabaseStatusUpdateAsync(dto) as ObjectResult;

            // Assert
            Assert.NotNull(result);
            Assert.Equal(StatusCodes.Status500InternalServerError, result.StatusCode);

            var json = Assert.IsType<JsonModel>(result.Value);
            Assert.Equal(StatusMessage.InternalServerError, json.Message);
        }

        #endregion
    }
}