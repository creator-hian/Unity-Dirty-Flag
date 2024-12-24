using System.Collections.Generic;
using Hian.DirtyFlag;
using NUnit.Framework;

public class DirtyBaseTests
{
    private DirtyBase<int> _dirtyInt;
    private DirtyBase<string> _dirtyString;
    private DirtyBase<TestClass> _dirtyClass;

    private class TestClass
    {
        public int Value { get; set; }

        public override bool Equals(object obj) => obj is TestClass other && Value == other.Value;

        public override int GetHashCode() => Value.GetHashCode();
    }

    [SetUp]
    public void Setup()
    {
        _dirtyInt = new DirtyBase<int>(42);
        _dirtyString = new DirtyBase<string>("초기값");
        _dirtyClass = new DirtyBase<TestClass>(new TestClass { Value = 1 });
    }

    [Test]
    public void Value_WhenChanged_SetsDirtyFlag()
    {
        // Arrange
        Assert.IsFalse(_dirtyInt.IsDirty);

        // Act
        _dirtyInt.Value = 100;

        // Assert
        Assert.IsTrue(_dirtyInt.IsDirty);
        Assert.AreEqual(100, _dirtyInt.Value);
    }

    [Test]
    public void Value_WithCustomComparer_UsesComparerForEquality()
    {
        // Arrange
        var comparer = new CustomIntComparer();
        var dirtyWithComparer = new DirtyBase<int>(10, comparer);

        // Act
        dirtyWithComparer.Value = 11; // 차이가 1이면 같은 값으로 취급

        // Assert
        Assert.IsFalse(dirtyWithComparer.IsDirty);
    }

    [Test]
    public void Commit_UpdatesOriginalValue()
    {
        // Arrange
        _dirtyString.Value = "새로운 값";

        // Act
        _dirtyString.Commit();

        // Assert
        Assert.AreEqual("새로운 값", _dirtyString.OriginalValue);
        Assert.IsFalse(_dirtyString.IsDirty);
    }

    [Test]
    public void Revert_RestoresOriginalValue()
    {
        // Arrange
        var originalValue = _dirtyClass.Value;
        _dirtyClass.Value = new TestClass { Value = 999 };

        // Act
        _dirtyClass.Revert();

        // Assert
        Assert.AreEqual(originalValue.Value, _dirtyClass.Value.Value);
        Assert.IsFalse(_dirtyClass.IsDirty);
    }

    [Test]
    public void OnValueChanged_ProvidesBothValues()
    {
        // Arrange
        int? oldValue = null;
        int? newValue = null;
        _dirtyInt.OnValueChanged += (o, n) =>
        {
            oldValue = o;
            newValue = n;
        };

        // Act
        _dirtyInt.Value = 100;

        // Assert
        Assert.AreEqual(42, oldValue);
        Assert.AreEqual(100, newValue);
    }

    [Test]
    public void Reset_InitializesNewValue()
    {
        // Arrange
        _dirtyInt.Value = 100;
        Assert.IsTrue(_dirtyInt.IsDirty);

        // Act
        _dirtyInt.Reset(200);

        // Assert
        Assert.AreEqual(200, _dirtyInt.Value);
        Assert.AreEqual(200, _dirtyInt.OriginalValue);
        Assert.IsFalse(_dirtyInt.IsDirty);
    }

    private class CustomIntComparer : IEqualityComparer<int>
    {
        public bool Equals(int x, int y) => System.Math.Abs(x - y) <= 1;

        public int GetHashCode(int obj) => obj.GetHashCode();
    }
}
