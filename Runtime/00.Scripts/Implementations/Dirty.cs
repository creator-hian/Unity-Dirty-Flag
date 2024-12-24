using System;

namespace Hian.DirtyFlag
{
    /// <summary>
    /// 제네릭을 사용하지 않는 기본 Dirty 클래스입니다.
    /// 모든 값을 object 타입으로 처리하며, 간단한 값 변경 추적 기능을 제공합니다.
    /// </summary>
    public class Dirty : IDirtyObject
    {
        private object _originalValue;
        private object _currentValue;
        private bool _isDirty;

        public event Action<object, object> OnValueChanged;
        public event Action OnCommit;
        public event Action OnRevert;

        /// <summary>
        /// Dirty 클래스를 초기화합니다.
        /// </summary>
        /// <param name="initialValue">초기값</param>
        public Dirty(object initialValue = null)
        {
            _originalValue = initialValue;
            _currentValue = initialValue;
            _isDirty = false;
        }

        /// <summary>
        /// 현재 값을 가져오거나 설정합니다.
        /// null 값도 허용됩니다.
        /// </summary>
        public object Value
        {
            get => _currentValue;
            set
            {
                if (!Equals(_currentValue, value))
                {
                    object oldValue = _currentValue;
                    _currentValue = value;
                    _isDirty = true;
                    OnValueChanged?.Invoke(oldValue, value);
                }
            }
        }

        /// <summary>
        /// 원본 값을 가져옵니다.
        /// </summary>
        public object OriginalValue => _originalValue;

        /// <summary>
        /// 값이 변경되었는지 여부를 가져옵니다.
        /// </summary>
        public bool IsDirty => _isDirty;

        /// <summary>
        /// 현재 값을 원본 값으로 커밋합니다.
        /// </summary>
        public void Commit()
        {
            _originalValue = _currentValue;
            _isDirty = false;
            OnCommit?.Invoke();
        }

        /// <summary>
        /// 현재 값을 원본 값으로 되돌립니다.
        /// </summary>
        public void Revert()
        {
            _currentValue = _originalValue;
            _isDirty = false;
            OnRevert?.Invoke();
        }

        /// <summary>
        /// 강제로 Dirty 상태로 설정합니다.
        /// </summary>
        public void ForceSetDirty()
        {
            _isDirty = true;
        }

        /// <summary>
        /// 값을 초기화합니다.
        /// </summary>
        public void Reset(object value)
        {
            _originalValue = value;
            _currentValue = value;
            _isDirty = false;
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
