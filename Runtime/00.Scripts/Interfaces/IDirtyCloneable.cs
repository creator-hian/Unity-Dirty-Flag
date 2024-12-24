namespace Hian.DirtyFlag
{
    /// <summary>
    /// Dirty 객체의 복제 및 비교 기능을 제공하는 인터페이스입니다.
    /// 이 인터페이스는 Dirty 객체의 상태를 복제하고 다른 객체와 비교하는 기능을 제공합니다.
    /// </summary>
    /// <typeparam name="T">관리할 값의 타입</typeparam>
    /// <remarks>
    /// 주요 기능:
    /// - 객체 복제
    /// - 값 비교
    ///
    /// 사용 예시:
    /// <code>
    /// var original = new ExtendedDirty&lt;int&gt;(10);
    /// var clone = original.Clone();
    ///
    /// original.Value = 20;
    ///
    /// bool areEqual = original.IsEqual(clone); // false
    /// </code>
    /// </remarks>
    public interface IDirtyCloneable<T>
    {
        /// <summary>
        /// 현재 Dirty 객체의 상태를 복제합니다.
        /// 복제된 객체는 현재 객체와 동일한 값을 가지지만 독립적으로 동작합니다.
        /// </summary>
        /// <returns>복제된 새로운 Dirty 객체</returns>
        IDirty<T> Clone();

        /// <summary>
        /// 현재 객체의 값을 다른 Dirty 객체의 값과 비교합니다.
        /// </summary>
        /// <param name="other">비교할 다른 Dirty 객체</param>
        /// <returns>값이 동일한지 여부</returns>
        bool IsEqual(IDirty<T> other);
    }
}
