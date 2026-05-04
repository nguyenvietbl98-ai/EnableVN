(function () {
  "use strict";

  function csrf() {
    var m = document.querySelector('meta[name="csrf-token"]');
    return m ? m.getAttribute("content") || "" : "";
  }

  async function readJsonOrText(res) {
    var text = await res.text();
    try {
      return { ok: res.ok, data: JSON.parse(text), raw: text };
    } catch (e) {
      return {
        ok: res.ok,
        data: {
          error: text
            ? text.slice(0, 400)
            : "Phản hồi không phải JSON (" + res.status + ").",
        },
        raw: text,
      };
    }
  }

  function apiFetch(url, body) {
    return fetch(url, {
      method: "POST",
      credentials: "same-origin",
      headers: {
        "Content-Type": "application/json",
        "X-XSRF-TOKEN": csrf(),
      },
      body: JSON.stringify(body),
    });
  }

  function appendBubble(container, text, kind) {
    var wrap = document.createElement("div");
    wrap.className =
      "evn-ai-bubble " +
      (kind === "user" ? "evn-ai-bubble--user" : "evn-ai-bubble--assistant");
    var inner = document.createElement("div");
    inner.className = "evn-ai-bubble__inner";
    inner.textContent = text;
    wrap.appendChild(inner);
    container.appendChild(wrap);
    container.scrollTop = container.scrollHeight;
  }

  function clearSuggestions(box) {
    box.innerHTML = "";
  }

  function renderSuggestions(box, matches) {
    clearSuggestions(box);
    if (!matches || !matches.length) return;
    matches.forEach(function (m) {
      var a = document.createElement("a");
      a.className =
        "btn btn-outline-primary text-start evn-ai-suggest-btn py-2 px-3";
      a.href = m.detailUrl || "#";
      a.target = "_self";
      a.rel = "noopener";
      a.innerHTML =
        "<span class=\"fw-bold d-block\">" +
        escapeHtml(m.title || "") +
        "</span><span class=\"small evn-muted\">" +
        escapeHtml(m.reason || "") +
        "</span>";
      box.appendChild(a);
    });
  }

  function escapeHtml(s) {
    return String(s)
      .replace(/&/g, "&amp;")
      .replace(/</g, "&lt;")
      .replace(/>/g, "&gt;")
      .replace(/"/g, "&quot;");
  }

  window.evnAiAssistantPageInit = function () {
    var root = document.querySelector(".evn-ai-page[data-evn-ai-role]");
    var log = document.getElementById("evn-ai-chat-log");
    var sug = document.getElementById("evn-ai-suggestions");
    var form = document.getElementById("evn-ai-chat-form");
    var input = document.getElementById("evn-ai-chat-input");
    var hint = document.getElementById("evn-ai-chat-hint");
    if (!root || !log || !sug || !form || !input || !hint) return;

    var role = root.getAttribute("data-evn-ai-role") || "";
    if (role === "Candidate") {
      hint.textContent =
        "Mô tả kỹ năng, kinh nghiệm hoặc nhu cầu làm việc — AI gợi ý tin đang tuyển phù hợp.";
    } else if (role === "Employer") {
      hint.textContent =
        "Mô tả ứng viên bạn cần — AI gợi ý hồ sơ công khai trên EnableVN.";
    }

    form.addEventListener("submit", async function (e) {
      e.preventDefault();
      var msg = (input.value || "").trim();
      if (!msg) return;

      appendBubble(log, msg, "user");
      input.value = "";
      clearSuggestions(sug);

      var url =
        role === "Employer"
          ? "/api/ai/employer-chat"
          : "/api/ai/candidate-chat";

      try {
        var res = await apiFetch(url, { message: msg });
        var parsed = await readJsonOrText(res);
        var data = parsed.data || {};
        if (!res.ok) {
          var err =
            data.error ||
            (res.status === 400
              ? "Yêu cầu không hợp lệ (400). Thử tải lại trang nếu phiên làm việc cũ."
              : "Lỗi " + res.status + ".");
          appendBubble(log, "Lỗi: " + err, "assistant");
          return;
        }
        appendBubble(
          log,
          data.assistantMessage || "(Không có phản hồi văn bản)",
          "assistant"
        );
        renderSuggestions(sug, data.matches || []);
      } catch (err) {
        appendBubble(log, "Lỗi mạng hoặc máy chủ.", "assistant");
      }
    });
  };
})();
