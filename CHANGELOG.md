# Changelog

All notable changes to this project will be documented in this file.

## 버전 관리 정책

이 프로젝트는 Semantic Versioning을 따릅니다:

- **Major.Minor.Patch** 형식
  - **Major**: 호환성이 깨지는 변경
  - **Minor**: 하위 호환성 있는 기능 추가
  - **Patch**: 하위 호환성 있는 버그 수정
- **최신 버전이 상단에, 이전 버전이 하단에 기록됩니다.**

## [0.1.0] - 2024-12-24

### Added

- 비제네릭 Dirty 클래스 구현
  - `IDirtyObject` 인터페이스 추가
  - `Dirty` 클래스 구현
  - object 타입 기반의 유연한 값 처리
  - 이벤트 시스템 지원

- 제네릭 Dirty 클래스 구현
  - `IDirty<T>` 인터페이스 및 `DirtyBase<T>` 구현
  - 타입 안전한 값 변경 추적
  - 커스텀 비교자 지원

- 이력 관리 기능
  - `IDirtyHistory<T>` 인터페이스 및 `HistoryDirty<T>` 구현
  - 변경 이력 저장 및 관리
  - 이력 기반 되돌리기 기능

- 배치 처리 기능
  - `IDirtyBatch<T>` 인터페이스 추가
  - 배치 작업 컨텍스트 및 결과 처리
  - 이벤트 억제 옵션

- 복제 및 비교 기능
  - `IDirtyCloneable<T>` 인터페이스 추가
  - 객체 상태 복제
  - 값 비교 기능

- 통합 기능
  - `IExtendedDirty<T>` 인터페이스 및 `ExtendedDirty<T>` 구현
  - 모든 기능을 통합한 완전한 구현체
