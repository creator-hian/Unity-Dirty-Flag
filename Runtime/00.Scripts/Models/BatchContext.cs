using System;
using System.Collections.Generic;

namespace Hian.DirtyFlag
{
    /// <summary>
    /// 복잡한 배치 작업의 실행 컨텍스트를 제공하는 클래스입니다.
    /// 이 클래스는 여러 작업을 순차적으로 추가하고 한 번에 실행할 수 있는 기능을 제공합니다.
    /// 또한 이벤트 발생을 제어할 수 있습니다.
    /// </summary>
    /// <typeparam name="T">관리되는 값의 타입</typeparam>
    /// <remarks>
    /// 주요 기능:
    /// - 작업 순차적 추가
    /// - 이벤트 발생 제어
    /// - 일괄 실행
    /// - 작업 결과 추적
    ///
    /// 사용 예시:
    /// <code>
    /// var context = dirty.CreateBatchContext()
    ///     .Add(value => value.Step1())
    ///     .Add(value => value.Step2())
    ///     .Add(value => value.Step3())
    ///     .SuppressEvents();
    ///
    /// var result = context.Execute();
    /// if (result.IsSuccess)
    /// {
    ///     Console.WriteLine("모든 작업이 성공적으로 완료됨");
    /// }
    /// </code>
    /// </remarks>
    public class BatchContext<T>
    {
        private readonly IDirty<T> _dirty;
        private readonly List<Action<T>> _operations;
        private bool _suppressEvents;

        /// <summary>
        /// BatchContext를 초기화합니다.
        /// </summary>
        /// <param name="dirty">작업을 수행할 Dirty 객체</param>
        public BatchContext(IDirty<T> dirty)
        {
            _dirty = dirty;
            _operations = new List<Action<T>>();
            _suppressEvents = false;
        }

        /// <summary>
        /// 배치 작업을 추가합니다.
        /// 작업들은 추가된 순서대로 실행됩니다.
        /// </summary>
        /// <param name="operation">추가할 작업</param>
        /// <returns>현재 컨텍스트 (메서드 체이닝 지원)</returns>
        public BatchContext<T> Add(Action<T> operation)
        {
            _operations.Add(operation);
            return this;
        }

        /// <summary>
        /// 이벤트 발생을 억제하도록 설정합니다.
        /// 이 설정이 활성화되면 배치 작업 중 어떤 이벤트도 발생하지 않습니다.
        /// </summary>
        /// <returns>현재 컨텍스트 (메서드 체이닝 지원)</returns>
        public BatchContext<T> SuppressEvents()
        {
            _suppressEvents = true;
            return this;
        }

        /// <summary>
        /// 추가된 모든 작업을 순차적으로 실행합니다.
        /// </summary>
        /// <returns>배치 작업의 결과</returns>
        public BatchResult<T> Execute()
        {
            T oldValue = _dirty.Value;
            T currentValue = oldValue;

            try
            {
                foreach (var operation in _operations)
                {
                    operation?.Invoke(currentValue);
                }

                if (!_dirty.Comparer.Equals(oldValue, currentValue))
                {
                    if (_suppressEvents)
                    {
                        // 이벤트 발생을 억제하고 값만 변경
                        (_dirty as DirtyBase<T>)?._SetValueWithoutEvent(currentValue);
                    }
                    else
                    {
                        _dirty.Value = currentValue;
                    }
                    return new BatchResult<T>(true, oldValue, currentValue);
                }

                return new BatchResult<T>(true, oldValue, currentValue);
            }
            catch (Exception ex)
            {
                return new BatchResult<T>(false, oldValue, currentValue, ex);
            }
        }

        /// <summary>
        /// 현재 컨텍스트의 상태를 문자열로 반환합니다.
        /// </summary>
        public override string ToString()
        {
            return $"BatchContext: {_operations.Count} operations, SuppressEvents: {_suppressEvents}";
        }
    }
}
