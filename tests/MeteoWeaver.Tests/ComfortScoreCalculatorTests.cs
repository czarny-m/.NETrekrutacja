using MeteoWeaver.Api.Services;

namespace MeteoWeaver.Tests;

public class ComfortScoreCalculatorTests
{
    [Fact]
    public void Calculate_ShouldReturnHigherScore_ForBetterConditions()
    {
        var calculator = new ComfortScoreCalculator();

        var good = calculator.Calculate(21, 0, 5, 0);
        var bad = calculator.Calculate(4, 8, 40, 95);

        Assert.True(good > bad);
    }

    [Fact]
    public void Calculate_ShouldClampScore_ToRangeZeroToHundred()
    {
        var calculator = new ComfortScoreCalculator();

        var result = calculator.Calculate(-30, 50, 120, 99);

        Assert.InRange(result, 0, 100);
    }
}
