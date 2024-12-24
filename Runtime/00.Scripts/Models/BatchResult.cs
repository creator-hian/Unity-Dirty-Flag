using System;

namespace Hian.DirtyFlag
{
    /// <summary>
    /// 배치 작업의 결과를 나타내는 불변 구조체입니다.
    /// 이 구조체는 배치 작업의 성공/실패 여부, 작업 전후의 값,
    /// 그리고 실패 시의 예외 정보를 포함합니다.
    /// </summary>
    /// <typeparam name="T">관리되는 값의 타입</typeparam>
    /// <remarks>
    /// 주요 정보:
    /// - 작업 성공/실패 여부
    /// - 작업 전 원본 값
    /// - 작업 후 결과 값
    /// - 실패 시 예외 정보
    ///
    /// 사용 예시:
    /// <code>
    /// var result = dirty.BatchUpdate(value => {
    ///     if (value == null)
    ///         throw new ArgumentNullException();
    ///     value.Process();
    /// });
    ///
    /// if (result.IsSuccess)
    /// {
    ///     Console.WriteLine($"값이 {result.OldValue}에서 {result.NewValue}로 변경됨");
    /// }
    /// else
    /// {
    ///     Console.WriteLine($"오류 발생: {result.Error.Message}");
    /// }
    /// </code>
    /// </remarks>
    public readonly struct BatchResult<T>
    {
        /// <summary>
        /// 배치 작업의 성공 여부를 나타냅니다.
        /// </summary>
        public readonly bool IsSuccess { get; }

        /// <summary>
        /// 배치 작업 실행 전의 원본 값입니다.
        /// </summary>
        public readonly T OldValue { get; }

        /// <summary>
        /// 배치 작업 실행 후의 결과 값입니다.
        /// 작업이 실패한 경우에도 마지막으로 시도된 값을 포함합니다.
        /// </summary>
        public readonly T NewValue { get; }

        /// <summary>
        /// 배치 작업 실행 중 발생한 예외입니다.
        /// 작업이 성공한 경우 null입니다.
        /// </summary>
        public readonly Exception Error { get; }

        /// <summary>
        /// BatchResult 구조체를 초기화합니다.
        /// </summary>
        /// <param name="isSuccess">작업 성공 여부</param>
        /// <param name="oldValue">작업 전 값</param>
        /// <param name="newValue">작업 후 값</param>
        /// <param name="error">발생한 예외 (실패 시)</param>
        public BatchResult(bool isSuccess, T oldValue, T newValue, Exception error = null)
        {
            IsSuccess = isSuccess;
            OldValue = oldValue;
            NewValue = newValue;
            Error = error;
        }

        /// <summary>
        /// 배치 작업의 결과를 문자열로 반환합니다.
        /// </summary>
        public override string ToString()
        {
            return IsSuccess
                ? $"Success: {OldValue} -> {NewValue}"
                : $"Failed: {OldValue} -> {NewValue}, Error: {Error?.Message}";
        }
    }
}
