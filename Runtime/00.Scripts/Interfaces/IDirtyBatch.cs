using System;

namespace Hian.DirtyFlag
{
    /// <summary>
    /// 값의 일괄 처리 기능을 제공하는 인터페이스입니다.
    /// 이 인터페이스는 여러 값 변경을 하나의 작업으로 처리하고,
    /// 작업의 성공/실패를 추적할 수 있는 기능을 제공합니다.
    /// </summary>
    /// <typeparam name="T">관리할 값의 타입</typeparam>
    /// <remarks>
    /// 주요 기능:
    /// - 일괄 값 설정
    /// - 일괄 커밋
    /// - 배치 작업 실행
    /// - 작업 결과 추적
    ///
    /// 사용 예시:
    /// <code>
    /// var dirty = new ExtendedDirty&lt;MyClass&gt;();
    ///
    /// // 단순 배치 업데이트
    /// var result = dirty.BatchUpdate(value => {
    ///     value.Property1 = "new value";
    ///     value.Property2 = 42;
    /// });
    ///
    /// // 컨텍스트를 사용한 복잡한 배치 작업
    /// var context = dirty.CreateBatchContext()
    ///     .Add(value => value.Step1())
    ///     .Add(value => value.Step2())
    ///     .SuppressEvents();
    ///
    /// var result = context.Execute();
    /// if (result.IsSuccess)
    /// {
    ///     Console.WriteLine("배치 작업 성공");
    /// }
    /// </code>
    /// </remarks>
    public interface IDirtyBatch<T>
    {
        /// <summary>
        /// 값을 안전하게 설정합니다.
        /// 현재 값과 다른 경우에만 설정이 이루어집니다.
        /// </summary>
        /// <param name="value">설정할 새로운 값</param>
        /// <returns>값이 실제로 변경되었는지 여부</returns>
        bool TrySetValue(T value);

        /// <summary>
        /// 현재 상태가 Dirty인 경우에만 커밋을 수행합니다.
        /// </summary>
        /// <returns>커밋이 실제로 수행되었는지 여부</returns>
        bool TryCommit();

        /// <summary>
        /// 하나의 업데이트 작업을 배치로 실행합니다.
        /// </summary>
        /// <param name="updateAction">실행할 업데이트 작업</param>
        /// <param name="suppressEvents">이벤트 발생을 억제할지 여부</param>
        /// <returns>배치 작업의 결과</returns>
        BatchResult<T> BatchUpdate(Action<T> updateAction, bool suppressEvents = false);

        /// <summary>
        /// 복잡한 배치 작업을 위한 컨텍스트를 생성합니다.
        /// </summary>
        /// <returns>새로운 배치 컨텍스트</returns>
        BatchContext<T> CreateBatchContext();
    }
}
