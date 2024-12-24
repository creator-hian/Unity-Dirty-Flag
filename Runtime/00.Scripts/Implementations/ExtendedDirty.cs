using System;

namespace Hian.DirtyFlag
{
    /// <summary>
    /// 모든 Dirty 기능이 포함된 확장 클래스입니다.
    /// 이력 관리, 배치 처리, 복제 및 비교 기능을 모두 제공합니다.
    /// </summary>
    /// <typeparam name="T">관리할 값의 타입</typeparam>
    public class ExtendedDirty<T>
        : HistoryDirty<T>,
            IExtendedDirty<T>,
            IDirtyBatch<T>,
            IDirtyCloneable<T>
    {
        /// <summary>
        /// ExtendedDirty 클래스를 초기화합니다.
        /// </summary>
        /// <param name="initialValue">초기값</param>
        /// <param name="comparer">값 비교를 위한 비교자</param>
        /// <param name="maxHistoryCount">저장할 최대 이력 수</param>
        public ExtendedDirty(
            T initialValue = default,
            IEqualityComparer<T> comparer = null,
            int maxHistoryCount = 10
        )
            : base(initialValue, comparer, maxHistoryCount) { }

        /// <summary>
        /// 값을 안전하게 설정합니다.
        /// </summary>
        public bool TrySetValue(T value)
        {
            if (!_comparer.Equals(_currentValue, value))
            {
                Value = value;
                return true;
            }
            return false;
        }

        /// <summary>
        /// 현재 상태가 Dirty인 경우에만 커밋을 수행합니다.
        /// </summary>
        public bool TryCommit()
        {
            if (_isDirty)
            {
                Commit();
                return true;
            }
            return false;
        }

        /// <summary>
        /// 배치 업데이트를 수행합니다.
        /// </summary>
        public BatchResult<T> BatchUpdate(Action<T> updateAction, bool suppressEvents = false)
        {
            T oldValue = _currentValue;

            try
            {
                if (suppressEvents)
                {
                    updateAction?.Invoke(_currentValue);
                    _SetValueWithoutEvent(_currentValue);
                }
                else
                {
                    updateAction?.Invoke(_currentValue);
                    if (!_comparer.Equals(_currentValue, Value))
                    {
                        Value = _currentValue;
                    }
                }

                return new BatchResult<T>(true, oldValue, _currentValue);
            }
            catch (Exception ex)
            {
                return new BatchResult<T>(false, oldValue, _currentValue, ex);
            }
        }

        /// <summary>
        /// 배치 작업을 위한 새로운 컨텍스트를 생성합니다.
        /// </summary>
        public BatchContext<T> CreateBatchContext()
        {
            return new BatchContext<T>(this);
        }

        /// <summary>
        /// 현재 객체의 상태를 복제합니다.
        /// </summary>
        public IDirty<T> Clone()
        {
            var clone = new ExtendedDirty<T>(_currentValue, _comparer, _maxHistoryCount);
            foreach (var historyItem in _history)
            {
                clone._history.Enqueue(historyItem);
            }
            return clone;
        }

        /// <summary>
        /// 다른 Dirty 객체와 값을 비교합니다.
        /// </summary>
        public bool IsEqual(IDirty<T> other)
        {
            return other != null && _comparer.Equals(_currentValue, other.Value);
        }

        /// <summary>
        /// 현재 상태를 문자열로 반환합니다.
        /// </summary>
        public override string ToString()
        {
            return $"Extended{base.ToString()}, HistoryCount: {_history.Count}";
        }
    }
}
