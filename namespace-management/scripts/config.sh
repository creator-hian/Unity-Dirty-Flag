#!/bin/bash

# 네임스페이스 설정
SOURCE_NS="Hian"
TARGET_NS="FAMOZ"

# 디렉토리 설정
TARGET_DIR="."
LOG_DIR="logs"

# 로그 파일 설정
LOG_FILE="${LOG_DIR}/namespace-conversion-$(date '+%Y%m%d_%H%M%S').log"
TEST_LOG_FILE="${LOG_DIR}/namespace-test-$(date '+%Y%m%d_%H%M%S').log"

# Git 필터 설정
GIT_FILTER_NAME="namespace-convert"
GIT_FILTER_CLEAN="sed -e \"s/namespace ${SOURCE_NS}/namespace ${TARGET_NS}/g\" -e \"s/using ${SOURCE_NS}/using ${TARGET_NS}/g\""
GIT_FILTER_SMUDGE="sed -e \"s/namespace ${TARGET_NS}/namespace ${SOURCE_NS}/g\" -e \"s/using ${TARGET_NS}/using ${SOURCE_NS}/g\""

# 색상 설정
RED='\033[0;31m'
GREEN='\033[0;32m'
YELLOW='\033[1;33m'
NC='\033[0m'

# 로그 함수
log_info() {
    local log_file=${1:-$LOG_FILE}
    echo -e "${GREEN}[INFO]${NC} $2" | tee -a "$log_file"
}

log_warn() {
    local log_file=${1:-$LOG_FILE}
    echo -e "${YELLOW}[WARN]${NC} $2" | tee -a "$log_file"
}

log_error() {
    local log_file=${1:-$LOG_FILE}
    echo -e "${RED}[ERROR]${NC} $2" | tee -a "$log_file"
}

# 디렉토리 생성 함수
ensure_directories() {
    mkdir -p "$LOG_DIR"
} 