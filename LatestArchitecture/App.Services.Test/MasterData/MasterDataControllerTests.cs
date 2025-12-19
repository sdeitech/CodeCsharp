using App.Api.Controllers.MasterData;
using App.Application.Dto.MasterCountry;
using App.Application.Interfaces.Services.MasterData;
using App.Common.Constant;
using App.Common.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Moq;

namespace App.Services.Test.MasterData
{
    public class MasterDataControllerTests
    {
        private readonly Mock<IMasterDataService> _mockMasterDataService;
        private readonly MasterDataController _controller;

        public MasterDataControllerTests()
        {
            _mockMasterDataService = new Mock<IMasterDataService>();
            _controller = new MasterDataController(_mockMasterDataService.Object);
            _controller.ControllerContext.HttpContext = new DefaultHttpContext();
        }

        #region MasterData


        [Fact]
        public async Task GetMasterDataAsync_ShouldReturnOk_WhenDataExists()
        {
            // Arrange
            var key = "country";
            var data = new Dictionary<string, object>
            {
                { "country", new List<object> { new { CountryID = 1, CountryName = "India" } } }
            };

            var jsonModel = new JsonModel
            {
                Data = data,
                StatusCode = StatusCodes.Status200OK
            };

            _mockMasterDataService
                .Setup(s => s.GetMasterDataAsync(key))
                .ReturnsAsync(jsonModel);

            // Act
            var result = await _controller.GetMasterDataAsync(key);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            var response = Assert.IsType<JsonModel>(okResult.Value);
            Assert.NotNull(response.Data);
            Assert.Equal(StatusCodes.Status200OK, response.StatusCode);
        }

        [Fact]
        public async Task GetMasterDataAsync_ShouldReturnBadRequest_WhenKeyIsInvalid()
        {
            // Arrange
            var key = "invalid";
            var jsonModel = new JsonModel
            {
                Message = "Invalid key",
                StatusCode = StatusCodes.Status400BadRequest
            };

            _mockMasterDataService
                .Setup(s => s.GetMasterDataAsync(key))
                .ReturnsAsync(jsonModel);

            // Act
            var result = await _controller.GetMasterDataAsync(key);

            // Assert
            var badRequestResult = Assert.IsType<BadRequestObjectResult>(result);
            var response = Assert.IsType<JsonModel>(badRequestResult.Value);
            Assert.Equal(StatusCodes.Status400BadRequest, response.StatusCode);
        }

        [Fact]
        public async Task GetMasterDataAsync_ShouldReturnInternalServerError_WhenNoDataFound()
        {
            // Arrange
            var key = "country";
            var jsonModel = new JsonModel
            {
                Message = StatusMessage.InternalServerError,
                StatusCode = StatusCodes.Status500InternalServerError
            };

            _mockMasterDataService
                .Setup(s => s.GetMasterDataAsync(key))
                .ReturnsAsync(jsonModel);

            // Act
            var result = await _controller.GetMasterDataAsync(key);

            // Assert
            var serverErrorResult = Assert.IsType<ObjectResult>(result);
            Assert.Equal(StatusCodes.Status500InternalServerError, serverErrorResult.StatusCode);
            var response = Assert.IsType<JsonModel>(serverErrorResult.Value);
            Assert.Equal(StatusCodes.Status500InternalServerError, response.StatusCode);
        }

        [Fact]
        public async Task GetMasterDataAsync_ShouldThrowException_WhenServiceFails()
        {
            // Arrange
            var key = "country";
            _mockMasterDataService
                .Setup(s => s.GetMasterDataAsync(key))
                .ThrowsAsync(new Exception("DB connection failed"));

            // Act & Assert
            await Assert.ThrowsAsync<Exception>(() => _controller.GetMasterDataAsync(key));
        }

        #endregion

        #region MasterState

        [Fact]
        public async Task GetMasterStateAsync_ShouldReturnOk_WhenDataExists()
        {
            // Arrange
            int countryId = 1;
            var states = new List<MasterStateResponseDto>
            {
                new MasterStateResponseDto { StateID = 1, StateName = "Punjab" },
                new MasterStateResponseDto { StateID = 2, StateName = "Haryana" }
            };

            var jsonModel = new JsonModel
            {
                Data = states,
                StatusCode = StatusCodes.Status200OK
            };

            _mockMasterDataService
                .Setup(s => s.GetMasterStateAsync(countryId))
                .ReturnsAsync(jsonModel);

            // Act
            var result = await _controller.GetMasterStateAsync(countryId);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            var response = Assert.IsType<JsonModel>(okResult.Value);
            Assert.NotNull(response.Data);
            Assert.Equal(StatusCodes.Status200OK, response.StatusCode);
        }

        [Fact]
        public async Task GetMasterStateAsync_ShouldReturnBadRequest_WhenCountryIdIsInvalid()
        {
            // Arrange
            int countryId = -1; // invalid
            var jsonModel = new JsonModel
            {
                Message = "Invalid CountryId",
                StatusCode = StatusCodes.Status400BadRequest
            };

            _mockMasterDataService
                .Setup(s => s.GetMasterStateAsync(countryId))
                .ReturnsAsync(jsonModel);

            // Act
            var result = await _controller.GetMasterStateAsync(countryId);

            // Assert
            var badRequestResult = Assert.IsType<BadRequestObjectResult>(result);
            var response = Assert.IsType<JsonModel>(badRequestResult.Value);
            Assert.Equal(StatusCodes.Status400BadRequest, response.StatusCode);
        }

        [Fact]
        public async Task GetMasterStateAsync_ShouldReturnInternalServerError_WhenNoStatesFound()
        {
            // Arrange
            int countryId = 99; // valid but no data
            var jsonModel = new JsonModel
            {
                Message = StatusMessage.InternalServerError,
                StatusCode = StatusCodes.Status500InternalServerError
            };

            _mockMasterDataService
                .Setup(s => s.GetMasterStateAsync(countryId))
                .ReturnsAsync(jsonModel);

            // Act
            var result = await _controller.GetMasterStateAsync(countryId);

            // Assert
            var serverErrorResult = Assert.IsType<ObjectResult>(result);
            Assert.Equal(StatusCodes.Status500InternalServerError, serverErrorResult.StatusCode);
            var response = Assert.IsType<JsonModel>(serverErrorResult.Value);
            Assert.Equal(StatusCodes.Status500InternalServerError, response.StatusCode);
        }

        [Fact]
        public async Task GetMasterStateAsync_ShouldThrowException_WhenServiceFails()
        {
            // Arrange
            int countryId = 1;
            _mockMasterDataService
                .Setup(s => s.GetMasterStateAsync(countryId))
                .ThrowsAsync(new Exception("Database connection failed"));

            // Act & Assert
            await Assert.ThrowsAsync<Exception>(() => _controller.GetMasterStateAsync(countryId));
        }

        #endregion
    }
}
