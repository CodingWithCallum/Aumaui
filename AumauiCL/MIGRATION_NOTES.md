# Database Migration Notes

## MicrosoftId Schema Change

**Date:** 2026-02-13
**Status:** Development-only — migration strategy needed before production

### What Changed

The `MicrosoftId` column on the `Users` table was modified:

| Property | Before | After |
|---|---|---|
| Nullability | `NOT NULL` (`string`) | Nullable (`string?`) |
| `[Unique]` constraint | Yes | **Removed** |
| `[Required]` annotation | Yes | **Removed** |
| Default for SHEQsys users | `"N/A"` | `null` |

### Why

- SHEQsys-only accounts don't have a Microsoft Object ID.
- The `[Unique]` constraint on `MicrosoftId` prevented more than one SHEQsys-only user from existing in the local SQLite database (all would share `MicrosoftId = "N/A"`).
- The `[Required]` annotation forced a sentinel value (`"N/A"`) which could break downstream processes expecting a valid GUID.

### Impact

- **SQLite schema mismatch** — Existing local databases created before this change have a `NOT NULL` + `UNIQUE` constraint on `MicrosoftId`. The new code writes `null` for SHEQsys users.
- **During development** — Delete the local SQLite database file and let it recreate on next launch. No data loss concern.
- **Before production** — Implement one of these strategies:
  1. **Auto-migration on startup** — Use `ALTER TABLE` to recreate the `Users` table with the new schema, copying data across.
  2. **Versioned migrations** — Introduce a `schema_version` table and run numbered migration scripts on first launch after update.
  3. **Force re-sync** — On version mismatch, drop and recreate the local database, then trigger a full sync from the API.

### Recommended Production Strategy

Option 3 (force re-sync) is simplest for a mobile app where the server is the source of truth. Implement a `schema_version` integer stored in `SecureStorage`. On startup, compare against the expected version. If mismatched, drop all tables, recreate, bump the stored version, and navigate to `/sync`.
