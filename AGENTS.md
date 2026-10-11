# AGENTS.md

# Important Project Information
* This is a C# 7 , NET Framework 4.8 project and thus must be built using MSBuild!
* The `docs/` directory is used for business-logic documentation. Agents should consult relevant docs before making code changes and update them when behavior changes. The file name should be in the following format <business-domain-name>.md
* The `docs/changes/` directory should be used for Markdown documents that describe implemented changes. The file name should be in the following format <YYYY-MM-DD>-<breif-description>.md
* The `docs/changes_plan/` directory should be used for Markdown documents that describe **planned but not yet implemented** changes (e.g., architectural improvements, refactoring plans, bug fix proposals). File names should be descriptive of the planned change, e.g., `e2e-testability-improvements.md`. Agents should consult this directory before starting work to check if a plan already exists, and add new plan documents here when proposing significant changes.
* For narrow researched XML data-mapping corrections, such as material group/link fixes in `material_db.xml` or `main_DB.xml`, do not add new unit tests unless the user explicitly requests tests or application logic changes. Prefer direct XML inspection, existing test runs when appropriate, and a `docs/changes/` note.
* Files in INSTALL_NET4.8 directory are populated by the build script and dont need to be modified when making code changes.

