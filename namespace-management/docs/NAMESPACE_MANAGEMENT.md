# 브랜치별 네임스페이스 관리 가이드

## 개요

이 가이드는 GitHub(Hian)와 GitLab(FAMOZ) 간의 네임스페이스를 관리하는 방법을 설명합니다.

## 네임스페이스 관리 방식

### 1. Git 필터 방식

- **장점**
  - 실시간 자동 변환
  - 개발자 개입 최소화
  - 낮은 오버헤드

- **단점**
  - 복잡한 변환 규칙 적용 어려움
  - 디버깅이 어려움
  - Git 설정 의존성

### 2. 변환 스크립트 방식 (권장)

- **장점**
  - 유연한 변환 규칙
  - 상세한 로깅
  - 안정적인 백업
  - Assembly Definition 파일 지원
  - 테스트 자동화
  - 중앙 집중식 설정 관리

- **단점**
  - 수동 실행 필요
  - 추가 스크립트 관리 필요

### 3. CI/CD 통합 방식

- **장점**
  - 자동화된 변환
  - 지속적 검증
  - 팀 프로세스 통합

- **단점**
  - CI/CD 인프라 필요
  - 설정 복잡성
  - 즉각적인 피드백 부족

## 권장 방식: 변환 스크립트 구현

### 1. 디렉토리 구조

```
namespace-management/
├── docs/
│   └── NAMESPACE_MANAGEMENT.md
├── scripts/
│   ├── config.sh             # 공통 설정 및 유틸리티
│   ├── convert-namespace.sh  # 네임스페이스 변환
│   ├── setup-git-filter.sh   # 보조 Git 필터 설정
│   └── test-namespace.sh     # 변환 테스트
└── ci/
    └── .gitlab-ci.yml        # 선택적 CI/CD 통합
```

### 2. 스크립트 구성

#### 2.1 공통 설정 (`config.sh`)

주요 기능:
- 네임스페이스 설정 (SOURCE_NS, TARGET_NS)
- 디렉토리 경로 설정
- Git 필터 설정
- 로깅 유틸리티
- 공통 함수

```bash
# 네임스페이스 설정
SOURCE_NS="Hian"
TARGET_NS="FAMOZ"

# 디렉토리 설정
TARGET_DIR="."
LOG_DIR="logs"
BACKUP_DIR="backups/$(date '+%Y%m%d_%H%M%S')"
TEST_DIR="test_namespace"

# Git 필터 설정
GIT_FILTER_NAME="namespace-convert"
GIT_FILTER_CLEAN="sed -e \"s/namespace ${SOURCE_NS}/namespace ${TARGET_NS}/g\" -e \"s/using ${SOURCE_NS}/using ${TARGET_NS}/g\""
```

#### 2.2 네임스페이스 변환 스크립트 (`convert-namespace.sh`)

주요 기능:
- 설정 파일 로드
- 파일별 백업 생성
- Assembly Definition 파일 처리
- 상세한 에러 처리
- 진행 상황 로깅

#### 2.3 Git 보조 필터 (`setup-git-filter.sh`)

주요 기능:
- 설정 파일 로드
- Git 필터 자동 설정
- .gitattributes 자동 구성
- 설정 상태 확인

#### 2.4 테스트 스크립트 (`test-namespace.sh`)

주요 기능:
- 설정 파일 로드
- Unity 스타일의 테스트 파일 생성
- Assembly Definition 테스트
- Git 필터 테스트
- 자동 정리 기능

### 3. 작업 프로세스

#### 3.1 초기 설정 (main 브랜치)

```bash
# 1. namespace-management 폴더 생성
mkdir -p namespace-management/{scripts,docs,ci}

# 2. 스크립트 파일 생성
cp config.sh convert-namespace.sh setup-git-filter.sh test-namespace.sh namespace-management/scripts/

# 3. 스크립트 권한 설정
chmod +x namespace-management/scripts/*.sh

# 4. .gitignore 설정
cat << EOF >> .gitignore
logs/
backups/
EOF
```

#### 3.2 FAMOZ 브랜치 설정

```bash
# 1. FAMOZ 브랜치 생성
git checkout -b famoz-main

# 2. namespace-management 제거
rm -rf namespace-management

# 3. 변경사항 커밋
git add -u
git commit -m "chore: Remove namespace-management folder"
```

### 4. 파일 관리

#### 4.1 자동 생성 디렉토리
- `logs/`: 변환 로그 저장
  - `namespace-conversion-[날짜_시간].log`: 변환 로그
  - `namespace-test-[날짜_시간].log`: 테스트 로그
- `backups/[날짜_시간]/`: 파일 백업 저장
  - 원본 파일 구조 유지
  - 변환 전 상태 보존

#### 4.2 버전 관리 대상
- `scripts/`: 모든 스크립트 파일
- `docs/`: 문서 파일
- `ci/`: CI/CD 설정 파일

### 5. 문제 해결

#### 5.1 일반적인 문제

- 병합 충돌
  ```bash
  # 1. 충돌 해결
  git merge --continue
  
  # 2. 변환 스크립트 재실행
  ./scripts/convert-namespace.sh
  ```

- 네임스페이스 누락
  ```bash
  # 로그 확인
  cat logs/namespace-conversion-*.log
  ```

#### 5.2 복구 절차

```bash
# 1. 백업에서 복구
cp backups/[날짜_시간]/[파일명] [원래경로]

# 2. 변환 재시도
./scripts/convert-namespace.sh
```

### 6. 로그 시스템

#### 6.1 로그 파일 구조
- 타임스탬프 포함
- 컬러 코딩된 로그 레벨
  - `[INFO]`: 녹색
  - `[WARN]`: 노란색
  - `[ERROR]`: 빨간색
- 상세한 작업 내역

#### 6.2 로그 종류
- 변환 로그: 네임스페이스 변환 작업 기록
- 테스트 로그: 테스트 실행 결과
- Git 필터 로그: Git 필터 설정 상태

### 7. 유지보수

#### 7.1 설정 관리
- `config.sh`에서 모든 설정 중앙 관리
- 네임스페이스 변경 시 한 곳에서 수정
- Git 필터 규칙 통합 관리

#### 7.2 백업 관리
- 날짜별 백업 디렉토리 자동 생성
- 원본 파일 구조 보존
- 주기적 정리 권장 (30일 이상 백업 삭제)
