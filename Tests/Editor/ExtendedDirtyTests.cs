using System;
using System.Linq;
using Hian.DirtyFlag;
using NUnit.Framework;

public class ExtendedDirtyTests
{
    private ExtendedDirty<TestData> _extendedDirty;

    private class TestData
    {
        public int Value { get; set; }
        public string Name { get; set; }

        public override bool Equals(object obj)
        {
            if (obj is TestData other)
                return Value == other.Value && Name == other.Name;
            return false;
        }

        public override int GetHashCode() => HashCode.Combine(Value, Name);
    }

    [SetUp]
    public void Setup()
    {
        _extendedDirty = new ExtendedDirty<TestData>(new TestData { Value = 1, Name = "초기값" });
    }

    [Test]
    public void TrySetValue_WithDifferentValue_ReturnsTrue()
    {
        // Arrange
        var newData = new TestData { Value = 2, Name = "새로운 값" };

        // Act
        bool result = _extendedDirty.TrySetValue(newData);

        // Assert
        Assert.IsTrue(result);
        Assert.IsTrue(_extendedDirty.IsDirty);
        Assert.AreEqual(newData.Value, _extendedDirty.Value.Value);
        Assert.AreEqual(newData.Name, _extendedDirty.Value.Name);
    }

    [Test]
    public void TrySetValue_WithSameValue_ReturnsFalse()
    {
        // Arrange
        var sameData = new TestData { Value = 1, Name = "초기값" };

        // Act
        bool result = _extendedDirty.TrySetValue(sameData);

        // Assert
        Assert.IsFalse(result);
        Assert.IsFalse(_extendedDirty.IsDirty);
    }

    [Test]
    public void TryCommit_WhenDirty_ReturnsTrue()
    {
        // Arrange
        _extendedDirty.Value = new TestData { Value = 2, Name = "새로운 값" };

        // Act
        bool result = _extendedDirty.TryCommit();

        // Assert
        Assert.IsTrue(result);
        Assert.IsFalse(_extendedDirty.IsDirty);
    }

    [Test]
    public void TryCommit_WhenNotDirty_ReturnsFalse()
    {
        // Act
        bool result = _extendedDirty.TryCommit();

        // Assert
        Assert.IsFalse(result);
    }

    [Test]
    public void BatchUpdate_ExecutesAllOperations()
    {
        // Act
        var result = _extendedDirty.BatchUpdate(data =>
        {
            data.Value = 100;
            data.Name = "배치 업데이트";
        });

        // Assert
        Assert.IsTrue(result.IsSuccess);
        Assert.AreEqual(100, _extendedDirty.Value.Value);
        Assert.AreEqual("배치 업데이트", _extendedDirty.Value.Name);
    }

    [Test]
    public void BatchUpdate_WithException_ReturnsFalseResult()
    {
        // Act
        var result = _extendedDirty.BatchUpdate(_ => throw new Exception("테스트 예외"));

        // Assert
        Assert.IsFalse(result.IsSuccess);
        Assert.IsNotNull(result.Error);
        Assert.AreEqual("테스트 예외", result.Error.Message);
    }

    [Test]
    public void BatchContext_ExecutesOperationsInOrder()
    {
        // Arrange
        var context = _extendedDirty
            .CreateBatchContext()
            .Add(data => data.Value = 10)
            .Add(data => data.Name = "첫번째")
            .Add(data => data.Value *= 2);

        // Act
        var result = context.Execute();

        // Assert
        Assert.IsTrue(result.IsSuccess);
        Assert.AreEqual(20, _extendedDirty.Value.Value);
        Assert.AreEqual("첫번째", _extendedDirty.Value.Name);
    }

    [Test]
    public void Clone_CreatesCopyWithSameValues()
    {
        // Arrange
        _extendedDirty.Value = new TestData { Value = 42, Name = "테스트" };

        // Act
        var clone = _extendedDirty.Clone();

        // Assert
        Assert.AreEqual(_extendedDirty.Value.Value, ((TestData)clone.Value).Value);
        Assert.AreEqual(_extendedDirty.Value.Name, ((TestData)clone.Value).Name);
        Assert.AreEqual(_extendedDirty.IsDirty, clone.IsDirty);
    }

    [Test]
    public void IsEqual_WithEqualValues_ReturnsTrue()
    {
        // Arrange
        var other = new ExtendedDirty<TestData>(new TestData { Value = 1, Name = "초기값" });

        // Act
        bool result = _extendedDirty.IsEqual(other);

        // Assert
        Assert.IsTrue(result);
    }

    [Test]
    public void IsEqual_WithDifferentValues_ReturnsFalse()
    {
        // Arrange
        var other = new ExtendedDirty<TestData>(new TestData { Value = 999, Name = "다른 값" });

        // Act
        bool result = _extendedDirty.IsEqual(other);

        // Assert
        Assert.IsFalse(result);
    }

    [Test]
    public void BatchUpdate_WithSuppressEvents_DoesNotTriggerEvents()
    {
        // Arrange
        bool eventTriggered = false;
        _extendedDirty.OnValueChanged += (_, _) => eventTriggered = true;

        // Act
        _extendedDirty.BatchUpdate(data => data.Value = 999, suppressEvents: true);

        // Assert
        Assert.IsFalse(eventTriggered);
        Assert.AreEqual(999, _extendedDirty.Value.Value);
    }

    [Test]
    public void BatchContext_WithSuppressEvents_DoesNotTriggerEvents()
    {
        // Arrange
        bool eventTriggered = false;
        _extendedDirty.OnValueChanged += (_, _) => eventTriggered = true;

        var context = _extendedDirty
            .CreateBatchContext()
            .Add(data => data.Value = 100)
            .SuppressEvents();

        // Act
        context.Execute();

        // Assert
        Assert.IsFalse(eventTriggered);
        Assert.AreEqual(100, _extendedDirty.Value.Value);
    }
}
