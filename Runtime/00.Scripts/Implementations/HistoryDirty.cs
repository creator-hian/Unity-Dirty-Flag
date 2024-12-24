using System.Collections.Generic;
using System.Linq;

namespace Hian.DirtyFlag
{
    /// <summary>
    /// 이력 관리 기능이 포함된 Dirty 클래스입니다.
    /// 값의 변경 이력을 저장하고 관리하는 기능을 제공합니다.
    /// </summary>
    /// <typeparam name="T">관리할 값의 타입</typeparam>
    public class HistoryDirty<T> : DirtyBase<T>, IDirtyHistory<T>
    {
        protected readonly Queue<T> _history;
        protected int _maxHistoryCount;

        /// <summary>
        /// HistoryDirty 클래스를 초기화합니다.
        /// </summary>
        /// <param name="initialValue">초기값</param>
        /// <param name="comparer">값 비교를 위한 비교자</param>
        /// <param name="maxHistoryCount">저장할 최대 이력 수</param>
        public HistoryDirty(
            T initialValue = default,
            IEqualityComparer<T> comparer = null,
            int maxHistoryCount = 10
        )
            : base(initialValue, comparer)
        {
            _history = new Queue<T>();
            _maxHistoryCount = maxHistoryCount;
            _history.Enqueue(initialValue);
        }

        /// <summary>
        /// 현재 값을 설정합니다.
        /// 값이 변경되면 이력에 추가됩니다.
        /// </summary>
        public override T Value
        {
            get => base.Value;
            set
            {
                if (!_comparer.Equals(_currentValue, value))
                {
                    base.Value = value;
                    AddToHistory(value);
                }
            }
        }

        /// <summary>
        /// 저장된 모든 값의 이력을 반환합니다.
        /// </summary>
        public IReadOnlyList<T> History => _history.ToList().AsReadOnly();

        /// <summary>
        /// 이력이 존재하는지 여부를 반환합니다.
        /// </summary>
        public bool HasHistory => _history.Count > 0;

        /// <summary>
        /// 저장할 최대 이력 수를 설정하거나 가져옵니다.
        /// </summary>
        public int MaxHistoryCount
        {
            get => _maxHistoryCount;
            set
            {
                _maxHistoryCount = value;
                while (_history.Count > _maxHistoryCount)
                {
                    _history.Dequeue();
                }
            }
        }

        /// <summary>
        /// 모든 이력을 제거하고 현재 값만 남깁니다.
        /// </summary>
        public void ClearHistory()
        {
            _history.Clear();
            _history.Enqueue(_currentValue);
        }

        /// <summary>
        /// 지정된 인덱스의 이력으로 현재 값을 되돌립니다.
        /// </summary>
        public bool RevertToHistory(int historyIndex)
        {
            var historyList = _history.ToList();
            if (historyIndex >= 0 && historyIndex < historyList.Count)
            {
                // 이력을 현재 인덱스까지만 유지
                _history.Clear();
                for (int i = 0; i <= historyIndex; i++)
                {
                    _history.Enqueue(historyList[i]);
                }

                // 값 설정
                _currentValue = historyList[historyIndex];
                _isDirty = !_comparer.Equals(_currentValue, _originalValue);

                return true;
            }
            return false;
        }

        /// <summary>
        /// 값을 이력에 추가합니다.
        /// </summary>
        protected virtual void AddToHistory(T value)
        {
            _history.Enqueue(value);
            while (_history.Count > _maxHistoryCount)
            {
                _history.Dequeue();
            }
        }

        /// <summary>
        /// 값을 초기화하고 이력을 제거합니다.
        /// </summary>
        public override void Reset(T value)
        {
            base.Reset(value);
            _history.Clear();
            _history.Enqueue(value);
        }
    }
}
