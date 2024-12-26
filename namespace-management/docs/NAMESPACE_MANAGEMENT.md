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
│   ├── convert-namespace.sh    # 네임스페이스 변환
│   ├── setup-git-filter.sh     # 보조 Git 필터 설정
│   └── test-namespace.sh       # 변환 테스트
└── ci/
    └── .gitlab-ci.yml          # 선택적 CI/CD 통합
```

### 2. 스크립트 구성

#### 2.1 네임스페이스 변환 스크립트 (`convert-namespace.sh`)

주요 기능:
- 컬러 로깅 지원
- 파일별 백업 생성
- Assembly Definition 파일 처리
- 상세한 에러 처리
- 로그 파일 생성

#### 2.2 Git 보조 필터 (`setup-git-filter.sh`)

주요 기능:
- 선택적 Git 필터 설정
- .gitattributes 자동 구성
- 설정 상태 확인

#### 2.3 테스트 스크립트 (`test-namespace.sh`)

주요 기능:
- Unity 스타일의 테스트 파일 생성
- Assembly Definition 테스트
- Git 필터 테스트
- 자동 정리 기능

### 3. 작업 프로세스

#### 3.1 초기 설정 (main 브랜치)

```bash
# 1. namespace-management 폴더 생성
mkdir -p namespace-management/{scripts,docs,ci}

# 2. 스크립트 파일 생성 및 권한 설정
chmod +x namespace-management/scripts/*.sh

# 3. .gitignore 설정
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

#### 3.3 동기화 프로세스

```bash
# 1. main 브랜치에서 변환 실행
git checkout main
./namespace-management/scripts/convert-namespace.sh

# 2. FAMOZ 브랜치 동기화
git checkout famoz-main
git merge main
```

### 4. 파일 관리

#### 4.1 자동 생성 디렉토리
- `logs/`: 변환 로그 저장
- `backups/`: 파일 백업 저장
- 두 디렉토리 모두 .gitignore에 포함

#### 4.2 버전 관리 대상
- `scripts/`: 변환 스크립트
- `docs/`: 문서
- `ci/`: CI/CD 설정

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

### 6. 모니터링 및 유지보수

#### 6.1 로그 관리
- 위치: `logs/namespace-conversion-[날짜_시간].log`
- 내용: 변환 이력, 에러, 경고

#### 6.2 백업 관리
- 위치: `backups/[날짜_시간]/[원본경로]`
- 정책: 주기적 정리 (예: 30일 이상 백업 삭제)

### 7. 선택적 CI/CD 통합

```yaml
# .gitlab-ci.yml
stages:
  - validate
  - convert
  - test

validate_namespace:
  stage: validate
  script:
    - ./scripts/test-namespace.sh
  only:
    - famoz-main

convert_namespace:
  stage: convert
  script:
    - ./scripts/convert-namespace.sh
  only:
    - famoz-main
```
