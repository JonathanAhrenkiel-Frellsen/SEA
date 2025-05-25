export const parseToDate = (dateStr: string | null) =>
  dateStr ? new Date(`${dateStr}T00:00:00`) : undefined;
