#!/bin/bash

# 설정 파일 로드
SCRIPT_DIR="$(cd "$(dirname "${BASH_SOURCE[0]}")" && pwd)"
source "$SCRIPT_DIR/config.sh"

# 디렉토리 생성
ensure_directories

# 시작 로그
log_info "$LOG_FILE" "네임스페이스 변환 시작 (${SOURCE_NS} -> ${TARGET_NS})"

# 파일 백업 및 변환 함수
process_file() {
    local file=$1
    local relative_path=${file#./}
    local backup_path="$BACKUP_DIR/$relative_path"
    
    # 백업 디렉토리 생성
    mkdir -p "$(dirname "$backup_path")"
    
    # 파일 백업
    cp "$file" "$backup_path"
    
    # 파일 내용 검사
    if grep -q "namespace ${SOURCE_NS}" "$file" || grep -q "using ${SOURCE_NS}" "$file"; then
        log_info "$LOG_FILE" "변환 중: $file"
        
        # 네임스페이스 변환
        sed -i "s/namespace ${SOURCE_NS}/namespace ${TARGET_NS}/g" "$file"
        sed -i "s/using ${SOURCE_NS}/using ${TARGET_NS}/g" "$file"
        
        # Assembly Definition 파일 처리
        if [[ "$file" == *.asmdef ]]; then
            sed -i "s/\"${SOURCE_NS}\"/\"${TARGET_NS}\"/g" "$file"
        fi
        
        # 변환 확인
        if [ $? -eq 0 ]; then
            log_info "$LOG_FILE" "성공적으로 변환됨: $file"
        else
            log_error "$LOG_FILE" "변환 실패: $file"
        fi
    else
        log_warn "$LOG_FILE" "네임스페이스를 찾을 수 없음: $file"
    fi
}

# 파일 찾기 및 처리
find "$TARGET_DIR" \( -name "*.cs" -o -name "*.asmdef" \) -type f -print0 | while IFS= read -r -d '' file; do
    process_file "$file"
done

# 완료 로그
log_info "$LOG_FILE" "네임스페이스 변환 완료"
log_info "$LOG_FILE" "백업 위치: $BACKUP_DIR"
log_info "$LOG_FILE" "로그 파일: $LOG_FILE" 