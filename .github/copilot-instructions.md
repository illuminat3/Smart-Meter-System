# Copilot PR Review Instructions (Focused & Strict)

## Mission
Only flag **clear, objective issues**. Prioritize:
1. Breaking/compilation/test errors  
2. Spelling/grammar in user-facing strings/docs  
3. Missing/incorrect imports & dependencies  
4. Style-guide violations (objective, not taste)

Avoid subjective refactors or “nice-to-have” comments.

---

## Out of Scope (do NOT comment)
- Micro-optimizations, speculative performance claims  
- Architecture debates, design patterns “preferences”  
- Formatting that an auto-formatter/linter will fix  
- Non-actionable nitpicks (“consider… maybe…”)  

---

## Must-Check Items

### 1. Breaking / correctness
- 🚫 **Compilation errors** or obviously unreachable code.
- 🚫 **Failing or removed tests** without justification.
- 🚫 **Public API breaking changes** (renamed/removed public types/members) without a clear migration note.
- 🚫 **Null-reference/Index out of range** risks that are plainly visible.
- 🚫 **Incorrect boundary logic** (off-by-one, wrong comparison).
- 🚫 **Threading/async gotchas** (missing `await`, fire-and-forget without handling).

### 2. Spelling / grammar
- **User-facing** UI text, logs, docs, comments that ship.  
- Ignore typos in variables/internal comments unless they harm readability or violate naming rules.

### 3. Imports & dependencies
- ✅ All used symbols have **imports/usings** present; no unresolved references.
- ✅ **No unused imports** (flag only if obvious).
- ✅ New code that uses a library has a **declared dependency** (e.g., `PackageReference`, `requirements.txt`, `package.json`) and correct version range.
- ✅ Remove **dead dependencies** added in the PR but not used.
- ✅ For multi-project repos, ensure **project references** are added when types cross boundaries.

### 4. Enforce style guide (objective)
Prefer **mechanical, verifiable** rules. Examples for C# and general style below.

#### C# (targeting modern C#)
- **Primary constructors**: If a class merely stores constructor parameters as fields/properties with no extra logic, prefer a **primary constructor**.
  - ❌
    ```csharp
    public class UserService {
      private readonly ILogger<UserService> _logger;
      public UserService(ILogger<UserService> logger) {
        _logger = logger;
      }
    }
    ```
  - ✅
    ```csharp
    public class UserService(ILogger<UserService> logger)
    {
        private readonly ILogger<UserService> _logger = logger;
    }
    ```
- **Naming**:
  - **Types, public members**: `PascalCase`
  - **Locals, parameters, private fields**: `camelCase`
  - **Private readonly fields**: `_camelCase`
  - **Async methods** return `Task/Task<T>` and end with `Async`.
- **Nullability**: Enable and satisfy **nullable reference types**; avoid `!` unless justified.
- **Access modifiers**: Be explicit; minimize surface area (prefer `internal`/`private`).
- **Immutability**: Prefer `readonly` fields and `init` accessors where feasible.
- **LINQ/readability**: Favor clarity over cleverness; avoid double enumeration.
- **Using directives**: File-scoped namespaces; remove unused usings; group System first.
- **Files**: One public type per file; file name matches type.
- **Disposables**: Use `using`/`await using` for `IDisposable/IAsyncDisposable`.
- **Patterns**: Prefer pattern matching and `switch` expressions where clearer.

#### General (language-agnostic)
- **Variables**: clear, intention-revealing names; avoid single letters except for indices.
- **Functions**: small, focused; parameters ≤ ~5 unless a cohesive DTO/config object is passed.
- **Constants**: no magic numbers/strings—use named constants/enums.
- **Docs**: public APIs have brief summaries; any breaking change has migration notes.

---

## Commenting Style
Only comment when an issue is **certain** and **actionable**.  
Use this format:

**Finding**: Short statement of the issue  
**Why it matters**: Concrete impact (build break, user confusion, rule violated)  
**Suggested fix**: Minimal, explicit change (include a small patch if helpful)

### Example comments
**Finding:** Possible NRE: `user.Name.ToUpper()` when `user` can be null.  
**Why it matters:** Dereferencing a null leads to runtime failure.  
**Suggested fix:** Safely guard or use null-propagation:
```csharp
var upper = user?.Name?.ToUpperInvariant();
