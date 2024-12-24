using System.Collections.Generic;

namespace Hian.DirtyFlag
{
    /// <summary>
    /// 값의 변경 이력을 관리하는 인터페이스입니다.
    /// 이 인터페이스는 값의 변경 사항을 시간 순서대로 저장하고,
    /// 특정 시점의 값으로 되돌아갈 수 있는 기능을 제공합니다.
    /// </summary>
    /// <typeparam name="T">관리할 값의 타입</typeparam>
    /// <remarks>
    /// 주요 기능:
    /// - 값 변경 이력 저장
    /// - 이력 조회
    /// - 특정 시점으로 되돌리기
    /// - 이력 관리 (최대 개수 제한, 초기화)
    ///
    /// 사용 예시:
    /// <code>
    /// var dirty = new HistoryDirty&lt;int&gt;(0);
    /// dirty.Value = 10;
    /// dirty.Value = 20;
    ///
    /// // 이력 확인
    /// foreach (var value in dirty.History)
    /// {
    ///     Console.WriteLine(value);
    /// }
    ///
    /// // 첫 번째 값으로 되돌리기
    /// dirty.RevertToHistory(0);
    /// </code>
    /// </remarks>
    public interface IDirtyHistory<T>
    {
        /// <summary>
        /// 저장된 모든 값의 이력변경 시간 순서대로 반환합니다.
        /// 가장 오래된 값이 첫 번째 요소입니다.
        /// </summary>
        IReadOnlyList<T> History { get; }

        /// <summary>
        /// 이력이 하나 이상 존재하는지 여부를 나타냅니다.
        /// </summary>
        bool HasHistory { get; }

        /// <summary>
        /// 모든 이력을 제거하고 현재 값만 남깁니다.
        /// </summary>
        void ClearHistory();

        /// <summary>
        /// 지정된 인덱스의 이력으로 현재 값을 되돌립니다.
        /// </summary>
        /// <param name="historyIndex">되돌리고자 하는 이력의 인덱스</param>
        /// <returns>되돌리기 성공 여부</returns>
        bool RevertToHistory(int historyIndex);

        /// <summary>
        /// 저장할 최대 이력 개수를 설정하거나 가져옵니다.
        /// 이력이 최대 개수를 초과하면 가장 오래된 이력부터 제거됩니다.
        /// </summary>
        int MaxHistoryCount { get; set; }
    }
}
