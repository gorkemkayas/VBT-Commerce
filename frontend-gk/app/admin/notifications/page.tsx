"use client"

import { useEffect, useState } from "react"
import * as notificationsApi from "@/lib/api/notifications"
import type { NotificationLog } from "@/lib/api/types"
import { formatDateTime } from "@/lib/format"
import { AdminPageHeader, AdminSection } from "@/components/admin/page-header"
import { Select } from "@/components/ui/select"
import { Badge } from "@/components/ui/badge"
import { Table, THead, TBody, TR, TH, TD } from "@/components/ui/table"
import { Pagination } from "@/components/ui/pagination"
import { PageSpinner } from "@/components/ui/spinner"
import { EmptyState } from "@/components/ui/empty-state"

const PAGE_SIZE = 30

export default function AdminNotificationsPage() {
  const [logs, setLogs] = useState<NotificationLog[]>([])
  const [isSuccess, setIsSuccess] = useState("")
  const [pageNumber, setPageNumber] = useState(1)
  const [totalPages, setTotalPages] = useState(1)
  const [loading, setLoading] = useState(true)

  useEffect(() => {
    let cancelled = false
    setLoading(true)
    notificationsApi
      .getNotificationLogs({ isSuccess: isSuccess === "" ? undefined : isSuccess === "true", pageNumber, pageSize: PAGE_SIZE })
      .then((result) => {
        if (cancelled) return
        setLogs(result.items)
        setTotalPages(result.totalPages || 1)
      })
      .finally(() => {
        if (!cancelled) setLoading(false)
      })
    return () => {
      cancelled = true
    }
  }, [isSuccess, pageNumber])

  return (
    <div>
      <AdminPageHeader title="Bildirim Günlüğü" description="Gönderilen e-posta/bildirim kayıtları." />
      <AdminSection>
        <div className="mb-4">
          <Select
            value={isSuccess}
            onChange={(e) => {
              setPageNumber(1)
              setIsSuccess(e.target.value)
            }}
            className="max-w-[180px]"
          >
            <option value="">Tümü</option>
            <option value="true">Başarılı</option>
            <option value="false">Başarısız</option>
          </Select>
        </div>

        {loading ? (
          <PageSpinner label="Kayıtlar yükleniyor…" />
        ) : logs.length === 0 ? (
          <EmptyState title="Kayıt bulunamadı" />
        ) : (
          <>
            <Table>
              <THead>
                <TR>
                  <TH>Tarih</TH>
                  <TH>Tip</TH>
                  <TH>Alıcı</TH>
                  <TH>Konu</TH>
                  <TH>Durum</TH>
                </TR>
              </THead>
              <TBody>
                {logs.map((log) => (
                  <TR key={log.id}>
                    <TD className="text-muted-foreground">{formatDateTime(log.createdAt)}</TD>
                    <TD>{log.notificationType}</TD>
                    <TD className="text-muted-foreground">{log.recipientEmail}</TD>
                    <TD>
                      {log.subject}
                      {log.errorMessage && <p className="mt-1 text-xs text-red-600">{log.errorMessage}</p>}
                    </TD>
                    <TD>
                      <Badge tone={log.isSuccess ? "success" : "danger"}>{log.isSuccess ? "Başarılı" : "Başarısız"}</Badge>
                    </TD>
                  </TR>
                ))}
              </TBody>
            </Table>
            <div className="mt-4">
              <Pagination pageNumber={pageNumber} totalPages={totalPages} onChange={setPageNumber} />
            </div>
          </>
        )}
      </AdminSection>
    </div>
  )
}
