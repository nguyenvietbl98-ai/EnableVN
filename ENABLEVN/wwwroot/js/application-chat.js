(function () {
  "use strict";

  function pad(n) {
    return n < 10 ? "0" + n : String(n);
  }

  function formatLocal(iso) {
    if (!iso) return "";
    var d = new Date(iso);
    if (isNaN(d.getTime())) return "";
    return (
      pad(d.getDate()) +
      "/" +
      pad(d.getMonth() + 1) +
      " " +
      pad(d.getHours()) +
      ":" +
      pad(d.getMinutes())
    );
  }

  function escapeHtml(s) {
    var d = document.createElement("div");
    d.textContent = s;
    return d.innerHTML;
  }

  window.evnApplicationChatInit = function () {
    var root = document.querySelector("[data-evn-app-chat-root]");
    if (!root) return;

    var applicationId = root.getAttribute("data-application-id");
    var log = document.getElementById("evn-app-chat-log");
    var input = document.getElementById("evn-app-chat-input");
    var sendBtn = document.getElementById("evn-app-chat-send");
    var statusEl = document.getElementById("evn-app-chat-status");
    if (!applicationId || !log || !input || !sendBtn) return;

    function setStatus(t) {
      if (statusEl) statusEl.textContent = t || "";
    }

    function appendBubble(m, currentUserId) {
      var mine = m.senderUserId === currentUserId;
      var wrap = document.createElement("div");
      wrap.className =
        "evn-app-chat__row " + (mine ? "evn-app-chat__row--mine" : "evn-app-chat__row--other");
      var warn =
        m.moderationOutcome === "warn" && m.moderationReasonVi
          ? '<div class="evn-app-chat__warn small">' +
            escapeHtml(m.moderationReasonVi) +
            "</div>"
          : "";
      wrap.innerHTML =
        '<div class="evn-app-chat__bubble">' +
        warn +
        '<div class="evn-app-chat__text">' +
        escapeHtml(m.body || "") +
        '</div><div class="evn-app-chat__meta">' +
        escapeHtml(formatLocal(m.sentAtUtc)) +
        "</div></div>";
      log.appendChild(wrap);
      log.scrollTop = log.scrollHeight;
    }

    var connection = new signalR.HubConnectionBuilder()
      .withUrl("/hubs/recruiter-chat")
      .withAutomaticReconnect()
      .build();

    var currentUserId = null;

    connection.on("ReceiveMessage", function (payload) {
      appendBubble(payload, currentUserId);
    });

    connection.on("MessageBlocked", function (payload) {
      var r = (payload && payload.reasonVi) || "Tin nhắn không được gửi.";
      setStatus(r);
    });

    connection.onreconnecting(function () {
      setStatus("Đang kết nối lại…");
    });

    connection.onreconnected(function () {
      setStatus("Đã kết nối lại.");
      connection.invoke("JoinThread", applicationId).catch(function () {});
    });

    function wireSend() {
      sendBtn.addEventListener("click", function () {
        var t = (input.value || "").trim();
        if (!t) return;
        setStatus("");
        connection
          .invoke("SendMessage", applicationId, t)
          .then(function () {
            input.value = "";
          })
          .catch(function (err) {
            setStatus(
              (err && (err.message || err.toString())) || "Không gửi được tin nhắn."
            );
          });
      });
    }

    fetch(
      "/ApplicationChat/History?applicationId=" + encodeURIComponent(applicationId),
      { credentials: "same-origin" }
    )
      .then(function (res) {
        return res.json().then(function (data) {
          return { ok: res.ok, data: data };
        });
      })
      .then(function (x) {
        if (!x.ok) {
          setStatus((x.data && x.data.error) || "Không tải được lịch sử chat.");
          return;
        }
        currentUserId = x.data.currentUserId;
        log.innerHTML = "";
        (x.data.messages || []).forEach(function (m) {
          appendBubble(m, currentUserId);
        });
      })
      .catch(function () {
        setStatus("Lỗi mạng khi tải lịch sử.");
      });

    connection
      .start()
      .then(function () {
        setStatus("");
        return connection.invoke("JoinThread", applicationId);
      })
      .then(function () {
        wireSend();
      })
      .catch(function (err) {
        setStatus(
          "Không kết nối được chat thời gian thực: " +
            ((err && err.message) || err)
        );
      });
  };
})();
