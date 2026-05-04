(function () {
  const STORAGE_KEY = "evn.theme";

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

  document.addEventListener("click", (e) => {
    const btn = e.target.closest?.("[data-evn-contrast-toggle]");
    if (!btn) return;

    const isContrast = document.documentElement.getAttribute("data-theme") === "contrast";
    const next = isContrast ? null : "contrast";
    storeTheme(next);
    applyTheme(next);
  });
})();

