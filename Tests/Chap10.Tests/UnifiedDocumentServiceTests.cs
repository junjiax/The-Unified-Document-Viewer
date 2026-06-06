using System.Threading.Tasks;
using Moq;
using Xunit;
using Chap10.Services;
using Chap10.Services.Shared;
using Chap10.Dtos.SaleDTO;
using Chap10.Dtos.ServiceDTO;

namespace Chap10.Tests
{
    public class UnifiedDocumentServiceTests
    {
        [Fact]
        public async Task GetDocumentsByVinAsync_ReturnsBoth_WhenApisReturnData()
        {
            var saleMock = new Mock<ISaleApiClient>();
            var serviceMock = new Mock<IServiceApiClient>();
            saleMock.Setup(s => s.GetSaleDataByVinAsync(123)).ReturnsAsync(new SaleDto());
            serviceMock.Setup(s => s.GetServiceDataByVinAsync(123)).ReturnsAsync(new ServiceDto());

            var logger = new Microsoft.Extensions.Logging.Abstractions.NullLogger<UnifiedDocumentService>();
            var svc = new UnifiedDocumentService(saleMock.Object, serviceMock.Object, logger);

            var result = await svc.GetDocumentsByVinAsync(123);

            Assert.Equal(123, result.VIN);
            Assert.NotNull(result.SaleAPI);
            Assert.NotNull(result.ServiceAPI);
        }

        [Fact]
        public async Task GetDocumentsByVinAsync_HandlesSaleException_ReturnsServiceData()
        {
            var saleMock = new Mock<ISaleApiClient>();
            var serviceMock = new Mock<IServiceApiClient>();
            saleMock.Setup(s => s.GetSaleDataByVinAsync(It.IsAny<int>())).ThrowsAsync(new System.Net.Http.HttpRequestException("fail"));
            serviceMock.Setup(s => s.GetServiceDataByVinAsync(It.IsAny<int>())).ReturnsAsync(new ServiceDto());

            var logger = new Microsoft.Extensions.Logging.Abstractions.NullLogger<UnifiedDocumentService>();
            var svc = new UnifiedDocumentService(saleMock.Object, serviceMock.Object, logger);

            var result = await svc.GetDocumentsByVinAsync(999);

            Assert.Equal(999, result.VIN);
            Assert.Null(result.SaleAPI);
            Assert.NotNull(result.ServiceAPI);
        }

        [Fact]
        public async Task GetDocumentsByVinAsync_HandlesServiceTimeout_ReturnsSaleData()
        {
            var saleMock = new Mock<ISaleApiClient>();
            var serviceMock = new Mock<IServiceApiClient>();
            saleMock.Setup(s => s.GetSaleDataByVinAsync(It.IsAny<int>())).ReturnsAsync(new SaleDto());
            serviceMock.Setup(s => s.GetServiceDataByVinAsync(It.IsAny<int>())).ThrowsAsync(new TaskCanceledException());

            var logger = new Microsoft.Extensions.Logging.Abstractions.NullLogger<UnifiedDocumentService>();
            var svc = new UnifiedDocumentService(saleMock.Object, serviceMock.Object, logger);

            var result = await svc.GetDocumentsByVinAsync(555);

            Assert.Equal(555, result.VIN);
            Assert.NotNull(result.SaleAPI);
            Assert.Null(result.ServiceAPI);
        }

        [Fact]
        public async Task GetDocumentsByVinAsync_ReturnsNulls_WhenBothApisFail()
        {
            var saleMock = new Mock<ISaleApiClient>();
            var serviceMock = new Mock<IServiceApiClient>();
            saleMock.Setup(s => s.GetSaleDataByVinAsync(It.IsAny<int>())).ThrowsAsync(new System.Net.Http.HttpRequestException("fail"));
            serviceMock.Setup(s => s.GetServiceDataByVinAsync(It.IsAny<int>())).ThrowsAsync(new System.Net.Http.HttpRequestException("fail"));

            var logger = new Microsoft.Extensions.Logging.Abstractions.NullLogger<UnifiedDocumentService>();
            var svc = new UnifiedDocumentService(saleMock.Object, serviceMock.Object, logger);

            var result = await svc.GetDocumentsByVinAsync(1);

            Assert.Equal(1, result.VIN);
            Assert.Null(result.SaleAPI);
            Assert.Null(result.ServiceAPI);
        }
    }
}
