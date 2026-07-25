import { apiFetch } from "./client"
import type { NotificationLog, PagedResult } from "./types"

export function getNotificationLogs(params: { isSuccess?: boolean; pageNumber: number; pageSize: number }) {
  return apiFetch<PagedResult<NotificationLog>>("/api/admin/notifications", { query: params })
}
