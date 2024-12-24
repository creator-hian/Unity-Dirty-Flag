using System.Linq;
using Hian.DirtyFlag;
using NUnit.Framework;
using UnityEngine;

public class HistoryDirtyTests
{
    private HistoryDirty<int> _historyDirty;
    private const int DefaultMaxHistory = 3;

    [SetUp]
    public void Setup()
    {
        _historyDirty = new HistoryDirty<int>(0, maxHistoryCount: DefaultMaxHistory);
    }

    [Test]
    public void Value_WhenChanged_AddsToHistory()
    {
        // Arrange
        Assert.AreEqual(1, _historyDirty.History.Count); // 초기값

        // Act
        _historyDirty.Value = 1;
        _historyDirty.Value = 2;

        // Assert
        Assert.AreEqual(3, _historyDirty.History.Count);
        CollectionAssert.AreEqual(new[] { 0, 1, 2 }, _historyDirty.History);
    }

    [Test]
    public void MaxHistoryCount_WhenExceeded_RemovesOldestEntries()
    {
        // Arrange & Act
        _historyDirty.Value = 1;
        _historyDirty.Value = 2;
        _historyDirty.Value = 3;
        _historyDirty.Value = 4;

        // Assert
        Assert.AreEqual(DefaultMaxHistory, _historyDirty.History.Count);
        CollectionAssert.AreEqual(new[] { 2, 3, 4 }, _historyDirty.History);
    }

    [Test]
    public void MaxHistoryCount_WhenDecreased_RemovesExcessEntries()
    {
        // Arrange
        _historyDirty.Value = 1;
        _historyDirty.Value = 2;
        _historyDirty.Value = 3;

        // Act
        _historyDirty.MaxHistoryCount = 2;

        // Assert
        Assert.AreEqual(2, _historyDirty.History.Count);
        CollectionAssert.AreEqual(new[] { 2, 3 }, _historyDirty.History);
    }

    [Test]
    public void ClearHistory_RemovesAllButCurrentValue()
    {
        // Arrange
        _historyDirty.Value = 1;
        _historyDirty.Value = 2;
        Assert.Greater(_historyDirty.History.Count, 1);

        // Act
        _historyDirty.ClearHistory();

        // Assert
        Assert.AreEqual(1, _historyDirty.History.Count);
        Assert.AreEqual(2, _historyDirty.History.Single());
    }

    [Test]
    public void RevertToHistory_WithValidIndex_ReturnsTrue()
    {
        // Arrange
        _historyDirty.Value = 10;
        _historyDirty.Value = 20;
        _historyDirty.Value = 30;

        // 이력 확인을 위한 출력
        Debug.Log($"History: [{string.Join(", ", _historyDirty.History)}]");

        // Act
        bool result = _historyDirty.RevertToHistory(0); // 초 번째 값(10)으로 되돌리기

        // Assert
        Assert.IsTrue(result);
        Assert.AreEqual(10, _historyDirty.Value); // 이력의 첫 번째 값인 10으로 되돌아가야 함
    }

    [Test]
    public void RevertToHistory_WithInvalidIndex_ReturnsFalse()
    {
        // Arrange
        _historyDirty.Value = 10;

        // Act
        bool result = _historyDirty.RevertToHistory(999);

        // Assert
        Assert.IsFalse(result);
        Assert.AreEqual(10, _historyDirty.Value);
    }

    [Test]
    public void HasHistory_ReturnsCorrectState()
    {
        // Initial state
        Assert.IsTrue(_historyDirty.HasHistory);

        // After clearing
        _historyDirty.ClearHistory();
        Assert.IsTrue(_historyDirty.HasHistory); // 현재 값이 항상 있음

        // After adding values
        _historyDirty.Value = 1;
        Assert.IsTrue(_historyDirty.HasHistory);
    }

    [Test]
    public void Reset_ClearsHistoryAndSetsNewValue()
    {
        // Arrange
        _historyDirty.Value = 1;
        _historyDirty.Value = 2;
        Assert.Greater(_historyDirty.History.Count, 1);

        // Act
        _historyDirty.Reset(100);

        // Assert
        Assert.AreEqual(1, _historyDirty.History.Count);
        Assert.AreEqual(100, _historyDirty.History.Single());
        Assert.AreEqual(100, _historyDirty.Value);
        Assert.AreEqual(100, _historyDirty.OriginalValue);
    }

    [Test]
    public void Value_WhenSetToSameValue_DoesNotAddToHistory()
    {
        // Arrange
        _historyDirty.Value = 1;
        int initialHistoryCount = _historyDirty.History.Count;

        // Act
        _historyDirty.Value = 1;

        // Assert
        Assert.AreEqual(initialHistoryCount, _historyDirty.History.Count);
    }
}
