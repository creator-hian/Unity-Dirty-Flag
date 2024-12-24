using Hian.DirtyFlag;
using NUnit.Framework;
using UnityEngine;

public class DirtyTests
{
    private Dirty _dirty;

    [SetUp]
    public void Setup()
    {
        _dirty = new Dirty("초기값");
    }

    [Test]
    public void Value_WhenChanged_SetsDirtyFlag()
    {
        // Arrange
        Assert.IsFalse(_dirty.IsDirty);

        // Act
        _dirty.Value = "새로운 값";

        // Assert
        Assert.IsTrue(_dirty.IsDirty);
        Assert.AreEqual("새로운 값", _dirty.Value);
    }

    [Test]
    public void Value_WhenSetToSameValue_DoesNotSetDirtyFlag()
    {
        // Arrange
        _dirty.Value = "같은 값";
        _dirty.Commit();
        Assert.IsFalse(_dirty.IsDirty);

        // Act
        _dirty.Value = "같은 값";

        // Assert
        Assert.IsFalse(_dirty.IsDirty);
    }

    [Test]
    public void Commit_ResetsIsDirtyFlag()
    {
        // Arrange
        _dirty.Value = "새로운 값";
        Assert.IsTrue(_dirty.IsDirty);

        // Act
        _dirty.Commit();

        // Assert
        Assert.IsFalse(_dirty.IsDirty);
        Assert.AreEqual("새로운 값", _dirty.OriginalValue);
    }

    [Test]
    public void Revert_RestoresOriginalValue()
    {
        // Arrange
        _dirty.Value = "변경된 값";
        Assert.AreNotEqual(_dirty.Value, _dirty.OriginalValue);

        // Act
        _dirty.Revert();

        // Assert
        Assert.IsFalse(_dirty.IsDirty);
        Assert.AreEqual(_dirty.Value, _dirty.OriginalValue);
        Assert.AreEqual("초기값", _dirty.Value);
    }

    [Test]
    public void ForceSetDirty_SetsDirtyFlag()
    {
        // Arrange
        Assert.IsFalse(_dirty.IsDirty);

        // Act
        _dirty.ForceSetDirty();

        // Assert
        Assert.IsTrue(_dirty.IsDirty);
    }

    [Test]
    public void Reset_ResetsValueAndClearsDirtyFlag()
    {
        // Arrange
        _dirty.Value = "변경된 값";
        Assert.IsTrue(_dirty.IsDirty);

        // Act
        _dirty.Reset("새로운 초기값");

        // Assert
        Assert.IsFalse(_dirty.IsDirty);
        Assert.AreEqual("새로운 초기값", _dirty.Value);
        Assert.AreEqual("새로운 초기값", _dirty.OriginalValue);
    }

    [Test]
    public void OnValueChanged_IsInvoked_WhenValueChanges()
    {
        // Arrange
        object oldValue = null;
        object newValue = null;
        _dirty.OnValueChanged += (o, n) =>
        {
            oldValue = o;
            newValue = n;
        };

        // Act
        _dirty.Value = "새로운 값";

        // Assert
        Assert.AreEqual("초기값", oldValue);
        Assert.AreEqual("새로운 값", newValue);
    }

    [Test]
    public void OnCommit_IsInvoked_WhenCommitted()
    {
        // Arrange
        bool commitInvoked = false;
        _dirty.OnCommit += () => commitInvoked = true;
        _dirty.Value = "새로운 값";

        // Act
        _dirty.Commit();

        // Assert
        Assert.IsTrue(commitInvoked);
    }

    [Test]
    public void OnRevert_IsInvoked_WhenReverted()
    {
        // Arrange
        bool revertInvoked = false;
        _dirty.OnRevert += () => revertInvoked = true;
        _dirty.Value = "새로운 값";

        // Act
        _dirty.Revert();

        // Assert
        Assert.IsTrue(revertInvoked);
    }

    [Test]
    public void Value_CanHandleNullValues()
    {
        // Arrange
        _dirty = new Dirty(null);

        // Act & Assert
        Assert.DoesNotThrow(() => _dirty.Value = null);
        Assert.IsNull(_dirty.Value);
        Assert.IsNull(_dirty.OriginalValue);
    }

    [Test]
    public void Value_CanHandleDifferentTypes()
    {
        // Arrange
        _dirty = new Dirty();

        // Act & Assert
        Assert.DoesNotThrow(() => _dirty.Value = 42);
        Assert.AreEqual(42, _dirty.Value);

        Assert.DoesNotThrow(() => _dirty.Value = "문자열");
        Assert.AreEqual("문자열", _dirty.Value);

        Assert.DoesNotThrow(() => _dirty.Value = new Vector3(1, 2, 3));
        Assert.AreEqual(new Vector3(1, 2, 3), _dirty.Value);
    }
}
