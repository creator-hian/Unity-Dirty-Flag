#!/bin/bash

# 설정 파일 로드
SCRIPT_DIR="$(cd "$(dirname "${BASH_SOURCE[0]}")" && pwd)"
source "$SCRIPT_DIR/config.sh"

# 디렉토리 생성
ensure_directories

# 시작 로그
log_info "$LOG_FILE" "네임스페이스 변환 시작 (${SOURCE_NS} -> ${TARGET_NS})"

# GUID 매핑을 저장할 연관 배열
declare -A guid_mapping

# GUID 매핑 초기화 함수
initialize_guid_mapping() {
    log_info "$LOG_FILE" "Assembly Definition GUID 매핑 초기화 중..."
    
    # 모든 .asmdef.meta 파일을 찾아서 GUID 매핑 생성
    while IFS= read -r -d '' meta_file; do
        local asmdef_file="${meta_file%.meta}"
        if [ -f "$asmdef_file" ]; then
            local old_guid=$(grep "guid: " "$meta_file" | awk '{print $2}')
            local assembly_name=$(grep "\"name\":" "$asmdef_file" | sed 's/.*"name":\s*"\([^"]*\)".*/\1/')
            if [ -n "$old_guid" ] && [ -n "$assembly_name" ]; then
                guid_mapping["$assembly_name"]="$old_guid"
                log_info "$LOG_FILE" "GUID 매핑 추가: $assembly_name -> $old_guid"
            fi
        fi
    done < <(find "$TARGET_DIR" -name "*.asmdef.meta" -type f -print0)
}

# 네임스페이스 변환 함수
convert_namespace() {
    local content=$1
    local is_test=$2
    
    # Assembly Definition 필드 변환 (name과 rootNamespace)
    content=$(echo "$content" | sed -E "s/\"name\":\s*\"${SOURCE_NS}([^\"]*)\"/\"name\": \"${TARGET_NS}\1\"/g")
    content=$(echo "$content" | sed -E "s/\"rootNamespace\":\s*\"${SOURCE_NS}([^\"]*)\"/\"rootNamespace\": \"${TARGET_NS}\1\"/g")
    
    # 기본 네임스페이스 변환
    content=$(echo "$content" | sed "s/namespace ${SOURCE_NS}/namespace ${TARGET_NS}/g")
    content=$(echo "$content" | sed "s/using ${SOURCE_NS}/using ${TARGET_NS}/g")
    
    # 테스트 특화 패턴 변환
    if [ "$is_test" = true ]; then
        content=$(echo "$content" | sed "s/${SOURCE_NS}\.Test/${TARGET_NS}.Test/g")
        content=$(echo "$content" | sed "s/${SOURCE_NS}\.Tests/${TARGET_NS}.Tests/g")
        content=$(echo "$content" | sed "s/${SOURCE_NS}\.Editor\.Tests/${TARGET_NS}.Editor.Tests/g")
    fi
    
    # Assembly Definition 참조 변환
    content=$(echo "$content" | sed -E "s/\"${SOURCE_NS}([^\"]*)\"/\"${TARGET_NS}\1\"/g")
    
    echo "$content"
}

# 파일 처리 함수
process_file() {
    local file=$1
    local is_test=false
    
    # 테스트 파일 여부 확인
    if [[ "$file" =~ /Tests?/ ]] || [[ "$file" =~ Test[s]?\. ]]; then
        is_test=true
    fi
    
    if [[ "$file" == *.asmdef ]]; then
        log_info "$LOG_FILE" "Assembly Definition 파일 변환 중: $file"
        
        # 파재 Assembly Definition의 이름과 GUID 저장
        local current_name=$(grep "\"name\":" "$file" | sed 's/.*"name":\s*"\([^"]*\)".*/\1/')
        local meta_file="${file}.meta"
        local current_guid=""
        if [ -f "$meta_file" ]; then
            current_guid=$(grep "guid: " "$meta_file" | awk '{print $2}')
        fi
        
        # 파일 이름 변경 (필요한 경우)
        local dir=$(dirname "$file")
        local filename=$(basename "$file" .asmdef)
        local new_filename="${filename//$SOURCE_NS/$TARGET_NS}"
        local new_file="$dir/$new_filename.asmdef"
        
        # Assembly Definition 내용 읽기 및 변환
        local content=$(<"$file")
        content=$(convert_namespace "$content" "$is_test")
        
        # 변환된 내용 저장
        echo "$content" > "$file"
        
        # 파일 이름 변경이 필요한 경우
        if [ "$filename" != "$new_filename" ]; then
            mv "$file" "$new_file"
            if [ -f "$meta_file" ]; then
                mv "$meta_file" "${new_file}.meta"
            fi
            log_info "$LOG_FILE" "파일 이름 변경됨: $filename.asmdef -> $new_filename.asmdef"
            file="$new_file"
        fi
        
        # GUID 매핑 업데이트
        if [ -n "$current_guid" ] && [ -n "$current_name" ]; then
            local new_name="${current_name//$SOURCE_NS/$TARGET_NS}"
            guid_mapping["$new_name"]="$current_guid"
            log_info "$LOG_FILE" "GUID 매핑 업데이트: $new_name -> $current_guid"
        fi
        
    elif [[ "$file" == *.asmdef.meta ]]; then
        # .meta 파일은 이미 처리됨
        return
        
    elif [[ "$file" == *.cs ]]; then
        # C# 파일 처리
        if grep -q "${SOURCE_NS}" "$file"; then
            log_info "$LOG_FILE" "변환 중: $file"
            
            # 파일 내용 읽기 및 변환
            local content=$(<"$file")
            content=$(convert_namespace "$content" "$is_test")
            
            # 변환된 내용 저장
            echo "$content" > "$file"
            
            # 변환 확인
            if [ $? -eq 0 ]; then
                log_info "$LOG_FILE" "성공적으로 변환됨: $file"
            else
                log_error "$LOG_FILE" "변환 실패: $file"
            fi
        else
            log_warn "$LOG_FILE" "네임스페이스를 찾을 수 없음: $file"
        fi
    fi
}

# GUID 매핑 초기화
initialize_guid_mapping

# 파일 찾기 및 처리
find "$TARGET_DIR" \( -name "*.cs" -o -name "*.asmdef" -o -name "*.asmdef.meta" \) -type f -print0 | while IFS= read -r -d '' file; do
    process_file "$file"
done

# GUID 참조 업데이트 함수
update_asmdef_references() {
    log_info "$LOG_FILE" "Assembly Definition 참조 업데이트 중..."
    
    while IFS= read -r -d '' file; do
        if grep -q "\"references\":" "$file"; then
            local temp_file=$(mktemp)
            local updated=false
            local is_test=false
            
            if [[ "$file" =~ /Tests?/ ]] || [[ "$file" =~ Test[s]?\. ]]; then
                is_test=true
            fi
            
            while IFS= read -r line; do
                if [[ $line =~ \"references\": ]]; then
                    echo "$line" >> "$temp_file"
                    # references 배열 처리 시작
                    while IFS= read -r ref_line; do
                        if [[ $ref_line =~ [\]}] ]]; then
                            echo "$ref_line" >> "$temp_file"
                            break
                        fi
                        
                        # 참조 이름에서 GUID 찾기
                        local ref_name=$(echo "$ref_line" | sed 's/.*"\([^"]*\)".*/\1/')
                        local new_ref_name="${ref_name//$SOURCE_NS/$TARGET_NS}"
                        
                        # 테스트 특화 패턴 처리
                        if [ "$is_test" = true ]; then
                            new_ref_name="${new_ref_name//.Test/.Test}"
                            new_ref_name="${new_ref_name//.Tests/.Tests}"
                            new_ref_name="${new_ref_name//.Editor.Tests/.Editor.Tests}"
                        fi
                        
                        if [ -n "${guid_mapping[$new_ref_name]}" ]; then
                            local new_line="${ref_line//$ref_name/$new_ref_name}"
                            echo "$new_line" >> "$temp_file"
                            updated=true
                            log_info "$LOG_FILE" "참조 업데이트: $ref_name -> $new_ref_name"
                        else
                            echo "$ref_line" >> "$temp_file"
                        fi
                    done
                else
                    echo "$line" >> "$temp_file"
                fi
            done < "$file"
            
            if [ "$updated" = true ]; then
                mv "$temp_file" "$file"
                log_info "$LOG_FILE" "Assembly Definition 참조 업데이트됨: $file"
            else
                rm "$temp_file"
            fi
        fi
    done < <(find "$TARGET_DIR" -name "*.asmdef" -type f -print0)
}

# 참조 업데이트
update_asmdef_references

# 완료 로그
log_info "$LOG_FILE" "네임스페이스 변환 완료"
log_info "$LOG_FILE" "로그 파일: $LOG_FILE" 