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

  function workModeValue(name) {
    var n = (name || "").trim();
    if (n === "Onsite" || n === "1") return "1";
    if (n === "Hybrid" || n === "3") return "3";
    return "2";
  }

  window.evnFillJobCreateForm = function (d) {
    function setId(id, val) {
      var el = document.getElementById(id);
      if (el && val != null) el.value = val;
    }
    setId("Title", d.title || "");
    setId("Description", d.description || "");
    setId("Requirement", d.requirement || "");
    var wm = document.querySelector('select[name="WorkMode"]');
    if (wm) wm.value = workModeValue(d.workMode);
    setId("MinSalary", d.minSalary != null ? String(d.minSalary) : "");
    setId("MaxSalary", d.maxSalary != null ? String(d.maxSalary) : "");
    function chk(name, on) {
      var el = document.querySelector('input[name="' + name + '"]');
      if (el) el.checked = !!on;
    }
    chk("SupportsWheelchairAccess", d.supportsWheelchairAccess);
    chk("SupportsRemoteWork", d.supportsRemoteWork);
    chk("SupportsFlexibleTime", d.supportsFlexibleTime);
    chk("ProvidesAssistiveDevices", d.providesAssistiveDevices);
    setId("AdditionalSupportDescription", d.additionalSupportDescription || "");
  };

  window.evnParseJdFromModal = async function () {
    var ta = document.getElementById("evn-jd-paste");
    var err = document.getElementById("evn-jd-error");
    var bias = document.getElementById("evn-jd-bias");
    if (!ta) return;
    if (err) err.textContent = "";
    if (bias) bias.innerHTML = "";

    var raw = (ta.value || "").trim();
    if (raw.length < 40) {
      if (err) err.textContent = "Vui lòng dán JD dài hơn (ít nhất ~40 ký tự).";
      return;
    }

    try {
      var res = await apiFetch("/api/ai/parse-jd", { rawText: raw });
      var parsed = await readJsonOrText(res);
      var data = parsed.data || {};
      if (!res.ok) {
        if (err)
          err.textContent =
            data.error ||
            (res.status === 400
              ? "Yêu cầu không hợp lệ — thử tải lại trang."
              : "Phân tích thất bại.");
        return;
      }
      window.evnFillJobCreateForm(data);
      if (data.biasWarnings && data.biasWarnings.length && bias) {
        var ul = document.createElement("ul");
        ul.className = "small text-warning mb-0 ps-3";
        data.biasWarnings.forEach(function (w) {
          var li = document.createElement("li");
          li.textContent = w;
          ul.appendChild(li);
        });
        bias.appendChild(ul);
      }
      if (window.bootstrap) {
        var jdModal = document.getElementById("evn-jd-ai-modal");
        if (jdModal) {
          var inst =
            bootstrap.Modal.getInstance(jdModal) || new bootstrap.Modal(jdModal);
          inst.hide();
        }
      }
    } catch (e) {
      if (err) err.textContent = "Lỗi mạng hoặc máy chủ.";
    }
  };
})();
