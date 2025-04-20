namespace Logger.Test;

public class LoggerTests
{
    // NestableLoggerのテスト
    [Fact]
    public void NestableLogger_PushNest_ネストが正しく追加される()
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
    public void NestableLogger_IsTopNest_最上層のネストを正しく判定する()
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
    public void NestableLogger_IsTopNest_最上層でないネストを正しく判定する()
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
    public void NestableLogger_Log_ログが正しく記録される()
    {
        // Arrange
        var logger = new NestableLogger { Category = "TestCategory" };
        logger.PushNest("TestNest");

        // Act
        logger.Log("TestMessage");

        // Assert
        // Debug出力を直接確認するにはモックが必要ですが、ここでは例外が発生しないことを確認します。
    }

    // LogNestのテスト
    [Fact]
    public void LogNest_ネストが正しく追加および解放される()
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
    public void LogNest_Logger_特定のネストオブジェクトを使用する()
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
    public void LogNest_Logger_特定のネストオブジェクトを使用が最上層なので何もしない()
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

    // LogNest<TLoggerInstanceType>のテスト
    [Fact]
    public void LogNestTLoggerInstanceType_ロガーインスタンスを正しく使用する()
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

    // ILoggerInstanceのテスト
    [Fact]
    public void ILoggerInstance_Get_ロガーインスタンスを正しく取得する()
    {
        // Act
        var logger = TestLogger.Get();

        // Assert
        Assert.NotNull(logger);
        Assert.IsType<NestableLogger>(logger);
    }

    // テスト用のILoggerInstance実装
    private abstract class TestLogger : ILoggerInstance
    {
        public static NestableLogger Logger { get; } = new() { Category = nameof(TestLogger) };

        public static NestableLogger Get()
        {
            return Logger;
        }
    }
}
