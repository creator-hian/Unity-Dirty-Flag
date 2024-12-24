namespace Hian.DirtyFlag
{
    /// <summary>
    /// 모든 Dirty 기능을 통합한 확장 인터페이스입니다.
    /// 이 인터페이스는 기본 Dirty 기능, 이력 관리, 배치 처리, 복제 및 비교 기능을
    /// 모두 포함하는 완전한 기능을 제공합니다.
    /// </summary>
    /// <typeparam name="T">관리할 값의 타입</typeparam>
    /// <remarks>
    /// 주요 기능:
    /// - 기본 Dirty 기능 (값 변경 추적, 커밋, 되돌리기)
    /// - 이력 관리 (변경 이력 저장, 조회, 되돌리기)
    /// - 배치 처리 (일괄 업데이트, 작업 결과 추적)
    /// - 복제 및 비교 (객체 복제, 값 비교)
    ///
    /// 사용 예시:
    /// <code>
    /// var dirty = new ExtendedDirty&lt;MyClass&gt;();
    ///
    /// // 값 변경 및 이력 관리
    /// dirty.Value = new MyClass { Name = "First" };
    /// dirty.Value = new MyClass { Name = "Second" };
    ///
    /// // 배치 처리
    /// var result = dirty.BatchUpdate(value => {
    ///     value.Name = "Updated";
    ///     value.Process();
    /// });
    ///
    /// // 이력 확인 및 되돌리기
    /// if (dirty.HasHistory)
    /// {
    ///     dirty.RevertToHistory(0);
    /// }
    ///
    /// // 복제 및 비교
    /// var clone = dirty.Clone();
    /// bool areEqual = dirty.IsEqual(clone);
    /// </code>
    /// </remarks>
    public interface IExtendedDirty<T>
        : IDirty<T>,
            IDirtyHistory<T>,
            IDirtyBatch<T>,
            IDirtyCloneable<T>
    {
        // 이 인터페이스는 다른 인터페이스들의 기능을 통합하므로
        // 추가적인 멤버를 정의할 필요가 없습니다.
    }
}
