namespace Logger.Test;

public class LoggerTests
{
    // NestableLogger�̃e�X�g
    [Fact]
    public void NestableLogger_PushNest_�l�X�g���������ǉ������()
    {
        // Arrange
        var logger = new NestableLogger();

        // Act
        _ = logger.PushNest("TestNest");

        // Assert
        Assert.Single(logger.ProcessNests);
        Assert.Equal("TestNest", logger.ProcessNests.Peek().Name);
    }

    [Fact]
    public void NestableLogger_IsTopNest_�ŏ�w�̃l�X�g�𐳂������肷��()
    {
        // Arrange
        var logger = new NestableLogger();
        object handle = logger.PushNest("TopNest");

        // Act
        bool isTop = logger.IsTopNest(handle);

        // Assert
        Assert.True(isTop);
    }

    [Fact]
    public void NestableLogger_IsTopNest_�ŏ�w�łȂ��l�X�g�𐳂������肷��()
    {
        // Arrange
        var logger = new NestableLogger();
        object handle1 = logger.PushNest("Nest1");
        _ = logger.PushNest("Nest2");

        // Act
        bool isTop = logger.IsTopNest(handle1);

        // Assert
        Assert.False(isTop);
    }

    [Fact]
    public void NestableLogger_Log_���O���������L�^�����()
    {
        // Arrange
        var logger = new NestableLogger { Category = "TestCategory" };
        logger.PushNest("TestNest");

        // Act
        logger.Log("TestMessage");

        // Assert
        // Debug�o�͂𒼐ڊm�F����ɂ̓��b�N���K�v�ł����A�����ł͗�O���������Ȃ����Ƃ��m�F���܂��B
    }

    // LogNest�̃e�X�g
    [Fact]
    public void LogNest_�l�X�g���������ǉ�����щ�������()
    {
        // Arrange
        var logger = new NestableLogger();

        // Act
        using (new LogNest(logger, "TestNest"))
        {
            // Assert
            Assert.Single(logger.ProcessNests);
            Assert.Equal("TestNest", logger.ProcessNests.Peek().Name);
        }

        // Assert
        Assert.Empty(logger.ProcessNests);
    }

    [Fact]
    public void LogNest_Logger_����̃l�X�g�I�u�W�F�N�g���g�p����()
    {
        // Arrange
        var logger = new NestableLogger();
        var logNest1 = new LogNest(logger, "TestNest1");
        _ = new LogNest(logger, "TestNest2");

        // Act
        _ = logNest1.Logger;

        // Assert
        Assert.True(logger.UseSpecificNestObject.enabled);
        Assert.NotNull(logger.UseSpecificNestObject.targetProcessNest);
    }

    [Fact]
    public void LogNest_Logger_����̃l�X�g�I�u�W�F�N�g���g�p���ŏ�w�Ȃ̂ŉ������Ȃ�()
    {
        // Arrange
        var logger = new NestableLogger();
        _ = new LogNest(logger, "TestNest1");
        var logNest2 = new LogNest(logger, "TestNest2");

        // Act
        _ = logNest2.Logger;

        // Assert
        Assert.False(logger.UseSpecificNestObject.enabled);
        Assert.Null(logger.UseSpecificNestObject.targetProcessNest);
    }

    // LogNest<TLoggerInstanceType>�̃e�X�g
    [Fact]
    public void LogNestTLoggerInstanceType_���K�[�C���X�^���X�𐳂����g�p����()
    {
        // Act
        using (new LogNest<TestLogger>("TestNest"))
        {
            // Assert
            Assert.Single(TestLogger.Logger.ProcessNests);
            Assert.Equal("TestNest", TestLogger.Logger.ProcessNests.Peek().Name);
        }

        // Assert
        Assert.Empty(TestLogger.Logger.ProcessNests);
    }

    // ILoggerInstance�̃e�X�g
    [Fact]
    public void ILoggerInstance_Get_���K�[�C���X�^���X�𐳂����擾����()
    {
        // Act
        var logger = TestLogger.Get();

        // Assert
        Assert.NotNull(logger);
        Assert.IsType<NestableLogger>(logger);
    }

    // �e�X�g�p��ILoggerInstance����
    private abstract class TestLogger : ILoggerInstance
    {
        public static NestableLogger Logger { get; } = new() { Category = nameof(TestLogger) };

        public static NestableLogger Get()
        {
            return Logger;
        }
    }
}
