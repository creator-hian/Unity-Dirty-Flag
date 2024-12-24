using System;

namespace Hian.DirtyFlag
{
    /// <summary>
    /// 제네릭을 사용하지 않는 기본 Dirty 인터페이스입니다.
    /// 모든 값을 object 타입으로 처리합니다.
    /// </summary>
    public interface IDirtyObject
    {
        /// <summary>
        /// 현재 값을 가져오거나 설정합니다.
        /// </summary>
        object Value { get; set; }

        /// <summary>
        /// 원본 값을 가져옵니다.
        /// </summary>
        object OriginalValue { get; }

        /// <summary>
        /// 값이 변경되었는지 여부를 나타냅니다.
        /// </summary>
        bool IsDirty { get; }

        /// <summary>
        /// 값이 변경될 때 발생하는 이벤트입니다.
        /// </summary>
        event Action<object, object> OnValueChanged;

        /// <summary>
        /// 변경 사항이 커밋될 때 발생하는 이벤트입니다.
        /// </summary>
        event Action OnCommit;

        /// <summary>
        /// 값이 원본 값으로 되돌려질 때 발생하는 이벤트입니다.
        /// </summary>
        event Action OnRevert;

        /// <summary>
        /// 현재 값을 원본 값으로 커밋합니다.
        /// </summary>
        void Commit();

        /// <summary>
        /// 현재 값을 원본 값으로 되돌립니다.
        /// </summary>
        void Revert();

        /// <summary>
        /// 강제로 Dirty 상태로 설정합니다.
        /// </summary>
        void ForceSetDirty();

        /// <summary>
        /// 값을 초기화합니다.
        /// </summary>
        /// <param name="value">새로운 초기값</param>
        void Reset(object value);
    }
}
