using EasyNetQ;
using Moq;

namespace Tests;

[TestFixture]
public class PubSubExtensionsTests
{
    private Mock<IPubSub> _mockPubSub;

    [SetUp]
    public void Setup()
    {
        _mockPubSub = new Mock<IPubSub>();
    }

    [Test]
    public async Task PublishAsync_ShouldPublishMessage_WithTopic_WhenCalled()
    {
        // Arrange
        var message = new { Id = "123", FilePath = "/path/to/file", Timestamp = DateTime.UtcNow };
        var topic = "my.topic";
        var cancellationToken = CancellationToken.None;

        _mockPubSub.Setup(pubSub =>
                pubSub.PublishAsync(It.IsAny<object>(), It.IsAny<Action<IPublishConfiguration>>(), cancellationToken))
            .Returns(Task.CompletedTask);

        // Act
        await _mockPubSub.Object.PublishAsync(message, topic, cancellationToken);

        // Assert
        _mockPubSub.Verify(
            pubSub => pubSub.PublishAsync(It.IsAny<object>(), It.IsAny<Action<IPublishConfiguration>>(),
                cancellationToken), Times.Once);
    }
}
