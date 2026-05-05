# Tài liệu chức năng Chat Realtime

## 1. Mục tiêu chức năng

Chức năng chat realtime cho phép ứng viên và nhà tuyển dụng trao đổi trực tiếp trong ngữ cảnh một hồ sơ ứng tuyển cụ thể.  
Mục tiêu chính:

- Trao đổi 2 chiều theo thời gian thực qua SignalR.
- Gắn chặt hội thoại với `applicationId` để tránh nhầm ngữ cảnh.
- Kiểm duyệt nội dung chat độc lập với các API AI khác.
- Cho phép cả Candidate và Employer quản lý hộp thư theo danh sách hội thoại.

---

## 2. Phạm vi hiện tại

Đã có các phần:

- Trang inbox hội thoại: `ApplicationChat/Inbox`.
- Trang thread chat theo hồ sơ: `ApplicationChat/Thread?applicationId=...`.
- Lấy lịch sử chat qua endpoint HTTP.
- Gửi/nhận tin realtime qua SignalR hub.
- Cơ chế kiểm duyệt (`allow`, `warn`, `block`) trước khi lưu và phát tin.

Không nằm trong phạm vi hiện tại:

- Trạng thái đã xem/chưa xem.
- Chỉ báo đang gõ (typing indicator).
- Đính kèm file/chat media.
- Thu hồi/chỉnh sửa tin nhắn.

---

## 3. Kiến trúc tổng thể (Hexagonal)

### 3.1 Domain/Application/Ports/Infrastructure/Presentation

- `Presentation`
  - `ApplicationChatController`
  - `RecruiterChatHub`
  - `wwwroot/js/application-chat.js`
  - Views: `Views/ApplicationChat/Inbox.cshtml`, `Views/ApplicationChat/Thread.cshtml`
- `Application`
  - `IJobApplicationUseCase` dùng để kiểm quyền truy cập chat theo hồ sơ.
- `Ports`
  - `IApplicationChatRepository` (outbound port lưu/đọc tin nhắn chat).
- `Infrastructure`
  - `SqliteApplicationChatRepository` triển khai lưu trữ SQLite.

Chat realtime tuân thủ Hexa:

- Controller/Hub **không truy cập DB trực tiếp**, chỉ gọi UseCase/Repository qua interface.
- Rule quyền truy cập nằm ở UseCase (`EnsureCurrentUserCanChatOnApplicationAsync`).
- Logic kiểm duyệt nằm ở service riêng (`ChatModerationService`) tách khỏi assistant AI.

---

## 4. Thành phần chính

## 4.1 HTTP Controller

`ApplicationChatController`:

- `Thread(applicationId)`:
  - kiểm quyền và trả view thread.
- `History(applicationId)`:
  - kiểm quyền chat,
  - trả về `currentUserId` + danh sách tin nhắn.
- `Inbox()`:
  - Candidate: lấy các hồ sơ của chính mình.
  - Employer: tổng hợp hồ sơ ứng tuyển vào các job thuộc doanh nghiệp mình.
  - build view model hội thoại, sort theo hoạt động mới nhất.

## 4.2 SignalR Hub

`RecruiterChatHub` (path: `/hubs/recruiter-chat`):

- `JoinThread(applicationId)`:
  - kiểm quyền user với hồ sơ,
  - add connection vào group theo `applicationId`.
- `SendMessage(applicationId, text)`:
  - validate text rỗng/độ dài,
  - kiểm session user,
  - kiểm quyền chat,
  - chạy moderation,
  - nếu block: gửi event `MessageBlocked` cho caller,
  - nếu allow/warn: lưu DB và broadcast `ReceiveMessage` cho group.

## 4.3 Frontend JS

`wwwroot/js/application-chat.js`:

- tải lịch sử bằng `fetch /ApplicationChat/History`.
- kết nối SignalR + auto reconnect.
- subscribe events:
  - `ReceiveMessage`
  - `MessageBlocked`
- gửi tin qua `connection.invoke("SendMessage", applicationId, text)`.
- có guard khi thiếu thư viện SignalR (CDN lỗi): hiển thị trạng thái lỗi rõ ràng.

---

## 5. Luồng hoạt động chi tiết

## 5.1 Mở thread chat

1. User mở `ApplicationChat/Thread`.
2. Frontend gọi API `History`.
3. Server trả lịch sử + `currentUserId`.
4. Frontend render bubble cũ.
5. Frontend start SignalR connection.
6. Frontend invoke `JoinThread(applicationId)`.
7. Khi join thành công -> bật input gửi.

## 5.2 Gửi tin nhắn

1. User bấm `Gửi` (hoặc Enter).
2. Frontend disable input tạm thời, set status `Đang gửi...`.
3. Hub nhận `SendMessage`.
4. Hub chạy moderation:
   - `block` -> không lưu DB, gửi `MessageBlocked`.
   - `warn` -> lưu DB với metadata cảnh báo.
   - `allow` -> lưu DB bình thường.
5. Hub broadcast `ReceiveMessage` đến group thread.
6. Frontend nhận event và append bubble.

## 5.3 Trường hợp bị chặn moderation

Đã xử lý lỗi UX quan trọng:

- Trước đây trạng thái bị chặn có thể bị xóa ngay khiến user tưởng bấm gửi không có phản hồi.
- Hiện tại frontend giữ lại thông báo chặn, không clear input/status trong case block.

---

## 6. Event contract realtime

## 6.1 ReceiveMessage

Server gửi object:

- `id`
- `jobApplicationId`
- `senderUserId`
- `body`
- `moderationOutcome` (`allow` | `warn`)
- `moderationReasonVi` (có thể null)
- `sentAtUtc`

## 6.2 MessageBlocked

Server gửi object:

- `reasonVi`

---

## 7. Dữ liệu lưu trữ

Bảng: `ApplicationChatMessages`

Các cột chính:

- `Id`
- `JobApplicationId`
- `SenderUserId`
- `Body` (max 4000)
- `ModerationOutcome`
- `ModerationReasonVi`
- `SentAtUtc`

Index:

- theo `JobApplicationId` để đọc lịch sử nhanh theo thread.

---

## 8. Phân quyền và bảo mật

- Chỉ user liên quan tới hồ sơ mới được chat:
  - Candidate sở hữu application.
  - Employer sở hữu job của application đó.
- Session `UserId` bắt buộc hợp lệ trước khi gửi.
- Hub lấy `HttpContext` trực tiếp từ `Context.GetHttpContext()` để ổn định trong ngữ cảnh SignalR.
- Tin nhắn bị block không được lưu DB.
- Nội dung render bubble được escape HTML ở client để tránh XSS.

---

## 9. Inbox behavior

Inbox hiển thị theo dạng conversation list:

- title job,
- preview tin nhắn gần nhất,
- thời gian hoạt động gần nhất,
- trạng thái hồ sơ ứng tuyển,
- gợi ý khi chưa bắt đầu chat.

Sort hiện tại:

1. hội thoại đã có message lên trước,
2. theo `LastActivityUtc` giảm dần,
3. tie-break theo thời điểm nộp hồ sơ.

---

## 10. Điểm đã fix gần đây

- Fix case bấm gửi nhưng không thấy gì khi bị moderation block.
- Fix lấy session trong hub để tránh lỗi phiên khi gửi.
- Thêm guard nếu SignalR CDN không load được.
- Bổ sung inbox cho Employer (trước đó chỉ candidate).
- Chuẩn hóa thứ tự hội thoại mới nhất lên đầu.

---

## 11. Test checklist đề xuất

## 11.1 Functional

- Candidate mở thread đúng quyền.
- Employer mở thread đúng quyền.
- User không thuộc thread bị chặn truy cập.
- Gửi tin `allow` -> hiển thị ngay 2 phía.
- Gửi tin `warn` -> hiển thị + badge cảnh báo.
- Gửi tin `block` -> không lưu DB, hiển thị lý do block.

## 11.2 Realtime

- Mất mạng tạm thời -> reconnect -> join lại thread.
- Gửi liên tiếp nhiều tin ngắn.
- Enter để gửi và Shift+Enter xuống dòng.

## 11.3 Inbox

- Có hội thoại mới -> nổi lên đầu.
- Thread chưa có message vẫn hiển thị fallback preview.
- Candidate/Employer đều thấy danh sách đúng dữ liệu của mình.

---

## 12. Troubleshooting nhanh

## 12.1 Bấm gửi không phản hồi

Kiểm tra:

- trạng thái kết nối ở `evn-app-chat-status`,
- có lỗi `MessageBlocked` không,
- session `UserId` còn hợp lệ không,
- SignalR script có load thành công không.

## 12.2 Không vào được chat

Nguyên nhân thường gặp:

- user không có quyền với `applicationId`,
- application đã rút hoặc không tồn tại,
- lỗi session/đăng nhập hết hạn.

## 12.3 Không thấy hội thoại trong inbox

Kiểm tra:

- role hiện tại (Candidate/Employer),
- quan hệ ownership job/application,
- dữ liệu application có bị `Withdrawn` (đặc biệt phía employer).

---

## 13. Hướng mở rộng tiếp theo

- Unread counter + read receipts.
- Tìm kiếm/filter trong inbox.
- Optimistic UI khi gửi (append tạm trước khi ack).
- Attachment (file, ảnh) với kiểm soát nội dung.
- Notification push khi có tin mới.
- Backup SignalR script local để giảm phụ thuộc CDN.

---

## 14. Tóm tắt

Chức năng chat realtime đã vận hành theo kiến trúc hiện tại, có kiểm quyền rõ ràng, kiểm duyệt nội dung độc lập, và hỗ trợ danh sách hội thoại cho cả ứng viên lẫn nhà tuyển dụng.  
Các lỗi UX trọng yếu liên quan thao tác gửi tin đã được xử lý, đồng thời có nền tảng tốt để mở rộng unread/notification và trải nghiệm chat nâng cao ở các phase tiếp theo.

