# Repository Review Fixes Implementation Plan

> **For agentic workers:** REQUIRED SUB-SKILL: Use superpowers:subagent-driven-development (recommended) or superpowers:executing-plans to implement this plan task-by-task. Steps use checkbox (`- [ ]`) syntax for tracking.

**Goal:** Fix the confirmed repository-wide review findings while preserving existing table output and the pre-existing README edit.

**Architecture:** Replace password hashing behind the existing console API with a versioned PBKDF2 format and public hash/verify methods. Make `GridView` snapshot inputs before shaping output, address its reflected-property and empty-input edge cases, then update normalizer coverage and repository metadata.

**Tech Stack:** .NET 8, C#, MSTest, `dotnet test` and `dotnet build`.

---

## File map

- Modify `Shos.Console/ConsoleHelper.cs`: password hashing and verification.
- Create `Shos.ConsoleTests/ConsoleHelperTests.cs`: hashing-format and verification tests.
- Modify `Shos.Console/GridView.cs`: snapshot inputs and filter unusable reflected properties.
- Create `Shos.ConsoleTests/GridViewTests.cs`: rendering, enumeration, and edge-case tests.
- Modify `Shos.Console/JapaneseTextNormalizer.cs` and `Shos.ConsoleTests/JapaneseTextNormalizerTest.cs`: full-width dash/tilde normalization.
- Modify `Shos.Console/Shos.Console.csproj`, `Shos.Console/Shos.Console.Sample/Shos.Console.Sample.csproj`, and `Shos.ConsoleTests/Shos.ConsoleTests.csproj`: remove analyzer references duplicated in `Directory.Build.props`.
- Modify `README.md`: correct the package name and framework statement without changing the existing MVP-date edit.

### Task 1: Replacing password digests with salted PBKDF2

**Files:**
- Modify: `Shos.Console/ConsoleHelper.cs`
- Create: `Shos.ConsoleTests/ConsoleHelperTests.cs`

- [ ] **Step 1: Add tests for hashing, random salts, verification, and malformed values**

Create `Shos.ConsoleTests/ConsoleHelperTests.cs`:

```csharp
namespace Shos.Console.Tests
{
    [TestClass]
    public class ConsoleHelperTests
    {
        [TestMethod]
        public void HashPassword_UsesDifferentSaltsAndVerifiesPassword()
        {
            var first = ConsoleHelper.HashPassword("Correct!Horse9");
            var second = ConsoleHelper.HashPassword("Correct!Horse9");

            Assert.AreNotEqual(first, second);
            Assert.IsTrue(ConsoleHelper.VerifyPassword("Correct!Horse9", first));
            Assert.IsFalse(ConsoleHelper.VerifyPassword("Incorrect!Horse9", first));
            Assert.IsTrue(ConsoleHelper.VerifyPassword("Correct!Horse9", second));
        }

        [DataTestMethod]
        [DataRow("")]
        [DataRow("not-a-password-hash")]
        [DataRow("shos-pbkdf2-sha256$v2$600000$AA==$AA==")]
        [DataRow("shos-pbkdf2-sha256$v1$not-an-integer$AA==$AA==")]
        public void VerifyPassword_ReturnsFalseForInvalidHash(string hash)
            => Assert.IsFalse(ConsoleHelper.VerifyPassword("password", hash));
    }
}
```

- [ ] **Step 2: Run the focused test and verify the new API is missing**

Run: `dotnet test .\Shos.ConsoleTests\Shos.ConsoleTests.csproj --filter FullyQualifiedName~ConsoleHelperTests`

Expected: FAIL to compile because `HashPassword` and `VerifyPassword` have not yet been added.

- [ ] **Step 3: Implement versioned PBKDF2-SHA256 hashing**

Add public `HashPassword(string password)` and `VerifyPassword(string password, string passwordHash)` methods. Use 600,000 PBKDF2 iterations, a cryptographically random 16-byte salt, SHA-256, and a 32-byte derived key. Encode the format as:

```text
shos-pbkdf2-sha256$v1$600000$<base64-salt>$<base64-derived-key>
```

Have `ReadPassword` pass the entered value to `HashPassword`. Parse the exact five-field format and accept only the supported v1 iteration count; return `false` for unknown versions, invalid Base64, or incorrect salt/key lengths. Use `CryptographicOperations.FixedTimeEquals` for the derived-key comparison, and clear temporary password-byte and derived-key arrays in `finally` blocks. Update the XML documentation on `ReadPassword` to explain the new stored format and recommend `VerifyPassword`, and document both new public methods and their parameters/return values.

- [ ] **Step 4: Run the focused password tests**

Run: `dotnet test .\Shos.ConsoleTests\Shos.ConsoleTests.csproj --filter FullyQualifiedName~ConsoleHelperTests`

Expected: PASS, including unique hashes for the same password and `false` for malformed or mismatched values.

### Task 2: Snapshotting GridView input

**Files:**
- Modify: `Shos.Console/GridView.cs`
- Create: `Shos.ConsoleTests/GridViewTests.cs`

- [ ] **Step 1: Add a single-use enumerable regression test**

Create a test helper implementing `IEnumerable<T>` which returns its underlying enumerator on the first `GetEnumerator()` call and throws `InvalidOperationException` on the second. Capture output by replacing `GridView.Write`, `WriteCharacter`, `WriteLine`, and `NewLine` with delegates that append to a `StringBuilder`; restore all four delegates in `finally`.

Add tests that call both `GridView.Show` overloads with single-use sequences: one for rows and one for explicit columns. Each call must finish and include the expected cell values. This verifies that the renderer does not rely on repeated enumeration.

- [ ] **Step 2: Run the focused tests and verify they fail before the fix**

Run: `dotnet test .\Shos.ConsoleTests\Shos.ConsoleTests.csproj --filter FullyQualifiedName~GridViewTests`

Expected: FAIL with the single-use helper's second-enumeration exception.

- [ ] **Step 3: Buffer each input before calculating dimensions or rendering**

In the object overload, materialize `IEnumerable` once into a row list, derive the row count from that list, and create property values from the buffered rows. In the tuple/column overload, materialize the column definitions and each column's values once before calculating the maximum row count. Retain the existing `Column` formatting, including headers, number alignment, cell padding, and blank padding for short columns.

- [ ] **Step 4: Run the focused GridView tests**

Run: `dotnet test .\Shos.ConsoleTests\Shos.ConsoleTests.csproj --filter FullyQualifiedName~GridViewTests`

Expected: PASS; each input enumerable is requested only once and the captured output retains the existing table layout.

### Task 3: Handling GridView shape and empty-input edge cases

**Files:**
- Modify: `Shos.Console/GridView.cs`
- Modify: `Shos.ConsoleTests/GridViewTests.cs`

- [ ] **Step 1: Add tests for null first rows, indexers, all-null rows, and no columns**

Add a reference-type row with one ordinary property and a public indexer. Test a row sequence whose first element is `null` and second element is non-null; rendering must include the ordinary property without throwing. The indexer must not be included or invoked. Test an all-null sequence and assert the object overload returns `false`. Test an empty explicit-column sequence and assert it emits no output and throws no exception.

- [ ] **Step 2: Run the focused tests and verify the pre-fix failures**

Run: `dotnet test .\Shos.ConsoleTests\Shos.ConsoleTests.csproj --filter FullyQualifiedName~GridViewTests`

Expected: FAIL on the null row or indexer case and on empty explicit columns.

- [ ] **Step 3: Filter reflected properties and guard empty inputs**

Use the first non-null buffered element to discover properties. Exclude properties where `GetIndexParameters().Length != 0`, return `false` when there is no non-null element or no renderable properties, and emit no output when the explicit-column list is empty. For a null row in a mixed sequence, yield `null` for each reflected property instead of calling `GetValue(null)`.

- [ ] **Step 4: Run the GridView regression tests**

Run: `dotnet test .\Shos.ConsoleTests\Shos.ConsoleTests.csproj --filter FullyQualifiedName~GridViewTests`

Expected: PASS for all GridView tests, including the cases added in Task 2.

### Task 4: Normalizing wave dash and full-width tilde

**Files:**
- Modify: `Shos.Console/JapaneseTextNormalizer.cs`
- Modify: `Shos.ConsoleTests/JapaneseTextNormalizerTest.cs`

- [ ] **Step 1: Add mapping tests**

Add assertions that `～`.Apply() and `〜`.Apply() each produce `"~"`, that `￣`.Apply() still produces `"~"`, and that mixed text normalizes without changing surrounding characters.

- [ ] **Step 2: Run the normalizer test and verify the new cases fail**

Run: `dotnet test .\Shos.ConsoleTests\Shos.ConsoleTests.csproj --filter FullyQualifiedName~JapaneseTextNormalizerTest`

Expected: FAIL for full-width wave dash/tilde before their mappings are added.

- [ ] **Step 3: Add explicit mappings without changing the existing macron mapping**

Add `～` and `〜` entries to `characterTable`, each mapped to ASCII `~`. Keep the current `￣` entry and its existing result unchanged.

- [ ] **Step 4: Run the normalizer tests**

Run: `dotnet test .\Shos.ConsoleTests\Shos.ConsoleTests.csproj --filter FullyQualifiedName~JapaneseTextNormalizerTest`

Expected: PASS, including existing ASCII and half-width kana coverage.

### Task 5: Removing duplicate analyzer references and correcting README claims

**Files:**
- Modify: `Shos.Console/Shos.Console.csproj`
- Modify: `Shos.Console.Sample/Shos.Console.Sample.csproj`
- Modify: `Shos.ConsoleTests/Shos.ConsoleTests.csproj`
- Modify: `README.md`

- [ ] **Step 1: Remove the three project-local Roslynator references**

Delete only each project's `PackageReference` for `Roslynator.Analyzers`; retain the shared configuration and package reference in `Directory.Build.props`.

- [ ] **Step 2: Correct the README package and framework statements**

Change `Shos.CsvHelper` to `Shos.Console` and state that the library targets .NET 8. Preserve the pre-existing MVP award date edit in the README.

- [ ] **Step 3: Build and inspect restore warnings**

Run: `dotnet build .\Shos.Console.sln`

Expected: Build succeeds with no `NU1504` duplicate-package warning. Other unrelated analyzer warnings should be reported but not broaden this change.

### Task 6: Full regression verification

**Files:**
- Verify all changed files listed above.

- [ ] **Step 1: Run all tests**

Run: `dotnet test .\Shos.Console.sln`

Expected: All tests pass.

- [ ] **Step 2: Build all solution projects**

Run: `dotnet build .\Shos.Console.sln --no-restore`

Expected: Build succeeds with zero errors.

- [ ] **Step 3: Verify the final worktree diff**

Run: `git diff --check; git status --short; git diff --stat`

Expected: No whitespace errors; changes are limited to the password API, GridView fixes/tests, normalizer mapping/tests, analyzer references, and README corrections. The user's pre-existing README date change remains present.
