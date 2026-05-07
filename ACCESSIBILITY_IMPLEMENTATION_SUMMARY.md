# EnableVN — Frontend Accessibility Implementation Summary

## 1. Overview (goals, scope, alignment)

### Goals

- Improve **frontend accessibility engineering** across the ASP.NET Core MVC + Razor + Bootstrap UI.
- Increase practical alignment with **WCAG 2.2 Level AA** in areas that are primarily frontend-controlled: semantics, keyboard, focus, forms, announcements, reduced motion, and responsive usability.

### Scope of improvements in this iteration

- Global page shell (`_Layout`) improvements (skip link already existed; enhanced announcements/alerts).
- Global JS accessibility layer for form validation and focus management.
- Targeted workflow improvements in key screens:
  - Auth (`Login`, `Register`)
  - Candidate profile (`Create`, `Edit`)
  - Employer profile (`Create`, `Edit`, `_EmployerProfileForm`)
  - Employer jobs (`Create`, `Edit`)
  - Interview schedule (`Interview/Create`)
  - Violation report (`ViolationReports/Create`)
  - Realtime application chat (`ApplicationChat/Thread` via `application-chat.js`)

### WCAG alignment statement (do not overclaim)

- Current state is **partially aligned with WCAG 2.2 AA** for the areas implemented here.
- This is **not** a full compliance claim because:
  - Not all views and UI states were exhaustively audited with assistive technologies.
  - Automated test coverage is advisory-only (recommended), not enforced in CI here.

---

## 2. Files changed


| File                                                         | Accessibility Improvement                                                                                         | WCAG Area                                                                   |
| ------------------------------------------------------------ | ----------------------------------------------------------------------------------------------------------------- | --------------------------------------------------------------------------- |
| `ENABLEVN/Views/Shared/_Layout.cshtml`                       | Added global live regions, improved alert announcement semantics                                                  | 4.1.3 Status Messages, 1.3.1 Info and Relationships                         |
| `ENABLEVN/wwwroot/js/enablevn-accessibility.js`              | Added SR announcements, form error focus + aria-invalid/aria-describedby wiring, Bootstrap modal focus restore    | 2.4.3 Focus Order, 3.3.1 Error Identification, 4.1.3 Status Messages        |
| `ENABLEVN/Views/Auth/Login.cshtml`                           | Added per-field validation messages with live polite updates; improved validation summary announcement            | 3.3.1, 3.3.3 Error Suggestion (partial)                                     |
| `ENABLEVN/Views/Auth/Register.cshtml`                        | Added per-field validation messages + improved summary announcement                                               | 3.3.1, 4.1.3                                                                |
| `ENABLEVN/Views/CandidateProfile/Create.cshtml`              | Added validation summary; fixed file input label association                                                      | 1.3.1, 3.3.1                                                                |
| `ENABLEVN/Views/CandidateProfile/Edit.cshtml`                | Added validation summary + per-field error hook; fixed file input label association                               | 1.3.1, 3.3.1                                                                |
| `ENABLEVN/Views/EmployerProfile/Create.cshtml`               | Added validation summary announcement                                                                             | 3.3.1, 4.1.3                                                                |
| `ENABLEVN/Views/EmployerProfile/Edit.cshtml`                 | Added validation summary announcement                                                                             | 3.3.1, 4.1.3                                                                |
| `ENABLEVN/Views/EmployerProfile/_EmployerProfileForm.cshtml` | Fixed file input label association (logo upload)                                                                  | 1.3.1                                                                       |
| `ENABLEVN/Views/EmployerJobs/Create.cshtml`                  | Added validation summary + per-field validation spans for core fields                                             | 3.3.1, 4.1.3                                                                |
| `ENABLEVN/Views/EmployerJobs/Edit.cshtml`                    | Added validation summary + per-field validation spans for core fields                                             | 3.3.1, 4.1.3                                                                |
| `ENABLEVN/Views/Interview/Create.cshtml`                     | Required indicators not color-only; live validation messages; toggle uses `hidden`+`disabled` for keyboard safety | 1.3.3 Sensory Characteristics, 2.1.1 Keyboard, 3.3.2 Labels or Instructions |
| `ENABLEVN/Views/ViolationReports/Create.cshtml`              | Required indicators not color-only; validation summary + field validation message                                 | 3.3.1, 3.3.2                                                                |
| `ENABLEVN/wwwroot/js/application-chat.js`                    | Improved chat message markup (text/meta/time) to better support SR reading in `role="log"` container              | 4.1.3, 1.3.1                                                                |
| `ENABLEVN/wwwroot/css/enablevn.css`                          | Added `prefers-reduced-motion` support; improved link focus affordance                                            | 2.3.3 Animation from Interactions (support), 2.4.7 Focus Visible            |


---

## 3. Semantic HTML improvements

- **Layout landmarks**:
  - Existing: `header` + `nav`, `main#main-content`, `footer`.
  - Enhanced: global SR live regions in `_Layout` for announcements.
- **Heading hierarchy**:
  - Key workflow pages already consistently use an `h1`.
  - Targeted pages were adjusted to avoid relying on style-only markers for required fields.

---

## 4. Keyboard navigation improvements

### Skip links

- Skip link already existed; retained and ensured `main` has `tabindex="-1"` so it can reliably receive focus.

### Focus management

- Added **Bootstrap modal focus restore** so closing a modal returns focus to the element that opened it.
- In interview schedule form, conditional sections now use `hidden` + `disabled` to avoid tabbing into off-screen/inactive inputs.

### Tab order

- Improvements focus on preventing hidden fields from entering the tab sequence and ensuring the first invalid field receives focus on submit.

---

## 5. Accessible forms improvements

### Labels

- Fixed label association for file inputs (candidate avatar/CV, employer logo).

### Required fields

- Required indicator is no longer “color-only” in updated forms:
  - Added `*` marked as `aria-hidden="true"` plus a screen-reader readable “(bắt buộc)”.
  - Added `aria-required="true"` where `required` is present in updated controls.

### Validation accessibility (server + client)

- Added validation summary regions with `role="alert"` + assertive live announcements for key forms.
- Added per-field validation containers (`asp-validation-for`) on key fields so errors can be:
  - visually discoverable,
  - **linked via `aria-describedby`** by global JS,
  - announced politely by screen readers.
- Global JS annotates invalid controls with `aria-invalid="true"` and focuses the first invalid control after submit.

---

## 6. ARIA & screen reader support

### Live regions

- Global live regions in `_Layout`:
  - `#evn-sr-announcer` (`aria-live="polite"`)
  - `#evn-sr-alert` (`aria-live="assertive"`)

### Notifications and alerts

- Layout “success/error” alerts now sit inside a labeled region and are configured for announcement.

### Realtime chat

- Chat log already uses `role="log"` + `aria-live="polite"` and status uses `role="status"`.
- Updated chat message DOM structure to include explicit message text and `<time>` metadata for more predictable screen reader reading.

---

## 7. Responsive & readability improvements

- Added `prefers-reduced-motion: reduce` support to reduce animation/transition and disable smooth scrolling when requested by user settings.
- Ensured link focus has an additional cue (`text-decoration: underline`) beyond color alone on focus.

---

## 8. Accessibility workflow coverage (what is supported)

### Auth workflow (register/login)

- Keyboard-friendly, with validation summary announcements and per-field errors prepared for SR linking.

### Candidate workflow (create/edit profile)

- Form-wide validation summary announcements added.
- File inputs now have proper labels.

### Employer workflow (create/edit profile, create/edit jobs)

- Validation summary announcements added for profile creation/editing.
- Job create/edit forms now include per-field validation message containers for key fields.

### Job discovery & application workflow

- Job list/details already use semantic `header/article` patterns and descriptive controls.
- Apply action remains a standard form submit (keyboard accessible).

### Realtime communication workflow (SignalR chat)

- `role="log"` chat feed with live announcements.
- Status updates (`role="status"`) for connection and moderation outcomes.

### Interview workflow

- Create interview schedule form has improved required cues and keyboard-safe conditional fields.

### Notification workflow

- Notifications list already uses `article` + `aria-labelledby` per card; layout now supports global announcements if needed in future.

---

## 9. Accessibility testing summary (recommended process)

### Manual checklist (WCAG 2.2 AA oriented)

- **Keyboard**
  - Tab through all interactive controls (links/buttons/inputs/select/textarea).
  - Verify focus is always visible and not trapped unexpectedly.
  - Close modals via Escape; verify focus returns to trigger element.
  - For chat input: Enter sends, Shift+Enter creates a newline.
- **Forms**
  - Submit empty/invalid forms and verify:
    - focus lands on the first invalid field,
    - invalid fields have `aria-invalid="true"`,
    - errors are discoverable and announced,
    - required fields are explained in text (not color only).
- **Screen reader (NVDA/JAWS/VoiceOver)**
  - Check landmark navigation: header/nav/main/footer.
  - Verify status messages (alerts, validation summary) are announced.
  - In chat, verify new messages are announced without reading the entire page.
- **Zoom / reflow**
  - Test at 200% and 400% zoom; ensure no critical content becomes unreachable.
- **Reduced motion**
  - Enable OS “Reduce motion” and ensure the site doesn’t rely on animation for meaning.

### Tooling (optional)

- **Lighthouse Accessibility** (Chrome DevTools): run on key pages (Login, Register, Jobs Index/Details, Chat Thread).
- **axe DevTools** (browser extension): scan critical workflows for ARIA/label/contrast issues.
- **Playwright (future)**: add basic accessibility smoke checks (e.g., `page.accessibility.snapshot()`) and axe integration for regression prevention.

---

## 10. Remaining limitations (known gaps)

- Not all Razor views were fully instrumented with per-field validation spans; many fields still rely on summary-only messages.
- Contrast was improved structurally (focus cues, high-contrast theme already exists) but a full, measured contrast audit of every component/state has not been performed here.
- Notifications do not yet use SignalR-driven live announcements (only the layout has the live regions prepared).
- No CI pipeline for automated accessibility checks is included in this iteration.

---

## 11. Final evaluation (pragmatic estimate)

- **Frontend Accessibility Engineering**: **~4.5/10 → ~7.5/10** (improved form error UX, focus management, SR announcements)
- **WCAG-oriented implementation**: **basic → partially aligned with WCAG 2.2 AA**

---

## 12. Future recommendations (high impact next steps)

- Add an **Accessibility settings panel** (font size, line height, contrast toggle, reduced motion toggle, focus style toggle).
- Add a **High contrast mode** audit with measured contrast ratios for all buttons/badges/alerts.
- Add **dyslexia-friendly typography option** (font + spacing presets).
- Improve **screen reader optimization** for notifications (realtime announcement + “mark as read” feedback).
- Implement an **automated accessibility CI pipeline**:
  - Playwright + axe-core checks on critical routes
  - Lighthouse CI accessibility budgets
- Add **accessibility analytics** (non-invasive) to detect repeated validation failures / navigation pain points.

---

## Interview Schedule Lifecycle Improvements

### Files updated

- `Domain/Interviews/InterviewSchedule.cs`
- `Application/UseCases/InterviewScheduleUseCase.cs`
- `InfrastructureSqlite/Repositories/SqliteInterviewScheduleRepository.cs`
- `ENABLEVN/Views/Interview/Create.cshtml`
- `ENABLEVN/Views/Interview/MyInterviews.cshtml`
- `ENABLEVN/Views/Interview/EmployerInterviews.cshtml`

### Completed logic (automatic end-of-interview marking)

- **Entity method**: `InterviewSchedule.MarkAsCompleted()`
  - Allowed transitions: **Pending → Completed**, **Accepted → Completed**
  - Not allowed: **Declined**, **Cancelled** (and already Completed)
- **Auto update workflow (application layer)**:
  - In `GetMyInterviewsAsCandidateAsync()` and `GetMyInterviewsAsEmployerAsync()`:
    - for each schedule: if `EndsAtUtc <= DateTime.UtcNow` and status is Pending/Accepted → `CompleteIfEnded(nowUtc)` → persist via repository `UpdateAsync()`
  - In `AcceptInterviewAsync/DeclineInterviewAsync/CancelInterviewAsync()`:
    - auto-complete first (if ended) then reject the action with a user-facing message.

### Sorting changes (timeline-focused)

- Repository no longer uses `OrderByDescending(ScheduledAt)` for candidate/employer lists.
- Use case sorts with this intent:
  - Upcoming (Pending/Accepted, not ended) first, **soonest first**
  - History (Completed/Cancelled/Declined/ended) after, **most recent first**

### Datetime input improvements (minute precision)

- `Views/Interview/Create.cshtml`:
  - Uses `type="datetime-local"` with `step="60"`
  - Formats value to `yyyy-MM-ddTHH:mm` to avoid seconds/milliseconds
  - Adds help text and links it via `aria-describedby`
- Server-side normalization:
  - In `CreateInterviewScheduleAsync`, scheduled time is normalized to **UTC minute precision** (seconds/ms set to 0).

### UI behavior for status/actions

- Completed schedules remain visible as history (no deletion).
- Invalid actions are blocked:
  - Completed cannot be Accepted/Declined/Cancelled (blocked in use case).
- Status text is explicit (not color-only):
  - Pending: “Chờ phản hồi”
  - Accepted: “Đã xác nhận”
  - Declined: “Đã từ chối”
  - Cancelled: “Đã hủy”
  - Completed: “Đã kết thúc”

### Accessibility notes

- Time input has explicit label + help text (`aria-describedby`) + field-level validation region (`asp-validation-for`).
- Global form error behavior remains consistent: on submit with errors, focus moves to first invalid field and errors are announced via live regions.