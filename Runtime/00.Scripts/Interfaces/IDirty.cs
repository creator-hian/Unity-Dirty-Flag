using System;
using System.Collections.Generic;

namespace Hian.DirtyFlag
{
    /// <summary>
    /// 값의 변경을 추적하고 관리하는 기본 Dirty 인터페이스입니다.
    /// 이 인터페이스는 값의 변경 사항을 추적하고, 원본 값과 현재 값을 관리하며,
    /// 값 변경에 대한 이벤트를 제공합니다.
    /// </summary>
    /// <typeparam name="T">관리할 값의 타입</typeparam>
    /// <remarks>
    /// 주요 기능:
    /// - 값 변경 추적
    /// - 원본 값 관리
    /// - 변경 사항 커밋
    /// - 이전 값으로 되돌리기
    /// - 값 변경 이벤트 처리
    ///
    /// 사용 예시:
    /// <code>
    /// var dirty = new DirtyBase&lt;int&gt;(0);
    /// dirty.Value = 10; // 값 변경
    /// if (dirty.IsDirty)
    /// {
    ///     dirty.Commit(); // 변경 사항 커밋
    /// }
    /// </code>
    /// </remarks>
    public interface IDirty<T>
    {
        /// <summary>
        /// 현재 값을 가져오거나 설정합니다.
        /// 값이 변경되면 IsDirty가 true로 설정되고 OnValueChanged 이벤트가 발생합니다.
        /// </summary>
        T Value { get; set; }

        /// <summary>
        /// 마지막으로 커밋된 원본 값을 가져옵니다.
        /// 이 값은 Commit()이 호출될 때만 업데이트됩니다.
        /// </summary>
        T OriginalValue { get; }

        /// <summary>
        /// 현재 값이 원본 값과 다른지 여부를 나타냅니다.
        /// </summary>
        bool IsDirty { get; }

        /// <summary>
        /// 값이 변경될 때 발생하는 이벤트입니다.
        /// 첫 번째 매개변수는 이전 값, 두 번째 매개변수는 새로운 값입니다.
        /// </summary>
        event Action<T, T> OnValueChanged;

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
        /// 이 작업은 IsDirty를 false로 설정하고 OnCommit 이벤트를 발생시킵니다.
        /// </summary>
        void Commit();

        /// <summary>
        /// 현재 값을 원본 값으로 되돌립니다.
        /// 이 ���업은 IsDirty를 false로 설정하고 OnRevert 이벤트를 발생시킵니다.
        /// </summary>
        void Revert();

        /// <summary>
        /// 현재 상태를 강제로 Dirty로 설정합니다.
        /// 값의 실제 변경 없이 Dirty 상태로 만들어야 할 때 사용합니다.
        /// </summary>
        void ForceSetDirty();

        /// <summary>
        /// 현재 값과 원본 값을 주어진 값으로 초기화합니다.
        /// 이 작업은 IsDirty를 false로 설정합니다.
        /// </summary>
        /// <param name="value">새로운 초기값</param>
        void Reset(T value);

        /// <summary>
        /// 값 비교에 사용되는 비교자를 가져옵니다.
        /// </summary>
        IEqualityComparer<T> Comparer { get; }
    }
}
