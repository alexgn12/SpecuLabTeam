export interface RequestStatusHistoryDto {
  requestId: number;
  requestDescription: string;
  oldStatusType: string;
  newStatusType: string;
  changeDate: string;
  comment: string;
}
