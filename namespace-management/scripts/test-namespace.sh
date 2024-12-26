#!/bin/bash

# 설정 파일 로드
SCRIPT_DIR="$(cd "$(dirname "${BASH_SOURCE[0]}")" && pwd)"
source "$SCRIPT_DIR/config.sh"

# 디렉토리 생성
ensure_directories

# 시작 로그
log_info "$TEST_LOG_FILE" "네임스페이스 변환 테스트 시작"

# 테스트 파일 생성
cat << EOF > "$TEST_DIR/TestClass.cs"
using UnityEngine;
using ${SOURCE_NS}.Core;

namespace ${SOURCE_NS}.Test
{
    public class TestClass : MonoBehaviour
    {
        private void Start()
        {
            Debug.Log("Test");
        }
    }
}
EOF

# Assembly Definition 테스트 파일 생성
cat << EOF > "$TEST_DIR/TestAssembly.asmdef"
{
    "name": "${SOURCE_NS}.Test",
    "references": [
        "${SOURCE_NS}.Core"
    ]
}
EOF

# Git 필터 테스트
log_info "$TEST_LOG_FILE" "Git 필터 테스트 중..."

# 파일 추가 및 커밋
git add "$TEST_DIR"
git status | tee -a "$TEST_LOG_FILE"

# 파일 내용 확인
log_info "$TEST_LOG_FILE" "\n변환된 C# 파일 내용:"
cat "$TEST_DIR/TestClass.cs" | tee -a "$TEST_LOG_FILE"

log_info "$TEST_LOG_FILE" "\n변환된 Assembly Definition 파일 내용:"
cat "$TEST_DIR/TestAssembly.asmdef" | tee -a "$TEST_LOG_FILE"

# 테스트 정리
rm -rf "$TEST_DIR"
git reset --hard HEAD

# 완료 로그
log_info "$TEST_LOG_FILE" "\n네임스페이스 변환 테스트 완료"
log_info "$TEST_LOG_FILE" "로그 파일: $TEST_LOG_FILE" 