using System;
using System.Collections.Generic;

namespace Hian.DirtyFlag
{
    /// <summary>
    /// 기본 Dirty 클래스의 추상 구현체입니다.
    /// 값 변경 추적, 이벤트 처리 등의 기본 기능을 제공합니다.
    /// </summary>
    /// <typeparam name="T">관리할 값의 타입</typeparam>
    public class DirtyBase<T> : IDirty<T>
    {
        protected T _originalValue;
        protected T _currentValue;
        protected bool _isDirty;
        protected readonly IEqualityComparer<T> _comparer;

        public event Action<T, T> OnValueChanged;
        public event Action OnCommit;
        public event Action OnRevert;

        /// <summary>
        /// DirtyBase 클래스를 초기화합니다.
        /// </summary>
        /// <param name="initialValue">초기값</param>
        /// <param name="comparer">값 비교를 위한 비교자</param>
        public DirtyBase(T initialValue = default, IEqualityComparer<T> comparer = null)
        {
            _originalValue = initialValue;
            _currentValue = initialValue;
            _isDirty = false;
            _comparer = comparer ?? EqualityComparer<T>.Default;
        }

        /// <summary>
        /// 현재 값을 가져오거나 설정합니다.
        /// </summary>
        public virtual T Value
        {
            get => _currentValue;
            set
            {
                if (!_comparer.Equals(_currentValue, value))
                {
                    T oldValue = _currentValue;
                    _currentValue = value;
                    _isDirty = true;
                    OnValueChanged?.Invoke(oldValue, value);
                }
            }
        }

        /// <summary>
        /// 원본 값을 가져옵니다.
        /// </summary>
        public T OriginalValue => _originalValue;

        /// <summary>
        /// 값이 변경되었는지 여부를 가져옵니다.
        /// </summary>
        public bool IsDirty => _isDirty;

        /// <summary>
        /// 값 비교에 사용되는 비교자를 가져옵니다.
        /// </summary>
        public IEqualityComparer<T> Comparer => _comparer;

        /// <summary>
        /// 현재 값을 원본 값으로 커밋합니다.
        /// </summary>
        public virtual void Commit()
        {
            _originalValue = _currentValue;
            _isDirty = false;
            OnCommit?.Invoke();
        }

        /// <summary>
        /// 현재 값을 원본 값으로 되돌립니다.
        /// </summary>
        public virtual void Revert()
        {
            _currentValue = _originalValue;
            _isDirty = false;
            OnRevert?.Invoke();
        }

        /// <summary>
        /// 강제로 Dirty 상태로 설정합니다.
        /// </summary>
        public virtual void ForceSetDirty()
        {
            _isDirty = true;
        }

        /// <summary>
        /// 값을 초기화합니다.
        /// </summary>
        public virtual void Reset(T value)
        {
            _originalValue = value;
            _currentValue = value;
            _isDirty = false;
        }

        /// <summary>
        /// 이벤트 발생 없이 값을 설정하는 내부 메서드입니다.
        /// </summary>
        internal void _SetValueWithoutEvent(T value)
        {
            if (!_comparer.Equals(_currentValue, value))
            {
                _currentValue = value;
                _isDirty = true;
            }
        }

        /// <summary>
        /// 현재 상태를 문자열로 반환합니다.
        /// </summary>
        public override string ToString()
        {
            return $"Current: {_currentValue}, Original: {_originalValue}, IsDirty: {_isDirty}";
        }
    }
}
