using DarkKitchen.Application.Abstractions;
using DarkKitchen.Application.Services.Audit;
using DarkKitchen.Domain.Entities;
using DarkKitchen.Domain.Enums;
using DarkKitchen.Domain.Repositories.EntityRepositories;
using Moq;

namespace DarkKitchen.Application.Test.Services;

[TestClass]
public sealed class AuditServiceTests
{
    private Mock<IAuditLogRepository> _repoMock = null!;
    private Mock<ICurrentUserContext> _currentUserMock = null!;
    private IAuditService _service = null!;

    private readonly Guid _userId = Guid.NewGuid();

    [TestInitialize]
    public void SetUp()
    {
        _repoMock = new Mock<IAuditLogRepository>();
        _currentUserMock = new Mock<ICurrentUserContext>();
        _currentUserMock.Setup(c => c.UserId).Returns(_userId);
        _currentUserMock.Setup(c => c.UserEmail).Returns("admin@darkkitchen.com");
        _service = new AuditService(_repoMock.Object, _currentUserMock.Object);
    }

    [TestMethod]
    public void Record_BuildsLogWithCurrentUserAndPersistsItOnce()
    {
        var entityId = Guid.NewGuid();
        AuditLog? captured = null;
        _repoMock.Setup(r => r.Add(It.IsAny<AuditLog>())).Callback<AuditLog>(l => captured = l);

        _service.Record(AuditAction.Created, "Product", entityId, "Product created: Pizza");

        _repoMock.Verify(r => r.Add(It.IsAny<AuditLog>()), Times.Once);
        Assert.IsNotNull(captured);
        Assert.AreEqual(AuditAction.Created, captured!.Action);
        Assert.AreEqual("Product", captured.EntityName);
        Assert.AreEqual(entityId, captured.EntityId);
        Assert.AreEqual("Product created: Pizza", captured.Description);
        Assert.AreEqual(_userId, captured.UserId);
        Assert.AreEqual("admin@darkkitchen.com", captured.UserEmail);
    }

    [TestMethod]
    public void Search_WithNoFilters_ForwardsNullsToRepository()
    {
        _repoMock.Setup(r => r.Search(null, null, null)).Returns([]);

        var result = _service.Search(new SearchAuditLogsDto());

        _repoMock.Verify(r => r.Search(null, null, null), Times.Once);
        Assert.AreEqual(0, result.Count);
    }

    [TestMethod]
    public void Search_ForwardsFiltersToRepositoryAndMapsResults()
    {
        var entityId = Guid.NewGuid();
        var log = new AuditLog(AuditAction.Deleted, "Product", entityId, "Product deleted", _userId, "admin@darkkitchen.com");

        _repoMock.Setup(r => r.Search("Product", "admin", AuditAction.Deleted)).Returns([log]);

        var dto = new SearchAuditLogsDto { EntityName = "Product", UserEmail = "admin", Action = AuditAction.Deleted };
        var result = _service.Search(dto);

        _repoMock.Verify(r => r.Search("Product", "admin", AuditAction.Deleted), Times.Once);
        Assert.AreEqual(1, result.Count);
        Assert.AreEqual(AuditAction.Deleted, result[0].Action);
        Assert.AreEqual("Product deleted", result[0].Description);
        Assert.AreEqual(entityId, result[0].EntityId);
    }

    [TestMethod]
    public void GetSummary_MapsPerActionCountsAndSumsTotal()
    {
        _repoMock.Setup(r => r.CountByAction()).Returns(new Dictionary<AuditAction, int>
        {
            [AuditAction.Created] = 124,
            [AuditAction.Updated] = 87,
            [AuditAction.Deleted] = 19
        });

        var result = _service.GetSummary();

        Assert.AreEqual(124, result.Created);
        Assert.AreEqual(87, result.Updated);
        Assert.AreEqual(19, result.Deleted);
        Assert.AreEqual(230, result.Total);
    }

    [TestMethod]
    public void GetSummary_DefaultsMissingActionsToZero()
    {
        _repoMock.Setup(r => r.CountByAction()).Returns(new Dictionary<AuditAction, int>
        {
            [AuditAction.Created] = 5
        });

        var result = _service.GetSummary();

        Assert.AreEqual(5, result.Created);
        Assert.AreEqual(0, result.Updated);
        Assert.AreEqual(0, result.Deleted);
        Assert.AreEqual(5, result.Total);
    }
}
