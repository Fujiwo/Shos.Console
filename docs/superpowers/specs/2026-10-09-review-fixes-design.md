# Repository Review Fixes Design

## Scope

Address the findings from the repository-wide review in order, with focused
regression tests and no unrelated refactoring:

1. Replace the deterministic password digest with a versioned PBKDF2-SHA256
   representation and add a verification API. Existing SHA-256 strings are not
   compatible with the new representation and require migration by consumers.
2. Make `GridView` consume each input sequence once, omit indexer properties,
   tolerate a null first row when a later non-null row can provide the row
   shape, and handle an empty set of supplied columns without throwing.
3. Normalize full-width wave dash and tilde characters to ASCII `~` while
   preserving the current full-width macron mapping.
4. Remove duplicated Roslynator analyzer references and correct the README's
   package name and target-framework claims.

The existing uncommitted README edit is outside this scope and must be retained.

## Behavior and implementation

### Password hashing

`ConsoleHelper.ReadPassword` continues to read and mask console input, but
returns a self-describing PBKDF2-SHA256 string containing a format version, a
random salt, the iteration count, and the derived key. Add a public
`HashPassword` method for non-console callers and a public `VerifyPassword`
method that parses this representation and compares derived keys using a
fixed-time comparison. `ReadPassword` delegates to `HashPassword`. Invalid or
unsupported hash strings return `false`; unexpected runtime and console errors
are not swallowed. Tests must verify successful and unsuccessful comparisons,
distinct salts for repeated hashes, and malformed input handling. The API
documentation must make the storage-format change and verification flow clear.

### Grid rendering

Snapshot row data before rendering so row count, width calculation, and cell
formatting do not repeatedly enumerate caller-provided sequences. The
reflection-based overload skips indexed properties and finds the first
non-null element to determine the row type; a collection with no usable row
shape keeps the existing `false` result. The explicit-column overload returns
without output for zero columns, and snapshots each column's values before
calculating row count and rendering. Preserve current layout, frame, padding,
and short-column behavior.

Tests capture the existing writer delegates and restore them after each test.
Cover single-use sequences, null first items, indexed properties, empty input,
and ordinary framed/unframed output.

### Japanese normalization

Add explicit mappings for full-width wave dash and full-width tilde to `~`.
Keep the existing macron-to-tilde mapping unchanged. Extend tests for each
character independently and in mixed text.

### Project metadata and documentation

Keep Roslynator analyzer configuration in `Directory.Build.props` only, and
remove the duplicate references from individual projects. Correct the README
package name to `Shos.Console` and make its .NET support statement match the
library's `net8.0` target.

## Validation

Run focused MSTest coverage for the changed APIs, then run
`dotnet test Shos.Console.sln` and `dotnet build Shos.Console.sln`. Confirm that
the duplicate-package warning is gone and that the pre-existing README edit
remains intact.
