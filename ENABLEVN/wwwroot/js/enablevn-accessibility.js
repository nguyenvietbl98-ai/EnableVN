(function () {
  const STORAGE_KEY = "evn.theme";
  const ANNOUNCER_ID = "evn-sr-announcer";
  const ALERT_ID = "evn-sr-alert";

  function applyTheme(theme) {
    if (theme === "contrast") {
      document.documentElement.setAttribute("data-theme", "contrast");
    } else {
      document.documentElement.removeAttribute("data-theme");
    }

    const pressed = theme === "contrast";
    document.querySelectorAll("[data-evn-contrast-toggle]").forEach((btn) => {
      btn.setAttribute("aria-pressed", pressed ? "true" : "false");
    });
  }

  function getStoredTheme() {
    try {
      return localStorage.getItem(STORAGE_KEY);
    } catch {
      return null;
    }
  }

  function storeTheme(theme) {
    try {
      if (!theme) localStorage.removeItem(STORAGE_KEY);
      else localStorage.setItem(STORAGE_KEY, theme);
    } catch {
      // ignore
    }
  }

  const initialTheme = getStoredTheme();
  applyTheme(initialTheme);

  function getLiveRegion(id) {
    return document.getElementById(id);
  }

  function announce(text, mode) {
    const msg = String(text || "").trim();
    if (!msg) return;
    const region = getLiveRegion(mode === "assertive" ? ALERT_ID : ANNOUNCER_ID);
    if (!region) return;
    // Clear first to force SR re-announce even if same text.
    region.textContent = "";
    window.setTimeout(() => {
      region.textContent = msg;
    }, 10);
  }

  function focusFirstInvalid(form) {
    const el =
      form.querySelector("[aria-invalid='true']") ||
      form.querySelector(".input-validation-error") ||
      form.querySelector(":invalid");
    if (el && typeof el.focus === "function") el.focus();
  }

  function ensureFieldDescribedBy(input, messageId) {
    const existing = (input.getAttribute("aria-describedby") || "")
      .split(/\s+/)
      .filter(Boolean);
    if (!existing.includes(messageId)) {
      existing.push(messageId);
      input.setAttribute("aria-describedby", existing.join(" "));
    }
  }

  function wireUpAspNetValidationA11y(root) {
    const scope = root || document;
    const forms = scope.querySelectorAll("form");

    forms.forEach((form) => {
      // On submit, let MVC/HTML validation run; then annotate.
      form.addEventListener(
        "submit",
        () => {
          // Give client-side validation a tick to update DOM classes/messages.
          window.setTimeout(() => {
            const invalidInputs = form.querySelectorAll(".input-validation-error, :invalid");

            invalidInputs.forEach((input) => {
              if (!(input instanceof HTMLElement)) return;
              input.setAttribute("aria-invalid", "true");

              const name = input.getAttribute("name");
              if (!name) return;

              // ASP.NET MVC: <span data-valmsg-for="Field">...</span>
              const msg = form.querySelector(`[data-valmsg-for="${CSS.escape(name)}"]`);
              if (msg && msg.id) {
                ensureFieldDescribedBy(input, msg.id);
              } else if (msg && !msg.id) {
                msg.id = `evn-valmsg-${name.replace(/[^a-zA-Z0-9_-]/g, "_")}`;
                ensureFieldDescribedBy(input, msg.id);
              }
            });

            // Clear aria-invalid if field is now valid
            form.querySelectorAll("[aria-invalid='true']").forEach((input) => {
              if (!(input instanceof HTMLElement)) return;
              const isInvalid =
                input.classList.contains("input-validation-error") ||
                (input instanceof HTMLInputElement || input instanceof HTMLTextAreaElement || input instanceof HTMLSelectElement
                  ? !input.checkValidity()
                  : false);
              if (!isInvalid) input.removeAttribute("aria-invalid");
            });

            if (invalidInputs.length > 0) {
              announce("Có lỗi trong biểu mẫu. Vui lòng kiểm tra các trường được đánh dấu.", "assertive");
              focusFirstInvalid(form);
            }
          }, 0);
        },
        true
      );

      // As user edits, remove aria-invalid on valid fields for SR clarity.
      form.addEventListener(
        "input",
        (e) => {
          const t = e.target;
          if (!(t instanceof HTMLInputElement || t instanceof HTMLTextAreaElement || t instanceof HTMLSelectElement)) return;
          if (t.classList.contains("input-validation-error")) return;
          if (t.checkValidity && t.checkValidity()) t.removeAttribute("aria-invalid");
        },
        true
      );
    });
  }

  function wireUpBootstrapModalFocusRestore() {
    // Restore focus to the trigger when modal closes (helps keyboard users).
    document.addEventListener("show.bs.modal", (e) => {
      const modal = e.target;
      if (!(modal instanceof HTMLElement)) return;
      const active = document.activeElement;
      if (active && active instanceof HTMLElement) {
        modal.dataset.evnRestoreFocusId = "";
        if (!active.id) active.id = `evn-focus-${Math.random().toString(36).slice(2)}`;
        modal.dataset.evnRestoreFocusId = active.id;
      }
    });

    document.addEventListener("hidden.bs.modal", (e) => {
      const modal = e.target;
      if (!(modal instanceof HTMLElement)) return;
      const id = modal.dataset.evnRestoreFocusId;
      if (!id) return;
      const el = document.getElementById(id);
      if (el && typeof el.focus === "function") el.focus();
    });
  }

  function wireUpDropdownEscapeClose() {
    document.addEventListener("keydown", (e) => {
      if (e.key !== "Escape") return;
      const openMenu = document.querySelector(".dropdown-menu.show");
      if (!openMenu) return;

      const dropdownRoot = openMenu.closest(".dropdown");
      const toggle = dropdownRoot ? dropdownRoot.querySelector('[data-bs-toggle="dropdown"]') : null;
      if (!toggle) return;

      // Close via Bootstrap API to keep aria-expanded correct.
      const Dropdown = window.bootstrap && window.bootstrap.Dropdown;
      if (Dropdown) {
        const inst = Dropdown.getOrCreateInstance(toggle);
        inst.hide();
      } else {
        // Fallback: click to toggle closed.
        toggle.click();
      }

      if (typeof toggle.focus === "function") toggle.focus();
    });
  }

  document.addEventListener("DOMContentLoaded", () => {
    wireUpAspNetValidationA11y(document);
    wireUpBootstrapModalFocusRestore();
    wireUpDropdownEscapeClose();
  });

  document.addEventListener("click", (e) => {
    const btn = e.target.closest?.("[data-evn-contrast-toggle]");
    if (!btn) return;

    const isContrast = document.documentElement.getAttribute("data-theme") === "contrast";
    const next = isContrast ? null : "contrast";
    storeTheme(next);
    applyTheme(next);
    announce(next ? "Đã bật chế độ tương phản cao." : "Đã tắt chế độ tương phản cao.", "polite");
  });
})();

