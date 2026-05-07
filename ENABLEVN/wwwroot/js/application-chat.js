(function () {
    "use strict";

    function escapeHtml(s) {
        var d = document.createElement("div");
        d.textContent = s || "";
        return d.innerHTML;
    }

    function formatLocal(iso) {
        var d = new Date(iso);
        return isNaN(d.getTime()) ? "" : d.toLocaleString("vi-VN");
    }

    window.evnApplicationChatInit = function () {
        var root = document.querySelector("[data-evn-app-chat-root]");
        if (!root) return;

        var applicationId = root.dataset.applicationId;
        var log = document.getElementById("evn-app-chat-log");
        var input = document.getElementById("evn-app-chat-input");
        var sendBtn = document.getElementById("evn-app-chat-send");
        var statusEl = document.getElementById("evn-app-chat-status");

        var currentUserId = null;
        var ready = false;
        var sendBlockedByModeration = false;

        function setReady(v) {
            ready = v;
            sendBtn.disabled = !v;
            input.disabled = !v;
        }

        function setStatus(text) {
            statusEl.textContent = text || "";
        }

        function appendBubble(m) {
            var mine = String(m.senderUserId).toLowerCase() === String(currentUserId).toLowerCase();
            var row = document.createElement("div");
            row.className = "evn-app-chat__row " + (mine ? "evn-app-chat__row--mine" : "evn-app-chat__row--other");

            var warn = m.moderationOutcome === "warn" && m.moderationReasonVi
                ? `<span class="evn-badge evn-badge--warn">${escapeHtml(m.moderationReasonVi)}</span>`
                : "";

            row.innerHTML = `
        <div class="evn-app-chat__bubble">
          <div class="evn-app-chat__sender">${mine ? "Bạn" : "Người đối thoại"}</div>
          ${warn}
          <div class="evn-app-chat__text">${escapeHtml(m.body)}</div>
          <div class="evn-app-chat__meta">
            <time datetime="${escapeHtml(m.sentAtUtc)}">${escapeHtml(formatLocal(m.sentAtUtc))}</time>
          </div>
        </div>`;

            log.appendChild(row);
            log.scrollTop = log.scrollHeight;
        }

        setReady(false);
        setStatus("Đang tải lịch sử chat...");

        if (!window.signalR || !window.signalR.HubConnectionBuilder) {
            setStatus("Không tải được thư viện realtime. Hãy kiểm tra mạng/CDN rồi tải lại trang.");
            return;
        }

        var connection = new signalR.HubConnectionBuilder()
            .withUrl("/hubs/recruiter-chat")
            .withAutomaticReconnect([0, 2000, 5000, 10000])
            .build();

        connection.on("ReceiveMessage", appendBubble);

        connection.on("MessageBlocked", function (payload) {
            sendBlockedByModeration = true;
            setStatus((payload && payload.reasonVi) || "Tin nhắn bị chặn.");
        });

        connection.onreconnecting(function () {
            setReady(false);
            setStatus("Mất kết nối, đang thử kết nối lại...");
        });

        connection.onreconnected(function () {
            connection.invoke("JoinThread", applicationId).then(function () {
                setReady(true);
                setStatus("Đã kết nối lại.");
            });
        });

        connection.onclose(function () {
            setReady(false);
            setStatus("Chat đã ngắt kết nối. Hãy tải lại trang.");
        });

        function send() {
            if (!ready) return;

            var text = (input.value || "").trim();
            if (!text) return;

            setReady(false);
            sendBlockedByModeration = false;
            setStatus("Đang gửi...");

            connection.invoke("SendMessage", applicationId, text)
                .then(function () {
                    if (!sendBlockedByModeration) {
                        input.value = "";
                        setStatus("");
                    }
                })
                .catch(function (err) {
                    setStatus((err && err.message) || "Không gửi được tin nhắn.");
                })
                .finally(function () {
                    setReady(true);
                    input.focus();
                });
        }

        sendBtn.addEventListener("click", send);

        input.addEventListener("keydown", function (e) {
            if (e.key === "Enter" && !e.shiftKey) {
                e.preventDefault();
                send();
            }
        });

        fetch("/ApplicationChat/History?applicationId=" + encodeURIComponent(applicationId), {
            credentials: "same-origin"
        })
            .then(function (res) {
                return res.json().then(function (data) {
                    return { ok: res.ok, data: data };
                });
            })
            .then(function (x) {
                if (!x.ok) throw new Error((x.data && x.data.error) || "Không tải được lịch sử chat.");
                currentUserId = x.data.currentUserId;
                log.innerHTML = "";
                (x.data.messages || []).forEach(appendBubble);
                setStatus("Đang kết nối realtime...");
                return connection.start();
            })
            .then(function () {
                return connection.invoke("JoinThread", applicationId);
            })
            .then(function () {
                setReady(true);
                setStatus("");
            })
            .catch(function (err) {
                setReady(false);
                setStatus((err && err.message) || "Không mở được chat.");
            });
    };
})();