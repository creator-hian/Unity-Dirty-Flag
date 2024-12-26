#!/bin/bash

# 설정 파일 로드
SCRIPT_DIR="$(cd "$(dirname "${BASH_SOURCE[0]}")" && pwd)"
source "$SCRIPT_DIR/config.sh"

# 디렉토리 생성
ensure_directories

# Git 필터 설정
log_info "$LOG_FILE" "Git 필터 설정 시작"

# clean 필터 설정 (체크인 시)
git config --local filter.${GIT_FILTER_NAME}.clean "$GIT_FILTER_CLEAN"

# smudge 필터 설정 (체크아웃 시)
git config --local filter.${GIT_FILTER_NAME}.smudge "$GIT_FILTER_SMUDGE"

# .gitattributes 파일 생성/수정
cat << EOF > .gitattributes
*.cs filter=${GIT_FILTER_NAME}
*.asmdef filter=${GIT_FILTER_NAME}
EOF

log_info "$LOG_FILE" "Git 필터 설정 완료"
log_info "$LOG_FILE" ".gitattributes 파일이 생성/수정되었습니다"

# 설정 확인
log_info "$LOG_FILE" "\n현재 Git 필터 설정:"
git config --local --get-regexp filter.${GIT_FILTER_NAME} 