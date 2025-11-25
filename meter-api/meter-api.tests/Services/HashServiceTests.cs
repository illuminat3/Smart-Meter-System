namespace meter_api.tests.Services;

public class HashServiceTests
{
    private readonly HashService _sut = new();

    [Fact]
    public void GetHash_SameInput_ReturnsSameHash()
    {
        // Arrange
        var input = "hello world";

        // Act
        var hash1 = _sut.GetHash(input);
        var hash2 = _sut.GetHash(input);

        // Assert
        hash1.Should().NotBeNullOrEmpty();
        hash2.Should().NotBeNullOrEmpty();
        hash1.Should().Be(hash2);
    }

    [Fact]
    public void GetHash_DifferentInputs_ReturnDifferentHashes()
    {
        // Arrange
        var input1 = "hello world";
        var input2 = "hello world!";

        // Act
        var hash1 = _sut.GetHash(input1);
        var hash2 = _sut.GetHash(input2);

        // Assert
        hash1.Should().NotBe(hash2);
    }

    [Fact]
    public void GetHash_KnownInput_ReturnsExpectedSha256Hash()
    {
        // Arrange
        var input = "test";
        var expected =
            "9f86d081884c7d659a2feaa0c55ad015" +
            "a3bf4f1b2b0b822cd15d6c15b0f00a08";

        // Act
        var hash = _sut.GetHash(input);

        // Assert
        hash.Should().Be(expected);
    }
}
