# Unity Dirty Flag

값의 변경을 추적하고 관리하는 Dirty Flag 패턴을 구현한 Unity 패키지입니다.

## 요구사항

- Unity 2021.3 이상
- .NET Standard 2.1

## 개요

Dirty Flag 패턴은 객체의 상태 변경을 추적하고 관리하는 디자인 패턴입니다. 이 패키지는 다음과 같은 기능을 제공합니다:

- 값 변경 추적
- 변경 이력 관리
- 배치 작업 처리
- 상태 복제 및 비교

## 주요 기능

### 1. 기본 Dirty 클래스

```csharp
// 제네릭을 사용하지 않는 기본 버전
var dirty = new Dirty("초기값");
dirty.Value = "새로운 값";
if (dirty.IsDirty)
    dirty.Commit();

// 제네릭을 사용하는 타입 안전 버전
var typedDirty = new DirtyBase<int>(0);
typedDirty.Value = 10;
if (typedDirty.IsDirty)
    typedDirty.Commit();
```

### 2. 이력 관리 (HistoryDirty)

```csharp
var history = new HistoryDirty<string>("initial");
history.Value = "first";
history.Value = "second";
history.RevertToHistory(0); // "initial"로 되돌리기
```

### 3. 배치 처리 (ExtendedDirty)

```csharp
var extended = new ExtendedDirty<MyClass>();

// 단순 배치 업데이트
var result = extended.BatchUpdate(value => {
    value.Property1 = "new value";
    value.Property2 = 42;
});

// 복잡한 배치 작업
var context = extended.CreateBatchContext()
    .Add(value => value.Step1())
    .Add(value => value.Step2())
    .SuppressEvents();

var batchResult = context.Execute();
```

## 클래스 구조

### 인터페이스

- `IDirtyObject`: 기본 Dirty Flag 인터페이스 (비제네릭)
- `IDirty<T>`: 타입 안전한 Dirty Flag 인터페이스
- `IDirtyHistory<T>`: 이력 관리 기능
- `IDirtyBatch<T>`: 배치 처리 기능
- `IDirtyCloneable<T>`: 복제 및 비교 기능
- `IExtendedDirty<T>`: 모든 기능을 통합

### 구현 클래스

- `Dirty`: 기본 구현체 (비제네릭)
- `DirtyBase<T>`: 타입 안전한 기본 구현체
- `HistoryDirty<T>`: 이력 관리 기능 추가
- `ExtendedDirty<T>`: 모든 기능 구현

### 유틸리티 클래스

- `BatchResult<T>`: 배치 작업 결과
- `BatchContext<T>`: 배치 작업 컨텍스트

## 설치 방법

### UPM을 통한 설치 (Git URL 사용)

#### 선행 조건

- Git 클라이언트(최소 버전 2.14.0)가 설치되어 있어야 합니다.
- Windows 사용자의 경우 `PATH` 시스템 환경 변수에 Git 실행 파일 경로가 추가되어 있어야 합니다.

#### 설치 방법 1: Package Manager UI 사용

1. Unity 에디터에서 Window > Package Manager를 엽니다.
2. 좌측 상단의 + 버튼을 클릭하고 "Add package from git URL"을 선택합니다.

   ![Package Manager Add Git URL](https://i.imgur.com/1tCNo66.png)
3. 다음 URL을 입력합니다:

```text
https://github.com/creator-hian/Unity-Dirty-Flag.git
```

4. 'Add' 버튼을 클릭합니다.

   ![Package Manager Add Button](https://i.imgur.com/yIiD4tT.png)

#### 설치 방법 2: manifest.json 직접 수정

1. Unity 프로젝트의 `Packages/manifest.json` 파일을 열어 다음과 같이 dependencies 블록에 패키지를 추가하세요:

```json
{
  "dependencies": {
    "com.creator-hian.unity.dirty-flag": "https://github.com/creator-hian/Unity-Dirty-Flag.git",
    ...
  }
}
```

## 사용 예시

### 1. 기본 Dirty 클래스 사용법

#### 비제네릭 버전

```csharp
public class DynamicEntity
{
    private readonly Dirty _data = new Dirty();
    
    // 다양한 타입의 데이터 처리
    public void SetData(object value)
    {
        _data.Value = value;
        if (_data.IsDirty)
            Debug.Log($"데이터 변경됨: {_data.Value}");
    }

    public void SaveChanges()
    {
        if (_data.IsDirty)
        {
            _data.Commit();
            Debug.Log("변경사항 저장됨");
        }
    }
}

// 동적 프로퍼티 시스템 예시
public class PropertyBag
{
    private readonly Dictionary<string, Dirty> _properties = new();

    public void SetProperty(string name, object value)
    {
        if (!_properties.ContainsKey(name))
            _properties[name] = new Dirty(value);
        else
            _properties[name].Value = value;
    }

    public object GetProperty(string name)
    {
        return _properties.TryGetValue(name, out var property) 
            ? property.Value 
            : null;
    }

    public void CommitAll()
    {
        foreach (var property in _properties.Values)
        {
            if (property.IsDirty)
                property.Commit();
        }
    }
}
```

#### 제네릭 버전

```csharp
public class PlayerData
{
    private readonly DirtyBase<int> _health = new(100);
    
    public int Health
    {
        get => _health.Value;
        set => _health.Value = value;
    }

    public void SaveChanges()
    {
        if (_health.IsDirty)
            _health.Commit();
    }
}
```

### 2. 고급 사용 예시

#### 비제네릭 동적 컴포넌트

```csharp
public class DynamicComponent
{
    private readonly Dirty _transform = new Dirty(Vector3.zero);
    private readonly Dirty _state = new Dirty("Idle");
    private readonly Dirty _data = new Dirty();

    public void UpdateComponent(string property, object value)
    {
        switch (property)
        {
            case "position":
                _transform.Value = value;
                break;
            case "state":
                _state.Value = value;
                break;
            case "data":
                _data.Value = value;
                break;
        }
    }

    public void CommitChanges()
    {
        // 변경된 속성만 커밋
        if (_transform.IsDirty) _transform.Commit();
        if (_state.IsDirty) _state.Commit();
        if (_data.IsDirty) _data.Commit();
    }

    public void RevertChanges()
    {
        // 모든 변경사항 되돌리기
        _transform.Revert();
        _state.Revert();
        _data.Revert();
    }
}
```

#### 제네릭 이력 관리

```csharp
public class DocumentEditor
{
    private readonly HistoryDirty<string> _content = new("");
    
    public void UpdateContent(string newContent)
    {
        _content.Value = newContent;
    }

    public void Undo()
    {
        if (_content.HasHistory)
            _content.RevertToHistory(_content.History.Count - 2);
    }
}
```

### 3. 배치 처리 예시

#### 비제네릭 배치 처리

```csharp
public class ConfigurationManager
{
    private readonly Dictionary<string, Dirty> _settings = new();

    public void BatchUpdate(Dictionary<string, object> updates)
    {
        foreach (var (key, value) in updates)
        {
            if (!_settings.ContainsKey(key))
                _settings[key] = new Dirty(value);
            else
                _settings[key].Value = value;
        }
    }

    public void CommitChanges()
    {
        foreach (var setting in _settings.Values)
        {
            if (setting.IsDirty)
                setting.Commit();
        }
    }
}
```

#### 제네릭 배치 처리

```csharp
public class CharacterStats
{
    private readonly ExtendedDirty<Stats> _stats = new(new Stats());
    
    public void LevelUp()
    {
        _stats.BatchUpdate(stats => {
            stats.Level++;
            stats.Health += 10;
            stats.Mana += 5;
            stats.Strength += 2;
        });
    }
}
```

## 참조 문서

- [Unity 공식 매뉴얼 - Git URL을 통한 패키지 설치](https://docs.unity3d.com/kr/2023.2/Manual/upm-ui-giturl.html)

## 라이센스

MIT License

## 작성자

- [Hian](https://github.com/creator-hian)
